﻿#if IsWeb
namespace FS.Utils.Component
{
    /// <summary>
    ///     JS提示框
    /// </summary>
    public class LhgDialog
    {
        /// <summary>
        /// 对话框脚本
        /// </summary>
        public string DialogScript;

        /// <summary>
        /// Page值
        /// </summary>
        /// <param name="scriptPre"></param>
        public LhgDialog(string scriptPre = "frameElement.api.opener.$$")
        {
            DialogScript = scriptPre;
        }

        /// <summary>
        ///     Dialog弹出框
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转页面URL</param>
        public string Alert(string message, string gotoUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(gotoUrl))
            {
                gotoUrl = "location.href='" + gotoUrl + "'";
            }
            return AlertFunc(message, gotoUrl);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.alert弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="func">确定后，执行的脚本</param>
        public string AlertFunc(string message, string func = "")
        {
            return DialogScript + ".dialog.alert('" + message + "',function(){ " + func + "; }).zindex().lock();";
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（可以防止数据保持问题）
        /// </summary>
        /// <param name="message">提示内容</param>
        /// <param name="gotoUrl">跳转地址</param>
        public string Tip(string message, string gotoUrl = "")
        {
            if (!string.IsNullOrWhiteSpace(gotoUrl))
            {
                gotoUrl = "location.href='" + gotoUrl + "'";
            }
            return Tip(message, gotoUrl, 2);
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public string Tip(string message, string func, int timeout)
        {
            return DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'tips.gif' ,function(){ " + func + "; }).zindex().lock();";
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public string TipSuccess(string message = "保存成功！", string func = "frameElement.api.close();", int timeout = 1)
        {
            return DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'succ.png' ,function(){ " + func + "; }).zindex().lock();";
        }

        /// <summary>
        ///     使用frameElement.getTopLevelWindow().$$.dialog.tips弹出框架来提示内容。（带脚本运行功能）
        /// </summary>
        /// <param name="timeout">自动关闭时间</param>
        /// <param name="func">确定后，执行的脚本</param>
        /// <param name="message">提示内容</param>
        public string TipError(string message = "数据不存在！", string func = "frameElement.api.close();", int timeout = 2)
        {
            return DialogScript + ".dialog.tips('" + message + "', " + (timeout > 0 ? timeout : 3600) + ", 'fail.png' ,function(){ " + func + "; }).zindex().lock();";
        }
    }
}
#endif