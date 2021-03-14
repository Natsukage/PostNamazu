using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PostNamazu.Models
{
    public delegate void ReceivedHttpCommandRequestEventHandler(string command);
    public delegate void ReceivedHttpWayMarksRequestEventHandler(string waymarkJson);
    public delegate void OnExceptionEventHandler(Exception ex);

    internal class HttpServer
    {
        private Thread _serverThread;
        private HttpListener _listener;
        SynchronizationContext ui = SynchronizationContext.Current;
        public int Port { get; private set; }

        public event ReceivedHttpCommandRequestEventHandler ReceivedCommandRequest;
        public event ReceivedHttpWayMarksRequestEventHandler ReceivedWayMarksRequest;
        public event OnExceptionEventHandler OnException;

        /// <summary>
        ///     在指定端口启动监听
        /// </summary>
        /// <param name="port">要启动的端口</param>
        public HttpServer(int port) {
            Initialize(port);
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
                                switch (context.Request.Url.AbsolutePath.ToLower()) {
                                    case @"/command":
                                    case @"/command/":
                                        DoTextCommand(ref context);
                                        break;
                                    case @"/place":
                                    case @"/place/":
                                        DoWayMarks(ref context);
                                        break;
                                    case @"/mark":
                                    case @"/mark/":
                                        break;
                                    default:
                                        throw new Exception(@"不支持的操作（目前仅支持/command /place）");
                                }
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

        private void DoTextCommand(ref HttpListenerContext context) {
            var command = new StreamReader(context.Request.InputStream, Encoding.UTF8).ReadToEnd();

            ReceivedCommandRequest?.Invoke(command);

            var buf = Encoding.UTF8.GetBytes(command);
            context.Response.ContentLength64 = buf.Length;
            context.Response.OutputStream.Write(buf, 0, buf.Length);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.OutputStream.Flush();
        }

        private void DoWayMarks(ref HttpListenerContext context) {
            var waymarkJson = new StreamReader(context.Request.InputStream, Encoding.UTF8).ReadToEnd();

            ReceivedWayMarksRequest?.Invoke(waymarkJson);

            var buf = Encoding.UTF8.GetBytes("OK!");
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
