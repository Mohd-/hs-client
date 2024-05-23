using System;
using System.Collections;
using System.Collections.Generic;
using bgs;
using UnityEngine;
using WTCG.BI;

// Token: 0x020000B3 RID: 179
public class BIReport : MonoBehaviour
{
	// Token: 0x0600086C RID: 2156 RVA: 0x00020B84 File Offset: 0x0001ED84
	private void Awake()
	{
		BIReport.s_instance = this;
		this.GenerateSessionID();
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00020B92 File Offset: 0x0001ED92
	private void OnDestroy()
	{
		BIReport.s_instance = null;
	}

	// Token: 0x0600086E RID: 2158 RVA: 0x00020B9A File Offset: 0x0001ED9A
	public static BIReport Get()
	{
		return BIReport.s_instance;
	}

	// Token: 0x0600086F RID: 2159 RVA: 0x00020BA4 File Offset: 0x0001EDA4
	public void Report_DataOnlyPatching(DataOnlyPatching.Status status, Locale locale, int currentBuild, int newBuild)
	{
		DataOnlyPatching.Locale locale_ = DataOnlyPatching.Locale.UnknownLocale;
		switch (locale)
		{
		case Locale.enUS:
			locale_ = DataOnlyPatching.Locale.enUS;
			break;
		case Locale.enGB:
			locale_ = DataOnlyPatching.Locale.enGB;
			break;
		case Locale.frFR:
			locale_ = DataOnlyPatching.Locale.frFR;
			break;
		case Locale.deDE:
			locale_ = DataOnlyPatching.Locale.deDE;
			break;
		case Locale.koKR:
			locale_ = DataOnlyPatching.Locale.koKR;
			break;
		case Locale.esES:
			locale_ = DataOnlyPatching.Locale.esES;
			break;
		case Locale.esMX:
			locale_ = DataOnlyPatching.Locale.esMX;
			break;
		case Locale.ruRU:
			locale_ = DataOnlyPatching.Locale.ruRU;
			break;
		case Locale.zhTW:
			locale_ = DataOnlyPatching.Locale.zhTW;
			break;
		case Locale.zhCN:
			locale_ = DataOnlyPatching.Locale.zhCN;
			break;
		case Locale.itIT:
			locale_ = DataOnlyPatching.Locale.itIT;
			break;
		case Locale.ptBR:
			locale_ = DataOnlyPatching.Locale.ptBR;
			break;
		case Locale.plPL:
			locale_ = DataOnlyPatching.Locale.plPL;
			break;
		case Locale.jaJP:
			locale_ = DataOnlyPatching.Locale.Locale15;
			break;
		case Locale.thTH:
			locale_ = DataOnlyPatching.Locale.Locale16;
			break;
		}
		DataOnlyPatching.Platform platform_ = DataOnlyPatching.Platform.UnknownPlatform;
		switch (Application.platform)
		{
		case 0:
			platform_ = DataOnlyPatching.Platform.Mac;
			break;
		case 1:
			platform_ = DataOnlyPatching.Platform.Mac;
			break;
		case 2:
			platform_ = DataOnlyPatching.Platform.Windows;
			break;
		case 7:
			platform_ = DataOnlyPatching.Platform.Windows;
			break;
		case 8:
			platform_ = ((!UniversalInputManager.UsePhoneUI) ? DataOnlyPatching.Platform.iPad : DataOnlyPatching.Platform.iPhone);
			break;
		case 11:
			platform_ = ((!UniversalInputManager.UsePhoneUI) ? DataOnlyPatching.Platform.Android_Tablet : DataOnlyPatching.Platform.Android_Phone);
			break;
		}
		DataOnlyPatching dataOnlyPatching = new DataOnlyPatching();
		dataOnlyPatching.Status_ = status;
		dataOnlyPatching.Locale_ = locale_;
		dataOnlyPatching.Platform_ = platform_;
		dataOnlyPatching.BnetRegion_ = BattleNet.GetCurrentRegion();
		dataOnlyPatching.GameAccountId_ = BattleNet.GetMyGameAccountId().lo;
		dataOnlyPatching.CurrentBuild_ = currentBuild;
		dataOnlyPatching.NewBuild_ = newBuild;
		dataOnlyPatching.Locale_ = locale_;
		dataOnlyPatching.Platform_ = platform_;
		dataOnlyPatching.BnetRegion_ = BattleNet.GetCurrentRegion();
		dataOnlyPatching.GameAccountId_ = BattleNet.GetMyGameAccountId().lo;
		dataOnlyPatching.CurrentBuild_ = currentBuild;
		dataOnlyPatching.NewBuild_ = newBuild;
		dataOnlyPatching.SessionId_ = BIReport.s_sessionId;
		dataOnlyPatching.DeviceUniqueIdentifier_ = SystemInfo.deviceUniqueIdentifier;
		Log.BIReport.Print("Report " + dataOnlyPatching.ToString(), new object[0]);
		base.StartCoroutine(this.Report(ProtobufUtil.ToByteArray(dataOnlyPatching), true));
	}

	// Token: 0x06000870 RID: 2160 RVA: 0x00020DD8 File Offset: 0x0001EFD8
	public void TelemetryInfo(BIReport.TelemetryEvent telemetryEvent, int errorCode = 0, string message = null, constants.BnetRegion overrideBnetRegion = -1)
	{
		this.Report_Telemetry(Telemetry.Level.LEVEL_INFO, telemetryEvent, errorCode, message, overrideBnetRegion);
	}

	// Token: 0x06000871 RID: 2161 RVA: 0x00020DE6 File Offset: 0x0001EFE6
	public void TelemetryWarn(BIReport.TelemetryEvent telemetryEvent, int errorCode = 0, string message = null, constants.BnetRegion overrideBnetRegion = -1)
	{
		this.Report_Telemetry(Telemetry.Level.LEVEL_WARN, telemetryEvent, errorCode, message, overrideBnetRegion);
	}

	// Token: 0x06000872 RID: 2162 RVA: 0x00020DF4 File Offset: 0x0001EFF4
	public void TelemetryError(BIReport.TelemetryEvent telemetryEvent, int errorCode = 0, string message = null, constants.BnetRegion overrideBnetRegion = -1)
	{
		this.Report_Telemetry(Telemetry.Level.LEVEL_ERROR, telemetryEvent, errorCode, message, overrideBnetRegion);
	}

	// Token: 0x06000873 RID: 2163 RVA: 0x00020E02 File Offset: 0x0001F002
	public void Report_Telemetry(Telemetry.Level level, BIReport.TelemetryEvent telemetryEvent)
	{
		this.Report_Telemetry(level, telemetryEvent, 0, null);
	}

	// Token: 0x06000874 RID: 2164 RVA: 0x00020E0E File Offset: 0x0001F00E
	public void Report_Telemetry(Telemetry.Level level, BIReport.TelemetryEvent telemetryEvent, constants.BnetRegion overrideBnetRegion)
	{
		this.Report_Telemetry(level, telemetryEvent, 0, null, overrideBnetRegion);
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00020E1B File Offset: 0x0001F01B
	public void Report_Telemetry(Telemetry.Level level, BIReport.TelemetryEvent telemetryEvent, string message)
	{
		this.Report_Telemetry(level, telemetryEvent, 0, message, -1);
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00020E28 File Offset: 0x0001F028
	public void Report_Telemetry(Telemetry.Level level, BIReport.TelemetryEvent telemetryEvent, int errorCode, string message)
	{
		this.Report_Telemetry(level, telemetryEvent, errorCode, message, -1);
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00020E38 File Offset: 0x0001F038
	public void Report_Telemetry(Telemetry.Level level, BIReport.TelemetryEvent telemetryEvent, int errorCode, string message, constants.BnetRegion overrideBnetRegion)
	{
		if (BIReport.s_sessionId == null)
		{
			Log.BIReport.Print("ERROR: Sending report while s_sessionId == NULL", new object[0]);
		}
		Telemetry.Locale locale_ = Telemetry.Locale.LOCALE_UNKNOWN;
		switch (Localization.GetLocale())
		{
		case Locale.enUS:
			locale_ = Telemetry.Locale.LOCALE_ENUS;
			break;
		case Locale.enGB:
			locale_ = Telemetry.Locale.LOCALE_ENGB;
			break;
		case Locale.frFR:
			locale_ = Telemetry.Locale.LOCALE_FRFR;
			break;
		case Locale.deDE:
			locale_ = Telemetry.Locale.LOCALE_DEDE;
			break;
		case Locale.koKR:
			locale_ = Telemetry.Locale.LOCALE_KOKR;
			break;
		case Locale.esES:
			locale_ = Telemetry.Locale.LOCALE_ESES;
			break;
		case Locale.esMX:
			locale_ = Telemetry.Locale.LOCALE_ESMX;
			break;
		case Locale.ruRU:
			locale_ = Telemetry.Locale.LOCALE_RURU;
			break;
		case Locale.zhTW:
			locale_ = Telemetry.Locale.LOCALE_ZHTW;
			break;
		case Locale.zhCN:
			locale_ = Telemetry.Locale.LOCALE_ZHCN;
			break;
		case Locale.itIT:
			locale_ = Telemetry.Locale.LOCALE_ITIT;
			break;
		case Locale.ptBR:
			locale_ = Telemetry.Locale.LOCALE_PTBR;
			break;
		case Locale.plPL:
			locale_ = Telemetry.Locale.LOCALE_PLPL;
			break;
		case Locale.jaJP:
			locale_ = Telemetry.Locale.LOCALE_15;
			break;
		case Locale.thTH:
			locale_ = Telemetry.Locale.LOCALE_16;
			break;
		}
		Telemetry.Platform platform_ = Telemetry.Platform.PLATFORM_UNKNOWN;
		switch (Application.platform)
		{
		case 0:
			platform_ = Telemetry.Platform.PLATFORM_MAC;
			break;
		case 1:
			platform_ = Telemetry.Platform.PLATFORM_MAC;
			break;
		case 2:
			platform_ = Telemetry.Platform.PLATFORM_PC;
			break;
		case 7:
			platform_ = Telemetry.Platform.PLATFORM_PC;
			break;
		case 8:
			platform_ = Telemetry.Platform.PLATFORM_IOS;
			break;
		case 11:
			platform_ = Telemetry.Platform.PLATFORM_ANDROID;
			break;
		}
		Telemetry.ScreenUI screenUI_;
		if (Application.platform == 8 || Application.platform == 11)
		{
			screenUI_ = ((!UniversalInputManager.UsePhoneUI) ? Telemetry.ScreenUI.SCREENUI_TABLET : Telemetry.ScreenUI.SCREENUI_PHONE);
		}
		else
		{
			screenUI_ = Telemetry.ScreenUI.SCREENUI_DESKTOP;
		}
		Telemetry telemetry = new Telemetry();
		telemetry.Time_ = (long)BIReport.ConvertDateTimeToUnixEpoch(DateTime.Now);
		telemetry.Level_ = level;
		telemetry.Version_ = string.Format("{0}.{1}", "5.0", 12574);
		telemetry.Locale_ = locale_;
		telemetry.Platform_ = platform_;
		telemetry.Os_ = SystemInfo.operatingSystem;
		telemetry.ScreenUI_ = screenUI_;
		telemetry.Store_ = Telemetry.Store.STORE_BLIZZARD;
		telemetry.SessionId_ = BIReport.s_sessionId;
		telemetry.DeviceUniqueIdentifier_ = SystemInfo.deviceUniqueIdentifier;
		telemetry.Event_ = (ulong)((long)telemetryEvent);
		if (BattleNet.IsInitialized() && BattleNet.GetCurrentRegion() != -1)
		{
			telemetry.BnetRegion_ = BattleNet.GetCurrentRegion();
			telemetry.GameAccountId_ = BattleNet.GetMyGameAccountId().lo;
		}
		if (overrideBnetRegion != -1)
		{
			telemetry.BnetRegion_ = overrideBnetRegion;
		}
		telemetry.ErrorCode_ = (long)errorCode;
		if (message != null)
		{
			telemetry.Message_ = message;
		}
		Log.BIReport.Print("Report: " + this.TelemetryDataToString(telemetry), new object[0]);
		base.StartCoroutine(this.Report(ProtobufUtil.ToByteArray(telemetry)));
	}

	// Token: 0x06000878 RID: 2168 RVA: 0x000210E0 File Offset: 0x0001F2E0
	public IEnumerator Report(byte[] data)
	{
		yield return base.StartCoroutine(this.Report(data, false));
		yield break;
	}

	// Token: 0x06000879 RID: 2169 RVA: 0x0002110C File Offset: 0x0001F30C
	public IEnumerator Report(byte[] data, bool isDataOnlyPatching)
	{
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers["ir-exchange"] = "biapi";
		string protoMessageType = (!isDataOnlyPatching) ? "WTCG.BI.ClientTelemetry" : "WTCG.BI.Session.DataOnlyPatching";
		headers["irrh-x-proto-message-type"] = protoMessageType;
		WWW www = new WWW("http://iir.blizzard.com:3724/submit/WTCG", data, headers);
		yield return www;
		yield break;
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x0002113C File Offset: 0x0001F33C
	private void GenerateSessionID()
	{
		if (BIReport.s_sessionId != null)
		{
			Log.BIReport.Print("WARNING: Replacing session ID [" + BIReport.s_sessionId + "]", new object[0]);
		}
		string text = SystemInfo.deviceUniqueIdentifier + DateTime.Now.ToFileTimeUtc().ToString();
		Log.BIReport.Print("rawSessionId = " + text, new object[0]);
		BIReport.s_sessionId = Blizzard.Crypto.SHA1.Calc(text);
		Log.BIReport.Print("s_sessionId = " + BIReport.s_sessionId, new object[0]);
	}

	// Token: 0x0600087B RID: 2171 RVA: 0x000211DC File Offset: 0x0001F3DC
	private string TelemetryDataToString(Telemetry data)
	{
		return string.Concat(new object[]
		{
			"Event_ = ",
			(BIReport.TelemetryEvent)data.Event_,
			" Time = ",
			data.Time_,
			" Level = ",
			data.Level_,
			" Version = ",
			data.Version_,
			" Locale = ",
			data.Locale_,
			" Platform = ",
			data.Platform_,
			" OS = ",
			data.Os_,
			" ScreenUI = ",
			data.ScreenUI_,
			" Store = ",
			data.Store_,
			" SessionId = ",
			data.SessionId_,
			" DeviceUniqueIdentifier = ",
			data.DeviceUniqueIdentifier_,
			" BnetRegion_ = ",
			data.BnetRegion_,
			" GameAccountId_ = ",
			data.GameAccountId_,
			" ErrorCode_ = ",
			data.ErrorCode_,
			" Message = ",
			data.Message_
		});
	}

	// Token: 0x0600087C RID: 2172 RVA: 0x00021340 File Offset: 0x0001F540
	public static double ConvertDateTimeToUnixEpoch(DateTime time)
	{
		DateTime dateTime;
		dateTime..ctor(1970, 1, 1);
		return (time - dateTime.ToLocalTime()).TotalSeconds;
	}

	// Token: 0x0400044E RID: 1102
	private const string BIURL = "http://iir.blizzard.com:3724/submit/WTCG";

	// Token: 0x0400044F RID: 1103
	private const string TELEMETRY_PROTO_MESSAGE_TYPE = "WTCG.BI.ClientTelemetry";

	// Token: 0x04000450 RID: 1104
	private const string DOP_PROTO_MESSAGE_TYPE = "WTCG.BI.Session.DataOnlyPatching";

	// Token: 0x04000451 RID: 1105
	private static BIReport s_instance;

	// Token: 0x04000452 RID: 1106
	private static string s_sessionId;

	// Token: 0x020000B6 RID: 182
	public enum TelemetryEvent
	{
		// Token: 0x0400046C RID: 1132
		GAMEPLAY_STUCK_DISCONNECT = 100,
		// Token: 0x0400046D RID: 1133
		EVENT_WEB_LOGIN_TOKEN_PROVIDED = 300,
		// Token: 0x0400046E RID: 1134
		EVENT_WEB_LOGIN_ERROR = 410,
		// Token: 0x0400046F RID: 1135
		EVENT_IGNORABLE_BNET_ERROR = 500,
		// Token: 0x04000470 RID: 1136
		EVENT_FATAL_BNET_ERROR = 600,
		// Token: 0x04000471 RID: 1137
		EVENT_ON_RESET = 700,
		// Token: 0x04000472 RID: 1138
		EVENT_ON_RESET_WITH_LOGIN = 710,
		// Token: 0x04000473 RID: 1139
		EVENT_THIRD_PARTY_PURCHASE_REQUEST = 800,
		// Token: 0x04000474 RID: 1140
		EVENT_THIRD_PARTY_PURCHASE_SUCCESS = 810,
		// Token: 0x04000475 RID: 1141
		EVENT_THIRD_PARTY_PURCHASE_SUCCESS_MALFORMED = 820,
		// Token: 0x04000476 RID: 1142
		EVENT_THIRD_PARTY_PURCHASE_FAILED = 830,
		// Token: 0x04000477 RID: 1143
		EVENT_THIRD_PARTY_PURCHASE_DEFERRED = 840,
		// Token: 0x04000478 RID: 1144
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SIZE = 850,
		// Token: 0x04000479 RID: 1145
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_RECEIVED = 860,
		// Token: 0x0400047A RID: 1146
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED = 870,
		// Token: 0x0400047B RID: 1147
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_FAILED,
		// Token: 0x0400047C RID: 1148
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_RESPONSE,
		// Token: 0x0400047D RID: 1149
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_DANGLING = 880,
		// Token: 0x0400047E RID: 1150
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_SUBMITTED_DANGLING_FAILED,
		// Token: 0x0400047F RID: 1151
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_REQUEST = 890,
		// Token: 0x04000480 RID: 1152
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_REQUEST_NOT_FOUND,
		// Token: 0x04000481 RID: 1153
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_REQUEST_FOUND,
		// Token: 0x04000482 RID: 1154
		EVENT_THIRD_PARTY_PURCHASE_AUTO_CANCEL,
		// Token: 0x04000483 RID: 1155
		EVENT_THIRD_PARTY_PURCHASE_CANCEL_RESPONSE,
		// Token: 0x04000484 RID: 1156
		EVENT_THIRD_PARTY_PURCHASE_RECEIPT_CONSUMED = 899,
		// Token: 0x04000485 RID: 1157
		EVENT_ERROR_NETWORK_UNAVAILABLE,
		// Token: 0x04000486 RID: 1158
		EVENT_ERROR_UNKNOWN_ERROR = 1000
	}
}
