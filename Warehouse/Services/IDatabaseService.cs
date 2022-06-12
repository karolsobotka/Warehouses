using System;
using System.Threading.Tasks;
using warehouses.Models;
using warehouses.Models.DTO;

namespace warehouses.Services
{
	public interface IDatabaseService
	{
		Task<bool> isProductPresent(int idProduct);
		Task<Order> GetOrder(int idProduct, int amount, DateTime createdAt);
		Task<bool> isCompleted(Order order);
		Task<int> RegisterProduct(int idOrder, ProductDto productDto);
		Task<int> RegisterProduct(ProductDto productDto);
	}
}
