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
    /// Interaction logic for GiaoDienThemThanhVien.xaml
    /// </summary>
    public partial class GiaoDienThemThanhVien : Window
    {
        private AppDbContext _dbContext;
        private string _currentUser;

        public GiaoDienThemThanhVien(string currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _dbContext = new AppDbContext();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        private void tb_Username_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra nếu phím nhấn là Enter
            if (e.Key == Key.Enter)
            {
                string enteredUsername = tb_Username.Text.Trim();

                // Xóa thông báo lỗi trước đó
                lb_Thongbao.Visibility = Visibility.Hidden;

                // Nếu nhập dữ liệu thì tìm kiếm trong CSDL
                if (!string.IsNullOrEmpty(enteredUsername))
                {
                    var user = _dbContext.Users.FirstOrDefault(u => u.Username == enteredUsername);

                    if (user != null)
                    {
                        tb_Hoten.Text = user.Fullname; // Hiển thị họ tên
                    }
                    else
                    {
                        tb_Hoten.Text = string.Empty; // Xóa họ tên nếu không tìm thấy
                        ThongBaoLoi thongBao = new ThongBaoLoi("Không tìm thấy người dùng.");
                        thongBao.ShowDialog();
                    }
                }
                else
                {
                    tb_Hoten.Text = string.Empty; // Xóa họ tên nếu Username rỗng
                }
            }
        }


        private bool IsTextBoxEmpty(System.Windows.Controls.TextBox textBox)
        {
            return string.IsNullOrWhiteSpace(textBox.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string enteredUsername =  tb_Username.Text.Trim();
            string displayedFullname = tb_Hoten.Text;

            // Kiểm tra dữ liệu có rỗng không
            if (IsTextBoxEmpty(tb_Username))
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Vui lòng nhập tên người dùng.");
                thongBao.ShowDialog();
                return;
            }

            if (tb_Hoten.Text == "" && tb_Username.Text != "")
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Không tìm thấy người dùng nào.");
                thongBao.ShowDialog();
                return;
            }

            // Kiểm tra Username của người tạo
            if (tb_Username.Text == _currentUser)
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Không thể thêm chính bạn vào danh sách.");
                thongBao.ShowDialog();
                return;
            }

            // Lấy ViewModel hiện tại
            var taoSoTietKiemNhom = this.DataContext as GiaoDienChinhViewModel;

            if (taoSoTietKiemNhom == null) return;

            using (var context = new AppDbContext())
            {
                // Tìm kiếm người dùng trong CSDL
                var user = context.Users.FirstOrDefault(u => u.Username == enteredUsername);

                if (user != null)
                {
                    // Kiểm tra người dùng đã có trong danh sách chưa
                    if (!taoSoTietKiemNhom.ItemsUsers.Any(u => u.Username == user.Username))
                    {
                        taoSoTietKiemNhom.ItemsUsers.Add(user);
                        lb_Thongbao.Visibility = Visibility.Visible;
                        lb_Thongbao.Content = $"Đã thêm {user.Username} thành công.";
                        lb_Thongbao.Foreground = System.Windows.Media.Brushes.Green;

                        // Reset TextBox
                        tb_Username.Text = string.Empty;
                        tb_Hoten.Text = string.Empty;
                    }
                    else
                    {
                        ThongBaoLoi thongBao = new ThongBaoLoi("Người dùng đã có trong danh sách.");
                        thongBao.ShowDialog();
                    }
                }
                else
                {
                    ThongBaoLoi thongBao = new ThongBaoLoi("Không tìm thấy người dùng.");
                    thongBao.ShowDialog();
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
