using UnityEngine;
using UnityEditor;
using System.Collections;

public class Builder {

	[MenuItem("Assets/Build CADImporter Package")]
	static public void BuildCADImporterPackage()
	{
		AssetDatabase.ExportPackage ("Assets/CADImporter", "UnityCADImporter.unitypackage", ExportPackageOptions.Recurse);
		Debug.Log ("Build!");
	}

}
