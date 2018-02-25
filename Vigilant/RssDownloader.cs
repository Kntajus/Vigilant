using System;
using System.Linq;
using System.Threading.Tasks;

namespace Vigilant
{
    public class RssDownloader
    {
        private readonly IFeedReader _feedReader;
        private readonly IArticleReader _articleReader;
        private readonly ISyndicationItemRepository _itemRepository;

        public RssDownloader(IFeedReader feedReader, IArticleReader articleReader, ISyndicationItemRepository itemRepository)
        {
            _feedReader = feedReader;
            _articleReader = articleReader;
            _itemRepository = itemRepository;
        }

        public async Task RetrieveItems(Uri feedUri)
        {
            var latestDate = _itemRepository.GetLatestPublicationDate(feedUri);

            var newItems = latestDate.HasValue ?
                await _feedReader.GetItemsSince(feedUri, latestDate.Value) :
                await _feedReader.GetMostRecentItems(feedUri, 10);

            foreach (var item in newItems)
            {
                // I don't know much about RSS, for simplicity I'm assuming there will always
                // be a link in the feed item and the first link will give me the actual article
                var articleUri = item.Links.First().Uri;
                var content = await _articleReader.GetContent(articleUri);

                _itemRepository.Save(item, content, feedUri);
            }
        }
    }
}
