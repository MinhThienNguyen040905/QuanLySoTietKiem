using PhanMemQuanLySTK.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace PhanMemQuanLySTK
{
    public class XoaSo_Nhom : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value[0] is long Money && value[1] is ICollection<Group_Details> details && value[2] is string username)
            {
                if (Money == 0)
                    foreach (var p in details)
                    {
                        if (p.Username == username && p.Is_Owner == true)
                        {
                            return Visibility.Visible;
                        }
                    }
                return Visibility.Collapsed;
            }

            return Visibility.Collapsed; // Nếu không tìm thấy, trả về giá trị mặc định
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class Total_Money : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value[0] is ICollection<Group_Details> details && value[1] is string username)
            {
                foreach (var p in details)
                {
                    if (p.Username == username)
                        return p.Total_Money;
                }
            }

            return 1; // Nếu không tìm thấy, trả về giá trị mặc định
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ExcludeOwnerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable groupDetails)
            {
                // Lọc danh sách để loại bỏ trưởng nhóm
                var filtered = groupDetails
                    .OfType<Group_Details>() // Chỉ lấy các phần tử kiểu Group_Details
                    .Where(d => d.Is_Owner == false) // Loại bỏ trưởng nhóm
                    .ToList();

                return filtered;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Soluong_thanhvien_khongphainhomtruong : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Group_Details> thanhvien)
            {
                return thanhvien.Count(p => p.Is_Owner == false);
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Fullname_truongnhom : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Group_Details> thanhvien)
            {
                foreach (var tv in thanhvien)
                {
                    if (tv.Is_Owner == true) return tv.User.Fullname;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Username_truongnhom : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Group_Details> thanhvien)
            {
                foreach (var tv in thanhvien)
                {
                    if (tv.Is_Owner == true) return tv.Username;
                }
                return string.Empty;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Avatar_truongnhom : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection<Group_Details> thanhvien)
            {
                foreach (var tv in thanhvien)
                {
                    if (tv.Is_Owner == true) return tv.User.Avatar;
                }
                return string.Empty;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Thutrongtuan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "Sunday", "Chủ nhật" },
                    { "Monday", "Thứ hai" },
                    { "Tuesday", "Thứ ba" },
                    { "Wednesday", "Thứ tư" },
                    { "Thursday", "Thứ năm" },
                    { "Friday", "Thứ sáu" },
                    { "Saturday", "Thứ bảy" }
                };
                return dic[dt.DayOfWeek.ToString()];
            }
            return "Lỗi";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TargetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "Không có";
            }

            return string.Format("{0:N0} VNĐ", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SoluongConverter_Lonhon0 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is long soluong)
            {
                return soluong > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TwoTextsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is string str1 && values[1] is string str2)
            {
                string result = str1 + "\n(" + str2 + ")";
                return result;
            }
            return string.Empty;
        }

        public object[] ConvertBack(object values, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Xacnhan_thongbaoNhom : IValueConverter
    {
        public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            if (obj is string str)
            {
                return str == "Đồng ý" || str == "Không đồng ý"; //tuc la da xac nhan
            }
            return false;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Xacnhan_thongbaoNhom_Dongy : IValueConverter
    {
        public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            if (obj is string str)
            {
                return str == "Đồng ý";
            }
            return false;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Xacnhan_thongbaoNhom_khongDongy : IValueConverter
    {
        public object Convert(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            if (obj is string str)
            {
                return str == "Không đồng ý";
            }
            return false;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class mota_Kyhan : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                string tmp = str.Substring(7, str.IndexOf("L") - 8);
                return tmp;
            }
            return string.Empty;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class mota_Laisuat : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                int begin = str.IndexOf("-") + 11;
                int length = str.IndexOf("-", begin + 1) - begin;
                string tmp = str.Substring(begin, length);
                return tmp;
            }
            return string.Empty;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class mota_Mota : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                int tmp = str.IndexOf("-") + 11;
                int begin = str.IndexOf("-", tmp + 1);
                string str_tmp = str.Substring(begin + 1);
                return str_tmp;
            }
            return string.Empty;
        }

        public object ConvertBack(object obj, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class Chao : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime.TimeOfDay >= new TimeSpan(5, 0, 0) && dateTime.TimeOfDay <= new TimeSpan(11, 59, 59)) return "Chào buổi sáng,";
                if (dateTime.TimeOfDay >= new TimeSpan(12, 0, 0) && dateTime.TimeOfDay <= new TimeSpan(13, 59, 59)) return "Chào buổi trưa,";
                if (dateTime.TimeOfDay >= new TimeSpan(14, 0, 0) && dateTime.TimeOfDay <= new TimeSpan(17, 59, 59)) return "Chào buổi chiều,";
                if (dateTime.TimeOfDay >= new TimeSpan(18, 0, 0) && dateTime.TimeOfDay <= new TimeSpan(23, 59, 59)) return "Chào buổi tối,";
            }
            return "Xin chào,";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyCollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                return collection.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                return dateTime.ToString("dd/MM/yyyy");
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmptyItemsVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value as int?;
            return count.HasValue && count.Value == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NullOrEmptyToDefaultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Kiểm tra giá trị nếu là null hoặc chuỗi rỗng
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return "Không có";  // Thay thế với chuỗi mặc định
            }
            return value;  // Trả về giá trị gốc nếu không phải null hoặc rỗng
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();  // Không cần xử lý ConvertBack trong trường hợp này
        }
    }

    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double number)
            {
                return $"{number:N0} VNĐ"; // Định dạng số và thêm "VNĐ"
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // Không cần thiết trong trường hợp này
        }
    }

    public class Group_Transaction_User : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Users user)
            {
                if (user != null)
                    return user.Fullname;
                else
                {
                    return "FinSave";
                }
            }
            return "FinSave";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
