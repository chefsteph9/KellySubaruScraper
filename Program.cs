using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System;
using PuppeteerSharp;
using System.Threading.Tasks;

namespace KellySubaruScraper
{
    class Program
    {
        private class ScrapeInfo
        {
            public string Vin { get; set; } = "";
            public string Price { get; set; } = "";
            public string Make { get; set; } = "";
            public string Model { get; set; } = "";
            public string  PhotoUrl { get; set; } = "";
        }

        private const string url = "https://www.parkerhonda.com/auto/new-2019-honda-accord-lx-morehead-city-nc/40557255/";

        static async Task Main(string[] args)
        {
            bool again = true;
            while ( again )
            {
                ScrapeInfo info;
                Console.WriteLine("Would you like to scrape information using [x]path or [p]uppeteer?");
                string parseType = Console.ReadLine().ToLower();
                
                if (parseType == "r")
                {
                    Console.WriteLine("This functionality isn't ready yet");
                    info = new ScrapeInfo();
                    //info = useRegex();
                }

                else if (parseType == "x")
                {
                    info = useXPath();
                }

                else if (parseType == "p")
                {
                    info = await usePuppeteer();
                }

                else
                {
                    Console.WriteLine("Unrecognized input, would you like to try again? y/n");
                    again = Console.ReadLine().ToLower() == "y";
                    continue;
                }

                Console.WriteLine($"VIN: {info.Vin}\nPrice: {info.Price}\nMake: {info.Make}\nModel: {info.Model}\nCover Photo: {info.PhotoUrl}");
                Console.WriteLine("Would you like to try again? y/n");
                again = Console.ReadLine().ToLower() == "y";
            }

            Console.WriteLine("Goodbye");
            Console.ReadLine();
            return;
        }

        private static ScrapeInfo useXPath()
        {
            ScrapeInfo values = new ScrapeInfo();

            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument firstInventoryPage = webClient.Load(url);
            HtmlNodeNavigator navigator = (HtmlNodeNavigator)firstInventoryPage.DocumentNode.SelectSingleNode("//div[contains(@class,\"hproduct\")][@data-index-position=\"1\"]").CreateNavigator();

            values.Vin = navigator.SelectSingleNode("@data-vin").Value;
            values.Price = navigator.SelectSingleNode("//span[contains(@class,\"internetPrice\")]//span[@class=\"value\"]/text()").Value;
            values.Make = navigator.SelectSingleNode("@data-make").Value;
            values.Model = navigator.SelectSingleNode("@data-model").Value;
            values.PhotoUrl = navigator.SelectSingleNode("//div[@class=\"media\"]//img/@src").Value;

            return values;
        }

        private static ScrapeInfo useRegex()
        {
            ScrapeInfo values = new ScrapeInfo();

            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument firstInventoryPage = webClient.Load(url);
            string html = firstInventoryPage.Text;

            Regex vinRegex = new Regex("data-index-position=\"1\"[\\s\\S]*?data-vin=\"([^\"]+)\"[\\s\\S]*?data-index-position=\"2\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex priceRegex = new Regex("data-index-position=\"1\"[\\s\\S]*?class=\"value\">(\\$[\\d,.]+)<[\\s\\S]*?data-index-position=\"2\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex makeRegex = new Regex("data-make=\"([^\"]+)\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex modelRegex = new Regex("data-model=\"([^\"]+)\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            Regex photoUrlRegex = new Regex("data-index-position=\"1\"[\\s\\S]*?<img(?=[^>]*class=\"[^\"]*thumb[^\"]* \") src=\"([^\"]+)\"[\\s\\S] *? data - index - position = \"2\"", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            
            values.Vin = vinRegex.Matches(html)[1].Value;
            values.Price = priceRegex.Matches(html)[1].Value;
            values.Make = makeRegex.Matches(html)[1].Value;
            values.Model = modelRegex.Matches(html)[1].Value;
            values.PhotoUrl = photoUrlRegex.Matches(html)[1].Value;


            return values;
        }

        private static async Task<ScrapeInfo> usePuppeteer()
        {
            ScrapeInfo values = new ScrapeInfo();

            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            var options = new LaunchOptions{ Headless = true };
            var browser = await Puppeteer.LaunchAsync(options);

            var page = await browser.NewPageAsync();

            var response = await page.GoToAsync(url);

            string html = await page.GetContentAsync();

            return values;
        }
    }
}
