using System.Security.Cryptography;

namespace HallBookingBhatPara.Utility
{
    public class BookingIdGenerator
    {
        public static string Generate()
        {
            // Generate random 6 bytes = ~10 Base32 chars
            byte[] randomBytes = RandomNumberGenerator.GetBytes(6);

            string base32 = Base32Encode(randomBytes);

            // Trim to desired length (e.g. 8 chars)
            return $"#BK{base32[..8]}";
        }

        private static string Base32Encode(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            int bits = 0, value = 0;
            var result = new System.Text.StringBuilder();

            foreach (var b in data)
            {
                value = (value << 8) | b;
                bits += 8;
                while (bits >= 5)
                {
                    result.Append(alphabet[(value >> (bits - 5)) & 31]);
                    bits -= 5;
                }
            }

            if (bits > 0)
                result.Append(alphabet[(value << (5 - bits)) & 31]);

            return result.ToString();
        }
    }
}
