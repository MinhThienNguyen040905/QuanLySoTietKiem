using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.Utils;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienDangKi.xaml
    /// </summary>
    public partial class GiaoDienDangKi : UserControl
    {

        private GiaoDien1 giaoDien1;

        public GiaoDienDangKi(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.FlowDirection = FlowDirection.LeftToRight;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var viewModel =  DataContext as DangKyViewModel;
                if (viewModel != null)
                {
                    viewModel.Password = passwordBox.Password;
                }
            }
        }

        private void PasswordBox_ConfirmPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var viewModel = DataContext as DangKyViewModel;
                if (viewModel != null)
                {
                    viewModel.ConfirmPassword = passwordBox.Password;
                }
            }
        }

        private void btn_QuayLai_Click(object sender, RoutedEventArgs e)
        {
            giaoDien1.QuayLaiDangNhap();
        }

        private void btn_TiepTuc_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwb_TenDangNhap.Text) || string.IsNullOrEmpty(pwb_MatKhau.Password))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ thông tin.");
                thongBaoLoi.ShowDialog();

            } else if (pwb_MatKhau.Password != pwb_MatKhau_Nhaplai.Password)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Mật khẩu không khớp.");
                thongBaoLoi.ShowDialog();
            } else if (!Validator.IsValidUsername(pwb_TenDangNhap.Text))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập tên đăng nhập theo đúng định dạng.");
                thongBaoLoi.ShowDialog();
            } else if (!Validator.IsValidPassword(pwb_MatKhau.Password))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng đặt mật khẩu theo đúng định dạng.");
                thongBaoLoi.ShowDialog();
            }
            else
            {
                var dangKyViewModel = this.DataContext  as DangKyViewModel;

                if (dangKyViewModel != null)
                {
                    var username = dangKyViewModel?.Username;

                    if (username != null)
                    {
                        using (var context = new AppDbContext())
                        {
                            bool userExists = context.Users.Any(u => u.Username == username);

                            if (userExists)
                            {
                                ThongBaoLoi mess = new 
                                    ThongBaoLoi("Tên người dùng đã tồn tại trong hệ thống. Vui lòng thử tên khác.");
                                mess.ShowDialog();
                            } else
                            {
                                var parent = this.Parent as ContentControl;
                                if (parent != null)
                                {
                                    GiaoDienDangKiThongTin giaoDienDangKiThongTin = new GiaoDienDangKiThongTin(giaoDien1);
                                    giaoDienDangKiThongTin.DataContext = dangKyViewModel;
                                    giaoDienDangKiThongTin.Margin = new Thickness(50, 180, 30, 120);
                                    parent.Content = giaoDienDangKiThongTin;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void pwb_TenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pwb_MatKhau.Focus();
            }
        }

        private void pwb_MatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pwb_MatKhau_Nhaplai.Focus();
            }
        }

        private void pwb_MatKhau_Nhaplai_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btn_TiepTuc_Click(sender, new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }

}
