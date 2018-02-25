# Vigilant RSS Downloader

### Configuration Settings
Since the application relies on saving things to disk, it needs to know which path to save things to. If you need to modify the locations, change the source code before running:
* **Main program** - Change the `path` variable on line 23 in *Program.cs* (Default: `E:\RSS`)
* **Unit tests** - Change the `RootDirectory` variable on line 10 in *SyndicationItemRepositoryTests.cs* (Default: `E:\RSSTest`)

### Running the application
Specify an RSS Feed URL as a parameter when running it from the command line. Use the .NET Core command line tool to run it in the build output directory.
e.g. `dotnet Vigilant.dll http://rss.cnn.com/rss/edition.rss`
