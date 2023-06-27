# C#でSDKを作成する手順

## 概要
Visual Studio2022を利用して、別のアプリケーションから利用可能なSDKを作成する。  
まず、SDK用のプロジェクトを作成し、そこでDLLを作成する。  
そのDLLを、SDKを利用するアプリケーションが参照することにより、利用可能になる。

今回作成するプログラムは以下の通り
-  SampleSDK
   - 今回の目的のSDKそのもの。クラスライブラリプロジェクトとして作成する
-  UseSampleSDKWinForms/UseSampleSDKWPF
   - SDKを利用するアプリケーションのサンプル。Windowsフォームアプリと、WPFアプリケーションの2種類のプロジェクトを用意

## 注意点
利用する.NETのバージョンに注意する必要がある。  
SDKは、SDKを利用するバージョンと同じか、それ以下でなければ動作しないので注意。  
（例えば、SDKだけ最新バージョンにして、SDKを利用する側は古いバージョンにすると動作しない）  

今回は、.NET Frameworkを除いた、現在サポートされている一番古い.NET 6を利用することにした。

# 1.SDKの作成
Windowsのスタートメニューから、「Visual Studio 2022」をクリックして起動させる。  
起動すると、画面右側の「開始する」という項目の一番下に「新しいプロジェクトの作成」が表示されるので、それをクリックする。  

画面上部真ん中の「テンプレート検索」で、「クラス ライブラリ」と検索する（※クラスの後に半角スペースを入れること）。  
以下のように表示されるので、クラス ライブラリの、「.NETまたは.NET Standardを対象とするクラス ライブラリを作成するためのプロジェクト」とかかれた項目をクリックして選択し、画面右下の「次へ」をクリックする。
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/6a7bd134-b57c-4c75-9b6c-1a65d6108ec4)

プロジェクト名に「SampleSDK」と入力し、「次へ」をクリックする。
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/b4331335-c020-44e4-8e68-1c7c7f9af8ed)

フレームワーク画面が表示されるので、「.NET 6.0（長期的なサポート）」を選択し、「作成」をクリックする。
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/547f90d0-a68b-4ea0-a3ce-4ee3a1caa440)

エディタ画面が表示される。既に「Class1.cs」ファイルが存在しているので、画面右側のソリューションエクスプローラーから該当ファイルを右クリックし、名前の変更をクリック。  
「LogWriter.cs」に変更する。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/aa3dfb6c-3f0e-4207-846f-33390fce33dc)

以下のメッセージが表示されたら、そのまま「はい」をクリックする。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/0b2ef814-1e4e-43eb-8153-1ac48e6ad6d3)

LogWriter.csの中身を以下のコードで置き換える。
```
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
```

次に、クラスライブラリのビルドを行う。  
以下の図のように「Debug」から「Release」に変更する。  
（Releaseビルドにした方が、コードの最適化などが行われるので、配布する際にはReleaseでビルドしたものを利用するのが普通。  
　尚、Releaseビルドにすると、ステップ実行などのデバッグが行えなくなるので、開発中はDebug、開発完了したらReleaseに変更して利用している）  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/ed9ae7af-e05c-4093-a9fe-e91ff24803e6)

メニューのビルド→ソリューションのビルドをクリックする。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/920d107e-45e0-4244-adad-bf71f02ba16a)

少し待つとビルドが終了する。以下のように「成功 1」と表示されていれば、ビルドが完了している。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/dc49efb6-ad83-428e-bbf0-4b2ddb305aeb)

作成されたファイルを確認する。  
ソリューションエクスプローラーの「SampleSDK」を右クリックし、「エクスプローラーでフォルダを開く」をクリックする。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/bece6a0c-3127-49f9-ab72-fb0938907cf4)

「SampleSDK.csproj」ファイルが保存されたフォルダが表示される。  
このフォルダ直下の「bin\Release\net6.0」フォルダに移動する。  
以下のように「SampleSDK.dll」が存在していることを確認する。  
![image](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/dab12423-9501-474f-996c-4124b1e88343)

これでSDKの作成は完了した。

# 2.SDKの利用





