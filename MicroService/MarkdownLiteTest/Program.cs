using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.DocAsCode.MarkdownLite;

namespace MarkdownLiteTest
{
    class Program
    {
        public static void Main(string[] args) => Run().Wait();

        private static async Task Run()
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("http://localhost:5000",
                    new StreamContent(new FileStream("test.md", FileMode.Open)));

                var markdown = await response.Content.ReadAsStringAsync();

                Console.WriteLine(markdown);
            }
        }
    }
}
