using Microsoft.SyndicationFeed;
using System;
using System.IO;
using Xunit;

namespace Vigilant.Test
{
    public class SyndicationItemRepositoryTests : IDisposable
    {
        private const string RootDirectory = @"E:\RSSTest";
        private readonly ISyndicationItemRepository _repository;

        public SyndicationItemRepositoryTests()
        {
            _repository = new SyndicationItemRepository(RootDirectory);
        }

        public void Dispose()
        {
            Directory.Delete(RootDirectory, true);
        }

        [Fact]
        public void GetLatestPublicationDateForNewFeedReturnsNull()
        {
            var uri = new Uri("http://new.feed/");
            Assert.Null(_repository.GetLatestPublicationDate(uri));
        }

        [Fact]
        public void GetLatestPublicationDateFollowingSaveShouldReturnItemDate()
        {
            var feedUri = new Uri("http://test.feed/");
            var publicationDate = DateTime.Now;

            SaveItem(feedUri, "Sample Title", publicationDate, new Uri("http://rss.test.feed/1"));

            Assert.Equal(publicationDate, _repository.GetLatestPublicationDate(feedUri));
        }

        [Fact]
        public void GetLatestPublicationDateFollowingMultipleSavesShouldReturnLatestItemDate()
        {
            var feedUri = new Uri("http://test.multiple.feed/");
            var latestDate = DateTime.Now;

            SaveItem(feedUri, "Middle", latestDate.AddDays(-1), new Uri("http://rss.multiple.feed/2"));
            SaveItem(feedUri, "Latest", latestDate, new Uri("http://rss.multiple.feed/3"));
            SaveItem(feedUri, "Oldest", latestDate.AddDays(-2), new Uri("http://rss.multiple.feed/1"));

            Assert.Equal(latestDate, _repository.GetLatestPublicationDate(feedUri));
        }

        private void SaveItem(Uri feedUri, string title, DateTimeOffset publicationDate, Uri link)
        {
            var item = new SyndicationItem()
            {
                Title = title,
                Published = publicationDate
            };
            item.AddLink(new SyndicationLink(link));
            _repository.Save(item, string.Empty, feedUri);
        }
    }
}
