#if IsWeb
using System;
using System.Web;
using FS.Utils.Common;

namespace FS.Utils.Common
{
    /// <summary>
    ///     �Ի���������з�װ
    /// </summary>
    public abstract class WebCache
    {
        private static readonly System.Web.Caching.Cache _webCache = HttpRuntime.Cache;

        /// <summary>
        ///     ��Ӷ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeOut">Ĭ�ϻ�������Ϊ1440����(24Сʱ)��λ��������</param>
        public static void Add<T>(string key, T t, int timeOut = 1440)
        {
            if (!string.IsNullOrEmpty(key) && t != null)
            {
                _webCache.Insert(key, t, null, DateTime.Now.AddMinutes(timeOut), System.Web.Caching.Cache.NoSlidingExpiration);
            }
        }

        /// <summary>
        ///     ���ض���
        /// </summary>
        public static T Get<T>(string key)
        {
            return string.IsNullOrEmpty(key) ? default(T) : ConvertHelper.ConvertType<T>(_webCache.Get(key));
        }

        /// <summary>
        ///     ���ض���
        /// </summary>
        public static object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        ///     ɾ������
        /// </summary>
        public static void Clear(string key)
        {
            if (string.IsNullOrEmpty(key)) { return; }
            _webCache.Remove(key);
        }
    }
}
#endif