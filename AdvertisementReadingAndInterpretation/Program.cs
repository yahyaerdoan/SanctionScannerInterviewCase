using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;

namespace AdvertisementReadingAndInterpretation
{
    class Program
    {
        static void Main(string[] args)
        {

            ReadData();
            Console.ReadLine();
        }

        private static async void ReadData()
        {

            const string url = "https://www.ebay.com/sch/i.html?_nkw=xbox+one&_in_kw=1&_ex_kw=&_sacat=0&LH_Complete=1&_udlo=&_udhi=&_samilow=&_samihi=&_sadis=15&_stpos=&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=60&_fosrp=1";
         
            var httpclient = new HttpClient();

            var html = await httpclient.GetStringAsync(url);
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "").Equals("ListViewInner")).ToList();

            var productListItems = productsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "").Contains("item")).ToList();
            
            Console.WriteLine(productListItems.Count);
            Console.WriteLine();

            foreach (var productListItem in productListItems)
            {
                //Products id
                Console.WriteLine(productListItem.GetAttributeValue("listingid", ""));

                //Products Name
                Console.WriteLine(productListItem
                     .Descendants("h3").FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("lvtitle"))
                     .InnerText.Trim('\r', '\n', '\t'));

                //Products Price
                Console.WriteLine(productListItem.Descendants("li").FirstOrDefault(node => node.GetAttributeValue("class", "").Equals("lvprice prc"))
                    .InnerText.Trim('\r', '\n', '\t'));

                Console.WriteLine("********************");
            }


            await using (var products = new StreamWriter(@"C:\Users\User\source\repos\SanctionScannerInterviewCase\Case1\ProductList1.txt"))
            {
                foreach (var product in productListItems)
                {
                    await products.WriteLineAsync(product.InnerText.Trim('\r', '\n', '\t'));
                }
            }

            Console.WriteLine();

        }
    }
}
