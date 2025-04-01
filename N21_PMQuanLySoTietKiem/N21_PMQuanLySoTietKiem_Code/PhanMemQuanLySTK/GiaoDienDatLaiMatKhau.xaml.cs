using PhanMemQuanLySTK.Data;
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
    /// Interaction logic for GiaoDienDatLaiMatKhau.xaml
    /// </summary>
    public partial class GiaoDienDatLaiMatKhau : UserControl
    {
        public event Action<GiaoDienChinhViewModel> OnButtonClickEvent;
        private GiaoDien1 giaoDien1;

        public GiaoDienDatLaiMatKhau(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
        }

        private async void btn_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pwb_MKMoi.Password) || string.IsNullOrEmpty(pwb_MKMoi.Password))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                thongBaoLoi.ShowDialog();
            } else 
            if (pwb_MKMoi.Password != pwb_MKMoi_Nhaplai.Password)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Mật khẩu không khớp.");
                thongBaoLoi.ShowDialog();
            }
            else
            {
                var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;

                if (quenMatKhauViewModel != null)
                {
                    using (var context = new AppDbContext())
                    {
                        var username = quenMatKhauViewModel?.Username;
                        var newPassword = pwb_MKMoi.Password;

                        if (username != null && newPassword != null)
                        {
                            var user = context.Users.SingleOrDefault(u => u.Username == username);

                            if (user != null)
                            {
                                user.Password = newPassword;
                                context.SaveChanges();
                                ThongBaoThanhCong thongBaoThanhCong = new 
                                    ThongBaoThanhCong("Thành công", "Đặt lại mật khẩu thành công.");
                                thongBaoThanhCong.ShowDialog();

                                if (thongBaoThanhCong.DialogResult == true)
                                {
                                    giaoDien1.QuayLaiDangNhap();
                                }
                            }
                            else
                            {
                                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Không tồn tại người dùng.");
                                thongBaoLoi.ShowDialog();
                            }
                        }
                    }
                }
            }
        }

        private void pwb_MKMoi_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                pwb_MKMoi_Nhaplai.Focus();
            }
        }

        private async void pwb_MKMoi_Nhaplai_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (string.IsNullOrEmpty(pwb_MKMoi.Password) || string.IsNullOrEmpty(pwb_MKMoi.Password))
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                    thongBaoLoi.ShowDialog();
                }
                else
            if (pwb_MKMoi.Password != pwb_MKMoi_Nhaplai.Password)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Mật khẩu không khớp.");
                    thongBaoLoi.ShowDialog();
                }
                else
                {
                    var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;

                    if (quenMatKhauViewModel != null)
                    {
                        using (var context = new AppDbContext())
                        {
                            var username = quenMatKhauViewModel?.Username;
                            var newPassword = pwb_MKMoi.Password;

                            if (username != null && newPassword != null)
                            {
                                var user = context.Users.SingleOrDefault(u => u.Username == username);

                                if (user != null)
                                {
                                    user.Password = newPassword;
                                    context.SaveChanges();

                                    ThongBaoThanhCong thongBaoThanhCong = new 
                                        ThongBaoThanhCong("Thành công", "Đặt lại mật khẩu thành công.");
                                    thongBaoThanhCong.ShowDialog();

                                    if (thongBaoThanhCong.DialogResult == true)
                                    {
                                        giaoDien1.QuayLaiDangNhap();
                                    }
                                }
                                else
                                {
                                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Không tồn tại người dùng.");
                                    thongBaoLoi.ShowDialog();
                                }
                            }
                        }
                    }
                }
            }
        }

        private void btn_QuayLai_Click(object sender, EventArgs e)
        {
            var dangKyViewModel = this.DataContext as QuenMatKhauViewModel;

            var parent = this.Parent as ContentControl;

            if (parent != null)
            {
                GiaoDienQuenMatKhau_NhapOTP giaoDienQuenMatKhau_NhapOTP =
                     new GiaoDienQuenMatKhau_NhapOTP(giaoDien1);
                giaoDienQuenMatKhau_NhapOTP.DataContext = dangKyViewModel;
                giaoDienQuenMatKhau_NhapOTP.Margin = new Thickness(50, 280, 50, 245);
                parent.Content = giaoDienQuenMatKhau_NhapOTP;
            }
        }
    }

}
