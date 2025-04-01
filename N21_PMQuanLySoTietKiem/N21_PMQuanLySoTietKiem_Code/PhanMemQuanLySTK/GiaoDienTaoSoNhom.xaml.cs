using MaterialDesignThemes.Wpf;
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDienTaoSoNhom.xaml
    /// </summary>
    public partial class GiaoDienTaoSoNhom : Window
    {
        private string _username;

        public GiaoDienTaoSoNhom()
        {
            InitializeComponent();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 50;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }

        public GiaoDienTaoSoNhom(string username)
        {
            InitializeComponent();

            _username = username;
        }

        private void rbtn_left_Click(object sender, RoutedEventArgs e)
        {
            var stackpanel = FindAncestor<StackPanel>(sender as RepeatButton);
            if (stackpanel != null)
            {
                var scrollviewer = FindChild<ScrollViewer>(stackpanel, "scrollViewer");
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - 10);
            }

        }

        private void rbtn_right_Click(object sender, RoutedEventArgs e)
        {
            var stackpanel = FindAncestor<StackPanel>(sender as RepeatButton);
            if (stackpanel != null)
            {
                var scrollviewer = FindChild<ScrollViewer>(stackpanel, "scrollViewer");
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + 10);
            }

        }

        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                    return (T)current;

                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }


        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T && ((T)child).GetValue(FrameworkElement.NameProperty).ToString() == childName)
                    return (T)child;

                var result = FindChild<T>(child, childName);
                if (result != null)
                    return result;
            }
            return null;
        }


        private void btn_Them_thanh_vien_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            GiaoDienThemThanhVien them_Thanh_Vien = new GiaoDienThemThanhVien(viewModel?.Username!);
            them_Thanh_Vien.DataContext = this.DataContext;
            them_Thanh_Vien.ShowDialog();
        }

        private void Button_Tao_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (string.IsNullOrEmpty(tb_Tensotietkiem.Text) || string.IsNullOrEmpty(tb_Sotiengui.Text) ||
                string.IsNullOrEmpty(cb_LaiSuat.Text)) {

                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng nhập đủ thông tin!");
                thongBaoLoi.ShowDialog();
                return;
            }

            if (!long.TryParse(tb_Sotiengui.Text, out long soTienGui) || soTienGui <= 0)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Số tiền gửi không hợp lệ!");
                thongBaoLoi.ShowDialog();
                return;
            }

            if (item_DSthanhvien.Items.Count <= 0)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Vui lòng thêm ít nhất 1 thành viên.");
                thongBaoLoi.ShowDialog();
                return;
            }


            // Kiểm tra số tiền phải là bội của 1000
            if (soTienGui % 1000 != 0)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Số tiền gửi phải là bội của 1000");
                thongBaoLoi.ShowDialog();
                return;
            }

            using (var dbContext = new AppDbContext())
            {

                // Lấy thông tin người tạo nhóm từ cơ sở dữ liệu
                var user = dbContext.Users.FirstOrDefault(u => u.Username == viewModel.Username);

                if (user == null)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Người dùng không tồn tại!");
                    thongBaoLoi.ShowDialog();
                    return;
                }


                if (dbContext.Group_Savings_Accounts.Any(p => p.Name == tb_Tensotietkiem.Text 
                && p.Group_Details.Any(u => u.Username
                == user.Username && u.Is_Owner == true)))
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Tên sổ tiết kiệm đã tồn tại!");
                    thongBaoLoi.ShowDialog();
                    return;
                }

                // Kiểm tra số dư
                if (user.Money < soTienGui)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Số dư không đủ để thực hiện giao dịch!");
                    thongBaoLoi.ShowDialog();
                    return;
                }

                // Trừ tiền từ tài khoản của người tạo
                user.Money -= soTienGui;
                viewModel.User.Money = user.Money;
                viewModel.OnPropertyChanged(nameof(viewModel.User));

                // Tạo mới Group_Savings_Accounts
                var newGroupSaving = new Group_Savings_Accounts
                {
                    Name = tb_Tensotietkiem.Text,
                    Creating_Date = DateTime.Now,
                    Money = soTienGui,
                    Target = string.IsNullOrEmpty(tb_Muctieu.Text) ? (long?)null : long.Parse(tb_Muctieu.Text),
                    Status = "Đang hoạt động",
                    Description = tb_Noidung.Text != "" ? tb_Noidung.Text : "Trống",
                    Interest_Rate_ID = int.Parse(cb_LaiSuat.SelectedValue.ToString()!)
                };

                dbContext.Group_Savings_Accounts.Add(newGroupSaving);
                dbContext.SaveChanges();

                var newTransactionInformation = new Group_Transactions_Information
                {
                    Transaction_Date = DateTime.Now,
                    Money = newGroupSaving.Money,
                    Description = "Tạo sổ tiết kiệm",
                    Saving_ID = newGroupSaving.Saving_ID,
                    Username = user.Username,
                };

                dbContext.Group_Transactions_Information.Add(newTransactionInformation);
                dbContext.SaveChanges();


                // Thêm người tạo vào Group_Details với IsOwner = true
                var leaderDetail = new Group_Details
                {
                    Saving_ID = newGroupSaving.Saving_ID,
                    Username = user.Username,
                    Is_Owner = true,
                    Total_Money = soTienGui
                };

                dbContext.Group_Details.Add(leaderDetail);

                var interestRate = dbContext.Interest_Rates.FirstOrDefault(ir => ir.Interest_Rate_ID ==
                newGroupSaving.Interest_Rate_ID);

                if (interestRate == null)
                {
                    ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Lãi suất không tồn tại!");
                    thongBaoLoi.ShowDialog();
                    return;
                }

                //tạo thông báo
                var memberNotification = new Group_Notifications
                {
                    Description = "Kỳ hạn: " + interestRate.Term.ToString() + " tháng-Lãi suất: "
                           + interestRate.Rate.ToString() + "%/năm" + "-" + newGroupSaving.Description,
                    Type = "MờiYC",
                    Notification_Date = newGroupSaving.Creating_Date,
                    Username_Sender = user.Username,
                    Saving_ID = newGroupSaving.Saving_ID
                };

                 dbContext.Group_Notifications.Add(memberNotification);
                   dbContext.SaveChanges(); // Lưu trước để lấy Group_Notification_ID

                // Thêm thông báo cho các thành viên
                if (viewModel.ItemsUsers != null && viewModel.ItemsUsers.Any())
                {
                    foreach (var member in viewModel.ItemsUsers)
                    {                    
                        var notificationDetail = new Group_Notifications_Details
                        {
                            Group_Notification_ID = memberNotification.Group_Notification_ID,
                            Username = member.Username,
                            Is_Deleted =false,
                            Status = "Chưa trả lời"
                        };
                        dbContext.Group_Notifications_Details.Add(notificationDetail);
                    }
                }
                dbContext.SaveChanges();

                viewModel.Group_Savings_Accounts.Insert(0, newGroupSaving);
                viewModel.ItemsUsers.Clear();
                viewModel.OnPropertyChanged(nameof(Group_Savings_Accounts));

                viewModel.IsEmptyAccountNhom = viewModel.Group_Savings_Accounts.Count == 0;

                this.Close();
            }
        }

        private void tb_Sotien_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var context = new AppDbContext())
            {
                var data = context.Interest_Rates.ToList();
                cb_LaiSuat.ItemsSource = data;
            }
        }

        private void chip_Thanhvien_DeleteClick(object sender, RoutedEventArgs e)
        {
            var viewModel = (GiaoDienChinhViewModel)this.DataContext;
            var chip = sender as Chip;
            if (chip != null)
            {
                var _chip = chip?.DataContext as Users;
                viewModel.ItemsUsers.Remove(_chip);
            }

            //viewModel.ItemsUsers.Remove(p=>p.Username==)
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
