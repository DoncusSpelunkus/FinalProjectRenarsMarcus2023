using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;

namespace Application.Services;

public class ProductLocationService : IProductLocationService
    {
        private readonly IProductLocationRepository _productLocationRepository;
        private readonly IMapper _mapper;
        private readonly AbstractValidator<CreateProductLocationDto> _productLocationValidator;
        private readonly ILogRepository _logRepository;
        public ProductLocationService(
            IProductLocationRepository productLocationRepository,
            ILogRepository logRepository,
            IMapper mapper,
            AbstractValidator<CreateProductLocationDto> productLocationValidator)
        {
            _productLocationRepository = productLocationRepository;
            _mapper = mapper;
            _productLocationValidator = productLocationValidator;
            _logRepository = logRepository;
        }

        public async Task<List<ProductLocationDto>> GetProductLocationsByWarehouseAsync(int warehouseId)
        {
            var productLocations = await _productLocationRepository.GetProductLocationsByWarehouseAsync(warehouseId);
            return _mapper.Map<List<ProductLocationDto>>(productLocations);
        }

        public async Task<ProductLocationDto> GetProductLocationAsync(int productLocationId)
        {
            var productLocation = await _productLocationRepository.GetProductLocationAsync(productLocationId);
            return _mapper.Map<ProductLocationDto>(productLocation);
        }

        public async Task IncreaseQuantityAsync(int productLocationId, int quantityToAdd)
        {
            await _productLocationRepository.IncreaseQuantityAsync(productLocationId, quantityToAdd);
        }

        public async Task DecreaseQuantityAsync(int productLocationId, int quantityToRemove)
        {
            await _productLocationRepository.DecreaseQuantityAsync(productLocationId, quantityToRemove);
        }

        public async Task MoveQuantityAsync(string productSKU, string sourceLocationId, string destinationLocationId, int quantityToMove)
        {
            await _productLocationRepository.MoveQuantityAsync(productSKU ,sourceLocationId, destinationLocationId, quantityToMove);
        }

        public async Task UpdateLastUpdatedAsync(int productLocationId, DateTime lastUpdated)
        {
            await _productLocationRepository.UpdateLastUpdatedAsync(productLocationId, lastUpdated);
        }

        public async Task<ProductLocationDto> CreateProductLocationAsync(CreateProductLocationDto createProductLocationDto)
        {
            var validationResult = _productLocationValidator.Validate(createProductLocationDto);

            if (!validationResult.IsValid)
            {
                throw new ApplicationException("something went wring caused by : " +validationResult.Errors);
            }
            
            var productLocation = _mapper.Map<ProductLocation>(createProductLocationDto);

            var createdProductLocation = await _productLocationRepository.CreateProductLocationAsync(productLocation);

            return _mapper.Map<ProductLocationDto>(createdProductLocation);
        }

        public async Task<List<LogDto>> GetLogsByWarehouseAsync(int warehouseId)
        {
            var logs = await _logRepository.GetLogsByWarehouseAsync(warehouseId);
            return _mapper.Map<List<LogDto>>(logs);
        }

        public async Task<LogDto> CreateLogAsync(LogDto createLogDto)
        {
            
            var logEntity = _mapper.Map<Log>(createLogDto);
            
            var createdLogEntity = await _logRepository.CreateLogAsync(logEntity);
            
            var createdLogDto = _mapper.Map<LogDto>(createdLogEntity);
            
            return createdLogDto;
        }

        public async Task<bool> DeleteLogsOlderThanOneYearAsync()
        {
            var success = await _logRepository.DeleteLogsOlderThanOneYearAsync();
            Console.WriteLine(success);
            return success;
        }
    }
