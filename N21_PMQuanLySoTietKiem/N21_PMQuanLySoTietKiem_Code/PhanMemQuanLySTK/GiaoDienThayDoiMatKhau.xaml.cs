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
using System.Windows.Shapes;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienThayDoiMatKhau.xaml
    /// </summary>
    public partial class GiaoDienThayDoiMatKhau : Window
    {
        private GiaoDien1 giaoDien1;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        public GiaoDienThayDoiMatKhau(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
        }

        private void btn_HuyBo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_XacNhan_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(pw_mkHienTai.Password) || string.IsNullOrEmpty(pw_mkMoi.Password)
                || string.IsNullOrEmpty(pw_NhapLaiMK.Password))
            {

                ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập đủ thông tin.");
                mess.ShowDialog();

            } else if (pw_NhapLaiMK.Password != pw_mkMoi.Password)
            {

                ThongBaoLoi mess = new ThongBaoLoi("Mật khẩu nhập lại không khớp.");
                mess.ShowDialog();
            } else if (pw_mkMoi.Password == pw_mkHienTai.Password)
            {
                ThongBaoLoi mess = new ThongBaoLoi("Mật khẩu mới phải khác so với mật khẩu hiện tại.");
                mess.ShowDialog();
            } else 
            {
                using (var context = new AppDbContext())
                {
                    var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                    if (username != "")
                    {
                        var user = context.Users.FirstOrDefault(u => u.Username == username);

                        if (user != null && user.Password != pw_mkHienTai.Password)
                        {
                            ThongBaoLoi mess = new ThongBaoLoi("Mật khẩu hiện tại không đúng.");
                            mess.ShowDialog();

                        } else if (user != null)
                        {
                            var result = MessageBox.Show(
                                    "Bạn có chắc chắn muốn thay đổi mật khẩu không?",
                                    "Xác nhận",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Warning
                            );

                            if (result == MessageBoxResult.Yes)
                            {
                                user.Password = pw_mkMoi.Password;
                                context.SaveChanges();
                                this.Tag = true;
                                this.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
