
namespace GeoApis
{


    public static class ArrayExtensions
    {

        public static LatLng[] Reverse(this LatLng[] array)
        {
            LatLng[] result = new LatLng[array.Length];

            for (int i = 0; i < array.Length; ++i)
            {
                result[i] = array[array.Length - 1 - i].Clone();
            }

            return result;
        }


        public static string Join(this object[] mem, string separator)
        {
            string retValue = null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < mem.Length; ++i)
            {
                if (i != 0)
                    sb.Append(separator);

                sb.Append(System.Convert.ToString(mem[i], System.Globalization.CultureInfo.InvariantCulture));
            } // Next i 

            retValue = sb.ToString();
            sb.Clear();
            sb = null;

            return retValue;
        } // End Function Join 


        public static string Join(this decimal[] mem, string separator)
        {
            string retValue = null;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < mem.Length; ++i)
            {
                if (i != 0)
                    sb.Append(separator);

                sb.Append(mem[i].ToString(System.Globalization.CultureInfo.InvariantCulture));
            } // Next i 

            retValue = sb.ToString();
            sb.Clear();
            sb = null;

            return retValue;
        } // End Function Join 


    } // End Class ArrayExtensions 


}
