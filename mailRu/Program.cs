using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace mailRu
{
    internal static class Program
    {
        private static readonly object _lock = new object();
        private static readonly string logFilePath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app.log");

        /// <summary>
        /// Запись информационного лога в файл
        /// </summary>
        public static void LogInfo(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] INFO: {message}{Environment.NewLine}";
            WriteLog(logEntry);
        }

        /// <summary>
        /// Запись ошибки в файл и вывод MessageBox
        /// </summary>
        public static void LogError(Exception ex)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] ERROR: {ex}\n";
            WriteLog(logEntry);
            MessageBox.Show(ex.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void WriteLog(string text)
        {
            lock (_lock)
            {
                File.AppendAllText(logFilePath, text, Encoding.UTF8);
            }
        }

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    LogError(ex);
                }
                else
                {
                    LogInfo("Неизвестное исключение");
                }
            };

            try
            {
                LogInfo("Приложение запущено");
                Application.Run(new Form1());
                LogInfo("Приложение завершено");
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }
    }
}
