using System;
using Microsoft.DocAsCode.MarkdownLite;

namespace MarkdownLiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string source = @"
Building Your First .NET Core Applications
======

In this chapter, we will learn how to setup our development environment,
create an application, and
";

            var builder = new GfmEngineBuilder(new Options());
            var engine = builder.CreateEngine(new HtmlRenderer());
            var result = engine.Markup(source);

            Console.WriteLine(result);
        }
    }
}
