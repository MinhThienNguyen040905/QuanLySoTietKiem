using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ThongBaoThanhCong.xaml
    /// </summary>
    public partial class ThongBaoThanhCong : Window, INotifyPropertyChanged
    {
        private string _tieude;

        public string Tieude
        {
            get => _tieude;
            set
            {
                if (_tieude != value)
                {
                    _tieude = value;
                    OnPropertyChanged(nameof(Tieude));
                }
            }
        }

        private string _noidung;

        public string Noidung
        {
            get => _noidung;
            set
            {
                if (_noidung != value)
                {
                    _noidung = value;
                    OnPropertyChanged(nameof(Noidung));
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 30;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin 
                && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }


        public ThongBaoThanhCong(string _tieude, string _noidung)
        {
            Tieude = _tieude;
            Noidung = _noidung;
            InitializeComponent();
            this.DataContext = this;
        }

        private void Nhaplai_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
