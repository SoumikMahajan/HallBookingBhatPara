namespace HallBookingBhatPara.Domain.DTO.HallBooking
{
    public class PaymentSummeryDTO
    {
        public long hall_availability_id_pk { get; set; }
        public double rate { get; set; }
        public double security_money { get; set; }
        public double payable_amount { get; set; }
        public double payable_rate { get; set; }
        public long payment_type_id_fk { get; set; }
    }
}
