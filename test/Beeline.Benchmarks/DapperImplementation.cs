using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Beeline.Benchmarks
{
    public static class DapperImplementation
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        
        public static async Task RunList(MemoryStream stream, int from, int to)
        {
            using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            using (var cn = new SqlConnection(Database.ConnectionString))
            {
                await cn.OpenAsync();
                var values = await cn.QueryAsync<ValueModel>("SELECT [Id], [Value] FROM [Values] WHERE [Id] BETWEEN @from AND @to",
                    new {from, to});
                Serializer.Serialize(writer, values);
            }
        }
    }
}