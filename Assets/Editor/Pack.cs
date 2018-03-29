using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

/// <summary>
/// 不同平台有很多配置不一样的地方，这里不提供共享的接口，请逐个平台实现
/// </summary>
public partial class Pack : MonoBehaviour
{
    


    [MenuItem("Pack/name Assetbundles", false, 110)]
    private static void SetName()
    {
        Debug.LogFormat("{0}Start set resources' bundle name...", PACKTAG);
        Stopwatch sw = new Stopwatch();
        sw.Start();

        List<string> fileAllPath = new List<string>();
        foreach (var path in ResourcesPath)
        {
            fileAllPath.AddRange(GetFiles(path, ResourceExt));
        }

        foreach (var file in fileAllPath)
        {
            string localPath = "";
            if (file.Contains("/Assets/"))
                localPath = file.Substring(file.IndexOf("/Assets/") + 1);
            //需要以Assets目录开头的路径
            AssetImporter improter = AssetImporter.GetAtPath(localPath);
            if (improter != null)
                improter.assetBundleName = Path.GetFileNameWithoutExtension(localPath);
        }
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();

        sw.Stop();
        Debug.LogFormat("[{0}]set name finished; cost {1}ms", PACKTAG, sw.ElapsedMilliseconds);
    }


    #region win32打包
    [MenuItem("Pack/Pack win32 exe", false, 1100)]
    public static void PackExe()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Debug.LogFormat("{0}Start pack windows executable...", PACKTAG);

        BuildTarget _buildTarget = BuildTarget.StandaloneWindows;
        BuildTargetGroup _buildGroup = BuildTargetGroup.Standalone;

        string[] Args = System.Environment.GetCommandLineArgs();
        string productName = "";
        string bundleVersion = "";
        int bundleVersionCode = 0;
        string bundle_identifier = "";
        string define_symbols = "";

        for (int i = 0; i < Args.Length; i++)
        {
            if (Args[i].Contains("-BUNDLEVERSIONCODE") && i + 1 < Args.Length)
            {
                bundleVersionCode = int.Parse(Args[i + 1]);
            }
            else if (Args[i].Contains("-BUNDLEVERSION") && i + 1 < Args.Length)
            {
                bundleVersion = Args[i + 1];
            }
            else if (Args[i].Contains("-PRODUCTNAME") && i + 1 < Args.Length)
            {
                productName = Args[i + 1];
            }
            else if (Args[i].Contains("-BUNDLEIDENTIFIER") && i + 1 < Args.Length)
            {
                bundle_identifier = Args[i + 1];
            }
            else if (Args[i].Contains("-SCRIPTINGDEFINESYMBOLS") && i + 1 < Args.Length)
            {
                define_symbols = Args[i + 1];
            }
        }

        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildGroup);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildGroup, symbols);
        PlayerSettings.companyName = CompanyName;
        PlayerSettings.productName = ProductName;
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.usePlayerLog = true;
        PlayerSettings.SplashScreen.show = false;

        QualitySettings.SetQualityLevel((int)QualityLevel.Fantastic);

        Debug.LogFormat("Generate Package productName = {0};bundleVersion = {1};bundleVersionCode = {2};bundle_identifier = {3};symbols = {4}",
            PlayerSettings.productName, PlayerSettings.bundleVersion, PlayerSettings.Android.bundleVersionCode, bundle_identifier, symbols);

        BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            StandaloneExeFullPath,
            _buildTarget,
            BuildOptions.None);

        sw.Stop();
        Debug.LogFormat("[{0}]pack finished; cost {1}ms", PACKTAG, sw.ElapsedMilliseconds);
    }

    [MenuItem("Pack/Pack Assetbundle(win32)", false, 1110)]
    public static void PackWindowsResources()
    {
        //Create .txt files from .lua files

        //name all files under the folder 
        
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Debug.LogFormat("[{0}]Start pack windows resources...", PACKTAG);

        Directory.CreateDirectory(StandaloneAbPath);

        BuildPipeline.BuildAssetBundles(StandaloneAbPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        sw.Stop();
        Debug.LogFormat("[{0}]pack finished; cost {1}ms", PACKTAG, sw.ElapsedMilliseconds);

        //delete the .txt files of .lua duplicate
    }

    private static void PackWindowResources(string path){
        string bundleName = Path.GetDirectoryName(path);
        AssetBundleBuild[] builds = new AssetBundleBuild[1];
        builds[0] = new AssetBundleBuild(){assetBundleName = bundleName};
        BuildPipeline.BuildAssetBundles(StandaloneAbPath, builds, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }

    /// <summary>
    /// Set file attribute of exe file;
    /// Need executable "ResourceHacker.exe";
    /// You can download from http://www.angusj.com/resourcehacker/;
    /// Change attributes to your needs
    /// </summary>
    private static void SetFileAttribute()
    {
        //远程机上生效
        string resourceHackerPath = "D:\\Resource Hacker\\ResourceHacker.exe";
        string resourcePath = "D:\\Resource Hacker\\GameOtome.res";
        string logPath = "D:\\Resource Hacker\\log.txt";
        string fileTobeProcess = ".\\Game\\otome.exe";

        Process process = new Process();

        string info = string.Format(" -open \"{0}\" -save \"{1}\" -action addoverwrite -res \"{2}\" -log \"{3}\"", fileTobeProcess, fileTobeProcess, resourcePath, logPath);
        ProcessStartInfo startInfo = new ProcessStartInfo(resourceHackerPath, info);
        process.StartInfo = startInfo;

        //隐藏DOS窗口    
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.Start();
        process.WaitForExit();

        process.Close();
    }
    #endregion


    #region Android打包
    [MenuItem("Assets/打包Assetbundle(Android)", false, 2200)]
    public static void PackAndroidResources()
    {

        
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Debug.LogFormat("{0}Start pack android resources...", PACKTAG);

        //EditorUtility.DisplayProgressBar("Placing Prefabs", "Working...", 0);

        Directory.CreateDirectory(AndroidAbPath);

        BuildPipeline.BuildAssetBundles(AndroidAbPath, BuildAssetBundleOptions.None, BuildTarget.Android);

        Debug.LogFormat("[{0}]pack finished; cost {1}s", PACKTAG, sw.Elapsed.TotalSeconds);
    }


    [MenuItem("打包/打包Apk")]
    public static void PackApk()
    {
        BuildTarget _buildTarget = BuildTarget.Android;
        BuildTargetGroup _buildGroup = BuildTargetGroup.Android;
        //if (ABMode)

        //_PackAllResource();
        //_CleanResourceDir();

        string[] Args = System.Environment.GetCommandLineArgs();
        string productName = "";
        string bundleVersion = "";
        int bundleVersionCode = 0;
        string bundle_identifier = "";
        string define_symbols = "";

        for (int i = 0; i < Args.Length; i++)
        {
            if (Args[i].Contains("-BUNDLEVERSIONCODE") && i + 1 < Args.Length)
            {
                bundleVersionCode = int.Parse(Args[i + 1]);
            }
            else if (Args[i].Contains("-BUNDLEVERSION") && i + 1 < Args.Length)
            {
                bundleVersion = Args[i + 1];
            }
            else if (Args[i].Contains("-PRODUCTNAME") && i + 1 < Args.Length)
            {
                productName = Args[i + 1];
            }
            else if (Args[i].Contains("-BUNDLEIDENTIFIER") && i + 1 < Args.Length)
            {
                bundle_identifier = Args[i + 1];
            }
            else if (Args[i].Contains("-SCRIPTINGDEFINESYMBOLS") && i + 1 < Args.Length)
            {
                define_symbols = Args[i + 1];
            }
        }

        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildGroup);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildGroup, symbols);
        PlayerSettings.companyName = "SHYeyou";
        PlayerSettings.productName = "新项目";
        PlayerSettings.bundleVersion = bundleVersion;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = false;
        PlayerSettings.allowedAutorotateToLandscapeRight = false;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
        PlayerSettings.usePlayerLog = true;
        PlayerSettings.SplashScreen.show = false;

        QualitySettings.SetQualityLevel((int)QualityLevel.Fantastic);

        GenKeyStoreInfo(symbols);

        Debug.LogFormat("Generate Package productName = {0} bundleVersion = {1} bundleVersionCode = {2} bundle_identifier = {3} symbols = {4}",
            PlayerSettings.productName, PlayerSettings.bundleVersion, PlayerSettings.Android.bundleVersionCode, bundle_identifier, symbols);

        BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            @"./Game/otome.apk", 
            _buildTarget, 
            BuildOptions.None);

    }

    public static List<string> GetFiles(string root, string[] extSet)
    {
        List<string> myFileList = new List<string>();
        
        var dirFiles = Directory.GetFiles(root);
        
        foreach (var file in dirFiles)
        {
            foreach (var ext in extSet)
            {
                Path.GetExtension(file).Equals(ext, System.StringComparison.InvariantCultureIgnoreCase);
            }
            myFileList.Add(file.Replace('\\', '/'));
        }

        var directorys = Directory.GetDirectories(root);
        
        foreach (var dir in directorys)
            myFileList.AddRange(GetFiles(dir, extSet));

        return myFileList;
    }

    private static void GenKeyStoreInfo(string parms)
    {
        if (parms == "PARTER_DOWNJOY")
        {
            PlayerSettings.Android.keystoreName = Application.dataPath + "/KeyStore/DownJoy.keystore";
            PlayerSettings.Android.keystorePass = "downjoy_357";
            PlayerSettings.Android.keyaliasName = "357";
            PlayerSettings.Android.keyaliasPass = "downjoy_357";
        }
        else
        {
            PlayerSettings.Android.keystoreName = Application.dataPath + "/KeyStore/DDLELW.keystore";
            PlayerSettings.Android.keystorePass = "DDLE_LW_20130624";
            PlayerSettings.Android.keyaliasName = "lianwu";
            PlayerSettings.Android.keyaliasPass = "lianwu20130624";
        }
    }

    private static void SetSplashImage(string parms)
    {
        string targetFileName = "Assets/Art_Res/Splash/ActiveSplash.jpg";
        string sourceFileName = GetSplashSourceIcon(parms);
        PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;
        FileUtil.ReplaceFile(sourceFileName, targetFileName);
        AssetDatabase.Refresh();
    }

    private static string GetSplashSourceIcon(string parms)
    {
        if (parms == "PARTER_DOWNJOY")
        {
            return "Assets/Art_Res/Splash/DownJoySplash.jpg";
        }
        return "Assets/Art_Res/Splash/CommonSplash.jpg";
    }
#endregion


    #region IOS打包
    #endregion


    [MenuItem("Pack/Generate md5 filelist")]
    public static void GenerateFileList() {
        //implemented in a single .exe program
    }

    [MenuItem("Assets/Pack", false, 1200)]
    public static void PackAllType(){
        Packager.BuildWindowsResource();
    }

    /// <summary>
    /// only directories that is in the <paramref name="ResourcesPath"/> array is allowed to be packed
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Pack", true)]
    public static bool IsPackValid(){
        string fullSelectedPath = Utilities.NormalizePath(Utilities.GetSelectedPathOrFallback());

        for (int i = 0; i < ResourcesPath.Length; i++)
        {
            if (fullSelectedPath.StartsWith(Utilities.NormalizePath(ResourcesPath[i])))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// limited lua script creation to folds of name LuaScripts
    /// </summary>
    /// <returns></returns>
    [MenuItem("Assets/Create/LuaScript", true)]
    public static bool IsCreateLuaScriptValid()
    {
        string fullSelectedPath = Utilities.NormalizePath(Utilities.GetSelectedPathOrFallback());

        if (fullSelectedPath.EndsWith("LUASCRIPTS"))
        {
            return true;
        }

        return false;
    }
}
