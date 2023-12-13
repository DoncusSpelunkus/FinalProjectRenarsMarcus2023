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

        public async Task IncreaseQuantityAsync(ChangeProductDto changeProductDto)
        {   
            await MakeAdminLog(changeProductDto, DateTime.Now);

            await _productLocationRepository.IncreaseQuantityAsync(changeProductDto.SourcePLocationId, changeProductDto.Quantity);
        }

        public async Task DecreaseQuantityAsync(ChangeProductDto changeProductDto)
        {   
            await MakeAdminLog(changeProductDto, DateTime.Now);

            await _productLocationRepository.DecreaseQuantityAsync(changeProductDto.SourcePLocationId, changeProductDto.Quantity);
        }

        public async Task MoveQuantityAsync(ChangeProductDto changeProductDto)
        {
            await _productLocationRepository.MoveQuantityAsync(changeProductDto.ProductSKU, changeProductDto.SourcePLocationId, changeProductDto.DestinationPLocationId, changeProductDto.Quantity);

            await _logRepository.CreateLogAsync(new MoveLog
            {
                ProductSKU = changeProductDto.ProductSKU,
                FromLocationId = changeProductDto.SourcePLocationId,
                ToLocationId = changeProductDto.DestinationPLocationId,
                Quantity = changeProductDto.Quantity,
                Timestamp = DateTime.Now,
                WarehouseId = changeProductDto.WarehouseId,
                UserId = changeProductDto.EmployeeId
            });
        }

        public async Task UpdateLastUpdatedAsync(ChangeProductDto changeProductDto, DateTime lastUpdated)
        {
            await MakeAdminLog(changeProductDto, DateTime.Now);

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

        public async Task<List<MoveLogDto>> GetAdminLogsByWarehouseAsync(int warehouseId)
        {
            var logs = await _logRepository.GetAdminLogsByWarehouseAsync(warehouseId);
            return _mapper.Map<List<MoveLogDto>>(logs);
        }

        private async Task MakeAdminLog(ChangeProductDto changeProductDto, DateTime timestamp){
            await _logRepository.CreateAdminLogAsync(new AdminLog
            {
                ProductSKU = changeProductDto.ProductSKU,
                ProductlocationId = changeProductDto.SourcePLocationId,
                QuantityChange = changeProductDto.Quantity,
                Timestamp =  timestamp,
                WarehouseId = changeProductDto.WarehouseId,
                EmployeeId = changeProductDto.EmployeeId
            });
        }
    }
