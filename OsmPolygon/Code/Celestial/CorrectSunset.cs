
namespace OsmPolygon.Celestial2
{


	// https://dotnetfiddle.net/Authors/58964/Erik%20Murphy
	// https://dotnetfiddle.net/yT2TXa
	// broken ?
	public class Test
	{
		public static void TestMe()
		{
			int year = 2020;
			int month = 9;
			int day = 2;
			double lat = 57.708900;
			double lng = 11.974600;
			string timeZoneName = System.TimeZoneInfo.Local.Id;

			SolarCalculator solarCalculator = new SolarCalculator();

			SolarEvents solarEvents = solarCalculator.Calculate(year, month, day, 57.708900, 11.974600, timeZoneName);
			System.Console.WriteLine("Sunrise: " + solarEvents.Sunrise);
			System.Console.WriteLine("Sunset: " + solarEvents.Sunset);
		}
	}

	public class SolarEvents
	{
		public System.DateTimeOffset Sunrise { get; set; }
		public System.DateTimeOffset Sunset { get; set; }
	}

	public class SolarCalculator
	{
		private const int secondsInDay = 24 * 60 * 60;
		private const double J0 = 0.0009;
		private const double J1970 = 2440588;
		private const double J2000 = 2451545;
		private const double perihelionOfEarth = 102.9372;
		private const double obliquityOfEarth = 23.4397;
		private const double solarDiskAltitude = -0.83;

		public SolarEvents Calculate(int year, int month, int day, double latitude, double longitude, string timezoneName)
		{
			double lw = toRad(-longitude);
			double phi = toRad(latitude);

			System.DateTimeOffset gregorianDate = new System.DateTimeOffset(year, month, day, 0, 0, 0, System.TimeSpan.Zero);
			double julianDate = this.ConvertGregorianToJulian(gregorianDate);
			double d = this.CalculateJulianDays(julianDate);
			double n = this.CalculateJulianCycle(d, lw);
			double ds = this.CalculateApproximateTransit(0, lw, n);
			double M = this.CalculateSolarMeanAnomaly(ds);
			double C = this.CalculateEquationOfCenter(M);
			double L = this.CalculateEclipticLongitude(M, C);
			double dec = this.CalculateDeclination(L);
			double et = this.CalculateEquationOfTime(M, L);
			double jNoon = this.CalculateSolarNoon(ds, et);
			double w = this.CalculateHourAngle(phi, dec);
			double a = this.CalculateApproximateTransit(w, lw, n);
			double jSet = this.CalculateSolarNoon(a, et);
			double jRise = jNoon - (jSet - jNoon);
			long gSet = this.ConvertJulianToGregorian(jSet);
			long gRise = this.ConvertJulianToGregorian(jRise);
			System.TimeZoneInfo timezone = System.TimeZoneInfo.FindSystemTimeZoneById(timezoneName);
			System.DateTimeOffset sunset = this.ConvertGregorianToDate(gSet, timezone);
			System.DateTimeOffset sunrise = this.ConvertGregorianToDate(gRise, timezone);

			return new SolarEvents
			{
				Sunrise = sunrise,
				Sunset = sunset
			};
		}

		private double ConvertGregorianToJulian(System.DateTimeOffset gregorianDate)
		{
			return gregorianDate.ToUnixTimeSeconds() / secondsInDay - 0.5 + J1970;
		}

		private long ConvertJulianToGregorian(double julianDate)
		{
			return (long)((julianDate + 0.5 - J1970) * secondsInDay);
		}

		private System.DateTimeOffset ConvertGregorianToDate(long timestamp, System.TimeZoneInfo timezone)
		{
			return System.TimeZoneInfo.ConvertTime(System.DateTimeOffset.FromUnixTimeSeconds(timestamp), timezone);
		}

		public double CalculateJulianDays(double julianDate)
		{
			return julianDate - J2000;
		}

		public double CalculateJulianCycle(double d, double lw)
		{
			return System.Math.Round(d - J0 - (lw / (2 * System.Math.PI)));
		}

		public double CalculateApproximateTransit(double Ht, double lw, double n)
		{
			return J0 + ((Ht + lw) / (2 * System.Math.PI)) + n;
		}

		public double CalculateSolarMeanAnomaly(double ds)
		{
			return toRad(357.5291 + 0.98560028 * ds);
		}

		public double CalculateEquationOfCenter(double M)
		{
			return toRad(1.9148 * System.Math.Sin(M) + 0.02 * System.Math.Sin(2 * M) + 0.0003 * System.Math.Sin(3 * M));
		}

		public double CalculateEclipticLongitude(double M, double C)
		{
			return M + C + toRad(perihelionOfEarth) + System.Math.PI;
		}

		public double CalculateDeclination(double L)
		{
			return System.Math.Asin(System.Math.Sin(toRad(obliquityOfEarth)) * System.Math.Sin(L));
		}

		public double CalculateEquationOfTime(double M, double L)
		{
			return 0.0053 * System.Math.Sin(M) - 0.0069 * System.Math.Sin(2 * L);
		}

		public double CalculateSolarNoon(double ds, double et)
		{
			return J2000 + ds + et;
		}

		public double CalculateHourAngle(double phi, double dec)
		{
			return System.Math.Acos((System.Math.Sin(toRad(solarDiskAltitude)) - (System.Math.Sin(phi) * System.Math.Sin(dec))) / (System.Math.Cos(phi) * System.Math.Cos(dec)));
		}

		private double toRad(double degrees)
		{
			return degrees * (System.Math.PI / 180);
		}


	}


}
