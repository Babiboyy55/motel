using System.Windows.Controls;
using baitaplon.ViewModels.Admin;

namespace baitaplon.Views.Admin.UserControls
{
    public partial class QuanLyNguoiDungView : UserControl
    {
        public QuanLyNguoiDungView()
        {
            InitializeComponent();
            this.DataContext = new QuanLyNguoiDungViewModel();
        }
    }
}