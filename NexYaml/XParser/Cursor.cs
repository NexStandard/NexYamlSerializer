using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.XParser
{
    public sealed class YamlCursor : IDisposable
    {
        private readonly StreamReader _reader;
        private string? _current;
        private string? _peeked;
        private bool _eof;

        public YamlCursor(StreamReader reader) => _reader = reader;

        public string? Current => _current;
        public bool EndOfFile => _eof;

        public bool Advance()
        {
            if (_peeked != null)
            {
                _current = _peeked;
                _peeked = null;
                return true;
            }
            if (_reader.EndOfStream) { _eof = true; return false; }
            _current = _reader.ReadLine();
            if (_current == null) { _eof = true; return false; }
            return true;
        }

        public string? Peek()
        {
            if (_peeked != null) return _peeked;
            if (_reader.EndOfStream) return null;
            _peeked = _reader.ReadLine();
            return _peeked;
        }

        public void Dispose() => _reader.Dispose();
    }

}
