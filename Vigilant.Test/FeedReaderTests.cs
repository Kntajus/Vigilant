using Microsoft.SyndicationFeed;
using System;
using System.Linq;
using Xunit;

namespace Vigilant.Test
{
    public class FeedReaderTests
    {
        private readonly FeedReader _reader;

        private readonly Uri _feedUri = new Uri("https://raw.githubusercontent.com/Kntajus/Vigilant/master/SampleData/vigilant.xml");
        // Note that "first" and "sixth" here are after being ordered by date, which isn't necessarily the order in the raw feed
        private readonly ExpectedItem _first = new ExpectedItem { Title = "Trump-Russia: Manafort 'paid European ex-politicians'", Published = new DateTime(2018, 2, 24, 7, 50, 20) };
        private readonly ExpectedItem _sixth = new ExpectedItem { Title = "Paper review: Meat safety fears and 'more questions' for Corbyn", Published = new DateTime(2018, 2, 24, 6, 16, 27) };

        public FeedReaderTests()
        {
            _reader = new FeedReader();
        }

        [Fact]
        public async void GetMostRecentItemsReturnsExpectedItems()
        {
            var items = (await _reader.GetMostRecentItems(_feedUri, 6)).ToArray();

            AssertItems(items);
        }

        [Fact]
        public async void GetItemsSinceReturnsExpectedItems()
        {
            var items = (await _reader.GetItemsSince(_feedUri, _sixth.Published.AddSeconds(-1))).ToArray();

            AssertItems(items);
        }

        private void AssertItems(ISyndicationItem[] items)
        {
            Assert.Equal(6, items.Length);

            Assert.Equal(_first.Published, items[0].Published);
            Assert.Equal(_first.Title, items[0].Title);

            Assert.Equal(_sixth.Published, items[5].Published);
            Assert.Equal(_sixth.Title, items[5].Title);
        }

        private class ExpectedItem
        {
            public string Title { get; set; }
            public DateTime Published { get; set; }
        }
    }
}
