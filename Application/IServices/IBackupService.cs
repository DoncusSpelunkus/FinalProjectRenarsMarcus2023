using Core.Entities;

namespace Application.IServices;

public interface IBackupService
{
     Task ExportDataToTextFileAsync(string fileName , int warehouseid);
     // Task ExportDataToTextFileAsync<T>(string fileName, List<T> data, Func<T, string> dataExtractor);
     // Task<List<Location>> GetLocationsAsync(int id);
     // Task<List<Product>> GetProductsAsync(int id);
     // Task<List<ProductLocation>> GetProductLocationsAsync(int id);
}