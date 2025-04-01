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
    /// Interaction logic for GiaoDienRutTien.xaml
    /// </summary>
    public partial class GiaoDienRutTien : Window
    {
        private Personal_Savings_Accounts _personal_Savings_Account;

        public GiaoDienRutTien(Personal_Savings_Accounts personal_Savings_Account)
        {
            InitializeComponent();
            this.txb_tenSTK.Text = personal_Savings_Account.Name;
            this.txb_SoDu.Text = $"{personal_Savings_Account.Money:N0} VNĐ";
            _personal_Savings_Account = personal_Savings_Account;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Rut_Click(object sender, RoutedEventArgs e)
        {
            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

            if (username != null && _personal_Savings_Account.Saving_ID > 0)
            {
                if (string.IsNullOrEmpty(tb_soTienRut.Text))
                {
                    ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập số tiền muốn rút.");
                    mess.ShowDialog();
                }
                else
                {

                    using (var context = new AppDbContext())
                    {
                        var user = context.Users.
                            FirstOrDefault(u => u.Username == username);

                        var savingAccount = context.Personal_Savings_Accounts
                            .FirstOrDefault(s => s.Saving_ID == _personal_Savings_Account.Saving_ID
                            && s.Username == username);

                        var soTienRut = long.Parse(tb_soTienRut.Text);

                        if (savingAccount.Money < soTienRut)
                        {
                            ThongBaoLoi mess = new ThongBaoLoi("Số tiền của sổ không đủ để rút.");
                            mess.ShowDialog();
                        }
                        else
                        {
                            user.Money += soTienRut;

                            savingAccount.Money -= soTienRut;

                            var transaction = new Personal_Transactions_Information
                            {
                                Transaction_Date = DateTime.Now,
                                Money = soTienRut * (-1),
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
                                    viewModel.OnPropertyChanged(nameof(viewModel.Personal_Savings_Accounts));
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
