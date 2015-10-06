using System;
using System.Collections.Generic;
using System.Drawing;
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
using Pingvi.TableCatchers;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for VmWindow.xaml
    /// </summary>
    public partial class VmWindow : Window {

        private VmTableCatcher vmTableCatcher;
        private ScreenTableManager _screenTableManager;
        public VmWindow() {
            InitializeComponent();
            _screenTableManager= new  ScreenTableManager();
            vmTableCatcher = new VmTableCatcher();
        }

        private void CatchButton_Click(object sender, RoutedEventArgs e)
        {
           // vmTableCatcher.CatchVmScreen();
         //   vmTableCatcher.NewTableBitmap += OnNewBitmap;
          //  vmTableCatcher.Start();
            _screenTableManager.NewTableBitmap += OnNewBitmap;
            _screenTableManager.Start();

        }

        private void OnNewBitmap(Bitmap bitmap) {
            TaleImage.Source = BitmapHelper.MakeImage(bitmap);
        }
    }
}
