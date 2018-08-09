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

namespace GeoVR.Client.WinForms
{
    public partial class Form1 : Form
    {

        double latitude;
        double longitude;
     


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            InitMap();
            AddMarker(51, 0, "EGLL_APP");
            AddRadioRing(51, 0, 50.0f);
          

        }


        private void AddRadioRing(double lat,double lon, double range)
        {

            GMapOverlay polygons = new GMapOverlay("polygons");
         
            List<PointLatLng> gpollist = new List<PointLatLng>();

            double seg = Math.PI * 2 / 30;

            int y = 0;
            for (int i = 0; i < 30; i++)
            {
                double theta = seg * i;
                double a = lat + Math.Cos(theta) * range/100*0.75f;
                double b = lon + Math.Sin(theta) * range/100;

                PointLatLng gpoi = new PointLatLng(a, b);

                gpollist.Add(gpoi);
            }

            GMapPolygon polygon = new GMapPolygon(gpollist, "Jardin des Tuileries");
            gMapControl1.Overlays.Add(polygons);
            polygons.Polygons.Add(polygon);
          
        }

        private void AddMarker(double lat,double lon,string name)
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

        private void txTrackBar_Scroll(object sender, EventArgs e)
        {
            txDistLabel.Text = txTrackBar.Value.ToString();
        }

        private void rxTrackBar_Scroll(object sender, EventArgs e)
        {
            rxDistLabel.Text = rxTrackBar.Value.ToString();
        }

        private void gMapControl1_OnPositionChanged(PointLatLng point)
        {
            latitude = point.Lat;
            longitude = point.Lng;

            lonLatLabel.Text = latitude.ToString() + "/" + longitude.ToString();
            gMapControl1.Overlays.Clear();
            AddRadioRing(latitude,longitude, txTrackBar.Value);
            gMapControl1.ReloadMap();

        }




    }

}
