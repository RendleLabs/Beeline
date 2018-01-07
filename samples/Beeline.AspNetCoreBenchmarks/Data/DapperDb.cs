using System.Data.Common;
using System.Threading.Tasks;
using Beeline.AspNetCoreBenchmarks.Configuration;
using Dapper;
using Microsoft.Extensions.Options;

namespace Beeline.AspNetCoreBenchmarks.Data
{
    public class DapperDb
    {
        private readonly IRandom _random;
        private readonly DbProviderFactory _dbProviderFactory;
        private readonly string _connectionString;

        public DapperDb(IRandom random, DbProviderFactory dbProviderFactory, IOptions<AppSettings> appSettings)
        {
            _random = random;
            _dbProviderFactory = dbProviderFactory;
            _connectionString = appSettings.Value.ConnectionString;
        }

        public async Task<World> LoadSingleQueryRow()
        {
            using (var db = _dbProviderFactory.CreateConnection())
            {
                db.ConnectionString = _connectionString;

                // Note: Don't need to open connection if only doing one thing; let dapper do it
                return await ReadSingleRow(db);
            }
        }

        async Task<World> ReadSingleRow(DbConnection db)
        {
            return await db.QueryFirstOrDefaultAsync<World>(
                "SELECT id, randomnumber FROM world WHERE id = @Id",
                new { Id = _random.Next(1, 10001) });
        }
    }
}