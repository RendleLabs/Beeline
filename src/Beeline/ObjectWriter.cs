using System;
using System.Buffers;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Beeline
{
    public class ObjectWriter
    {
        private readonly RowSerializer _serializer;
        private readonly int _bufferSize;

        public ObjectWriter(RowSerializer serializer, int bufferSize)
        {
            _serializer = serializer;
            _bufferSize = bufferSize;
        }
        
        public async Task<int> WriteSingle(DbDataReader reader, Memory<byte> buffer, CancellationToken ct = default)
        {
            if (await reader.ReadAsync(ct).ConfigureAwait(false))
            {
                return _serializer.Write(reader, buffer.Span);
            }

            return 0;
        }

        public async Task<int> WriteSingle(DbDataReader reader, Stream stream, CancellationToken ct = default)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            try
            {
                if (await reader.ReadAsync(ct).ConfigureAwait(false))
                {
                    int bytes = _serializer.Write(reader, buffer);
                    await stream.WriteAsync(buffer, 0, bytes, ct);
                    return 1;
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return 0;
        }
    }
}