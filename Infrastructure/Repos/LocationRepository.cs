﻿using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class LocationRepository : ILocationRepository
{
    private readonly DbContextManagement _context;

    // the bug was here : forget to add the constructor 
    public LocationRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<Location> GetLocationAsync(string id)
    {
        return await _context.Locations
            .FirstOrDefaultAsync(l => l.LocationId == id);
    }

    public async Task<List<Location>> GetLocationsByWarehouseAsync(int warehouseId)
    {
        return await _context.Locations
            .Where(l => l.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Location> CreateLocationAsync(Location location)
    {
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<Location> UpdateLocationAsync(Location location)
    {
        _context.Locations.Update(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task<bool> DeleteLocationAsync(string id)
    {
        var existingLocation = await GetLocationAsync(id);
        if (existingLocation == null)
            return false;

        _context.Locations.Remove(existingLocation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Location>> CreateLocationBatch(List<Location> locations)
    {
        _context.Locations.AddRange(locations);
        await _context.SaveChangesAsync();
        return locations;
    }

    public async Task<int> getBiggestAisleInt(int warehouseId){
        try
        {
            return await _context.Locations.Where(l => l.WarehouseId == warehouseId).MaxAsync(l => l.Aisle);
        }
        catch (Exception e)
        {
            return 0;
        }
    }
}