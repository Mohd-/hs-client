using System;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class Logger
{
	// Token: 0x060003F6 RID: 1014 RVA: 0x00012149 File Offset: 0x00010349
	public Logger(string name)
	{
		this.m_name = name;
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00012158 File Offset: 0x00010358
	public bool CanPrint(LogTarget target, LogLevel level, bool verbose)
	{
		LogInfo logInfo = Log.Get().GetLogInfo(this.m_name);
		if (logInfo == null)
		{
			return false;
		}
		if (level < logInfo.m_minLevel)
		{
			return false;
		}
		if (verbose && !logInfo.m_verbose)
		{
			return false;
		}
		switch (target)
		{
		case LogTarget.CONSOLE:
			return logInfo.m_consolePrinting;
		case LogTarget.SCREEN:
			return logInfo.m_screenPrinting;
		case LogTarget.FILE:
			return logInfo.m_filePrinting;
		default:
			return false;
		}
	}

	// Token: 0x060003F8 RID: 1016 RVA: 0x000121D0 File Offset: 0x000103D0
	public bool CanPrint()
	{
		LogInfo logInfo = Log.Get().GetLogInfo(this.m_name);
		return logInfo != null && (logInfo.m_consolePrinting || logInfo.m_screenPrinting || logInfo.m_filePrinting);
	}

	// Token: 0x060003F9 RID: 1017 RVA: 0x00012220 File Offset: 0x00010420
	public LogLevel GetDefaultLevel()
	{
		LogInfo logInfo = Log.Get().GetLogInfo(this.m_name);
		if (logInfo == null)
		{
			return LogLevel.Debug;
		}
		return logInfo.m_defaultLevel;
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0001224C File Offset: 0x0001044C
	public bool IsVerbose()
	{
		LogInfo logInfo = Log.Get().GetLogInfo(this.m_name);
		return logInfo != null && logInfo.m_verbose;
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x00012278 File Offset: 0x00010478
	public void Print(string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.Print(defaultLevel, false, format, args);
	}

	// Token: 0x060003FC RID: 1020 RVA: 0x00012296 File Offset: 0x00010496
	public void Print(LogLevel level, string format, params object[] args)
	{
		this.Print(level, false, format, args);
	}

	// Token: 0x060003FD RID: 1021 RVA: 0x000122A4 File Offset: 0x000104A4
	public void Print(bool verbose, string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.Print(defaultLevel, verbose, format, args);
	}

	// Token: 0x060003FE RID: 1022 RVA: 0x000122C4 File Offset: 0x000104C4
	public void Print(LogLevel level, bool verbose, string format, params object[] args)
	{
		string message = GeneralUtils.SafeFormat(format, args);
		this.Print(level, verbose, message);
	}

	// Token: 0x060003FF RID: 1023 RVA: 0x000122E4 File Offset: 0x000104E4
	public void Print(LogLevel level, bool verbose, string message)
	{
		this.FilePrint(level, verbose, message);
		this.ConsolePrint(level, verbose, message);
		this.ScreenPrint(level, verbose, message);
	}

	// Token: 0x06000400 RID: 1024 RVA: 0x0001230C File Offset: 0x0001050C
	public void PrintDebug(string format, params object[] args)
	{
		this.PrintDebug(false, format, args);
	}

	// Token: 0x06000401 RID: 1025 RVA: 0x00012317 File Offset: 0x00010517
	public void PrintDebug(bool verbose, string format, params object[] args)
	{
		this.Print(LogLevel.Debug, verbose, format, args);
	}

	// Token: 0x06000402 RID: 1026 RVA: 0x00012323 File Offset: 0x00010523
	public void PrintInfo(string format, params object[] args)
	{
		this.PrintInfo(false, format, args);
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x0001232E File Offset: 0x0001052E
	public void PrintInfo(bool verbose, string format, params object[] args)
	{
		this.Print(LogLevel.Info, verbose, format, args);
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x0001233A File Offset: 0x0001053A
	public void PrintWarning(string format, params object[] args)
	{
		this.PrintWarning(false, format, args);
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00012345 File Offset: 0x00010545
	public void PrintWarning(bool verbose, string format, params object[] args)
	{
		this.Print(LogLevel.Warning, verbose, format, args);
	}

	// Token: 0x06000406 RID: 1030 RVA: 0x00012351 File Offset: 0x00010551
	public void PrintError(string format, params object[] args)
	{
		this.PrintError(false, format, args);
	}

	// Token: 0x06000407 RID: 1031 RVA: 0x0001235C File Offset: 0x0001055C
	public void PrintError(bool verbose, string format, params object[] args)
	{
		this.Print(LogLevel.Error, verbose, format, args);
	}

	// Token: 0x06000408 RID: 1032 RVA: 0x00012368 File Offset: 0x00010568
	public void FilePrint(string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.FilePrint(defaultLevel, false, format, args);
	}

	// Token: 0x06000409 RID: 1033 RVA: 0x00012386 File Offset: 0x00010586
	public void FilePrint(LogLevel level, string format, params object[] args)
	{
		this.FilePrint(level, false, format, args);
	}

	// Token: 0x0600040A RID: 1034 RVA: 0x00012394 File Offset: 0x00010594
	public void FilePrint(bool verbose, string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.FilePrint(defaultLevel, verbose, format, args);
	}

	// Token: 0x0600040B RID: 1035 RVA: 0x000123B4 File Offset: 0x000105B4
	public void FilePrint(LogLevel level, bool verbose, string format, params object[] args)
	{
		string message = GeneralUtils.SafeFormat(format, args);
		this.FilePrint(level, verbose, message);
	}

	// Token: 0x0600040C RID: 1036 RVA: 0x000123D4 File Offset: 0x000105D4
	public void FilePrint(LogLevel level, bool verbose, string message)
	{
		if (!this.CanPrint(LogTarget.FILE, level, verbose))
		{
			return;
		}
		this.InitFileWriter();
		if (this.m_fileWriter == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		switch (level)
		{
		case LogLevel.Debug:
			stringBuilder.Append("D ");
			break;
		case LogLevel.Info:
			stringBuilder.Append("I ");
			break;
		case LogLevel.Warning:
			stringBuilder.Append("W ");
			break;
		case LogLevel.Error:
			stringBuilder.Append("E ");
			break;
		}
		stringBuilder.Append(DateTime.Now.TimeOfDay.ToString());
		stringBuilder.Append(" ");
		stringBuilder.Append(message);
		this.m_fileWriter.WriteLine(stringBuilder.ToString());
		this.m_fileWriter.Flush();
	}

	// Token: 0x0600040D RID: 1037 RVA: 0x000124B8 File Offset: 0x000106B8
	public void ConsolePrint(string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.ConsolePrint(defaultLevel, false, format, args);
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x000124D6 File Offset: 0x000106D6
	public void ConsolePrint(LogLevel level, string format, params object[] args)
	{
		this.ConsolePrint(level, false, format, args);
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x000124E4 File Offset: 0x000106E4
	public void ConsolePrint(bool verbose, string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.ConsolePrint(defaultLevel, verbose, format, args);
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x00012504 File Offset: 0x00010704
	public void ConsolePrint(LogLevel level, bool verbose, string format, params object[] args)
	{
		string message = GeneralUtils.SafeFormat(format, args);
		this.ConsolePrint(level, verbose, message);
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00012524 File Offset: 0x00010724
	public void ConsolePrint(LogLevel level, bool verbose, string message)
	{
		if (!this.CanPrint(LogTarget.CONSOLE, level, verbose))
		{
			return;
		}
		string text = string.Format("[{0}] {1}", this.m_name, message);
		switch (level)
		{
		case LogLevel.Debug:
		case LogLevel.Info:
			Debug.Log(text);
			break;
		case LogLevel.Warning:
			Debug.LogWarning(text);
			break;
		case LogLevel.Error:
			Debug.LogError(text);
			break;
		}
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00012594 File Offset: 0x00010794
	public void ScreenPrint(string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.ScreenPrint(defaultLevel, false, format, args);
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x000125B2 File Offset: 0x000107B2
	public void ScreenPrint(LogLevel level, string format, params object[] args)
	{
		this.ScreenPrint(level, false, format, args);
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x000125C0 File Offset: 0x000107C0
	public void ScreenPrint(bool verbose, string format, params object[] args)
	{
		LogLevel defaultLevel = this.GetDefaultLevel();
		this.ScreenPrint(defaultLevel, verbose, format, args);
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x000125E0 File Offset: 0x000107E0
	public void ScreenPrint(LogLevel level, bool verbose, string format, params object[] args)
	{
		string message = GeneralUtils.SafeFormat(format, args);
		this.ScreenPrint(level, verbose, message);
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00012600 File Offset: 0x00010800
	public void ScreenPrint(LogLevel level, bool verbose, string message)
	{
		if (!this.CanPrint(LogTarget.SCREEN, level, verbose))
		{
			return;
		}
		if (SceneDebugger.Get() == null)
		{
			return;
		}
		string message2 = string.Format("[{0}] {1}", this.m_name, message);
		SceneDebugger.Get().AddMessage(message2);
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x0001264C File Offset: 0x0001084C
	private void InitFileWriter()
	{
		if (this.m_fileWriterInitialized)
		{
			return;
		}
		this.m_fileWriter = null;
		string text = "Logs";
		if (!Directory.Exists(text))
		{
			try
			{
				Directory.CreateDirectory(text);
			}
			catch (Exception)
			{
			}
		}
		string text2 = string.Format("{0}/{1}.{2}", text, this.m_name, "log");
		try
		{
			this.m_fileWriter = new StreamWriter(new FileStream(text2, 2, 3));
		}
		catch (Exception)
		{
		}
		this.m_fileWriterInitialized = true;
	}

	// Token: 0x04000191 RID: 401
	private const string OUTPUT_DIRECTORY_NAME = "Logs";

	// Token: 0x04000192 RID: 402
	private const string OUTPUT_FILE_EXTENSION = "log";

	// Token: 0x04000193 RID: 403
	private string m_name;

	// Token: 0x04000194 RID: 404
	private StreamWriter m_fileWriter;

	// Token: 0x04000195 RID: 405
	private bool m_fileWriterInitialized;
}
