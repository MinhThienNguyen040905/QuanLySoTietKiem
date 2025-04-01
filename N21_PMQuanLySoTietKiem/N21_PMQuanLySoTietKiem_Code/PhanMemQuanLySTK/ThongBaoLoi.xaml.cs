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
    /// Interaction logic for ThongBaoLoi.xaml
    /// </summary>
    public partial class ThongBaoLoi : Window, INotifyPropertyChanged
    {
        private string _loi;

        public string Loi
        {
            get => _loi;
            set
            {
                if (_loi != value)
                {
                    _loi = value;
                    OnPropertyChanged(nameof(Loi));
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            int margin = 30;
            var location = e.GetPosition(this);
            if (location.X >= margin && location.X <= ActualWidth - margin && location.Y >= margin && location.Y <= ActualHeight - margin)
                this.DragMove();
        }


        public ThongBaoLoi(string _Loi)
        {
            Loi = _Loi;
            InitializeComponent();
            this.DataContext = this;
        }


        private void Nhaplai_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
            }
        }
    }
}
