using System.Diagnostics.CodeAnalysis;

namespace NexYaml.Parser
{
    public class YamlReader
    {
        public required StreamReader Reader { get; set; }

        private string? _peekBuffer;

        public bool Move([NotNullWhen(true)] out string? currentLine)
        {
            if (_peekBuffer != null)
            {
                currentLine = _peekBuffer;
                _peekBuffer = null;
                return true;
            }

            if (Reader.EndOfStream)
            {
                currentLine = null;
                return false;
            }

            currentLine = Reader.ReadLine();
            if (currentLine == null)
            {
                return false;
            }
            return true;
        }
        public bool Move()
        {
            if (_peekBuffer != null)
            {
                _peekBuffer = null;
                return true;
            }

            if (Reader.EndOfStream)
            {
                return false;
            }

            var line = Reader.ReadLine();
            if (line == null)
            {
                return false;
            }
            return true;
        }
        public bool Peek([NotNullWhen(true)] out string? nextLine)
        {
            if (_peekBuffer != null)
            {
                nextLine = _peekBuffer;
                return true;
            }

            if (Reader.EndOfStream)
            {
                nextLine = null;
                return false;
            }

            _peekBuffer = Reader.ReadLine();
            if (_peekBuffer == null)
            {
                nextLine = null;
                return false;
            }

            nextLine = _peekBuffer;
            return true;
        }
    }
}
