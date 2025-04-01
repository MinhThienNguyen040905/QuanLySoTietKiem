using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.ViewModels
{
    public class QuenMatKhauViewModel : BaseViewModel
    {
        private string _username;
        private string _otp;
        private string _email;
        private int demNguoc;
        private string _new_password;

        public string NewPassword
        {
            get => _new_password;
            set
            {
                _new_password = value;
                OnPropertyChanged(nameof(NewPassword));
            }
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

        public string OTP
        {
            get { return _otp; }
            set { _otp = value; OnPropertyChanged(nameof(OTP)); }
        }
    }
}
