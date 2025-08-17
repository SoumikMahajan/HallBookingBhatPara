namespace HallBookingBhatPara.Domain.DTO.HallBooking
{
    public class InsertUserConfirmhallDTO
    {
        public long catId { get; set; }
        public long hallId { get; set; }
        public long hallAvailId { get; set; }
        public double rate { get; set; }
        public double securityMoney { get; set; }
        public double initial_payable_amount { get; set; }
        public string fullName { get; set; }
        public string phone { get; set; }
        public string alternatePhone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public long eventType { get; set; }
        public string eventDate { get; set; }
        public string EntryIP { get; set; }
        public UserClaims? userClaims { get; set; } = new UserClaims();
    }
}
