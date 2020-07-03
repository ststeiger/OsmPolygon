
namespace OsmPolygon
{


    public class WebClientWithCustomTimeout
        : System.Net.WebClient
    {

        protected int m_timeOut;


        public WebClientWithCustomTimeout()
            : this(5)
        { }


        public WebClientWithCustomTimeout(int timeOut)
            : base()
        {
            this.m_timeOut = timeOut;
        }


        protected override System.Net.WebRequest GetWebRequest(System.Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = this.m_timeOut * 1000;
            return w;
        }
    }


}
