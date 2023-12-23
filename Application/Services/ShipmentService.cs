

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

    private readonly IValidator<AddToShipmentDetails> _addToShipmentDetailsValidator;

    public ShipmentService(
        IShipmentRepository shipmentRepository,
        IMapper mapper,
        IValidator<ShipmentDto> shipmentValidator,
        IValidator<ShipmentDetailDto> shipmentDetailValidator,
        IValidator<AddToShipmentDetails> addToShipmentDetailsValidator)
 {
        _shipmentRepository = shipmentRepository;
        _mapper = mapper;
        _shipmentValidator = shipmentValidator;
        _shipmentDetailValidator = shipmentDetailValidator;
        _addToShipmentDetailsValidator = addToShipmentDetailsValidator;
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

    public async Task<ShipmentDto> AddProductToShipmentAsync(AddToShipmentDetails addToShipmentDetails, int shipmentId)
    {
       
        var validationResult = await _addToShipmentDetailsValidator.ValidateAsync(addToShipmentDetails);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        foreach (var shipmentDetailDto in addToShipmentDetails.ShipmentDetails)
        {
            var shipmentDetail = _mapper.Map<ShipmentDetail>(shipmentDetailDto);
            await _shipmentRepository.AddProductToShipmentAsync(shipmentId, shipmentDetail);
        }
        
        var shipment = await _shipmentRepository.GetShipmentByIdAsync(shipmentId);
        var shipmentDto = _mapper.Map<ShipmentDto>(shipment);
      
        return shipmentDto;
    }

    public async Task<bool> RemoveProductFromShipmentAsync( int shipmentId ,int[] shipmentDetailId )
    {
        foreach (var id in shipmentDetailId)
        {
            await _shipmentRepository.RemoveProductFromShipmentAsync(shipmentId, id);
        }
        
        return true;
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