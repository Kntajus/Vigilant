using System;
using Xunit;

namespace Vigilant.Test
{
    public class ArticleReaderTests
    {
        [Fact]
        public async void GetContentReturnsExpectedContent()
        {
            var articleUri = new Uri("https://raw.githubusercontent.com/Kntajus/Vigilant/master/SampleData/vigilant.html");
            var content = await new ArticleReader().GetContent(articleUri);

            Assert.Equal("\n        Main Title\n        \n            \n            Here is some 'sample text' including some markup and some «HTML encoding» within it.\n        \n    ", content);
        }

        [Fact]
        public async void GetContentFromBrokenUrlReturnsDefaultError()
        {
            var articleUri = new Uri("http://broken.url/");
            var content = await new ArticleReader().GetContent(articleUri);

            Assert.Equal("Error: Unable to retrieve content", content);
        }
    }
}
