------------------------------------------------------------
 Unity CAD Importer
 Ver 0.5.2
 2017/4/4
------------------------------------------------------------

[System Requirements]
・Windows 7 or later (Mac OS not supported)
・.NET Framework 4.5 or later ( https://www.microsoft.com/ja-jp/download/details.aspx?id=30653)
・Visual Studio 2013 runntime（ https://www.microsoft.com/ja-jp/download/details.aspx?id=40784 )

[Setup]

1. Activate Unity license

Please note that you will still need to activate your license, even if you already have a Unity Profession serial number.
Please make sure to activate the license with the serial number for the trial version of Unity CAD Importer.

・Users already using Unity Professional
　1. Open Unity.
　2. Select “Help” then “Manage License” in the menu.
　3. Select “Activate New License”.
　4. Enter the serial number for the Unity CAD Importer trial version in the “Enter your serial number” field in “PROFESSIONAL EDITION”.
　5. Once completed, click “Start using Unity” to activate.

・Users already using Unity Personal
　1. Open Unity.
　2. Select “Help” then “Manage License” in the menu.
　3. Select “Activate New License”.
　4. Select “PROFESSIONAL EDITION”.
　4. Enter the serial number for the Unity CAD Importer trial version in the “Enter your serial number” field.
　5. Once completed, click “Start using Unity” to activate.

・First-time users of Unity
　1. Open Unity.
　2. Select “PROFESSIONAL EDITION”.
　3. Enter the serial number for the Unity CAD Importer trial version in the “Enter your serial number” field.
　4. Once completed, click “Start using Unity” to activate.

2. Import unitypackage

　1. Select “Assets” then “Import Package” then “Custom Package” in the menu.
　2. Select the distributed package “UnityCADImporter.unitypackage”.
　3. Click “Import”. 

3. Extract MoNo.RAIL

　1. Extract Assets/CADImporter/MoNoRAIL.zip in Windows Explorer.
　2. Place the “MoNo.RAIL” folder on the same hierarchical level as the “Assets” and “Library” folders. 

4. Activate Unity CAD Importer License

　1. Select “Assets” then “CAD Importer” in the menu.
　2. Two windows will appear. Drag and drop the license file for the trial version to the “Drag & Drop” field in the “License Installer” window.
　3. The path will be filled in License File Path. Click “Install”. 
　4. Once completed, a message saying “License installed.” will appear in the Console window.


[How to Use]

1. Select “Assets” then “CAD Importer” in the menu.
2. Drag and drop the CAD file (STL or IGES file) you wish to import to the “Drag & Drop” field.
3. Click “Import” to start importing. 

 [Target Vertices Count]
　Specify the number of vertices in the polygon mesh when partitioning a curved surface shape into a triangular polygon mesh. The shape will be partitioned into multiple polygon meshes with 65,000 vertices if you do not specify the number of vertices. The specified number number of vertices will be applied to each assembly (hierarchical level) if there are multiple assemblies in the IGES file.
  
 [Smooth - Rough]
  Adjust the smoothness when partitioning a curved surface shape into a triangular polygon mesh. This will affect the processing time. The smoother you make it, the finer the partitions and the longer the processing time. The rougher you make it, the larger the partitions and the shorter the processing time. Use the “Advanced Settings” below if you wish to adjust the partition even further.

［Advanced Settings］
・”Max Angle”: Angle precision specified in degrees. Specifies the maximum angle permitted in the refraction after converting a curved surface into a polygon.
・”Max Aspect Ratio”: Maximum aspect ratio permitted, specified in ratios of “1:X”. The maximum aspect ratio of the patches when partitioning a curved surface into rectangular patches.
・”Min Edge Length”: Minimum edge length permitted, specified in mm. The permitted minimum edge length of the patches when partitioning a curved surface into rectangular patches. The curved surface will not be partitioned into patches with a smaller edge length than the one specified here.
・”Max Edge Length”: Maximum edge length permitted. The maximum edge length of the patches when partitioning a curved surface into rectangular patches.
・”Max Deviation”: Distance accuracy specified in mm. Specifies the maximum deviation between the original curved surface and the generated polygon.
・”Discarding Threshold”： The polygon mesh will be ignored if the length of all the sides of the smallest cuboid surrounding a polygon mesh is smaller than the discarding threshold.
・”Mesh Healing”： Enable / Disable of healing feature

[Importing Tips]

Try adjusting the Quick Settings sliders (Smooth - Rough) first to see if you can get your desired partition in the polygon mesh.
Use the “Advanced Settings” to make more detailed changes to the partition. First, set the “Max Angle” and see what can be made in the shortest amount of time. Turn on “Max Aspect Ratio” and try to eliminate as many long polygons as possible if there are any polygon fragments. You do not need to adjust any of the other parameters.
