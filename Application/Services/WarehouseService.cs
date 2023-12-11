using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using Application.Validators;
using Core.Entities;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Application.Services;
public class WarehouseService : IWarehouseService
{
    private readonly WarehouseValidator _warehouseValidator;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public WarehouseService(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseValidator = new WarehouseValidator();
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<WarehouseDto> CreateWarehouse(WarehouseDto warehouseDto) 
    {
        var validation = _warehouseValidator.Validate(warehouseDto);

        if (!validation.IsValid)
        {
            throw new ValidationException(validation.ToString());
        }

        var warehouse = new Warehouse
        {
            Name = warehouseDto.Name,
            Capacity = warehouseDto.Capacity,
            Address = warehouseDto.Address
        };

        await _warehouseRepository.CreateWarehouseAsync(warehouse);
        
        return _mapper.Map<WarehouseDto>(warehouse);
    }
    

    public async Task<WarehouseDto> UpdateWarehouseAsync(int warehouseId, WarehouseDto warehouseDto)
    {
        var existingWarehouse = await _warehouseRepository.GetWarehouseByIdAsync(warehouseId);

        if (existingWarehouse != null)
        {
          
            existingWarehouse.Name = warehouseDto.Name;
            existingWarehouse.Capacity = warehouseDto.Capacity;
            existingWarehouse.Address = warehouseDto.Address;

            await _warehouseRepository.UpdateWarehouseAsync(existingWarehouse);
            
            return _mapper.Map<WarehouseDto>(existingWarehouse);
        }
        return null; 
    }

    public async Task<bool> DeleteWarehouseAsync(int warehouseId)
    {
        return await _warehouseRepository.DeleteWarehouseAsync(warehouseId);
    }
}

