using System.Windows.Controls;
using baitaplon.ViewModels.Admin; // Khai báo đường dẫn tới ViewModel

namespace baitaplon.Views.Admin.UserControls
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();

            // Liên kết (Bind) Giao diện này với DashboardViewModel
            this.DataContext = new DashboardViewModel();
        }
    }
}