using System.Windows;
using System.Windows.Controls;
using baitaplon.ViewModels.Admin;

namespace baitaplon.Views.Admin.UserControls
{
    public partial class QuanLyHopDongView : UserControl
    {
        public QuanLyHopDongView()
        {
            InitializeComponent();
            this.DataContext = new QuanLyHopDongViewModel();
        }

        private void BtnMoFormTao_Click(object sender, RoutedEventArgs e)
        {
            // 1. Mở cửa sổ tạo hợp đồng
            var window = new TaoHopDongWindow();
            window.ShowDialog();

            // 2. Sau khi cửa sổ đóng, gọi ViewModel tải lại dữ liệu để lưới tự cập nhật
            if (this.DataContext is QuanLyHopDongViewModel vm)
            {
                vm.TaiDuLieu();
            }
        }
    }
}