using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Beeline.Tests
{
    public class SqlServerTests
    {
        [Fact]
        public async Task CreatesSerializerForSqlDataReader()
        {
            int written = 0;
            byte[] buffer = new byte[1024];
            using (var cn = new SqlConnection("data source=tcp:localhost;initial catalog=JsonDb;integrated security=false;user=sa;password=S3cr3tSqu1rr3l"))
            using (var cmd = cn.CreateCommand())
            {
                cmd.CommandText = "SELECT [Id], [Name] FROM [Test]";
                cmd.CommandType = CommandType.Text;
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var actual = DbDataReaderToJsonSerializer.For(reader, false);
                    while (await reader.ReadAsync())
                    {
                        written = actual.Write(reader, buffer);
                    }
                }
            }
            Assert.NotEqual(0, written);
            var json = Encoding.UTF8.GetString(buffer, 0, written);
            Assert.Equal(@"{ ""Id"": 1, ""Name"": ""Bob"" }", json);
        }
    }
}
