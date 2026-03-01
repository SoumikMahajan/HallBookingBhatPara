namespace HallBookingBhatPara.Domain.DTO.CashFreePayment
{
	public class CashfreeSettings
	{
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string Environment { get; set; } = "sandbox"; // "sandbox" or "production"
		public string ApiVersion { get; set; } = "2023-08-01";

		public string BaseUrl { get; set; }
	}
}
