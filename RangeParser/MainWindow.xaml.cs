using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Pingvi;
using PokerModel;

namespace RangeParser {

    public partial class MainWindow : Window {

       
        private string path = @"P:\Dropbox\pAd\pAd\bin\Debug\Data\Ranges";
        public MainWindow() {
            InitializeComponent();
            
        }

        
       
        private void LoadRangeButton_Click(object sender, RoutedEventArgs e) {
            var handRange = XmlRangeHelper.Load(path);
         
            StringBuilder sb = new StringBuilder();
            foreach (var h in handRange.Hands) {
                sb.Append(h.Name + " ");
            }
            RangeBox.Text = sb.ToString();

        }

        private void SaveRangeButton_Click(object sender, RoutedEventArgs e) {
            var  handRange = new Range(NameTextBox.Text, RangeBox.Text);
            path = String.Format("{0}.xml", NameTextBox.Text);
            XmlRangeHelper.Save(handRange, path);
        }


 


      

        
    }
}
