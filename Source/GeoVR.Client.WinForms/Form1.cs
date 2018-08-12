using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;
using System.Timers;

namespace GeoVR.Client.WinForms
{
    public partial class Form1 : Form
    {
        ClientManager clientManager = new ClientManager();
        string address = "antifaffvoice.vatsim.uk";
        System.Windows.Forms.Timer timer = null;
        double latDeg = 51;
        double lonDeg = 0;
        string username = "";

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;

            foreach (var inputDevice in clientManager.GetInputDevices())
            {
                cbInput.Items.Add(inputDevice);
            }

            foreach (var outputDevice in clientManager.GetOutputDevices())
            {
                cbOutput.Items.Add(outputDevice);
            }
            cbInput.SelectedIndex = 0;
            cbOutput.SelectedIndex = 0;

            cbFrequency.Items.Add("118.500");
            cbFrequency.Items.Add("121.900");
            cbFrequency.Items.Add("EGLL Controllers Conference Call");
            cbFrequency.SelectedIndex = 0;

            tbUsername.Text = Environment.UserName;

            if (timer == null)
            {
                timer = new System.Windows.Forms.Timer();
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
            }
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //var timeDiff = DateTime.Now - _startTime;
            //var bpsSend = client.BytesSent / timeDiff.TotalSeconds;
            //var bpsReceive = client.B    ytesReceived / timeDiff.TotalSeconds;
            //Dispatcher.Invoke(() => { lbStats.Content = string.Format("Sent: {0:N0} B, Received: {1:N0} B", client.ClientStatistics.AudioBytesSent, client.ClientStatistics.AudioBytesReceived); });
            //Dispatcher.Invoke(() => { lbUsers.ItemsSource = client.LastReceivedOneSecondInfo.ClientIDs; });
            RefreshMapMarkers();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitMap();
            //RefreshMapMarkers();
        }

        private void AddRadioRing(double lat, double lon, double range, Color color, string name)
        {
            range = range * 2;
            GMapOverlay polygons = new GMapOverlay("polygons");

            List<PointLatLng> gpollist = new List<PointLatLng>();

            double seg = Math.PI * 2 / 30;

            int y = 0;
            for (int i = 0; i < 30; i++)
            {
                double theta = seg * i;
                double a = lat + Math.Cos(theta) * range / 100;// * 0.75f;
                double b = lon + Math.Sin(theta) * range / 100;

                PointLatLng gpoi = new PointLatLng(a, b);

                gpollist.Add(gpoi);
            }

            GMapPolygon polygon = new GMapPolygon(gpollist, name);
            polygon.Stroke = new Pen(color);
            polygon.Fill = Brushes.Transparent;
            gMapControl1.Overlays.Add(polygons);
            polygons.Polygons.Add(polygon);

        }

        private void AddMarker(double lat, double lon, string name)
        {
            GMapOverlay markers = new GMapOverlay(name);
            GMapMarker marker = new GMarkerGoogle(
                new PointLatLng(lat, lon),
                GMarkerGoogleType.gray_small);

            marker.ToolTipText = name;
            gMapControl1.Overlays.Add(markers);
            markers.Markers.Add(marker);
        }

        public static double DistanceTwoPoint(double startLat, double startLong, double endLat, double endLong)
        {

            var startPoint = new GeoCoordinate(startLat, startLong);
            var endPoint = new GeoCoordinate(endLat, endLong);

            return startPoint.GetDistanceTo(endPoint);
        }

        private void InitMap()
        {
            gMapControl1.MapProvider = GoogleMapProvider.Instance;
            //get tiles from server only
            gMapControl1.Manager.Mode = AccessMode.ServerOnly;
            //not use proxy
            GMapProvider.WebProxy = null;
            //center map on moscow
            gMapControl1.Position = new PointLatLng(51.755786121111, 0.617633343333);

            gMapControl1.MinZoom = 1;
            gMapControl1.MaxZoom = 20;
            //set zoom
            gMapControl1.Zoom = 5;
        }

        private void gMapControl1_OnPositionChanged(PointLatLng point)
        {
            latDeg = point.Lat;
            lonDeg = point.Lng;
            clientManager.Position(latDeg, lonDeg, 9144);
            //RefreshMapMarkers();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                PTT(true);
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                PTT(false);
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            PTT(false);
        }

        private void PTT(bool active)
        {
            if (clientManager.PTT(active) & active)
                lblPTT.Text = "Radio: Transmitting";
            else
                lblPTT.Text = "Radio: Idle";
        }

        private void RefreshMapMarkers()
        {
            lonLatLabel.Text = latDeg.ToString() + "/" + lonDeg.ToString();

            gMapControl1.Overlays.Clear();

            foreach (var client in clientManager.LastReceivedAdminInfo.Clients)
            {
                switch (client.Type)
                {
                    case Shared.ClientType.RadioMobile:
                        if (client.Online)
                        {
                            if (client.Callsign != username)
                                AddMarker(client.Transceivers[0].LatDeg, client.Transceivers[0].LonDeg, client.Callsign + " - " + client.Frequency);
                            AddRadioRing(client.Transceivers[0].LatDeg, client.Transceivers[0].LonDeg, client.Transceivers[0].ReceiveRadiusMeters / 1609.34, Color.Red, client.Callsign + " receive");
                            AddRadioRing(client.Transceivers[0].LatDeg, client.Transceivers[0].LonDeg, client.Transceivers[0].TransmitRadiusMeters / 1609.34, Color.Blue, client.Callsign + " transmit");
                        }
                        break;
                    case Shared.ClientType.RadioFixed:
                        foreach (var transceiver in client.Transceivers)
                        {
                            AddMarker(transceiver.LatDeg, transceiver.LonDeg, client.Callsign + " - " + client.Frequency);
                            AddRadioRing(transceiver.LatDeg, transceiver.LonDeg, transceiver.ReceiveRadiusMeters / 1609.34, Color.Gray, client.Callsign + " receive");
                            AddRadioRing(transceiver.LatDeg, transceiver.LonDeg, transceiver.TransmitRadiusMeters / 1609.34, Color.Gray, client.Callsign + " transmit");
                        }
                        break;
                }

            }
            //gMapControl1.ReloadMap();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            //Validation
            var username = tbUsername.Text.Trim();
            var frequency = (string)cbFrequency.SelectedItem;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username invalid.");
                return;
            }

            if (string.IsNullOrWhiteSpace(frequency))
            {
                MessageBox.Show("Frequency invalid.");
                return;
            }

            if (cbInput.SelectedIndex < 0)
            {
                MessageBox.Show("Input device invalid.");
                return;
            }

            if (cbOutput.SelectedIndex < 0)
            {
                MessageBox.Show("Output device invalid.");
                return;
            }

            cbInput.Enabled = false;
            cbOutput.Enabled = false;
            tbUsername.Enabled = false;
            btnConnect.Enabled = false;
            lblPTT.Select();

            this.username = username;
            var inputDevice = (string)cbInput.SelectedItem;
            var outputDevice = (string)cbOutput.SelectedItem;
            clientManager.Start(address, username, inputDevice, outputDevice);
            clientManager.Position(latDeg, lonDeg, 9144);
            clientManager.Frequency(frequency);
        }

        private void cbFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            var frequency = (string)cbFrequency.SelectedItem;

            if (string.IsNullOrWhiteSpace(frequency))
            {
                MessageBox.Show("Frequency invalid.");
                return;
            }

            clientManager.Frequency(frequency);
        }
    }
}
