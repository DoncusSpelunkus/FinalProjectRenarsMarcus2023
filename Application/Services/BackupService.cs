using Application.InfraInterfaces;
using Application.IServices;


namespace Application.Services
{
    public class BackupService : IBackupService
    {
        private readonly IProductLocationRepository _productLocationRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILocationRepository _locationRepository;

        public BackupService(
            IProductLocationRepository productLocationRepository,
            IProductRepository productRepository,
            ILocationRepository locationRepository)
        {
            _productLocationRepository = productLocationRepository;
            _productRepository = productRepository;
            _locationRepository = locationRepository;
        }

        public async Task ExportDataToTextFileAsync(string fileName, int warehouseId)
        {
            var dataToExport = new List<string>();

            try
            {
               
                var products = await _productRepository.GetProductsByWarehouseAsync(warehouseId);
                if (products != null && products.Any())
                {
                    //dataToExport.AddRange(products.Select(p => $"{p.Name}, {p.Description},{p.Category},{p.Height},{p.Length}"));
                    dataToExport.AddRange(products.Select(p => $"{p.Name}, {p.Description} {p.Category}, {p.Height}, {p.Length}"));
                }
                dataToExport.Add("----------------");
                dataToExport.Add("");
                dataToExport.Add("");

                
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                await File.WriteAllLinesAsync(filePath, dataToExport);
        
               
                // foreach (var product in products)
                // {
                //     Console.WriteLine($"Product: {product.Name}, {product.Description}, {product.Brand.Name}, {product.ProductType.Name}, {product.Category}, {product.Height}, {product.Length}");
                // }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Handle the exception as required
            }
        }
    }
}



// // Backup Locations
// var locations = await _locationRepository.GetLocationsByWarehouseAsync(warehouseid);
// if (locations != null && locations.Any())
// {
//     dataToExport.AddRange(locations.Select(l => $"{l.LocationId}, {l.Aisle}, {l.Rack} , {l.Shelf},{l.Bin},"));
// }
// dataToExport.Add("----------------");
// dataToExport.Add("");
// dataToExport.Add("");
