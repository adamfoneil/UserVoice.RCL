using Dapper;
using System.Data;

namespace UserVoice.Service.Extensions
{
    internal static class ConnectionExtensions
    {
        internal static async Task<bool> RowExistsAsync(this IDbConnection connection, string fromWhere, object? parameters = null, IDbTransaction? transaction = null)
        {
            return (await connection.QueryFirstOrDefaultAsync<int>($"SELECT 1 FROM {fromWhere}", parameters, transaction) == 1);
        }

        internal static async Task<bool> TableExistsAsync(this IDbConnection connection, string schema, string tableName, IDbTransaction? transaction = null)
        {
            return await RowExistsAsync(connection, "[sys].[tables] WHERE SCHEMA_NAME([schema_id])=@schema AND [name]=@tableName", new { schema, tableName }, transaction);
        }

        internal static async Task<bool> SchemaExistsAsync(this IDbConnection connection, string schema, IDbTransaction? transaction = null)
        {
            return await RowExistsAsync(connection, "[sys].[schemas] WHERE [name]=@schema", new { schema }, transaction);
        }
    }
}
