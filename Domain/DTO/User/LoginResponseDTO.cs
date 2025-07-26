using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.DTO.User
{
    public class LoginResponseDTO
    {
        [Key]
        public long stake_holder_login_id_pk { get; set; }
        public long stake_id_fk { get; set; }
        public string login_id { get; set; }
        public string login_password { get; set; }
        public short active_status { get; set; }
        public DateTime? entry_time { get; set; }
        public DateTime? update_time { get; set; }
        public string? entry_ip { get; set; }
        public string stake_holder_details { get; set; }
        public long stake_details_id_fk { get; set; }
        public string? base_password { get; set; }
        public string? RedirectUrl { get; set; } = string.Empty;
        //public UserClaims? User { get; set; }
    }
}
