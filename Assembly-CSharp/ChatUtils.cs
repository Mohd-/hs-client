using System;
using bgs;

// Token: 0x02000552 RID: 1362
public static class ChatUtils
{
	// Token: 0x06003E86 RID: 16006 RVA: 0x0012E828 File Offset: 0x0012CA28
	public static string GetMessage(BnetWhisper whisper)
	{
		string message = whisper.GetMessage();
		return ChatUtils.GetMessage(message);
	}

	// Token: 0x06003E87 RID: 16007 RVA: 0x0012E842 File Offset: 0x0012CA42
	public static string GetMessage(string message)
	{
		if (Localization.GetLocale() == Locale.zhCN)
		{
			return BattleNet.FilterProfanity(message);
		}
		return message;
	}

	// Token: 0x0400280F RID: 10255
	public const int MAX_INPUT_CHARACTERS = 512;

	// Token: 0x04002810 RID: 10256
	public const int MAX_RECENT_WHISPER_RECEIVERS = 10;

	// Token: 0x04002811 RID: 10257
	public const float FRIENDLIST_CHATICON_INACTIVE_SEC = 10f;
}
