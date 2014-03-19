using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace LOLVideoShow.Class
{
    public class DataCache
    {
        public static int CacheTime = 86400 * 1000; //缓存一天

        public static T GetCache<T>(string key, Boolean useCache = false)
        {
            int now = CommonTools.ConvertDateTimeInt(DateTime.Now);
            int CacheLimit = IsolatedStorageHelper.GetObject<int>(key + "_t");
            if (useCache || ((now - CacheLimit) < CacheTime))
            {
                return IsolatedStorageHelper.GetObject<T>(key);
            }
            return default(T);
        }

        public static void SaveCache(string key, object obj)
        {
            int now = CommonTools.ConvertDateTimeInt(DateTime.Now);
            IsolatedStorageHelper.SaveObject(key + "_t", now);
            IsolatedStorageHelper.SaveObject(key, obj);
        }
    }
}
