using Quanlynhatro.ViewModels.Admin;
using System.Windows;

namespace Quanlynhatro.Views.Admin
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            this.DataContext = new AdminMainViewModel();
        }
    }
}
