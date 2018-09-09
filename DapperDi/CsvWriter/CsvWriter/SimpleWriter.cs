using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace CsvWriter
{
    public class SimpleWriter
    {
        private readonly TextWriter _target;
        private string[] _columns;

        public SimpleWriter(TextWriter target)
        {
            _target = target;
        }

        public void WriteHeader(params string[] columns)
        {
            _columns = columns;
            _target.Write(columns[0]);

            foreach (var column in columns.Skip(1))
            {
                _target.Write($",{column}");
            }

            _target.WriteLine();
        }

        public void WriteLine(Dictionary<string, string> values)
        {
            _target.Write(values[_columns[0]]);

            foreach (var column in _columns.Skip(1))
            {
                _target.Write($",{values[column]}");
            }

            _target.WriteLine();
        }
    }
}
