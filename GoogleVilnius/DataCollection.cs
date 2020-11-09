using System.Collections.Generic;
using RestSharp;
using HtmlAgilityPack;
using System;

namespace GoogleVilnius
{
    public class DataCollection
    {
        private string url = "/search?client=firefox-b-d&q=vilnius";
        private List<string> urlList = new List<string>();
        private List<DataCollection> allData = new List<DataCollection>();

        public string pageUrl { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        
        private DataCollection(string pageUrl, string link, string title, string text)
        {
            this.pageUrl = pageUrl;
            this.link = link;
            this.title = title;
            this.text = text;
        }

        public DataCollection()
        {

        }

        public void collectingData()
        {
            firstPageLoad(url);
            foreach(string url in urlList)
            {
                loadingPage(url);
            }
            convertingToArray();
        }
        
        private void firstPageLoad(string url)
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

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(response.Content);

            List<string> collectedUrls = new List<string>();
            foreach (HtmlNode data in htmlDocument.DocumentNode.SelectNodes("//td//a[@class='fl']"))
            {
                urlList.Add(data.GetAttributeValue("href", string.Empty));
            }
            extracting(response.Content, "https://www.google.com" + url);
        }

        private void loadingPage(string url)
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
            extracting(response.Content, "https://www.google.com" + url);
        }
        
        private string formatUrl(string url)
        {
            url = url.Replace("amp;", "");
            return url;
        }

        private void extracting(string content, string url)
        {
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
                DataCollection dataCollection = new DataCollection(url, collectedLink[i], collectedTitle[i], collectedText[i]);
                allData.Add(dataCollection);
            }
        }

        private string[, ,] convertingToArray()
        {
            int pageNr = 0;
            int pageElement = 0;

            string[, ,] dataArray = new string[10, 8, 3];
            for(int i = 0; i < allData.Count; i++)
            {
                if(i != 0)
                {
                    if (!allData[i].pageUrl.Equals(allData[i - 1].pageUrl))
                    {
                        pageNr = pageNr + 1;
                        pageElement = 0;
                    }
                }
                dataArray[pageNr, pageElement, 0] = allData[i].link;
                dataArray[pageNr, pageElement, 1] = allData[i].title;
                dataArray[pageNr, pageElement, 2] = allData[i].text;
                pageElement = pageElement + 1;
            }
            return dataArray;
        }
    }
}
