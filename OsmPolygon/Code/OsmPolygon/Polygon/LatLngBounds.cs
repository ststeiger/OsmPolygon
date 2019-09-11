
namespace GeoApis
{


    [System.Diagnostics.DebuggerDisplay("{North} {West}, {South} {East}")]
    public class LatLngBounds
    {
        decimal North;
        decimal South;
        decimal East;
        decimal West;



        public LatLngBounds(decimal north, decimal south, decimal east, decimal west)
        {
            this.North = north;
            this.South = south;
            this.East = east;
            this.West = west;
        }


        public LatLngBounds(LatLng southWest, LatLng northEast)
            :this(northEast.lat, southWest.lat, northEast.lng, southWest.lng)
        { }

        public LatLngBounds(LatLng[] latlongs)
            :this(latlongs[0], latlongs[1])
        { }

        public LatLngBounds()
            : this(0m, 0m, 0m, 0m)
        { }


        public LatLng Center{ get { return new LatLng() { lat = (North +South)/2.0m, lng = (West+East)/2.0m }; } }

        public LatLng NorthEast { get{ return new LatLng() { lat = North, lng = East }; } }
        public LatLng NorthWest { get { return new LatLng() { lat = North, lng = West }; } }
        public LatLng SouthWest { get{ return new LatLng() { lat=South, lng = West }; } }
        public LatLng SouthEast { get { return new LatLng() { lat = South, lng = East }; } }
        

        public string ToBBoxString()
        {
            decimal[] a = new decimal[]{this.West, this.South, this.East, this.North};
            
            return a.Join(",");
        }


        public decimal BoundsArea
        {
            get
            {
                LatLng nw = this.NorthWest;
                LatLng se = this.SouthEast;
                // https://github.com/openstreetmap/cgimap/blob/master/src/bbox.cpp

                decimal maxLng = System.Math.Max(nw.lng, se.lng);
                decimal maxLat = System.Math.Max(nw.lat, se.lat);

                decimal minLng = System.Math.Min(nw.lng, se.lng);
                decimal minLat = System.Math.Min(nw.lat, se.lat);
                decimal area = (maxLng - minLng) * (maxLat - minLat);

                return area;
            }

        } // End Property BoundsArea 


        // radius = sizeInMeters/2
        public static LatLngBounds FromPoint(LatLng point, decimal sizeInMeters)
        {
            decimal latAccuracy = 180.0m * sizeInMeters / 40075017m;
            decimal lngAccuracy = latAccuracy / (decimal)System.Math.Cos((System.Math.PI / 180.0d) * (double)point.lat);

            //           N
            //          180
            // (W) -180     +180 (E)
            //         -180
            //           S

            // https://github.com/Leaflet/Leaflet/blob/master/src/geo/LatLng.js
            // https://github.com/Leaflet/Leaflet/blob/master/src/geo/LatLngBounds.js
            // constructor(southWest: LatLngExpression, northEast: LatLngExpression);
            // a = [point.lat - latAccuracy, point.lng - lngAccuracy],
            // b = [point.lat + latAccuracy, point.lng + lngAccuracy]
            // new LatLngBounds(a, b); 

            decimal south = point.lat - latAccuracy;
            decimal west = point.lng - lngAccuracy;
            decimal north = point.lat + latAccuracy;
            decimal east = point.lng + lngAccuracy;


            // https://en.wikipedia.org/wiki/Ellipse
            // https://en.wikipedia.org/wiki/Latitude
            // https://en.wikipedia.org/wiki/Longitude
            // https://de.wikipedia.org/wiki/Wendekreis_(Breitenkreis)

            return new LatLngBounds(north, south, east, west);
        }


    }


}
