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

        public async Task<ProductLocationDto> GetProductLocationAsync(string productLocationId)
        {
            var productLocation = await _productLocationRepository.GetProductLocationAsync(productLocationId);
            return _mapper.Map<ProductLocationDto>(productLocation);
        }

        public async Task ChangeQuantity(ChangeProductDto changeProductDto)
        {   
            await MakeLog(changeProductDto, "ChangeQuantiy");

            await _productLocationRepository.ChangeQuantity(changeProductDto.SourcePLocationId, changeProductDto.Quantity);
        }


        public async Task MoveQuantityAsync(ChangeProductDto changeProductDto)
        {
            var pl = await _productLocationRepository.GetProductLocationAsync(changeProductDto.SourcePLocationId);

            await MakeLog(changeProductDto, "MoveQuantity");

            await _productLocationRepository.MoveQuantityAsync(pl.ProductSKU, changeProductDto.SourcePLocationId, changeProductDto.DestinationPLocationId, changeProductDto.Quantity);
        }

        public async Task UpdateLastUpdatedAsync(ChangeProductDto changeProductDto, DateTime lastUpdated)
        {
            await MakeLog(changeProductDto, "UpdateLastUpdated");

            await _productLocationRepository.UpdateLastUpdatedAsync(changeProductDto.SourcePLocationId, lastUpdated);
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

        private async Task MakeLog(ChangeProductDto changeProductDto, string type)
        {

            var DateTimeNow = DateTime.Now;
            var pl = await _productLocationRepository.GetProductLocationAsync(changeProductDto.SourcePLocationId);
            var toLocation = !string.IsNullOrEmpty(changeProductDto.DestinationPLocationId) ? changeProductDto.DestinationPLocationId : "n/a";

            await _logRepository.CreateLogAsync(new Log
            {
                ProductSKU = pl.ProductSKU,
                FromLocationId = changeProductDto.SourcePLocationId,
                ToLocationId = toLocation,
                Quantity = changeProductDto.Quantity,
                Timestamp = DateTimeNow,
                WarehouseId = changeProductDto.WarehouseId,
                UserId = changeProductDto.EmployeeId,
                Type = type
            });
        }
    }
