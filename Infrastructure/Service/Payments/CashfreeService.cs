using HallBookingBhatPara.Application.Interface.Payments;
using HallBookingBhatPara.Domain.DTO.CashFreePayment;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HallBookingBhatPara.Infrastructure.Service.Payments
{
	public class CashfreeService : ICashfreeService
	{
		private readonly HttpClient _httpClient;
		private readonly ILogger<CashfreeService> _logger;

		public CashfreeService(HttpClient httpClient, ILogger<CashfreeService> logger)
		{
			_httpClient = httpClient;
			_logger = logger;
		}

		public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request)
		{
			try
			{
				var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
				{
					DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
				});

				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync("orders", content);

				if ((int)response.StatusCode == 429)
				{
					_logger.LogWarning("Cashfree rate limit hit. OrderId: {OrderId}", request.OrderId);
					throw new Exception("Payment system is busy. Please try again in a moment.");
				}

				var responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					_logger.LogInformation("Order created successfully: {OrderId}", request.OrderId);
					return JsonSerializer.Deserialize<CreateOrderResponse>(responseBody);
				}

				// Handle 409 Conflict — order already exists in Cashfree

				if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
				{
					_logger.LogWarning("Cashfree order already exists. OrderId: {OrderId}. " +
						"Fetching existing order...", request.OrderId);

					// Fetch the existing order from Cashfree
					return await GetOrderAsCreateResponseAsync(request.OrderId);
				}

				var errorResponse = JsonSerializer.Deserialize<CashfreeErrorResponse>(responseBody);
				_logger.LogError("Cashfree CreateOrder failed. Status: {Status}, " +
					"Code: {Code}, Message: {Message}",
					response.StatusCode,
					errorResponse?.Code,
					errorResponse?.Message);

				return null;
			}
			catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
			{
				_logger.LogError(ex, "Cashfree CreateOrder timed out. OrderId: {OrderId}",
					request.OrderId);
				throw new Exception("Payment gateway timeout. Please try again.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception in CreateOrderAsync. OrderId: {OrderId}",
					request.OrderId);
				throw;
			}
		}

		private async Task<CreateOrderResponse?> GetOrderAsCreateResponseAsync(string orderId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"orders/{orderId}");
				var responseBody = await response.Content.ReadAsStringAsync();

				if (!response.IsSuccessStatusCode)
				{
					_logger.LogError("Failed to fetch existing Cashfree order. " +
					"OrderId: {OrderId}, Status: {Status}", orderId, response.StatusCode);
					return null;
				}

				var orderStatus = JsonSerializer.Deserialize<OrderStatusResponse>(responseBody);

				if (orderStatus == null)
					return null;

				return new CreateOrderResponse
				{
					CfOrderId = orderStatus.CfOrderId,
					OrderId = orderStatus.OrderId,
					OrderAmount = orderStatus.OrderAmount,
					OrderCurrency = orderStatus.OrderCurrency,
					OrderStatus = orderStatus.OrderStatus,
					PaymentSessionId = orderStatus.PaymentSessionId ?? string.Empty,
					CreatedTime = orderStatus.CreatedAt
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception in GetOrderAsCreateResponseAsync. OrderId: {OrderId}",
				orderId);
				return null;
			}
		}

		public async Task<OrderStatusResponse?> GetOrderStatusAsync(string orderId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"orders/{orderId}");

				if ((int)response.StatusCode == 429)
				{
					_logger.LogWarning("Cashfree rate limit hit. OrderId: {OrderId}", orderId);
					throw new Exception("Payment system is busy. Please try again in a moment.");
				}

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<OrderStatusResponse>(responseBody);
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("Failed to get order status. Status: {Status}, Error: {Error}",
						response.StatusCode, errorContent);
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occurred while getting order status");
				throw;
			}
		}

		public async Task<List<PaymentDetails>?> GetPaymentDetailsAsync(string orderId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"orders/{orderId}/payments");

				if ((int)response.StatusCode == 429)
				{
					_logger.LogWarning("Cashfree rate limit hit. OrderId: {OrderId}", orderId);
					throw new Exception("Payment system is busy. Please try again in a moment.");
				}

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<List<PaymentDetails>>(responseBody);
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("Failed to get payment details. Status: {Status}, Error: {Error}",
						response.StatusCode, errorContent);
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occurred while getting payment details");
				throw;
			}
		}
	}
}
