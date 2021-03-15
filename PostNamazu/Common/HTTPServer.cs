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
        SynchronizationContext ui = SynchronizationContext.Current;
        public int Port { get; private set; }

        public event ReceivedRequestEventHandler ReceivedCommandRequest;
        public event ReceivedRequestEventHandler ReceivedWayMarksRequest;
        public event ReceivedRequestEventHandler ReceivedSendKeyRequest;
        public event ReceivedRequestEventHandler ReceivedMarkingRequest;
        public event OnExceptionEventHandler OnException;

        private delegate void URLAction(ref HttpListenerContext context);
        private delegate void ReceivedEventHandler(string marking);
        private Dictionary<string, URLAction> UrlBind = new Dictionary<string, URLAction>();


        /// <summary>
        ///     在指定端口启动监听
        /// </summary>
        /// <param name="port">要启动的端口</param>
        public HttpServer(int port) {
            Initialize(port);
            URLBind();
        }

        /// <summary>
        ///     在随机端口启动监听
        /// </summary>
        public HttpServer() {
            //get an empty port
            var l = new TcpListener(IPAddress.Loopback, 0);
            l.Start();
            var port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            Initialize(port);
        }

        /// <summary>
        ///     停止监听并释放资源
        /// </summary>
        public void Stop() {
            _serverThread.Abort();
            _listener.Stop();
        }

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
                            try {
                                GetAction(context.Request.Url.AbsolutePath)(ref context);
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

        private void URLBind() {
            SetAction("command", delegate (ref HttpListenerContext context) { DoAction(ref context, ReceivedCommandRequest); });
            SetAction("place", delegate (ref HttpListenerContext context) { DoAction(ref context, ReceivedWayMarksRequest); });
            SetAction("sendkey", delegate (ref HttpListenerContext context) { DoAction(ref context, ReceivedSendKeyRequest); });
            SetAction("mark", delegate (ref HttpListenerContext context) { DoAction(ref context, ReceivedMarkingRequest); });
        }

        private void SetAction(string url, URLAction uRLAction) {
            url = url.Trim(new char[] { '/' }).ToLower();
            UrlBind.Add(url, uRLAction);
        }

        private URLAction GetAction(string url) {
            try {
                url = url.Trim(new char[] { '/' }).ToLower();
                return UrlBind[url];

            }
            catch {
                throw new Exception($@"不支持的操作{url}");
            }
        }

        private void DoAction(ref HttpListenerContext context, ReceivedRequestEventHandler e) {

            var command = new StreamReader(context.Request.InputStream, Encoding.UTF8).ReadToEnd();

            e?.Invoke(command);

            var buf = Encoding.UTF8.GetBytes(command);
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf, 0, buf.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
        }

        private void Initialize(int port) {
            Port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
        }
    }
}
