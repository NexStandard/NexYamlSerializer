using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NexYaml.Parser
{
    /// <summary>
    /// Provides line-by-line reading with single-line lookahead for YAML parsing.
    /// </summary>
    public class YamlReader
    {
        /// <summary>
        /// The underlying text reader supplying YAML input.
        /// </summary>
        public required TextReader Reader { get; set; }

        private string? _peekBuffer;

        /// <summary>
        /// Advances to the next line and returns it.
        /// </summary>
        public bool Move([NotNullWhen(true)] out string? currentLine)
        {
            if (_peekBuffer != null)
            {
                currentLine = _peekBuffer;
                _peekBuffer = null;
                return true;
            }

            currentLine = Reader.ReadLine();
            return currentLine != null;
        }

        /// <summary>
        /// Advances to the next line, discarding it.
        /// </summary>
        public bool Move()
        {
            if (_peekBuffer != null)
            {
                _peekBuffer = null;
                return true;
            }

            var line = Reader.ReadLine();
            return line != null;
        }

        /// <summary>
        /// Peeks at the next line without consuming it.
        /// </summary>
        public bool Peek([NotNullWhen(true)] out string? nextLine)
        {
            if (_peekBuffer != null)
            {
                nextLine = _peekBuffer;
                return true;
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
