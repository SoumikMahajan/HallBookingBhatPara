using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long hall_id_pk { get; set; }
        public long category_id_fk { get; set; }
        public string hall_name { get; set; }
        public short active_status { get; set; }
        public DateTime entry_time { get; set; }
        public byte[] hall_image { get; set; }
        public long entry_by_stake_details_id_fk { get; set; }
    }
}
