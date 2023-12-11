using Application.Dtos;
using Application.ErrorHandler;
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

        public async Task<LocationDto> GetLocationAsync(LocationDto locationDto)
        {
            var validationResult = _locationValidator.Validate(locationDto);

            if (!validationResult.IsValid)
            {
                throw new ApplicationException("Validation failed for locations");
            }

            var location = await _locationRepository.GetLocationAsync(_mapper.Map<Location>(locationDto));
            return _mapper.Map<LocationDto>(location);
        }

        public async Task<List<LocationDto>> GetLocationsByWarehouseAsync(int warehouseId)
        {
            Console.WriteLine(warehouseId);
            var locations = await _locationRepository.GetLocationsByWarehouseAsync(warehouseId);
            Console.WriteLine(locations.Count);
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
        
        // useless method :(
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

        public async Task<bool> DeleteLocationAsync(LocationDto locationDto)
        {
            var location = _mapper.Map<Location>(locationDto);
            return await _locationRepository.DeleteLocationAsync(location);
        }
    }
