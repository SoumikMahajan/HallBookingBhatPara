namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class UpdateHallAvailableDTO
    {
        public long HallAvailId { get; set; }
        public long CategoryId { get; set; }
        public long SubcategoryId { get; set; }
        public long PaymentTypeId { get; set; }
        public DateOnly AvailableFrom { get; set; }
        public DateOnly AvailableTo { get; set; }
        public long ProposedRate { get; set; }
        public long SecurityMoney { get; set; }
        public long FloorId { get; set; }
        public UserClaims userClaims { get; set; } = new UserClaims();
    }
}
