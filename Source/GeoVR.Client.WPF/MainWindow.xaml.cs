using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeoVR.Client.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Client client = new Client();
        string address = "antifaffvoice.vatsim.uk";
        System.Timers.Timer timer = null;
        DateTime _startTime;

        public MainWindow()
        {
            InitializeComponent();
            client.Start(address, Environment.MachineName + " - " + Environment.UserName, Shared.ClientType.Mobile);
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.Interval = 100;
                timer.Elapsed += timer_Elapsed;
            }
            timer.Start();
            lbStatus.Content = "Status: Started.";
            _startTime = DateTime.Now;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            PTTon();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            PTToff();
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var timeDiff = DateTime.Now - _startTime;
            //var bpsSend = client.BytesSent / timeDiff.TotalSeconds;
            //var bpsReceive = client.B    ytesReceived / timeDiff.TotalSeconds;
            Dispatcher.Invoke(() => { lbStats.Content = string.Format("Sent: {0:N0} B, Received: {1:N0} B", client.ClientStatistics.AudioBytesSent, client.ClientStatistics.AudioBytesReceived); });
            Dispatcher.Invoke(() => { lbUsers.ItemsSource = client.LastReceivedOneSecondInfo.ClientIDs; });
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            PTToff();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            PTToff();
        }

        private void PTToff()
        {
            client.PTT(false);
            lbStatus.Content = "Status: Connected to antifaffvoice.vatsim.uk";
        }

        private void PTTon()
        {
            client.PTT(true);
            lbStatus.Content = "Status: Transmitting...";
        }
    }
}
