using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienDangKiThongTin.xaml
    /// </summary>
    public partial class GiaoDienDangKiThongTin : UserControl
    {
        private GiaoDien1 giaoDien1;
        int _OTP;
        Random random = new Random();

        public GiaoDienDangKiThongTin(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
        }

        private async void btn_DangKi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_HoTen.Text) || string.IsNullOrEmpty(tb_CCCD.Text) ||
                string.IsNullOrEmpty(tb_Email.Text) || string.IsNullOrEmpty(tb_Diachi.Text) || 
                string.IsNullOrEmpty(tb_SDT.Text) || dp_DOB.SelectedDate == null || ckb_Camket.IsChecked == false)
            {
                if (ckb_Camket.IsChecked != false)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ thông tin.");
                    thongBaoLoi.ShowDialog();
                }
                else if (!string.IsNullOrEmpty(tb_HoTen.Text) && !string.IsNullOrEmpty(tb_CCCD.Text) 
                    && !string.IsNullOrEmpty(tb_Email.Text) && !string.IsNullOrEmpty(tb_Diachi.Text)
                    && !string.IsNullOrEmpty(tb_SDT.Text) && dp_DOB.SelectedDate != null)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ và cam kết thông tin.");
                    thongBaoLoi.ShowDialog();
                }
                else
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ thông tin và cam kết.");
                    thongBaoLoi.ShowDialog();
                }
            } else if (!IsValidEmail(tb_Email.Text))
            {
                ThongBaoLoi mess = new ThongBaoLoi("Email không đúng định dạng.");
                mess.ShowDialog();
            }
            else
            {
                using (var context = new AppDbContext())
                {
                    bool emailExists = context.Users
                              .Any(u => u.Email == tb_Email.Text);

                    if (emailExists)
                    {
                        ThongBaoLoi mess = new ThongBaoLoi("Email đã được đăng ký với tài khoản khác.");
                        mess.ShowDialog();
                    }
                    else
                    {

                        var parent = this.Parent as ContentControl;

                        if (parent != null)
                        {
                 

                            var dangKyViewModel = this.DataContext as DangKyViewModel;

                            if (dangKyViewModel != null)
                            {
                                GiaoDienXacThucEmail_NhapOTP giaoDienXacThucEmail_NhapOTP =
                                new GiaoDienXacThucEmail_NhapOTP(giaoDien1);
                                giaoDienXacThucEmail_NhapOTP.DataContext = dangKyViewModel;
                                giaoDienXacThucEmail_NhapOTP.Margin = new Thickness(50, 280, 50, 245);
                                parent.Content = giaoDienXacThucEmail_NhapOTP;
                            }
                            await send_OTP();

                        }
                    }
                }
            }
        }

        private async Task send_OTP()
        {
            var dangKyViewModel = this.DataContext as DangKyViewModel;

            var email = dangKyViewModel?.Email;
            if (email != null)
            {
                _OTP = random.Next(0, 1000000);
                var fromAddress = new MailAddress("finsave123@gmail.com");
                var toAddress = new MailAddress(dangKyViewModel!.Email);
                const string frompass = "sxpbhdfufikgibxh";
                string OTP = string.Format("{0:D6}", _OTP);
                const string subject = "OTP CODE";
                string body = "Mã OTP của bạn là: " + OTP.ToString();
                dangKyViewModel.OTP = OTP;
                var smtp = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, frompass),

                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                })

                {
                    await smtp.SendMailAsync(message);
                }
            }
        }

        private void btn_QuayLai_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                GiaoDienDangKi giaoDienDangKi = new GiaoDienDangKi(giaoDien1);
                giaoDienDangKi.DataContext = this.DataContext;
                giaoDienDangKi.Margin = new Thickness(50, 180, 40, 30);
                parent.Content = giaoDienDangKi;
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            Regex regex = new Regex(pattern);   
            return regex.IsMatch(email);
        }

        private void tb_HoTen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var datePickerTextBox = (dp_DOB.Template.FindName("PART_TextBox", dp_DOB) as FrameworkElement);
                var popup = datePickerTextBox?.TemplatedParent as DatePicker;

                if (popup != null)
                {
                    popup.IsDropDownOpen = true;
                }
            }
        }

        private void dp_DOB_CalendarClosed(object sender, RoutedEventArgs e)
        {
            cb_GioiTinh.IsDropDownOpen = true;
        }

        private void cb_GioiTinh_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            tb_CCCD.Focus();
        }

        private void tb_CCCD_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tb_Email.Focus();
            }
        }

        private void tb_Email_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                tb_SDT.Focus();
            }
        }

        private void tb_SDT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) 
            {
                tb_Diachi.Focus();
            }
        }
    }
}
