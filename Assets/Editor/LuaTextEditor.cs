using UnityEngine;
using UnityEditor;
using System;

public class LuaTextEditor {


    static readonly string yourVisualStudioInstallPath = "D:/Microsoft Visual Studio 12.0(64-bit)/Common7/IDE/devenv.exe";
    [UnityEditor.Callbacks.OnOpenAssetAttribute(1)]
    public static bool step1(int instanceID, int line)
    {
        //string name = EditorUtility.InstanceIDToObject(instanceID).name;
        //Debug.Log("Open Asset step: 1 (" + name + ")");

        return false;
    }

    [UnityEditor.Callbacks.OnOpenAssetAttribute(2)]
    public static bool step2(int instanceID, int line)
    {
        string strFilePath = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceID));
        string strFileName = Application.dataPath + "/" + strFilePath.Replace("Assets/", "");

        if (strFileName.EndsWith(".lua"))
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = yourVisualStudioInstallPath;
                startInfo.Arguments = "/Edit " + strFileName;
                process.StartInfo = startInfo;
                process.Start();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("deveafdnv.exe not valid. Define your own .lua default launcher file path.");
                Debug.LogException(e);
            }
        }

        return false;
    }

}
