﻿using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;


namespace gcloud_voice
{
	public class GCloudVoiceEngine : IGCloudVoice
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR
		public const string LibName = "GCloudVoice";
		#else
		#if UNITY_IPHONE || UNITY_XBOX360
		public const string LibName = "__Internal";
		#else
		public const string LibName = "GCloudVoice";
		#endif
		#endif
		
		enum NoticeMessageType {
			MSG_ON_JOINROOM_COMPLETE =1,
			MSG_ON_QUITROOM_COMPLETE,
			MSG_ON_UPLOADFILE_COMPLETE,
			MSG_ON_DOWNFILE_COMPLETE,
			MSG_ON_MEMBER_VOICE,
			MSG_ON_APPLY_AUKEY_COMPLETE,
			MSG_ON_PLAYFILE_COMPLETE,
			MSG_ON_SPEECH_TO_TEXT,
			MSG_ON_ROOM_OFFLINE,
			MSG_ON_STREAM_SPEECH_TO_TEXT,
            MSG_ON_ROLE_CHANGED,
		};
		public class NoticeMessage
		{
			public int what;
			public int intArg1;
			public int intArg2;
			public string strArg;
			public byte[] custom;
			public int datalen;
			public NoticeMessage()
			{
				what = -1;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				datalen = 0;
				custom = new byte[2048];
			}
			public void clear()
			{
				what = -1;
				intArg1 = 0;
				intArg2 = 0;
				strArg = "";
				datalen = 0;
			}
		}
		public override event JoinRoomCompleteHandler OnJoinRoomComplete;
		public override event QuitRoomCompleteHandler OnQuitRoomComplete;
		public override event MemberVoiceHandler      OnMemberVoice;
		public override event ApplyMessageKeyCompleteHandler OnApplyMessageKeyComplete;
		public override event UploadReccordFileCompleteHandler OnUploadReccordFileComplete;
		public override event DownloadRecordFileCompleteHandler OnDownloadRecordFileComplete;
		public override event PlayRecordFilCompleteHandler OnPlayRecordFilComplete;
		public override event SpeechToTextHandler OnSpeechToText;
		public override event StreamSpeechToTextHandler OnStreamSpeechToText;
        public override event StatusUpdateHandler OnStatusUpdate;
        public override event ChangeRoleCompleteHandler OnRoleChangeComplete;
        public override event RoomMemberVoiceHandler OnRoomMemberVoice;
		
		private static bool bInit = false;
		private static bool bIsSetAppInfo = false;
		private int pollBufLen = 2048;
		private byte[] pollBuf;
		private NoticeMessage pollMsg = null;
		private int[] memberVoice = null;
		private byte[] roomNameBuf;
		
		#region DllImport 
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CreateInstance();	
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetServerInfo([MarshalAs(UnmanagedType.LPArray)] string URL);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetAppInfo([MarshalAs(UnmanagedType.LPArray)] string appID, [MarshalAs(UnmanagedType.LPArray)]string appKey,[MarshalAs(UnmanagedType.LPArray)] string openID);	
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Init();	
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMode(int mode);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_Poll(byte[] buf, int length);		

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Pause();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_Resume();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinTeamRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_QueryRoomName(byte[] buf, int length, int index);

        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_JoinRangeRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinNationalRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int role, int msTimeout);
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_ChangeRole(int role, [MarshalAs(UnmanagedType.LPArray)] string roomName);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinFMRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_QuitRoom([MarshalAs(UnmanagedType.LPArray)] string roomName, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_OpenMic();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CloseMic();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_OpenSpeaker();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_CloseSpeaker();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableRoomMicrophone([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableRoomSpeaker([MarshalAs(UnmanagedType.LPArray)] string roomName, bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableMultiRoom(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_UpdateCoordinate([MarshalAs(UnmanagedType.LPArray)] string roomName, long x, long y, long z, long r);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ApplyMessageKey(int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMaxMessageLength(int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartRecording([MarshalAs(UnmanagedType.LPArray)] string filePath, bool bOptimized);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopRecording();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_UploadRecordedFile([MarshalAs(UnmanagedType.LPArray)] string filePath, int msTimeout, bool bPermanent);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_DownloadRecordedFile([MarshalAs(UnmanagedType.LPArray)] string fileID, [MarshalAs(UnmanagedType.LPArray)] string downloadFilePath, int msTimeout, bool bPermanent);	
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_PlayRecordedFile([MarshalAs(UnmanagedType.LPArray)] string downloadFilePath);	
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopPlayFile();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechToText([MarshalAs(UnmanagedType.LPArray)] string fileID, int language, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ForbidMemberVoice(int member, bool bEnable, [MarshalAs(UnmanagedType.LPArray)] string roomName);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableLog(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetDataFree(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetMicLevel(bool bFadeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetSpeakerLevel();
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetSpeakerVolume(int vol);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_TestMic() ;
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetFileParam([MarshalAs(UnmanagedType.LPArray)] string filepath, int [] bytes, float []seconds);		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_invoke( uint nCmd, uint nParam1, uint nParam2, [MarshalAs(UnmanagedType.LPArray)] uint [] pOutput );
		
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinNationalRoom_Token([MarshalAs(UnmanagedType.LPArray)] string roomName, int role, [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_JoinTeamRoom_Token( [MarshalAs(UnmanagedType.LPArray)] string roomName,  [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ApplyMessageKey_Token([MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout);
		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SpeechToText_Token([MarshalAs(UnmanagedType.LPArray)] string fileID, [MarshalAs(UnmanagedType.LPArray)] string token, int timestamp, int msTimeout, int language);								

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableSpeakerOn(bool enable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetMicVol(int vol);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr GCloudVoice_GetInstance();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetVoiceEffects(int mode);
		
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GCloudVoice_IsSpeaking();
        
        [DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableReverb(bool bEnable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetReverbMode(int mode);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetVoiceIdentify();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBGMPath([MarshalAs(UnmanagedType.LPArray)] string path);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StartBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_StopBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_PauseBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_ResumeBGMPlay();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetBGMPlayState();

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBGMVol(int vol);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_EnableNativeBGMPlay(bool bEnable);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_SetBitRate(int bitrate);

		[DllImport(LibName, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GCloudVoice_GetHwState();


		#endregion 
		
		public GCloudVoiceEngine()
		{
			int ret = GCloudVoice_CreateInstance();
			if(ret != 0)
			{
				Debug.Log("Create GCloudVoiceInstance failed!");
			}
			pollBuf = new byte[pollBufLen];
			pollMsg = new NoticeMessage();
			roomNameBuf = new byte[256];
			memberVoice = new int[1024];
		}
		
		public override int SetAppInfo(string appID, string appKey, string openID)
		{
			int ret = GCloudVoice_SetAppInfo(appID, appKey, openID);
			if(ret == 0)
			{
				bIsSetAppInfo = true;
			}
			return ret;
		}

		public override int SetServerInfo(string URL)
		{
			int ret = GCloudVoice_SetServerInfo(URL);
			return ret;
		}
		
		public override int Init()
		{
			if(!bIsSetAppInfo)
			{
				Debug.Log("please set appinfo first");
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_SETAPPINFO;
			}
			if(!bInit)
			{
				int ret = GCloudVoice_Init();
				if(ret != 0)
				{
					Debug.Log("Init GCloudVoice failed!");
					return ret;
				}
				bInit = true;
			} 
			return (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC;         
		}
		
		public override  int SetMode(GCloudVoiceMode nMode)
		{
			Debug.Log("GCloudVoice_C# API: _SetMode");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			return GCloudVoice_SetMode((int)nMode);
		}
		
		public NoticeMessage NoticeMessage_ParseFrom(byte[] buf, int buflen)
		{
			int guard = 0;
			if (buflen - guard < sizeof(Int32)) {
				Debug.Log("notifymsg,parse error, buf small then sizeof(int)");
				return null;
			}
			pollMsg.what = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			pollMsg.intArg1 = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			pollMsg.intArg2 = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			int strlen = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			if (strlen == 0) {
				pollMsg.strArg = "";
			} else {
				byte[] bstr = new byte[strlen];
				Array.Copy(buf, guard, bstr, 0, strlen);	
				pollMsg.strArg = System.Text.Encoding.Default.GetString ( bstr );
			}
			guard += strlen;
			pollMsg.datalen = BitConverter.ToInt32 (buf, guard);
			guard += sizeof(UInt32);
			if (pollMsg.datalen > 0) {
				Array.Copy(buf, guard, pollMsg.custom, 0, pollMsg.datalen);
			}
			return pollMsg;
			
		}
		
		public  override  int Poll()
		{	
			int ret = GCloudVoice_Poll(pollBuf, pollBufLen);
			if (ret != 0) {
				if(ret == (int)GCloudVoiceErr.GCLOUD_VOICE_POLL_MSG_NO)
				{
					//poll no msg, return succ
					return 0;
				}
				return ret;
			}
			pollMsg.clear ();
			NoticeMessage msg = NoticeMessage_ParseFrom (pollBuf, pollBufLen);
			if (msg == null) {
				return (int)GCloudVoiceErr.GCLOUD_VOICE_POLL_MSG_PARSE_ERR;
			}
			if (msg.what == (int)NoticeMessageType.MSG_ON_JOINROOM_COMPLETE) {
				if (OnJoinRoomComplete != null) {
					Debug.Log ("c# poll callback OnJoinRoomComplete not null");
					OnJoinRoomComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
				}
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_QUITROOM_COMPLETE) {
				if (OnQuitRoomComplete != null) {
					OnQuitRoomComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2);
				}
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_APPLY_AUKEY_COMPLETE) {
				if (OnApplyMessageKeyComplete != null) {
					OnApplyMessageKeyComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1);
				} 
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_UPLOADFILE_COMPLETE) {
				if (OnUploadReccordFileComplete != null) {
					OnUploadReccordFileComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.Default.GetString (msg.custom, 0, msg.datalen) : "");
				} 
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_DOWNFILE_COMPLETE) {
				if (OnDownloadRecordFileComplete != null) {
					OnDownloadRecordFileComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.Default.GetString (msg.custom, 0, msg.datalen) : "");
				} 
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_PLAYFILE_COMPLETE) {
				if (OnPlayRecordFilComplete != null) {
					OnPlayRecordFilComplete ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg);
				}
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_SPEECH_TO_TEXT) {
				if (OnSpeechToText != null) {
					OnSpeechToText ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.custom != null ? System.Text.Encoding.UTF8.GetString (msg.custom, 0, msg.datalen) : "");
				} 
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_STREAM_SPEECH_TO_TEXT) {
				if (OnStreamSpeechToText != null) {
					OnStreamSpeechToText ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.intArg2, msg.custom != null ? System.Text.Encoding.UTF8.GetString (msg.custom, 0, msg.datalen) : "", msg.strArg);
				} 			
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_MEMBER_VOICE) {
				
					Array.Clear (memberVoice, 0, memberVoice.Length);
					int memcount = msg.intArg1;
					for (int i = 0; i < memcount; i++) {
						memberVoice [2 * i] = System.BitConverter.ToInt32 (pollMsg.custom, 2 * i * 4);
						
						memberVoice [2 * i + 1] = System.BitConverter.ToInt32 (pollMsg.custom, (2 * i + 1) * 4);
					int index = (int)  (memberVoice [2 * i] &0xf0000000) >>28;
					int memID =  memberVoice[2*i] & 0x0fffffff;
					int roomNameBufLen = 256;
					int roomNameLen = GCloudVoice_QueryRoomName(roomNameBuf, roomNameBufLen, index);
					if (OnRoomMemberVoice != null) {
						string roomNameStr = System.Text.Encoding.Default.GetString(roomNameBuf, 0, roomNameLen);
						OnRoomMemberVoice(roomNameStr, memID, memberVoice [2 * i + 1]);
					}
						
					}

				if (OnMemberVoice != null) {
					OnMemberVoice (memberVoice, memcount);
				}
			} else if (msg.what == (int)NoticeMessageType.MSG_ON_ROOM_OFFLINE) {
				if (OnStatusUpdate != null) {
					OnStatusUpdate ((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1,  msg.strArg, msg.intArg2);
				}
            }
            else if (msg.what == (int)NoticeMessageType.MSG_ON_ROLE_CHANGED)
            {
                if (OnRoleChangeComplete != null)
                {
                    //Debug.Log("ChangeRole array num : [" + msg.custom[0] + "," + msg.custom[1] + "," + msg.custom[2] + "," + msg.custom[3]);
                    OnRoleChangeComplete((IGCloudVoice.GCloudVoiceCompleteCode)msg.intArg1, msg.strArg, msg.intArg2, msg.custom[0]);
                }
            }
			return (int)GCloudVoiceErr.GCLOUD_VOICE_SUCC;
		}
		
		public override  int Pause()
		{
			Debug.Log("GCloudVoice_C# API: _Pause");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_Pause();
			Debug.Log("GCloudVoice_C# API: _Pause nRet=" + nRet);
			return nRet;
		}
		
		public  override int Resume()
		{
			Debug.Log("GCloudVoice_C# API: _Resume");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_Resume();
			Debug.Log("GCloudVoice_C# API: _Resume nRet=" + nRet);
			return nRet;
		}
		
		public override int JoinTeamRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_JoinTeamRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom  nRet=" + nRet);
			return nRet;
		}

		public override int JoinRangeRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinRangeRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_JoinRangeRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinRangeRoom  nRet=" + nRet);
			return nRet;
		}


		public override int JoinTeamRoom(string roomName, string token, int timestamp, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom"+" mstimeout:"+msTimeout);
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_JoinTeamRoom_Token(roomName, token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinTeamRoom  nRet=" + nRet);
			return nRet;
		}		
		
		public override int JoinNationalRoom(string roomName, GCloudVoiceRole role, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_JoinNationalRoom(roomName, (int)role, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom  nRet=" + nRet);
			return nRet;
		}

        public override int ChangeRole(GCloudVoiceRole role, string roomName="")
        {
            Debug.Log("GCloudVoice_C# API: ChangeRole");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }
            int nRet = GCloudVoice_ChangeRole((int)role, roomName);
            Debug.Log("GCloudVoice_C# API: GCloudVoice_ChangeRole  nRet=" + nRet);
            return nRet;
        }

		public override int JoinFMRoom(string roomID,int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinFMID");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = 0;// = GCloudVoice_JoinFMRoom(roomName, msTimeout);
			//Debug.Log("GCloudVoice_C# API: JoinFMRoom  nRet=" + nRet);
			return nRet;
		}		

		public override int JoinNationalRoom(string roomName, string token, int timestamp, GCloudVoiceRole role, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_JoinNationalRoom_Token(roomName, (int)role, token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: JoinNationalRoom  nRet=" + nRet);
			return nRet;
		}		
		
		public override int QuitRoom(string roomName, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: QuitRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_QuitRoom(roomName, msTimeout);
			Debug.Log("GCloudVoice_C# API: QuitRoom  nRet=" + nRet);
			return nRet;
		}
		
		public override int OpenMic()
		{
			Debug.Log("GCloudVoice_C# API: OpenMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_OpenMic();
			Debug.Log("GCloudVoice_C# API: OpenMic  nRet=" + nRet);
			return nRet;
		}
		
		public override int CloseMic()
		{
			Debug.Log("GCloudVoice_C# API: CloseMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_CloseMic();
			Debug.Log("GCloudVoice_C# API: CloseMic  nRet=" + nRet);
			return nRet;
		}
		
		public override int OpenSpeaker()
		{
			Debug.Log("GCloudVoice_C# API: OpenSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_OpenSpeaker();
			Debug.Log("GCloudVoice_C# API: OpenSpeaker  nRet=" + nRet);
			return nRet;
		}
		
		public override int CloseSpeaker()
		{
			Debug.Log("GCloudVoice_C# API: CloseSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_CloseSpeaker();
			Debug.Log("GCloudVoice_C# API: CloseSpeaker  nRet=" + nRet);
			return nRet;
		}
		

		public override int EnableRoomMicrophone(string roomName, bool enable)
		{
			Debug.Log("GCloudVoice_C# API: EnableRoomMicrophone");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableRoomMicrophone(roomName, enable);
			Debug.Log("GCloudVoice_C# API: EnableRoomMicrophone  nRet=" + nRet);
			return nRet;
		}

		public override int EnableRoomSpeaker(string roomName, bool enable)
		{
			Debug.Log("GCloudVoice_C# API: EnableRoomSpeaker");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableRoomSpeaker(roomName, enable);
			Debug.Log("GCloudVoice_C# API: EnableRoomSpeaker  nRet=" + nRet);
			return nRet;

		}

		public override int EnableMultiRoom(bool enable)
		{
			Debug.Log("GCloudVoice_C# API: EnableMultiRoom");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableMultiRoom(enable);
			Debug.Log("GCloudVoice_C# API: EnableMultiRoom  nRet=" + nRet);
			return nRet;

		}

		public override int UpdateCoordinate (string roomName, long x, long y, long z, long r)
		{
			Debug.Log("GCloudVoice_C# API: UpdateCoordinate");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}

			int nRet = GCloudVoice_UpdateCoordinate (roomName, x, y, z, r);
			Debug.Log("GCloudVoice_C# API: UpdateCoordinate  nRet=" + nRet);
			return nRet;
		}
		
		public override int ApplyMessageKey(int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_ApplyMessageKey(msTimeout);
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey  nRet=" + nRet);
			return nRet;
		}

		public override int ApplyMessageKey(string token, int timestamp, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_ApplyMessageKey_Token(token, timestamp, msTimeout);
			Debug.Log("GCloudVoice_C# API: ApplyMessageKey  nRet=" + nRet);
			return nRet;
		}		
		
		public override int SetMaxMessageLength(int msTime)
		{
			Debug.Log("GCloudVoice_C# API: SetMaxMessageLength");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetMaxMessageLength(msTime);
			Debug.Log("GCloudVoice_C# API: SetMaxMessageLength  nRet=" + nRet);
			return nRet;
		}
		
		public override int StartRecording(string filePath)
		{
			Debug.Log("GCloudVoice_C# API: StartRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StartRecording(filePath, false);
			Debug.Log("GCloudVoice_C# API: StartRecording  nRet=" + nRet);
			return nRet;
		}
		
		public override int StopRecording ()
		{
			Debug.Log("GCloudVoice_C# API: StopRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StopRecording();
			Debug.Log("GCloudVoice_C# API: StopRecording  nRet=" + nRet);
			return nRet;
		}

#if UNITY_IPHONE
        public override int StartRecording(string filePath, bool bOptimized)
		{
			Debug.Log("GCloudVoice_C# API: StartRecording");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StartRecording(filePath, bOptimized);
			Debug.Log("GCloudVoice_C# API: StartRecording  nRet=" + nRet);
			return nRet;
		}
		
#endif
		
		// path should be like "//"
		public override int UploadRecordedFile(string filePath, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: UploadRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_UploadRecordedFile(filePath, msTimeout, false);
			Debug.Log("GCloudVoice_C# API: UploadRecordedFile  nRet=" + nRet);
			return nRet;
		}	
		
		public override int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout)
		{
			Debug.Log("GCloudVoice_C# API: DownloadRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_DownloadRecordedFile(fileID, downloadFilePath, msTimeout, false);
			Debug.Log("GCloudVoice_C# API: DownloadRecordedFile  nRet=" + nRet);
			return nRet;
		}

        //overidload
        public override int UploadRecordedFile(string filePath, int msTimeout, bool bPermanent)
        {
            Debug.Log("GCloudVoice_C# API: UploadRecordedFile");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_UploadRecordedFile(filePath, msTimeout, bPermanent);
            Debug.Log("GCloudVoice_C# API: UploadRecordedFile  nRet=" + nRet);
            return nRet;
        }

        public override int DownloadRecordedFile(string fileID, string downloadFilePath, int msTimeout, bool bPermanent)
        {
            Debug.Log("GCloudVoice_C# API: DownloadRecordedFile");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_DownloadRecordedFile(fileID, downloadFilePath, msTimeout, bPermanent);
            Debug.Log("GCloudVoice_C# API: DownloadRecordedFile  nRet=" + nRet);
            return nRet;
        }	
		
		public override int PlayRecordedFile(string downloadFilePath)
		{
			Debug.Log("GCloudVoice_C# API: PlayRecordedFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_PlayRecordedFile(downloadFilePath);
			Debug.Log("GCloudVoice_C# API: PlayRecordedFile  nRet=" + nRet);
			return nRet;
		}		
		
		public override int StopPlayFile()
		{
			Debug.Log("GCloudVoice_C# API: StopPlayFile");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StopPlayFile();
			Debug.Log("GCloudVoice_C# API: StopPlayFile  nRet=" + nRet);
			return nRet;
		}	
		
		public override int SpeechToText(string fileID, int language = 0, int msTimeout = 6000)
		{
			Debug.Log("GCloudVoice_C# API: SpeechToText");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SpeechToText(fileID, language, msTimeout);
			Debug.Log("GCloudVoice_C# API: SpeechToText  nRet=" + nRet);
			return nRet;
		}	

		public override int SpeechToText(string fileID, string token, int timestamp, int language = 0, int msTimeout = 6000)
		{
			Debug.Log("GCloudVoice_C# API: SpeechToText");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SpeechToText_Token(fileID, token, timestamp, language, msTimeout);
			Debug.Log("GCloudVoice_C# API: SpeechToText  nRet=" + nRet);
			return nRet;
		}			
		
		public override int ForbidMemberVoice(int member, bool bEnable, string roomName="")
		{
			Debug.Log("GCloudVoice_C# API: ForbidMemberVoice");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_ForbidMemberVoice(member, bEnable, roomName);
			Debug.Log("GCloudVoice_C# API: ForbidMemberVoice  nRet=" + nRet);
			return nRet;
		}
		
		public override int EnableLog(bool enable)
		{
			Debug.Log("GCloudVoice_C# API: EnableLog");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableLog(enable);
			Debug.Log("GCloudVoice_C# API: EnableLog  nRet=" + nRet);
			return nRet;
		}

		public override int SetDataFree(bool enable)
		{
			Debug.Log("GCloudVoice_C# API: SetDataFree");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetDataFree(enable);
			Debug.Log("GCloudVoice_C# API: SetDataFree  nRet=" + nRet);
			return nRet;

		}
		
		public override int GetMicLevel()
		{
			Debug.Log("GCloudVoice_C# API: GetMicLevel");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetMicLevel(true);
			Debug.Log("GCloudVoice_C# API: GetMicLevel  nRet=" + nRet);
			return nRet;
		}

        public override int GetMicLevel(bool bFadeOut)
        {
            Debug.Log("GCloudVoice_C# API: GetMicLevel");
            if (!bInit)
            {
                return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
            }

            int nRet = GCloudVoice_GetMicLevel(bFadeOut);
            Debug.Log("GCloudVoice_C# API: GetMicLevel  nRet=" + nRet);
            return nRet;
        }
		
		public override int GetSpeakerLevel()
		{
			Debug.Log("GCloudVoice_C# API: GetSpeakerLevel");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetSpeakerLevel();
			Debug.Log("GCloudVoice_C# API: GetSpeakerLevel  nRet=" + nRet);
			return nRet;
		}
		
		public override int SetSpeakerVolume(int vol)
		{
			Debug.Log("GCloudVoice_C# API: SetSpeakerVolume");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetSpeakerVolume(vol);
			Debug.Log("GCloudVoice_C# API: SetSpeakerVolume  nRet=" + nRet);
			return nRet;
		}
		
		public override int TestMic()
		{
			Debug.Log("GCloudVoice_C# API: TestMic");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_TestMic();
			Debug.Log("GCloudVoice_C# API: TestMic  nRet=" + nRet);
			return nRet;
		}
		
		public override int GetFileParam(string filepath, int [] bytes, float [] seconds)
		{
			Debug.Log("GCloudVoice_C# API: GetFileParam");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetFileParam(filepath, bytes, seconds);
			Debug.Log("GCloudVoice_C# API: GetFileParam  nRet=" + nRet);
			return nRet;
		}
		
		public override int invoke( uint nCmd, uint nParam1, uint nParam2, uint [] pOutput )
		{
			Debug.Log("GCloudVoice_C# API: invoke");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_invoke(nCmd, nParam1, nParam2, pOutput);
			Debug.Log("GCloudVoice_C# API: invoke  nRet=" + nRet);
			return nRet;
		}
		public override int EnableSpeakerOn(bool bEnable)
		{
			Debug.Log("GCloudVoice_C# API: EnableSpeakerOn");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableSpeakerOn(bEnable);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_EnableSpeakerOn  nRet=" + nRet);
			return nRet;

		}

		public override int SetMicVolume(int vol)
		{
			Debug.Log("GCloudVoice_C# API: SetMicVol");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetMicVol(vol);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_SetMicVol  nRet=" + nRet);
			return nRet;

		}

		public override int SetVoiceEffects(SoundEffects mode)
		{
			Debug.Log("GCloudVoice_C# API: SetVoiceEffects");
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetVoiceEffects((int)mode);
			Debug.Log("GCloudVoice_C# API: SetVoiceEffects  nRet=" + nRet);
			return nRet;
		}
		
        public override int IsSpeaking()
        {
            if (!bInit)
            {
                return 0;
            }

            return GCloudVoice_IsSpeaking();            
        }
        
        
		public override int EnableReverb(bool bEnable)
		{
			Debug.Log("GCloudVoice_C# API: EnableReverb");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableReverb(bEnable);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_EnableReverb  nRet=" + nRet);
			return nRet;	
		}	

		public override IntPtr GetExtensionPluginContext()
        {
             return GCloudVoice_GetInstance();
        }

			

		public override int SetReverbMode(int mode)
		{
			Debug.Log("GCloudVoice_C# API: SetReverbMode");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetReverbMode(mode);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_SetReverbMode  nRet=" + nRet);
			return nRet;

		}
		public override int GetVoiceIdentify()
		{
			Debug.Log("GCloudVoice_C# API: GetVoiceIdentify");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetVoiceIdentify();
			Debug.Log("GCloudVoice_C# API: GCloudVoice_GetVoiceIdentify nRet = "+ nRet);
			return nRet;

		}

		public override int SetBGMPath(string path)
		{
			Debug.Log("GCloudVoice_C# API: SetBGMPath");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetBGMPath(path);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_SetBGMPath nRet = "+ nRet);
			return nRet;

		}

		public override int StartBGMPlay()
		{
			Debug.Log("GCloudVoice_C# API: StartBGMPlay");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StartBGMPlay();
			Debug.Log("GCloudVoice_C# API: GCloudVoice_StartBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int StopBGMPlay()
		{
			Debug.Log("GCloudVoice_C# API: StopBGMPlay");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_StopBGMPlay();
			Debug.Log("GCloudVoice_C# API: GCloudVoice_StopBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int PauseBGMPlay()
		{
			Debug.Log("GCloudVoice_C# API: PauseBGMPlay");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_PauseBGMPlay();
			Debug.Log("GCloudVoice_C# API: GCloudVoice_PauseBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int ResumeBGMPlay()
		{
			Debug.Log("GCloudVoice_C# API: ResumeBGMPlay");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_ResumeBGMPlay();
			Debug.Log("GCloudVoice_C# API: GCloudVoice_ResumeBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int SetBGMVol(int vol)
		{
			Debug.Log("GCloudVoice_C# API: SetBGMVol");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetBGMVol(vol);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_SetBGMVol nRet = "+ nRet);
			return nRet;

		}

		public override int GetBGMPlayState()
		{
			//Debug.Log("GCloudVoice_C# API: GetBGMPlayState");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetBGMPlayState();
			//Debug.Log("GCloudVoice_C# API: GCloudVoice_GetBGMPlayState nRet = "+ nRet);
			return nRet;

		}

		public override int EnableNativeBGMPlay(bool bEnable)
		{
			Debug.Log("GCloudVoice_C# API: GetVoiceIdentify");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_EnableNativeBGMPlay(bEnable);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_EnableNativeBGMPlay nRet = "+ nRet);
			return nRet;

		}

		public override int SetBitRate(int bitrate)
		{
			Debug.Log("GCloudVoice_C# API: SetBitRate");
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_SetBitRate(bitrate);
			Debug.Log("GCloudVoice_C# API: GCloudVoice_SetBitRate nRet = "+ nRet);
			return nRet;

		}

		public override int GetHwState()
		{
			
			
			if (!bInit)
			{
				return (int)GCloudVoiceErr.GCLOUD_VOICE_NEED_INIT;
			}
			
			int nRet = GCloudVoice_GetHwState();
			
			return nRet;

		}






		///////////////////////////////////////////////////////////////////////////////
		
	}
}//end namespace
