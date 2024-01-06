using Application.InfraInterfaces;
using Application.IServices;


namespace Application.Services // Should be removed as backup can be handled by AWS
{
    public class BackupService : IBackupService
    {
        private readonly IProductRepository _productRepository;
        

        public BackupService(
            IProductRepository productRepository
        )
        {
            _productRepository = productRepository;
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
        
               
           
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
               
            }
        }
    }
}
