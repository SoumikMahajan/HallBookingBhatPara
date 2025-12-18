using HallBookingBhatPara.Application.Interface.Payments;
using HallBookingBhatPara.Domain.DTO.CashFreePayment;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace HallBookingBhatPara.Infrastructure.Service.Payments
{
	public class CashfreeService : ICashfreeService
	{
		private readonly HttpClient _httpClient;
		private readonly CashfreeSettings _settings;
		private readonly ILogger<CashfreeService> _logger;

		public CashfreeService(
			HttpClient httpClient,
			IOptions<CashfreeSettings> settings,
			ILogger<CashfreeService> logger)
		{
			_httpClient = httpClient;
			_settings = settings.Value;
			_logger = logger;

			// Configure HttpClient base address and default headers
			_httpClient.BaseAddress = new Uri(_settings.BaseUrl);
			_httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
			_httpClient.DefaultRequestHeaders.Add("x-client-secret", _settings.ClientSecret);
			_httpClient.DefaultRequestHeaders.Add("x-api-version", _settings.ApiVersion);
		}

		public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request)
		{
			try
			{
				var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
				{
					DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
				});

				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await _httpClient.PostAsync("orders", content);

				if (response.IsSuccessStatusCode)
				{
					var responseBody = await response.Content.ReadAsStringAsync();
					_logger.LogInformation("Order created successfully: {OrderId}", request.OrderId);
					return JsonSerializer.Deserialize<CreateOrderResponse>(responseBody);
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					_logger.LogError("Failed to create order. Status: {Status}, Error: {Error}",
						response.StatusCode, errorContent);
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception occurred while creating order");
				throw;
			}
		}

		public async Task<OrderStatusResponse?> GetOrderStatusAsync(string orderId)
		{
			try
			{
				var response = await _httpClient.GetAsync($"orders/{orderId}");

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
