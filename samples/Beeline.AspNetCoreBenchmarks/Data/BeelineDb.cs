using System;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Beeline.AspNetCoreBenchmarks.Configuration;
using Microsoft.Extensions.Options;

namespace Beeline.AspNetCoreBenchmarks.Data
{
    public class BeelineDb
    {
        private readonly IRandom _random;
        private readonly DbProviderFactory _dbProvider;
        private readonly string _connectionString;
        private ObjectWriter _objectWriter;

        public BeelineDb(IRandom random, DbProviderFactory dbProvider, IOptions<AppSettings> appSettings)
        {
            _random = random;
            _dbProvider = dbProvider;
            _connectionString = appSettings.Value.ConnectionString;
        }

        public async Task<int> LoadSingleQueryRow(byte[] buffer, CancellationToken ct = default)
        {
            using (var db = _dbProvider.CreateConnection())
            {
                Debug.Assert(db != null);
                db.ConnectionString = _connectionString;
                using (var cmd = CreateSingleQueryCommand(db, _random.Next(1, 10001)))
                {
                    await db.OpenAsync(ct);
                    using (var reader = await cmd.ExecuteReaderAsync(ct))
                    {
                        if (_objectWriter is null)
                        {
                            _objectWriter = new ObjectWriter(RowSerializer.For(reader, true), 1024);
                        }

                        return await _objectWriter.WriteSingle(reader, buffer, ct);
                    }
                }
            }
        }

        private static DbCommand CreateSingleQueryCommand(DbConnection db, int id)
        {
            var cmd = db.CreateCommand();
            cmd.CommandText = "SELECT id, randomnumber FROM world WHERE id = @id";
            var idParameter = cmd.CreateParameter();
            idParameter.ParameterName = "id";
            idParameter.DbType = DbType.Int32;
            idParameter.Value = id;
            cmd.Parameters.Add(idParameter);
            return cmd;
        }
    }
}