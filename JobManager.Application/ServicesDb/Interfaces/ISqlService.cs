using JobManager.Application.Models.Jobs.Base;
using System.Data;

namespace JobManager.Application.ServicesDb.Interfaces
{
    public interface ISqlService
    {
        Task<bool> AnyAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null);
        Task<int> ExecuteAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null);
        Task<int> ExecuteScalarAsync(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null);
        Task<T?> QueryFirstOrDefaultAsync<T>(JobResponse jobResponse, string sql, object? parameters = default, int? commandTimeout = null);
        Task<IEnumerable<T>> QueryAsync<T>(JobResponse jobResponse, string sql, object? parameters = null, int? commandTimeout = null, CommandType commandType = CommandType.Text);
    }
}
