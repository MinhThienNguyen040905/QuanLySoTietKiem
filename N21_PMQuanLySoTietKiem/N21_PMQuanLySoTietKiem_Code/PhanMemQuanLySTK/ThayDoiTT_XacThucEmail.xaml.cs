using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
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
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.Globalization;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for ThayDoiTT_XacThucEmail.xaml
    /// </summary>
    /// 

    public partial class ThayDoiTT_XacThucEmail : Window
    {
        private DispatcherTimer _timer;
        private bool isRunning;
        int _OTP;
        Random random = new Random();
        private string _new_email;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        private async Task send_OTP(string newEmail)
        {
            var dangNhapViewModel = this.DataContext as GiaoDienChinhViewModel;

            if (dangNhapViewModel != null && newEmail != "")
            {
                _OTP = random.Next(0, 1000000);
                var fromAddress = new MailAddress("finsave123@gmail.com");
                var toAddress = new MailAddress(newEmail);
                const string frompass = "sxpbhdfufikgibxh";
                string OTP = string.Format("{0:D6}", _OTP);
                const string subject = "OTP code";
                string body = "Ma OTP cua ban la " + OTP.ToString();
                dangNhapViewModel.OTP = OTP;
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


        public ThayDoiTT_XacThucEmail(string newEmail)
        {
            InitializeComponent();
            this._new_email = newEmail;
            Loaded += async (sender, args) =>
            {
                var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;
                if (giaoDienChinhViewModel != null)
                {
                    giaoDienChinhViewModel.DemNguoc = 120;
                    StartCountdown();
                    await send_OTP(newEmail);
                }
            };
            isRunning = true;
            tb_OTP1.SelectAll();
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
            if (this.DataContext is GiaoDienChinhViewModel giaoDienChinhViewModel)
            {
                while (giaoDienChinhViewModel.DemNguoc > 0 && isRunning)
                {
                    await Task.Delay(1000);
                    giaoDienChinhViewModel.DemNguoc--;
                }
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
            await send_OTP();
            tt_maotp.Visibility = Visibility.Visible;
            grid_otp.Height = 120;
            isRunning = true;
            StartCountdown();
        }

        private async Task send_OTP()
        {
            var dangNhapViewModel = this.DataContext as GiaoDienChinhViewModel;

            if (dangNhapViewModel != null && _new_email != "")
            {
                _OTP = random.Next(0, 1000000);
                var fromAddress = new MailAddress("finsave123@gmail.com");
                var toAddress = new MailAddress(_new_email);
                const string frompass = "sxpbhdfufikgibxh";
                string OTP = string.Format("{0:D6}", _OTP);
                const string subject = "OTP code";
                string body = "Ma OTP cua ban la " + OTP.ToString();
                dangNhapViewModel.OTP = OTP;
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

                var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;
                
                if (giaoDienChinhViewModel != null)
                {
                    if (userInputOtp != giaoDienChinhViewModel.OTP)
                    {
                        tb_OTP1.BorderThickness = tb_OTP2.BorderThickness = tb_OTP3.BorderThickness
                        = tb_OTP4.BorderThickness = tb_OTP5.BorderThickness = tb_OTP6.BorderThickness
                        = new Thickness(1.5);
                        tb_OTP1.BorderBrush = tb_OTP2.BorderBrush =
                            tb_OTP3.BorderBrush = tb_OTP4.BorderBrush = tb_OTP5.BorderBrush
                            = tb_OTP6.BorderBrush = Brushes.Red;
                        tt_maotp.Visibility = Visibility.Collapsed;
                        grid_otp.Height = 100;
                        ThongBaoLoi mess = new ThongBaoLoi("Mã OTP không chính xác, vui lòng kiểm tra lại");
                        mess.ShowDialog();
                    } else
                    {
                        giaoDienChinhViewModel.NewEmail = _new_email;
                        grid_otp.Height = 100;
                        tt_maotp.Visibility = Visibility.Collapsed;
                        tb_OTP1.BorderThickness = tb_OTP2.BorderThickness = tb_OTP3.BorderThickness
= tb_OTP4.BorderThickness = tb_OTP5.BorderThickness = tb_OTP6.BorderThickness
= new Thickness(1.5);
                        tb_OTP1.BorderBrush = tb_OTP2.BorderBrush = tb_OTP3.BorderBrush =
                            tb_OTP4.BorderBrush = tb_OTP5.BorderBrush = tb_OTP6.BorderBrush = Brushes.Green;
                        await Task.Delay(1000);
                        this.Close();
                    }
                }

            }
        }

        private void btn_Thoat_MouseEnter(object sender, MouseEventArgs e)
        {
            thoat.TextDecorations = TextDecorations.Underline;
        }

        private void btn_Thoat_MouseLeave(object sender, MouseEventArgs e)
        {
            thoat.TextDecorations = null;
        }

        private void btn_Thoat_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
