using Microsoft.EntityFrameworkCore;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.Utils;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienDangNhap.xaml
    /// </summary>
    public partial class GiaoDienDangNhap : UserControl
    {
        private GiaoDien1 giaoDien1;
        private int login_number = 5;
        private DispatcherTimer timer;
        private int demNguoc;

        public int DemNguoc
        {
            get => demNguoc;
            set
            {
                demNguoc = value;
                OnPropertyChanged(nameof(DemNguoc));
            }
        }

        public GiaoDienDangNhap(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            DemNguoc = 120;
            this.DataContext = new GiaoDienChinhViewModel();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            this.giaoDien1 = giaoDien1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (DemNguoc > 0)
            {
                DemNguoc--;
                thongbao.Content = $"Bạn phải chờ {DemNguoc} giây để đăng nhập lại.";
            }
            else
            {
                timer.Stop();
                btn_DangNhap.IsEnabled = true;
                thongbao.Content = "Bạn có thể đăng nhập lại.";
            }
        }

        private void tbl_DangKi_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var giaoDienDangKy = new GiaoDienDangKi(giaoDien1);
            giaoDienDangKy.Margin = new Thickness(50, 180, 40, 40);
            giaoDien1.GiaoDien.Content = giaoDienDangKy;
        }

        private void tbl_QuenMK_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            
            if (parent != null)
            {
                GiaoDienQuenMatKhau quenMatKhau = new GiaoDienQuenMatKhau(giaoDien1);
                quenMatKhau.DataContext = new QuenMatKhauViewModel();
                quenMatKhau.Margin = new Thickness(50, 280, 50, 270);
                parent.Content = quenMatKhau;
            }
        }

        private void tbl_QuenMK_MouseEnter(object sender, MouseEventArgs e)
        {
            tbl_QuenMK.TextDecorations = TextDecorations.Underline;
        }

        private void tbl_QuenMK_MouseLeave(object sender, MouseEventArgs e)
        {
            tbl_QuenMK.TextDecorations = null;
        }

        private void tbl_DangKi_MouseEnter(object sender, MouseEventArgs e)
        {
            tbl_DangKi.TextDecorations = TextDecorations.Underline;
        }

        private void tbl_DangKi_MouseLeave(object sender, MouseEventArgs e)
        {
            tbl_DangKi.TextDecorations = null;
        }

        public void btn_DangNhap_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_tenDangNhap.Text) || string.IsNullOrEmpty(pwb_MatKhau.Password))
            {
                ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                mess.ShowDialog();
            } else
            {
                using (var context = new AppDbContext())
                {                                                                                                                                                     
                    var user = context.Users.FirstOrDefault(u => u.Username == tb_tenDangNhap.Text);
                    if (user == null || pwb_MatKhau.Password != user.Password)
                    {
                        login_number--;
                        thongbao.Content = $"Tên đăng nhập hoặc mật khẩu không chính xác. " +
                            $"Bạn còn {login_number} lần đăng nhập lại.";
                        thongbao.Visibility = Visibility.Visible;
                        this.Margin = new Thickness(50, 280, 50, 240);
                        Grid3.Height = 100;

                        if (login_number <= 0)
                        {
                            btn_DangNhap.IsEnabled = false;
                            DemNguoc = 120;
                            timer.Start();
                            login_number = 5;
                        }
                    }
                    else
                    {
                        thongbao.Visibility = Visibility.Collapsed;
                        this.Margin = new Thickness(50, 280, 50, 270);

                        var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;

                        if (giaoDienChinhViewModel != null)
                        {
                            var username = giaoDienChinhViewModel.Username;

                            giaoDienChinhViewModel.LoadUserInfoAsync(username);
                            giaoDienChinhViewModel.LoadPersonalNotifications(username);
                            giaoDienChinhViewModel.LoadGroupSavingsAccounts(username);
                            giaoDienChinhViewModel.LoadPersonalSavingsAccounts(username);

                            OnButtonClickEvent?.Invoke(giaoDienChinhViewModel);
                        }
                    }
                }
            }
        }

        public event Action<GiaoDienChinhViewModel> OnButtonClickEvent;

        private void tb_tenDangNhap_KeyDown(object sender, KeyEventArgs e)
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
                btn_DangNhap_Click(sender, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}