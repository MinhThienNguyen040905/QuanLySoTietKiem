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
    /// Interaction logic for GiaoDienRutTienNhom.xaml
    /// </summary>
    public partial class GiaoDienRutTienNhom : Window
    {
        private Group_Savings_Accounts _group_saving_account;

        public GiaoDienRutTienNhom(Group_Savings_Accounts group_Savings_Account)
        {
            InitializeComponent();
            txb_SoDu.Text = $"{group_Savings_Account.Money:N0} VNĐ";
            txb_tenSTK.Text = group_Savings_Account.Name;
            _group_saving_account = group_Savings_Account;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= 
                margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        private void tb_Sotien_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        // Sự kiện khi nhấn nút "RÚT"
        private void btn_Rut_Click(object sender, RoutedEventArgs e)
        {
            if (!long.TryParse(tb_Sotienrut.Text.Trim(), out long soTienRut) || soTienRut <= 0)
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Vui lòng nhập số tiền rút.");
                thongBao.ShowDialog();
                return;
            }

            if (soTienRut > _group_saving_account.Money)
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Số tiền rút lớn hơn số dư của sổ.");
                thongBao.ShowDialog();
                return;
            }

            if (string.IsNullOrWhiteSpace(tb_Noidung.Text))
            {
                ThongBaoLoi thongBao = new ThongBaoLoi("Vui lòng nhập nội dung.");
                thongBao.ShowDialog();
                return;
            }

            using (var context = new AppDbContext())
            {
                var viewModel = this.DataContext as GiaoDienChinhViewModel;
                var user = viewModel.User;

                if (viewModel != null && user != null)
                {
                    var userDetail = context.Group_Details
                   .FirstOrDefault(ud => ud.Username == user.Username && ud.Saving_ID == 
                   _group_saving_account.Saving_ID);

                    if (userDetail == null)
                    {
                        ThongBaoLoi thongBao = new ThongBaoLoi("Người dùng không thuộc nhóm này.");
                        thongBao.ShowDialog();
                        return;
                    }

                    if (userDetail.Is_Owner == true)
                    {
                        // Trường hợp người rút là trưởng nhóm
                        var saving = context.Group_Savings_Accounts.FirstOrDefault(s => 
                                    s.Saving_ID == _group_saving_account.Saving_ID);

                        var findUser = context.Users.FirstOrDefault(u => u.Username == user.Username);

                        var groupDetails = context.Group_Details.FirstOrDefault(p => p.Username == user.Username
                        && p.Saving_ID == _group_saving_account.Saving_ID);

                        if (saving != null && findUser != null && groupDetails != null)
                        {
                            saving.Money -= soTienRut; // Trừ tiền khỏi sổ tiết kiệm
                            findUser.Money += soTienRut;  // Cộng tiền vào tài khoản cá nhân

                            groupDetails.Total_Money -= soTienRut; // Trừ tiền đóng góp

                            viewModel.User = findUser;
                            viewModel.OnPropertyChanged(nameof(viewModel.User));

                            // Thêm giao dịch rút tiền
                            var transaction = new Group_Transactions_Information
                            {
                                Transaction_Date = DateTime.Now,
                                Money = -1 * soTienRut,
                                Description = tb_Noidung.Text.Trim() != "" ? tb_Noidung.Text.Trim() : "Trống",
                                Saving_ID = _group_saving_account.Saving_ID,
                                Username = user.Username
                            };

                            context.Group_Transactions_Information.Add(transaction);

                            var updatedAccount = context.Group_Savings_Accounts
                                .Include(p => p.Group_Details)
                                    .ThenInclude(d => d.User)
                                .Include(t => t.Group_Transactions_Information)
                                    .ThenInclude(f => f.User)
                                .Include(e => e.Interest_Rates)
                                .Where(p => p.Saving_ID == _group_saving_account.Saving_ID && 
                                    p.Status == "Đang hoạt động")
                                .FirstOrDefault();

                            if (updatedAccount != null)
                            {
                                var existingAccount = viewModel.Group_Savings_Accounts
                                            .FirstOrDefault(p => p.Saving_ID ==
                                            _group_saving_account.Saving_ID);

                                if (existingAccount != null)
                                {
                                    var index = viewModel.Group_Savings_Accounts.IndexOf(existingAccount);
                                    viewModel.Group_Savings_Accounts[index] = updatedAccount;
                                    viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                                }
                            }

                            // Tạo thông báo cho các thành viên khác
                            var groupMembers = context.Group_Details
                                .Where(gd => gd.Saving_ID == _group_saving_account.Saving_ID
                                && gd.Username != user.Username)
                                .ToList();

                            var groupNotification = new Group_Notifications
                            {
                                Description = tb_Noidung.Text.Trim() != "" ? tb_Noidung.Text.Trim() : "Trống",
                                Type = "Rút",
                                Money = -1 * soTienRut,
                                Notification_Date = DateTime.Now,
                                Username_Sender = user.Username,
                                Saving_ID = _group_saving_account.Saving_ID
                            };

                            context.Group_Notifications.Add(groupNotification);
                            context.SaveChanges();  // Lưu thay đổi và lấy ID của thông báo chung

                            var groupNotificationDetails = groupMembers.Select(member => new Group_Notifications_Details
                            {
                                Username = member.Username,
                                Is_Deleted = false,
                                Status = "Chưa đọc",
                                Group_Notification_ID = groupNotification.Group_Notification_ID  // Gán ID của thông báo chung vào
                            }).ToList();

                            context.Group_Notifications_Details.AddRange(groupNotificationDetails);

                            context.SaveChanges();
                            this.Close();
                        }
                    }
                    else
                    {
                        // Trường hợp người rút không phải là trưởng nhóm
                        var groupOwner = context.Group_Details
                            .FirstOrDefault(gd => gd.Saving_ID == _group_saving_account.Saving_ID
                            && gd.Is_Owner == true);

                        if (groupOwner != null)
                        {
                            var groupNotification = new Group_Notifications
                            {
                                Description = tb_Noidung.Text.Trim() != "" ? tb_Noidung.Text.Trim() : "Trống",
                                Type = "RútYC",
                                Money =  soTienRut,
                                Notification_Date = DateTime.Now,
                                Username_Sender = user.Username,
                                Saving_ID = _group_saving_account.Saving_ID
                            };

                            // Thêm thông báo chung vào cơ sở dữ liệu
                            context.Group_Notifications.Add(groupNotification);
                            context.SaveChanges();  // Lưu để lấy ID của thông báo chung

                            var groupNotificationDetail = new Group_Notifications_Details
                            {
                                Username = groupOwner.Username,
                                Is_Deleted = false,
                                Status = "Chưa trả lời",
                                Group_Notification_ID = groupNotification.Group_Notification_ID  // Gán ID của thông báo chung vào
                            };

                            context.Group_Notifications_Details.Add(groupNotificationDetail);

                            context.SaveChanges();  // Lưu thay đổi
                            this.DialogResult = true;
                            this.Close();
                        }
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
