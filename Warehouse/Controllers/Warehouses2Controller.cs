using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using warehouses.Models.DTO;
using warehouses.Services;

namespace warehouses.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class Warehouses2Controller : ControllerBase
	{
		private readonly IDatabaseService _databaseService;

		public Warehouses2Controller(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		[HttpPost]
		public async Task<IActionResult> RegisterWarehouseProductAsync([FromBody] ProductDto productDto)
		{
			int id = await _databaseService.RegisterProduct(productDto);

			return id > 0
				? Ok(id)
				: BadRequest(id);
		}
	}
}
