using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Vigilant
{
    class Program
    {
        static void Main(string[] args) => MainAsync(args).GetAwaiter().GetResult();

        static async Task MainAsync(string[] args)
        {
            if (args.Length != 1 || !Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
            {
                Console.WriteLine($"Usage: {Assembly.GetExecutingAssembly().GetName().Name} <RSS Feed URL>");
                Console.WriteLine("Press any key to exit.");
                Console.Read();
                return;
            }

            Console.WriteLine("Downloading...");

            var path = @"E:\RSS";
            var downloader = new RssDownloader(new FeedReader(), new ArticleReader(), new SyndicationItemRepository(path));
            await downloader.RetrieveItems(uri);

            Console.WriteLine("Download complete.");
            Console.WriteLine($"You can read the articles by navigating to {path} in Windows Explorer and double clicking the files.");
            Console.WriteLine("Press any key to exit.");
            Console.Read();
        }
    }
}
