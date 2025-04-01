using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static MaterialDesignThemes.Wpf.Theme;

namespace PhanMemQuanLySTK
{
    public class InterestRateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null)
            {
                return "Không kì hạn";
            }

            return $"{value} tháng";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for GiaoDienTaoSoTietKiemCaNhan.xaml
    /// </summary>
    public partial class GiaoDienTaoSoTietKiemCaNhan : Window
    {
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            using (var context = new AppDbContext()) { 
                var data = context.Interest_Rates.ToList();
                cb_LaiSuat.ItemsSource = data; 
            }
         }

        public GiaoDienTaoSoTietKiemCaNhan(GiaoDienChinhViewModel giaoDienChinhViewModel)
        {   
            this.DataContext = giaoDienChinhViewModel;
            InitializeComponent();
        }
        private void btnHuyBo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnTao_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_TenSoTK.Text) || string.IsNullOrEmpty(cb_LaiSuat?.SelectedValue?.ToString())
                || string.IsNullOrEmpty(tb_Sotien.Text))
            {
                ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                mess.ShowDialog();
            } else
            {
                using (var context = new AppDbContext())
                {
                    var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                    var isExistedAccount = context.Personal_Savings_Accounts
    .FirstOrDefault(p => p.Name != null &&
                         p.Name.Trim().ToLower() == tb_TenSoTK.Text.Trim().ToLower() && p.Username == username
                         && p.Status == "Đang hoạt động");

                    if (isExistedAccount != null)
                    {
                        ThongBaoLoi mess = new ThongBaoLoi("Tên sổ tiết kiệm đã tồn tại. Vui lòng nhập tên khác.");
                        mess.ShowDialog();
                    }
                    else if (isExistedAccount == null)
                    {

                        if (username != null)
                        {

                            var user = context.Users.
                            FirstOrDefault(u => u.Username == username);

                            if (user != null)
                            {
                                if (user.Money < long.Parse(tb_Sotien.Text))
                                {
                                    ThongBaoLoi mess = new ThongBaoLoi("Số dư không đủ để tạo sổ.");
                                    mess.ShowDialog();

                                } else
                                {
                                    user.Money -= long.Parse(tb_Sotien.Text);

                                    var soTietKiemMoi = new Personal_Savings_Accounts()
                                    {
                                        Name = tb_TenSoTK.Text,
                                        Creating_Date = DateTime.Now,
                                        Money = long.Parse(tb_Sotien.Text),
                                        Status = "Đang hoạt động",
                                        Username = username,
                                        Interest_Rate_ID = int.Parse(cb_LaiSuat.SelectedValue.ToString()!),
                                        Description = tb_Mota.Text != "" ? tb_Mota.Text : "Trống",
                                        Target = tb_Muctieu.Text != "" ? long.Parse(tb_Muctieu.Text) : null,
                                    };

                                    context.Personal_Savings_Accounts.Add(soTietKiemMoi);

                                    context.SaveChanges();

                                    context.Entry(soTietKiemMoi).Reference(s => s.Interest_Rate).Load();

                                    var transaction = new Personal_Transactions_Information() 
                                    { 
                                       Transaction_Date = DateTime.Now,
                                       Money = long.Parse(tb_Sotien.Text),
                                       Saving_ID = soTietKiemMoi.Saving_ID,
                                       Description = "Tạo sổ tiết kiệm"
                                    };

                                    context.Personal_Transactions_Information.Add(transaction);

                                    context.SaveChanges();

                                    var viewModel = this.DataContext as GiaoDienChinhViewModel;

                                    if (viewModel != null)
                                    {
                                        viewModel.Personal_Savings_Accounts.Insert(0, soTietKiemMoi);
                                        viewModel.IsEmptyAccount = viewModel.Personal_Savings_Accounts.Count > 0 
                                            ? false : true;
                                        viewModel.User.Money = user.Money;
                                        viewModel.OnPropertyChanged(nameof(viewModel.Personal_Savings_Accounts));
                                        viewModel.OnPropertyChanged(nameof(viewModel.User)); 
                                    }

                                    this.DialogResult = true;
                                    this.Close();
                                }
                                  
                            }
                        }
                    }

                }
            }
         }

        private void tb_Sotien_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }
    }
}
