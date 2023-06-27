# C#でSDKを作成する手順

## 概要
Visual Studio2022を利用して、別のアプリケーションから利用可能なSDKを作成する。  
まず、SDK用のプロジェクトを作成し、そこでDLLを作成する。  
そのDLLを利用するアプリケーションが参照することにより、利用可能になる。

今回作成するプログラムは以下の通り
-  SampleSDK
   - 今回の目的のSDKそのもの。「クラスライブラリ」プロジェクトとして作成する
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
![249024553-6a7bd134-b57c-4c75-9b6c-1a65d6108ec4](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/6391e432-5314-4846-80a6-df1f3d434d41)

プロジェクト名に「SampleSDK」と入力し、「次へ」をクリックする。
![249024909-b4331335-c020-44e4-8e68-1c7c7f9af8ed](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/c8500e06-cda9-4850-aa9a-a62a55323167)

フレームワーク画面が表示されるので、「.NET 6.0（長期的なサポート）」を選択し、「作成」をクリックする。
![249025263-547f90d0-a68b-4ea0-a3ce-4ee3a1caa440](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/5bb19292-3e89-45fa-9791-89e108312f64)

エディタ画面が表示される。既に「Class1.cs」ファイルが存在しているので、画面右側のソリューションエクスプローラーから該当ファイルを右クリックし、名前の変更をクリック。  
「LogWriter.cs」に変更する。  
![249025834-aa3dfb6c-3f0e-4207-846f-33390fce33dc](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/9f9b3ffc-b2fd-401e-8325-9192251a411a)

以下のメッセージが表示されたら、そのまま「はい」をクリックする。  
![249025932-0b2ef814-1e4e-43eb-8153-1ac48e6ad6d3](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/0793b19b-0f09-4143-8f56-f2e7240b04e6)

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
![249026528-ed9ae7af-e05c-4093-a9fe-e91ff24803e6](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/3d350845-5b0c-40d0-ba04-58997d8c5987)

メニューのビルド→ソリューションのビルドをクリックする。  
![249027377-920d107e-45e0-4244-adad-bf71f02ba16a](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/0862bb44-93af-4b2b-a0ec-de90d628af61)

少し待つとビルドが終了する。以下のように「成功 1」と表示されていれば、ビルドが完了している。  
![249027601-dc49efb6-ad83-428e-bbf0-4b2ddb305aeb](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/238e9227-510e-490c-aa40-2d08cc78eeae)

作成されたファイルを確認する。  
ソリューションエクスプローラーの「SampleSDK」を右クリックし、「エクスプローラーでフォルダを開く」をクリックする。  
![249027866-bece6a0c-3127-49f9-ab72-fb0938907cf4](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/7067a504-d93d-490a-a9f2-0209c4f6380b)

「SampleSDK.csproj」ファイルが保存されたフォルダが表示されるので、このフォルダ直下の「bin\Release\net6.0」フォルダに移動する。  
以下のように「SampleSDK.dll」が存在していることを確認する。  
![249028298-dab12423-9501-474f-996c-4124b1e88343](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/b5b532c9-ec54-4242-b590-1c2f1a625755)

これでSDKの作成は完了した。

# 2.SDKの利用
ここでは、先ほど作成したSDKを利用する手順について記載する。  
利用する側のアプリケーション自体の作成方法は割愛するが、プロジェクト作成時には、  
以下の図のように「Windows フォームアプリ」や「WPF アプリケーション」を選択すること。  
（.NET Frameworkと記載のあるものを選択してしまうと、.NET 6を指定することができないので注意！）  
![249028661-f9d08a3a-2ab0-4305-bb2c-276193c78539](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/223695fa-bd1d-4cf3-bb6d-c6adacb83ae6)

最初に、先ほど作成した「SampleSDK.dll」を参照設定に追加する。  
ソリューションエクスプローラーのプロジェクト名直下の「依存関係」を右クリックし、  
「プロジェクト参照の追加」をクリックする。  
![249029761-eea44333-8bc0-4138-b2ae-04b34b5e7953](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/5a3c7a46-28f4-436d-872a-a816f5ad54d6)

画面右下の「参照」をクリックする。  
![249030182-ceceab37-0b5e-4dba-9138-0f0e16229c3f](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/bf01704d-4f98-4837-b3d1-a112387114f4)

先ほど作成した「SampleSDK.dll」を指定して「追加」をクリックする。  
尚、保存パスは、デフォルトは「C:\Users\（ユーザー名）\source\repos\SampleSDK\SampleSDK\bin\Release\net6.0」のようになっている。  
![249030324-9604a4e4-723a-4489-8c48-d64ac0de3b9e](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/42fbdc52-3e5c-4a61-b82d-d207286e8499)

追加したファイルにチェックが付いていることを確認し、「OK」ボタンをクリックする。  
![249030617-9556d800-7df5-4f98-9abc-315641d66322](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/1d88ac90-7ece-4494-bbd8-b7fc2dd4fe54)

以下のように、依存関係のアセンブリの下に、「SampleSDK」が追加されていることを確認する。  
![249030901-73789985-772d-472c-9ee4-2f921a0e325a](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/7067a7f4-56c0-401f-8e5d-7000a6bff835)

これで、SDKを利用する準備は完了。  
以下のようなコードを記載することで呼び出すことができる。  
```
SampleSDK.LogWriter.Error("WinFormsから呼び出し（Error）");
```
左から、名前空間、クラス名、メソッド名  
![249031577-1f5a6901-547f-4015-b499-e4b121ee5124](https://github.com/JurakuSoftware/SampleSDK/assets/55858517/1c5280a3-9d3b-46e6-9013-bacb6236c1e6)

実行し、デスクトップに「SampleSDK.log」というファイルが出力されていることを確認する。  

以上。  

尚、呼び出しで利用したサンプルソースは、以下に保存してある。  
https://github.com/JurakuSoftware/UseSampleSDK
