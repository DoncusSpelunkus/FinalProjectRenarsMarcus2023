using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class LogRepository : ILogRepository
{
    private readonly DbContextManagement _context;

    public LogRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<List<MoveLog>> GetLogsByWarehouseAsync(int warehouseId)
    {
        return await _context.MoveLogs
            .Where(log => log.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<List<AdminLog>> GetAdminLogsByWarehouseAsync(int warehouseId)
    {
        return await _context.AdminLogs
            .Where(log => log.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<AdminLog> CreateAdminLogAsync(AdminLog log)
    {
        _context.AdminLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }
    public async Task<MoveLog> CreateLogAsync(MoveLog log)
    {
        _context.MoveLogs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<bool> DeleteLogsOlderThanOneYearAsync()
    {
        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        var logsToDelete = await _context.MoveLogs
            .Where(log => log.Timestamp < oneYearAgo)
            .ToListAsync();

        if (logsToDelete.Any())
        {
            _context.MoveLogs.RemoveRange(logsToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}