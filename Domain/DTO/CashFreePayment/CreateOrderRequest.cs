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

		[JsonPropertyName("created_at")]
		public DateTime? CreatedTime { get; set; }

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

		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonPropertyName("customer_details")]
		public CustomerDetails? CustomerDetails { get; set; }

		[JsonPropertyName("order_meta")]
		public OrderMeta? OrderMeta { get; set; }

		[JsonPropertyName("payment_session_id")]
		public string PaymentSessionId { get; set; } = string.Empty;
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

		[JsonPropertyName("payment_group")]      
		public string? PaymentGroup { get; set; } 

		[JsonPropertyName("payment_method")]
		public PaymentMethod? PaymentMethod { get; set; } = new();

		[JsonPropertyName("order_id")]
		public string OrderId { get; set; } = string.Empty;

		[JsonPropertyName("is_captured")]      
		public bool IsCaptured { get; set; }

		[JsonPropertyName("error_details")] 
		public PaymentErrorDetails? ErrorDetails { get; set; } = new();
	}

	public class PaymentErrorDetails
	{
		[JsonPropertyName("error_code")]
		public string? ErrorCode { get; set; }

		[JsonPropertyName("error_description")]
		public string? ErrorDescription { get; set; }

		[JsonPropertyName("error_reason")]
		public string? ErrorReason { get; set; }

		[JsonPropertyName("error_source")]
		public string? ErrorSource { get; set; }  // "bank", "user", "cashfree"
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

	public class CashfreeErrorResponse
	{
		[JsonPropertyName("message")]
		public string Message { get; set; } = string.Empty;

		[JsonPropertyName("code")]
		public string Code { get; set; } = string.Empty;

		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;
	}

	public class ExistingCashfreeOrder
	{
		public long Id { get; set; }
		public string OrderId { get; set; } = string.Empty;
		public string CfOrderId { get; set; } = string.Empty;
		public string PaymentSessionId { get; set; } = string.Empty;
		public string OrderStatus { get; set; } = string.Empty;
		public decimal OrderAmount { get; set; }
		public string OrderCurrency { get; set; } = string.Empty;
		public DateTime? OrderExpiryTime { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
