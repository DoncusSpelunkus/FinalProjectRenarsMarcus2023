using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class ShipmentRepository : IShipmentRepository
{
    private readonly DbContextManagement _context;

    public ShipmentRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<Shipment> CreateShipmentAsync(Shipment shipment)
    {
        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();
        return shipment;
    }

    public async Task<bool> DeleteShipmentAsync(int shipmentId)
    {
        var shipment = await _context.Shipments.FindAsync(shipmentId);
        if (shipment == null)
            return false;

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddProductToShipmentAsync(int shipmentId, ShipmentDetail shipmentDetail)
    {
        var shipment = await _context.Shipments.Include(s => s.ShipmentDetails)
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
            return false;

        shipment.ShipmentDetails.Add(shipmentDetail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveProductFromShipmentAsync(int shipmentId, int shipmentDetailId)
    {
        var shipment = await _context.Shipments.Include(s => s.ShipmentDetails)
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
            return false;

        var shipmentDetail = shipment.ShipmentDetails.FirstOrDefault(sd => sd.ShipmentDetailId == shipmentDetailId);
        if (shipmentDetail == null)
            return false;

        shipment.ShipmentDetails.Remove(shipmentDetail);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeProductQuantityInShipmentAsync(int shipmentId, int shipmentDetailId, int newQuantity)
    {
        var shipment = await _context.Shipments.Include(s => s.ShipmentDetails)
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);

        if (shipment == null)
            return false;

        var shipmentDetail = shipment.ShipmentDetails.FirstOrDefault(sd => sd.ShipmentDetailId == shipmentDetailId);
        if (shipmentDetail == null)
            return false;

        shipmentDetail.Quantity = newQuantity;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Shipment>> GetShipmentsByWarehouseAsync(int warehouseId)
    {
        return await _context.Shipments
            .Where(s => s.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Shipment> GetShipmentByIdAsync(int shipmentId)
    {
        return await _context.Shipments
            .FirstOrDefaultAsync(s => s.ShipmentId == shipmentId);
    }
}