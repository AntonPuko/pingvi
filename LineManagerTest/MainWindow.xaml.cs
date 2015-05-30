using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    enum PlayersInPot { None, HU, MultiPot}
    enum PotState { None, LimpPot, _2BetPot, _3BetPot, _4BetPot }
    enum PreflopState { None, Open, FacingLimp, FacingOpen, Vs3Bet, Vs4Bet, VsSqueeze, VsPush}

    enum FlopState { None, Cbet, Donk, VsBet, VsCbet, VsDonk, VsRaise }

    enum TurnState { None, _2Donk, _2barrel, Vs2Barrel, Vs2Donk, VsCbetR, VsRaise }

    enum RiverState { None, _3Donk, _3barrel, Vs3Barrel, Vs3Donk, Vs2BarrelR, VsRaise}


    public partial class MainWindow : Window {

        private HeroPosition _heroPosition = HeroPosition.None;

        private string[] linesMass;
        public MainWindow()
        {
            InitializeComponent();
            BtnRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.Button;
            SBRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.SmallBlind;
            BBRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.BigBlind;

        }

        private void CheckButton_Click(object sender, RoutedEventArgs e) {
            UpperHeroDecision();

            linesMass = new []{BtnLineTB.Text, SbLineTB.Text, BbLineTB.Text};
            

            var max = linesMass.Select(mass => mass.Length).Max();
            StringBuilder st = new StringBuilder();
            //preflop
            for (int i = 0; i < max; i++)
            {

                if (BtnLineTB.Text.Length > i) {
                    if (BtnLineTB.Text[i] == ',') break;
                    st.Append(BtnLineTB.Text[i]);
                    st.Append('-');
                    
                }

                if (SbLineTB.Text.Length > i) {
                    if (SbLineTB.Text[i] == ',') break;
                    st.Append(SbLineTB.Text[i]);
                    st.Append('-');
                }

                if (BbLineTB.Text.Length > i) {
                    if (BbLineTB.Text[i] == ',') break;
                    st.Append(BbLineTB.Text[i]);
                    st.Append('-');
                }
                
            }
            //postflop

            st.Replace(st[st.Length - 1], '|', st.Length - 1,1);

            

            var postBtnLine = BtnLineTB.Text.Any(c => c == ',') ? BtnLineTB.Text.Substring(BtnLineTB.Text.IndexOf(',')+1) : "";
            var postSbLine = SbLineTB.Text.Any(c => c== ',') ? SbLineTB.Text.Substring(SbLineTB.Text.IndexOf(',')+1) : "";
            var postBbLine = BbLineTB.Text.Any(c => c== ',') ? BbLineTB.Text.Substring(BbLineTB.Text.IndexOf(',')+1) : "";
            
            var postLineMass = new[] {postSbLine, postBbLine, postBtnLine};

            var maxPost = postLineMass.Select(mass => mass.Length).Max();

            for (int i = 0; i < maxPost; i++) {

                
                
                   

                if (postSbLine.Length > i) {
                    st.Append(postSbLine[i]);
                    st.Append('-');
                }
                if (postBbLine.Length > i) {
                    st.Append(postBbLine[i]);
                    st.Append('-');
                }
                if (postBtnLine.Length > i) {
                    st.Append(postBtnLine[i]);
                    st.Append('-');
                }

                if (st[st.Length - 2] == 'c' || st[st.Length - 2] == 'f' ||
                    st[st.Length - 2] == 'C' || st[st.Length - 2] == 'F') st.Replace(st[st.Length - 1], '|', st.Length - 1, 1);
            }


            ResultLabel.Content = st.ToString();
            

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
