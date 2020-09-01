
namespace OsmPolygon.Celestial
{


    // NOAA 
    // TODO: https://www.codeproject.com/Articles/78486/Solar-Calculator-Calculate-Sunrise-Sunset-and-Maxi
    public class Test
    {

        // https://dotnetfiddle.net/N3j5th
        public static void TestMe()
        {

            // http://www.stargazing.net/kepler/sunrise.html
            int year = 2020;
            int month = 9;
            int day = 2;
            double lat = 57.708900;
            double lng = 11.974600;
            string timeZoneName = System.TimeZoneInfo.Local.Id;

            SolarCalculator solarCalculator = new SolarCalculator();

            SolarEvents solarEvents = solarCalculator.Calculate(year, month, day, lat, lng, timeZoneName);

            System.Console.WriteLine("Sunrise: " + solarEvents.Sunrise);
            System.Console.WriteLine("Sunset: " + solarEvents.Sunset);
        }
    }

    public class SolarEvents
    {
        public System.DateTimeOffset Sunrise { get; set; }
        public System.DateTimeOffset Sunset { get; set; }
    }

    /// <summary>
    /// Solar calculator used to  the position of the sun
    /// relative to a vantage point on the earth. The primary output is the 
    /// local time of sunrise and sunset for a given long/lat and timezone.
    /// 
    /// The functions for the equations don't call eachother. They are kept
    /// flat for simplicity and testability. The function names represent what they are
    /// but are not phrased as actions (i.e. prefixed with "Calculate") to avoid redundancy.
    /// The variable names used in the mathematical formulas are abbreviations
    /// as opposed to standard descriptive names for readability.
    /// 
    /// These calculations are based off of the NOAA solar calc spreadsheet:
    /// https://www.esrl.noaa.gov/gmd/grad/solcalc/calcdetails.html
    /// https://www.esrl.noaa.gov/gmd/grad/solcalc/main.js
    /// 
    /// The following are some related sources of interest:
    /// https://en.wikipedia.org/wiki/Sunrise_equation
    /// https://github.com/mourner/suncalc/blob/master/suncalc.js
    /// http://aa.quae.nl/en/reken/zonpositie.html
    /// http://users.electromagnetic.net/bu/astro/sunrise-set.php
    /// </summary>
    public class SolarCalculator
    {
        private const double MinutesInDay = 24 * 60;
        private const double SecondsInDay = MinutesInDay * 60;
        private const double J2000 = 2451545;

        /// <summary>
        ///  the sunrise/sunset for the given calendar date in the given timezone at the given location.
        /// Accounts for DST.
        /// </summary>
        /// <param name="year">Calendar year in timezone</param>
        /// <param name="month">Calendar month in timezone</param>
        /// <param name="day">Calendar day in timezone</param>
        /// <param name="latitude">Latitude of location in degrees</param>
        /// <param name="longitude">Longitude of location in degrees</param>
        /// <param name="timezoneId">Id of the timezone as specified by the .net framework</param>
        /// <returns></returns>
        public SolarEvents Calculate(int year, int month, int day, double latitude, double longitude, string timeZoneId)
        {
            double lat = latitude;
            double lng = longitude;
            System.DateTime gDate = new System.DateTime(year, month, day, 12, 0, 0, System.DateTimeKind.Utc);
            System.TimeZoneInfo timeZone = System.TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            System.TimeSpan timeZoneOffset = timeZone.GetUtcOffset(gDate);
            double tzOffHr = timeZoneOffset.TotalHours;
            double jDate = GregorianToJulian(gDate, tzOffHr); // D
            double t = JulianCentury(jDate); // G
            double ml = GeomMeanLongitudeSun(t); // I - deg
            double ma = GeomMeanAnomalySun(t); // J - deg
            double eo = EccentricityEarthOrbit(t); // K
            double eoc = EquationOfCenterSun(ma, t); // L
            double tl = TrueLongitudeSun(ml, eoc); // M - deg
            double al = ApparentLongitudeSun(tl, t); // P - deg
            double oe = MeanObliquityOfEcliptic(t); // Q - deg
            double oc = ObliquityCorrection(oe, t); // R - deg
            double d = DeclinationSun(oc, al); // T - deg
            double eot = EquationOfTime(oc, ml, eo, ma); // V - minutes
            double ha = HourAngleSunrise(lat, d); // W - Deg
            double sn = SolarNoon(lng, eot, tzOffHr); // X - LST
            double sunrise = Sunrise(sn, ha); // Y - LST
            double sunset = Sunset(sn, ha); // Z - LST
            System.DateTimeOffset sunriseOffset = ToDate(timeZone, gDate, sunrise);
            System.DateTimeOffset sunsetOffset = ToDate(timeZone, gDate, sunset);

            System.Console.WriteLine("timeZoneOffset: " + timeZoneOffset.TotalHours);
            System.Console.WriteLine("julianDate: " + jDate);
            System.Console.WriteLine("julianCentury: " + t);
            System.Console.WriteLine("geomMeanLongitudeSun: " + ml);
            System.Console.WriteLine("geomMeanAnomalySun: " + ma);
            System.Console.WriteLine("eccentricityEarthOrbit: " + eo);
            System.Console.WriteLine("equationOfCenterSun: " + eoc);
            System.Console.WriteLine("trueLongitudeSun: " + tl);
            System.Console.WriteLine("solarApparentLongitude: " + al);
            System.Console.WriteLine("meanObliquityOfEcliptic: " + oe);
            System.Console.WriteLine("obliquityCorrection: " + oc);
            System.Console.WriteLine("equationOfTime: " + eot);
            System.Console.WriteLine("solarDecline: " + d);
            System.Console.WriteLine("hourAngleSunrise: " + ha);
            System.Console.WriteLine("solarNoon: " + sn);
            System.Console.WriteLine("sunrise: " + sunrise);
            System.Console.WriteLine("sunset: " + sunset);

            return new SolarEvents
            {
                Sunrise = sunriseOffset,
                Sunset = sunsetOffset
            };
        }

        private double GregorianToJulian(System.DateTime gDate, double timeZoneOffsetHours)
        {
            int year = gDate.Year;
            int month = gDate.Month;
            if (month <= 2)
            {
                year -= 1;
                month += 12;
            }
            double A = System.Math.Floor(year / 100d);
            double B = 2 - A + System.Math.Floor(A / 4d);
            double jDay = System.Math.Floor(365.25 * (year + 4716)) + System.Math.Floor(30.6001 * (month + 1)) + gDate.Day + B - 1524.5;
            double jTime = ((gDate.Hour * (60 * 60)) + (gDate.Minute * 60) + gDate.Second) / SecondsInDay;
            return jDay + jTime - timeZoneOffsetHours / 24;
        }

        public static System.DateTimeOffset ToDate(System.TimeZoneInfo timeZone, System.DateTime gDate, double time)
        {
            int hours = (int)System.Math.Floor(time * 24);
            int minutes = (int)System.Math.Floor((time * 24 * 60) % 60);
            int seconds = (int)System.Math.Floor((time * 24 * 60 * 60) % 60);
            return new System.DateTimeOffset(gDate.Year, gDate.Month, gDate.Day, hours, minutes, seconds, timeZone.GetUtcOffset(gDate));
        }

        private double JulianCentury(double jDate)
        {
            const double daysInCentury = 36525;
            return (jDate - J2000) / daysInCentury;
        }

        private double GeomMeanAnomalySun(double t)
        {
            return 357.52911 + t * (35999.05029 - 0.0001537 * t);
        }

        private double GeomMeanLongitudeSun(double t)
        {
            return Mod(280.46646 + t * (36000.76983 + t * 0.0003032), 0, 360);
        }

        private double EccentricityEarthOrbit(double t)
        {
            return 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
        }

        private double EquationOfCenterSun(double ma, double t)
        {
            return System.Math.Sin(Radians(ma)) * (1.914602 - t * (0.004817 + 0.000014 * t))
                + System.Math.Sin(Radians(2 * ma)) * (0.019993 - 0.000101 * t)
                + System.Math.Sin(Radians(3 * ma)) * 0.000289;
        }

        private double TrueLongitudeSun(double ml, double eoc)
        {
            return ml + eoc;
        }

        private double ApparentLongitudeSun(double tl, double t)
        {
            return tl - 0.00569 - 0.00478 * System.Math.Sin(Radians(125.04 - 1934.136 * t));
        }

        private double MeanObliquityOfEcliptic(double t)
        {
            return 23 + (26 + ((21.448 - t * (46.815 + t * (0.00059 - t * 0.001813)))) / 60) / 60;
        }

        private double ObliquityCorrection(double oe, double t)
        {
            return oe + 0.00256 * System.Math.Cos(Radians(125.04 - 1934.136 * t));
        }

        private double EquationOfTime(double oc, double ml, double eo, double ma)
        {
            double y = System.Math.Tan(Radians(oc / 2)) * System.Math.Tan(Radians(oc / 2)); // U
            double eTime = y * System.Math.Sin(2 * Radians(ml))
                - 2 * eo * System.Math.Sin(Radians(ma))
                + 4 * eo * y * System.Math.Sin(Radians(ma)) * System.Math.Cos(2 * Radians(ml))
                - 0.5 * y * y * System.Math.Sin(4 * Radians(ml))
                - 1.25 * eo * eo * System.Math.Sin(2 * Radians(ma));
            return 4 * Degrees(eTime);
        }

        private double DeclinationSun(double oc, double al)
        {
            return Degrees(System.Math.Asin(System.Math.Sin(Radians(oc)) * System.Math.Sin(Radians(al))));
        }

        private double HourAngleSunrise(double lat, double d)
        {
            return Degrees(System.Math.Acos(System.Math.Cos(Radians(90.833)) / 
                (System.Math.Cos(Radians(lat)) * System.Math.Cos(Radians(d))) 
                - System.Math.Tan(Radians(lat)) * System.Math.Tan(Radians(d))));
        }

        private double SolarNoon(double lng, double eot, double tzOff)
        {
            return (720 - 4 * lng - eot + tzOff * 60) / MinutesInDay;
        }

        private double Sunrise(double sn, double ha)
        {
            return sn - ha * 4 / MinutesInDay;
        }

        private double Sunset(double sn, double ha)
        {
            return sn + ha * 4 / MinutesInDay;
        }


        private double Mod(double x, double lo, double hi)
        {
            while (x > hi) x -= hi;
            while (x < lo) x += hi;
            return x;
        }

        private double Radians(double degrees)
        {
            return degrees * System.Math.PI / 180;
        }

        private double Degrees(double radians)
        {
            return radians * 180 / System.Math.PI;
        }
    }


}
