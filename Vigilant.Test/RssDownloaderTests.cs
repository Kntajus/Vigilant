using Microsoft.SyndicationFeed;
using NSubstitute;
using System;
using Xunit;

namespace Vigilant.Test
{
    public class RssDownloaderTests
    {
        private readonly Uri _feedUri = new Uri("http://feed.test/");
        private readonly IFeedReader _feedReader;
        private readonly IArticleReader _articleReader;
        private readonly ISyndicationItemRepository _repository;
        private readonly RssDownloader _downloader;

        public RssDownloaderTests()
        {
            _feedReader = Substitute.For<IFeedReader>();
            _articleReader = Substitute.For<IArticleReader>();
            _repository = Substitute.For<ISyndicationItemRepository>();
            _downloader = new RssDownloader(_feedReader, _articleReader, _repository);
        }

        [Fact]
        public void RetrieveItemsForNewFeedStores10Items()
        {
            _repository.GetLatestPublicationDate(_feedUri).Returns((DateTime?)null);
            var feedItems = CreateCollection(10);
            _feedReader.GetMostRecentItems(_feedUri, 10).Returns(feedItems);

            _downloader.RetrieveItems(_feedUri);

            _articleReader.Received(10).GetContent(Arg.Any<Uri>());
            _repository.Received(10).Save(Arg.Any<ISyndicationItem>(), Arg.Any<string>(), _feedUri);
        }

        [Fact]
        public void RetrieveItemsForExistingFeedStoresNewItems()
        {
            var lastDate = new DateTime(2018, 2, 24);
            _repository.GetLatestPublicationDate(_feedUri).Returns(lastDate);
            var feedItems = CreateCollection(7);
            _feedReader.GetItemsSince(_feedUri, lastDate).Returns(feedItems);

            _downloader.RetrieveItems(_feedUri);

            _articleReader.Received(7).GetContent(Arg.Any<Uri>());
            _repository.Received(7).Save(Arg.Any<ISyndicationItem>(), Arg.Any<string>(), _feedUri);
        }

        private ISyndicationItem[] CreateCollection(int length)
        {
            var items = new ISyndicationItem[length];
            for (var i = 0; i < length; i++)
            {
                var link = Substitute.For<ISyndicationLink>();
                link.Uri.Returns(new Uri("http://article.test/"));

                var item = Substitute.For<ISyndicationItem>();
                item.Links.Returns(new[] { link });

                items[i] = item;
            }
            return items;
        }
    }
}
