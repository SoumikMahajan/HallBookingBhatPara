using System.Text.Json.Serialization;

namespace HallBookingBhatPara.Domain.DTO.CashFreePayment
{
	public class CreateOrderRequest
	{
		[JsonPropertyName("order_id")]
		public string OrderId { get; set; } = string.Empty;
		[JsonPropertyName("order_currency")]
		public string OrderCurrency { get; set; } = "INR";

		[JsonPropertyName("order_amount")]
		public decimal OrderAmount { get; set; }

		[JsonPropertyName("customer_details")]
		public CustomerDetails CustomerDetails { get; set; } = new();

		[JsonPropertyName("order_note")]
		public string? OrderNote { get; set; }

		[JsonPropertyName("order_meta")]
		public OrderMeta? OrderMeta { get; set; }

	}

	public class CustomerDetails
	{
		[JsonPropertyName("customer_id")]
		public string CustomerId { get; set; } = string.Empty;
		[JsonPropertyName("customer_phone")]
		public string CustomerPhone { get; set; } = string.Empty;

		[JsonPropertyName("customer_email")]
		public string CustomerEmail { get; set; } = string.Empty;

		[JsonPropertyName("customer_name")]
		public string? CustomerName { get; set; }
	}

	public class OrderMeta
	{
		[JsonPropertyName("return_url")]
		public string ReturnUrl { get; set; } = string.Empty;

		//[JsonPropertyName("notify_url")]
		//public string? NotifyUrl { get; set; }

		//[JsonPropertyName("payment_methods")]
		//public string? PaymentMethods { get; set; }
	}

	public class CreateOrderResponse
	{
		[JsonPropertyName("cf_order_id")]
		public string CfOrderId { get; set; } = string.Empty;

		[JsonPropertyName("order_id")]
		public string OrderId { get; set; } = string.Empty;

		[JsonPropertyName("entity")]
		public string Entity { get; set; } = string.Empty;

		[JsonPropertyName("order_currency")]
		public string OrderCurrency { get; set; } = string.Empty;

		[JsonPropertyName("order_amount")]
		public decimal OrderAmount { get; set; }

		[JsonPropertyName("order_status")]
		public string OrderStatus { get; set; } = string.Empty;

		[JsonPropertyName("payment_session_id")]
		public string PaymentSessionId { get; set; } = string.Empty;

		[JsonPropertyName("order_expiry_time")]
		public DateTime? OrderExpiryTime { get; set; }

		[JsonPropertyName("order_note")]
		public string? OrderNote { get; set; }
	}

	public class OrderStatusResponse
	{
		[JsonPropertyName("cf_order_id")]
		public string CfOrderId { get; set; } = string.Empty;

		[JsonPropertyName("order_id")]
		public string OrderId { get; set; } = string.Empty;

		[JsonPropertyName("order_amount")]
		public decimal OrderAmount { get; set; }

		[JsonPropertyName("order_currency")]
		public string OrderCurrency { get; set; } = string.Empty;

		[JsonPropertyName("order_status")]
		public string OrderStatus { get; set; } = string.Empty;

		[JsonPropertyName("order_token")]
		public string? OrderToken { get; set; }

		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonPropertyName("customer_details")]
		public CustomerDetails? CustomerDetails { get; set; }

		[JsonPropertyName("order_meta")]
		public OrderMeta? OrderMeta { get; set; }
	}

	public class PaymentDetails
	{
		[JsonPropertyName("cf_payment_id")]
		public string CfPaymentId { get; set; } = string.Empty;

		[JsonPropertyName("payment_status")]
		public string PaymentStatus { get; set; } = string.Empty;

		[JsonPropertyName("payment_amount")]
		public decimal PaymentAmount { get; set; }

		[JsonPropertyName("payment_currency")]
		public string PaymentCurrency { get; set; } = string.Empty;

		[JsonPropertyName("payment_time")]
		public DateTime? PaymentTime { get; set; }

		[JsonPropertyName("payment_method")]
		public PaymentMethod? PaymentMethod { get; set; }
	}

	public class PaymentMethod
	{
		[JsonPropertyName("payment_method")]
		public string Method { get; set; } = string.Empty;
	}

	// Webhook Model
	public class WebhookData
	{
		[JsonPropertyName("order")]
		public OrderStatusResponse? Order { get; set; }

		[JsonPropertyName("payment")]
		public PaymentDetails? Payment { get; set; }
	}

	public class PaymentResultViewModel
	{
		public string OrderId { get; set; }
		public string BookingId { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public string TransactionId { get; set; }
		public string ErrorMessage { get; set; }
		public string Message { get; set; }
	}
}
