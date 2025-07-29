namespace HallBookingBhatPara.Domain.Utility
{
    public static class FileHelper
    {
        public static async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Array.Empty<byte>();

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        // Convert byte[] to base64 string with MIME
        public static string ConvertToBase64Image(byte[] imageBytes, string contentType = "image/png")
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return null;

            return $"data:{contentType};base64,{Convert.ToBase64String(imageBytes)}";
        }

        // Combine both: file → byte[] → base64
        public static async Task<string> ConvertFileToBase64ImageAsync(IFormFile file, string contentType = "image/jpeg")
        {
            var bytes = await ConvertToByteArrayAsync(file);
            return ConvertToBase64Image(bytes, contentType);
        }
    }
}
