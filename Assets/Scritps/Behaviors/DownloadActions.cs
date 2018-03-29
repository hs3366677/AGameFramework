using UnityEngine;
using System.Collections;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
//using AssetFileLibrary;
using LuaFramework;

[TaskDescription("Wait until download finished.")]
[TaskIcon("{SkinColor}WaitIcon.png")]
public class DownloadFilelistAction : Action
{
    // The time to wait
    private bool downloadFinished;

    private bool downloadSuccess;

    public override void OnStart()
    {
        downloadFinished = false;
        downloadSuccess = false;
        StartCoroutine(DownloadProcess());
    }

    public override TaskStatus OnUpdate()
    {
        if (!downloadFinished)
            return TaskStatus.Running;
        else if (downloadSuccess)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        downloadFinished = false;
        downloadSuccess = false;
    }

    IEnumerator DownloadProcess()
    {
        //下载所有文件列表
        {
            //下载服务器的文件列表
            AssetFileInfo info = new AssetFileInfo();
            info.m_fileName = AppConst.FileListName;
            info.m_filePath = AppConst.FileListNameWithPostfix;
            info.m_size = 100; //此处无用
            info.CallBack = (x) => FileManager.Instance.ReadFileList(x);
            IEnumerator itor = DownloadManager.Instance.StartDownload(info);
            while (itor.MoveNext())
                yield return null;

            downloadFinished = true;
            downloadSuccess = info.IsDownLoadFinish;
        }
    }
}


[TaskDescription("Wait until download finished. Always return success")]
[TaskIcon("{SkinColor}WaitIcon.png")]
public class DownloadResfilesAction : Action
{
    // The time to wait
    private bool downloadFinished;

    public override void OnStart()
    {
        downloadFinished = false;
        StartCoroutine(DownloadProcess());
    }

    public override TaskStatus OnUpdate()
    {
        if (!downloadFinished)
            return TaskStatus.Running;
        else
            return TaskStatus.Success;
    }

    public override void OnReset()
    {
        downloadFinished = false;
    }

    IEnumerator DownloadProcess()
    {
        //下载/更新所有服务器文件
        {
            IEnumerator itor = DownloadManager.Instance.StartDownload(FileManager.Instance.ServerFileList);
            while (itor.MoveNext())
            {
                yield return null;
            }

            downloadFinished = true;
        }
    }
}

[TaskDescription("Wait until download finished. Always return success")]
[TaskIcon("{SkinColor}WaitIcon.png")]
public class DownloadCompressedpackageAction : Action
{
    // The time to wait
    private bool downloadSuccess;
    private bool downloadFinished;

    public override void OnStart()
    {
        downloadFinished = false;
        downloadSuccess = false;

        StartCoroutine(DownloadProcess());
    }

    public override TaskStatus OnUpdate()
    {
        if (!downloadFinished)
            return TaskStatus.Running;
        else if (downloadSuccess)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

    public override void OnReset()
    {
        downloadSuccess = false;
        downloadFinished = false;
    }

    IEnumerator DownloadProcess()
    {
        //下载/更新所有服务器文件
        {
            AssetFileInfo info = new AssetFileInfo();
            info.m_fileName = AppConst.PackageName;
            info.m_filePath = AppConst.PackageNameWithPostfix;
            info.m_size = 1000000; //此处无用
            info.CallBack = (x) =>
            {
                ZipUtil.DecompressToDirectory(
                    AppConst.GameSetting.ExternFileRootPath + AppConst.PackageNameWithPostfix,
                    AppConst.GameSetting.ExternFileRootPath, null);
            };
            
            IEnumerator itor = DownloadManager.Instance.StartDownload(info);
            while (itor.MoveNext())
            {
                yield return null;
            }

            downloadFinished = true;
            downloadSuccess = info.IsDownLoadFinish;
        }
    }
}


public class IsTooManyUpdates : Conditional
{
    public override TaskStatus OnUpdate()
    {
        if (FileManager.Instance.ifUpdatePackage())
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}