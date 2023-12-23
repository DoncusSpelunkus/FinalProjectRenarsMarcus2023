using System.Diagnostics;
using Application.Dtos;
using Application.Helpers;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;

namespace Application.Services;

public class ProductLocationService : IProductLocationService
{
    private readonly IProductLocationRepository _productLocationRepository;

    private readonly IValidator<ActionDto> _validator;
    private readonly IMapper _mapper;
    private readonly ILogRepository _logRepository;
    public ProductLocationService(
        IProductLocationRepository productLocationRepository,
        ILogRepository logRepository,
        IMapper mapper,
        IValidator<ActionDto> validator
        )
    {
        _productLocationRepository = productLocationRepository;
        _mapper = mapper;
        _logRepository = logRepository;
        _validator = validator;
    }

    public async Task<List<ProductLocationDto>> GetProductLocationsByWarehouseAsync(int warehouseId)
    {
        var productLocations = await _productLocationRepository.GetProductLocationsByWarehouseAsync(warehouseId);
        return _mapper.Map<List<ProductLocationDto>>(productLocations);
    }

    public async Task<ProductLocationDto> GetProductLocationAsync(string productLocationId)
    {
        var productLocation = await _productLocationRepository.GetProductLocationAsync(productLocationId);
        return _mapper.Map<ProductLocationDto>(productLocation);
    }

    public async Task ChangeQuantity(ActionDto actionDto)
    {
        var validationResult = _validator.Validate(actionDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("something went wring caused by : " + validationResult);
        }

        await _productLocationRepository.ChangeQuantity(actionDto.SourcePLocationId, actionDto.Quantity);

        await MakeLog(actionDto, "ChangeQuantity");
        return;
    }


    public async Task MoveQuantityAsync(ActionDto actionDto)
    {
        var validationResult = _validator.Validate(actionDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("something went wring caused by : " + validationResult);
        }

        if (actionDto.Type == RoleEnum.MoveToNewLocation)
        {
            var productLocation = _mapper.Map<ProductLocation>(actionDto);

            await _productLocationRepository.MoveToNewLocationAsync(productLocation, actionDto.SourcePLocationId);
        }
        else if (actionDto.Type == RoleEnum.MoveToExistingLocation)
        {
            await _productLocationRepository.MoveToExistingLocationAsync(actionDto.SourcePLocationId, actionDto.DestinationPLocationId, actionDto.Quantity);
        }

        await MakeLog(actionDto, "ChangeQuantity");

        return;
    }

    public async Task UpdateLastUpdatedAsync(ActionDto actionDto, DateTime lastUpdated)
    {
        var validationResult = _validator.Validate(actionDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("something went wring caused by : " + validationResult);
        }

        await _productLocationRepository.UpdateLastUpdatedAsync(actionDto.SourcePLocationId, lastUpdated);

        await MakeLog(actionDto, "ChangeQuantity");

        return;
    }

    public async Task<ProductLocationDto> CreateProductLocationAsync(ActionDto actionDto)
    {

        var validationResult = _validator.Validate(actionDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("something went wring caused by : " + validationResult);
        }

        var productLocation = _mapper.Map<ProductLocation>(actionDto);

        var createdProductLocation = await _productLocationRepository.CreateProductLocationAsync(productLocation);

        await MakeLog(actionDto, "ChangeQuantity");

        return _mapper.Map<ProductLocationDto>(createdProductLocation);

    }


    public async Task<bool> DeleteLogsOlderThanOneYearAsync()
    {
        var success = await _logRepository.DeleteLogsOlderThanOneYearAsync();
        Console.WriteLine(success);
        return success;
    }

    public async Task<List<MoveLogDto>> GetLogsByWarehouseAsync(int warehouseId)
    {
        var logs = await _logRepository.GetLogsByWarehouseAsync(warehouseId);
        return _mapper.Map<List<MoveLogDto>>(logs);
    }

    private async Task MakeLog(ActionDto actionDto, string type)
    {

        var DateTimeNow = DateTime.Now;
    
        var sku = !string.IsNullOrEmpty(actionDto.ProductSKU) ? actionDto.ProductSKU : "n/a"; // Fix: Add null check (shouldn´t happen :P)
        var fromLocation = !string.IsNullOrEmpty(actionDto.SourcePLocationId) ? actionDto.SourcePLocationId : "n/a";
        var toLocation = !string.IsNullOrEmpty(actionDto.DestinationPLocationId) ? actionDto.DestinationPLocationId : "n/a";

        await _logRepository.CreateLogAsync(new Log
        {
            ProductSKU = sku,
            FromLocationId = fromLocation,
            ToLocationId = toLocation,  
            Quantity = actionDto.Quantity,
            Timestamp = DateTimeNow,
            WarehouseId = actionDto.WarehouseId,
            UserId = actionDto.EmployeeId,
            Type = type
        });
    }
}
