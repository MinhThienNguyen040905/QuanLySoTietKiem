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
    /// Interaction logic for GiaoDienChuyenTien.xaml
    /// </summary>
    public partial class GiaoDienChuyenTien : Window
    {
        private Personal_Savings_Accounts _personal_savings_account;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        public GiaoDienChuyenTien(Personal_Savings_Accounts personal_Savings_Account, Users user)
        {
            InitializeComponent();
            this.txb_tenSTK.Text = personal_Savings_Account.Name;

            this.txb_soDu.Text = $"{personal_Savings_Account.Money:N0} VNĐ";

            _personal_savings_account = personal_Savings_Account;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void tb_Sotien_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void SoTietKiemCaNhan_Checked(object sender, RoutedEventArgs e)
        {
            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;
            var radioButton = sender as RadioButton;

            if (radioButton != null)
            {
                if (username != null)
                {
                    ObservableCollection<Personal_Savings_Accounts> accounts =
                      GetOtherSavingsAccountsAsync(username, _personal_savings_account.Saving_ID);

                    if (accounts.Count == 0)
                    {
                        this.cbBox_soTK.IsEnabled = false;
                    } else
                    {
                        this.cbBox_soTK.ItemsSource = accounts;
                        this.cbBox_soTK.IsEnabled = true;
                    }
                }
            }
        }

        private ObservableCollection<Personal_Savings_Accounts>
            GetOtherSavingsAccountsAsync(string username, int excludedId)
        {
            using (var context = new AppDbContext())
            {
                var accounts = context.Personal_Savings_Accounts
                    .Where(x => x.Username == username && x.Saving_ID != excludedId && x.Status == "Đang hoạt động")
                    .ToList();

                return new ObservableCollection<Personal_Savings_Accounts>(accounts);
            }
        }

        private ObservableCollection<Group_Savings_Accounts>
            GetGroupSavingsAccounts(string username)
        {
            using (var context = new AppDbContext())
            {
                var accounts = context.Group_Details
             .Where(gd => gd.Username == username && gd.Group_Savings_Accounts.Status == "Đang hoạt động")
                    .Select(gd => gd.Group_Savings_Accounts)
                    .Distinct()
                    .ToList();

                return new ObservableCollection<Group_Savings_Accounts>(accounts);
            }
        }

        private void SoTietKiemNhom_Checked(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (viewModel != null)
            {
                var username = viewModel.Username;

                if (username != "")
                {
                    var accounts = GetGroupSavingsAccounts(username);

                    if (accounts != null)
                    {

                        if (accounts.Count == 0)
                        {
                            this.cbBox_soTK.IsEnabled = false;
                        }
                        else
                        {
                            this.cbBox_soTK.IsEnabled = true;
                            this.cbBox_soTK.ItemsSource = accounts;
                        }
                    }
                }
            }
        }

        private void button_Chuyen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tb_soTienChuyen.Text) || cbBox_soTK.SelectedIndex == -1)
            {
                ThongBaoLoi mess = new ThongBaoLoi("Vui lòng nhập đầy đủ thông tin.");
                mess.ShowDialog();
            } else 
            {
                var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                using (var context = new AppDbContext())
                {
                    // chuyển tiền sổ cá nhân
                    if (radio_button_canhan.IsChecked == true)
                    {
                        if (username != "" && _personal_savings_account.Saving_ID > 0)
                        {
                            var user = context.Users.FirstOrDefault(user => user.Username == username);

                            var savingAccount = context.Personal_Savings_Accounts
                                .FirstOrDefault(s => s.Saving_ID == _personal_savings_account.Saving_ID
                                && s.Username == username);

                            var soTienChuyen = long.Parse(tb_soTienChuyen.Text);

                            if (savingAccount != null && savingAccount.Money < soTienChuyen)
                            {
                                ThongBaoLoi mess = new ThongBaoLoi("Số dư tài khoản không đủ để chuyển.");
                                mess.ShowDialog();
                            }
                            else
                            {
                                var findSavingAccount = context.Personal_Savings_Accounts
                                .FirstOrDefault(s => s.Saving_ID ==
                                int.Parse(cbBox_soTK.SelectedValue.ToString())
                                && s.Username == username);

                                if (findSavingAccount != null)
                                {
                                    findSavingAccount.Money += soTienChuyen;
                                    savingAccount.Money -= soTienChuyen;

                                    if (radio_button_canhan.IsChecked ?? false)
                                    {
                                        // sổ chuyển
                                        var transaction = new Personal_Transactions_Information
                                        {
                                            Transaction_Date = DateTime.Now,
                                            Money = soTienChuyen * (-1),
                                            Description = "Chuyển tiền đến " + findSavingAccount.Name + " (cá nhân)",
                                            Saving_ID = _personal_savings_account.Saving_ID,
                                        };

                                        // sổ nhận
                                        var other_transaction = new Personal_Transactions_Information
                                        {
                                            Transaction_Date = DateTime.Now,
                                            Money = soTienChuyen,
                                            Description = noidung.Text != "" ? noidung.Text : "Trống",
                                            Saving_ID = findSavingAccount.Saving_ID,
                                        };


                                        context.Personal_Transactions_Information.Add(transaction);
                                        context.Personal_Transactions_Information.Add(other_transaction);
                                    }

                                    context.SaveChanges();

                                    var updatedAccount = context.Personal_Savings_Accounts
                                            .Include(p => p.Personal_Transactions_Information)
                                              .Include(p => p.Interest_Rate)
                                             .FirstOrDefault(p => p.Saving_ID ==
                                             _personal_savings_account.Saving_ID && p.Username == username);

                                    var updatedAccount1 = context.Personal_Savings_Accounts
        .Include(p => p.Personal_Transactions_Information)
          .Include(p => p.Interest_Rate)
         .FirstOrDefault(p => p.Saving_ID ==
         int.Parse(cbBox_soTK.SelectedValue.ToString()) && p.Username == username);

                                    if (updatedAccount != null && updatedAccount1 != null)
                                    {
                                        var viewModel = this.DataContext as GiaoDienChinhViewModel;

                                        var existingAccount = viewModel.Personal_Savings_Accounts
                                                                       .FirstOrDefault(p => p.Saving_ID ==
                                                                       _personal_savings_account.Saving_ID);

                                        var existingAccount1 = viewModel.Personal_Savings_Accounts
                                                        .FirstOrDefault(p => p.Saving_ID ==
                                                 int.Parse(cbBox_soTK.SelectedValue.ToString()));

                                        if (existingAccount != null && existingAccount1 != null)
                                        {
                                            updatedAccount.Personal_Transactions_Information =
    new ObservableCollection<Personal_Transactions_Information>(
        updatedAccount.Personal_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList()
    );

                                            updatedAccount1.Personal_Transactions_Information =
    new ObservableCollection<Personal_Transactions_Information>(
        updatedAccount1.Personal_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList()
    );
                                            var index = viewModel.Personal_Savings_Accounts.IndexOf(existingAccount);
                                            var index1 = viewModel.Personal_Savings_Accounts.IndexOf(existingAccount1);
                                            viewModel.Personal_Savings_Accounts[index] = updatedAccount;
                                            viewModel.Personal_Savings_Accounts[index1] = updatedAccount1;
                                            viewModel.User.Money = user.Money;
                                            viewModel.OnPropertyChanged(nameof(viewModel.Personal_Savings_Accounts));
                                            viewModel.OnPropertyChanged(nameof(viewModel.User));
                                        }
                                    }
                                    this.DialogResult = true;
                                    this.Close();
                                }
                            }
                        }
                    } 
                    else if (radio_button_nhom.IsChecked == true)
                    {
                        // chuyển tiền sổ nhóm

                        var groupSaving = context.Group_Savings_Accounts.FirstOrDefault(gs => gs.Saving_ID == 
                        int.Parse(cbBox_soTK.SelectedValue.ToString()));

                        var personalSaving = context.Personal_Savings_Accounts.FirstOrDefault(ps => ps.Saving_ID
                        == _personal_savings_account.Saving_ID && ps.Username == username);

                        if (personalSaving != null && personalSaving.Money < long.Parse(tb_soTienChuyen.Text))
                        {
                            ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Số dư tài khoản không đủ để chuyển.");
                            thongBaoLoi.ShowDialog();
                            return;
                        }

                            if (groupSaving != null && username != "" && personalSaving != null)
                        {
                            long soTienChuyen = long.Parse(tb_soTienChuyen.Text);

                            var groupDetails = context.Group_Details.FirstOrDefault(p => p.Username == username
                        &&   p.Saving_ID == groupSaving.Saving_ID);

                            if (groupDetails != null)
                            {
                                // tăng số tiền đóng góp
                                groupDetails.Total_Money += soTienChuyen;
                            }

                            groupSaving.Money += soTienChuyen;
                            personalSaving.Money -= soTienChuyen;

                            // tạo lịch sử giao dịch cho các sổ

                            var personalTransaction = new Personal_Transactions_Information()
                            {
                                Transaction_Date = DateTime.Now,
                                Money = soTienChuyen * (-1),
                                Description = "Chuyển tiền đến " + groupSaving.Name + " (Nhóm)",
                                Saving_ID = personalSaving.Saving_ID,
                            };

                            var groupTransaction = new Group_Transactions_Information()
                            {
                                Transaction_Date = DateTime.Now,
                                Money = soTienChuyen,
                                Description = noidung.Text != "" ? noidung.Text : "Trống",
                                Saving_ID = groupSaving.Saving_ID,
                                Username = username,
                            };

                            context.Personal_Transactions_Information.Add(personalTransaction);
                            context.Group_Transactions_Information.Add(groupTransaction);

                            // tạo thông báo cho các thành viên còn lại trong nhóm biết
                            var groupDetailsNotifications = context.Group_Details
                        .Where(gd => gd.Saving_ID == groupSaving.Saving_ID && gd.Username != username)
                        .ToList();

                            var groupNotification = new Group_Notifications
                            {
                                Description = noidung.Text.Trim() != "" ? noidung.Text.Trim() : "Trống",
                                Type = "Nạp",
                                Money = soTienChuyen,
                                Notification_Date = DateTime.Now,
                                Username_Sender = username,
                                Saving_ID = groupSaving.Saving_ID
                            };

                            // Thêm thông báo chung vào cơ sở dữ liệu
                            context.Group_Notifications.Add(groupNotification);
                            context.SaveChanges();  // Lưu thay đổi và lấy ID của thông báo chung

                            var groupNotificationDetails = groupDetailsNotifications.
                                Select(member => new Group_Notifications_Details
                            {
                                Username = member.Username,
                                Is_Deleted = false,
                                Status = "Chưa đọc",
                                Group_Notification_ID = groupNotification.Group_Notification_ID  // Gán ID của thông báo chung vào
                            }).ToList();

                            context.Group_Notifications_Details.AddRange(groupNotificationDetails);

                            context.SaveChanges();

                            var viewModel = this.DataContext as GiaoDienChinhViewModel;

                            if (viewModel != null)
                            {

                                var updatedAccount = context.Personal_Savings_Accounts
                                         .Include(p => p.Personal_Transactions_Information)
                                           .Include(p => p.Interest_Rate)
                                          .FirstOrDefault(p => p.Saving_ID ==
                                          _personal_savings_account.Saving_ID && p.Username == username);

                                var updatedAccountNhom = context.Group_Savings_Accounts
                                    .Include(p => p.Group_Details)
                                        .ThenInclude(d => d.User)
                                    .Include(t => t.Group_Transactions_Information)
                                        .ThenInclude(f => f.User)
                                    .Include(e => e.Interest_Rates)
                                    .Where(p => p.Group_Details.Any(d => d.Username == username)
                                                && p.Status == "Đang hoạt động"
                                                && p.Saving_ID == groupSaving.Saving_ID)
                                    .FirstOrDefault(); // Lấy phần tử đầu tiên

                                if (updatedAccountNhom != null)
                                {
                                    // Sắp xếp các giao dịch của tài khoản
                                    updatedAccountNhom.Group_Transactions_Information = updatedAccountNhom.Group_Transactions_Information
                                        .OrderByDescending(t => t.Transaction_Date)
                                        .ToList();
                                }

                                var index = viewModel.Personal_Savings_Accounts
                               .Select((account, idx) => new { account.Saving_ID, idx }) // Lấy cả Id và index
                               .FirstOrDefault(x => x.Saving_ID == personalSaving.Saving_ID)?.idx ?? -1; // Tìm id khớp và trả về index

                                var index1 = viewModel.Group_Savings_Accounts
                                   .Select((account, idx) => new { account.Saving_ID, idx })
                                   .FirstOrDefault(x => x.Saving_ID == groupSaving.Saving_ID)?.idx ?? -1;

                                if (index != -1)
                                {
                                    updatedAccount.Personal_Transactions_Information =
                                        new ObservableCollection<Personal_Transactions_Information>(
                                            updatedAccount.Personal_Transactions_Information
                                                .OrderByDescending(t => t.Transaction_Date)
                                                .ToList()
                                        );
                                    viewModel.Personal_Savings_Accounts[index] = updatedAccount;

                                    viewModel.OnPropertyChanged(nameof(viewModel.Personal_Savings_Accounts));
                                }

                                if (index1 != -1)
                                {
                                    viewModel.Group_Savings_Accounts[index1] = updatedAccountNhom;
                                    viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
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
}
