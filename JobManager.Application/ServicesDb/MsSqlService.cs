using JobManager.Application.Helpers.Extensions;
using JobManager.Application.Helpers.Services;
using JobManager.Application.Models.Jobs.Base;
using JobManager.Application.ServicesDb.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace JobManager.Application.ServicesDb
{
    public class MsSqlService : ISqlService
    {
        private string _connectionString;

        public MsSqlService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MsSql") ?? throw new NullReferenceException("MsSql ConnectionString Not Provided");
        }


        public async Task<bool> AnyAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null)
        {
            return await ExecuteScalarAsync(jobResponse, sql, parameters, commandTimeout) > 0;
        }

        public async Task<int> ExecuteScalarAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null)
        {
            if (jobResponse.JobRequest.IsProd || !DatabaseServiceHelper.IsWriteQuery(sql))
            {
                jobResponse.LogSqlQuery(sql, parameters);
                await using var conn = new SqlConnection(_connectionString);
                conn.Open();
                return 1; // await conn.ExecuteScalarAsync<int>(sql, parameters, commandTimeout: commandTimeout);
            }

            return default;
        }

        public async Task<int> ExecuteAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null)
        {
            if (jobResponse.JobRequest.IsProd || !DatabaseServiceHelper.IsWriteQuery(sql))
            {
                jobResponse.LogSqlQuery(sql, parameters);
                await using var conn = new SqlConnection(_connectionString);
                conn.Open();
                return 1; // await conn.ExecuteAsync(sql, parameters, commandTimeout: commandTimeout);
            }

            return default;
        }


        public async Task<IEnumerable<T>> QueryAsync<T>(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null, CommandType commandType = CommandType.Text)
        {
            if (jobResponse.JobRequest.IsProd || !DatabaseServiceHelper.IsWriteQuery(sql))
            {
                jobResponse.LogSqlQuery(sql, parameters);
                await using var conn = new SqlConnection(_connectionString);
                conn.Open();
                return new List<T>(); // await conn.QueryAsync<T>(sql, parameters, commandTimeout: commandTimeout);
            }

            return new List<T>();
        }

        public async Task<T?> QueryFirstOrDefaultAsync<T>(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null)
        {
            if (jobResponse.JobRequest.IsProd || !DatabaseServiceHelper.IsWriteQuery(sql))
            {
                jobResponse.LogSqlQuery(sql, parameters);
                await using var conn = new SqlConnection(_connectionString);
                conn.Open();
                return default; // (await conn.QueryAsync<T>(sql, parameters, commandTimeout: commandTimeout)).FirstOrDefault();
            }

            return default;
        }
    }
}
