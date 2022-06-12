using System;
using System.ComponentModel.DataAnnotations;

namespace warehouses.Models.DTO
{
	public class ProductDto
	{
		[Required]
		[DataType(DataType.DateTime, ErrorMessage = "Niepoprawna data utworzenia produktu")]
		public DateTime CreatedAt { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Niepoprawna ilość produktu")]
		public int Amount { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "niepopranwny id magazynu")]
		public int IdWarehouse { get; set; }

		

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "niepoprawne id produktu")]
		public int IdProduct { get; set; }



	}
}
