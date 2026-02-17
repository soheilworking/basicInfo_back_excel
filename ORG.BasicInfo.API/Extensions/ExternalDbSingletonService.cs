using Microsoft.Data.SqlClient;
using System.Data;

namespace ORG.BasicInfo.API.Extensions
{
 
    public class ExternalDbSingletonService
    {
        private readonly SqlConnection _connection;

        public ExternalDbSingletonService(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open(); // اتصال یکبار باز می‌شود
        }

        public async Task<DataTable> ExecuteQueryAsync(string sql)
        {
            using var command = new SqlCommand(sql, _connection);
            using var reader = await command.ExecuteReaderAsync();

            var table = new DataTable();
            table.Load(reader);
            return table;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql)
        {
            using var command = new SqlCommand(sql, _connection);
            return await command.ExecuteNonQueryAsync();
        }
    }
}
