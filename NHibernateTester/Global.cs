using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernateTester
{
    public class Global
    {
        private static ITestDAO m_InstanceDAO;
        public static ITestDAO InstanceDAO
        {
            get
            {
                if (m_InstanceDAO == null)
                {
                    m_InstanceDAO = new TestDAO();
                }

                return m_InstanceDAO;
            }
        }

        private Global()
        { }
    }
}
