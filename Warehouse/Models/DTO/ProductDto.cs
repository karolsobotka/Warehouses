using System;
using System.ComponentModel.DataAnnotations;

namespace warehouses.Models.DTO
{
	public class ProductDto
	{
		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Invalid product ID")]
		public int IdProduct { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Invalid product warehouse ID")]
		public int IdWarehouse { get; set; }

		[Required]
		[Range(1, int.MaxValue, ErrorMessage = "Invalid product amount")]
		public int Amount { get; set; }

		[Required]
		[DataType(DataType.DateTime, ErrorMessage = "Invalid product creation date")]
		public DateTime CreatedAt { get; set; }
	}
}
