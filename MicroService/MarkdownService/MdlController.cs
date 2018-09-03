using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DocAsCode.MarkdownLite;

namespace MarkdownService
{
    [Route("/")]
    public class MdlController : Controller
    {
        private readonly IMarkdownEngine _engine;

        public MdlController(IMarkdownEngine engine)
        {
            if (engine == null)
            {
                throw new ArgumentNullException(nameof(engine));
            }

            _engine = engine;
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