using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_category_master
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long category_id_pk { get; set; }
        public string? category_name { get; set; }
        public short active_status { get; set; }
    }
}
