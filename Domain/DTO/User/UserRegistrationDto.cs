namespace HallBookingBhatPara.Domain.DTO.User
{
    public class UserRegistrationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long Gender { get; set; }
        public DateOnly DOB { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string Password { get; set; }
        public string? BasePassword { get; set; }
        public IFormFile? ProfilePic { get; set; }

        //extra fields
        public byte[]? ImageData { get; set; }
        public string? EntryIP { get; set; }
        public long? CreatedBy { get; set; }
    }
}
