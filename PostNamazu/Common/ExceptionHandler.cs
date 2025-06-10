using System;
using System.Windows.Forms;
using PostNamazu.Common.Localization;

namespace PostNamazu.Common
{
    /// <summary>
    /// 异常处理管理器
    /// </summary>
    public static class ExceptionHandler
    {
        /// <summary>
        /// 处理HTTP服务器异常
        /// </summary>
        public static void HandleHttpServerException(Exception ex, int port, PostNamazuUi ui, Action enableStartButton, Action disableStopButton)
        {
            var errorMessage = L.Get("PostNamazu/httpException", port, ex.Message);

            enableStartButton?.Invoke();
            disableStopButton?.Invoke();

            ui?.Log(errorMessage);
            MessageBox.Show(errorMessage);
        }

        /// <summary>
        /// 处理进程注入异常
        /// </summary>
        public static void HandleProcessInjectionException(Exception ex, PostNamazuUi ui)
        {
            ui?.Log(L.Get("PostNamazu/xivProcInjectException", ex.Message + " \n" + ex.StackTrace));
        }

        /// <summary>
        /// 处理动作执行异常
        /// </summary>
        public static void HandleActionExecutionException(Exception ex, string command, PostNamazuUi ui)
        {
            ui?.Log(L.Get("PostNamazu/doActionFail", command, ex.Message + "\n" + ex.StackTrace));
        }

        /// <summary>
        /// 处理模组初始化异常
        /// </summary>
        public static void HandleModuleInitializationException(Exception ex, string moduleName, PostNamazuUi ui)
        {
            ui?.Log(L.Get("PostNamazu/getOffsetsFail", moduleName, ex.Message + " \n" + ex.StackTrace));
        }

        /// <summary>
        /// 处理区域检测异常
        /// </summary>
        public static void HandleRegionDetectionException(Exception ex, PostNamazuUi ui)
        {
            ui?.Log(L.Get("PostNamazu/getRegionMemoryFail", ex.Message));
        }

        /// <summary>
        /// 处理内存读取异常
        /// </summary>
        public static void HandleMemoryReadException()
        {
            // 重构修正：初始化时内存读取失败是正常情况，不应该抛出异常
            // 仅记录调试信息，避免在用户界面显示错误
#if DEBUG
            System.Diagnostics.Debug.WriteLine(L.Get("PostNamazu/readMemoryFail"));
#endif
        }
    }
} 