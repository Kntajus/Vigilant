using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Vigilant
{
    public interface IFeedReader
    {
        Task<IEnumerable<ISyndicationItem>> GetMostRecentItems(Uri feedUri, int maxCount);
        Task<IEnumerable<ISyndicationItem>> GetItemsSince(Uri feedUri, DateTime dateTime);
    }

    public class FeedReader : IFeedReader
    {
        public async Task<IEnumerable<ISyndicationItem>> GetMostRecentItems(Uri feedUri, int maxCount) =>
            (await GetItems(feedUri)).OrderByDescending(i => i.Published).Take(maxCount);

        public async Task<IEnumerable<ISyndicationItem>> GetItemsSince(Uri feedUri, DateTime dateTime) =>
            (await GetItems(feedUri)).OrderByDescending(i => i.Published).TakeWhile(i => i.Published > dateTime);

        private async Task<IEnumerable<ISyndicationItem>> GetItems(Uri feedUri)
        {
            var items = new List<ISyndicationItem>();
            
            using (var xmlReader = XmlReader.Create(feedUri.ToString()))
            {
                var feedReader = new RssFeedReader(xmlReader);
                while (await feedReader.Read())
                {
                    if (feedReader.ElementType == SyndicationElementType.Item)
                    {
                        items.Add(await feedReader.ReadItem());
                    }
                }
            }

            return items;
        }
    }
}
