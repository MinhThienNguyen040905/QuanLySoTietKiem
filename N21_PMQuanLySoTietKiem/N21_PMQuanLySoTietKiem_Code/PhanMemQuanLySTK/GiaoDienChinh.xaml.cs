using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Security.Policy;
using MaterialDesignThemes.Wpf;
using System.Windows.Media.Animation;
using System.Windows.Media.TextFormatting; 
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using PhanMemQuanLySTK.ViewModels;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices;
using System.Windows.Controls;
using LiveCharts;
using PhanMemQuanLySTK.Utils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GiaoDienChinh : UserControl, INotifyPropertyChanged
    {
        private GiaoDienChinhViewModel _giaoDienChinhViewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        private GiaoDien1 giaoDien1;

        List<Border> SelectedItem { get; set; }
        List<Border> SelectedItemNhom { get; set; }


        public GiaoDienChinh(GiaoDien1 giaoDien1)
        {
            InitializeComponent();
            UpdateWeekView();

            if (this.DataContext != null)
                _giaoDienChinhViewModel = (this.DataContext as GiaoDienChinhViewModel)!;
            SelectedItem = new List<Border>();
            SelectedItemNhom = new List<Border>();  
            this.giaoDien1 = giaoDien1;
            Keyboard.ClearFocus();
        }

        #region hieu ung di chuot cac button trong expander
        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF69DA26"));
                button.BorderThickness = new Thickness(2);
            }
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.BorderThickness = new Thickness(0);
            }
        }
        #endregion

        #region FindChild & FindAncestor
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
        #endregion

        #region sua mota so tiet kiem ca nhan
        private void lb_Mota_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = sender as Label;
            if (label != null)
            {
                var textbox = FindChild<TextBox>(FindAncestor<StackPanel>(label), "tb_Mota");

                if (textbox != null)
                {
                    textbox.Text = label.Content.ToString();
                    textbox.Visibility = Visibility.Visible;
                    label.Visibility = Visibility.Collapsed;
                    textbox.Focus();
                    textbox.SelectAll();
                }
            }
        }

        private void tb_Mota_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                if (textbox != null)
                {
                    var label = FindChild<Label>(FindAncestor<StackPanel>(textbox), "lb_Mota");

                    int saving_id = (textbox.DataContext as Personal_Savings_Accounts).Saving_ID;

                    using (var context = new AppDbContext())
                    {
                        var account = context.Personal_Savings_Accounts.FirstOrDefault(p => p.Saving_ID == saving_id);

                        if (account != null)
                        {
                            if (textbox.Text == "")
                            {
                                account.Description = "Trống";
                            } else
                            {
                                account.Description = textbox.Text;
                            }

                            context.SaveChanges();
                        }
                    }

                    label.Content = textbox.Text != "" ? textbox.Text : "Trống";

                    label.Visibility = Visibility.Visible;
                    textbox.Visibility = Visibility.Collapsed;
                    Keyboard.ClearFocus();
                }
            }
        }
        #endregion

        #region hieu ung di chuot so tiet kiem
        private void borer_datagrid_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.BorderThickness = new Thickness(2.5);
                border.BorderBrush = Brushes.DarkGreen;
            }
        }

        private void borer_datagrid_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.BorderThickness = new Thickness(1.2);
                border.BorderBrush = Brushes.Gray;
            }
        }
        #endregion

        #region hien thi button rename khi di chuot vao ten so tiet kiem va sua ten so tiet kiem
        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            var row = FindAncestor<DataGridRow>((sender as DependencyObject)!);
            if (row != null)
            {
                var button = FindChild<Button>(row, "btn_rename");
                if (button != null)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }

        private void tb_tenSTK_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                var row = FindAncestor<DataGridRow>(textbox);
                if (row != null)
                {
                    var label = FindChild<Label>(row, "lb_tenSTK");

                    int saving_ID = (textbox.DataContext as Personal_Savings_Accounts).Saving_ID;

                    if (label != null && label.Content != null && saving_ID > 0)
                    {
                        string newValue = textbox.Text;

                        string oldValue = label.Content.ToString();

                        label.Content = newValue;

                        label.Visibility = Visibility.Visible;
                        textbox.Visibility = Visibility.Collapsed;

                        UpdateDatabase(newValue, saving_ID);

                        Keyboard.ClearFocus();
                    }
                }
            }
        }

        private void tb_tenSTK_Nhom_KeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            if (e.Key == Key.Enter && viewModel != null)
            {
                var textbox = sender as TextBox;
                var row = FindAncestor<DataGridRow>(textbox);
                if (row != null)
                {
                    var label = FindChild<Label>(row, "lb_tenSTK_nhom");

                    int saving_ID = (textbox.DataContext as Group_Savings_Accounts).Saving_ID;

                    if (label != null && label.Content != null && saving_ID > 0)
                    {
                        string newValue = textbox.Text;

                        string oldValue = label.Content.ToString();

                        label.Content = newValue;

                        label.Visibility = Visibility.Visible;
                        textbox.Visibility = Visibility.Collapsed;

                        using (var context = new AppDbContext())
                        {

                            if (context.Group_Savings_Accounts.Any(p => p.Name == newValue && p.Saving_ID != saving_ID))
                            {
                                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Tên sổ đã tồn tại.");
                                thongBaoLoi.ShowDialog();
                                return;
                            }

                            var account = context.Group_Savings_Accounts.FirstOrDefault(u => u.Saving_ID == saving_ID);

                            if (account != null)
                            {
                                account.Name = newValue;
                                var index = viewModel.Group_Savings_Accounts.IndexOf(account);

                                if (index > 0)
                                {
                                    viewModel.Group_Savings_Accounts[index] = account;
                                    viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                                }
                                context.SaveChanges();
                            }
                        }

                        Keyboard.ClearFocus();
                    }
                }
            }
        }

        private void btn_rename_Nhom_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var row = FindAncestor<DataGridRow>(button);

            if (row != null)
            {
                var label = FindChild<Label>(row, "lb_tenSTK_nhom");
                var textBox = FindChild<TextBox>(row, "tb_tenSTK_nhom");

                if (label != null && textBox != null)
                {
                    textBox.Text = label.Content.ToString();
                    textBox.Visibility = Visibility.Visible;
                    label.Visibility = Visibility.Collapsed;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void StackPanel_MouseEnter_Nhom(object sender, MouseEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            var username = viewModel?.Username;

            if (username != "")
            {

                var row = FindAncestor<DataGridRow>((sender as DependencyObject)!);
                if (row != null)
                {
                    var button = FindChild<Button>(row, "btn_rename_nhom");

                    var saving_Id = (button.DataContext as Group_Savings_Accounts).Saving_ID;

                    if (button != null && saving_Id > 0 && CheckIsLeaderGroup.IsLeaderGroup(username, saving_Id) == true)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void StackPanel_MouseLeave_Nhom(object sender, MouseEventArgs e)
        {
            var row = FindAncestor<DataGridRow>(sender as DependencyObject);
            if (row != null)
            {
                var button = FindChild<Button>(row, "btn_rename_nhom");
                if (button != null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void tb_Mota_Nhom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                if (textbox != null)
                {
                    var label = FindChild<Label>(FindAncestor<StackPanel>(textbox), "lb_Mota_Nhom");

                    int saving_id = (textbox.DataContext as Group_Savings_Accounts).Saving_ID;

                    using (var context = new AppDbContext())
                    {
                        var account = context.Group_Savings_Accounts.FirstOrDefault(p => p.Saving_ID == saving_id);

                        if (account != null)
                        {
                            if (textbox.Text == "")
                            {
                                account.Description = "Trống";
                            }
                            else
                            {
                                account.Description = textbox.Text;
                            }

                            context.SaveChanges();
                        }
                    }

                    label.Content = textbox.Text != "" ? textbox.Text : "Trống";

                    label.Visibility = Visibility.Visible;
                    textbox.Visibility = Visibility.Collapsed;
                    Keyboard.ClearFocus();
                }
            }
        }

        private void lb_Mota_Nhom_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var label = sender as Label;

            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

            if (label != null && username != "")
            {
                var textbox = FindChild<TextBox>(FindAncestor<StackPanel>(label), "tb_Mota_Nhom");

                var saving_id = (textbox.DataContext as Group_Savings_Accounts).Saving_ID;

                if (textbox != null && saving_id > 0)
                {
                    if (CheckIsLeaderGroup.IsLeaderGroup(username, saving_id) == true)
                    {
                        textbox.Text = label.Content.ToString();
                        textbox.Visibility = Visibility.Visible;
                        label.Visibility = Visibility.Collapsed;
                        textbox.Focus();
                        textbox.SelectAll();
                    }
                }
            }
        }

        private void UpdateDatabase(string newValue, int saving_ID)
        {
            using (var context = new AppDbContext())
            {
                var account = context.Personal_Savings_Accounts.FirstOrDefault(p => p.Saving_ID == saving_ID);

                if (account != null)
                {
                    account.Name = newValue;

                    context.SaveChanges();
                }
            }
        }

        private void btn_rename_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var row = FindAncestor<DataGridRow>(button);

            if (row != null)
            {
                var label = FindChild<Label>(row, "lb_tenSTK");
                var textBox = FindChild<TextBox>(row, "tb_tenSTK");

                if (label != null && textBox != null)
                {
                    textBox.Text = label.Content.ToString();
                    textBox.Visibility = Visibility.Visible;
                    label.Visibility = Visibility.Collapsed;
                    textBox.Focus();
                    textBox.SelectAll();
                }
            }
        }

        private void StackPanel_MouseMove(object sender, MouseEventArgs e)
        {
            var row = FindAncestor<DataGridRow>(sender as DependencyObject);
            if (row != null)
            {
                var button = FindChild<Button>(row, "btn_rename");
                if (button != null)
                {
                    button.Visibility = Visibility.Visible;
                }
            }
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            var row = FindAncestor<DataGridRow>(sender as DependencyObject);
            if (row != null)
            {
                var button = FindChild<Button>(row, "btn_rename");
                if (button != null)
                {
                    button.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region danh dau da xem khi nhan vao xem chi tiet thong bao
        private void Xemchitiet_MouseLeftButtonDown_CaNhan(object sender, MouseButtonEventArgs e)
        {
            var notification = (sender as TextBlock)?.DataContext as Personal_Notifications;

            if (notification != null)
            {
                notification.Is_Open = !notification.Is_Open;
            }

            var row = FindAncestor<DataGridRow>((sender as TextBlock)!);
            var stackPanel = FindChild<StackPanel>(row, "stp_noidung");

            if (stackPanel != null)
            {
                if (notification != null)
                    stackPanel.Visibility = notification.Is_Open ? Visibility.Visible : Visibility.Collapsed;
            }

            if (notification != null && notification.Is_Read == false)
            {
                notification.Is_Read = true;

                using (var context = new AppDbContext())
                {
                    var findNotification = context.Personal_Notifications
                        .Include(p => p.Personal_Savings_Account)
                        .FirstOrDefault(pn => pn.Personal_Notification_ID
                    == notification.Personal_Notification_ID);

                    var viewModel = this.DataContext as GiaoDienChinhViewModel;

                    if (findNotification != null)
                    {
                        findNotification.Is_Read = true;

                        context.SaveChanges();

                        var index = viewModel.Personal_Notifications.ToList().FindIndex(p => p.Personal_Notification_ID
                        == findNotification.Personal_Notification_ID);

                        if (index != -1)
                        {
                            viewModel.Personal_Notifications[index] = findNotification;
                            viewModel.OnPropertyChanged(nameof(viewModel.Personal_Notifications));
                        }
                    }
                }

                var border = FindChild<Border>(row, "bd_thongbao");
                border.Background = (notification.Is_Selected) ?
                    (Brush?)new BrushConverter().ConvertFromString("#F0FFF0") :
                    (Brush?)new BrushConverter().ConvertFromString("#F3F3F3");
                (this.DataContext as GiaoDienChinhViewModel).CNT_ThongBao_ChuaDoc_CaNhan--;
            }
            e.Handled = true;
        }
        #endregion


        #region Cai lich
        private void UpdateWeekView()
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;


            DateTime startOfWeek = viewModel.currentDate.AddDays(-(int)viewModel.currentDate.DayOfWeek); // Start with Sunday
            var textBlocks = new TextBlock[]
            {
                tbl_MonText, tbl_TueText, tbl_WedText, tbl_ThuText, tbl_FriText, tbl_SatText, tbl_SunText
            };

            var borders = new Border[]
            {
                bd_MonText, bd_TueText, bd_WedText, bd_ThuText, bd_FriText, bd_SatText, bd_SunText
            };

            var packicon = new PackIcon[]
            {
                Mon, Tue, Wed, Thu, Fri, Sat, Sun
            };

            foreach (var border in borders)
                border.ToolTip = "";
            foreach (var Packicon in packicon)
                Packicon.Visibility = Visibility.Collapsed;

            for (int i = 0; i < 7; i++)
            {
                DateTime day = startOfWeek.AddDays(i);
                textBlocks[i].Text = day.ToString("dd");
                for (int j = 0; j < viewModel.Personal_Savings_Accounts.Count; j++)
                {
                    if (viewModel.Personal_Savings_Accounts[j].Creating_Date.Day == day.Day && (day.Month -
                        viewModel.Personal_Savings_Accounts[j].Creating_Date.Month) % 
                        viewModel.Personal_Savings_Accounts[j].Interest_Rate.Term == 0) // && Đang họat động
                    {
                        packicon[i].Visibility = Visibility.Visible;
                        borders[i].ToolTip = borders[i].ToolTip + viewModel.Personal_Savings_Accounts[j].Name
                            + " đến ngày đáo hạn (Cá nhân)" + "\n";
                    }
                }

                for (int j = 0; j < viewModel.Group_Savings_Accounts.Count; j++)
                {
                    if (viewModel.Group_Savings_Accounts[j].Creating_Date.Day == day.Day && (day.Month -
                        viewModel.Group_Savings_Accounts[j].Creating_Date.Month) % 
                        viewModel.Group_Savings_Accounts[j].Interest_Rates.Term == 0) // && Đang hoạt động
                    {
                        packicon[i].Visibility = Visibility.Visible;
                        borders[i].ToolTip = borders[i].ToolTip + viewModel.Group_Savings_Accounts[j].Name +
                            " đến ngày đáo hạn (Nhóm)" + "\n";
                    }
                }

                if (day.Date == viewModel.dt.Date)
                {
                    borders[i].BorderBrush = Brushes.Black;
                }
                else borders[i].BorderBrush = Brushes.Transparent;
                if (borders[i].ToolTip.ToString().Length > 0) borders[i].ToolTip =
                        borders[i].ToolTip.ToString().Remove(borders[i].ToolTip.ToString().Length - 1);
                else borders[i].ToolTip = null;
            }

            if (int.Parse(textBlocks[6].Text) < 7)
            {
                foreach (var tb in textBlocks)
                {
                    if (int.Parse(tb.Text) > 7) tb.Foreground = Brushes.Gray;
                }
            }
            else if (textBlocks[0].Foreground == Brushes.Gray)
            {
                foreach (var tb in textBlocks)
                {
                    tb.Foreground = Brushes.Black;
                }
            }

            string Thang = startOfWeek.AddDays(6).Month.ToString();
            string Nam = startOfWeek.AddDays(6).Year.ToString();
            Thangnam.Content = "Tháng " + Thang + " - Năm " + Nam;
        }

        private void PrevWeek_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            viewModel.currentDate = viewModel.currentDate.AddDays(-7);
            UpdateWeekView();
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            viewModel.currentDate = viewModel.currentDate.AddDays(7);
            UpdateWeekView();
        }
        #endregion
      

        #region focus textbox tim kiem
        private void tb_Timkiem_Canhan_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox != null)
            {
                textbox.Focusable = true;
                textbox.Focus();
            }
        }

        private void tb_Timkiem_Canhan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                if (textbox != null)
                    textbox.Focusable = false;
                Keyboard.ClearFocus();
            }
        }
        #endregion

        #region hieu ung di chuot thong bao
        private void bd_thongbao_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.BorderBrush = Brushes.DarkGreen;
                var xemchitiet = FindChild<TextBlock>(border, "Xemchitiet");

                if (xemchitiet != null)
                {
                    xemchitiet.Visibility = Visibility.Visible;
                }
            }
        }

        private void bd_thongbao_MouseLeave(object sender, MouseEventArgs e)
        {
            var notification = (sender as Border)?.DataContext as Personal_Notifications;
            var border = sender as Border;

            if (border != null)
            {
                border.BorderThickness = new Thickness(.5);
                if (notification.Is_Selected == true)
                {
                    border.BorderThickness = new Thickness(1.5);
                    border.BorderBrush = Brushes.Black;
                    border.Background = (Brush?)new BrushConverter().ConvertFromString("#F0FFF0");
                }
                else
                {
                    border.BorderBrush = Brushes.Gray;
                    if (notification?.Is_Read == true)
                    {
                        border.Background = (Brush?)new BrushConverter().ConvertFromString("#F3F3F3");
                    }
                    else
                    {
                        border.Background = Brushes.White;
                    }
                }
                var xemchitiet = FindChild<TextBlock>((sender as Border)!, "Xemchitiet");

                if (xemchitiet != null)
                {
                    xemchitiet.Visibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region button popup so tiet kiem nhom
        private void tbl_MouseEnter(object sender, MouseEventArgs e)
        {
            var textblock = sender as TextBlock;
            textblock.TextDecorations = TextDecorations.Underline;
        }

        private void tbl_MouseLeave(object sender, MouseEventArgs e)
        {
            var textblock = sender as TextBlock;
            textblock.TextDecorations = null;
        }

        private void btn_Huy_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            // code o day

            if (button?.Command?.CanExecute(button.CommandParameter) == true)
            {
                button.Command.Execute(button.CommandParameter);
            }
        }

        private bool IsAnyCheckboxCheckedInItemsControl(ItemsControl itemsControl)
        {
            // Duyệt qua tất cả các phần tử trong ItemsControl
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                // Lấy item hiện tại
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (item != null)
                {
                    // Kiểm tra checkbox trong item
                    if (CheckForCheckedCheckbox(item))
                    {
                        return true; // Nếu có checkbox được chọn, trả về true
                    }
                }
            }

            return false; // Không có checkbox nào được chọn
        }

        private bool CheckForCheckedCheckbox(FrameworkElement element)
        {
            // Kiểm tra nếu phần tử này là một checkbox và được chọn
            if (element is CheckBox checkBox && checkBox.IsChecked == true)
            {
                return true;
            }

            // Duyệt qua các phần tử con trong cây trực quan (Visual Tree)
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child != null)
                {
                    // Đệ quy tìm checkbox đã được chọn trong phần tử con
                    if (CheckForCheckedCheckbox(child))
                    {
                        return true;
                    }
                }
            }

            return false; // Không có checkbox nào được chọn
        }

        // datapiker

        private void btn_Timkiem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            // code o day

            var grid = FindAncestor<Grid>(VisualTreeHelper.GetParent(FindAncestor<Grid>(sender as Button)));
            var buttonDatlai = FindChild<Button>(grid, "btn_Datlai");
            var datepicker = FindChild<DatePicker>(grid, "datepicker");
            var itemcontrol = FindChild<ItemsControl>(grid, "itemcontrol");

            var context = datepicker.DataContext as Group_Savings_Accounts;
            ObservableCollection<Users> users = new ObservableCollection<Users>();


            var checkedItems = new List<string>();
            foreach (var item in itemcontrol.Items)
            {
                var container = itemcontrol.ItemContainerGenerator.ContainerFromItem(item) as ContentPresenter;
                if (container != null)
                {
                    // Tìm CheckBox trong VisualTree của container
                    var checkBox = FindVisualChild<CheckBox>(container);
                    if (checkBox != null && checkBox.IsChecked == true)
                    {
                        var _checkbox = checkBox.DataContext as Group_Details;
                        users.Add(_checkbox.User);

                    }
                }


            }

            var view = CollectionViewSource.GetDefaultView(context.Group_Transactions_Information);
            if (datepicker.SelectedDate != null || users.Count != 0)
            {
                view.Filter = item =>
                {
                    var transaction = item as Group_Transactions_Information;
                    if (datepicker.SelectedDate == null)
                        return transaction != null && users.Any(p => p.Username == transaction.Username);
                    if (users.Count == 0)
                        return transaction != null && transaction.Transaction_Date.Date == datepicker.SelectedDate;

                    return transaction != null && transaction.Transaction_Date.Date == datepicker.SelectedDate && users.Any(p => p.Username == transaction.Username);
                };

                view.Refresh();
            }




            if (buttonDatlai != null && (IsAnyCheckboxCheckedInItemsControl(itemcontrol) || datepicker.SelectedDate != null)) buttonDatlai.Visibility = Visibility.Visible;
            if (button?.Command?.CanExecute(button.CommandParameter) == true)
            {
                button.Command.Execute(button.CommandParameter);
            }




        }

        private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T childOfType)
                    return childOfType;

                var childInChild = FindVisualChild<T>(child);
                if (childInChild != null)
                    return childInChild;
            }
            return null;
        }


        private void UncheckAllCheckboxesInItemsControl(ItemsControl itemsControl)
        {
            for (int i = 0; i < itemsControl.Items.Count; i++)
            {
                var item = itemsControl.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;

                if (item != null)
                {
                    FindAndUncheckCheckboxes(item);
                }
            }
        }

        private void FindAndUncheckCheckboxes(FrameworkElement element)
        {
            if (element is CheckBox checkBox)
            {
                checkBox.IsChecked = false;
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;
                if (child != null)
                {
                    FindAndUncheckCheckboxes(child);
                }
            }
        }


        private void btn_Datlai_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var grid = FindAncestor<Grid>(VisualTreeHelper.GetParent(FindAncestor<Grid>(sender as Button)));
            var datepicker = FindChild<DatePicker>(grid, "datepicker");
            datepicker.SelectedDate = null;
            var itemcontrol = FindChild<ItemsControl>(grid, "itemcontrol");
            UncheckAllCheckboxesInItemsControl(itemcontrol);
            var buttonDatlai = FindChild<Button>(grid, "btn_Datlai");
            buttonDatlai.Visibility = Visibility.Collapsed;

            // code o day
            var context = datepicker.DataContext as Group_Savings_Accounts;
            var view = CollectionViewSource.GetDefaultView(context.Group_Transactions_Information);
            view.Filter = null; // Loại bỏ bộ lọc
            view.Refresh(); // Làm mới chế độ xem

            view.Refresh();
            if (button?.Command?.CanExecute(button.CommandParameter) == true)
            {
                button.Command.Execute(button.CommandParameter);
            }
        }
        #endregion

        #region hieu ung nhan chuot thong bao
        private void bd_thongbao_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            var notification = (sender as Border)?.DataContext as Personal_Notifications;

            if (!notification.Is_Read) return;

            var border = sender as Border;

            if (border != null)
                border.BorderBrush = Brushes.DarkGreen;

            if (notification.Is_Selected == false)
            {
                if (border != null)
                    border.Background = (Brush?)new BrushConverter().ConvertFromString("#F0FFF0");
                notification.Is_Selected = true;
                SelectedItem.Add(border!);
            }
            else
            {
                if (notification?.Is_Read == true)
                {
                    border.Background = (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
                }
                else
                {
                    border.Background = Brushes.White;
                }
                notification.Is_Selected = false;
                SelectedItem.Remove(border);
            }

            btn_Xoa_Canhan.Visibility = (SelectedItem.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region xoa thong bao ca nhan
        private void btn_Xoa_Click(object sender, RoutedEventArgs e)
        {
            ThongBaoXoa thongBaoXoa = new ThongBaoXoa("Bạn có muốn xoá các thông báo được chọn không?");
            thongBaoXoa.ShowDialog();

            var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;

            if (giaoDienChinhViewModel != null)
            {
                if (thongBaoXoa.DialogResult == true)
                {
                    var selectedItemsCopy = SelectedItem.ToList();

                    foreach (var border in selectedItemsCopy)
                    {
                        var notification = border.DataContext as Personal_Notifications;
                        SelectedItem.Remove(border); 
                        giaoDienChinhViewModel.Personal_Notifications.Remove(notification);
                        giaoDienChinhViewModel.DeletePersonalNotifications(notification);

                        if (notification.Is_Read == false)
                        {
                            giaoDienChinhViewModel.CNT_ThongBao_ChuaDoc_CaNhan--;
                        }

                        giaoDienChinhViewModel.OnPropertyChanged(nameof(giaoDienChinhViewModel.Personal_Notifications));
                        btn_Xoa_Canhan.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        #endregion

        #region nhan chip thanh vien so tiet kiem nhom
        private void chip_Thanhvien_Click(object sender, RoutedEventArgs e)
        {
            var chip = sender as Chip;
            if (chip != null)
            {
                var label = FindChild<Label>(chip, "lb_thanhvien_username");
                label.Visibility = label.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void chip_TrgNhom_Click(object sender, RoutedEventArgs e)
        {
            var chip = sender as Chip;
            if (chip != null)
            {
                var label = FindChild<Label>(chip, "lb_TrgNhom_Username");
                if (label != null)
                {
                    label.Visibility = label.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region scroll danh sach thanh vien nhom
        private void ScrollLeft_Click(object sender, RoutedEventArgs e)
        {
            var stackpanel = FindAncestor<StackPanel>(sender as RepeatButton);
            if (stackpanel != null)
            {
                var scrollviewer = FindChild<ScrollViewer>(stackpanel, "scrollViewer");
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - 10);
            }
        }

        private void ScrollRight_Click(object sender, RoutedEventArgs e)
        {
            var stackpanel = FindAncestor<StackPanel>(sender as RepeatButton);
            if (stackpanel != null)
            {
                var scrollviewer = FindChild<ScrollViewer>(stackpanel, "scrollViewer");
                scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + 10);
            }
        }
        #endregion

        private void lb_Muctieu_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = sender as Label;
            var textbox = FindChild<TextBox>(FindAncestor<StackPanel>(label), "tb_Muctieu");

            if (textbox != null)
            {
                textbox.Text = label.Content.ToString();
                textbox.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Collapsed;
                textbox.Focus();
                textbox.SelectAll();
            }
        }

        private void lb_Muctieu_Nhom_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var label = sender as Label;
            var textbox = FindChild<TextBox>(FindAncestor<StackPanel>(label), "tb_Muctieu_Nhom");

            if (textbox != null)
            {

                var username = (this.DataContext as GiaoDienChinhViewModel).Username;
                var saving_id = (textbox.DataContext as Group_Savings_Accounts).Saving_ID;

                if (username != "" && saving_id > 0 && CheckIsLeaderGroup.IsLeaderGroup(username, saving_id))
                {
                    textbox.Text = label.Content.ToString();
                    textbox.Visibility = Visibility.Visible;
                    label.Visibility = Visibility.Collapsed;
                    textbox.Focus();
                    textbox.SelectAll();
                }
            }
        }

        private void tb_Muctieu_Nhom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                var label = FindChild<Label>(FindAncestor<StackPanel>(textbox), "lb_Muctieu_Nhom");

                using (var context = new AppDbContext())
                {
                    var saving_id = (textbox.DataContext as Group_Savings_Accounts).Saving_ID;
                    var username = (this.DataContext as GiaoDienChinhViewModel).Username;

                    if (saving_id > 0 && username != "")
                    {
                        var account = context.Group_Savings_Accounts.FirstOrDefault(p =>
                        p.Saving_ID == saving_id);

                        if (account != null)
                        {
                            if (long.TryParse(textbox.Text, out long target))
                            {
                                account.Target = target;
                                label.Content = string.Format("{0:N0} VNĐ", textbox.Text);
                            }
                            else
                            {
                                account.Target = null;
                                label.Content = "Không có";
                            }

                            context.SaveChanges();

                            var viewModel = this.DataContext as GiaoDienChinhViewModel;

                            if (viewModel != null)
                            {
                                var index = viewModel.Group_Savings_Accounts.IndexOf(account);

                                var specificSavingAccount = context.Group_Savings_Accounts
                                        .Include(p => p.Group_Details)
                                            .ThenInclude(d => d.User)
                                        .Include(t => t.Group_Transactions_Information)
                                            .ThenInclude(f => f.User)
                                        .Include(e => e.Interest_Rates)
                                        .Where(p => p.Saving_ID == account.Saving_ID && p.Status == "Đang hoạt động")
                                        .FirstOrDefault(); 

                                if (index != -1)
                                {
                                    viewModel.Group_Savings_Accounts[index] = specificSavingAccount;
                                    viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                                }
                            }
                        }
                    }
                }


                label.Visibility = Visibility.Visible;

                textbox.Visibility = Visibility.Collapsed;

                Keyboard.ClearFocus();
            }
        }

        private void tb_Muctieu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textbox = sender as TextBox;
                var label = FindChild<Label>(FindAncestor<StackPanel>(textbox), "lb_Muctieu");


                using (var context = new AppDbContext())
                {
                    var saving_id = (textbox.DataContext as Personal_Savings_Accounts).Saving_ID;
                    var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                    if (saving_id > 0 && username != "")
                    {
                        var account = context.Personal_Savings_Accounts.FirstOrDefault(p => 
                        p.Saving_ID == saving_id);

                        if (account != null)
                        {
                            if (long.TryParse(textbox.Text, out long target))
                            {
                                account.Target = target;
                            } else
                            {
                                account.Target = null;
                            }

                            if (textbox.Text == "")
                            {
                                label.Content = "Không có";
                            } else
                            {
                                label.Content = string.Format("{0:N0} VNĐ", textbox.Text);
                            }

                            label.Visibility = Visibility.Visible;

                            textbox.Visibility = Visibility.Collapsed;

                            Keyboard.ClearFocus();

                            context.SaveChanges();

                            var viewModel = this.DataContext as GiaoDienChinhViewModel;

                            if (viewModel != null)
                            {
                                var updatedAccount = viewModel.Personal_Savings_Accounts
                                    .FirstOrDefault(acc => acc.Saving_ID == saving_id);

                                if (updatedAccount != null)
                                {
                                    updatedAccount.Target = account.Target;
                                    viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                                }
                            }
                        }
                    }
                }
            }
        }

        private async void btn_Taoso_Canhan_Click(object sender, RoutedEventArgs e)
        {
            var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;

            if (giaoDienChinhViewModel != null)
            {
                GiaoDienTaoSoTietKiemCaNhan dgSoCaNhan = new GiaoDienTaoSoTietKiemCaNhan(giaoDienChinhViewModel);
                dgSoCaNhan.ShowDialog();
            }
        }

        private async void btn_ChuyenTien_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (button != null && viewModel != null && viewModel.User != null)
            {
                var savingsAccount = button.CommandParameter as Personal_Savings_Accounts;

                if (savingsAccount != null)
                {
                    string name = savingsAccount.Name;
                    long money = savingsAccount.Money;
                    int saving_ID = savingsAccount.Saving_ID;


                  GiaoDienChuyenTien giaoDienChuyenTien = new GiaoDienChuyenTien(savingsAccount, 
                      viewModel.User);
                    giaoDienChuyenTien.DataContext = this.DataContext as GiaoDienChinhViewModel;
                    giaoDienChuyenTien.ShowDialog();
                }
            }
        }

        private void btn_RutTien_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (button != null && viewModel != null)
            {
                var savingsAccount = button.CommandParameter as Personal_Savings_Accounts;

                if (savingsAccount != null)
                {
                    string name = savingsAccount.Name;
                    long money = savingsAccount.Money;
                    int saving_ID = savingsAccount.Saving_ID;


                    GiaoDienRutTien giaoDienRutTien = new GiaoDienRutTien(savingsAccount);
                    giaoDienRutTien.DataContext = this.DataContext as GiaoDienChinhViewModel;
                    giaoDienRutTien.ShowDialog();
                }
            }
        }

        private void btn_NapTien_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            var viewModel = this.DataContext as GiaoDienChinhViewModel;


            if (button != null && viewModel != null && viewModel.User != null)
            {
                var savingsAccount = button.CommandParameter as Personal_Savings_Accounts;

                if (savingsAccount != null)
                {
                    GiaoDienNapTien giaoDienNapTien = new GiaoDienNapTien(savingsAccount, viewModel.User);

                    giaoDienNapTien.DataContext = this.DataContext as GiaoDienChinhViewModel;
                    giaoDienNapTien.ShowDialog();
                }
            }
        }

        private void tb_Timkiem_Canhan_TextChanged(object sender, System.EventArgs e)
        {
            string searchText = tb_Timkiem_Canhan.Text;

            (this.DataContext as GiaoDienChinhViewModel).SearchQuery = searchText;
        }

        private void tb_Timkiem_Nhom_TextChanged(object sender, System.EventArgs e)
        {
            string searchText = tb_Timkiem_Nhom.Text;

            (this.DataContext as GiaoDienChinhViewModel).SearchQuery_Nhom = searchText;         
        }


        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            var datePicker = sender as DatePicker;

            var context = datePicker.DataContext as Personal_Savings_Accounts;

            if (context != null)
            {
                var view = CollectionViewSource.GetDefaultView(context.Personal_Transactions_Information);

                if (datePicker.SelectedDate.HasValue)
                {
                    // Nếu có ngày được chọn, áp dụng bộ lọc
                    DateTime selectedDate = datePicker.SelectedDate.Value;

                    view.Filter = item =>
                    {
                        var transaction = item as Personal_Transactions_Information;
                        return transaction != null && transaction.Transaction_Date.Date == selectedDate.Date;
                    };
                }
                else
                {
                    view.Filter = null;
                }

                view.Refresh();
            }
        }

        private void btn_xoaSoSTK_Click(object sender, RoutedEventArgs e)
        {
            ThongBaoXoa thongBaoXoa = new ThongBaoXoa("Bạn có muốn xoá sổ tiết kiệm này không?");
            thongBaoXoa.ShowDialog();

            if (thongBaoXoa.DialogResult == true)
            {
                var button = sender as Button;

                if (button != null)
                {
                    var saving_id = (button.DataContext as Personal_Savings_Accounts)?.Saving_ID;

                    if (saving_id != null)
                    {
                        using (var context = new AppDbContext())
                        {
                            var saving_account = context.Personal_Savings_Accounts.Find(saving_id);

                            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

                            var viewModel = this.DataContext as GiaoDienChinhViewModel;

                            if (saving_account != null && username != "")
                            {
                                saving_account.Status = "Không hoạt động";

                                context.SaveChanges();

                                int index = viewModel.Personal_Savings_Accounts
    .ToList()
    .FindIndex(account => account.Saving_ID == saving_account.Saving_ID);

                                if (index != -1)
                                {
                                    viewModel.Personal_Savings_Accounts.RemoveAt(index);
                                    viewModel.IsEmptyAccount = viewModel.Personal_Savings_Accounts.Count == 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = this.Parent as ContentControl;
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            var username = viewModel?.Username;

            if (parent != null && username != "" && viewModel != null)
            {
                var giaoDienThongTin = new GiaoDienThongTinNguoiDung(giaoDien1);
                viewModel.LoadUserInfoAsync(username);
                ThongKe(username);
                giaoDienThongTin.DataContext = viewModel;
                parent.Margin = new Thickness(0);
                parent.Content = giaoDienThongTin;
            }
        }

        private void ThongKe(string username)
        {
            if (username != "")
            {
                using (var context = new AppDbContext())
                {
                    var viewModel = this.DataContext as GiaoDienChinhViewModel;

                    if (viewModel != null)
                    {
                        long tongTienCaNhan = context.Personal_Savings_Accounts
                            .Where(x => x.Username == username)
                            .Sum(a => a.Money);

                         long tongTienNhom = context.Group_Details
                            .Where(x => x.User.Username == username)
                            .Sum(a => a.Total_Money);

                        int soLuongSoCaNhan = context.Personal_Savings_Accounts
                                                .Where(x => x.Username == username && x.Status == "Đang hoạt động") 
                                                 .Count();

                        int soLuongSoNhom = context.Group_Details
    .Where(gd => gd.Username == username)
    .Select(gd => gd.Saving_ID) 
    .Distinct()
    .Count();

                        viewModel.PersonalSavings = new ChartValues<long> { tongTienCaNhan };
                        viewModel.GroupSavings = new ChartValues<long> { tongTienNhom };
                        viewModel.PersonalAccountCount = new ChartValues<int> { soLuongSoCaNhan };
                        viewModel.GroupAccountCount = new ChartValues<int> { soLuongSoNhom };
                        viewModel.OnPropertyChanged(nameof(viewModel.PersonalSavings));
                        viewModel.OnPropertyChanged(nameof(viewModel.GroupSavings));
                        viewModel.OnPropertyChanged(nameof(viewModel.PersonalAccountCount));
                        viewModel.OnPropertyChanged(nameof(viewModel.GroupAccountCount));
                    }
                }
            }
        }

        private void Lammoi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GiaoDienChinh gdChinh = new GiaoDienChinh(giaoDien1);
            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                parent.Content = gdChinh;
            }
        }

        private void btn_Lammoi_MouseEnter(object sender, MouseEventArgs e)
        {
            lammoi.Foreground = Brushes.Blue;
            icon.Foreground = Brushes.Blue;
        }

        private void btn_Lammoi_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;
            PackIcon rotatingTextBlock = icon;
            if (rotatingTextBlock.RenderTransform is RotateTransform rotateTransform)
            {
                rotateTransform.CenterX = rotatingTextBlock.ActualWidth / 2;
                rotateTransform.CenterY = rotatingTextBlock.ActualHeight / 2;

                DoubleAnimation rotateAnimation = new DoubleAnimation
                {
                    By = 360,
                    Duration = TimeSpan.FromSeconds(.5),
                    RepeatBehavior = new RepeatBehavior(1)
                };

                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
            }
            if (viewModel != null)
            {
                var username = viewModel.Username;
                if (username != "")
                {
                    viewModel.LoadUserInfoAsync(username);
                    viewModel.LoadGroupSavingsAccounts(username);
                    viewModel.LoadPersonalNotifications(username);
                    viewModel.LoadPersonalSavingsAccounts(username);
                }
            }
            
        }

        // nhóm

        private void btn_Taoso_Nhom_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as GiaoDienChinhViewModel;

            if (viewModel != null && viewModel?.Username != "")
            {
                GiaoDienTaoSoNhom tao_So_Tiet_Kiem_Nhom = new GiaoDienTaoSoNhom();
                tao_So_Tiet_Kiem_Nhom.DataContext = viewModel;
                tao_So_Tiet_Kiem_Nhom.ShowDialog();
            }
        }

        private void tb_Naptien_Nhom_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            var savingsAccount = btn.CommandParameter as Group_Savings_Accounts;

            if (btn != null && savingsAccount != null)
            {
                int saving_ID = savingsAccount.Saving_ID;

                var viewModel = this.DataContext as GiaoDienChinhViewModel;

                if (viewModel != null && viewModel?.Username != "")
                {
                    GiaoDienNapTienNhom napTien_Nhom = new GiaoDienNapTienNhom(savingsAccount, viewModel.User);
                    napTien_Nhom.DataContext = viewModel;
                    napTien_Nhom.ShowDialog();
                }
            }
        }

        private async void bt_Ruttien_Nhom_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            var savingsAccount = btn.CommandParameter as Group_Savings_Accounts;

            if (btn != null && savingsAccount != null)
            {
                int saving_ID = savingsAccount.Saving_ID;
                var viewModel = this.DataContext as GiaoDienChinhViewModel;

                if (viewModel != null && viewModel?.Username != "")
                {
                    GiaoDienRutTienNhom rutTien_Nhom = new 
                        GiaoDienRutTienNhom(savingsAccount);
                    rutTien_Nhom.DataContext = viewModel;
                    rutTien_Nhom.ShowDialog();

                    await Task.Delay(200);

                    if (rutTien_Nhom.DialogResult == true)
                    {
                        ThongBaoThanhCong thongBaoThanhCong =
    new ThongBaoThanhCong("Thành công", "Đã gửi yêu cầu tới trưởng nhóm.");
                        thongBaoThanhCong.ShowDialog();
                    }
                }
            }
        }

       
        private void bt_Xoaso_Nhom_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var savingsAccount = btn.CommandParameter as Group_Savings_Accounts;

            ThongBaoXoa thongBaoXoa = new ThongBaoXoa("Xóa sổ tiết kiệm nhóm ?");
            thongBaoXoa.ShowDialog();
            if (thongBaoXoa.DialogResult == true)
                if (btn != null && savingsAccount != null)
                {
                    int saving_ID = savingsAccount.Saving_ID;
                    var viewModel = this.DataContext as GiaoDienChinhViewModel;
                    using (var context = new AppDbContext())
                    {
                        // Tìm sổ tiết kiệm nhóm theo ID
                        var saving = context.Group_Savings_Accounts.FirstOrDefault(s => s.Saving_ID == saving_ID);

                        if (saving != null)
                        {


                            var groupDetails = context.Group_Details
                            .Where(gd => gd.Saving_ID == savingsAccount.Saving_ID && gd.Username != viewModel.Username)
                            .ToList();
                            var groupNotification = new Group_Notifications
                            {
                                Description = "Trưởng nhóm đã xóa sổ tiết kiệm nhóm !",
                                Type = "Xóa",
                                Notification_Date = DateTime.Now,
                                Username_Sender = viewModel.Username,
                                Saving_ID = savingsAccount.Saving_ID
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

                            // Cập nhật trạng thái của sổ tiết kiệm nhóm thành "Không hoạt động"
                            saving.Status = "Không hoạt động";
                            // Lưu tất cả thay đổi vào cơ sở dữ liệu
                            var existingAccount = viewModel.Group_Savings_Accounts
                       .FirstOrDefault(p => p.Saving_ID == savingsAccount.Saving_ID);

                            if (existingAccount != null)
                            {
                                viewModel.Group_Savings_Accounts.Remove(existingAccount);
                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                            }

                            context.SaveChanges();

                            viewModel.IsEmptyAccountNhom = viewModel.Group_Savings_Accounts.Count == 0;

                        }
                        else
                        {
                            ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Không tìm thấy sổ tiết kiệm với ID được cung cấp!");
                            thongBaoLoi.ShowDialog();
                           
                        }
                    }
                }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var notificationDetail = (sender as Button)?.DataContext as Group_Notifications_Details;
            var username = (this.DataContext as GiaoDienChinhViewModel)?.Username;

            if (username == "") return;

            if (notificationDetail == null)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Thông báo không hợp lệ!");
                thongBaoLoi.ShowDialog();
                return;
            }

            using (var dbContext = new AppDbContext())
            {
                try
                {
                    // Lấy thông báo gốc
                    var notification = dbContext.Group_Notifications
    .Include(n => n.Group_Savings_Accounts) // Tải dữ liệu từ Group_Savings_Accounts
    .FirstOrDefault(p => p.Group_Notification_ID == notificationDetail.Group_Notification_ID);

                    if (notification == null)
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Không tìm thấy thông báo!");
                        thongBaoLoi.ShowDialog();
                        return;
                    }

                    // Lấy chi tiết thông báo
                    var notificationDetailDb = dbContext.Group_Notifications_Details.FirstOrDefault(d =>
                        d.Group_Notification_ID == notification.Group_Notification_ID &&
                        d.Username == username); // Người đang thực hiện từ chối

                    if (notificationDetailDb == null)
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Không tìm thấy chi tiết thông báo!");
                        thongBaoLoi.ShowDialog();
                        return;
                    }

                    // Cập nhật trạng thái thông báo chi tiết
                    notificationDetailDb.Status = "Không đồng ý";

                    notificationDetailDb.Is_Deleted = false; // Giữ thông báo này trong hệ thống

                    // Trường hợp 1: Type = "RútYC"
                    if (notification.Type == "RútYC")
                    {
                        // Tạo thông báo phản hồi cho người gửi
                        var reverseNotification = new Group_Notifications
                        {
                            Saving_ID = notification.Saving_ID,
                            Money = notification.Money,
                            Description = "Không đồng ý",
                            Type = "RútPH",
                            Notification_Date = DateTime.Now,
                            Username_Sender = username, // Người thực hiện từ chối,
                        };

                        dbContext.Group_Notifications.Add(reverseNotification);
                        dbContext.SaveChanges(); // Lưu để lấy ID thông báo

                        // Thêm chi tiết thông báo phản hồi
                        var reverseNotificationDetail = new Group_Notifications_Details
                        {
                            Group_Notification_ID = reverseNotification.Group_Notification_ID,
                            Username = notification.Username_Sender, // Người gửi yêu cầu ban đầu
                            Status = "Chưa đọc",
                            Is_Deleted = false
                        };

                        dbContext.Group_Notifications_Details.Add(reverseNotificationDetail);
                    }

                    // Trường hợp 2: Type = "MờiYC"
                    else if (notification.Type == "MờiYC")
                    {
                        // Tạo thông báo phản hồi cho người gửi
                        var reverseNotification = new Group_Notifications
                        {
                            Saving_ID = notification.Saving_ID,
                            Description = "Từ chối",
                            Type = "MờiPH",
                            Notification_Date = DateTime.Now,
                            Username_Sender = username // Người thực hiện từ chối
                        };
                        dbContext.Group_Notifications.Add(reverseNotification);
                        dbContext.SaveChanges(); // Lưu để lấy ID thông báo

                        // Thêm chi tiết thông báo phản hồi
                        var reverseNotificationDetail = new Group_Notifications_Details
                        {
                            Group_Notification_ID = reverseNotification.Group_Notification_ID,
                            Username = notification.Username_Sender, // Người gửi yêu cầu ban đầu
                            Status = "Chưa đọc",
                            Is_Deleted = false
                        };

                        dbContext.Group_Notifications_Details.Add(reverseNotificationDetail);
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    dbContext.SaveChanges();

                    var viewModel = (this.DataContext as GiaoDienChinhViewModel);

                    if (viewModel != null)
                    {
                        var element = dbContext.Group_Notifications_Details
     .Include(details => details.Group_Notifications) // Tải Group_Notifications
         .ThenInclude(notification => notification.User_Sender) // Tải User_Sender từ Group_Notifications
     .Include(details => details.Group_Notifications) // Lặp lại để tải Group_Savings_Accounts
         .ThenInclude(notification => notification.Group_Savings_Accounts) // Tải Group_Savings_Accounts qua Group_Notifications
             .ThenInclude(saving => saving.Interest_Rates) // Tải Interest_Rates qua Group_Savings_Accounts
     .FirstOrDefault(details_ =>
        
         details_.Username == username &&
         details_.Is_Deleted == false &&
         details_.Group_Notification_ID == notificationDetailDb.Group_Notification_ID &&  
         !(details_.Group_Notifications.Type == "MờiYC" &&
                              details_.Group_Notifications.Group_Savings_Accounts.Status == "Không hoạt động"));


                        var index = viewModel.Group_Notifications_Details
                     .ToList()
                   .FindIndex(p => p.Group_Notification_ID == notificationDetailDb.Group_Notification_ID);

                        if (index != null)
                        {
                            viewModel.Group_Notifications_Details[index] = element;
                            viewModel.OnPropertyChanged(nameof(viewModel.Group_Notifications_Details));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Cập nhật giao diện (màu nền của thông báo)
            var border = FindAncestor<Border>(sender as Button);

            if (border != null)
            {
                border.Background = (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
            }
        }

        private void btn_Dongy_Click(object sender, RoutedEventArgs e)
        {
            // Cập nhật giao diện
            var notificationDetail = (sender as Button)?.DataContext as Group_Notifications_Details;

            var username = (this.DataContext as GiaoDienChinhViewModel).Username;

            if (username == "") return;

            if (notificationDetail == null)
            {
                ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Dữ liệu thông báo không hợp lệ!");
                thongBaoLoi.ShowDialog();
                return;
            }

            int groupNotificationId = notificationDetail.Group_Notification_ID;

            using (var dbContext = new AppDbContext())
            {
                try
                {
                    var notification = dbContext.Group_Notifications
    .Include(n => n.Group_Savings_Accounts) 
    .FirstOrDefault(p => p.Group_Notification_ID == groupNotificationId);

                    if (notification == null)
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Thông báo không tồn tại!");
                        thongBaoLoi.ShowDialog();
                        return;
                    }

                    // Lấy thông tin chi tiết thông báo
                    var notificationDetailDb = dbContext.Group_Notifications_Details.FirstOrDefault(d =>
                        d.Group_Notification_ID == groupNotificationId &&
                        d.Username == username); // username: Người đang xử lý

                    if (notificationDetailDb == null)
                    {
                        ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Thông tin thông báo không tồn tại!");
                        thongBaoLoi.ShowDialog();
                        return;
                    }

                    // Trường hợp 1: "RútYC"
                    if (notification.Type == "RútYC")
                    {
                        var groupSaving = dbContext.Group_Savings_Accounts.FirstOrDefault(g => 
                        g.Saving_ID == notification.Group_Savings_Accounts.Saving_ID);


                        if (groupSaving == null)
                        {
                            ThongBaoLoi thongBaoLoi = new ThongBaoLoi("Sổ tiết kiệm không tồn tại!");
                            thongBaoLoi.ShowDialog();
                            return;
                        }

                        // Cập nhật số tiền trong sổ tiết kiệm
                        groupSaving.Money -= notification.Money ?? 0;


                        // Cộng tiền vào tài khoản người gửi thông báo
                        var userNguoiGui = dbContext.Users.FirstOrDefault(u => u.Username == 
                        notification.Username_Sender);

                        if (userNguoiGui != null)
                        {
                            userNguoiGui.Money += notification.Money ?? 0;

                            // Cập nhật số tiền đóng góp
                            var groupDetails = dbContext.Group_Details.FirstOrDefault(p => p.Username ==
                            userNguoiGui.Username
                            && p.Saving_ID == groupSaving.Saving_ID);

                            if (groupDetails != null)
                            {
                                groupDetails.Total_Money -= notification.Money ?? 0;
                            }
                        }

                        // Cập nhật trạng thái chi tiết thông báo
                        notificationDetailDb.Status = "Đồng ý";

                        // tạo giao dịch
                        var transaction_Information = new Group_Transactions_Information
                        {
                            Transaction_Date = DateTime.Now,
                            Money = (notification.Money * (-1)) ?? 0,
                            Description = notification.Description != "" ? notification.Description : "Trống",
                            Saving_ID = groupSaving.Saving_ID,
                            Username = notification.Username_Sender,
                        };

                        dbContext.Group_Transactions_Information.Add(transaction_Information);

                        // Thêm thông báo phản hồi
                        var reverseNotification = new Group_Notifications
                        {
                            Saving_ID = notification.Saving_ID,
                            Money = notification.Money,
                            Description = "Đồng ý",
                            Type = "RútPH",
                            Notification_Date = DateTime.Now,
                            Username_Sender = username // Người thực hiện hành động
                        };

                        var viewModel = this.DataContext as GiaoDienChinhViewModel;

                        dbContext.Group_Notifications.Add(reverseNotification);
                        dbContext.SaveChanges(); // Lưu để lấy Group_Notification_ID

                        var reverseNotificationDetail = new Group_Notifications_Details
                        {
                            Group_Notification_ID = reverseNotification.Group_Notification_ID,
                            Username = notification.Username_Sender, // Người gửi ban đầu
                            Status = "Chưa đọc",
                            Is_Deleted = false,
                        };

                        dbContext.Group_Notifications_Details.Add(reverseNotificationDetail);

                        // Thêm thông báo cho các thành viên còn lại trong nhóm
                        var groupMembers = dbContext.Group_Details.Where(g => g.Saving_ID == 
                        groupSaving.Saving_ID && g.Username != notification.Username_Sender).ToList();

                        var memberNotification = new Group_Notifications
                        {
                            Saving_ID = notification.Saving_ID,
                            Money = notification.Money * (-1),
                            Description = notification.Description,
                            Type = "Rút",
                            Notification_Date = DateTime.Now,
                            Username_Sender = notification.Username_Sender // Người gửi ban đầu
                        };
                               
                        dbContext.Group_Notifications.Add(memberNotification);
                        dbContext.SaveChanges(); // Lưu để lấy Group_Notification_ID

                        foreach (var member in groupMembers)
                        {
                            var memberNotificationDetail = new Group_Notifications_Details
                            {
                                Group_Notification_ID = memberNotification.Group_Notification_ID,
                                Username = member.Username,
                                Status = "Chưa đọc",
                                Is_Deleted = false
                            };

                            dbContext.Group_Notifications_Details.Add(memberNotificationDetail);
                            dbContext.SaveChanges();

                            var indexDetails = viewModel.Group_Notifications_Details.ToList()
                                .FindIndex(p => p.Group_Notification_ID == memberNotificationDetail.Group_Notification_ID
                                && p.Username == username);

                            if (indexDetails == -1)
                            {
                                viewModel.Group_Notifications_Details.Insert(0, memberNotificationDetail);
                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Notifications_Details));
                            }
                        }

                        if (viewModel != null)
                        {
                            var element = dbContext.Group_Notifications_Details
    .Include(details => details.Group_Notifications) // Tải Group_Notifications
        .ThenInclude(notification => notification.User_Sender) // Tải User_Sender từ Group_Notifications
    .Include(details => details.Group_Notifications) // Lặp lại để tải Group_Savings_Accounts
        .ThenInclude(notification => notification.Group_Savings_Accounts) // Tải Group_Savings_Accounts qua Group_Notifications
            .ThenInclude(saving => saving.Interest_Rates) // Tải Interest_Rates qua Group_Savings_Accounts
    .FirstOrDefault(details_ =>
       
        details_.Username == username &&
        details_.Is_Deleted == false &&
        details_.Group_Notification_ID == notificationDetailDb.Group_Notification_ID &&
        !(details_.Group_Notifications.Type == "MờiYC" &&
                              details_.Group_Notifications.Group_Savings_Accounts.Status == "Không hoạt động"));


                            var index = viewModel.Group_Notifications_Details
                         .ToList()
                       .FindIndex(p => p.Group_Notification_ID == notificationDetailDb.Group_Notification_ID);

                            if (index != -1)
                            {
                                viewModel.Group_Notifications_Details[index] = element;
                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Notifications_Details));

                                viewModel.Group_Savings_Accounts = new ObservableCollection<Group_Savings_Accounts>(
                                dbContext.Group_Savings_Accounts
                                    .Include(p => p.Group_Details)
                                        .ThenInclude(d => d.User)
                                    .Include(t => t.Group_Transactions_Information)
                                        .ThenInclude(f => f.User)
                                    .Include(e => e.Interest_Rates)
                                    .Where(p => p.Group_Details.Any(d => d.Username
                                    == username) && p.Status == "Đang hoạt động")
                                    .OrderByDescending(p => p.Creating_Date)
                                    .ToList()
                                 );

                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                            }
                        }

                        dbContext.SaveChanges();
                    }

                    // Trường hợp 2: "MờiYC"
                    else if (notification.Type == "MờiYC")
                    {
                        // Cập nhật trạng thái chi tiết thông báo
                        notificationDetailDb.Status = "Đồng ý";

                        // Thêm User hiện tại vào Group_Details
                        var groupDetail = new Group_Details
                        {
                            Saving_ID = notification.Group_Savings_Accounts.Saving_ID,
                            Username = username, // Người hiện tại
                            Is_Owner = false, // Không phải chủ sở hữu
                            Total_Money = 0 // Tổng tiền là 0 ban đầu
                        };

                        dbContext.Group_Details.Add(groupDetail);

                        // Thêm thông báo phản hồi
                        var reverseNotification = new Group_Notifications
                        {
                            Saving_ID = notification.Saving_ID,
                            Description = "Đồng ý",
                            Type = "MờiPH",
                            Notification_Date = DateTime.Now,
                            Username_Sender = username // Người thực hiện hành động
                        };
                        dbContext.Group_Notifications.Add(reverseNotification);
                        dbContext.SaveChanges(); // Lưu để lấy Group_Notification_ID

                        var reverseNotificationDetail = new Group_Notifications_Details
                        {
                            Group_Notification_ID = reverseNotification.Group_Notification_ID,
                            Username = notification.Username_Sender, // Người gửi ban đầu
                            Status = "Chưa đọc",
                            Is_Deleted = false
                        };
                        dbContext.Group_Notifications_Details.Add(reverseNotificationDetail);


                        var viewModel = (this.DataContext as GiaoDienChinhViewModel);

                        if (viewModel != null)
                        {
                            var element = dbContext.Group_Notifications_Details
     .Include(details => details.Group_Notifications) // Tải Group_Notifications
         .ThenInclude(notification => notification.User_Sender) // Tải User_Sender từ Group_Notifications
     .Include(details => details.Group_Notifications) // Lặp lại để tải Group_Savings_Accounts
         .ThenInclude(notification => notification.Group_Savings_Accounts) // Tải Group_Savings_Accounts qua Group_Notifications
             .ThenInclude(saving => saving.Group_Transactions_Information) // Tải Group_Transaction_Information
     .FirstOrDefault(details_ =>
      
         details_.Username == username &&
         details_.Is_Deleted == false &&
         details_.Group_Notification_ID == notificationDetailDb.Group_Notification_ID &&
         !(details_.Group_Notifications.Type == "MờiYC" &&
                              details_.Group_Notifications.Group_Savings_Accounts.Status == "Không hoạt động"));

                            if (element != null && element.Group_Notifications.Group_Savings_Accounts != null)
                            {
                                // Lấy danh sách giao dịch đã sắp xếp
                                var sortedTransactions = element.Group_Notifications.Group_Savings_Accounts
                                    .Group_Transactions_Information
                                    .OrderByDescending(transaction => transaction.Transaction_Date) // Sắp xếp giảm dần theo ngày
                                    .ToList();

                                // Cập nhật lại danh sách giao dịch trong Group_Savings_Accounts
                                element.Group_Notifications.Group_Savings_Accounts.Group_Transactions_Information =
                                    sortedTransactions;
                            }

                            var index = viewModel.Group_Notifications_Details
                         .ToList()
                       .FindIndex(p => p.Group_Notification_ID == notificationDetailDb.Group_Notification_ID);

                            if (index != -1)
                            {
                                viewModel.Group_Notifications_Details[index] = element;
                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Notifications_Details));

                                viewModel.Group_Savings_Accounts = new ObservableCollection<Group_Savings_Accounts>(
                                dbContext.Group_Savings_Accounts
                                    .Include(p => p.Group_Details)
                                        .ThenInclude(d => d.User)
                                    .Include(t => t.Group_Transactions_Information)
                                        .ThenInclude(f => f.User)
                                    .Include(e => e.Interest_Rates)
                                    .Where(p => p.Group_Details.Any(d => d.Username 
                                    == username) && p.Status == "Đang hoạt động")
                                    .OrderByDescending(p => p.Creating_Date)
                                    .ToList()
                                 );

                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Savings_Accounts));
                            }
                        }
                    }

                    // Lưu các thay đổi cuối cùng
                    dbContext.SaveChanges();
                    var viewmodel = this.DataContext as GiaoDienChinhViewModel;
                    viewmodel.IsEmptyAccountNhom = viewmodel.Group_Savings_Accounts.Count == 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            // Cập nhật giao diện
            var border = FindAncestor<Border>(sender as Button);
            border.Background = (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
        }

        private void bd_thongbao_Nhom_MouseLeave(object sender, MouseEventArgs e)
        {
            var notification = (sender as Border)?.DataContext as Group_Notifications_Details;

            var border = sender as Border;
            border.BorderThickness = new Thickness(.5);

            if (notification != null)
            {
                if (notification.Is_Selected == true)
                {
                    border.BorderThickness = new Thickness(1.5);
                    border.BorderBrush = Brushes.Black;
                    border.Background = (Brush)new BrushConverter().ConvertFromString("#F0FFF0");
                }
                else
                {
                    border.BorderBrush = Brushes.Gray;
                    if (notification.Status == "Đã đọc" || notification.Status == "Đồng ý" ||
                        notification.Status == "Không đồng ý")
                    {
                        border.Background = (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
                    }
                    else
                    {
                        border.Background = Brushes.White;
                    }
                }
            }

            var xemchitiet = FindChild<TextBlock>(sender as Border, "Xemchitiet");

            if (xemchitiet != null)
            {
                if (notification.Is_Selected == true)
                {
                    xemchitiet.Margin = new Thickness(0, -10, 5, 2);
                    xemchitiet.Visibility = Visibility.Hidden;
                }
                else xemchitiet.Visibility = Visibility.Collapsed;
            }
        }

        private void bd_thongbao_Nhom_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;

            var xemchitiet = FindChild<TextBlock>(border, "Xemchitiet");
            if (xemchitiet != null)
            {
                xemchitiet.Margin = new Thickness(0, -3, 5, 2);
                xemchitiet.Visibility = Visibility.Visible;
            }
        }

        private void bd_thongbao_Nhom_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var notification = (sender as Border)?.DataContext as Group_Notifications_Details;

            var border = sender as Border;
            border.BorderBrush = Brushes.DarkGreen;

            if (notification.Is_Selected == false && (notification.Status == "Đã đọc" ||
                notification.Status == "Đồng ý" || notification.Status == "Không đồng ý"))
            {

                border.Background = (Brush)new BrushConverter().ConvertFromString("#F0FFF0");
                notification.Is_Selected = true;
                SelectedItemNhom.Add(border);
            }
            else
            {
                if (notification.Status == "Đã đọc" || notification.Status == "Đồng ý"
                    || notification.Status == "Không đồng ý")
                {
                    border.Background = (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
                }
                notification.Is_Selected = false;
                SelectedItemNhom.Remove(border);
            }

            btn_Xoa_Nhom.Visibility = (SelectedItemNhom.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btn_Xoa_Nhom_Click(object sender, RoutedEventArgs e)
        {
            ThongBaoXoa thongBaoXoa = new ThongBaoXoa("Bạn có muốn xoá các thông báo được chọn không?");
            thongBaoXoa.ShowDialog();

            var giaoDienChinhViewModel = this.DataContext as GiaoDienChinhViewModel;

            if (giaoDienChinhViewModel != null)
            {
                if (thongBaoXoa.DialogResult == true)
                {
                    var selectedItemsCopy = SelectedItemNhom.ToList();

                    foreach (var border in selectedItemsCopy)
                    {
                        var notification = border.DataContext as Group_Notifications_Details;
                        SelectedItemNhom.Remove(border);
                        giaoDienChinhViewModel.Group_Notifications_Details.Remove(notification);
                       giaoDienChinhViewModel.DeleteGroupNotificationsDetails(notification);

                        if (notification.Is_Deleted == false)
                        {
                            giaoDienChinhViewModel.CNT_ThongBao_ChuaDoc_Nhom--;
                        }

                        giaoDienChinhViewModel.OnPropertyChanged(nameof(giaoDienChinhViewModel.Group_Notifications_Details));

                        btn_Xoa_Nhom.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Xemchitiet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var notification = (sender as TextBlock)?.DataContext as Group_Notifications_Details;

            notification.Is_Open = !notification.Is_Open;

            var row = FindAncestor<DataGridRow>(sender as TextBlock);
            var stackPanel = FindChild<StackPanel>(row, "stp_noidung");

            if (stackPanel != null)
            {
                stackPanel.Visibility = notification.Is_Open ? Visibility.Visible : Visibility.Collapsed;
            }
            if (notification != null && (notification.Status == "Chưa đọc" || notification.Status == "Chưa trả lời"))
            {
                var username = (this.DataContext as GiaoDienChinhViewModel).Username;
                var viewModel = this.DataContext as GiaoDienChinhViewModel; 

                using (var context = new AppDbContext())
                {
                    var findNotification = context.Group_Notifications_Details.FirstOrDefault(p => p.Group_Notification_ID 
                    == notification.Group_Notification_ID && p.Username == username);

                    if (findNotification != null)
                    {

                        if (findNotification.Status == "Chưa đọc")
                        {
                            findNotification.Status = "Đã đọc";

                            var element = context.Group_Notifications_Details
    .Include(details => details.Group_Notifications) // Tải Group_Notifications
        .ThenInclude(notification => notification.User_Sender) // Tải User_Sender từ Group_Notifications
    .Include(details => details.Group_Notifications) // Lặp lại để tải Group_Savings_Accounts
        .ThenInclude(notification => notification.Group_Savings_Accounts) // Tải Group_Savings_Accounts qua Group_Notifications
            .ThenInclude(saving => saving.Interest_Rates) // Tải Interest_Rates qua Group_Savings_Accounts
    .FirstOrDefault(details_ =>
        //details_.Group_Notifications.Group_Savings_Accounts.Status == "Đang hoạt động" &&
        details_.Username == username &&
        details_.Is_Deleted == false &&
        details_.Group_Notification_ID == findNotification.Group_Notification_ID &&
        !(details_.Group_Notifications.Type == "MờiYC" &&
                              details_.Group_Notifications.Group_Savings_Accounts.Status == "Không hoạt động"));

                            var index = viewModel.Group_Notifications_Details
                         .ToList()
                       .FindIndex(p => p.Group_Notification_ID == findNotification.Group_Notification_ID);

                            if (index != -1)
                            {
                                viewModel.Group_Notifications_Details[index] = element;
                                viewModel.OnPropertyChanged(nameof(viewModel.Group_Notifications_Details));
                            }
                        }

                        context.SaveChanges();

                        viewModel.CNT_ThongBao_ChuaDoc_Nhom--;

                        viewModel.OnPropertyChanged(nameof(viewModel.CNT_ThongBao_ChuaDoc_Nhom));
                    }
                }

                var border = FindChild<Border>(row, "bd_thongbao_Nhom");

                border.Background = (notification.Is_Selected) ? 
                    (Brush)new BrushConverter().ConvertFromString("#F0FFF0") : 
                    (Brush)new BrushConverter().ConvertFromString("#F3F3F3");
            }

            e.Handled = true;
        }

     


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btn_Lammoi_MouseLeave(object sender, MouseEventArgs e)
        {
            lammoi.TextDecorations = null;
            lammoi.Foreground = Brushes.Black;
            icon.Foreground = Brushes.Black;
        }
    }
}