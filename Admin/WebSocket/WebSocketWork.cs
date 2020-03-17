using System;
using System.Collections.Generic;
using System.Text;

namespace Admin
{

    using HzySocket.WebSocket;
    using HzySocket.WebSocket.Class;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Entitys.SysClass;
    using System.Threading.Tasks;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using System.Timers;
    using Toolkit;

    public class WebSocketWork : SocketService<WebSocketMsgModel>
    {
        private static Timer timer;
        public static StringBuilder OldMessages { get; set; } = new StringBuilder();

        public WebSocketWork()
        {
            //不活跃连接关闭 回调
            InactiveCloseCallBack = (_WebSocketMsgModel) =>
            {
                Tools.Log.Write($"InactiveCloseCallBack.Close.SessionKey：{_WebSocketMsgModel.SessionKey},UserID=" + _WebSocketMsgModel.UserID);
            };

            //连接主动关闭 回调
            ConnectCloseCallBack = (_WebSocketMsgModel) =>
            {
                Tools.Log.Write($"ConnectCloseCallBack.Close.SessionKey：{_WebSocketMsgModel.SessionKey},UserID=" + _WebSocketMsgModel.UserID);
            };

            //发送异常 回调
            SendExceptionCall = (ex) =>
            {
                Tools.Log.Write($"SendExceptionCall：{ex.Message}", ex);
            };

            //连接成功回调
            ConnectCall = (_WebSocketMsgModel) =>
            {
                Tools.Log.Write($"ConnectCall：{_WebSocketMsgModel.SessionKey},UserID={_WebSocketMsgModel.UserID}");
            };

            //更新 session 回调
            UpdateAppSessionCall = (_WebSocketMsgModel) =>
            {
                Tools.Log.Write($"UpdateAppSessionCall：{_WebSocketMsgModel.SessionKey},UserID={_WebSocketMsgModel.UserID}");
            };
        }

        public static void RegisterService(IServiceCollection services)
        {
            services.AddTransient<WebSocketWork>();
        }

        public static void Register(IApplicationBuilder _IApplicationBuilder)
        {
            //webScoket
            _IApplicationBuilder.UseWebSockets(new WebSocketOptions()
            {
                ReceiveBufferSize = 1024 * 100,
                KeepAliveInterval = TimeSpan.FromSeconds(60)//小时
            });
            _IApplicationBuilder.UseMiddleware<WebSocketWork>();
            //定时监听
            timer = new Timer();
            timer.Interval = 1000 * 5;
            timer.Start();
            timer.Elapsed += async (sender, e) =>
            {
                try
                {
                    await CheckInactiveSessionAsync();
                }
                catch (Exception ex) { Tools.Log.Write($"CheckInactiveSessionAsync => {ex.Message}", ex); }
            };
        }

        /// <summary>
        /// 接收前端发过来的指令
        /// </summary>
        /// <param name="appSession"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public override async Task ExecuteCommand(WebSocketMsgModel appSession, string Content)
        {
            WebSocketMsgModel msg = JsonConvert.DeserializeObject<WebSocketMsgModel>(Content);

            try
            {
                //注册
                if (msg.Action == WebSocketActionEnum.Register)
                {
                    await appSession.SendAsync(new
                    {
                        status = 1,
                        action = "register",
                        msg = $"{DateTime.Now}/{msg.UserID}/>>WebSocket注册成功!\n\n"
                    });

                    await SendAllAsync(new
                    {
                        status = 1,
                        action = "init",
                        msg = $"{DateTime.Now}/{msg.UserID}/>>欢迎加入!\n\n"
                    });
                }

                //发送所有
                if (msg.Action == WebSocketActionEnum.SendAll)
                {
                    var _msg = $"{DateTime.Now}/{msg.UserID}/>>{msg.Message}\n\n";
                    if (OldMessages.Length > 1024 * 3) OldMessages.Clear();
                    OldMessages.Append(_msg);
                    await SendAllAsync(new
                    {
                        status = 1,
                        action = "message",
                        msg = _msg
                    });
                }

            }
            catch (Exception ex)
            {
                //接收到 非 json 字符串
                await appSession.SendAsync(new { status = 1, msg = $"程序异常:{ex.Message}，接收到您的字符串:{Content} 请尽量使用 Json 字符串 与服务器交互!" });
            }

        }

        /// <summary>
        /// 根据条件筛选 Session 对象
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<WebSocketMsgModel> GetAppSessions(Func<WebSocketMsgModel, bool> predicate)
        {
            return WebSocketSend<WebSocketMsgModel>.GetAppSessions(predicate);
        }

        /// <summary>
        /// 获取 所有 session 对象
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<WebSocketMsgModel> GetAllAppSessions()
        {
            return WebSocketSend<WebSocketMsgModel>.GetAllAppSessions();
        }

        /// <summary>
        /// 插入Session 对象
        /// </summary>
        /// <param name="appSession"></param>
        public static void AddAppSession(WebSocketMsgModel appSession)
        {
            WebSocketSend<WebSocketMsgModel>.AddAppSession(appSession);
        }

        /// <summary>
        /// 移除Session 对象
        /// </summary>
        /// <param name="predicate"></param>
        public static void RemoveSession(Func<WebSocketMsgModel, bool> predicate)
        {
            WebSocketSend<WebSocketMsgModel>.RemoveSession(predicate);
        }

        /// <summary>
        /// 获取 session 连接数
        /// </summary>
        /// <returns></returns>
        public static int GetAppSessionCount()
        {
            return WebSocketSend<WebSocketMsgModel>.GetAppSessionCount();
        }

        /// <summary>
        /// 检查 session 是否存在
        /// </summary>
        /// <returns></returns>
        public static bool Any(Func<WebSocketMsgModel, bool> predicate)
        {
            return WebSocketSend<WebSocketMsgModel>.Any(predicate);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="Data"></param>
        /// <param name="Delay">延迟毫秒数</param>
        public static async Task SendAsync<T>(Func<WebSocketMsgModel, bool> predicate, T Data, int Delay = 0)
        {
            try
            {
                await WebSocketSend<WebSocketMsgModel>.SendAsync(predicate, Data, Delay);
            }
            catch (Exception ex)
            {
                Tools.Log.Write($"WebSocketWork.SendAsync.Error.{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 所有连接用户发送消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Data"></param>
        /// <param name="Delay">延迟毫秒数</param>
        public static async Task SendAllAsync<T>(T Data, int Delay = 0)
        {
            await WebSocketSend<WebSocketMsgModel>.SendAllAsync(Data, Delay);
        }


    }

    /// <summary>
    /// WebSocket 消息 模型
    /// </summary>
    public class WebSocketMsgModel : WebSocketAppSession
    {
        public string UserID { get; set; }

        public string ToUserID { get; set; }

        public WebSocketActionEnum Action { get; set; }

        public string Message { get; set; }

    }

    public enum WebSocketActionEnum
    {
        Register,
        SendAll
    }


}
