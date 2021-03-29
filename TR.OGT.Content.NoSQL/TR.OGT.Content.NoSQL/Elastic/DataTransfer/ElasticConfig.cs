using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{

    public class ElasticConfig
    {
        public String RunID = "";
        public String SerializedOutput = @"D:\TR.OGT.Content\Data\";
        public String ErrorOutput = @"D:\TR.OGT.Content\Errors\";
        public String ElasticURLs = "http://54.91.211.110/elastic/";
        public int MemoryTarget = 512;
        public int MaxBulkJSONSize = 5242880;
        public int MaxRecordsPerRequest = 10000;
        internal String OutputDirectory = null;
        internal String ErrorDirectory = null;
        internal Utils Helpers;
        public Boolean DebugMode = false;
        public String AppName = "TR.OGT.Content.NoSQL";

        public String LogPath
        {
            get { return Helpers.LogPath; }
            set { Helpers.LogPath = value; }
        }

        public ElasticConfig(String AppName, Boolean ManualConfig = false)
        {
            this.AppName = AppName;
            this.Helpers = new Utils();
            this.Helpers.LogPath = @".\Logs\";
            this.RunID = System.Guid.NewGuid().ToString().Substring(0, 8);
        }

    }


    internal class Utils
    {
        internal Boolean DebugMode = false;
        internal String LogPath = "";

        internal void WriteSerializedToDisk(String OutputDirectory, String SerializedData, String Uniquifier)
        {
            Directory.CreateDirectory(OutputDirectory);
            String FileName = OutputDirectory + "\\Serialized" + System.DateTime.Now.ToString("_mmssffffff_")
                    + Uniquifier + ".json";

            using (System.IO.FileStream _SerializeLogger = new System.IO.FileStream(FileName, System.IO.FileMode.Create))
            {
                using (StreamWriter _OutputFile = new StreamWriter(_SerializeLogger))
                {
                    try
                    {
                        if (DebugMode) { WriteLog(String.Format("Serializing: {0}", FileName)); }
                        _OutputFile.WriteLine(SerializedData);
                        _OutputFile.Flush();
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.ToString());
                        throw;
                    }
                }
            }
        }

        internal void WriteLog(String Message)
        {
            using (System.IO.FileStream _LogFileStream = new FileStream(LogPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter _LogFile = new StreamWriter(_LogFileStream))
                {
                    try
                    {
                        _LogFile.WriteLine(DateTime.Now.ToString() + " | " + Environment.MachineName + " | " + Message);
                        _LogFile.Flush();
                    }
                    catch
                    {
                        //just in case we want to do something else later
                        throw;
                    }
                }
            }
        }
    }
}

