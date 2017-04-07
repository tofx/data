using System;
using System.Collections.Generic;
using System.Linq;

namespace TOF.Data.Providers.SqlServer
{
    public static class ParameterUtils
    {
        private class ParameterNameComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                int ix = Convert.ToInt32(x.Substring(1));
                int iy = Convert.ToInt32(y.Substring(1));

                if (ix < iy) return -1;
                if (ix > iy) return 1;

                return 0;
            }
        }

        public static string CreateNewParameter(ref IDictionary<string, object> parameterDictionary)
        {
            if (!parameterDictionary.Any())
            {
                parameterDictionary.Add("p0", null);
                return "p0";
            }
            else
            {
                var lastItem = parameterDictionary.OrderByDescending(
                    i => i.Key, new ParameterNameComparer())
                    .First();

                int num = Convert.ToInt32(lastItem.Key.Substring(1));
                num++;
                parameterDictionary.Add("p" + num, null);
                return "p" + num;
            }
        }
    }
}
