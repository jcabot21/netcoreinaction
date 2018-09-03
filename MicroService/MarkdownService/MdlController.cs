using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DocAsCode.MarkdownLite;
using Microsoft.Extensions.Configuration;

namespace MarkdownService
{
    [Route("/")]
    public class MdlController : Controller
    {
        private static readonly HttpClient _client = new HttpClient();
        private readonly IMarkdownEngine _engine;
        private readonly string AccountName;
        private readonly string AccountKey;
        private readonly string BlobEndpoint;
        private readonly string ServiceVersion;

        public MdlController(IMarkdownEngine engine, IConfigurationRoot configRoot)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            if (configRoot == null)
            {
                throw new ArgumentNullException(nameof(configRoot));
            }

            _engine = engine;
            AccountName = configRoot[nameof(AccountName)];
            AccountKey = configRoot[nameof(AccountKey)];
            BlobEndpoint = configRoot[nameof(BlobEndpoint)];
            ServiceVersion = configRoot[nameof(ServiceVersion)];
        }

        [HttpPost]
        public async Task<IActionResult> Convert()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var markdown = await reader.ReadToEndAsync();
                return Content(_engine.Markup(markdown));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlog(string container, string blob)
        {
            var path = $"{container}/{blob}";
            var rfcDate = DateTime.UtcNow.ToString("R");
            var devStorage = BlobEndpoint.StartsWith("http://127.0.0.1:10000") ? $"/{AccountName}" : string.Empty;
            var signme = $"GET\n\n\n\n\n\n\n\n\n\n\n\nx-ms-blob-type:BlockBlob\nx-ms-date:{rfcDate}\nx-ms-version:{ServiceVersion}\n/{AccountName}/{path}";
            var uri = new Uri(BlobEndpoint + path);
            
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                request.Headers.Add("x-ms-blob-type", "BlockBlobl");
                request.Headers.Add("x-ms-date", rfcDate);
                request.Headers.Add("x-ms-version", ServiceVersion);

                var signature = string.Empty;

                using (var sha = new HMACSHA256(System.Convert.FromBase64String(AccountKey)))
                {
                    var data = Encoding.UTF8.GetBytes(signme);
                    signature = System.Convert.ToBase64String(sha.ComputeHash(data));
                }

                request.Headers.Add("Authorization", $"SharedKey {AccountName}:{signature}");

                using (var response = await _client.SendAsync(request))
                {
                    var markdown = await response.Content.ReadAsStringAsync();

                    return Content(_engine.Markup(markdown));
                }
            }
        }
    }
}