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

    enum PlayersInPot { None, Hu, MultiPot}
    enum PotState { None, LimpPot, _2BetPot, _3BetPot, _4BetPot }
    enum PreflopState { None, Open, FacingLimp, FacingOpen, Vs3Bet, Vs4Bet, VsSqueeze, VsPush}

    enum FlopState { None, Cbet, Donk, VsBet, VsCbet, VsDonk, VsRaise }

    enum TurnState { None, _2Donk, _2Barrel, Vs2Barrel, Vs2Donk, VsCbetR, VsRaise }

    enum RiverState { None, _3Donk, _3Barrel, Vs3Barrel, Vs3Donk, Vs2BarrelR, VsRaise}


    public partial class MainWindow : Window {

        private HeroPosition _heroPosition = HeroPosition.None;

        private string[] _linesMass;
        public MainWindow()
        {
            InitializeComponent();
            BtnRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.Button;
            SbRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.SmallBlind;
            BbRadioButton.Checked += (sender, args) => _heroPosition = HeroPosition.BigBlind;

        }

        private void CheckButton_Click(object sender, RoutedEventArgs e) {
            UpperHeroDecision();

            _linesMass = new []{BtnLineTb.Text, SbLineTb.Text, BbLineTb.Text};
            

            var max = _linesMass.Select(mass => mass.Length).Max();
            StringBuilder st = new StringBuilder();
            //preflop
            for (int i = 0; i < max; i++)
            {

                if (BtnLineTb.Text.Length > i) {
                    if (BtnLineTb.Text[i] == ',') break;
                    st.Append(BtnLineTb.Text[i]);
                    st.Append('-');
                    
                }

                if (SbLineTb.Text.Length > i) {
                    if (SbLineTb.Text[i] == ',') break;
                    st.Append(SbLineTb.Text[i]);
                    st.Append('-');
                }

                if (BbLineTb.Text.Length > i) {
                    if (BbLineTb.Text[i] == ',') break;
                    st.Append(BbLineTb.Text[i]);
                    st.Append('-');
                }
                
            }
            //postflop

            st.Replace(st[st.Length - 1], '|', st.Length - 1,1);

            

            var postBtnLine = BtnLineTb.Text.Any(c => c == ',') ? BtnLineTb.Text.Substring(BtnLineTb.Text.IndexOf(',')+1) : "";
            var postSbLine = SbLineTb.Text.Any(c => c== ',') ? SbLineTb.Text.Substring(SbLineTb.Text.IndexOf(',')+1) : "";
            var postBbLine = BbLineTb.Text.Any(c => c== ',') ? BbLineTb.Text.Substring(BbLineTb.Text.IndexOf(',')+1) : "";
            
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

            ResultLabel.Content = String.Format("{0}{1}{2}", BtnLineTb.Text, SbLineTb.Text, BbLineTb.Text);
        }

        private void UpperHeroDecision() {
            switch (_heroPosition) {
                case HeroPosition.Button:
                    BtnLineTb.Text = BtnLineTb.Text.ToUpper();
                    break;
                    case HeroPosition.SmallBlind:
                    SbLineTb.Text = SbLineTb.Text.ToUpper();
                    break;
                    case HeroPosition.BigBlind:
                    BbLineTb.Text = BbLineTb.Text.ToUpper();
                    break;
            }
        }
      
    }
}
