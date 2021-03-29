using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TR.OGT.Content.NoSQL.Elastic.DataTransfer
{
    delegate void LogFunction(String Message);
    class ElasticAPI : IDisposable
    {

        public LogFunction LogJunk;
        public Boolean Completed = false;
        public String RequestID;
        public HttpResponseMessage Response;

        public ElasticAPI(LogFunction Logger)
        {
            LogJunk = Logger;
        }

        public Int32 Uploader(String Data, ElasticConfig Settings)
        {
            try
            {
                String[] _ElasticURLs = Settings.ElasticURLs.Split(',');
                Random rnd = new Random();

                HttpClientHandler RequestHandler = new HttpClientHandler();
                using (HttpClient Client = new HttpClient(RequestHandler))
                {
                    Client.Timeout = new TimeSpan(0, 30, 0);
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Add("User-Agent", Settings.AppName);

                    Uri _Address = new Uri(_ElasticURLs[rnd.Next(_ElasticURLs.Length - 1)] + @"_bulk");
                    //Uri _Address = new Uri(@"https://localhost/_bulk");

                    ByteArrayContent _Content = new ByteArrayContent(Encoding.UTF8.GetBytes(Data));
                    _Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                    Response = Client.PostAsync(_Address, _Content).Result;
                }

                return (Int32)Response.StatusCode;
            }
            catch (Exception ex)
            {
                LogJunk(ex.Message);
                throw;
            }
        }

        public void Dispose()
        {
            
            Response.Dispose();
        }


    }
}
