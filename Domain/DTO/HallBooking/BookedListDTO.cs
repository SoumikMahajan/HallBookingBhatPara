namespace HallBookingBhatPara.Domain.DTO.HallBooking
{
    public class BookedListDTO
    {
        public long hall_booking_id_pk { get; set; }
        public string? booking_reference_id { get; set; }
        public long payment_type_id_fk { get; set; }
        public string? payment_status { get; set; }
        public int remaining_payment_slab { get; set; }
        public short? booking_active_status { get; set; }
        public long? booking_process_status_id { get; set; }
        public string? booking_process_name { get; set; }
        public double total_price_summary_amount { get; set; }
        public int? booked_day_count { get; set; }
        public short? half_payment_complete_status { get; set; }
        public short? full_payment_complete_status { get; set; }
        public short? final_payment_complete_status { get; set; }
        public string? user_booked_on { get; set; }
        public string? booking_date_range { get; set; }
        //public double? booked_rate { get; set; }
        //public double? booked_security_money { get; set; }
        //public short? payment_active_status { get; set; }
        //public long? payment_process_status_id { get; set; }
        //public string? payment_process_name { get; set; }
        //public long? payment_type_id_fk { get; set; }
        //public long? payment_percentage { get; set; }
        //public double? payment_amount { get; set; }
        //public double? remaining_payment_amount { get; set; }
        public long? hall_availability_id_pk { get; set; }
        public string? category_name { get; set; }
        public string? hall_name { get; set; }
        public string? floor_name { get; set; }
        public string? event_type_name { get; set; }
    }
}
