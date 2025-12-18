using HallBookingBhatPara.Domain.DTO.CashFreePayment;

namespace HallBookingBhatPara.Application.Interface.Payments
{
	public interface ICashfreeService
	{
		Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest request);
		Task<OrderStatusResponse?> GetOrderStatusAsync(string orderId);
		Task<List<PaymentDetails>?> GetPaymentDetailsAsync(string orderId);
	}
}
