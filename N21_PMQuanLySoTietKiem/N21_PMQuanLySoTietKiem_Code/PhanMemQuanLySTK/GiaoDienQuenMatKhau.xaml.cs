using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using PhanMemQuanLySTK.Utils;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienQuenMatKhau.xaml
    /// </summary>
    public partial class GiaoDienQuenMatKhau : UserControl
    {
        private GiaoDien1 giaoDien1;
        int _OTP;
        Random random = new Random();

        public GiaoDienQuenMatKhau(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;
        }

        private void btn_QuayLai_Click(object sender, RoutedEventArgs e)
        {
            giaoDien1.QuayLaiDangNhap();
        }

        private async Task send_OTP()
        {
            var quenMatKhau = (QuenMatKhauViewModel)this.DataContext;

            _OTP = random.Next(0, 1000000);
            var fromAddress = new MailAddress("finsave123@gmail.com");
            var toAddress = new MailAddress(quenMatKhau.Email);
            const string frompass = "sxpbhdfufikgibxh";
            string OTP = string.Format("{0:D6}", _OTP);
            quenMatKhau.OTP = OTP;
            const string subject = "OTP CODE VERIFICATION";
            string body = "Mã OTP của bạn là: " + OTP.ToString() + " . Vui lòng nhập mã này để lấy lại mật khẩu.";
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

        private async void btn_GuiMa_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_TenDangnhap.Text) || string.IsNullOrEmpty(tb_Email.Text))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                thongBaoLoi.ShowDialog();
            } else if (!Validator.IsValidEmail(tb_Email.Text))
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Email không đúng định dạng.");
                thongBaoLoi.ShowDialog();
            } else 
            {
                using (var context = new AppDbContext())
                {
                    var user = context.Users.FirstOrDefault(u => u.Username == tb_TenDangnhap.Text
                    && u.Email == tb_Email.Text);
                    if (user == null)
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Tên đăng nhập hoặc email không khớp với hệ thống.");
                        thongBaoLoi.ShowDialog();
                    }
                    else
                    {
                        var parent = this.Parent as ContentControl;

                        if (parent != null)
                        {

                            var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;

                            if (quenMatKhauViewModel != null)
                            {

                                GiaoDienQuenMatKhau_NhapOTP giaoDienQuenMatKhau_OTP = new
                                        GiaoDienQuenMatKhau_NhapOTP(giaoDien1);

                                giaoDienQuenMatKhau_OTP.DataContext = quenMatKhauViewModel;
                                giaoDienQuenMatKhau_OTP.Margin = new Thickness(50, 280, 50, 245);
                                parent.Content = giaoDienQuenMatKhau_OTP;
                                await send_OTP();
                            }
                        }
                    }

                }
            }
        }

        private void tb_TenDangnhap_KeyDown(object sender, KeyEventArgs e)
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
                btn_GuiMa.Focus();
            }
        }

        
    }

}
