using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienXacThucEmail_NhapOTP.xaml
    /// </summary>
    /// 
    public partial class GiaoDienXacThucEmail_NhapOTP : UserControl, INotifyPropertyChanged
    {
        private GiaoDien1 giaoDien1;
        private DispatcherTimer _timer;
        private bool isRunning;
        private int demNguoc;
        int _OTP;
        Random random = new Random();

        public GiaoDienXacThucEmail_NhapOTP(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                var dangKyViewModel = this.DataContext as DangKyViewModel;
                if (dangKyViewModel != null)
                {
                    dangKyViewModel.DemNguoc = 120;
                    StartCountdown();
                }
            };

            isRunning = true;
            tb_OTP1.SelectAll();
            this.giaoDien1 = giaoDien1;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async void StartCountdown()
        {
            await StartCountdownAsync();
        }
        private async Task StartCountdownAsync()
        {
            if (this.DataContext is DangKyViewModel dangKyViewModel)
            {
                while (dangKyViewModel.DemNguoc > 0 && isRunning)
                {
                    await Task.Delay(1000);
                    dangKyViewModel.DemNguoc--;
                }
            }
        }

        private void btn_QuayLai_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as ContentControl;

            if (parent != null)
            {
                GiaoDienDangKiThongTin giaoDienDangKiThongTin = new GiaoDienDangKiThongTin(giaoDien1);
                giaoDienDangKiThongTin.DataContext = this.DataContext;
                giaoDienDangKiThongTin.Margin = new Thickness(50, 180, 30, 120);
                parent.Content = giaoDienDangKiThongTin;
            }
        }


        private void tb_OTP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox currentBox = sender as TextBox;
            if (currentBox == null) return;
            if (e.Key == Key.Left)
            {
                MoveToPreviousBox(currentBox);
                e.Handled = true;
            }
            else if (e.Key == Key.Right) MoveToNextBox(currentBox);
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                e.Handled = false;
                if (currentBox.Text.Length == 0 || currentBox.Text.Length == 1)
                {
                    currentBox.Text = e.Key.ToString().Last().ToString();
                }
                MoveToNextBox(currentBox);
                e.Handled = true;
            }
            else if (e.Key == Key.Back)
            {
                e.Handled = false;
                if (string.IsNullOrEmpty(currentBox.Text))
                {
                    MoveToPreviousBox(currentBox);
                }
                else
                {
                    currentBox.Clear();
                    MoveToPreviousBox(currentBox);
                }
            }
            else
            {
                e.Handled = true;
            }
            Check();
        }

        private void MoveToNextBox(TextBox currentBox)
        {
            if (currentBox != null)
            {
                int currentIndex = int.Parse(currentBox.Name.Substring(currentBox.Name.Length - 1));
                TextBox nextBox = (TextBox)this.FindName($"tb_OTP{currentIndex + 1}");

                if (nextBox != null)
                {
                    nextBox.Focus();
                }
            }
        }
        private void MoveToPreviousBox(TextBox currentBox)
        {
            if (currentBox != null)
            {
                int currentIndex = int.Parse(currentBox.Name.Substring(currentBox.Name.Length - 1));
                TextBox previousBox = (TextBox)this.FindName($"tb_OTP{currentIndex - 1}");

                if (previousBox != null)
                {
                    previousBox.Focus();
                }
            }
        }

        private void QuenMatKhau_Unloaded(object sender, RoutedEventArgs e)
        {
            isRunning = false;
        }

        private async void btn_GuiMa_Click(object sender, RoutedEventArgs e)
        {
            var dangKyViewModel = this.DataContext as DangKyViewModel;
            dangKyViewModel!.DemNguoc = 120;
            await send_OTP();
            StartCountdown();
        }

        private async Task send_OTP()
        {
            var dangKyViewModel = this.DataContext as DangKyViewModel;

            _OTP = random.Next(0, 1000000);
            var fromAddress = new MailAddress("finsave123@gmail.com");
            var toAddress = new MailAddress(dangKyViewModel!.Email);
            const string frompass = "sxpbhdfufikgibxh";
            string OTP = string.Format("{0:D6}", _OTP);
            const string subject = "OTP code";
            string body = "Ma OTP cua ban la " + OTP.ToString();
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

        private async void Check()
        {
            if (!string.IsNullOrEmpty(tb_OTP1.Text) &&
                !string.IsNullOrEmpty(tb_OTP2.Text) &&
                !string.IsNullOrEmpty(tb_OTP3.Text) &&
                !string.IsNullOrEmpty(tb_OTP4.Text) &&
                !string.IsNullOrEmpty(tb_OTP5.Text) &&
                !string.IsNullOrEmpty(tb_OTP6.Text))
            {
                string userInputOtp = tb_OTP1.Text + tb_OTP2.Text + tb_OTP3.Text +
                    tb_OTP4.Text + tb_OTP5.Text + tb_OTP6.Text;


                var dangKyViewModel = this.DataContext as DangKyViewModel;

                if (userInputOtp == dangKyViewModel!.OTP)
                {
                    isRunning = false;
                    Keyboard.ClearFocus();


                    tb_OTP1.BorderThickness = tb_OTP2.BorderThickness = tb_OTP3.BorderThickness
        = tb_OTP4.BorderThickness = tb_OTP5.BorderThickness = tb_OTP6.BorderThickness
        = new Thickness(1.5);
                    tb_OTP1.BorderBrush = tb_OTP2.BorderBrush = tb_OTP3.BorderBrush =
                        tb_OTP4.BorderBrush = tb_OTP5.BorderBrush = tb_OTP6.BorderBrush = Brushes.Green;

                    var parent = this.Parent as ContentControl;
                    if (parent != null)
                    {
                        ThongBaoThanhCong tbThanhcong = new ThongBaoThanhCong("Thành công", "Tạo tài khoản thành công.");
                        tbThanhcong.ShowDialog();

                        if (tbThanhcong.DialogResult == true)
                        {
                            using (var context = new AppDbContext())
                            {
                                var user = new Users()
                                {
                                    Username = dangKyViewModel.Username,
                                    Password = dangKyViewModel.Password,
                                    Fullname = dangKyViewModel.Fullname,
                                    Dob = dangKyViewModel.Dob!.Value,
                                    Email = dangKyViewModel.Email,
                                    Money = 0,
                                    Gender = dangKyViewModel.Gender,
                                    Address = dangKyViewModel.Address,
                                    Phone = dangKyViewModel.Phone,
                                    Avatar = "https://res.cloudinary.com/dpnvyfwnp/image/upload/v1735444323/yhrrp3yxft1iksg4z732.png",
                                    Identity_Card = dangKyViewModel.Identity_Card
                                };

                                context.Users.Add(user);
                                context.SaveChanges();
                            }
                            giaoDien1.QuayLaiDangNhap();
                        }
                    }
                }
                else
                {
                    ThongBaoLoi tbLoi = new ThongBaoLoi("Mã xác nhận không chính xác.");
                    tbLoi.ShowDialog();
                    tb_OTP1.BorderThickness = tb_OTP2.BorderThickness = tb_OTP3.BorderThickness 
                        = tb_OTP4.BorderThickness = tb_OTP5.BorderThickness = tb_OTP6.BorderThickness
                        = new Thickness(1.5);
                    tb_OTP1.BorderBrush = tb_OTP2.BorderBrush =
                        tb_OTP3.BorderBrush = tb_OTP4.BorderBrush = tb_OTP5.BorderBrush 
                        = tb_OTP6.BorderBrush = Brushes.Red;
                }
            }
        }
    }
}
