using System;
using System.Diagnostics;
using System.IO;

// Token: 0x020008A0 RID: 2208
public class BattleNetClient
{
	// Token: 0x1700064A RID: 1610
	// (get) Token: 0x060053F7 RID: 21495 RVA: 0x00191C13 File Offset: 0x0018FE13
	public static bool needsToRun
	{
		get
		{
			return BattleNetClient.usedOnThisPlatform && !BattleNetClient.launchedHearthstone;
		}
	}

	// Token: 0x060053F8 RID: 21496 RVA: 0x00191C2C File Offset: 0x0018FE2C
	public static void quitHearthstoneAndRun()
	{
		Blizzard.Log.Warning("Hearthstone was not run from Battle.net Client");
		if (!BattleNetClient.bootstrapper.Exists)
		{
			Blizzard.Log.Warning("Hearthstone could not find Battle.net client");
			Error.AddFatalLoc("GLUE_CANNOT_FIND_BATTLENET_CLIENT", new object[0]);
			return;
		}
		try
		{
			Process process = new Process
			{
				StartInfo = 
				{
					UseShellExecute = false,
					FileName = BattleNetClient.bootstrapper.FullName,
					Arguments = "-uid hs_beta"
				},
				EnableRaisingEvents = true
			};
			process.Start();
			Blizzard.Log.Warning("Hearthstone ran Battle.net Client.  Exiting.");
			ApplicationMgr.Get().Exit();
		}
		catch (Exception ex)
		{
			Error.AddFatalLoc("GLUE_CANNOT_RUN_BATTLENET_CLIENT", new object[0]);
			Blizzard.Log.Warning("Hearthstone could not launch Battle.net client: {0}", new object[]
			{
				ex.Message
			});
		}
	}

	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x060053F9 RID: 21497 RVA: 0x00191D0C File Offset: 0x0018FF0C
	private static bool usedOnThisPlatform
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700064C RID: 1612
	// (get) Token: 0x060053FA RID: 21498 RVA: 0x00191D10 File Offset: 0x0018FF10
	private static bool launchedHearthstone
	{
		get
		{
			foreach (string text in Environment.GetCommandLineArgs())
			{
				if (text.Equals("-launch", 5))
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x1700064D RID: 1613
	// (get) Token: 0x060053FB RID: 21499 RVA: 0x00191D4F File Offset: 0x0018FF4F
	private static FileInfo bootstrapper
	{
		get
		{
			return new FileInfo("Hearthstone Beta Launcher.exe");
		}
	}
}
