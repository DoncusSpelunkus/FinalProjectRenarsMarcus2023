using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;

namespace Application.Services;

public class TypeService : ITypeService
{
    private readonly ITypeRepository _typeRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<TypeDto> _typeValidator;

    public TypeService(ITypeRepository typeRepository, IMapper mapper, IValidator<TypeDto> typeValidator)
    {
        _typeRepository = typeRepository;
        _mapper = mapper;
        _typeValidator = typeValidator;
    }

    public async Task<List<TypeDto>> GetTypesByWarehouseAsync(int warehouseId)
    {
        var types = await _typeRepository.GetTypesByWarehouseAsync(warehouseId);
        return _mapper.Map<List<TypeDto>>(types);
    }

    public async Task<TypeDto> CreateTypeAsync(TypeDto typeDto)
    {
        var validationResult = await _typeValidator.ValidateAsync(typeDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var type = _mapper.Map<ProductType>(typeDto);

        var createdType = await _typeRepository.CreateProductTypeAsync(type);

        var createdTypeDto = _mapper.Map<TypeDto>(createdType);

        return createdTypeDto;
    }

    public async Task<bool> DeletTypeAsync(int id)
    {
        return await _typeRepository.DeleteProductTypeAsync(id);
    }

    public async Task<TypeDto> GetTypeByIdAsync(int id)
    {
        var type = await _typeRepository.GetTypeByIdAsync(id);

        var typeDto = _mapper.Map<TypeDto>(type);

        return typeDto;
    }
}
