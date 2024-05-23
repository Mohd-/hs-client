using System;
using System.IO;

// Token: 0x0200002E RID: 46
public class Log
{
	// Token: 0x0600041A RID: 1050 RVA: 0x00012A6C File Offset: 0x00010C6C
	public static Log Get()
	{
		if (Log.s_instance == null)
		{
			Log.s_instance = new Log();
			Log.s_instance.Initialize();
		}
		return Log.s_instance;
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00012A94 File Offset: 0x00010C94
	public void Load()
	{
		string text = string.Format("{0}/{1}", FileUtils.PersistentDataPath, "log.config");
		if (File.Exists(text))
		{
			this.m_logInfos.Clear();
			this.LoadConfig(text);
		}
		foreach (LogInfo logInfo in this.DEFAULT_LOG_INFOS)
		{
			if (!this.m_logInfos.ContainsKey(logInfo.m_name))
			{
				this.m_logInfos.Add(logInfo.m_name, logInfo);
			}
		}
		Log.ConfigFile.Print("log.config location: " + text, new object[0]);
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00012B38 File Offset: 0x00010D38
	public LogInfo GetLogInfo(string name)
	{
		LogInfo result = null;
		this.m_logInfos.TryGetValue(name, out result);
		return result;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00012B57 File Offset: 0x00010D57
	private void Initialize()
	{
		this.Load();
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00012B60 File Offset: 0x00010D60
	private void LoadConfig(string path)
	{
		ConfigFile configFile = new ConfigFile();
		if (!configFile.LightLoad(path))
		{
			return;
		}
		foreach (ConfigFile.Line line in configFile.GetLines())
		{
			string sectionName = line.m_sectionName;
			string lineKey = line.m_lineKey;
			string value = line.m_value;
			LogInfo logInfo;
			if (!this.m_logInfos.TryGetValue(sectionName, out logInfo))
			{
				logInfo = new LogInfo
				{
					m_name = sectionName
				};
				this.m_logInfos.Add(logInfo.m_name, logInfo);
			}
			if (lineKey.Equals("ConsolePrinting", 5))
			{
				logInfo.m_consolePrinting = GeneralUtils.ForceBool(value);
			}
			else if (lineKey.Equals("ScreenPrinting", 5))
			{
				logInfo.m_screenPrinting = GeneralUtils.ForceBool(value);
			}
			else if (lineKey.Equals("FilePrinting", 5))
			{
				logInfo.m_filePrinting = GeneralUtils.ForceBool(value);
			}
			else if (lineKey.Equals("MinLevel", 5))
			{
				try
				{
					LogLevel @enum = EnumUtils.GetEnum<LogLevel>(value, 5);
					logInfo.m_minLevel = @enum;
				}
				catch (ArgumentException)
				{
				}
			}
			else if (lineKey.Equals("DefaultLevel", 5))
			{
				try
				{
					LogLevel enum2 = EnumUtils.GetEnum<LogLevel>(value, 5);
					logInfo.m_defaultLevel = enum2;
				}
				catch (ArgumentException)
				{
				}
			}
			else if (lineKey.Equals("Verbose", 5))
			{
				logInfo.m_verbose = GeneralUtils.ForceBool(value);
			}
		}
	}

	// Token: 0x04000196 RID: 406
	private const string CONFIG_FILE_NAME = "log.config";

	// Token: 0x04000197 RID: 407
	public static Logger Bob = new Logger("Bob");

	// Token: 0x04000198 RID: 408
	public static Logger Mike = new Logger("Mike");

	// Token: 0x04000199 RID: 409
	public static Logger Brian = new Logger("Brian");

	// Token: 0x0400019A RID: 410
	public static Logger Jay = new Logger("Jay");

	// Token: 0x0400019B RID: 411
	public static Logger Rachelle = new Logger("Rachelle");

	// Token: 0x0400019C RID: 412
	public static Logger Ben = new Logger("Ben");

	// Token: 0x0400019D RID: 413
	public static Logger Derek = new Logger("Derek");

	// Token: 0x0400019E RID: 414
	public static Logger Kyle = new Logger("Kyle");

	// Token: 0x0400019F RID: 415
	public static Logger Cameron = new Logger("Cameron");

	// Token: 0x040001A0 RID: 416
	public static Logger Ryan = new Logger("Ryan");

	// Token: 0x040001A1 RID: 417
	public static Logger JMac = new Logger("JMac");

	// Token: 0x040001A2 RID: 418
	public static Logger Yim = new Logger("Yim");

	// Token: 0x040001A3 RID: 419
	public static Logger Becca = new Logger("Becca");

	// Token: 0x040001A4 RID: 420
	public static Logger Henry = new Logger("Henry");

	// Token: 0x040001A5 RID: 421
	public static Logger MikeH = new Logger("MikeH");

	// Token: 0x040001A6 RID: 422
	public static Logger Robin = new Logger("Robin");

	// Token: 0x040001A7 RID: 423
	public static Logger Josh = new Logger("Josh");

	// Token: 0x040001A8 RID: 424
	public static Logger BattleNet = new Logger("BattleNet");

	// Token: 0x040001A9 RID: 425
	public static Logger Net = new Logger("Net");

	// Token: 0x040001AA RID: 426
	public static Logger Packets = new Logger("Packet");

	// Token: 0x040001AB RID: 427
	public static Logger Power = new Logger("Power");

	// Token: 0x040001AC RID: 428
	public static Logger Zone = new Logger("Zone");

	// Token: 0x040001AD RID: 429
	public static Logger Asset = new Logger("Asset");

	// Token: 0x040001AE RID: 430
	public static Logger Sound = new Logger("Sound");

	// Token: 0x040001AF RID: 431
	public static Logger HealthyGaming = new Logger("HealthyGaming");

	// Token: 0x040001B0 RID: 432
	public static Logger FaceDownCard = new Logger("FaceDownCard");

	// Token: 0x040001B1 RID: 433
	public static Logger LoadingScreen = new Logger("LoadingScreen");

	// Token: 0x040001B2 RID: 434
	public static Logger MissingAssets = new Logger("MissingAssets");

	// Token: 0x040001B3 RID: 435
	public static Logger UpdateManager = new Logger("UpdateManager");

	// Token: 0x040001B4 RID: 436
	public static Logger GameMgr = new Logger("GameMgr");

	// Token: 0x040001B5 RID: 437
	public static Logger CardbackMgr = new Logger("CardbackMgr");

	// Token: 0x040001B6 RID: 438
	public static Logger Reset = new Logger("Reset");

	// Token: 0x040001B7 RID: 439
	public static Logger DbfXml = new Logger("DbfXml");

	// Token: 0x040001B8 RID: 440
	public static Logger BIReport = new Logger("BIReport");

	// Token: 0x040001B9 RID: 441
	public static Logger Downloader = new Logger("Downloader");

	// Token: 0x040001BA RID: 442
	public static Logger PlayErrors = new Logger("PlayErrors");

	// Token: 0x040001BB RID: 443
	public static Logger Hand = new Logger("Hand");

	// Token: 0x040001BC RID: 444
	public static Logger ConfigFile = new Logger("ConfigFile");

	// Token: 0x040001BD RID: 445
	public static Logger DeviceEmulation = new Logger("DeviceEmulation");

	// Token: 0x040001BE RID: 446
	public static Logger Spectator = new Logger("Spectator");

	// Token: 0x040001BF RID: 447
	public static Logger Party = new Logger("Party");

	// Token: 0x040001C0 RID: 448
	public static Logger FullScreenFX = new Logger("FullScreenFX");

	// Token: 0x040001C1 RID: 449
	public static Logger InnKeepersSpecial = new Logger("InnKeepersSpecial");

	// Token: 0x040001C2 RID: 450
	public static Logger EventTiming = new Logger("EventTiming");

	// Token: 0x040001C3 RID: 451
	public static Logger Arena = new Logger("Arena");

	// Token: 0x040001C4 RID: 452
	public static Logger EndOfGame = new Logger("EndOfGame");

	// Token: 0x040001C5 RID: 453
	public static Logger Achievements = new Logger("Achievements");

	// Token: 0x040001C6 RID: 454
	public static Logger AdTracking = new Logger("AdTracking");

	// Token: 0x040001C7 RID: 455
	public static Logger ClientRequestManager = new Logger("ClientRequestManager");

	// Token: 0x040001C8 RID: 456
	public static Logger BugReporter = new Logger("BugReporter");

	// Token: 0x040001C9 RID: 457
	public static Logger Graphics = new Logger("Graphics");

	// Token: 0x040001CA RID: 458
	public static Logger Store = new Logger("Store");

	// Token: 0x040001CB RID: 459
	public static Logger DeckTray = new Logger("DeckTray");

	// Token: 0x040001CC RID: 460
	public static Logger ChangedCards = new Logger("ChangedCards");

	// Token: 0x040001CD RID: 461
	public static Logger DeckRuleset = new Logger("DeckRuleset");

	// Token: 0x040001CE RID: 462
	public static Logger DeckHelper = new Logger("DeckHelper");

	// Token: 0x040001CF RID: 463
	public static Logger UserAttention = new Logger("UserAttention");

	// Token: 0x040001D0 RID: 464
	private readonly LogInfo[] DEFAULT_LOG_INFOS = new LogInfo[0];

	// Token: 0x040001D1 RID: 465
	private static Log s_instance;

	// Token: 0x040001D2 RID: 466
	private Map<string, LogInfo> m_logInfos = new Map<string, LogInfo>();
}
