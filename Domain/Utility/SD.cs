namespace HallBookingBhatPara.Domain.Utility
{
    public class SD
    {
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public static string AccessToken = "Token";
        public static string UserId = "";
        public static string RoleName = "role";
        public enum ContentType
        {
            Json,
            MultipartFormData,
        }
    }
}
