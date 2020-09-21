
namespace OsmPolygon
{


    class InsensitiveReplace
    {


        // https://stackoverflow.com/questions/6025560/how-to-ignore-case-in-string-replace/13511149
        public static string Replace(string source, string oldValue, string newValue, System.StringComparison comparisonType)
        {
            if (source.Length == 0 || oldValue.Length == 0)
                return source;

            System.Text.StringBuilder result = new System.Text.StringBuilder();
            int startingPos = 0;
            int nextMatch;
            while ((nextMatch = source.IndexOf(oldValue, startingPos, comparisonType)) > -1)
            {
                result.Append(source, startingPos, nextMatch - startingPos);
                result.Append(newValue);
                startingPos = nextMatch + oldValue.Length;
            } // Whend 

            result.Append(source, startingPos, source.Length - startingPos);

            return result.ToString();
        } // End Function Replace 


    } // End Class InsensitiveReplace 


} // End Namespace OsmPolygon 