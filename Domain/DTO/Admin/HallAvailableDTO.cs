using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class HallAvailableDTO
    {
        [Key]
        public long hall_availability_id_pk { get; set; }
        public string? category_name { get; set; }
        public string? hall_name { get; set; }
        public DateTime? hall_availability_from_date { get; set; }
        public DateTime? hall_availability_to_date { get; set; }
        public double? rate { get; set; }
        public double? security_money { get; set; }
        public short? active_status { get; set; }
        public long? floor_id_fk { get; set; }
        public string? floor_name { get; set; }
        public long payment_type_id_fk { get; set; }
    }
}
