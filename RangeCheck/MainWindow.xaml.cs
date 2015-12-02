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
using PokerModel;

namespace RangeCheck
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Range _range;
        public MainWindow()
        {
            InitializeComponent();

            _range = XmlRangeHelper.Load(@"Data\NashPush.xml");
        }

        private void CheckButton_Click(object sender, RoutedEventArgs e) {
            var card1 = HandTextBox.Text.Substring(0, 2);
            var card2 = HandTextBox.Text.Substring(2, 2);
            var hand = new Hand(new Card(card1), new Card(card2));
            double stack = double.Parse(StackTextBox.Text);
            bool isPush;
            var hPlaybility = _range.Hands.First(h => h.Name == hand.Name).Playability;
            if (hPlaybility  >= stack)
            {
                isPush = true;
            }
            else {
                isPush = false;
            }


            ResultLabel.Content = String.Format("playbility: {0}, PUSH: {1}",
                _range.Hands.First(h => h.Name == hand.Name).Playability, isPush);
        }
    }
}
