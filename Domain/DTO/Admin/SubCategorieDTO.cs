using System.ComponentModel.DataAnnotations;

namespace HallBookingBhatPara.Domain.DTO.Admin
{
    public class SubCategorieDTO
    {
        [Key]
        public int RowNumber { get; set; }
        public long hall_id_pk { get; set; }
        public string? category_name { get; set; }
        public string? hall_name { get; set; }
        public byte[]? hall_image { get; set; }
        public short? active_status { get; set; }

        public string? hall_image_base64 { get; set; }
    }
}
