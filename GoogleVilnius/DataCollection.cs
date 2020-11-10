using System.Collections.Generic;
using RestSharp;
using HtmlAgilityPack;
using System;

namespace GoogleVilnius
{
    public class DataCollection
    {
        private string startUrl = "/search?client=firefox-b-d&q=vilnius";

        public void collectingData()
        {
            string content = PageLoad(startUrl);
            List<string> urlList = GetUrls(content);


            List<CollectedData> allData = ExtractData(content);

            foreach (string url in urlList)
            {
                content = PageLoad(url);
                List<CollectedData> data = ExtractData(content);
                allData.AddRange(data);
            }
            
            foreach(CollectedData data in allData)
            {
                Console.WriteLine("Link: " + data.Link + " Title: " + data.Title /* + " Text: " + data.Text*/);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
        }
        
        private string PageLoad(string url)
        {
            RestClient client = new RestClient("https://www.google.com" + url);
            client.AddDefaultHeader("Host", "www.google.com");
            client.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:82.0) Gecko/20100101 Firefox/82.0";
            client.AddDefaultHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            client.AddDefaultHeader("Accept-Language", "lt,en-US;q=0.8,en;q=0.6,ru;q=0.4,pl;q=0.2");
            client.AddDefaultHeader("DNT", "1");
            client.AddDefaultHeader("Connection", "keep-alive");
            client.AddDefaultHeader("Upgrade-Insecure-Requests", "1");
            client.FollowRedirects = false;

            RestRequest request = new RestRequest("", Method.GET);
            IRestResponse response = client.Execute(request);

            return response.Content;
        }

        private List<string> GetUrls(string content)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);
            
            List<string> collectedUrls = new List<string>();
            foreach (HtmlNode data in htmlDocument.DocumentNode.SelectNodes("//td//a[@class='fl']"))
            {
                string url = data.GetAttributeValue("href", string.Empty);
                
                collectedUrls.Add(formatUrl(url));
            }

            return collectedUrls;
        }

        private string formatUrl(string url)
        {
            url = url.Replace("amp;", "");
            return url;
        }

        private List<CollectedData> ExtractData(string content)
        {
            List<CollectedData> allData = new List<CollectedData>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            List<string> collectedLink = new List<string>();
            foreach (HtmlNode data in htmlDocument.DocumentNode.SelectNodes("//div[@class='TbwUpd NJjxre']"))
            {
                collectedLink.Add(data.InnerText);
            }

            List<string> collectedTitle = new List<string>();
            foreach (HtmlNode data in htmlDocument.DocumentNode.SelectNodes("//h3[@class='LC20lb DKV0Md']"))
            {
                collectedTitle.Add(data.InnerText);
            }

            List<string> collectedText = new List<string>();
            foreach (HtmlNode data in htmlDocument.DocumentNode.SelectNodes("//span[@class='aCOpRe']"))
            {
                collectedText.Add(data.InnerText);
            }

            for(int i = 0; i < collectedTitle.Count; i++)
            {
                CollectedData collectedData = new CollectedData(collectedLink[i], collectedTitle[i], collectedText[i]);
                allData.Add(collectedData);
            }

            return allData; 
        }

    }
}
