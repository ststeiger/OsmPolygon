
namespace GeoApis
{


    [System.Diagnostics.DebuggerDisplay("{lat} {lng}, {alt}")]
    public class LatLng
    {
        public decimal lat;
        public decimal lng;
        public decimal? alt;

        public LatLng(decimal latitude, decimal longitude)
        {
            this.lat = latitude;
            this.lng = longitude;
        }

        public LatLng()
            : this(0, 0)
        { }


        public LatLng Clone()
        {
            return new LatLng(this.lat, this.lng);
        }

        public override string ToString()
        {
            return lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + ", " + lng.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }


    }


}
