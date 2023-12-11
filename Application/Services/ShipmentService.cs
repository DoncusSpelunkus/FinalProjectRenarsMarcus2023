

using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;

public class ShipmentService : IShipmentService
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<ShipmentDto> _shipmentValidator;
    private readonly IValidator<ShipmentDetailDto> _shipmentDetailValidator;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IMapper mapper,
        IValidator<ShipmentDto> shipmentValidator,
        IValidator<ShipmentDetailDto> shipmentDetailValidator)
    {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
        _shipmentValidator = shipmentValidator;
        _shipmentDetailValidator = shipmentDetailValidator;
    }

    public async Task<ShipmentDto> CreateShipmentAsync(ShipmentDto shipmentDto)
    {
        var validationResult = await _shipmentValidator.ValidateAsync(shipmentDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var shipment = _mapper.Map<Shipment>(shipmentDto);
        
        var createdShipment = await _shipmentRepository.CreateShipmentAsync(shipment);
        var createdShipmentDto = _mapper.Map<ShipmentDto>(createdShipment);

        return createdShipmentDto;
    }

    public async Task<bool> DeleteShipmentAsync(int shipmentId)
    {
        return await _shipmentRepository.DeleteShipmentAsync(shipmentId);
    }

    public async Task<ShipmentDetailDto> AddProductToShipmentAsync(ShipmentDetailDto shipmentDetailDto, int shipmentId)
    {
       
        var validationResult = await _shipmentDetailValidator.ValidateAsync(shipmentDetailDto);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        var shipmentDetail = _mapper.Map<ShipmentDetail>(shipmentDetailDto);

        var addedShipmentDetail = await _shipmentRepository.AddProductToShipmentAsync(shipmentId, shipmentDetail);
        var addedShipmentDetailDto = _mapper.Map<ShipmentDetailDto>(addedShipmentDetail);
      
        return addedShipmentDetailDto;
    }

    public async Task<bool> RemoveProductFromShipmentAsync( int shipmentId ,int shipmentDetailId )
    {
        return await _shipmentRepository.RemoveProductFromShipmentAsync(shipmentId,shipmentDetailId);
    }

    public async Task<bool> ChangeProductQuantityInShipmentAsync( int shipmentId ,int shipmentDetailId, int newQuantity)
    {
        
        return await _shipmentRepository.ChangeProductQuantityInShipmentAsync(shipmentId,shipmentDetailId, newQuantity);
    }

    public async Task<List<ShipmentDto>> GetShipmentsByWarehouseAsync(int warehouseId)
    {
        var shipments = await _shipmentRepository.GetShipmentsByWarehouseAsync(warehouseId);

        var shipmentDtos = _mapper.Map<List<ShipmentDto>>(shipments);

        return shipmentDtos;
    }

    public async Task<ShipmentDto> GetShipmentByIdAsync(int shipmentId)
    {
        var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);
        var shipmentDto = _mapper.Map<ShipmentDto>(shipment);

        return shipmentDto;
    }
}