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
        Client client = new Client();
        string address = "antifaffvoice.vatsim.uk";
        System.Windows.Forms.Timer timer = null;
        double latitude = 51;
        double longitude = 0;

        public Form1()
        {
            InitializeComponent();
            this.KeyPreview = true;
            client.Start(address, Environment.MachineName + " - " + Environment.UserName, Shared.ClientType.Mobile);
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
            client.SetPosition(latitude, longitude, 0);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            InitMap();

            RefreshMapMarkers();
        }


        private void AddRadioRing(double lat, double lon, double range)
        {
            range = range * 2;
            GMapOverlay polygons = new GMapOverlay("polygons");

            List<PointLatLng> gpollist = new List<PointLatLng>();

            double seg = Math.PI * 2 / 30;

            int y = 0;
            for (int i = 0; i < 30; i++)
            {
                double theta = seg * i;
                double a = lat + Math.Cos(theta) * range / 100 * 0.75f;
                double b = lon + Math.Sin(theta) * range / 100;

                PointLatLng gpoi = new PointLatLng(a, b);

                gpollist.Add(gpoi);
            }

            GMapPolygon polygon = new GMapPolygon(gpollist, "Jardin des Tuileries");
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
            latitude = point.Lat;
            longitude = point.Lng;
            RefreshMapMarkers();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                PTT(true);
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
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
            client.PTT(active);
            if (active)
                lblPTT.Text = "Radio: Transmitting";
            else
                lblPTT.Text = "Radio: Idle";
        }

        private void RefreshMapMarkers()
        {
            lonLatLabel.Text = latitude.ToString() + "/" + longitude.ToString();

            gMapControl1.Overlays.Clear();

            var positions = client.LastReceivedOneSecondInfo.ClientPositions;
            var radioRadii = client.LastReceivedOneSecondInfo.ClientRadioRadii;
            for (int i = 0; i < positions.Count; i++)
            {
                if (positions[i].ClientID != Environment.MachineName + " - " + Environment.UserName)
                    AddMarker(positions[i].LatDeg, positions[i].LonDeg, positions[i].ClientID);
                AddRadioRing(positions[i].LatDeg, positions[i].LonDeg, radioRadii[i].ReceiveRadiusM / 1609.34);
            }
            gMapControl1.ReloadMap();
        }
    }
}
