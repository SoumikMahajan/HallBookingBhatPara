namespace HallBookingBhatPara.Domain.DTO.HallBooking
{
    public class InsertUserConfirmhallDTO
    {
        public long catId { get; set; }
        public long hallId { get; set; }
        public long hallAvailId { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string alternatePhone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public long eventType { get; set; }
        public string eventDate { get; set; }
        public string EntryIP { get; set; }
        public long OnloadPaymentTypeId { get; set; }
        public long SelectedPaymentTypeId { get; set; }
        public PaymentSummeryDTO? PaymentSummeryDTO { get; set; } = new();
        public UserClaims? userClaims { get; set; } = new UserClaims();
    }

	public class InsertUserConfirmhallForCounterAdminDTO
	{
		public long catId { get; set; }
		public long hallId { get; set; }
		public long hallAvailId { get; set; }
		public string fullName { get; set; } = string.Empty;
		public string email { get; set; } = string.Empty;
		public string phone { get; set; } = string.Empty;
		public long eventType { get; set; }
		public string eventDate { get; set; } = string.Empty;
		public string EntryIP { get; set; } = string.Empty;
		public long OnloadPaymentTypeId { get; set; }
		public long SelectedPaymentTypeId { get; set; }
		public string MrNumber { get; set; } = string.Empty;
		public PaymentSummeryDTO? PaymentSummeryDTO { get; set; } = new();
		public UserClaims? userClaims { get; set; } = new UserClaims();
	}
}
