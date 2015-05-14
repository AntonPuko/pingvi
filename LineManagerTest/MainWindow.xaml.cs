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

namespace LineManagerTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    enum HeroPosition {  None, Button, SmallBlind, BigBlind}
    public partial class MainWindow : Window {

        private HeroPosition _heroPosition = HeroPosition.None;
        public MainWindow()
        {
            InitializeComponent();
            BtnRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.Button;
            SBRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.SmallBlind;
            BBRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.BigBlind;

        }

        private void CheckButton_Click(object sender, RoutedEventArgs e) {
            
            DefinePreflopLine();

        }

        private void DefinePreflopLine() {
            UpperHeroDecision();

            ResultLabel.Content = String.Format("{0}{1}{2}", BtnLineTB.Text, SbLineTB.Text, BbLineTB.Text);
        }

        private void UpperHeroDecision() {
            switch (_heroPosition) {
                case HeroPosition.Button:
                    BtnLineTB.Text = BtnLineTB.Text.ToUpper();
                    break;
                    case HeroPosition.SmallBlind:
                    SbLineTB.Text = SbLineTB.Text.ToUpper();
                    break;
                    case HeroPosition.BigBlind:
                    BbLineTB.Text = BbLineTB.Text.ToUpper();
                    break;
            }
        }
      
    }
}
