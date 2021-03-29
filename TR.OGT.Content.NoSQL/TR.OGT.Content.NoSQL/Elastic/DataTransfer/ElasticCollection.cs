using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    /// <summary>
    /// Use this class as a wrapper around a list of your items, so that the list can be uploaded to ES with ProcessStuff()
    /// </summary>
    public class ElasticCollection<T> where T : ElasticItem
    {
        public List<T> Items;
        public ElasticConfig Settings;

        /// <summary>
        /// If you are going to UploadItems, you need to call this.Settings = [some ProcessorConfig] but I'm leaving that up to the calling app.
        /// </summary>
        public ElasticCollection()
        {
            Items = new List<T>();
        }

        /// <summary>
        /// Serialize sub items so you can include them in a larger collection.
        /// </summary>
        /// <returns>Serialized ndjson with the collection elements</returns>
        public String SerializeListItems()
        {
            return Serialize.ElasticCollection(Items);
        }

        /// <summary>
        /// If this is a subcollection, this is *not* what you want to do.
        /// </summary>
        public Boolean UploadItems()
        {
            if (this.Settings == null) { throw new Exception("You didn't set the ProcessorConfig for this collection before trying to load."); }
            if (Settings.DebugMode) { Settings.Helpers.WriteLog("-- " + Settings.AppName + " Started --"); }

            if (!String.IsNullOrEmpty(Settings.SerializedOutput))
            {
                Settings.OutputDirectory = Path.Combine(Settings.SerializedOutput, Settings.RunID);
                Directory.CreateDirectory(Settings.OutputDirectory);
            }
            if (!String.IsNullOrEmpty(Settings.ErrorOutput))
            {
                Settings.ErrorDirectory = Path.Combine(Settings.ErrorOutput, Settings.RunID);
                Directory.CreateDirectory(Settings.ErrorDirectory);
            }

            Int32 RecordID = 0;
            String Uniqueifier = System.Guid.NewGuid().ToString().Substring(0, 8);
            List<String> SerializedStrings = new List<String>();
            List<Task> ThreadList = new List<Task>();
            Boolean _SuccessCode = true;

            foreach (ElasticItem _item in Items)
            {
                RecordID++;
                SerializedStrings.Add(_item.SerializeForES());

                if (SerializedStrings.Count >= Settings.MaxRecordsPerRequest)
                {
                    List<String> _ThreadRecords = new List<String>();

                    SerializedStrings.ForEach((String item) => { _ThreadRecords.Add(item); });
                    SerializedStrings.Clear();

                    ESWriter _ThisLogWriter = new ESWriter(_ThreadRecords, Settings, Uniqueifier + "-" + RecordID.ToString());
                    Task _Uploader = new Task(() => _SuccessCode = _ThisLogWriter.ProcessStuff() ? _SuccessCode : false);
                    ThreadList.Add(_Uploader);
                    _Uploader.Start();

                    //If we're over the memory limit, chill out for a few seconds and wait for some threads to clear up.
                    System.Diagnostics.Process _Process = System.Diagnostics.Process.GetCurrentProcess();
                    if (_Process.WorkingSet64 > (Settings.MemoryTarget * 1048576))
                    {
                        Settings.Helpers.WriteLog("PAUSE! -- WorkingSet64 Memory Use [MB]: " + (_Process.WorkingSet64 / 1048576).ToString());
                        try { Task.WaitAll(ThreadList.ToArray()); }
                        catch (AggregateException exGroup)
                        {
                            foreach (Exception ex in exGroup.InnerExceptions) { Settings.Helpers.WriteLog(ex.Message); }
                        }
                        System.Threading.Thread.Sleep(5000);
                        _Process = System.Diagnostics.Process.GetCurrentProcess();
                        Settings.Helpers.WriteLog("Resume -- WorkingSet64 Memory Use [MB]: " + (_Process.WorkingSet64 / 1048576).ToString());
                    }
                }
            }
            Items.Clear();
            if (SerializedStrings.Count > 0)
            {
                ESWriter _ThisLogWriter = new ESWriter(SerializedStrings, Settings, Uniqueifier + "-" + RecordID.ToString());
                Task _Uploader = new Task(() => _SuccessCode = _ThisLogWriter.ProcessStuff() ? _SuccessCode : false);
                ThreadList.Add(_Uploader);
                _Uploader.Start();
            }

            if (!String.IsNullOrEmpty(Settings.OutputDirectory) &&
                Directory.Exists(Settings.OutputDirectory) &&
                (Directory.GetFiles(Settings.OutputDirectory).Length == 0)) { Directory.Delete(Settings.OutputDirectory); }
            if (!String.IsNullOrEmpty(Settings.ErrorDirectory) &&
                Directory.Exists(Settings.ErrorDirectory) &&
                (Directory.GetFiles(Settings.ErrorDirectory).Length == 0)) { Directory.Delete(Settings.ErrorDirectory); }

            try { Task.WaitAll(ThreadList.ToArray()); }
            catch (AggregateException exGroup)
            {
                foreach (Exception ex in exGroup.InnerExceptions) { Settings.Helpers.WriteLog(ex.Message); }
            }
            if (Settings.DebugMode) { Settings.Helpers.WriteLog("Task Count:" + ThreadList.Count.ToString()); }
            if (Settings.DebugMode) { Settings.Helpers.WriteLog("-- " + Settings.AppName + " Finished --"); }
            return _SuccessCode;
        }

        /// <summary>
        /// HTTP upload and sending to ElasticSearch
        /// </summary>
        private class ESWriter
        {
            private String _Uniqueifier;
            private List<String> _ThreadRecords;
            private ElasticConfig Settings;

            /// <summary>
            /// Takes a list of records and loads them to ElasticSearch
            /// </summary>
            /// <param name="ThreadRecords">Serialized Records waiting for the bulk API</param>
            /// <param name="Settings">ElasticSearch Bulk Loader Settings</param>
            /// <param name="Uniqueifier">Unique ID for this instance of the Loader</param>
            internal ESWriter(List<String> ThreadRecords, ElasticConfig Settings, String Uniqueifier)
            {
                _Uniqueifier = Uniqueifier;
                _ThreadRecords = ThreadRecords;
                this.Settings = Settings;
            }

            /// <summary>
            /// Reloads the failed records from a serialized JSON file (reloads everything, even status 200s)
            /// </summary>
            /// <param name="FileName">FileName (including full path) of the file to be reloaded</param>
            /// <param name="Settings">ElasticSearch Bulk Loader Settings</param>
            /// <param name="Uniqueifier">Unique ID for this instance of the Loader</param>
            internal ESWriter(String FileName, ElasticConfig Settings, String Uniqueifier)
            {
                _Uniqueifier = Uniqueifier;
                _ThreadRecords = new List<String>();
                String _FileContents = File.ReadAllText(FileName);
                _ThreadRecords.Add(_FileContents);
                this.Settings = Settings;
            }

            /// <summary>
            /// Process the records to Elastic.  Returns true on success, false on any errors.
            /// </summary>
            /// <returns></returns>
            internal Boolean ProcessStuff()
            {
                Boolean _SuccessCode = true;
                while (_ThreadRecords.Count > 0)
                {
                    //throw new Exception("Test Exception for Task");
                    Int32 _iteration = 1;
                    String _BulkLoadList = SerializeLogRecords();
                    using (ElasticAPI Elastic = new ElasticAPI(Settings.Helpers.WriteLog))
                    {
                        Elastic.RequestID = _Uniqueifier;
                        System.Diagnostics.Process _Process = System.Diagnostics.Process.GetCurrentProcess();
                        try
                        {
                            Elastic.Uploader(_BulkLoadList, Settings);
                            //Elastic.Response.Content.ReadAsStringAsync().Wait();
                            String _ResponseContent = Elastic.Response.Content.ReadAsStringAsync().Result;

                            if (!String.IsNullOrEmpty(Settings.OutputDirectory))
                            {
                                Settings.Helpers.WriteSerializedToDisk(Settings.OutputDirectory, _BulkLoadList, _Uniqueifier + _iteration + "_req");
                                Settings.Helpers.WriteSerializedToDisk(Settings.OutputDirectory, _ResponseContent, _Uniqueifier + _iteration + "_res");
                            }
                            if (!String.IsNullOrEmpty(Settings.ErrorDirectory) && _ResponseContent.Contains("\"errors\":true,"))
                            {
                                Settings.Helpers.WriteLog("Data errors with " + Elastic.RequestID + " -- please check " + _Uniqueifier + _iteration + " for details");
                                Settings.Helpers.WriteSerializedToDisk(Settings.ErrorDirectory, _BulkLoadList, _Uniqueifier + _iteration + "_req");
                                Settings.Helpers.WriteSerializedToDisk(Settings.ErrorDirectory, _ResponseContent, _Uniqueifier + _iteration + "_res");
                            }
                            if (Elastic.Response.IsSuccessStatusCode)
                            {
                                if (Settings.DebugMode)
                                {
                                    Settings.Helpers.WriteLog("Uploaded RequestID " + Elastic.RequestID + " with result " + (Int32)Elastic.Response.StatusCode +
                                                         " -- Memory Use [MB]: " + (_Process.WorkingSet64 / 1048576).ToString());
                                }
                            }
                            else
                            {
                                String _ErrorDir = !String.IsNullOrEmpty(Settings.ErrorDirectory) ? Settings.ErrorDirectory : @".\Logs\" ;
                                Settings.Helpers.WriteLog("Upload FAILED for " + Elastic.RequestID + " with result " + (Int32)Elastic.Response.StatusCode +
                                                            " -- " + Elastic.Response.ReasonPhrase);
                                Settings.Helpers.WriteSerializedToDisk(_ErrorDir, _BulkLoadList, _Uniqueifier + _iteration + "_req");
                                Settings.Helpers.WriteSerializedToDisk(_ErrorDir, _ResponseContent, _Uniqueifier + _iteration + "_res");
                                _SuccessCode = false;
                            }

                        }
                        catch (Exception ex)
                        {
                            String _ErrorDir = !String.IsNullOrEmpty(Settings.ErrorDirectory) ? Settings.ErrorDirectory : @".\Logs\";
                            Settings.Helpers.WriteLog("Exception with RequestID " + Elastic.RequestID + ": " + ex.ToString());
                            Settings.Helpers.WriteSerializedToDisk(_ErrorDir, _BulkLoadList, _Uniqueifier + _iteration + "_req");
                            _SuccessCode = false;
                        }
                        _iteration++;
                    }
                }
                return _SuccessCode;
            }

            private String SerializeLogRecords()
            {
                StringBuilder sb = new StringBuilder();
                Int32 _LoadedCount = 0;
                foreach (String _log in _ThreadRecords)
                {
                    //Control the size of the ElasticSearch Messages to ~5M (if Max JSON size is set)
                    if ((Settings.MaxBulkJSONSize > 0) && (sb.Length < Settings.MaxBulkJSONSize))
                    {
                        sb.AppendLine(_log);
                        _LoadedCount++;
                    }
                }
                _ThreadRecords.RemoveRange(0, _LoadedCount);
                return sb.ToString();
            }
        }
    }
}
