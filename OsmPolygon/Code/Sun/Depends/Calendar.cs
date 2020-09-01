
namespace OsmPolygon.sunrisesunset
{


    public class Calendar
    {
        
        public static int DAY_OF_YEAR;
        public static int HOUR_OF_DAY;
        public static int MINUTE;
        public static int SECOND;
        public static int MILLISECOND;
        public static int ZONE_OFFSET;


        protected System.TimeZone m_tz;

        public void setTimeZone(System.TimeZone tz)
        {
            this.m_tz = tz;
        }

        public System.TimeZone getTimeZone()
        {
            return m_tz;
        }

        public System.DateTime getTime()
        {
            return System.DateTime.Now;
        }

        public Calendar clone()
        {
            return new Calendar();
        }


        public Calendar Add(int a, int b)
        {
            return null;
        }

        public Calendar set(int a, int b)
        {
            return null;
        }

        public string get(int a)
        {
            return null;
        }



    }
}
