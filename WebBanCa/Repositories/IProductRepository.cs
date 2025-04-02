using WebBanCa.Models;
using System.Collections.Generic;

namespace WebBanCa.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        Task<List<string>> GetImageUrlsByProductIdAsync(int productId);
        Task AddProductImageAsync(ProductImage productImage);

        Task<List<ProductImage>> GetProductImagesByProductIdAsync(int productId);
        Task<IEnumerable<Product>> SearchProductsAsync(string name);
    }
}
