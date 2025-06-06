namespace Catalog.Application;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<ProductDto> AddAsync(ProductDto dto);
    Task UpdateAsync(ProductDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<ProductDto>> GetPagedAsync(int pageNumber, int pageSize);
}
