namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class UpdateHallAvailableDTO
    {
        public long HallId { get; set; }
        public long CategoryId { get; set; }
        public long SubcategoryId { get; set; }
        public DateOnly AvailableFrom { get; set; }
        public DateOnly AvailableTo { get; set; }
        public long ProposedRate { get; set; }
        public long SecurityMoney { get; set; }
    }
}
