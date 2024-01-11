using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;
using ApplicationException = Application.ErrorHandler.ApplicationException;

namespace Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;
    private readonly AbstractValidator<LocationDto> _locationValidator;

    public LocationService(ILocationRepository locationRepository, IMapper mapper, AbstractValidator<LocationDto> locationValidator)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
        _locationValidator = locationValidator;
    }


    public async Task<List<LocationDto>> GetLocationsByWarehouseAsync(int warehouseId)
    {
        var locations = await _locationRepository.GetLocationsByWarehouseAsync(warehouseId);
        return _mapper.Map<List<LocationDto>>(locations);
    }

    public async Task<LocationDto> CreateLocationAsync(LocationDto locationDto)
    {
        var validationResult = _locationValidator.Validate(locationDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("Validation failed for locations");
        }

        var location = _mapper.Map<Location>(locationDto);
        var createdLocation = await _locationRepository.CreateLocationAsync(location);
        return _mapper.Map<LocationDto>(createdLocation);
    }

    // useless method :( EDIT it's not??? Imagine the label within the warehouse changing
    public async Task<LocationDto> UpdateLocationAsync(LocationDto locationDto)
    {
        var validationResult = _locationValidator.Validate(locationDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("Validation failed for locations");
        }

        var location = _mapper.Map<Location>(locationDto);
        var updatedLocation = await _locationRepository.UpdateLocationAsync(location);
        return _mapper.Map<LocationDto>(updatedLocation);
    }

    public async Task<bool> DeleteLocationAsync(string id)
    {
        return await _locationRepository.DeleteLocationAsync(id);
    }

    public async Task<List<LocationDto>> CreateLocationBatch(LocationDto locationDto)
    {   
        var currentMaxAisle = await _locationRepository.getBiggestAisleInt(locationDto.WarehouseId); // get the current largest aisle number to avoid duplicates
        var list = await BatchScript(locationDto, currentMaxAisle); // Await the BatchScript method
        var returnList = await _locationRepository.CreateLocationBatch(list);
        return  _mapper.Map<List<LocationDto>>(returnList);;
    }

    private Task<List<Location>> BatchScript(LocationDto locationDto, int currentMaxAisle) // auto generate locations
    {
        var values = new List<Location>();

        for (int i = 1 + currentMaxAisle; i < locationDto.Aisle + 1 + currentMaxAisle; i++)
        {
            for (int y = 1; y < locationDto.Rack + 1; y++)
            {
                for (int x = 1; x < locationDto.Shelf + 1; x++)
                {
                    for (int z = 1; z < locationDto.Bin + 1; z++)
                    {
                        values.Add(new Location
                        {
                            Aisle = i,
                            Rack = y,
                            Shelf = x,
                            Bin = z,
                            WarehouseId = locationDto.WarehouseId
                        });
                    }
                }
            }
        }
        return Task.FromResult(values);
    }
}
