using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using static PhanMemQuanLySTK.GiaoDienChinh;

namespace PhanMemQuanLySTK.ViewModels
{
    public class GiaoDienChinhViewModel : BaseViewModel
    {
        // binding tên người dùng
        private string _username;

        // binding email để xác nhận otp
        private string _email;

        // binding thông tin người dùng
        private Users _user;

        // lưu thông tin tạm của người dùng
        private Users _tmp_user;

        // binding thời gian đếm ngược (bộ đếm)
        private int demNguoc;

        // binding mã otp để xác thực
        private string _otp;

        // binding email mới, trong trường hợp người dùng đổi email
        private string _new_email;

        // binding sổ cá nhân
        private ObservableCollection<Personal_Savings_Accounts> _personal_savings_accounts;

        // binding sổ nhóm
        private ObservableCollection<Group_Savings_Accounts> _group_saving_accounts;

        // binding thông báo đáo hạn sổ cá nhân
        private ObservableCollection<Personal_Notifications> _personal_notifications;

        // binding lịch sử giao dịch sổ cá nhân
        private ObservableCollection<Personal_Transactions_Information> _personal_transactions_information;

        // binding thông tin chi tiết của sổ nhóm
        private ObservableCollection<Group_Notifications_Details> _group_notifications_details;

        // binding danh sách users khi tạo sổ nhóm
        private ObservableCollection<Users> _itemsUsers;

        private long _cnt_Thongbao_chuadoc_canhan;
        private long _cnt_Thongbao_chuadoc_nhom;



        // binding text input để tìm kiếm sổ tiết kiệm
        private string _searchQuery_Nhom;
        private string _searchQuery;
        private bool _noSavingsFound;
        private bool _noSavingsFoundNhom;
        private bool _isEmptyAccounts;
        private bool _isEmptyAccountsNhom;
        private string _readonlyFullname;
        private DispatcherTimer _timer;
        private DateTime _dt;
        private DateTime _currentDate;
        public Func<ChartPoint, string> LabelFormatter { get; set; }
        private int _sotien_Money; // lay su User
        public ObservableCollection<string> GenderOptions { get; set; } = 
            new ObservableCollection<string> { "Nam", "Nữ", "Khác" };

        public bool IsEmptyAccountNhom
        {
            get => _isEmptyAccountsNhom;
            set
            {
                if (_isEmptyAccountsNhom != value)
                {
                    _isEmptyAccountsNhom = value;
                    OnPropertyChanged(nameof(IsEmptyAccountNhom));
                }
            }
        }


        public bool NoSavingsFoundNhom
        {
            get => _noSavingsFoundNhom;
            set
            {
                if (_noSavingsFoundNhom != value)
                {
                    _noSavingsFoundNhom = value;
                    OnPropertyChanged(nameof(NoSavingsFoundNhom));
                }
            }
        }

        public string SearchQuery_Nhom
        {
            get => _searchQuery_Nhom;
            set
            {
                if (_searchQuery_Nhom != value)
                {
                    _searchQuery_Nhom = value;
                    OnPropertyChanged(nameof(SearchQuery_Nhom));
                    SearchSavingsAccounts_Nhom(_searchQuery_Nhom);
                }
            }
        }

        public ObservableCollection<Users> ItemsUsers
        {
            get { return _itemsUsers; }
            set { _itemsUsers = value; OnPropertyChanged(nameof(ItemsUsers)); }
        }

        public ObservableCollection<Group_Notifications_Details> Group_Notifications_Details
        {
            get { return _group_notifications_details; }
            set
            {
                _group_notifications_details = value;
                OnPropertyChanged(nameof(Group_Notifications_Details));
            }
        }

        public int Sotien_Money
        {
            get => _sotien_Money;
            set
            {
                _sotien_Money = value;
                OnPropertyChanged(nameof(Sotien_Money));
            }
        }

        public DateTime currentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                OnPropertyChanged(nameof(currentDate));
            }
        }

        public DateTime dt
        {
            get => _dt;
            set
            {
                _dt = value;
                OnPropertyChanged(nameof(dt));
            }
        }

        public string NewEmail
        {
            get { return _new_email; }
            set { _new_email = value; OnPropertyChanged(nameof(NewEmail)); }
        }

        public string OTP
        {
            get { return _otp; }
            set { _otp = value; OnPropertyChanged(nameof(OTP)); }
        }

        public int DemNguoc
        {
            get => demNguoc;
            set
            {
                demNguoc = value;
                OnPropertyChanged(nameof(DemNguoc));
            }
        }

        public string ReadOnlyFullname
        {
            get => _readonlyFullname;
            set
            {
                if (_readonlyFullname != value)
                {
                    _readonlyFullname = value;
                    OnPropertyChanged(nameof(ReadOnlyFullname));
                }
            }
        }

        public ChartValues<long> PersonalSavings { get; set; }

        public ChartValues<long> GroupSavings { get; set; }

        public ChartValues<int> PersonalAccountCount { get; set; }

        public ChartValues<int> GroupAccountCount { get; set; }

        public string[] Labels { get; set; }


        public bool NoSavingsFound
        {
            get => _noSavingsFound;
            set
            {
                if (_noSavingsFound != value)
                {
                    _noSavingsFound = value;
                    OnPropertyChanged(nameof(NoSavingsFound));
                }
            }
        }

        public Users User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public Users TmpUser
        {
            get { return _tmp_user; }
            set
            {
                if (_tmp_user != value)
                {
                    _tmp_user = value;
                    OnPropertyChanged(nameof(TmpUser));
                }
            }
        }

        public bool IsEmptyAccount
        {
            get => _isEmptyAccounts;
            set
            {
                if (_isEmptyAccounts != value)
                {
                    _isEmptyAccounts = value;
                    OnPropertyChanged(nameof(IsEmptyAccount));
                }
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                    SearchSavingsAccounts(_searchQuery);
                }
            }
        }

        public long CNT_ThongBao_ChuaDoc_CaNhan
        {
            get => _cnt_Thongbao_chuadoc_canhan;
            set
            {
                _cnt_Thongbao_chuadoc_canhan = value;
                OnPropertyChanged(nameof(CNT_ThongBao_ChuaDoc_CaNhan));
            }
        }

        public long CNT_ThongBao_ChuaDoc_Nhom
        {
            get => _cnt_Thongbao_chuadoc_nhom;
            set
            {
                _cnt_Thongbao_chuadoc_nhom = value;
                OnPropertyChanged(nameof(CNT_ThongBao_ChuaDoc_Nhom));
            }
        }

        public string Email
        {
            get { return _email; }
            set { _email = value; OnPropertyChanged(nameof(Email)); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        public ObservableCollection<Personal_Notifications> Personal_Notifications
        {
            get { return _personal_notifications; }
            set { _personal_notifications = value; OnPropertyChanged(nameof(Personal_Notifications)); }
        }

        public ObservableCollection<Personal_Savings_Accounts> Personal_Savings_Accounts
        {
            get { return _personal_savings_accounts; }
            set { _personal_savings_accounts = value; 
                OnPropertyChanged(nameof(Personal_Savings_Accounts)); }
        }

        public ObservableCollection<Group_Savings_Accounts> Group_Savings_Accounts
        {
            get { return _group_saving_accounts; }
            set
            {
                _group_saving_accounts = value;
                OnPropertyChanged(nameof(Group_Savings_Accounts));
            }
        }

        public ObservableCollection<Personal_Transactions_Information> Personal_Transactions_Informations
        {
            get { return _personal_transactions_information; }
            set { _personal_transactions_information = value; 
            OnPropertyChanged(nameof(Personal_Transactions_Informations)); }
        }

        // functions

        private void SearchSavingsAccounts_Nhom(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                using (var context = new AppDbContext())
                {
                    var accounts = context.Group_Savings_Accounts
    .Include(p => p.Group_Details)
        .ThenInclude(d => d.User)
    .Include(t => t.Group_Transactions_Information)
        .ThenInclude(f => f.User)
    .Include(e => e.Interest_Rates)
    .Where(p => p.Group_Details.Any(d => d.Username == Username) && p.Status == "Đang hoạt động" &&
    p.Name.Contains(searchText))
    .Select(account => new Group_Savings_Accounts
    {
        // Giữ nguyên tài khoản và các thuộc tính
        Saving_ID = account.Saving_ID,
        Name = account.Name,
        Creating_Date = account.Creating_Date,
        Status = account.Status,
        Group_Details = account.Group_Details,
        Interest_Rates = account.Interest_Rates,
        // Sắp xếp lịch sử giao dịch giảm dần
        Group_Transactions_Information = account.Group_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList(),
        Money = account.Money,
        Interest_Rate_ID = account.Interest_Rate_ID,
        Description = account.Description,
        Target = account.Target,
    })
    .OrderByDescending(account => account.Creating_Date) // Sắp xếp sổ theo ngày tạo
    .ToList();


                    Group_Savings_Accounts.Clear();

                    foreach (var account in accounts)
                    {
                        Group_Savings_Accounts.Add(account);
                    }

                    NoSavingsFoundNhom = !accounts.Any() && searchText != "";
                    IsEmptyAccountNhom = Group_Savings_Accounts.Count == 0 && searchText == "";
                }
            }
            else
            {
                LoadDefaultSavingsAccounts_Nhom();
            }
        }

        private void LoadDefaultSavingsAccounts_Nhom()
        {
            using (var context = new AppDbContext())
            {
                var accounts = context.Group_Savings_Accounts
    .Include(p => p.Group_Details)
        .ThenInclude(d => d.User)
    .Include(t => t.Group_Transactions_Information)
        .ThenInclude(f => f.User)
    .Include(e => e.Interest_Rates)
    .Where(p => p.Group_Details.Any(d => d.Username == Username) && p.Status == "Đang hoạt động")
    .Select(account => new Group_Savings_Accounts
    {
        // Giữ nguyên tài khoản và các thuộc tính
        Saving_ID = account.Saving_ID,
        Name = account.Name,
        Creating_Date = account.Creating_Date,
        Status = account.Status,
        Group_Details = account.Group_Details,
        Interest_Rates = account.Interest_Rates,
        // Sắp xếp lịch sử giao dịch giảm dần
        Group_Transactions_Information = account.Group_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList(),
        Money = account.Money,
        Interest_Rate_ID = account.Interest_Rate_ID,
        Description = account.Description,
        Target = account.Target,
    })
    .OrderByDescending(account => account.Creating_Date) // Sắp xếp sổ theo ngày tạo
    .ToList();


                Group_Savings_Accounts.Clear();
                foreach (var account in accounts)
                {
                    Group_Savings_Accounts.Add(account);
                }

                NoSavingsFoundNhom = false;
                IsEmptyAccountNhom = Group_Savings_Accounts.Count == 0;
            }
        }

        public void LoadPersonalNotifications(string username)
        {
            using (var context = new AppDbContext())
            {
                var personalNotifications = context.Personal_Notifications
                    .Include(pn => pn.Personal_Savings_Account)
    .Where(n => n.Personal_Savings_Account.Username == username &&
                !n.Is_Deleted &&
                n.Personal_Savings_Account.Interest_Rate_ID != -1 &&
                n.Personal_Savings_Account.Interest_Rate.Term > 0)
    .ToList();

                Personal_Notifications = new ObservableCollection<Personal_Notifications>(
                    personalNotifications);

                CNT_ThongBao_ChuaDoc_CaNhan = Personal_Notifications.Count(p => p.Is_Read == false);
            }
        }

        private void SearchSavingsAccounts(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                using (var context = new AppDbContext())
                {
                    var searchResults = context.Personal_Savings_Accounts
                                        .Include(p => p.Personal_Transactions_Information)
                                        .Include(p => p.Interest_Rate)
                                        .Where(p => p.Username == Username && p.Status == "Đang hoạt động" 
                                        && p.Name.Contains(searchText))
                                        .OrderByDescending(p => p.Creating_Date)
                                        .Select(p => new
                                        {
                                            Account = p,
                                            Transactions = p.Personal_Transactions_Information
                                                            .OrderByDescending(t => t.Transaction_Date)
                                                            .ToList()
                                        })
                                        .AsEnumerable()
                                        .Select(p =>
                                        {
                                            p.Account.Personal_Transactions_Information = p.Transactions;
                                            return p.Account;
                                        })
                                        .ToList();

                    Personal_Savings_Accounts.Clear();
                    foreach (var account in searchResults)
                    {
                        Personal_Savings_Accounts.Add(account);
                    }

                    NoSavingsFound = !searchResults.Any() && searchText != "";
                    IsEmptyAccount = Personal_Savings_Accounts.Count == 0 && searchText == "";
                }
            }
            else
            {
                LoadDefaultSavingsAccounts();
            }
        }

        private void LoadDefaultSavingsAccounts()
        {
            using (var context = new AppDbContext())
            {
                var defaultAccounts = context.Personal_Savings_Accounts
                                        .Include(p => p.Personal_Transactions_Information)
                                        .Include(p => p.Interest_Rate)
                                        .Where(p => p.Username == Username && p.Status == "Đang hoạt động")
                                        .OrderByDescending(p => p.Creating_Date)
                                        .Select(p => new
                                        {
                                            Account = p,
                                            Transactions = p.Personal_Transactions_Information
                                                            .OrderByDescending(t => t.Transaction_Date)
                                                            .ToList()
                                        })
                                        .AsEnumerable()
                                        .Select(p =>
                                        {
                                            p.Account.Personal_Transactions_Information = p.Transactions;
                                            return p.Account;
                                        })
                                        .ToList();

                Personal_Savings_Accounts.Clear();

                 foreach (var account in defaultAccounts)
                {
                    Personal_Savings_Accounts.Add(account);
                }

                NoSavingsFound = false;
                IsEmptyAccount = Personal_Savings_Accounts.Count == 0;
            }
        }

        public void LoadPersonalSavingsAccounts(string username)
        {
            using (var context = new AppDbContext())
            {
                var savingsAccounts = context.Personal_Savings_Accounts
                                        .Include(p => p.Personal_Transactions_Information)
                                        .Include(p => p.Interest_Rate)
                                        .Where(p => p.Username == username && p.Status == "Đang hoạt động")
                                        .OrderByDescending(p => p.Creating_Date)
                                        .Select(p => new
                                        {
                                            Account = p,
                                            Transactions = p.Personal_Transactions_Information
                                                            .OrderByDescending(t => t.Transaction_Date)
                                                            .ToList()
                                        })
                                        .AsEnumerable()
                                        .Select(p =>
                                        {
                                            p.Account.Personal_Transactions_Information = p.Transactions;
                                            return p.Account;
                                        })
                                        .ToList();

                Personal_Savings_Accounts =
                                      new ObservableCollection<Personal_Savings_Accounts>(savingsAccounts);

                if (savingsAccounts.Count == 0)
                {
                    IsEmptyAccount = true;
                }
            }
        }

        public void LoadGroupSavingsAccounts(string username)
        { 
            using (var context = new AppDbContext())
            {
                var accounts = context.Group_Savings_Accounts
    .Include(p => p.Group_Details)
        .ThenInclude(d => d.User)
    .Include(t => t.Group_Transactions_Information)
        .ThenInclude(f => f.User)
    .Include(e => e.Interest_Rates)
    .Where(p => p.Group_Details.Any(d => d.Username == username) && p.Status == "Đang hoạt động")
    .Select(account => new Group_Savings_Accounts
    {
        // Giữ nguyên tài khoản và các thuộc tính
        Saving_ID = account.Saving_ID,
        Name = account.Name,
        Creating_Date = account.Creating_Date,
        Status = account.Status,
        Group_Details = account.Group_Details,
        Interest_Rates = account.Interest_Rates,
        // Sắp xếp lịch sử giao dịch giảm dần
        Group_Transactions_Information = account.Group_Transactions_Information
            .OrderByDescending(t => t.Transaction_Date)
            .ToList(),
        Money = account.Money,
        Interest_Rate_ID = account.Interest_Rate_ID,
        Description = account.Description,
        Target = account.Target
    })
    .OrderByDescending(account => account.Creating_Date) // Sắp xếp sổ theo ngày tạo
    .ToList();

                // Chuyển đổi danh sách thành ObservableCollection
                Group_Savings_Accounts = new ObservableCollection<Group_Savings_Accounts>(accounts);

                Group_Notifications_Details = new ObservableCollection<Group_Notifications_Details>(
                context.Group_Notifications_Details
                    .Include(details => details.Group_Notifications) // Tải Group_Notifications
                        .ThenInclude(notification => notification.User_Sender) // Tải User_Sender từ Group_Notifications
                    .Include(details => details.Group_Notifications) // Tải thêm Group_Savings_Accounts qua Group_Notifications
                        .ThenInclude(notification => notification.Group_Savings_Accounts) // Tải Group_Savings_Accounts qua Group_Notifications
                            .ThenInclude(saving => saving.Interest_Rates) // Tải Interest_Rates qua Group_Savings_Accounts
                    .Where(details_ =>
                           details_.Is_Deleted == false && details_.Username == username &&
                            !(details_.Group_Notifications.Type == "MờiYC" &&
                              details_.Group_Notifications.Group_Savings_Accounts.Status == "Không hoạt động"))

                    .OrderByDescending(p => p.Group_Notifications.Notification_Date) // Sắp xếp giảm dần theo Notification_Date
                    .ToList() // Chuyển đổi sang danh sách
                 );

        CNT_ThongBao_ChuaDoc_Nhom = Group_Notifications_Details.Count(p => p.Status == "Chưa đọc" || p.Status == "Chưa trả lời"
                   );

                IsEmptyAccountNhom = Group_Savings_Accounts.Count == 0;
            }
        }

        public async Task<ObservableCollection<Personal_Savings_Accounts>> 
            GetOtherSavingsAccountsAsync(string username, int excludedId)
        {
            using (var context = new AppDbContext())
            {
                var accounts = await context.Personal_Savings_Accounts
                    .Where(account => account.Username == username && account.Saving_ID != excludedId)
                    .ToListAsync();

                return new ObservableCollection<Personal_Savings_Accounts>(accounts);
            }
        }

        public void LoadUserInfoAsync(string username)
        {
            using (var context = new AppDbContext())
            {
                var user = context.Users.FirstOrDefault(p => p.Username == username);
                if (user != null)
                {
                    User = user;
                    ReadOnlyFullname = user.Fullname;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateClock();
        }

        private void UpdateClock()
        {
            dt = DateTime.Now;
        }

        public void DeletePersonalNotifications(Personal_Notifications personal_Notification)
        {
            using (var context = new AppDbContext())
            {
                var persoanlNotification = context.Personal_Notifications
           .FirstOrDefault(g => g.Personal_Notification_ID == personal_Notification.Personal_Notification_ID);

                if (persoanlNotification != null)
                {
                    persoanlNotification.Is_Deleted = true;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Personal notification not found.");
                }
            }
        }

        public void DeleteGroupNotificationsDetails(Group_Notifications_Details group_Notification)
        {
            using (var context = new AppDbContext())
            {
                var groupNotification = context.Group_Notifications_Details
            .FirstOrDefault(g => g.Group_Notification_ID == group_Notification.Group_Notification_ID);

                if (groupNotification != null)
                {
                    groupNotification.Is_Deleted = true;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("Group notification not found.");
                }
            }
        }

        public GiaoDienChinhViewModel()
        {
            currentDate = DateTime.Now;
            DemNguoc = 120;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start(); 

            UpdateClock();

            Personal_Savings_Accounts = new ObservableCollection<Personal_Savings_Accounts>();
            Group_Savings_Accounts = new ObservableCollection<Group_Savings_Accounts>();
            User = new Users() { 
                Email = "",
                Fullname = "",
                Dob = DateTime.Now,
                Identity_Card = "",
                Phone = "",
                Gender = "Nam",
                Address = "",
                Username = "",
                Password = "",
                Money = 0,
                Avatar = ""
            };
             TmpUser = new Users()
            {
                Email = "",
                Fullname = "",
                Dob = DateTime.Now,
                Identity_Card = "",
                Phone = "",
                Gender = "Nam",
                Address = "",
                Username = "",
                Password = "",
                Money = 0,
                Avatar = ""
            };
            Username = "";
            Email = "";
            OTP = "";
            NewEmail = "";
            GroupSavings = new ChartValues<long> { 0 };
            SearchQuery = "";
            SearchQuery_Nhom = "";
            NoSavingsFoundNhom = false;
            NoSavingsFound = false;
            IsEmptyAccount = false;
            IsEmptyAccountNhom = false;
            ReadOnlyFullname = "";
            PersonalSavings = new ChartValues<long> { 0 };
            PersonalAccountCount = new ChartValues<int> { 0 };
            ItemsUsers = new ObservableCollection<Users>();
            GroupAccountCount = new ChartValues<int> { 0 };  
            Labels = new[] { "Chi tiết" };
            LabelFormatter = point => $"{point.Y.ToString("N0", CultureInfo.InvariantCulture)} VNĐ";
            Personal_Notifications = new ObservableCollection<Personal_Notifications>();
            Personal_Transactions_Informations = new ObservableCollection<Personal_Transactions_Information>();
            Group_Notifications_Details = new ObservableCollection<Group_Notifications_Details>();
            CNT_ThongBao_ChuaDoc_CaNhan = Personal_Notifications.Count(p => p.Is_Read == false);
            CNT_ThongBao_ChuaDoc_Nhom = Group_Notifications_Details.Count(p => p.Status == "Chưa đọc" || p.Status == "Chưa trả lời");
        }
    }
}
