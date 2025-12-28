namespace HallBookingBhatPara.Domain.DTO
{
    public class DropDownListDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
    }

    public class UserListDTO
    {
        public long user_id_pk { get; set; } = 0;
        public string user_name { get; set; } = string.Empty;
		public string mobile { get; set; } = string.Empty;
		public string email { get; set; } = string.Empty;		
	}

    public class UserListForAdminDTO
    {
		public long user_id_pk { get; set; }
		public string user_name { get; set; } = string.Empty;
		public string mobile { get; set; } = string.Empty;
		public string email { get; set; } = string.Empty;
		public long gender { get; set; } = 0;
		public string dob { get; set; } = string.Empty;
		public string address { get; set; } = string.Empty;
		public string city { get; set; } = string.Empty;
		public string pin { get; set; } = string.Empty;
		public string role_name { get; set; } = string.Empty;
		public int role_id { get; set; }
	}
	public class UserDetailsForAdminDTO
	{
		public long user_id_pk { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string mobile { get; set; } = string.Empty;
		public string email { get; set; } = string.Empty;
		public long gender { get; set; } = 0;
		public string dob { get; set; } = string.Empty;
		public string address { get; set; } = string.Empty;
		public string city { get; set; } = string.Empty;
		public string pin { get; set; } = string.Empty;
		public string role_name { get; set; } = string.Empty;
		public int role_id { get; set; }
	}
}
