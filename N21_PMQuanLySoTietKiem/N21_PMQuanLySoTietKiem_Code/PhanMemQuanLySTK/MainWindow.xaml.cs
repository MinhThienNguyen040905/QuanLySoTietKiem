using PhanMemQuanLySTK;
using PhanMemQuanLySTK.Data;
using PhanMemQuanLySTK.Models;
using PhanMemQuanLySTK.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //
    public partial class MainWindow : Window
    {
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var workArea = SystemParameters.WorkArea;
            this.Left = workArea.Left;
            this.Top = workArea.Top;
            this.Width = workArea.Width;
            this.Height = workArea.Height;
        }

        public MainWindow()
        {
            InitializeComponent();
            GiaoDien1 gdChinh = new GiaoDien1();
            DangNhap.Content = gdChinh;
        }
        private void btn_Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void btn_Minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void btn_Close_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = (Brush)new BrushConverter().ConvertFromString("#E54646");
            }
        }

        private void btn_MouseLeave(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = Brushes.White;
            }
        }

        private void btn_Minimize_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                border.Background = (Brush)new BrushConverter().ConvertFromString("#ECECEC");
            }
        }
    }

}