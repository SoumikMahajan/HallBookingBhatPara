using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class GetHallAvailableDTO
    {
        [Key]
        public long hall_availability_id_pk { get; set; }
        public long category_id_pk { get; set; }
        public string? category_name { get; set; }
        public long hall_id_pk { get; set; }
        public string? hall_name { get; set; }
        public DateTime hall_availability_from_date { get; set; }
        public DateTime hall_availability_to_date { get; set; }
        public Double rate { get; set; }
        public Double security_money { get; set; }
        public short active_status { get; set; }
    }
}
