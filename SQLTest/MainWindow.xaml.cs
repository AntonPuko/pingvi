using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Npgsql;

namespace SQLTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            string connectionString = "Server=localhost;Port=5432;User=postgres;Password=dbpass;Database=HoldemManager2;";
            NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionString);
            Debug.WriteLine(npgSqlConnection.State);
            npgSqlConnection.Open();
            Debug.WriteLine(npgSqlConnection.State);
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand("SELECT  tourneydata.buyinincents, tourneydata.winningsincents, tourneydata.rakeincents,  tourneydata.finishposition FROM public.tourneydata WHERE tourneydata.player_id =1 AND tourneydata.firsthandtimestamp > '2015-06-26 0:00';"
                , npgSqlConnection);

            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();

            int count = 0;
            double result = 0;
            double rake = 0;
            double vpp = 0;
            if (npgSqlDataReader.HasRows) {
                foreach (DbDataRecord dbDataRecord in npgSqlDataReader) {
                    count++;
                    // wining - (bi+rake)
                    double tRes = dbDataRecord.GetInt32(1) - (dbDataRecord.GetInt32(0) + dbDataRecord.GetInt32(2));
                    if (tRes.ToString().Length > 2) tRes = tRes/100;
                    result += tRes;
                    
                    double tRake = dbDataRecord.GetInt32(2)/100.0;
                    rake += tRake;
                    vpp += tRake * 5.5;
                }
            }

            double rakeback = vpp*3.5/40000*600;
            

            ResultLabel.Content = String.Format("count: {0} result: {1} rake: {2} vpp: {3} rakeback: {4}", count, result, rake, vpp, rakeback);
        }
    }
}
