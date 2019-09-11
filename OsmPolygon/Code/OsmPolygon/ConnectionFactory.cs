
using Microsoft.Extensions.Configuration;

namespace OsmPolygon
{


    public interface IConnectionFactory
    {
        bool CheckConnection { get; }
        string ConnectionString { get; }
        string ConnectionStringWithoutPassword { get; }

        System.Data.Common.DbConnection Connection { get; }
        System.Data.Common.DbConnection ClosedConnection { get; }

    } // End interface IConnectionFactory 


    public class ConnectionFactory
        : IConnectionFactory
    {
        protected string m_cs;
        protected System.Data.Common.DbProviderFactory m_factory;

        protected Des3EncryptionService m_encryption;
        protected DataBase m_db;


        private static System.Data.Common.DbProviderFactory GetFactory(System.Type type)
        {
            if (type != null && System.Reflection.IntrospectionExtensions.GetTypeInfo(type).IsSubclassOf(typeof(System.Data.Common.DbProviderFactory)))
            {
                // Provider factories are singletons with Instance field having
                // the sole instance


                System.Reflection.FieldInfo field = System.Reflection.IntrospectionExtensions.GetTypeInfo(type).GetField("Instance"
                    , System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                );

                if (field != null)
                {
                    return (System.Data.Common.DbProviderFactory)field.GetValue(null);
                    //return field.GetValue(null) as DbProviderFactory;
                } // End if (field != null)

            } // End if (type != null && type.IsSubclassOf(typeof(System.Data.Common.DbProviderFactory)))

            return null;
        } // End Function GetFactory


        private static System.Data.Common.DbProviderFactory GetFactory(string assemblyQualifiedName)
        {
            try
            {

                if (
                       "PG".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                    || "Npgsql".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                )
                    return Npgsql.NpgsqlFactory.Instance;

                if ("MySql".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                    || "MySql.Data.MySqlClient".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                    )
                    return MySql.Data.MySqlClient.MySqlClientFactory.Instance;

                if ("MS".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                    || "System.Data.SqlClient".Equals(assemblyQualifiedName, System.StringComparison.InvariantCultureIgnoreCase)
                    )
                    return System.Data.SqlClient.SqlClientFactory.Instance;


                if (!string.IsNullOrWhiteSpace(assemblyQualifiedName))
                {
                    System.Data.Common.DbProviderFactory fac = null;

                    try
                    {
                        System.Type type = System.Type.GetType(assemblyQualifiedName, false, true);
                        fac = GetFactory(type);
                    }
                    catch (System.Exception)
                    { }


                    if (fac != null)
                        return fac;
                }

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }

            return System.Data.SqlClient.SqlClientFactory.Instance;
        }


        private static string GetCS(DataBase db, System.Data.Common.DbProviderFactory df)
        {
            if (object.ReferenceEquals(df.GetType(), typeof(System.Data.SqlClient.SqlClientFactory)))
            {
                System.Data.SqlClient.SqlConnectionStringBuilder csb = new System.Data.SqlClient.SqlConnectionStringBuilder();

                csb.DataSource = db.Server;
                if (db.Port.HasValue && db.Port.Value > 0)
                    csb.DataSource += "," + db.Port.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);

                csb.InitialCatalog = db.Database;

                if (db.IntegratedSecurity.HasValue)
                    csb.IntegratedSecurity = db.IntegratedSecurity.Value;
                else
                    csb.IntegratedSecurity = false;

                if (!csb.IntegratedSecurity)
                {
                    csb.UserID = db.Username;

                    try
                    {
                        csb.Password = new Des3EncryptionService().DeCrypt(db.Password);
                    }
                    catch (System.Exception)
                    {
                        // Password is unencrypted ... 
                        csb.Password = db.Password;
                    }

                } // End if (!csb.IntegratedSecurity) 

                csb.PacketSize = 4096;
                csb.PersistSecurityInfo = false;
                csb.ApplicationName = "LDAP-Service";
                csb.MultipleActiveResultSets = false;
                csb.WorkstationID = System.Environment.MachineName;

                return csb.ConnectionString;
            } // End if (object.ReferenceEquals(this.m_db.ProviderFactory.GetType(), typeof(System.Data.SqlClient.SqlClientFactory))) 


            if (object.ReferenceEquals(df.GetType(), typeof(Npgsql.NpgsqlFactory)))
            {
                Npgsql.NpgsqlConnectionStringBuilder csb = new Npgsql.NpgsqlConnectionStringBuilder();


                csb.Host = db.Server;

                if (db.Port.HasValue && db.Port.Value > 0)
                    csb.Port = db.Port.Value;
                else
                    csb.Port = 5432;

                csb.Database = db.Database;


                if (db.IntegratedSecurity.HasValue)
                    csb.IntegratedSecurity = db.IntegratedSecurity.Value;
                else
                    csb.IntegratedSecurity = false;


                if (!csb.IntegratedSecurity)
                {
                    csb.Username = db.Username;

                    try
                    {
                        csb.Password = new Des3EncryptionService().DeCrypt(db.Password);
                    }
                    catch (System.Exception)
                    {
                        // Password is unencrypted ... 
                        csb.Password = db.Password;
                    }

                } // End if (!csb.IntegratedSecurity) 

                csb.PersistSecurityInfo = false;
                csb.ApplicationName = "LDAP-Service";
                csb.CommandTimeout = 30;
                csb.Timeout = 15;

                return csb.ConnectionString;
            } // End if (object.ReferenceEquals(this.m_db.ProviderFactory.GetType(), typeof(Npgsql.NpgsqlFactory))) 


            if (object.ReferenceEquals(df.GetType(), typeof(MySql.Data.MySqlClient.MySqlClientFactory)))
            {
                MySql.Data.MySqlClient.MySqlConnectionStringBuilder csb =
                    new MySql.Data.MySqlClient.MySqlConnectionStringBuilder();

                csb.Server = db.Server;

                if (db.Port.HasValue && db.Port.Value > 0)
                    csb.Port = (uint)db.Port.Value;
                else
                    csb.Port = 3306u;

                csb.Database = db.Database;

                csb.PersistSecurityInfo = false;
                csb.ApplicationName = "LDAP-Service";
                csb.DefaultCommandTimeout = 30;
                csb.ConnectionTimeout = 15;

                return csb.ConnectionString;
            }


            System.Data.Common.DbConnectionStringBuilder dbcsb = df.CreateConnectionStringBuilder();

            dbcsb.Add("Server", db.Server);


            if (db.Port.HasValue && db.Port.Value > 0)
                dbcsb.Add("Port", db.Port);

            dbcsb.Add("Database", db.Database);

            if (db.IntegratedSecurity.HasValue)
                dbcsb.Add("IntegratedSecurity", db.IntegratedSecurity);
            else
                dbcsb.Add("IntegratedSecurity", false);


            if (db.IntegratedSecurity.HasValue && !db.IntegratedSecurity.Value)
            {
                dbcsb.Add("User Id", db.Username);

                try
                {
                    dbcsb.Add("Password", new Des3EncryptionService().DeCrypt(db.Password));
                }
                catch (System.Exception)
                {
                    // Password is unencrypted ... 
                    dbcsb.Add("Password", db.Password);
                }

            } // End if (!csb.IntegratedSecurity) 

            return dbcsb.ConnectionString;
        }


        public ConnectionFactory(DataBase db
            , Des3EncryptionService encryption
        )
        {
            try
            {
                if (db.Factory != null)
                    this.m_factory = db.Factory;
                else
                    this.m_factory = GetFactory(db.Type);

                if (string.IsNullOrEmpty(db.ConnectionString))
                    this.m_cs = GetCS(db, this.m_factory);
                else
                    this.m_cs = db.ConnectionString;

                this.m_encryption = encryption;
                this.m_db = db;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                throw;
            }


        } // End Constructor 


        public ConnectionFactory(Microsoft.Extensions.Configuration.IConfiguration config
            , Des3EncryptionService encryption
        )
            : this(config.GetSection("DB").Get<DataBase>(), encryption)
        { } // End Constructor 


        public ConnectionFactory(string connectionString, System.Data.Common.DbProviderFactory factory)
            : this(
                 new DataBase() {
                     ConnectionString = connectionString,
                     Factory = factory
                 }
                , new Des3EncryptionService()
            )
        { }


        public ConnectionFactory(string connectionString, System.Type factory)
            : this(
                  new DataBase() {
                      ConnectionString = connectionString,
                      Factory = GetFactory(factory)
                  }
                  , new Des3EncryptionService()
        )
        { }


        public ConnectionFactory(string connectionString)
                : this(connectionString, typeof(System.Data.SqlClient.SqlClientFactory))
        { }


        private static string DefaultConnectionString
        {
            get
            {
                string ret = null;
                System.Data.SqlClient.SqlConnectionStringBuilder csb =
                    new System.Data.SqlClient.SqlConnectionStringBuilder();

                csb.DataSource = System.Environment.MachineName + @"\SQLEXPRESS";
                csb.InitialCatalog = "COR_Basic_Demo_V4";

                csb.IntegratedSecurity = true;
                csb.PersistSecurityInfo = false;
                csb.PacketSize = 4096;
                csb.MultipleActiveResultSets = false;
                csb.ApplicationName = "OsmPolygon";
                csb.ConnectTimeout = 15;
                csb.Pooling = true;
                csb.WorkstationID = System.Environment.MachineName;
                ret = csb.ConnectionString;

                csb = null;

                return ret;
            }
        }


        public ConnectionFactory()
            : this(DefaultConnectionString)
        { }
        

        public string ConnectionString
        {
            get
            {
                if (this.m_cs != null)
                    return this.m_cs;

                this.m_cs = "";
                return this.m_cs;
            }
        } // End Property ConnectionString 


        public string ConnectionStringWithoutPassword
        {
            get
            {
                System.Data.Common.DbConnectionStringBuilder dbc = new System.Data.Common.DbConnectionStringBuilder();
                dbc.ConnectionString = this.ConnectionString;

                if (dbc.ContainsKey("password") && !string.IsNullOrEmpty(System.Convert.ToString(dbc["Password"])))
                    dbc["Password"] = new string('*', 8);

                return dbc.ConnectionString;
            }
        } // End Property ConnectionStringWithoutPassword 


        protected System.Data.Common.DbConnection GetConnection(bool opened)
        {
            System.Data.Common.DbConnection cn = this.m_factory.CreateConnection();
            cn.ConnectionString = this.ConnectionString;

            if (opened && cn.State != System.Data.ConnectionState.Open)
                cn.Open();

            return cn;
        } // End Function GetConnection 


        public System.Data.Common.DbConnection Connection
        {
            get { return this.GetConnection(true); }
        }


        public System.Data.Common.DbConnection ClosedConnection
        {
            get { return this.GetConnection(false); }
        }


        public bool CheckConnection
        {
            get
            {
                try
                {
                    using (System.Data.Common.DbConnection connection = this.ClosedConnection)
                    {
                        if (connection.State != System.Data.ConnectionState.Open)
                            connection.Open();

                        if (connection.State != System.Data.ConnectionState.Closed)
                            connection.Close();
                    }
                }
                catch (System.Exception)
                {
                    return false;
                }

                return true;
            } // End Get 

        } // End Property CheckConnection 


    } // End Class ConnectionFactory 


} // End Namespace LdapService
