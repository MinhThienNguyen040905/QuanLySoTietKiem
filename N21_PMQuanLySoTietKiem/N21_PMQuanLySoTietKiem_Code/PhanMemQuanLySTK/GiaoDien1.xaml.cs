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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PhanMemQuanLySTK
{
    /// <summary>
    /// Interaction logic for GiaoDien1.xaml
    /// </summary>
    public partial class GiaoDien1 : UserControl
    {
        public GiaoDien1()
        {
            InitializeComponent();
            LoadDangNhapGiaoDien();
        }

        private void LoadDangNhapGiaoDien()
        {
            var GDDangNhap = new GiaoDienDangNhap(this);
            GDDangNhap.Margin = new Thickness(50, 280, 50, 270);

            GDDangNhap.OnButtonClickEvent -= GiaoDienDangNhap_OnButtonClickEvent;
            GDDangNhap.OnButtonClickEvent += GiaoDienDangNhap_OnButtonClickEvent;

            GiaoDien.Content = GDDangNhap;
        }

        private void GiaoDienDangNhap_OnButtonClickEvent(GiaoDienChinhViewModel giaoDienChinhViewModel)
        {
            var parent = this.Parent as ContentControl;
            if (parent != null)
            {
                var giaoDienChinh = new GiaoDienChinh(this);
                giaoDienChinh.DataContext = giaoDienChinhViewModel;
                parent.Margin = new Thickness(0, 3, 0, 0);
                parent.Content = giaoDienChinh;
            }
        }

        public void QuayLaiDangNhap()
        {
            LoadDangNhapGiaoDien();
        }
    }
}
