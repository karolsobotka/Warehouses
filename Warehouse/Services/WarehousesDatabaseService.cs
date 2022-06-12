using System.Data;
using System.Data.SqlClient;
using warehouses.Models;
using warehouses.Models.DTO;

namespace warehouses.Services
{
	public class WarehousesDatabaseService : IDatabaseService
	{
		private readonly IConfiguration _configuration;

		public WarehousesDatabaseService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task<bool> isProductPresent(int  idProduct)
		{
			using SqlConnection conn = GetSqlConnection();
			using SqlCommand comm = new(
				"SELECT 1 FROM Product WHERE IdProduct = @idProduct",
				conn
			);

			comm.Parameters.AddWithValue("@idProduct", idProduct);

			await conn.OpenAsync();

			try
			{
				using SqlDataReader reader = await comm.ExecuteReaderAsync();

				return reader.HasRows;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<Order> GetOrder(int idProduct, int amount, DateTime createdAt)
		{
			using SqlConnection conn = GetSqlConnection();
			using SqlCommand comm = new(
				@"SELECT TOP 1 * FROM ""Order"" WHERE IdProduct = @idProduct 
				AND Amount = @amount AND CreatedAt < @createdAt", conn
			);

			comm.Parameters.AddWithValue("@idProduct", idProduct);
			comm.Parameters.AddWithValue("@amount", amount);
			comm.Parameters.AddWithValue("@createdAt", createdAt);

			await conn.OpenAsync();

			try
			{
				using SqlDataReader reader = await comm.ExecuteReaderAsync();

				await reader.ReadAsync();

				return new()
				{
					IdOrder = Convert.ToInt32(reader["IdOrder"]),
					IdProduct = Convert.ToInt32(reader["IdProduct"]),
					Amount = Convert.ToInt32(reader["Amount"]),
					CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
					FulfilledAt = Convert.ToDateTime(reader["CreatedAt"])
				};
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task<bool> isCompleted(Order order)
		{
			using SqlConnection connection = GetSqlConnection();
			using SqlCommand command = new(
				"SELECT 1 FROM Product_Warehouse WHERE IdOrder = @idOrder",
				connection
			);

			command.Parameters.AddWithValue("@idOrder", order.IdOrder);

			await connection.OpenAsync();

			try
			{
				using SqlDataReader reader = await command.ExecuteReaderAsync();

				return reader.HasRows;
			}
			catch (Exception)
			{
				return true;
			}
		}

		public async Task<int> RegisterProduct(int idOrder, ProductDto productDto)
		{
			using SqlConnection conn = GetSqlConnection();

			using SqlCommand comm = conn.CreateCommand();

			await conn.OpenAsync();

			var transaction = await conn.BeginTransactionAsync();

			comm.Transaction = (SqlTransaction)transaction;

			try
			{
				var now = DateTime.Now;

				comm.CommandText = @"UPDATE ""Order"" SET FulfilledAt = @fulfilledAt WHERE IdOrder = @idOrder";
				comm.Parameters.AddWithValue("@fulfilledAt", now);
				comm.Parameters.AddWithValue("@idOrder", idOrder);

				await comm.ExecuteNonQueryAsync();

				comm.Parameters.Clear();

				comm.CommandText = "SELECT Price FROM Product WHERE IdProduct = @idProduct";
				comm.Parameters.AddWithValue("@idProduct", productDto.IdProduct);

				using (SqlDataReader reader = await comm.ExecuteReaderAsync())
				{
					await reader.ReadAsync();

					double price = Convert.ToDouble(reader["Price"]);

					comm.Parameters.Clear();

					comm.CommandText = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) " +
						"OUTPUT INSERTED.IdProductWarehouse " +
						"VALUES (@idWarehouse, @idProduct, @idOrder, @amount, @price, @createdAt)";
					comm.Parameters.AddWithValue("@idWarehouse", productDto.IdWarehouse);
					comm.Parameters.AddWithValue("@idProduct", productDto.IdProduct);
					comm.Parameters.AddWithValue("@idOrder", idOrder);
					comm.Parameters.AddWithValue("@amount", productDto.Amount);
					comm.Parameters.AddWithValue("@price", productDto.Amount * price);
					comm.Parameters.AddWithValue("@createdAt", now);
				}

				var id = await comm.ExecuteScalarAsync();

				await transaction.CommitAsync();

				return Convert.ToInt32(id);
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();

				return -1;
			}
		}

		public async Task<int> RegisterProduct(ProductDto productDto)
		{
			using SqlConnection conn = GetSqlConnection();
			using SqlCommand comm = conn.CreateCommand();

			await conn.OpenAsync();

			comm.CommandText = "AddProductToWarehouse";
			comm.CommandType = CommandType.StoredProcedure;

			comm.Parameters.AddWithValue("@IdProduct", productDto.IdProduct);
			comm.Parameters.AddWithValue("@IdWarehouse", productDto.IdWarehouse);
			comm.Parameters.AddWithValue("@Amount", productDto.Amount);
			comm.Parameters.AddWithValue("@CreatedAt", productDto.CreatedAt);

			try
			{
				return Convert.ToInt32(await comm.ExecuteScalarAsync());
			}
			catch (Exception)
			{
				return -1;
			}
		}

		private SqlConnection GetSqlConnection()
		{
			return new(_configuration.GetConnectionString("DefaultDatabaseConnection"));
		}
	}
}
