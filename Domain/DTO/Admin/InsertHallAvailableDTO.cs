namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class InsertHallAvailableDTO
    {
        public long CategoryId { get; set; }
        public long SubcategoryId { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableTo { get; set; }
        public long ProposedRate { get; set; }
        public long SecurityMoney { get; set; }
        public UserClaims userClaims { get; set; } = new UserClaims();
    }
}
