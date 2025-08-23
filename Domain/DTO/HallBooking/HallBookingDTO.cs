using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.DTO.HallBooking
{
    public class HallBookingDTO
    {
        [Key]
        public long hall_availability_id_pk { get; set; }
        public string? category_name { get; set; }
        public string? hall_name { get; set; }
        public long hall_floor_id_pk { get; set; }
        public string? floor_name { get; set; }
        public double rate { get; set; }
        public double security_money { get; set; }
        //public double payable_amount { get; set; }
        public string hall_availability_from_date { get; set; }
        public string hall_availability_to_date { get; set; }
        public long hall_id_pk { get; set; }
        public long category_id_pk { get; set; }
        public long payment_type_id_fk { get; set; }
        public int avl_count { get; set; }
        public List<HallAvailableDateDTO>? hallAvailableDateDTOs { get; set; } = new();
    }

    public class HallAvailableDateDTO
    {
        [Key]
        public long hall_available_date_id_pk { get; set; }
        public string? available_date { get; set; }
        public long hall_availability_fk { get; set; }
    }
}