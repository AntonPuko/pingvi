using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for RWindow.xaml
    /// </summary>
    public partial class RWindow : Window
    {
        public RWindow()
        {
            InitializeComponent();
        }

       // private Action _dbOpenAction;
        private DispatcherTimer _timer;
       // private NpgsqlConnection _npgSqlConnection;

        private bool _isResultRefreshing = false;

        private double _usdRubExRate = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e) {

            try {
                string exchangeRatesURL = "http://www.apilayer.net/api/live?access_key=b5a73c66b2b8f58fee2d61ac8e9695a2";

                string ratesJson;
                using (var webClient = new System.Net.WebClient()) {
                    ratesJson = webClient.DownloadString(exchangeRatesURL);
                }

                var ratesJObect = JObject.Parse(ratesJson);
                _usdRubExRate = (double)ratesJObect["quotes"]["USDRUB"];
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); }

            if (_usdRubExRate == 0) _usdRubExRate = 55;
         

                _timer = new DispatcherTimer() {
                    Interval = TimeSpan.FromSeconds(5),
                };

                _timer.Tick += OnTimerTick;
                _timer.IsEnabled = true;
            }


        private void OnTimerTick(object sender, EventArgs e) {
            int tagCount = 0;
            double evBB100 = 0;
            double result = 0;
            double rake = 0;
            double rakeback = 0;

            string dtNow = DateTime.Now.Hour < 5 ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");
            string statsURL = "http://localhost:8001/query?q=select StatTourneyCount, StatEVBigBlindsPer100 from stats where HandTimestamp > {d \"" + dtNow + " 05:00:00 AM\"}";
            string statsJson;
            string tourneysURL = "http://localhost:8001/query?q=select BuyInPlusRake, RakeInCents, WinningsInCents from TOURNAMENTS where FirstHandTimestamp > {d \"" + dtNow + " 05:00:00 AM\"} and FinishPosition > 0 ";
            string tourneysJson;

            
            using (var webClient = new System.Net.WebClient()) {
                try {
                    statsJson = webClient.DownloadString(statsURL);
                    tourneysJson = webClient.DownloadString(tourneysURL);
                }
                catch  {
                    CleartStats();
                    return;
                }
                
            }

            try {
                var statsJObect = JObject.Parse(statsJson);
                var statsResults = statsJObect["Results"];

                //tagCount = int.Parse(statsResults[0]["TagCount"].ToString().Replace("В", "").Trim());
                evBB100 = double.Parse(statsResults[0]["EVbb100"].ToString().Replace("В", "").Trim());
            }
            catch {
                return;
            }




            try {
                var tourneysJObject = JObject.Parse(tourneysJson);
                var tourneysResults = tourneysJObject["Results"];



                foreach (var tourney in tourneysResults) {
                    tagCount++;
                    var tResult = (double.Parse(tourney["WinningsInCents"].ToString().Replace("В", "").Trim())
                                   - double.Parse(tourney["BuyInPlusRake"].ToString().Replace("В", "").Trim()))/100;
                    result += tResult;

                    var tRake = double.Parse(tourney["RakeInCents"].ToString().Replace("В", "").Trim())/100;
                    rake += tRake;
                }
            }
            catch {
                return;
            }
          


            //ChipsEVTourney = tagCount == 0 ?  0 : chipsEV/tagCount;
             


            const double vppMultiplicator = 5.5;
            const double bonusFormula = 3.5/40000*600;
            rakeback = rake*vppMultiplicator * bonusFormula * _usdRubExRate;

            
            
            CountRun.Text = tagCount.ToString();
            RakeBackRun.Text = rakeback.ToString("#");

            if (_isResultRefreshing) {

                ResultRun.Text = result.ToString();
                if (result < 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                if (result > 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));

                EvBB100Run.Text = evBB100.ToString("0.0");
                if (evBB100 < 0) EvBB100Run.Foreground = new SolidColorBrush(Color.FromRgb(255, 125, 125));
                if (evBB100 > 0) EvBB100Run.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));
            }

            
          
        }

       // private bool isVisible = false;

       

        private void CleartStats() {
            ResultRun.Text = "";
            RakeBackRun.Text = "";
            EvBB100Run.Text = "";
            CountRun.Text = "";
        }




        private void Button_Click(object sender, RoutedEventArgs e) {
            //show or hide Results
            if (_isResultRefreshing) {
                ResultRun.Text = "";
                EvBB100Run.Text = "";
                ResultEnableButton.Background = new SolidColorBrush(Color.FromRgb(99, 99, 99));
                _isResultRefreshing = false;
            }
            else {
                ResultEnableButton.Background = new SolidColorBrush(Color.FromRgb(69, 96, 69));
                _isResultRefreshing = true;
            }
        }
    }
}
