using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
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
    }
}