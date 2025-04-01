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

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienQuenMatKhau_NhapOTP.xaml
    /// </summary>
    /// 

    public class PositiveNumberToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int demNguoc)
            {
                return demNguoc == 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class GiaoDienQuenMatKhau_NhapOTP : UserControl, INotifyPropertyChanged
    {
        private GiaoDien1 giaoDien1;
        private bool isRunning;

        public GiaoDienQuenMatKhau_NhapOTP(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            this.giaoDien1 = giaoDien1;

            Loaded += (sender, args) =>
         {
             var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;
             if (quenMatKhauViewModel != null)
             {
                 quenMatKhauViewModel.DemNguoc = 120;
                 StartCountdown();
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
            if (this.DataContext is QuenMatKhauViewModel quenMatKhauViewModel)
            {
                while (quenMatKhauViewModel.DemNguoc > 0 && isRunning)
                {
                    await Task.Delay(1000);
                    quenMatKhauViewModel.DemNguoc--;
                }
            }

        }

        private void btn_QuayLai_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                GiaoDienQuenMatKhau giaoDienQuenMatKhau = new GiaoDienQuenMatKhau(giaoDien1);

                giaoDienQuenMatKhau.DataContext = this.DataContext;

                giaoDienQuenMatKhau.Margin = new Thickness(50, 280, 50, 270);
                parent.Content = giaoDienQuenMatKhau;
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

        private void btn_GuiMa_Click(object sender, RoutedEventArgs e)
        {
            var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;
            quenMatKhauViewModel!.DemNguoc = 120;
            StartCountdown();
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


                var quenMatKhauViewModel = this.DataContext as QuenMatKhauViewModel;

                if (quenMatKhauViewModel != null)
                {

                    if (userInputOtp == quenMatKhauViewModel?.OTP)
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
                            await Task.Delay(1000);
                            GiaoDienDatLaiMatKhau giaoDienDatLaiMatKhau = new GiaoDienDatLaiMatKhau(giaoDien1);
                            giaoDienDatLaiMatKhau.DataContext = quenMatKhauViewModel;
                            giaoDienDatLaiMatKhau.Margin = new Thickness(50, 280, 40, 290);
                            parent.Content = giaoDienDatLaiMatKhau;
                        }
                    }
                    else
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Mã OTP không đúng.");
                        thongBaoLoi.ShowDialog();
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
}

