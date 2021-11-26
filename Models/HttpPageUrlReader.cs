using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using HtmlTagCounter.Abstractions;

namespace HtmlTagCounter.Models
{
    public class HttpPageUrlReader : IUrlReader
    {
        private string _urlAddress;
        public string UrlAddress => _urlAddress;

        public string ReadPage(string url)
        {
            var resultText = Read(url);
            return resultText;
        }

        private string Read(string url)
        {
            _urlAddress = url;
            string resultText;
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        resultText = content.ReadAsStringAsync().Result;
                    }
                }
            }
            
            return resultText;
        }
    }

}
    