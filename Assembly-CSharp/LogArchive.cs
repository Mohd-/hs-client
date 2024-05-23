using System;
using System.IO;
using UnityEngine;

// Token: 0x02000126 RID: 294
internal class LogArchive
{
	// Token: 0x06000F22 RID: 3874 RVA: 0x00041540 File Offset: 0x0003F740
	public LogArchive()
	{
		string text = FileUtils.PersistentDataPath + "/Logs";
		this.MakeLogPath(text);
		try
		{
			Directory.CreateDirectory(text);
			this.CleanOldLogs(text);
			Application.logMessageReceived += new Application.LogCallback(this.HandleLog);
			Log.Cameron.Print("Logging Unity output to: " + this.m_logPath, new object[0]);
		}
		catch (IOException ex)
		{
			Debug.LogWarning("Failed to write archive logs to: \"" + this.m_logPath + "\"!");
			Debug.LogWarning(ex);
		}
	}

	// Token: 0x06000F24 RID: 3876 RVA: 0x000415F2 File Offset: 0x0003F7F2
	public static void Init()
	{
		LogArchive.Get();
	}

	// Token: 0x06000F25 RID: 3877 RVA: 0x000415FA File Offset: 0x0003F7FA
	public static LogArchive Get()
	{
		if (LogArchive.s_instance == null)
		{
			LogArchive.s_instance = new LogArchive();
		}
		return LogArchive.s_instance;
	}

	// Token: 0x06000F26 RID: 3878 RVA: 0x00041618 File Offset: 0x0003F818
	private void CleanOldLogs(string logFolderPath)
	{
		int num = 5;
		FileInfo[] files = new DirectoryInfo(logFolderPath).GetFiles();
		Array.Sort<FileInfo>(files, (FileInfo a, FileInfo b) => a.LastWriteTime.CompareTo(b.LastWriteTime));
		int num2 = files.Length - (num - 1);
		int num3 = 0;
		while (num3 < num2 && num3 < files.Length)
		{
			files[num3].Delete();
			num3++;
		}
	}

	// Token: 0x06000F27 RID: 3879 RVA: 0x00041684 File Offset: 0x0003F884
	private void MakeLogPath(string logFolderPath)
	{
		if (!string.IsNullOrEmpty(this.m_logPath))
		{
			return;
		}
		string text = Blizzard.Time.Stamp();
		text = text.Replace("-", "_").Replace(" ", "_").Replace(":", "_").Remove(text.Length - 4);
		string text2 = "hearthstone_" + text + ".log";
		this.m_logPath = logFolderPath + "/" + text2;
	}

	// Token: 0x06000F28 RID: 3880 RVA: 0x00041708 File Offset: 0x0003F908
	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		if (this.m_stopLogging)
		{
			return;
		}
		try
		{
			if (this.m_numLinesWritten % 100UL == 0UL)
			{
				FileInfo fileInfo = new FileInfo(this.m_logPath);
				if (fileInfo.Exists && fileInfo.Length > (long)(this.m_maxFileSizeKB * 1024))
				{
					this.m_stopLogging = true;
					using (StreamWriter streamWriter = new StreamWriter(this.m_logPath, true))
					{
						Blizzard.Log.SayToFile(streamWriter, string.Empty, new object[0]);
						Blizzard.Log.SayToFile(streamWriter, string.Empty, new object[0]);
						Blizzard.Log.SayToFile(streamWriter, "==================================================================", new object[0]);
						string format = string.Format("Truncating log, which has reached the size limit of {0}KB", this.m_maxFileSizeKB);
						Blizzard.Log.SayToFile(streamWriter, format, new object[0]);
						Blizzard.Log.SayToFile(streamWriter, "==================================================================\n\n", new object[0]);
					}
					return;
				}
			}
			using (StreamWriter streamWriter2 = new StreamWriter(this.m_logPath, true))
			{
				if (type == null || type == 4)
				{
					Blizzard.Log.SayToFile(streamWriter2, "{0}\n{1}", new object[]
					{
						logString,
						stackTrace
					});
				}
				else
				{
					Blizzard.Log.SayToFile(streamWriter2, "{0}", new object[]
					{
						logString,
						stackTrace
					});
				}
				this.m_numLinesWritten += 1UL;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("LogArchive.HandleLog() - Failed to write \"{0}\". Exception={1}", logString, ex.Message));
		}
	}

	// Token: 0x0400081A RID: 2074
	private static LogArchive s_instance;

	// Token: 0x0400081B RID: 2075
	private string m_logPath;

	// Token: 0x0400081C RID: 2076
	private ulong m_numLinesWritten;

	// Token: 0x0400081D RID: 2077
	private int m_maxFileSizeKB = 5000;

	// Token: 0x0400081E RID: 2078
	private bool m_stopLogging;
}
