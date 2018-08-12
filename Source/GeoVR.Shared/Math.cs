using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoVR.Shared
{
    public static class Math
    {
        public static double DistanceBetweenTwoPointsMeters(double startLat, double startLong, double endLat, double endLong)
        {
            var startPoint = new GeoCoordinate(startLat, startLong);
            var endPoint = new GeoCoordinate(endLat, endLong);

            return startPoint.GetDistanceTo(endPoint);
        }
    }
}
