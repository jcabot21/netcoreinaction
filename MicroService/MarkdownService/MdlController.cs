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

        [HttpPut]
        public async Task<IActionResult> PutBlob(string container, string blob)
        {
            var contentLength = Request.ContentLength;

            using (var request = CreateRequest(HttpMethod.Put, container, blobl, contentLength))
            {
                request.Content = new StreamContent(Request.Body);

                request.Content.Headers.Add("Content-Length", contentLength.ToString());

                using (var response = await _client.SendAsync(request))
                {
                    if (response.StatusCode == HttpStatusCode.Created)
                    {
                        return Created($"{AccountName}/{container}/{blob}", null);
                    }
                    else
                    {
                        return Content(await response.Content.ReadAsStringAsync());
                    }
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBlog(string container, string blob)
        {
            using (var request = CreateRequest(HttpMethod.Get, container, blob))
            {
                using (var response = await _client.SendAsync(request))
                {
                    var markdown = await response.Content.ReadAsStringAsync();

                    return Content(_engine.Markup(markdown));
                }
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod verb, string container, string blob, long? contentLength = default(long?))
        {
            var path = $"{container}/{blob}";
            var rfcDate = DateTime.UtcNow.ToString("R");
            var uri = new Uri(BlobEndpoint + path);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Add("x-ms-blob-type", "BlockBlobl");
            request.Headers.Add("x-ms-date", rfcDate);
            request.Headers.Add("x-ms-version", ServiceVersion);
            request.Headers.Add("Authorization", GetAuthHeader(verb.ToString().ToUpper(), path, rfcDate, contentLength));

            return request;
        }

        private string GetAuthHeader(string verb, string path, string rfcDate, long? contentLength)
        {
            var devStorage = BlobEndpoint.StartsWith("http://127.0.0.1:10000") ? $"/{AccountName}" : string.Empty;
            var signme = 
                $"{verb}\n\n\n{contentLength}\n\n\n\n\n\n\n\n\nx-ms-blob-type:BlockBlob\nx-ms-date:{rfcDate}\nx-ms-version:{ServiceVersion}\n/{AccountName}/{path}";

            using (var sha = new HMACSHA256(System.Convert.FromBase64String(AccountKey)))
            {
                var data = Encoding.UTF8.GetBytes(signme);
                return $"SharedKey {AccountName}:{System.Convert.ToBase64String(sha.ComputeHash(data))}"; // Signature
            }
        }
    }
}