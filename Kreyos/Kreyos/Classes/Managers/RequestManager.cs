using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreyos.Classes.Managers
{

    using Kreyos.Classes.Extensions;

    class RequestManager
    {
        /****************************************************************
        * Singleton
        **/
        private static RequestManager m_instance = null;

        public static RequestManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new RequestManager();
                }

                return m_instance;
            }
        }
  
    }
}
