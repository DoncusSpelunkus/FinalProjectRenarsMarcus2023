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

    public async Task<List<Log>> GetLogsByWarehouseAsync(int warehouseId)
    {
        return await _context.Logs
            .Where(log => log.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Log> CreateLogAsync(Log log)
    {

        _ = await _context.Employees.FirstOrDefaultAsync(p => p.EmployeeId == log.UserId) ?? throw new ApplicationException("User not found");
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return log;
    }

    public async Task<bool> DeleteLogsOlderThanOneYearAsync()
    {
        var oneYearAgo = DateTime.UtcNow.AddYears(-1);
        var logsToDelete = await _context.Logs
            .Where(log => log.Timestamp < oneYearAgo)
            .ToListAsync();

        if (logsToDelete.Any())
        {
            _context.Logs.RemoveRange(logsToDelete);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}