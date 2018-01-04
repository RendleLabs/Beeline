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
        
        private readonly RowSerializer _serializer;
        private readonly int _bufferSize;

        public ArrayWriter(RowSerializer serializer, int bufferSize)
        {
            _serializer = serializer;
            _bufferSize = bufferSize;
        }

        public async Task<int> Write(DbDataReader reader, Stream stream, CancellationToken ct = default)
        {
            int rowCount = 0;
            var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            stream.WriteByte(OpenBracket);
            
            try
            {
                while (await reader.ReadAsync(ct).ConfigureAwait(false))
                {
                    int bytes = _serializer.Write(reader, buffer);
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