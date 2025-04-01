using Microsoft.EntityFrameworkCore;
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

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienNapTien.xaml
    /// </summary>
    public partial class GiaoDienNapTien : Window
    {
        private int _saving_ID;
        private Personal_Savings_Accounts _personal_Savings_Account;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        public GiaoDienNapTien(Personal_Savings_Accounts personal_Savings_Account, Users user)
        {
            InitializeComponent();
            this.txb_tenSTK.Text = personal_Savings_Account.Name;
            this.txb_soDu.Text = $"{user.Money:N0} VNĐ";
            _personal_Savings_Account = personal_Savings_Account;
        }

        private void btn_HuyBo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Nap_Click(object sender, RoutedEventArgs e)
        {
            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

            if (username != null && _personal_Savings_Account.Saving_ID > 0)
            {
                if (string.IsNullOrEmpty(tb_soTienNap.Text))
                {
                    ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập số tiền muốn nạp.");
                    mess.ShowDialog();
                } else if (long.Parse(tb_soTienNap.Text) % 1000 != 0)
                {
                    ThongBaoLoi mess = new ThongBaoLoi("Số tiền nạp phải chia hết cho 1000.");
                    mess.ShowDialog();
                } else 
                {
                    using (var context = new AppDbContext())
                    {
                        var user = context.Users.
                            FirstOrDefault(u => u.Username == username);

                        var savingAccount = context.Personal_Savings_Accounts
                            .FirstOrDefault(s => s.Saving_ID == _personal_Savings_Account.Saving_ID
                            && s.Username == username);

                        var soTienNap = long.Parse(tb_soTienNap.Text);

                        if (user.Money < soTienNap)
                        {
                            ThongBaoLoi mess = new 
                                ThongBaoLoi("Số dư của bạn không đủ để nạp tiền vào sổ tiết kiệm này.");
                            mess.ShowDialog();
                        } else
                        {
                            user.Money -= soTienNap;
                            savingAccount.Money += soTienNap;

                            var transaction = new Personal_Transactions_Information { 
                                Transaction_Date = DateTime.Now,
                                Money = soTienNap,
                                Description = noidung.Text != "" ? noidung.Text : "Trống",
                                Saving_ID = _personal_Savings_Account.Saving_ID,
                            };

                            context.Personal_Transactions_Information.Add(transaction);

                            context.SaveChanges();

                            var updatedAccount = context.Personal_Savings_Accounts
                                             .Include(p => p.Personal_Transactions_Information)
                                               .Include(p => p.Interest_Rate)
                                              .FirstOrDefault(p => p.Saving_ID ==
                                              savingAccount.Saving_ID && p.Username == username);

                            if (updatedAccount != null)
                            {
                                updatedAccount.Personal_Transactions_Information =
    new ObservableCollection<Personal_Transactions_Information>(
        updatedAccount.Personal_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList()
    );
                                var viewModel = this.DataContext as GiaoDienChinhViewModel;

                                var existingAccount = viewModel.Personal_Savings_Accounts
                                    .FirstOrDefault(p => p.Saving_ID == 
                                    _personal_Savings_Account.Saving_ID);

                                if (existingAccount != null)
                                {
                                    var index = viewModel.Personal_Savings_Accounts.IndexOf(existingAccount);
                                    viewModel.Personal_Savings_Accounts[index] = updatedAccount;
                                    viewModel.User.Money = user.Money;
                                    viewModel.OnPropertyChanged(nameof(Personal_Savings_Accounts));
                                    viewModel.OnPropertyChanged(nameof(viewModel.User));
                                }
                            }

                            this.Close();
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
