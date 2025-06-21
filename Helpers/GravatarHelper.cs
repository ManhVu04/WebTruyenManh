using System.Security.Cryptography;
using System.Text;

namespace WebTruyenHay.Helpers
{
    public static class GravatarHelper
    {
        /// <summary>
        /// Tạo URL Gravatar từ email
        /// </summary>
        /// <param name="email">Email của người dùng</param>
        /// <param name="size">Kích thước avatar (mặc định 40px)</param>
        /// <param name="defaultImage">Hình mặc định khi không có Gravatar</param>
        /// <returns>URL của Gravatar</returns>
        public static string GetGravatarUrl(string email, int size = 40, string defaultImage = "identicon")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return GetDefaultAvatarUrl(size);
            }

            // Chuyển email thành lowercase và trim
            email = email.Trim().ToLowerInvariant();
            
            // Tạo MD5 hash từ email
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(email);
                var hashBytes = md5.ComputeHash(inputBytes);
                
                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                
                var hash = sb.ToString();
                
                // Tạo URL Gravatar
                return $"https://www.gravatar.com/avatar/{hash}?s={size}&d={defaultImage}&r=pg";
            }
        }
        
        /// <summary>
        /// Tạo URL avatar mặc định
        /// </summary>
        /// <param name="size">Kích thước avatar</param>
        /// <returns>URL của avatar mặc định</returns>
        public static string GetDefaultAvatarUrl(int size = 40)
        {
            // Sử dụng một default avatar từ Gravatar với identicon
            return $"https://www.gravatar.com/avatar/00000000000000000000000000000000?s={size}&d=identicon&r=pg";
        }
        
        /// <summary>
        /// Tạo avatar từ tên người dùng (lấy chữ cái đầu)
        /// </summary>
        /// <param name="name">Tên người dùng</param>
        /// <param name="size">Kích thước avatar</param>
        /// <returns>URL của avatar UI Avatars</returns>
        public static string GetInitialsAvatarUrl(string name, int size = 40)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetDefaultAvatarUrl(size);
            }
            
            // Lấy chữ cái đầu của tên
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var initials = "";
            
            if (words.Length >= 2)
            {
                initials = $"{words[0][0]}{words[1][0]}".ToUpperInvariant();
            }
            else if (words.Length == 1)
            {
                initials = words[0].Length >= 2 ? 
                    $"{words[0][0]}{words[0][1]}".ToUpperInvariant() : 
                    $"{words[0][0]}".ToUpperInvariant();
            }
            else
            {
                initials = "U";
            }
            
            // Sử dụng UI Avatars service để tạo avatar từ initials
            return $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(initials)}&size={size}&background=6f42c1&color=ffffff&bold=true&format=png";
        }
    }
}
