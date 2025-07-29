namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class InsertSubCategoryDTO
    {
        public long CategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public IFormFile? fileUpload { get; set; }
        public long CreatedBy { get; set; }
        //extra fields
        public byte[]? ImageData { get; set; }
    }
}
