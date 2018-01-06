using System;
using System.Buffers;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Beeline
{
    public class ArrayWriter
    {
        private const byte OpenBracket = 91;
        private const byte CloseBracket = 93;
        private const byte Comma = 44;
        private const byte Newline = 10;

        private readonly RowSerializer _serializer;
        private readonly int _bufferSize;

        public ArrayWriter(RowSerializer serializer, int bufferSize)
        {
            _serializer = serializer;
            _bufferSize = bufferSize;
        }

        public async Task<int> Write(DbDataReader reader, Memory<byte> buffer, CancellationToken ct = default)
        {
            int rowCount = 0;
            int pos = 0;
            bool first = true;
            buffer.Span[pos++] = OpenBracket;

            if (!await reader.ReadAsync(ct).ConfigureAwait(false)) return 0;

            pos += _serializer.Write(reader, buffer.Span.Slice(pos, _bufferSize));
            rowCount++;

            while (await reader.ReadAsync(ct).ConfigureAwait(false))
            {
                buffer.Span[pos++] = Comma;
                buffer.Span[pos++] = Newline;
                pos += _serializer.Write(reader, buffer.Span.Slice(pos, _bufferSize));
                rowCount++;
            }
            buffer.Span[pos] = CloseBracket;

            return rowCount;
        }

        public async Task<int> Write(DbDataReader reader, Stream stream, CancellationToken ct = default)
        {
            int rowCount = 0;
            var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            stream.WriteByte(OpenBracket);

            try
            {
                if (!await reader.ReadAsync(ct).ConfigureAwait(false)) return 0;

                int bytes = _serializer.Write(reader, buffer);
                await stream.WriteAsync(buffer, 0, bytes, ct).ConfigureAwait(false);
                rowCount++;

                while (await reader.ReadAsync(ct).ConfigureAwait(false))
                {
                    stream.WriteByte(Comma);
                    stream.WriteByte(Newline);
                    bytes = _serializer.Write(reader, buffer);
                    await stream.WriteAsync(buffer, 0, bytes, ct).ConfigureAwait(false);
                    rowCount++;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
                stream.WriteByte(CloseBracket);
            }

            return rowCount;
        }
    }
}