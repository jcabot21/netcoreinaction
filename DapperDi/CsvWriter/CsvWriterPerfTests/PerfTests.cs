using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xunit.Performance;
using CsvWriter;

namespace CsvWriterPerfTests
{
    public class PerfTests
    {
        [Benchmark(InnerIterationCount=10000)]
        public void BenchmarkSimpleWriter()
        {
            var buffer = new byte[500000];

            using (var memoryStream = new MemoryStream(buffer))
            {
                var values = new Dictionary<string, string>()
                {
                    { "Year", "2008" },
                    { "Title", "Iron Man" },
                    { "Production Studio", "Marvel Studios" }
                };

                foreach (var iteration in Benchmark.Iterations)
                {
                    using (var streamWriter = new StreamWriter(memoryStream, Encoding.Default, 512, true))
                    {
                        using (iteration.StartMeasurement())
                        {
                            var simpleWriter = new SimpleWriter(streamWriter);

                            simpleWriter.WriteHeader("Year", "Title", "Production Studio");

                            for (int innerIteration = 0; innerIteration < Benchmark.InnerIterationCount; innerIteration++)
                            {
                                simpleWriter.WriteLine(values);
                            }

                            streamWriter.Flush();
                        }
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
            }
        }

        [Benchmark(InnerIterationCount=10000)]
        public void BenchmarkSimpleWriterToFile()
        {
            int outerIterations = 0;
            var values = new Dictionary<string, string>()
            {
                { "Year", "2008" },
                { "Title", "Iron Man" },
                { "Production Studio", "Marvel Studios" }
            };

            foreach (var iteration in Benchmark.Iterations)
            {
                var fileStream = new FileStream($"tempfile{outerIterations++}.csv", FileMode.Create, FileAccess.Write);

                using (var streamWriter = new StreamWriter(fileStream, Encoding.Default, 512, false))
                {
                    using (iteration.StartMeasurement())
                    {
                        var simpleWriter = new SimpleWriter(streamWriter);

                        simpleWriter.WriteHeader("Year", "Title", "Production Studio");

                        for (int innerIteration = 0; innerIteration < Benchmark.InnerIterationCount; innerIteration++)
                        {
                            simpleWriter.WriteLine(values);
                        }

                        streamWriter.Flush();
                    }
                }
            }
        }
    }
}