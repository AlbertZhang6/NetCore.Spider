namespace NetCore.WebApi
{
    public class WebApiOptions
    {
        public bool DisableJwtAuthentication
        {
            get;
            set;
        }

        public bool DisableHeaderSession
        {
            get;
            set;
        }

        public bool DisableCORS
        {
            get;
            set;
        }

        public CorsOptions CORS
        {
            get;
            set;
        }
    }
}
