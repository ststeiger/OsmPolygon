
namespace OsmPolygon
{


    public class ProxyHelper
    {
        

        public static string[] GetProxyArray()
        {
            string htmlFile = @"d:\proxyList.htm";
            string jsonFile = @"d:\proxyList.json";

            if (!System.IO.File.Exists(jsonFile))
                ProxyHelper.GetProxyList(htmlFile, jsonFile);


            if (!System.IO.File.Exists(jsonFile))
                return null;


            string jsonTable = System.IO.File.ReadAllText(jsonFile, System.Text.Encoding.UTF8);
            return GetProxyArray(jsonTable);
        } // End Function GetProxyArray 


        public static string[] GetProxyArray(string json)
        {
            string[] filterCountries = new string[] {
                 "Switzerland"
                ,"Germany"
                ,"Netherlands"
                ,"Singapore"
                /*
                ,"United Kingdom"
                ,"United States"
                ,"Japan"
                ,"Canada"
                ,"France"
                ,"Italy"
                ,"Australia"
                ,"Spain"
                ,"Ukraine"
                ,"Thailand"
                ,"Argentina"
                */
            };

            string allowedCountries = "'" + string.Join("', '", filterCountries) + "'";
            
            System.Data.DataTable proxyList = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Data.DataTable>(json);
            System.Data.DataRow[] rows = proxyList.Select("Anonymity <> 'transparent' AND Country IN ( " + allowedCountries + ") ");

            System.Collections.Generic.List<string> ls = new System.Collections.Generic.List<string>();

            System.Data.DataTable proxyList2 = proxyList.Clone();
            foreach (System.Data.DataRow row in rows)
            {
                // proxyList2.ImportRow(row);
                string address = System.Convert.ToString(row["IP Address"]);
                string port = System.Convert.ToString(row["Port"]);

                ls.Add(address + ":" + port);
            }


            string proxys = "string[] proxyList = new string[]{ \r\n    \"" + string.Join("\"\r\n    ,\"", ls.ToArray()) + "\" \r\n }; ";
            // System.Console.WriteLine(proxys);

            return ls.ToArray();
        } // End Function GetProxyArray 


        public static void GetProxyList(string htmlFile, string jsonFile)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            string html = null;
            if (System.IO.File.Exists(htmlFile))
                html = System.IO.File.ReadAllText(htmlFile);

            if (html == null)
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    html = wc.DownloadString("https://free-proxy-list.net/");
                    System.IO.File.WriteAllText(htmlFile, html, System.Text.Encoding.UTF8);
                } // End Using wc 

            } // End if (html == null) 


            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            string selector = "//table[@id='proxylisttable']";
            
            HtmlAgilityPack.HtmlNode tableNode = doc.DocumentNode.SelectSingleNode(selector);
            System.Console.WriteLine(tableNode);

            if (tableNode == null)
            {
                System.Console.WriteLine("GetProxyList: TableNode is NULL. \r\nFailed to fetch proxy list.");
                return;
            }
                

            HtmlAgilityPack.HtmlNodeCollection ths = tableNode.SelectNodes("./thead/tr/th");
            foreach (HtmlAgilityPack.HtmlNode th in ths)
            {
                dt.Columns.Add(th.InnerText, typeof(string));
            } // Next th 


            HtmlAgilityPack.HtmlNodeCollection trs = tableNode.SelectNodes("./tbody/tr");
            foreach (HtmlAgilityPack.HtmlNode tr in trs)
            {
                System.Data.DataRow dr = dt.NewRow();

                int i = 0;
                HtmlAgilityPack.HtmlNodeCollection tds = tr.SelectNodes("./td");
                foreach (HtmlAgilityPack.HtmlNode td in tds)
                {
                    // System.Console.WriteLine(td); 
                    dr[i] = td.InnerText; 
                    ++i; 
                } // Next td 

                dt.Rows.Add(dr);
            } // Next tr 
            
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented); 
            System.IO.File.WriteAllText(jsonFile, json, System.Text.Encoding.UTF8); 
        } // End Function GetProxyList


    } // End Class ProxyHelper 


} // End Namespace GeoApis 
