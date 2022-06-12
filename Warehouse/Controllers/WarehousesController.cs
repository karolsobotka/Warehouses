using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using warehouses.Models.DTO;
using warehouses.Services;

namespace warehouses.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class WarehousesController : ControllerBase
	{
		private readonly IDatabaseService _databaseService;

		public WarehousesController(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		[HttpPost]
		public async Task<IActionResult> RegisterProduct([FromBody] ProductDto productDto)
		{
			if (!await _databaseService.isProductPresent(productDto.IdProduct))
				return NotFound($"Nie można znaleźć produktu z ID '{productDto.IdProduct}'");

			var order = await _databaseService
				.GetOrder(productDto.IdProduct, productDto.Amount, productDto.CreatedAt);

			if (order == null)
				return NotFound($"Nie można znaleźć zamówienia produktu o id '{productDto.IdProduct}'");

			if (await _databaseService.isCompleted(order))
				return UnprocessableEntity($"zamówienie z ID {order.IdOrder} zostało zakończoneS");

			int id = await _databaseService.RegisterProduct(order.IdOrder, productDto);

			return id > 0
				? Ok(id)
				: BadRequest(id);
		}
	}
}
