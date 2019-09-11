
using System.Collections.Generic;
using System.Data;

namespace Dapper
{


    public static partial class SqlMapper
    {


        private static string GetEmbeddedResource(System.Reflection.Assembly asm, string resourceName)
        {
            string resource = null;

            string foundResourceName = null;

            foreach (string thisResourceName in asm.GetManifestResourceNames())
            {
                if (thisResourceName.EndsWith(resourceName, System.StringComparison.OrdinalIgnoreCase))
                {
                    foundResourceName = thisResourceName;
                    break;
                }
            } // Next thisResourceName 

            if (foundResourceName == null)
                throw new System.IO.InvalidDataException("The provided resourceName is not present.");

            using (System.IO.Stream strm = asm.GetManifestResourceStream(foundResourceName))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(strm))
                {
                    resource = sr.ReadToEnd();
                } // End Using sr 

            } // End Using strm 

            return resource;
        } // End Function GetEmbeddedResource 


        private static string GetEmbeddedResource(System.Type type, string resourceName)
        {
            System.Reflection.Assembly asm = System.Reflection.IntrospectionExtensions
                .GetTypeInfo(type).Assembly;

            return GetEmbeddedResource(asm, resourceName);
        } // End Function GetEmbeddedResource 



        public static int ExecuteFromResource(this System.Data.IDbConnection cnn, string resourceName
            , System.Reflection.Assembly asm, object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            string sql = GetEmbeddedResource(asm, resourceName);
            return Execute(cnn, sql, param, transaction, commandTimeout, commandType);
        } // End Function ExecuteFromResource 


        public static int ExecuteFromResource(this System.Data.IDbConnection cnn, string resourceName
            , System.Type type, object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            string sql = GetEmbeddedResource(type.Assembly, resourceName);
            return Execute(cnn, sql, param, transaction, commandTimeout, commandType);
        } // End Function ExecuteFromResource 


        public static int ExecuteFromResource(this System.Data.IDbConnection cnn, string resourceName
            , object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            return ExecuteFromResource(cnn, resourceName, typeof(SqlMapper), param, transaction, commandTimeout, commandType);
        } // End Function ExecuteFromResource 



        ////////////////////////////


        public static System.Collections.Generic.List<int> ExecuteGoScriptFromResource(this System.Data.IDbConnection cnn
            , string resourceName
            , System.Reflection.Assembly asm, object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            System.Collections.Generic.List<int> ls = new System.Collections.Generic.List<int>();
            string sql = GetEmbeddedResource(asm, resourceName);

            ScriptSplitter splittedScripts = new ScriptSplitter(sql);
            foreach (string thisScript in splittedScripts)
            {
                int retValue = Execute(cnn, thisScript, param, transaction, commandTimeout, commandType);
                ls.Add(retValue);
            } // Next thisScript 

            return ls;
        } // End Function ExecuteFromResource 


        public static System.Collections.Generic.List<int> ExecuteGoScriptFromResource(this System.Data.IDbConnection cnn
            , string resourceName
            , System.Type type, object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            System.Collections.Generic.List<int> ls = new System.Collections.Generic.List<int>();
            string sql = GetEmbeddedResource(type.Assembly, resourceName);

            ScriptSplitter splittedScripts = new ScriptSplitter(sql);
            foreach (string thisScript in splittedScripts)
            {
                int retValue = Execute(cnn, thisScript, param, transaction, commandTimeout, commandType);
                ls.Add(retValue);
            } // Next thisScript 

            return ls;
        } // End Function ExecuteFromResource 


        public static System.Collections.Generic.List<int> ExecuteGoScriptFromResource(this System.Data.IDbConnection cnn
            , string resourceName
            , object param = null
            , System.Data.IDbTransaction transaction = null, int? commandTimeout = null
            , System.Data.CommandType? commandType = null)
        {
            return ExecuteGoScriptFromResource(cnn, resourceName, typeof(SqlMapper), param, transaction, commandTimeout, commandType);
        } // End Function ExecuteGoScriptFromResource 




        /// <summary>
        /// Executes a query, returning the data typed as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of results to return.</typeparam>
        /// <param name="cnn">The connection to query on.</param>
        /// <param name="sql">The SQL to execute for the query.</param>
        /// <param name="param">The parameters to pass, if any.</param>
        /// <param name="transaction">The transaction to use, if any.</param>
        /// <param name="buffered">Whether to buffer results in memory.</param>
        /// <param name="commandTimeout">The command timeout (in seconds).</param>
        /// <param name="commandType">The type of command to execute.</param>
        /// <returns>
        /// A sequence of data of the supplied type; if a basic type (int, string, etc) is queried then the data from the first column in assumed, otherwise an instance is
        /// created per row, and a direct column-name===member-name mapping is assumed (case insensitive).
        /// </returns>
        public static IEnumerable<T> Query<T>(this IDbConnection cnn, string resourceName, System.Type type, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            string sql = GetEmbeddedResource(type.Assembly, resourceName);

            var command = new CommandDefinition(sql, param, transaction, commandTimeout, commandType, buffered ? CommandFlags.Buffered : CommandFlags.None);
            var data = QueryImpl<T>(cnn, command, typeof(T));
            return command.Buffered ? System.Linq.Enumerable.ToList(data) : data;
        }


    } // End Class SqlMapper 


} // End Namespace Dapper 
