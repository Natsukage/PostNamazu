using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PostNamazu.Common
{
    public delegate void OnExceptionEventHandler(Exception ex);
    
    internal class HttpServer
    {
        private Thread _serverThread;
        private HttpListener _listener;

        public int Port { get; private set; }

        public Action<string, string> PostNamazuDelegate = null;
        public event OnExceptionEventHandler OnException;

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

            PostNamazuDelegate?.Invoke(TrimUrl(context.Request.Url.AbsolutePath), payload);

            var buf = Encoding.UTF8.GetBytes(payload);
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf, 0, buf.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
        }
        public string TrimUrl(string url) {
            return url.Trim(new char[] { '/' });
        }


    }
}
