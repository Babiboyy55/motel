using System.Windows.Controls;
using Quanlynhatro.ViewModels.Admin;

namespace Quanlynhatro.Views.Admin.UserControls
{
    public partial class QuanLyKhachUC : UserControl
    {
        public QuanLyKhachUC()
        {
            InitializeComponent();
            this.DataContext = new QuanLyKhachViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
