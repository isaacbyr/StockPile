using DesktopUI.Library.Models;
using NewsAPI;
using NewsAPI.Constants;
using NewsAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DesktopUI.Library.Api
{
    public class NewsEndpoint: INewsEndpoint
    {
        private readonly IApiHelper _apiHelper;

        public NewsEndpoint(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<List<NewsArticleModel>> GetMarketNews(string query)
        {
            //TODO: Add api key to config manager
            string API_KEY = "168e565f2de14e3a994fa91d6aa6b0fe";

            string url = $"https://newsapi.org/v2/everything?q={query}&sortBy=publishedAt&language=en&apiKey={API_KEY}";
            using (HttpResponseMessage response = await _apiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = (JObject)JsonConvert.DeserializeObject(result);
                    var items = data.SelectToken("articles");

                    List<NewsArticleModel> articles = new List<NewsArticleModel>();

                    foreach(var item in items)
                    {
                        articles.Add(new NewsArticleModel
                        {
                            Name = item.SelectToken("source.name").ToString(),
                            Author = item.SelectToken("author").ToString(),
                            Title = item.SelectToken("title").ToString(),
                            Url = item.SelectToken("url").ToString(),
                            UrlToImage = item.SelectToken("urlToImage").ToString(),
                            PublishedAt = item.SelectToken("publishedAt").ToString()
                        }) ;
                    }
                    return articles;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }

        }
    }
}
