using System.Collections.Generic;

namespace NetCore.WebApi
{
    public class CorsOptions
    {
        public List<string> Origins
        {
            get;
            set;
        }

        public List<string> ExposedHeaders
        {
            get;
            set;
        }
    }

}
