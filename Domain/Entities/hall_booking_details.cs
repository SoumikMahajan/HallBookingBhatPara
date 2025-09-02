using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_booking_details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? hall_booking_id_pk { get; set; }
        public long? hall_category_id_fk { get; set; }
        public long? hall_id_fk { get; set; }
        public long? hall_availability_id_fk { get; set; }
        public decimal? booked_rate { get; set; }
        public decimal? booked_security_money { get; set; }
        public string? booking_reference_id { get; set; }
        public short? active_status { get; set; }
        public long? process_status_id_fk { get; set; }
        public string? name_of_user { get; set; }
        public string? mobile_no { get; set; }
        public string? email { get; set; }
        public string? address { get; set; }
        public string? name_of_organizations { get; set; }
        public string? organization_mobile_no { get; set; }
        public string? organization_address { get; set; }
        public long? event_type { get; set; }
        public string? event_description { get; set; }
        public DateTime? booking_from_date { get; set; }
        public DateTime? booking_to_date { get; set; }
        public DateTime? entry_time { get; set; }
        public string? entry_ip { get; set; }
        public long? entry_by_stake_id_fk { get; set; }
        public long? entry_by_stake_details_id_fk { get; set; }
        public string? alternate_mobile_no { get; set; }
        public long? payment_type_id_fk { get; set; }
        public short? half_payment_complete_status { get; set; }
        public short? full_payment_complete_status { get; set; }
        public short? final_payment_complete_status { get; set; }
        public int? booked_day_count { get; set; }
        public decimal? total_price_summary_amount { get; set; }
    }
}
