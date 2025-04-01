using Microsoft.EntityFrameworkCore;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for GiaoDienNapTienNhom.xaml
    /// </summary>
    public partial class GiaoDienNapTienNhom : Window
    {
        private Group_Savings_Accounts _group_Savings_Accounts;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        public GiaoDienNapTienNhom(Group_Savings_Accounts group_Savings_Accounts, Users user)
        {
            InitializeComponent();
            lb_Sodu.Text = $"{user.Money:N0} VNĐ";
            txb_tenSTK.Text = group_Savings_Accounts.Name;
            _group_Savings_Accounts = group_Savings_Accounts;
        }


        private void tb_Sotien_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }


        private void btn_Nap_Click(object sender, RoutedEventArgs e)
        {
            var currentMoney = (this.DataContext as GiaoDienChinhViewModel).User.Money;
            var username = (this.DataContext as GiaoDienChinhViewModel).Username;

            if (currentMoney > 0 && username != "")
            {
                if (!long.TryParse(tb_Sotiennap.Text.Trim(), out long sotienNap) || sotienNap <= 0)
                {
                    ThongBaoLoi thongBao = new ThongBaoLoi("Vui lòng nhập số tiền muốn nạp.");
                    thongBao.ShowDialog();
                    return;
                }

                // Kiểm tra số tiền phải là bội của 1000
                if (sotienNap % 1000 != 0)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Số tiền nạp phải là bội của 1000");
                    thongBaoLoi.ShowDialog();
                    return;
                }


                if (sotienNap > currentMoney)
                {
                    ThongBaoLoi thongBao = new ThongBaoLoi("Số tiền nạp vượt quá số dư hiện tại.");
                    thongBao.ShowDialog();
                    return;
                }

                // Cập nhật số tiền của User
                using (var context = new AppDbContext())
                {
                    var viewModel = this.DataContext as GiaoDienChinhViewModel;
                    var user = context.Users.FirstOrDefault(u => u.Username == username);

                    if (user != null)
                    {
                        user.Money -= sotienNap;
                        viewModel.User = user; // Cập nhật lại User trong ViewModel
                        viewModel.OnPropertyChanged(nameof(viewModel.User)); // Thông báo rằng User đã thay đổi
                    }

                    // cộng sổ phần nạp
                    var saving = context.Group_Savings_Accounts.FirstOrDefault(s => s.Saving_ID == _group_Savings_Accounts.Saving_ID);
                    if (saving != null)
                    {
                        saving.Money += sotienNap;
                    }
                    // Thêm giao dịch mới vào Group_Transactions_Information
                    var transaction = new Group_Transactions_Information
                    {
                        Transaction_Date = DateTime.Now,
                        Money = sotienNap,
                        Description = tb_Noidung.Text.Trim() != "" ? tb_Noidung.Text.Trim() : "Trống",
                        Saving_ID = _group_Savings_Accounts.Saving_ID,
                        Username = username
                    };

                    context.Group_Transactions_Information.Add(transaction);

                    var senderDetails = context.Group_Details.FirstOrDefault(p => p.Username ==
                    username && p.Saving_ID == _group_Savings_Accounts.Saving_ID);

                    if (senderDetails != null)
                    {
                        senderDetails.Total_Money += sotienNap;
                    }

                    // Tạo thông báo cho các thành viên khác trong nhóm
                    var groupDetails = context.Group_Details
                        .Where(gd => gd.Saving_ID == _group_Savings_Accounts.Saving_ID && gd.Username != username)
                        .ToList();

                    var groupNotification = new Group_Notifications
                    {
                        Description = tb_Noidung.Text.Trim() != "" ? tb_Noidung.Text.Trim() : "Trống",
                        Type = "Nạp",
                        Money = sotienNap,
                        Notification_Date = DateTime.Now,
                        Username_Sender = username,
                        Saving_ID = _group_Savings_Accounts.Saving_ID
                    };

                    // Thêm thông báo chung vào cơ sở dữ liệu
                    context.Group_Notifications.Add(groupNotification);
                    context.SaveChanges();  // Lưu thay đổi và lấy ID của thông báo chung

                    var groupNotificationDetails = groupDetails.Select(member => new Group_Notifications_Details
                    {
                        Username = member.Username,
                        Is_Deleted = false,
                        Status = "Chưa đọc",
                        Group_Notification_ID = groupNotification.Group_Notification_ID  // Gán ID của thông báo chung vào
                    }).ToList();

                    context.Group_Notifications_Details.AddRange(groupNotificationDetails);

                    // Lưu tất cả thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();

                    var updatedAccount = context.Group_Savings_Accounts
                            .Include(p => p.Group_Details)
                                .ThenInclude(d => d.User)
                            .Include(t => t.Group_Transactions_Information)
                                .ThenInclude(f => f.User)
                            .Include(e => e.Interest_Rates)
                            .Where(p => p.Saving_ID == _group_Savings_Accounts.Saving_ID &&
                                p.Status == "Đang hoạt động")
                            .FirstOrDefault();

                    if (updatedAccount != null)
                    {
                        var existingAccount = viewModel.Group_Savings_Accounts
                                    .FirstOrDefault(p => p.Saving_ID ==
                                   _group_Savings_Accounts.Saving_ID);

                        if (existingAccount != null)
                        {
                            var index = viewModel.Group_Savings_Accounts.IndexOf(existingAccount);
                            viewModel.Group_Savings_Accounts[index] = updatedAccount;
                            viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                        }
                    }

                    this.Close(); 
                }

            }
        }

        private void btn_HuyBo_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
