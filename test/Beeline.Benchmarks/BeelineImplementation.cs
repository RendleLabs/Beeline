using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Beeline.Benchmarks
{
    public static class BeelineImplementation
    {
        private static ArrayWriter _writer;
        
        public static async Task RunList(MemoryStream stream, int from, int to)
        {
            using (var cn = new SqlConnection(Database.ConnectionString))
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT [Id], [Value] FROM [Values] WHERE [Id] BETWEEN @from AND @to";
                cmd.Parameters.Add("@from", SqlDbType.Int).Value = from;
                cmd.Parameters.Add("@to", SqlDbType.Int).Value = to;

                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (_writer == null)
                    {
                        _writer = CreateWriter(reader);
                    }

                    await _writer.Write(reader, stream);
                }
            }
        }

        private static ArrayWriter CreateWriter(DbDataReader reader)
        {
            var serializer = RowSerializer.For(reader, true);
            return new ArrayWriter(serializer, 1024);
        }
    }
}