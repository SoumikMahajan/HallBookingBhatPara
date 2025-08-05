using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_floor_map_details
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long hall_floor_map_id_pk { get; set; }
        public long hall_id_fk { get; set; }
        public long floor_id_fk { get; set; }
        public short active_status { get; set; }
        public DateTime entry_time { get; set; }
        public long entry_by_stake_details_id_fk { get; set; }
    }
}
