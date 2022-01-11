using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PostNamazu.Models
{
    public delegate void ReceivedRequestEventHandler(string payload);
    public delegate void OnExceptionEventHandler(Exception ex);

    internal class HttpServer
    {
        private Thread _serverThread;
        private HttpListener _listener;

        public int Port { get; private set; }

        public event OnExceptionEventHandler OnException;

        private Dictionary<string, ReceivedRequestEventHandler> UrlBind = new Dictionary<string, ReceivedRequestEventHandler>();

        #region Init
        /// <summary>
        ///     在指定端口启动监听
        /// </summary>
        /// <param name="port">要启动的端口</param>
        public HttpServer(int port)
        {
            Initialize(port);
        }

        /// <summary>
        ///     在随机端口启动监听
        /// </summary>
        public HttpServer()
        {
            //get an empty port
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            Initialize(port);
        }

        /// <summary>
        ///     初始化并启动监听
        /// </summary>
        /// <param name="port">监听的端口</param>
        private void Initialize(int port)
        {
            Port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
        }

        /// <summary>
        ///     停止监听并释放资源
        /// </summary>
        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }
        #endregion


        private void Listen() {
            try {
                _listener = new HttpListener();
                _listener.Prefixes.Add("http://*:" + Port + "/");
                _listener.Start();
            }
            catch (Exception ex) {
                OnException?.Invoke(ex);
                return;
            }

            ThreadPool.QueueUserWorkItem(o => {
                try {
                    while (_listener.IsListening)
                        ThreadPool.QueueUserWorkItem(c => {
                            if (!(c is HttpListenerContext context))
                                throw new ArgumentNullException(nameof(context));
                            try
                            {
                                DoAction(ref context);
                            }
                            catch (Exception ex) {
                                MessageBox.Show(ex.Message);
                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            }
                            finally {
                                context.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                }
                catch {
                    // ignored
                }
            });
        }

        /// <summary>
        ///     根据HTTP请求内容执行对应的指令
        /// </summary>
        /// <param name="context">HTTP请求内容</param>
        private void DoAction(ref HttpListenerContext context)
        {
            var payload = new StreamReader(context.Request.InputStream, Encoding.UTF8).ReadToEnd();

            GetAction(context.Request.Url.AbsolutePath)?.Invoke(payload);

            var buf = Encoding.UTF8.GetBytes(payload);
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf, 0, buf.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
        }

        /// <summary>
        ///     设置指令与对应的方法委托
        /// </summary>
        /// <param name="url">通过url传递的指令类型</param>
        /// <param name="action">对应指令的方法委托</param>
        public void SetAction(string url, ReceivedRequestEventHandler action) {
            url = url.Trim(new char[] { '/' }).ToLower();
            UrlBind[url]= action;
        }

        /// <summary>
        ///     获取指令对应的方法
        /// </summary>
        /// <param name="url">通过url传递的指令类型</param>
        /// <returns>对应指令的委托方法</returns>
        private ReceivedRequestEventHandler GetAction(string url) {
            try {
                url = url.Trim(new char[] { '/' }).ToLower();
                return UrlBind[url];
            }
            catch {
                throw new Exception($@"不支持的操作{url}");
            }
        }

        /// <summary>
        ///     清空绑定的委托列表
        /// </summary>
        public void ClearAction()
        {
            UrlBind.Clear();
        }


    }
}
