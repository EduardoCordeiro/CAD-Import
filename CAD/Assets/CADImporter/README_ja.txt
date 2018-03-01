------------------------------------------------------------
 Unity CAD Importer
 Ver 0.5.2
 2017/4/4
------------------------------------------------------------

【仕様条件】
・Windows 7以降 (MacOSは非対応）
・.NET Framework 4.5 以上 ( https://www.microsoft.com/ja-jp/download/details.aspx?id=30653)
・Visual Studio 2013 runntime（ https://www.microsoft.com/ja-jp/download/details.aspx?id=40784 )

【初期設定】

１．Unityライセンスアクティベーション

既にUnity Professional版シリアルナンバーをお持ちの方は、そのままのライセンスは使えませんのでご注意ください。
必ず「Unity CAD Importer試用版シリアルナンバー」でアクティベーションしてください。

・既にUnity Professional版をお使いの方
　１．Unityを起動してください。
　２．メニューの「Help」→「Manage License...」を選択します
　３．「Activate New License」 を選択します
　４．「PROFESSIONAL EDITION」の「Enter your serial number」の箇所に、お伝えしている「Unity CAD Importer試用版シリアルナンバー」を入力します
　５．成功していれば「Start using Unity」をクリックして起動します

・既にUnity Personal版をお使いの方
　１．Unityを起動してください。
　２．メニューの「Help」→「Manage License...」を選択します
　３．「Activate New License」 を選択します
　４．「PROFESSIONAL EDITION」を選択します
　５．「Enter your serial number」の箇所に、お伝えしている「Unity CAD Importer試用版シリアルナンバー」を入力します
　６．成功していれば「Start using Unity」をクリックして起動します

・初めてUnityを利用する方
　１．Unityを起動してください。
　２．「PROFESSIONAL EDITION」を選択します
　３．「Enter your serial number」の箇所に、お伝えしている「Unity CAD Importer試用版シリアルナンバー」を入力します
　４．成功していれば「Start using Unity」をクリックして起動します

２．unitypackage のインポート

　１．メニューから「Assets」→「Import Package」→「Custom Package...」を選択します
　２．配布した「UnityCADImporter.unitypackage」を選択します
　３．「Import」ボタンを押します

３．MoNo.RAILの展開

　１．Windowsエクスプローラ上から Assets/CADImporter/MoNo.RAIL.zip を解凍します。
　２．「Assets」や「Library」と同階層のフォルダに「MoNo.RAIL」フォルダを配置します。

４．Unity CAD Importer ライセンスアクティベーション

　１．メニューから「Assets」→「CAD Importer」を選択します
　２．ウィンドウが二つ出ます。「License Installer」のウィンドウに、「試用版ライセンスファイル」を「Drag & Drop」という場所にドラッグアンドドロップします
　３．License File Pathにパスが入ります。「Install」ボタンを押します。
　４．Consoleウィンドウに「License installed.」というメッセージが出ていれば成功です


【使用法】

１．メニューから「Assets」→「CAD Importer」を選択します
２．インポートしたいCADファイル（STLまたはIGESファイル）を「Drag & Drop」という場所にドラッグアンドドロップします
３．「Import」ボタンを押すとインポートが開始されます

 [Target Vertices Count]
　曲面形状を三角ポリゴンメッシュに分割する際の、ポリゴンメッシュの頂点数を指定します。指定しない場合は、65000頂点毎に複数のポリゴンメッシュに分割されます。IGES ファイル中に複数のアセンブリが在る場合は、アセンブリ（階層）毎に目標頂点数が適用されます。
  
 [Smooth - Rough]
  曲面形状を三角ポリゴンメッシュに分割する際の滑らかさと処理時間を調整します。「Smooth」側にするほど細かく分割され、処理時間は長くなります。「Rough」側にするほど粗く分割され、処理時間は短くなります。この分割具合を細かく調整したい場合は、下の [Advanced Settings] を行います。

［Advanced Settings］
・「Max Angle」: 角度精度。単位は度。曲面をポリゴン化したときの「折れ」の許容できる最大角度を指定します
・「Max Aspect Ratio」: 許容できるアスペクト比。単位は比率で「１：□」の□の部分。曲面を矩形パッチに分割していく際のパッチの最大アスペクト比
・「Min Edge Length」: 許容できる最小距離。単位はmm。曲面を矩形パッチに分割していく際のパッチの許容できる最小距離。これ以上小さくなった際は分割しない
・「Max Edge Length」: エッジの最大長。曲面を矩形パッチに分解していく際のパッチの最大長さ
・「Max Deviation」: 距離精度。単位はmm。元の曲面と生成されたポリゴンとの最大誤差を指定
・「Discarding Threshold」：ポリゴンメッシュを囲む最小の直方体のすべての辺の長さがこの値より小さい場合、そのポリゴンメッシュは無視されます
・「Mesh Healing」：ヒーリング機能のオン/オフ

【インポートのコツ】

まずは、簡単設定([Smooth - Rough])スライダーを調整し、ポリゴンメッシュへの分割具合が希望どおりになるか確認しましょう。
分割具合を細かく変更したい場合は、「Advanced Settings」を行います。まずは「Max Angle」を最大にして、最短の時間でどのぐらいのものができるか見てみましょう。ポリゴン欠けがある場合は、「Max Aspect Ratio」をオンにして長いポリゴンをできるだけないようにしましょう。それ以外のパラメータはあまりいじらなくても良いでしょう。
