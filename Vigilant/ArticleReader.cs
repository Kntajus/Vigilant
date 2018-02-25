using HtmlAgilityPack;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vigilant
{
    public interface IArticleReader
    {
        Task<string> GetContent(Uri uri);
    }

    public class ArticleReader : IArticleReader
    {
        public async Task<string> GetContent(Uri uri)
        {
            string articleSource;
            try
            {
                articleSource = await new HttpClient().GetStringAsync(uri);
            }
            catch (Exception)
            {
                return "Error: Unable to retrieve content";
            }

            var document = new HtmlDocument();
            document.LoadHtml(articleSource);
            var body = document.DocumentNode.SelectSingleNode("//body");
            body.Descendants()
                .Where(n => n.Name == "script")
                .ToList()
                .ForEach(n => n.Remove());
            return WebUtility.HtmlDecode(body.InnerText);
        }
    }
}
