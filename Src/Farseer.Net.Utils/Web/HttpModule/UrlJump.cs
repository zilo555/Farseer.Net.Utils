#if IsWeb
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using FS.Configs;
using FS.Extends;
using FS.Utils.Common;

namespace FS.Utils.HttpModule
{
    /// <summary>
    ///     ��תģ��ĳ������
    /// </summary>
    /// <remarks></remarks>
    public class UrlJump : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     �����¼��ܵ�
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += UrlJump_AuthorizeRequest; }

        /// <summary>
        ///     ע��
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     ִ����ת����
        /// </summary>
        protected void UrlJump_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;

            var appPath = app.Context.Request.ApplicationPath;

            // д��׷����־��Ϣ
            app.Context.Trace.Write("Url��ת", "��ʼִ�С�");

            // ѭ��������ת����
            foreach (var rule in WebUrlJumpConfigs.ConfigEntity.JumpRules)
            {
                // ȡ�ù����ַ
                var lookFor = "^" + ResolveUrl(appPath, rule.OriginalUrl) + "$";
                var re = new Regex(lookFor, RegexOptions.IgnoreCase);
                var url = lookFor.ToLower().StartsWith("^http://") ? app.Request.Url.AbsoluteUri : app.Request.Path;

                if (!re.IsMatch(url)) continue;

                // ȡ����ת�����ַ
                var sendToUrl = ResolveUrl(appPath, re.Replace(url, rule.NewUrl));
                // д��׷����־��Ϣ
                app.Context.Trace.Write("Url��ת", "��ת����" + sendToUrl);
                // ��ת��ַ
                Jump(app.Context, sendToUrl, rule.JumpCode);
                break;
            }
            app.Context.Trace.Write("Url��ת", "����ִ��");
        }

        /// <summary>
        ///     �滻����򡢲��ҽ�����·��ת������վ·��
        /// </summary>
        /// <param name="appPath">��վ��Ŀ¼.</param>
        /// <param name="url">�����ַ</param>
        protected string ResolveUrl(string appPath, string url)
        {
            //�滻�����
            url = string.Format(url, WebGeneralConfigs.ConfigEntity.RewriterDomain.ToArray("", ";"));
            if (url.Length == 0 || url[0] != '~') { return url; }
            // �������ʹ��Ŀ¼��ַ����ֱ�ӷ���ԭ��ַ
            if (url.Length == 1) { return appPath; }

            // ���ظ�Ŀ¼��ַ
            return appPath + Url.ConvertPath(url[1] == '/' ? url.Substring(2) : url.Substring(1));
        }

        /// <summary>
        ///     ��ת��ַ
        /// </summary>
        /// <param name="context">�����������</param>
        /// <param name="sendToUrl">��ת��Ŀ��URL</param>
        /// <param name="code">��תĿ��URL��ʵ������·��</param>
        protected void Jump(HttpContext context, string sendToUrl, int code)
        {
            context.Response.Clear();
            context.Response.StatusCode = code;
            context.Response.Status = code + " Jump";
            context.Response.AddHeader("Location", sendToUrl);
            context.Response.End();
        }
    }
}
#endif