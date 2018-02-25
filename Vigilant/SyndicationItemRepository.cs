using Microsoft.SyndicationFeed;
using System;
using System.IO;
using System.Linq;

namespace Vigilant
{
    public interface ISyndicationItemRepository
    {
        DateTime? GetLatestPublicationDate(Uri feedUri);
        void Save(ISyndicationItem item, string content, Uri feedUri);
    }

    public class SyndicationItemRepository : ISyndicationItemRepository
    {
        private string _root;

        public SyndicationItemRepository(string path)
        {
            _root = path;
            if (!Directory.Exists(_root))
            {
                Directory.CreateDirectory(_root);
            }
        }

        public DateTime? GetLatestPublicationDate(Uri feedUri)
        {
            var feedDirectory = GetFeedDirectory(feedUri);
            if (!Directory.Exists(feedDirectory))
            {
                return null;
            }

            return (from file in new DirectoryInfo(GetFeedDirectory(feedUri)).GetFiles()
            let date = file.CreationTime
            orderby date descending
            select date).FirstOrDefault();
        }

        public void Save(ISyndicationItem item, string content, Uri feedUri)
        {
            var feedDirectory = GetFeedDirectory(feedUri);
            if (!Directory.Exists(feedDirectory))
            {
                Directory.CreateDirectory(feedDirectory);
            }

            var outputFile = Path.Combine(feedDirectory, GetSafeFilename(item.Title));
            outputFile = Path.ChangeExtension(outputFile, ".txt");

            using (var writer = File.CreateText(outputFile))
            {
                writer.WriteLine(item.Title);
                writer.WriteLine(item.Links.First().Uri);
                writer.WriteLine(content);
            }
            File.SetCreationTime(outputFile, item.Published.LocalDateTime);
        }

        private string GetFeedDirectory(Uri feedUri) => Path.Combine(_root, GetSafeFilename(feedUri.ToString()));
        private string GetSafeFilename(string filename) => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}
