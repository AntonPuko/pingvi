using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Npgsql;

namespace Pingvi
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public ResultsWindow()
        {
            InitializeComponent();
        }

        private Action _dbOpenAction;
        private DispatcherTimer _timer;
        private NpgsqlConnection _npgSqlConnection;
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this._dbOpenAction += () => {
                _timer = new DispatcherTimer() {
                    Interval = TimeSpan.FromSeconds(3),
                };

                _timer.Tick += OnTimerTick;
                _timer.IsEnabled = true;
            };
            ConnectToHMDB();
        }



        private void ConnectToHMDB() {
            string connectionString = "Server=localhost;Port=5432;User=postgres;Password=dbpass;Database=HoldemManager2;";
             _npgSqlConnection = new NpgsqlConnection(connectionString);
 
             _npgSqlConnection.StateChange += (o, a) =>
             {

                 if (a.CurrentState == ConnectionState.Open || _dbOpenAction != null) _dbOpenAction();
            };

             _npgSqlConnection.Open();
        }

        private void OnTimerTick(object sender, EventArgs e) {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT  tourneydata.buyinincents, tourneydata.winningsincents, tourneydata.rakeincents,  tourneydata.finishposition FROM public.tourneydata WHERE tourneydata.player_id =1 AND tourneydata.finishposition != 0 AND tourneydata.firsthandtimestamp > '2015-06-26 0:00';"
               , _npgSqlConnection);

            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

            int count = 0;
            double result = 0;
            double rake = 0;
            double vpp = 0;
            if (npgSqlDataReader.HasRows)
            {
                foreach (DbDataRecord dbDataRecord in npgSqlDataReader)
                {
                    count++;
                    // wining - (bi+rake)
                    double tRes = dbDataRecord.GetInt32(1) - (dbDataRecord.GetInt32(0) + dbDataRecord.GetInt32(2));
                    if (tRes.ToString().Length > 2) tRes = tRes / 100;
                    result += tRes;

                    double tRake = dbDataRecord.GetInt32(2) / 100.0;
                    rake += tRake;
                    vpp += tRake * 5.5;
                }
            }

            double rakeback = vpp * 3.5 / 40000 * 600;

            CountRun.Text = count.ToString();
            ResultRun.Text = result.ToString();
            if (result < 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(255,125,125));
            if (result > 0) ResultRun.Foreground = new SolidColorBrush(Color.FromRgb(90, 190, 80));
            RakeBackRun.Text = rakeback.ToString("##.#");

        }

        private bool isVisible = false;
     
            
        
    }
}
