using System;
using bgs;

// Token: 0x02000511 RID: 1297
public static class WhisperUtil
{
	// Token: 0x06003BF2 RID: 15346 RVA: 0x00121D61 File Offset: 0x0011FF61
	public static BnetPlayer GetSpeaker(BnetWhisper whisper)
	{
		return BnetUtils.GetPlayer(whisper.GetSpeakerId());
	}

	// Token: 0x06003BF3 RID: 15347 RVA: 0x00121D6E File Offset: 0x0011FF6E
	public static BnetPlayer GetReceiver(BnetWhisper whisper)
	{
		return BnetUtils.GetPlayer(whisper.GetReceiverId());
	}

	// Token: 0x06003BF4 RID: 15348 RVA: 0x00121D7C File Offset: 0x0011FF7C
	public static BnetPlayer GetTheirPlayer(BnetWhisper whisper)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (myPlayer == null)
		{
			return null;
		}
		BnetPlayer speaker = WhisperUtil.GetSpeaker(whisper);
		BnetPlayer receiver = WhisperUtil.GetReceiver(whisper);
		if (myPlayer == speaker)
		{
			return receiver;
		}
		if (myPlayer == receiver)
		{
			return speaker;
		}
		return null;
	}

	// Token: 0x06003BF5 RID: 15349 RVA: 0x00121DC0 File Offset: 0x0011FFC0
	public static BnetGameAccountId GetTheirGameAccountId(BnetWhisper whisper)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		if (myPlayer == null)
		{
			return null;
		}
		if (myPlayer.HasGameAccount(whisper.GetSpeakerId()))
		{
			return whisper.GetReceiverId();
		}
		if (myPlayer.HasGameAccount(whisper.GetReceiverId()))
		{
			return whisper.GetSpeakerId();
		}
		return null;
	}

	// Token: 0x06003BF6 RID: 15350 RVA: 0x00121E14 File Offset: 0x00120014
	public static bool IsDisplayable(BnetWhisper whisper)
	{
		BnetPlayer speaker = WhisperUtil.GetSpeaker(whisper);
		BnetPlayer receiver = WhisperUtil.GetReceiver(whisper);
		return speaker != null && speaker.IsDisplayable() && receiver != null && receiver.IsDisplayable();
	}

	// Token: 0x06003BF7 RID: 15351 RVA: 0x00121E5A File Offset: 0x0012005A
	public static bool IsSpeaker(BnetPlayer player, BnetWhisper whisper)
	{
		return player != null && player.HasGameAccount(whisper.GetSpeakerId());
	}

	// Token: 0x06003BF8 RID: 15352 RVA: 0x00121E70 File Offset: 0x00120070
	public static bool IsReceiver(BnetPlayer player, BnetWhisper whisper)
	{
		return player != null && player.HasGameAccount(whisper.GetReceiverId());
	}

	// Token: 0x06003BF9 RID: 15353 RVA: 0x00121E88 File Offset: 0x00120088
	public static bool IsSpeakerOrReceiver(BnetPlayer player, BnetWhisper whisper)
	{
		return player != null && (player.HasGameAccount(whisper.GetSpeakerId()) || player.HasGameAccount(whisper.GetReceiverId()));
	}
}
