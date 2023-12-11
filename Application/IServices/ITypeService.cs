using Application.Dtos;

namespace Application.IServices;

public interface ITypeService
{
    Task<List<TypeDto>> GetTypesByWarehouseAsync(int warehouseId);
    Task<TypeDto> CreateTypeAsync(TypeDto brandDto);
    Task<bool> DeletTypeAsync(int id);
    Task<TypeDto> GetTypeByIdAsync(int id);
}