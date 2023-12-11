﻿using System.Text.Json;
using System.Threading.Tasks;
using Application.ErrorHandler;
using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using ApplicationException = System.ApplicationException;

namespace Infrastructure.Repos;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DbContextManagement _context;

    public EmployeeRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<Employee> CreateEmployee(Employee employee)
    {
        
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return employee;
    }

    
    public async Task<List<Employee>> GetEmployeesByWarehouseId(int warehouseId)
    {
        return await _context.Employees
            .Where(e => e.WarehouseId == warehouseId)
            .ToListAsync();
    }
    
    public async Task<Employee> GetEmployeeById(int employeeId)
    {
        return await _context.Employees.FindAsync(employeeId);
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        _context.Entry(employee).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return employee;
    }

    public async Task<bool> DeleteEmployee(int employeeId)
    {
        var employee = await _context.Employees.FindAsync(employeeId);

        if (employee == null)
            return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task<bool> UserExists(string username)
    {
        return await _context.Employees.AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }
    
    public async Task<Employee> GetUserByUsernameAsync(string username)
    {
        return await _context.Employees.SingleOrDefaultAsync(x => x.Username == username);
    }
    
    public void CreateDB()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        
        SeedDatabase();
    }
    
    private void SeedDatabase()
    {
        try
        {
            if (!_context.Warehouses.Any())
            {
                var warehousesData = File.ReadAllText("../Infrastructure/Mock/MockData/warehouses.json");
                var warehouses = JsonSerializer.Deserialize<List<Warehouse>>(warehousesData);

                foreach (var item in warehouses)
                {
                    _context.Warehouses.Add(item);
                }
                _context.SaveChangesAsync();
            }
            
            if (!_context.Brands.Any()) 
            {
                var brandsData = File.ReadAllText("../Infrastructure/Mock/MockData/brands.json"); 
                var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData); 
            
                foreach (var item in brands)
                {
                    _context.Brands.Add(item);
                }
          
                _context.SaveChanges();
            }
        
            if (!_context.ProductTypes.Any()) 
            {
                var typesData = File.ReadAllText("../Infrastructure/Mock/MockData/producttype.json"); 
                var types = JsonSerializer.Deserialize<List<ProductType>>(typesData); 
           
                foreach (var item in types)
                {
                    _context.ProductTypes.Add(item);
                }
           
                _context.SaveChanges();
            }
        
            if (!_context.Products.Any()) 
            {
                var productData = File.ReadAllText("../Infrastructure/Mock/MockData/products.json");  
                var product = JsonSerializer.Deserialize<List<Product>>(productData); 
            
                foreach (var item in product)
                {
                    _context.Products.Add(item);
                }
             
                _context.SaveChanges();
            }
            
            if (!_context.Locations.Any()) 
            {
                var locationData = File.ReadAllText("../Infrastructure/Mock/MockData/locations.json");  
                var location = JsonSerializer.Deserialize<List<Location>>(locationData); 
            
                foreach (var item in location)
                {
                    _context.Locations.Add(item);
                }
             
                _context.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("something went wrong while seeding ");
        }
    }
}