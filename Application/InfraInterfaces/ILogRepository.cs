using Core.Entities;

namespace Application.InfraInterfaces;

public interface ILogRepository
{
    Task<List<Log>> GetLogsByWarehouseAsync(int warehouseId);
    Task<Log> CreateLogAsync(Log log);
    Task<bool> DeleteLogsOlderThanOneYearAsync();
}