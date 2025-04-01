using LiveCharts;
using Microsoft.Win32;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PhanMemQuanLySTK.Utils;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienThongTinNguoiDung.xaml
    /// </summary>
    public partial class GiaoDienThongTinNguoiDung : UserControl
    {
        private GiaoDien1 giaoDien1;

        public GiaoDienThongTinNguoiDung(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
            PieSeriesPersonalSavings.LabelPoint = chartPoint => $"{chartPoint.Y:N0} VNĐ";
            PieSeriesGroupSavings.LabelPoint = chartPoint => $"{chartPoint.Y:N0} VNĐ";
            soluong.LabelFormatter = value => value.ToString("0");

        }

        private void btn_Home_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (parent != null && viewModel != null)
            {
                var giaoDienChinh = new GiaoDienChinh(giaoDien1);
                giaoDienChinh.DataContext = viewModel;
                parent.Margin = new Thickness(0, 3, 0, 0);
                parent.Content = giaoDienChinh;
            }
        }

        private void btn_DangXuat_Click(object sender, RoutedEventArgs e)
        {
            var giaoDien1 = new GiaoDien1();
            giaoDien1.DataContext = this.DataContext as GiaoDienChinhViewModel;
            (this.Parent as ContentControl).Content = giaoDien1;
        }

        private void btn_DoiAnh_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                var uploader = new CloudinaryUploader();

                string selectedImagePath = openFileDialog.FileName;

                string imageUrl = uploader.UploadImage(selectedImagePath);

                var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                if (username != "" && imageUrl != "")
                {
                    SaveImageToDatabase(username, imageUrl);
                }
            }
        }

        private void SaveImageToDatabase(string username, string imagePath)
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == username);
                    var viewModel = this.DataContext as GiaoDienChinhViewModel;

                    if (user != null && viewModel != null)
                    {
                        user.Avatar = imagePath;
                        context.SaveChanges();

                        viewModel.User.Avatar = imagePath;
                        viewModel.OnPropertyChanged(nameof(viewModel.User));
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy người dùng.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        private void btn_ThayDoi_Click(object sender, RoutedEventArgs e)
        {
            var username = (this.DataContext as GiaoDienChinhViewModel).Username;
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            if (username != "" && viewModel != null)
            {
                using (var context = new AppDbContext())
                {
                    var user = context.Users.FirstOrDefault(p => p.Username == username);
                    if (user != null)
                    {
                        viewModel.TmpUser = user;
                    }
                }
            }

            btn_Luu.IsEnabled = true; 
            btn_Huy.Opacity = 100;
            btn_Huy.IsEnabled = true;
            tb_Cccd.IsReadOnly = false;
            tb_DiaChi.IsReadOnly = false;
            tb_Dienthoai.IsReadOnly = false;
            tb_Email.IsReadOnly = false;
            dp_NgaySinh.IsEnabled = true;
            cb_GioiTinh.IsHitTestVisible = true;
            tb_HoTen.IsReadOnly = false;
            btn_Luu.Visibility = Visibility.Visible;
            btn_Thaydoi.Visibility = Visibility.Collapsed;
            btn_ThayDoiMK.IsEnabled = false;
        }

        private async void btn_Luu_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_HoTen.Text) || string.IsNullOrEmpty(cb_GioiTinh.Text) || string.IsNullOrEmpty(tb_DiaChi.Text)
                || string.IsNullOrEmpty(tb_Dienthoai.Text) || string.IsNullOrEmpty(tb_Email.Text) || 
                string.IsNullOrEmpty(dp_NgaySinh.Text) 
                || string.IsNullOrEmpty(tb_Cccd.Text))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ thông tin.");
                thongBaoLoi.ShowDialog();
                return;
            } else if (!IsValidEmail(tb_Email.Text))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đúng định dạng email.");
                thongBaoLoi.ShowDialog();
                return;
            }
            {
                var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                if (username != "")
                {
                    if (checkEmailExists(tb_Email.Text, username))
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Email đã được sử dụng bởi tài khoản khác.");
                        thongBaoLoi.ShowDialog();
                        return;
                    }
                }

                btn_ThayDoiMK.IsEnabled = true;

                using (var context = new AppDbContext())
                {
                    if (username != "")
                    {
                        var user = context.Users.FirstOrDefault(u => u.Username == username);

                        if (user != null)
                        {
                            if (tb_Email.Text != user.Email)
                            {
                                var giaoDienXacThuc = new ThayDoiTT_XacThucEmail(tb_Email.Text);
                                //await send_OTP(tb_Email.Text);
                                giaoDienXacThuc.DataContext = this.DataContext as GiaoDienChinhViewModel;
                                giaoDienXacThuc.ShowDialog();
                            } else
                            {
                                user.Fullname = tb_HoTen.Text;
                                user.Gender = cb_GioiTinh.Text;
                                user.Email = tb_Email.Text;
                                user.Address = tb_DiaChi.Text;
                                user.Dob = dp_NgaySinh.SelectedDate.Value;
                                user.Identity_Card = tb_Cccd.Text;
                                user.Phone = tb_Dienthoai.Text;

                                context.SaveChanges();

                                var viewModel = this.DataContext as GiaoDienChinhViewModel;

                                if (viewModel != null)
                                {
                                    viewModel.User = user;
                                    viewModel.ReadOnlyFullname = user.Fullname;
                                    viewModel.OnPropertyChanged(nameof(viewModel.User));
                                }

                                tb_Cccd.IsReadOnly = true;
                                tb_DiaChi.IsReadOnly = true;
                                tb_Dienthoai.IsReadOnly = true;
                                tb_Email.IsReadOnly = true;
                                dp_NgaySinh.IsEnabled = false;
                                cb_GioiTinh.IsHitTestVisible = false;
                                tb_HoTen.IsReadOnly = true;
                                btn_Luu.Visibility = Visibility.Collapsed;
                                btn_Thaydoi.Visibility = Visibility.Visible;
                            }
                            
                            var newEmail = (this.DataContext as GiaoDienChinhViewModel)?.NewEmail;

                            if (newEmail != null && dp_NgaySinh.SelectedDate.HasValue && newEmail != "")
                            {
                                var viewModel = this.DataContext as GiaoDienChinhViewModel;

                                user.Fullname = tb_HoTen.Text;
                                user.Gender = cb_GioiTinh.Text;
                                user.Email = newEmail;
                                user.Address = tb_DiaChi.Text;
                                user.Dob = dp_NgaySinh.SelectedDate.Value;
                                user.Identity_Card = tb_Cccd.Text;
                                user.Phone = tb_Dienthoai.Text;

                                context.SaveChanges();

                                if (viewModel != null)
                                {
                                    viewModel.User = user;
                                    viewModel.ReadOnlyFullname = user.Fullname;
                                    viewModel.OnPropertyChanged(nameof(viewModel.User));
                                }
                                
                                tb_Cccd.IsReadOnly = true;
                                tb_DiaChi.IsReadOnly = true;
                                tb_Dienthoai.IsReadOnly = true;
                                tb_Email.IsReadOnly = true;
                                dp_NgaySinh.IsEnabled = false;
                                cb_GioiTinh.IsHitTestVisible = false;
                                tb_HoTen.IsReadOnly = true;
                                btn_Luu.Visibility = Visibility.Collapsed;
                                btn_Thaydoi.Visibility = Visibility.Visible;
                            }
                        }
                    }
                }
                btn_Huy.Opacity = 0;
                btn_Huy.IsEnabled = false;
            }
            Keyboard.ClearFocus();
        }

        private bool checkEmailExists(string email, string username)
        {
            using (var context = new AppDbContext())
            {
                return context.Users.Any(u => u.Email == email && u.Username != username);
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        private async void btn_ThayDoiMK_Click(object sender, EventArgs e)
        {
            ThongBaoXoa thongBaoXoa = new ThongBaoXoa("Bạn có chắc chắn muốn thay đổi mật khẩu hiện tại?");
            thongBaoXoa.ShowDialog();

            if (thongBaoXoa.DialogResult == true)
            {
                var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;
                if (giaoDienChinhViewModel != null)
                {
                    GiaoDienThayDoiMatKhau giaoDienThayDoiMatKhau = new GiaoDienThayDoiMatKhau(giaoDien1);
                    giaoDienThayDoiMatKhau.DataContext = giaoDienChinhViewModel;
                    giaoDienThayDoiMatKhau.ShowDialog();

                    if (giaoDienThayDoiMatKhau.Tag != null)
                    {
                        if (giaoDienThayDoiMatKhau.Tag.ToString() == "True")
                        {
                            var parent = this.Parent as ContentControl;
                            if (parent != null)
                            {
                                parent.Content = new GiaoDien1();
                                await Task.Delay(500);
                                ThongBaoThanhCong thongBaoThanhCong = new ThongBaoThanhCong("ĐĂNG NHẬP LẠI", 
                                    "Vui lòng đăng nhập" +
                                    " lại.");
                                thongBaoThanhCong.ShowDialog();
                            }
                        }
                    }
                }
            }
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            if (viewModel != null)
            {
                var tmpUser = viewModel.TmpUser;
                var user = viewModel.User;

                if (user != null && tmpUser != null)
                {
                    user.Fullname = tmpUser.Fullname;
                    user.Address = tmpUser.Address;
                    user.Password = tmpUser.Password;
                    user.Identity_Card = tmpUser.Identity_Card;
                    user.Username = tmpUser.Username;
                    user.Avatar = tmpUser.Avatar;   
                    user.Dob = tmpUser.Dob;
                    user.Email = tmpUser.Email;
                    user.Phone = tmpUser.Phone;
                    user.Gender = tmpUser.Gender;

                    viewModel.OnPropertyChanged(nameof(viewModel.User));

                    tb_Cccd.IsReadOnly = true;
                    tb_DiaChi.IsReadOnly = true;
                    tb_Dienthoai.IsReadOnly = true;
                    tb_Email.IsReadOnly = true;
                    dp_NgaySinh.IsEnabled = false;
                    cb_GioiTinh.IsHitTestVisible = false;
                    tb_HoTen.IsReadOnly = true;
                }

                btn_ThayDoiMK.IsEnabled = true;
                btn_Huy.Opacity = 0;
                btn_Huy.IsEnabled = false;
                btn_Thaydoi.IsEnabled = true;
                btn_Thaydoi.Visibility = Visibility.Visible;
                btn_Luu.IsEnabled = false;
                btn_Luu.Visibility = Visibility.Collapsed;
            }
        }
    }
}
