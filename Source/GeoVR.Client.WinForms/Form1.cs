using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            latitude = gMapControl1.Position.Lat;
            longitude = gMapControl1.Position.Lng;

            lonLatLabel.Text = latitude.ToString() + "/" + longitude.ToString();

                      

            
        }




    }

}
