using Application.Dtos;
using Application.InfraInterfaces;
using Application.IServices;
using AutoMapper;
using Core.Entities;
using FluentValidation;

namespace Application.Services;

 public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly AbstractValidator<ProductDto> _productValidator;

    public ProductService(IProductRepository productRepository, IMapper mapper, AbstractValidator<ProductDto> productValidator)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _productValidator = productValidator;
    }

    public async Task<ProductDto> GetProductBySKUAsync(string sku)
    {
        var product = await _productRepository.GetProductBySKUAsync(sku);
        if (product == null)
        {
            throw new ApplicationException("Product not found.");
        }
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<List<ProductDto>> GetProductsByWarehouseAsync(int warehouseId)
    {
        var products = await _productRepository.GetProductsByWarehouseAsync(warehouseId);
        return _mapper.Map<List<ProductDto>>(products);
        
        // an example using linq build in sorting algorithm 
        /*
         *   bool sortByNameAscending = true;
            var products = await _productRepository.GetProductsByWarehouseAsync(warehouseId);
   
            var productDtos = _mapper.Map<List<ProductDto>>(products);

            var sorter = new ObjectSorter<ProductDto>();
            var sortedProducts = sorter.SortByProperty(productDtos, "Name", sortByNameAscending);

            return sortedProducts;
         */
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        var validationResult = _productValidator.Validate(productDto);

        if (!validationResult.IsValid)
        {
            throw new ApplicationException("something went wring caused by : " +validationResult.Errors);
        }

        var product = _mapper.Map<Product>(productDto);
        var createdProduct = await _productRepository.CreateProductAsync(product);
        return _mapper.Map<ProductDto>(createdProduct);
    }
    // error cased by the auto mapper 
    public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
    {
        var validationResult = _productValidator.Validate(productDto);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var existingProduct = await _productRepository.GetProductBySKUAsync(productDto.SKU);
        if (existingProduct == null)
        {
            throw new ApplicationException("Product not found.");
        }

        _mapper.Map(productDto, existingProduct);
        var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
        return _mapper.Map<ProductDto>(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(string sku)
    {
        var existingProduct = await _productRepository.GetProductBySKUAsync(sku);
        if (existingProduct == null)
        {
            throw new ApplicationException("Product not found.");
        }

        return await _productRepository.DeleteProductAsync(sku);
    }
}
