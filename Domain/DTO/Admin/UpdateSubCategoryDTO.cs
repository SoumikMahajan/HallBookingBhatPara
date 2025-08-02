namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class UpdateSubCategoryDTO
    {
        public long Subcategoryid { get; set; }
        public long CategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public bool HasNewImage { get; set; }
        public IFormFile? fileUpload { get; set; }
        //extra fields
        public byte[]? ImageData { get; set; }
    }
}
