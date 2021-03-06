#if IsWeb
using System;
using System.Web;
using FS.Utils.Common;

namespace FS.Utils.Common
{
    /// <summary>
    ///     对缓存操作进行封装
    /// </summary>
    public abstract class WebCache
    {
        private static readonly System.Web.Caching.Cache _webCache = HttpRuntime.Cache;

        /// <summary>
        ///     添加对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeOut">默认缓存存活期为1440分钟(24小时)单位：／分钟</param>
        public static void Add<T>(string key, T t, int timeOut = 1440)
        {
            if (!string.IsNullOrEmpty(key) && t != null)
            {
                _webCache.Insert(key, t, null, DateTime.Now.AddMinutes(timeOut), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        ///     返回对象
        /// </summary>
        public static T Get<T>(string key)
        {
            return string.IsNullOrEmpty(key) ? default(T) : ConvertHelper.ConvertType<T>(_webCache.Get(key));
        }

        /// <summary>
        ///     返回对象
        /// </summary>
        public static object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        ///     删除对象
        /// </summary>
        public static void Clear(string key)
        {
            if (string.IsNullOrEmpty(key)) { return; }
            _webCache.Remove(key);
        }
    }
}
#endif