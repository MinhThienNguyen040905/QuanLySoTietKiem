using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhanMemQuanLySTK.ViewModels
{
    public class DangKyViewModel : BaseViewModel
    {
        private string _username;
        private string _password;
        private string _confirm_password;
        private string _fullname;
        private string _email;
        private string _gender;
        private DateTime? _dob;
        private string _address;
        private string _avatar;
        private string _identity_card;
        private string _phone;
        private int demNguoc;
        private bool _is_agree;
        private string _otp;

        public int DemNguoc
        {
            get => demNguoc;
            set
            {
                demNguoc = value;
                OnPropertyChanged(nameof(DemNguoc));
            }
        }

        public string OTP
        {
            get => _otp;
            set
            {
                _otp = value;
                OnPropertyChanged(nameof(OTP));
            }
        }

        public bool IsArgee
        {
            get => _is_agree;
            set
            {
                _is_agree = value;
                OnPropertyChanged(nameof(IsArgee));
            }
        }

        public string Username
        {
            get { return _username; }
            set {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public string ConfirmPassword
        {
            get { return _confirm_password; }
            set
            {
                if (_confirm_password != value)
                {
                    _confirm_password = value;
                    OnPropertyChanged(nameof(ConfirmPassword));
                }
            }
        }


        public string Fullname
        {
            get { return _fullname; }
            set
            {
                if (_fullname != value)
                {
                    _fullname = value;
                    OnPropertyChanged(nameof(Fullname));
                }
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Gender
        {
            get { return _gender; }
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(nameof(Gender));
                }
            }
        }

        public DateTime? Dob
        {
            get { return _dob; }
            set
            {
                if (_dob != value)
                {
                    _dob = value;
                    OnPropertyChanged(nameof(Dob));
                }
            }

        }

        public string Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(nameof(Address));
                }
            }
        }

        public string Avatar
        {
            get { return _avatar; }
            set
            {
                if (_avatar != value)
                {
                    _avatar = value;
                    OnPropertyChanged(nameof(Avatar));
                }
            }
        }

        public string Identity_Card
        {
            get { return _identity_card; }
            set
            {
                if (_identity_card != value)
                {
                    _identity_card = value;
                    OnPropertyChanged(nameof(Identity_Card));
                }
            }
        }

        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }
    }
}
