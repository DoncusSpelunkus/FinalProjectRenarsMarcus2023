using Core.Entities;

namespace Application.InfraInterfaces;

public interface ILogRepository
{
    Task<List<MoveLog>> GetLogsByWarehouseAsync(int warehouseId);
    Task<MoveLog> CreateLogAsync(MoveLog log);
    Task<bool> DeleteLogsOlderThanOneYearAsync();
    Task<AdminLog> CreateAdminLogAsync(AdminLog log);
    Task<List<AdminLog>> GetAdminLogsByWarehouseAsync(int warehouseId);
}