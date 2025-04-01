using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.Utils
{
    public class Validator
    {
        public static bool IsValidUsername(string username)
        {
            string pattern = "^[a-zA-Z0-9]+$";

            return Regex.IsMatch(username, pattern);
        }

        public static bool IsValidPassword(string password)
        {
            // Kiểm tra độ dài từ 8 đến 20 ký tự
            if (password.Length < 8 || password.Length > 20)
                return false;

            // Kiểm tra không chứa khoảng trắng
            if (password.Contains(" "))
                return false;

            // Kiểm tra không chứa ký tự tiếng Việt
            string vietnamesePattern = "[áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ]";
            if (Regex.IsMatch(password, vietnamesePattern))
                return false;

            // Kiểm tra ít nhất 1 chữ cái viết thường (a-z)
            if (!Regex.IsMatch(password, "[a-z]"))
                return false;

            // Kiểm tra ít nhất 1 chữ cái viết hoa (A-Z)
            if (!Regex.IsMatch(password, "[A-Z]"))
                return false;

            // Kiểm tra ít nhất 1 chữ số (0-9)
            if (!Regex.IsMatch(password, "[0-9]"))
                return false;

            // Kiểm tra ít nhất 1 ký tự đặc biệt (!,@,#,$,%,&,*,...)
            if (!Regex.IsMatch(password, "[!@#$%&*]"))
                return false;

            // Nếu tất cả các điều kiện đều thỏa mãn
            return true;
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }
    }
}
