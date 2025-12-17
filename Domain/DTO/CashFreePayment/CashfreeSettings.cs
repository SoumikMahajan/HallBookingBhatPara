namespace HallBookingBhatPara.Domain.DTO.CashFreePayment
{
	public class CashfreeSettings
	{
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string Environment { get; set; } = "sandbox"; // "sandbox" or "production"
		public string ApiVersion { get; set; } = "2023-08-01";

		public string BaseUrl => Environment == "production"
			? "https://api.cashfree.com/pg/"
			: "https://sandbox.cashfree.com/pg/";
	}
}
