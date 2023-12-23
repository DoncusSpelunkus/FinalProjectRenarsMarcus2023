using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;


namespace Application.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<BrandDto> _brandValidator;

    public BrandService(IBrandRepository brandRepository, IMapper mapper, IValidator<BrandDto> brandValidator)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
        _brandValidator = brandValidator;
    }

    public async Task<List<BrandDto>> GetBrandsByWarehouseAsync(int warehouseId)
    {
        var brands = await _brandRepository.GetBrandsByWarehouseAsync(warehouseId);
        return _mapper.Map<List<BrandDto>>(brands);
    }

    public async Task<BrandDto> CreateBrandAsync(BrandDto brandDto)
    {
        var validationResult = await _brandValidator.ValidateAsync(brandDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var brand = _mapper.Map<Brand>(brandDto);
        var createdBrand = await _brandRepository.CreateBrandAsync(brand);
        var createdBrandDto = _mapper.Map<BrandDto>(createdBrand);

        return createdBrandDto;
    }

    public async Task<bool> DeleteBrandAsync(int id)
    {
        return await _brandRepository.DeleteBrandAsync(id);
    }

    public async Task<BrandDto> GetBrandByIdAsync(int id)
    {
        var brand = await _brandRepository.GetBrnadByIdAsync(id);

        var brandDto = _mapper.Map<BrandDto>(brand);

        return brandDto;
    }

    public async Task<BrandDto> UpdateBrandAsync(BrandDto brandDto)
    {
        var validationResult = _brandValidator.Validate(brandDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var brand = _mapper.Map<Brand>(brandDto);

        var updatedBrand = await _brandRepository.UpdateBrandAsync(brand);

        var updatedBrandDto = _mapper.Map<BrandDto>(updatedBrand);

        return updatedBrandDto;
    }
}