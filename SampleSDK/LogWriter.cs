using System.Text;

namespace SampleSDK
{
    public class LogWriter
    {
        private enum LogLevel
        {
            Info,
            Error
        }

        /// <summary>
        /// [Info]ログ出力
        /// </summary>
        /// <param name="message"></param>
        public static void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        /// <summary>
        /// [Error]ログ出力
        /// </summary>
        /// <param name="message"></param>
        public static void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        /// <summary>
        /// ログ出力（デスクトップに「SampleSDK.log」というファイル名で出力する）
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="message"></param>
        private static void Write(LogLevel logLevel, string message)
        {
            // デスクトップパス取得
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // デスクトップパスとログファイル名を結合し、「C:\Users\UserName\Desktop\SampleSDK.log」のようなログファイルパスを取得
            var logFilePath = Path.Combine(desktopPath, "SampleSDK.log");

            // ログファイルに追記
            using (var sw = new StreamWriter(logFilePath, true, Encoding.UTF8))
            {
                // 「2023/06/27 11:09:00 [Error] メッセージ内容」という形式になるように
                sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}\t[{logLevel.ToString().PadRight(5)}]\t{message}");
            }
        }
    }
}