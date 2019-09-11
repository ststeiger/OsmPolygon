
// Requires assembly Microsoft.Extensions.Configuration.Binder 
using Microsoft.Extensions.Configuration; // For config IConfiguration.Get


namespace OsmPolygon
{


    public class DataBase
    {
        public string Type { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public bool? IntegratedSecurity { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string ConnectionString { get; set; }
        public System.Data.Common.DbProviderFactory Factory { get; set; }
    }


}
