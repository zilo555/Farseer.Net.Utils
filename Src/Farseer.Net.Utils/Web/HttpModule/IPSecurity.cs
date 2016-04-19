#if IsWeb
using System;
using System.Web;
using System.Web.SessionState;
using FS.Configs;
using FS.Extends;
using FS.Utils.Common;
using FS.Utils.Component;
using System.Linq;

namespace FS.Utils.HttpModule
{
    /// <summary>
    ///     ��תģ��ĳ������
    /// </summary>
    /// <remarks></remarks>
    public class IPSecurity : System.Web.IHttpModule, IRequiresSessionState
    {
        /// <summary>
        ///     �����¼��ܵ�
        /// </summary>
        public void Init(HttpApplication app) { app.AuthorizeRequest += IPSecurity_AuthorizeRequest; }

        /// <summary>
        ///     ע��
        /// </summary>
        public void Dispose() { }

        /// <summary>
        ///     ִ����ת����
        /// </summary>
        protected void IPSecurity_AuthorizeRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var clientIP = Req.GetIP();
            var config = WebIPSecurityConfigs.ConfigEntity;

            //app.Response.Write($"<!--{location.Area}|| {location.Province}|| {location.City}|| {location.Address}-->");

            // ѭ��������ת����
            if (config.IPList.Select(ip => IpAdress.IsContains(clientIP, ip)).Any(result => config.IsBlacklistMode == result)) { Jump(app.Context); return; }

            // ���ĵ�ַ�ķ�ʽ
            if (config.AddressList.Count <= 0) return;
            var location = IpAdress.GetLocation(clientIP);
            if (config.IsBlacklistMode != config.AddressList.Any(o => location.Area.Contains(o)|| location.Province.Contains(o)|| location.City.Contains(o)|| location.Address.Contains(o))) return;
            Jump(app.Context);
        }

        /// <summary>
        ///     ��ת��ַ
        /// </summary>
        /// <param name="context">�����������</param>
        protected void Jump(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = WebIPSecurityConfigs.ConfigEntity.StatusCode;
            context.Response.Status = WebIPSecurityConfigs.ConfigEntity.StatusCode + " Jump";

            // ��ת��ʽ
            if (!string.IsNullOrWhiteSpace(WebIPSecurityConfigs.ConfigEntity.JumpUrl)) { context.Response.AddHeader("Location", WebIPSecurityConfigs.ConfigEntity.JumpUrl); }
            else
            {
                // ���ַ�ʽ
                context.Response.Write(WebIPSecurityConfigs.ConfigEntity.Content);
                context.Response.End();
            }
        }
    }
}
#endif