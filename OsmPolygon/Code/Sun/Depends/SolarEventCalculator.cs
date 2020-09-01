/*
 * Copyright 2008-2009 Mike Reedell / LuckyCatLabs.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using OsmPolygon.sunrisesunset.dto;
using System;


namespace OsmPolygon.sunrisesunset
{




//package com.luckycatlabs.sunrisesunset.calculator;

//import java.math.BigFloat;
//import java.math.MathContext;
//import java.math.RoundingMode;
//import java.util.Calendar;
//import java.util.TimeZone;

//import com.luckycatlabs.sunrisesunset.Zenith;
//import com.luckycatlabs.sunrisesunset.dto.Location;

/**
 * Parent class of the Sunrise and Sunset calculator classes.
 */
public class SolarEventCalculator
{
    private Location location;
    private TimeZone timeZone;

    /**
     * Constructs a new <code>SolarEventCalculator</code> using the given parameters.
     *
     * @param location
     *            <code>Location</code> of the place where the solar event should be calculated from.
     * @param timeZoneIdentifier
     *            time zone identifier of the timezone of the location parameter. For example,
     *            "America/New_York".
     */
    public SolarEventCalculator(Location location, String timeZoneIdentifier)
    {
        this.location = location;
            
        this.timeZone = System.TimeZone.CurrentTimeZone; // TimeZone.GetTimeZone(timeZoneIdentifier);
    }

    /**
     * Constructs a new <code>SolarEventCalculator</code> using the given parameters.
     *
     * @param location
     *            <code>Location</code> of the place where the solar event should be calculated from.
     * @param timeZone
     *            timezone of the location parameter.
     */
    public SolarEventCalculator(Location location, TimeZone timeZone)
    {
        this.location = location;
        this.timeZone = timeZone;
    }

    /**
     * Computes the sunrise time for the given zenith at the given date.
     *
     * @param solarZenith
     *            <code>Zenith</code> enum corresponding to the type of sunrise to compute.
     * @param date
     *            <code>Calendar</code> object representing the date to compute the sunrise for.
     * @return the sunrise time, in HH:MM format (24-hour clock), 00:00 if the sun does not rise on the given
     *         date.
     */
    public String computeSunriseTime(Zenith solarZenith, Calendar date)
    {
        return getLocalTimeAsString(computeSolarEventTime(solarZenith, date, true));
    }

    /**
     * Computes the sunrise time for the given zenith at the given date.
     *
     * @param solarZenith
     *            <code>Zenith</code> enum corresponding to the type of sunrise to compute.
     * @param date
     *            <code>Calendar</code> object representing the date to compute the sunrise for.
     * @return the sunrise time as a calendar or null for no sunrise
     */
    public Calendar computeSunriseCalendar(Zenith solarZenith, Calendar date)
    {
        return getLocalTimeAsCalendar(computeSolarEventTime(solarZenith, date, true), date);
    }

    /**
     * Computes the sunset time for the given zenith at the given date.
     *
     * @param solarZenith
     *            <code>Zenith</code> enum corresponding to the type of sunset to compute.
     * @param date
     *            <code>Calendar</code> object representing the date to compute the sunset for.
     * @return the sunset time, in HH:MM format (24-hour clock), 00:00 if the sun does not set on the given
     *         date.
     */
    public String computeSunsetTime(Zenith solarZenith, Calendar date)
    {
        return getLocalTimeAsString(computeSolarEventTime(solarZenith, date, false));
    }

    /**
     * Computes the sunset time for the given zenith at the given date.
     *
     * @param solarZenith
     *            <code>Zenith</code> enum corresponding to the type of sunset to compute.
     * @param date
     *            <code>Calendar</code> object representing the date to compute the sunset for.
     * @return the sunset time as a Calendar or null for no sunset.
     */
    public Calendar computeSunsetCalendar(Zenith solarZenith, Calendar date)
    {
        return getLocalTimeAsCalendar(computeSolarEventTime(solarZenith, date, false), date);
    }

    private BigFloat computeSolarEventTime(Zenith solarZenith, Calendar date, bool isSunrise)
    {
        date.setTimeZone(this.timeZone);
        BigFloat longitudeHour = getLongitudeHour(date, isSunrise);

        BigFloat meanAnomaly = getMeanAnomaly(longitudeHour);
        BigFloat sunTrueLong = getSunTrueLongitude(meanAnomaly);
        BigFloat cosineSunLocalHour = getCosineSunLocalHour(sunTrueLong, solarZenith);
        if ((cosineSunLocalHour.doubleValue() < -1.0) || (cosineSunLocalHour.doubleValue() > 1.0))
        {
            return null;
        }

        BigFloat sunLocalHour = getSunLocalHour(cosineSunLocalHour, isSunrise);
        BigFloat localMeanTime = getLocalMeanTime(sunTrueLong, longitudeHour, sunLocalHour);
        BigFloat localTime = getLocalTime(localMeanTime, date);
        return localTime;
    }

    /**
     * Computes the base longitude hour, lngHour in the algorithm.
     *
     * @return the longitude of the location of the solar event divided by 15 (deg/hour), in
     *         <code>BigFloat</code> form.
     */
    private BigFloat getBaseLongitudeHour()
    {
        return divideBy(location.getLongitude(), BigFloat.valueOf(15));
    }

    /**
     * Computes the longitude time, t in the algorithm.
     *
     * @return longitudinal time in <code>BigFloat</code> form.
     */
    private BigFloat getLongitudeHour(Calendar date, bool isSunrise)
    {
        int offset = 18;
        if (isSunrise)
        {
            offset = 6;
        }
        BigFloat dividend = BigFloat.valueOf(offset).Subtract(getBaseLongitudeHour());
        BigFloat addend = divideBy(dividend, BigFloat.valueOf(24));
        BigFloat longHour = getDayOfYear(date).Add(addend);
        return setScale(longHour);
    }

    /**
     * Computes the mean anomaly of the Sun, M in the algorithm.
     *
     * @return the suns mean anomaly, M, in <code>BigFloat</code> form.
     */
    private BigFloat getMeanAnomaly(BigFloat longitudeHour)
    {
        BigFloat meanAnomaly = multiplyBy(new BigFloat("0.9856"), longitudeHour).Subtract(new BigFloat("3.289"));
        return setScale(meanAnomaly);
    }

    /**
     * Computes the true longitude of the sun, L in the algorithm, at the given location, adjusted to fit in
     * the range [0-360].
     *
     * @param meanAnomaly
     *            the suns mean anomaly.
     * @return the suns true longitude, in <code>BigFloat</code> form.
     */
    private BigFloat getSunTrueLongitude(BigFloat meanAnomaly)
    {
        BigFloat sinMeanAnomaly = new BigFloat(Math.Sin(convertDegreesToRadians(meanAnomaly).doubleValue()));
        BigFloat sinDoubleMeanAnomaly = new BigFloat(Math.Sin(multiplyBy(convertDegreesToRadians(meanAnomaly), BigFloat.valueOf(2))
                .doubleValue()));

        BigFloat firstPart = meanAnomaly.Add(multiplyBy(sinMeanAnomaly, new BigFloat("1.916")));
        BigFloat secondPart = multiplyBy(sinDoubleMeanAnomaly, new BigFloat("0.020")).Add(new BigFloat("282.634"));
        BigFloat trueLongitude = firstPart.Add(secondPart);

        if (trueLongitude.doubleValue() > 360)
        {
            trueLongitude = trueLongitude.Subtract(BigFloat.valueOf(360));
        }
        return setScale(trueLongitude);
    }

    /**
     * Computes the suns right ascension, RA in the algorithm, adjusting for the quadrant of L and turning it
     * into degree-hours. Will be in the range [0,360].
     *
     * @param sunTrueLong
     *            Suns true longitude, in <code>BigFloat</code>
     * @return suns right ascension in degree-hours, in <code>BigFloat</code> form.
     */
    private BigFloat getRightAscension(BigFloat sunTrueLong)
    {
        BigFloat tanL = new BigFloat(Math.Tan(convertDegreesToRadians(sunTrueLong).doubleValue()));

        BigFloat innerParens = multiplyBy(convertRadiansToDegrees(tanL), new BigFloat("0.91764"));
        BigFloat rightAscension = new BigFloat(Math.Atan(convertDegreesToRadians(innerParens).doubleValue()));
        rightAscension = setScale(convertRadiansToDegrees(rightAscension));

        if (rightAscension.doubleValue() < 0)
        {
            rightAscension = rightAscension.Add(BigFloat.valueOf(360));
        }
        else if (rightAscension.doubleValue() > 360)
        {
            rightAscension = rightAscension.Subtract(BigFloat.valueOf(360));
        }

        BigFloat ninety = BigFloat.valueOf(90);
        BigFloat longitudeQuadrant = sunTrueLong.Divide(ninety, 0, RoundingMode.FLOOR);
        longitudeQuadrant = longitudeQuadrant.Multiply(ninety);

        BigFloat rightAscensionQuadrant = rightAscension.Divide(ninety, 0, RoundingMode.FLOOR);
        rightAscensionQuadrant = rightAscensionQuadrant.Multiply(ninety);

        BigFloat augend = longitudeQuadrant.Subtract(rightAscensionQuadrant);
        return divideBy(rightAscension.Add(augend), BigFloat.valueOf(15));
    }

    private BigFloat getCosineSunLocalHour(BigFloat sunTrueLong, Zenith zenith)
    {
        BigFloat sinSunDeclination = getSinOfSunDeclination(sunTrueLong);
        BigFloat cosineSunDeclination = getCosineOfSunDeclination(sinSunDeclination);

        BigFloat zenithInRads = convertDegreesToRadians(zenith.degrees());
        BigFloat cosineZenith = BigFloat.valueOf(Math.Cos(zenithInRads.doubleValue()));
        BigFloat sinLatitude = BigFloat.valueOf(Math.Sin(convertDegreesToRadians(location.getLatitude()).doubleValue()));
        BigFloat cosLatitude = BigFloat.valueOf(Math.Cos(convertDegreesToRadians(location.getLatitude()).doubleValue()));

        BigFloat sinDeclinationTimesSinLat = sinSunDeclination.Multiply(sinLatitude);
        BigFloat dividend = cosineZenith.Subtract(sinDeclinationTimesSinLat);
        BigFloat divisor = cosineSunDeclination.Multiply(cosLatitude);

        return setScale(divideBy(dividend, divisor));
    }

    private BigFloat getSinOfSunDeclination(BigFloat sunTrueLong)
    {
        BigFloat sinTrueLongitude = BigFloat.valueOf(Math.Sin(convertDegreesToRadians(sunTrueLong).doubleValue()));
        BigFloat sinOfDeclination = sinTrueLongitude.Multiply(new BigFloat("0.39782"));
        return setScale(sinOfDeclination);
    }

    private BigFloat getCosineOfSunDeclination(BigFloat sinSunDeclination)
    {
        BigFloat arcSinOfSinDeclination = BigFloat.valueOf(Math.Asin(sinSunDeclination.doubleValue()));
        BigFloat cosDeclination = BigFloat.valueOf(Math.Cos(arcSinOfSinDeclination.doubleValue()));
        return setScale(cosDeclination);
    }

    private BigFloat getSunLocalHour(BigFloat cosineSunLocalHour, bool isSunrise)
    {
        BigFloat arcCosineOfCosineHourAngle = getArcCosineFor(cosineSunLocalHour);
        BigFloat localHour = convertRadiansToDegrees(arcCosineOfCosineHourAngle);
        if (isSunrise)
        {
            localHour = BigFloat.valueOf(360).Subtract(localHour);
        }
        return divideBy(localHour, BigFloat.valueOf(15));
    }

    private BigFloat getLocalMeanTime(BigFloat sunTrueLong, BigFloat longitudeHour, BigFloat sunLocalHour)
    {
        BigFloat rightAscension = this.getRightAscension(sunTrueLong);
        BigFloat innerParens = longitudeHour.Multiply(new BigFloat("0.06571"));
        BigFloat localMeanTime = sunLocalHour.Add(rightAscension).Subtract(innerParens);
        localMeanTime = localMeanTime.Subtract(new BigFloat("6.622"));

        if (localMeanTime.doubleValue() < 0)
        {
            localMeanTime = localMeanTime.Add(BigFloat.valueOf(24));
        }
        else if (localMeanTime.doubleValue() > 24)
        {
            localMeanTime = localMeanTime.Subtract(BigFloat.valueOf(24));
        }
        return setScale(localMeanTime);
    }

    private BigFloat getLocalTime(BigFloat localMeanTime, Calendar date)
    {
        BigFloat utcTime = localMeanTime.Subtract(getBaseLongitudeHour());
        BigFloat utcOffSet = getUTCOffSet(date);
        BigFloat utcOffSetTime = utcTime.Add(utcOffSet);
        return adjustForDST(utcOffSetTime, date);
    }

    private BigFloat adjustForDST(BigFloat localMeanTime, Calendar date)
    {
        BigFloat localTime = localMeanTime;
        if (timeZone.IsDaylightSavingTime(date.getTime()))
        {
            localTime = localTime.Add(BigFloat.One);
        }
        if (localTime.doubleValue() > 24.0)
        {
            localTime = localTime.Subtract(BigFloat.valueOf(24));
        }
        return localTime;
    }

    /**
     * Returns the local rise/set time in the form HH:MM.
     *
     * @param localTime
     *            <code>BigFloat</code> representation of the local rise/set time.
     * @return <code>String</code> representation of the local rise/set time in HH:MM format.
     */
    private String getLocalTimeAsString(BigFloat localTimeParam)
    {
        if (localTimeParam == null)
        {
            return "99:99";
        }

        BigFloat localTime = localTimeParam;
        if (localTime.CompareTo(BigFloat.Zero) == -1)
        {
            localTime = localTime.Add(BigFloat.valueOf(24.0D));
        }

            String[] timeComponents = localTime.toPlainString().Split("\\.");
        int hour = int.Parse(timeComponents[0], System.Globalization.CultureInfo.InvariantCulture);

        BigFloat minutes = new BigFloat("0." + timeComponents[1]);
        minutes = minutes.Multiply(BigFloat.valueOf(60)).setScale(0, RoundingMode.HALF_EVEN);
        if (minutes.intValue() == 60)
        {
            minutes = BigFloat.Zero;
            hour += 1;
        }
        if (hour == 24)
        {
            hour = 0;
        }

        String minuteString = minutes.intValue() < 10 ? "0" + minutes.toPlainString() : minutes.toPlainString();
        String hourString = (hour < 10) ? "0" + hour.ToString(System.Globalization.CultureInfo.InvariantCulture) 
                : hour.ToString(System.Globalization.CultureInfo.InvariantCulture);
            return hourString + ":" + minuteString;
    }

    /**
     * Returns the local rise/set time in the form HH:MM.
     *
     * @param localTimeParam
     *            <code>BigFloat</code> representation of the local rise/set time.
     * @return <code>Calendar</code> representation of the local time as a calendar, or null for none.
     */
    protected Calendar getLocalTimeAsCalendar(BigFloat localTimeParam, Calendar date)
    {
        if (localTimeParam == null)
        {
            return null;
        }

        // Create a clone of the input calendar so we get locale/timezone information.
        Calendar resultTime = (Calendar)date.clone();

        BigFloat localTime = localTimeParam;
        if (localTime.CompareTo(BigFloat.Zero) == -1)
        {
            localTime = localTime.Add(BigFloat.valueOf(24.0D));
            resultTime.Add(Calendar.HOUR_OF_DAY, -24);
        }
        String[] timeComponents = localTime.toPlainString().Split("\\.");
        int hour = int.Parse(timeComponents[0], System.Globalization.CultureInfo.InvariantCulture);

        BigFloat minutes = new BigFloat("0." + timeComponents[1]);
        minutes = minutes.Multiply(BigFloat.valueOf(60)).setScale(0, RoundingMode.HALF_EVEN);
        if (minutes.intValue() == 60)
        {
            minutes = BigFloat.Zero;
            hour += 1;
        }
        if (hour == 24)
        {
            hour = 0;
        }

        // Set the local time
        resultTime.set(Calendar.HOUR_OF_DAY, hour);
        resultTime.set(Calendar.MINUTE, minutes.intValue());
        resultTime.set(Calendar.SECOND, 0);
        resultTime.set(Calendar.MILLISECOND, 0);
        resultTime.setTimeZone(date.getTimeZone());

        return resultTime;
    }

    /** ******* UTILITY METHODS (Should probably go somewhere else. ***************** */

    private BigFloat getDayOfYear(Calendar date)
    {
        return new BigFloat(date.get(Calendar.DAY_OF_YEAR));
    }

    private BigFloat getUTCOffSet(Calendar date)
    {
        BigFloat offSetInMillis = new BigFloat(date.get(Calendar.ZONE_OFFSET));
        BigFloat offSet = offSetInMillis.Divide(new BigFloat(3600000), new MathContext(2));
        return offSet;
    }

    private BigFloat getArcCosineFor(BigFloat radians)
    {
        BigFloat arcCosine = BigFloat.valueOf(Math.Acos(radians.doubleValue()));
        return setScale(arcCosine);
    }

    private BigFloat convertRadiansToDegrees(BigFloat radians)
    {
        return multiplyBy(radians, new BigFloat(180 / Math.PI));
    }

    private BigFloat convertDegreesToRadians(BigFloat degrees)
    {
        return multiplyBy(degrees, BigFloat.valueOf(Math.PI / 180.0));
    }

    private BigFloat multiplyBy(BigFloat multiplicand, BigFloat multiplier)
    {
        return setScale(multiplicand.Multiply(multiplier));
    }

    private BigFloat divideBy(BigFloat dividend, BigFloat divisor)
    {
        return dividend.Divide(divisor, 4, RoundingMode.HALF_EVEN);
    }

    private BigFloat setScale(BigFloat number)
    {
        return number.setScale(4, RoundingMode.HALF_EVEN);
    }
}


}