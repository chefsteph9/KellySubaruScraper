using HtmlAgilityPack;
using System;

namespace KellySubaruScraper
{
    class Program
    {

        static void Main(string[] args)
        {
            string vin;
            string price;
            string make;
            string model;
            string photoUrl;

            string url = "https://www.kellysubaru.com/used-inventory/index.htm";

            HtmlWeb webClient = new HtmlWeb();
            HtmlDocument firstInventoryPage = webClient.Load(url);
            HtmlNodeNavigator navigator = (HtmlNodeNavigator)firstInventoryPage.DocumentNode.SelectSingleNode("//div[contains(@class,\"hproduct\")][@data-index-position=\"1\"]").CreateNavigator();

            vin = navigator.SelectSingleNode("@data-vin").Value;
            price = navigator.SelectSingleNode("//span[contains(@class,\"internetPrice\")]//span[@class=\"value\"]/text()").Value;
            make = navigator.SelectSingleNode("@data-make").Value;
            model = navigator.SelectSingleNode("@data-model").Value;
            photoUrl = navigator.SelectSingleNode("//div[@class=\"media\"]//img/@src").Value;

            Console.WriteLine($"VIN: {vin}\nPrice: {price}\nMake: {make}\nModel: {model}\nCover Photo: {photoUrl}");
            Console.ReadLine();

            return;
        }
    }
}
