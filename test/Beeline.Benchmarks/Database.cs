using System;
using System.Data;
using System.Data.SqlClient;

namespace Beeline.Benchmarks
{
    public static class Database
    {
        public const string ConnectionString = "data source=tcp:localhost;initial catalog=BeelineBench;integrated security=false;user=sa;password=S3cr3tSqu1rr3l";

        public static void Setup()
        {
            using (var cn = new SqlConnection(ConnectionString))
            using (var cmd = cn.CreateCommand())
            {
                cn.Open();
                cmd.CommandText = "IF OBJECT_ID('dbo.Values') IS NOT NULL DROP TABLE [dbo].[Values]";
                cmd.ExecuteNonQuery();
                cmd.CommandText = CreateTable;
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO [dbo].[Values] ([Id], [Value], [Timestamp]) VALUES (@id, @value, GETDATE())";
                var @id = cmd.Parameters.Add("@id", SqlDbType.Int);
                var @value = cmd.Parameters.Add("@value", SqlDbType.VarChar, 50);
                cmd.Prepare();
                for (int i = 1; i <= 1000; i++)
                {
                    @id.Value = i;
                    @value.Value = Guid.NewGuid().ToString("N");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private const string CreateTable = @"CREATE TABLE [dbo].[Values](
	[Id] [int] NOT NULL,
	[Value] [varchar](50) NOT NULL,
    [Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_Values] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
    }
}