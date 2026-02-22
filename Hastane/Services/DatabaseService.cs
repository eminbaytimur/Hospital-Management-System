using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.SqlClient;

namespace Hastane.Services
{
    internal static class DatabaseService
    {
        private static readonly string _connectionString =
            "Server=localhost;Database=HastaneDB;User Id=sa;Password=5960;TrustServerCertificate=True";

        private static SqlConnection GetConnection() => new SqlConnection(_connectionString);

        public static async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using var conn = GetConnection();
                using var cmd = new SqlCommand(query, conn);
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
                return -1;
            }
        }

        public static async Task<object> ExecuteScalarAsync(string query, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync();
        }

        public static async Task<DataTable> ExecuteQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            using var conn = GetConnection();
            using var adapter = new SqlDataAdapter(query, conn);
            if (parameters != null)
                adapter.SelectCommand.Parameters.AddRange(parameters);

            var dataTable = new DataTable();
            await Task.Run(() => adapter.Fill(dataTable));

            return dataTable;
        }

        public static async Task<SqlDataReader> ExecuteReaderAsync(string query, SqlParameter[]? parameters = null)
        {
            var conn = GetConnection();
            var cmd = new SqlCommand(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            await conn.OpenAsync();
            return await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }
    }
}