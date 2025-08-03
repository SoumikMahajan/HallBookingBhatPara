using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_availability_details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long hall_availability_id_pk { get; set; }
        public long category_id_fk { get; set; }
        public long hall_id_fk { get; set; }
        public DateOnly hall_availability_from_date { get; set; }
        public DateOnly hall_availability_to_date { get; set; }
        public decimal rate { get; set; }
        public decimal security_money { get; set; }
        public short active_status { get; set; }
        public DateTime entry_time { get; set; }
        public long entry_by_stake_details_id_fk { get; set; }
    }
}
