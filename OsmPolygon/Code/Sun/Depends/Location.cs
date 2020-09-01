using System;
using System.Collections.Generic;
using System.Text;

// import java.math.BigDecimal;


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


namespace OsmPolygon.sunrisesunset.dto
{





/**
 * Simple VO class to store latitude/longitude information.
 */
public class Location
{
    private BigFloat latitude;
    private BigFloat longitude;

    /**
     * Creates a new instance of <code>Location</code> with the given parameters.
     * 
     * @param latitude
     *            the latitude, in degrees, of this location. North latitude is positive, south negative.
     * @param longitude
     *            the longitude, in degrees of this location. East longitude is positive, west negative.
     */
    public Location(String latitude, String longitude)
    {
        this.latitude = new BigFloat(latitude);
        this.longitude = new BigFloat(longitude);
    }

    /**
     * Creates a new instance of <code>Location</code> with the given parameters.
     * 
     * @param latitude
     *            the latitude, in degrees, of this location. North latitude is positive, south negative.
     * @param longitude
     *            the longitude, in degrees, of this location. East longitude is positive, east negative.
     */
    public Location(double latitude, double longitude)
    {
        this.latitude = new BigFloat(latitude);
        this.longitude = new BigFloat(longitude);
    }

    /**
     * @return the latitude
     */
    public BigFloat getLatitude()
    {
        return latitude;
    }

    /**
     * @return the longitude
     */
    public BigFloat getLongitude()
    {
        return longitude;
    }

    /**
     * Sets the coordinates of the location object.
     *
     * @param latitude
     *            the latitude, in degrees, of this location. North latitude is positive, south negative.
     * @param longitude
     *            the longitude, in degrees, of this location. East longitude is positive, east negative.
     */
    public void setLocation(String latitude, String longitude)
    {
        this.latitude = new BigFloat(latitude);
        this.longitude = new BigFloat(longitude);
    }

    /**
     * Sets the coordinates of the location object.
     *
     * @param latitude
     *            the latitude, in degrees, of this location. North latitude is positive, south negative.
     * @param longitude
     *            the longitude, in degrees, of this location. East longitude is positive, east negative.
     */
    public void setLocation(double latitude, double longitude)
    {
        this.latitude = new BigFloat(latitude);
        this.longitude = new BigFloat(longitude);
    }
}

}
