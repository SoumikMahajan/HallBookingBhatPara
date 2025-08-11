using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.Entities
{
    public class hall_event_type_master
    {
        [Key]
        public long hall_event_type_id_pk { get; set; }
        public string event_type_name { get; set; }
        public short active_status { get; set; }
    }
}
