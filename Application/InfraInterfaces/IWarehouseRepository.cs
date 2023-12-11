using Core.Entities;
using System.Threading.Tasks;

namespace Application.InfraInterfaces;
public interface IWarehouseRepository
{
    Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse);
    Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse);
    Task<bool> DeleteWarehouseAsync(int warehouseId);
    Task<Warehouse> GetWarehouseByIdAsync(int warehouseId);
}