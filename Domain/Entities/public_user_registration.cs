using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HallBookingBhatPara.Domain.Entities
{
    public class public_user_registration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long user_id_pk { get; set; }
        public string? user_name { get; set; }
        public string? mobile { get; set; }
        public string? email { get; set; }
        public string? entry_ip { get; set; }
        public short? active_status { get; set; }
        public long? create_by { get; set; }
        public DateTime? created_date { get; set; }
        public long? update_by { get; set; }
        public DateTime? update_date { get; set; }
        public bool? delete_status { get; set; }
        public long? gender { get; set; }
        public DateTime? dob { get; set; }
        public string? address { get; set; }
        public string? city { get; set; }
        public string? pin { get; set; }
    }
}
