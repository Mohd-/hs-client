using System;
using bgs;
using PegasusShared;

// Token: 0x0200005E RID: 94
public static class BnetUtils
{
	// Token: 0x06000541 RID: 1345 RVA: 0x0001419C File Offset: 0x0001239C
	public static BnetPlayer GetPlayer(BnetGameAccountId id)
	{
		if (id == null)
		{
			return null;
		}
		BnetPlayer bnetPlayer = BnetNearbyPlayerMgr.Get().FindNearbyStranger(id);
		if (bnetPlayer == null)
		{
			bnetPlayer = BnetPresenceMgr.Get().GetPlayer(id);
		}
		return bnetPlayer;
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x000141D8 File Offset: 0x000123D8
	public static string GetPlayerBestName(BnetGameAccountId id)
	{
		BnetPlayer player = BnetUtils.GetPlayer(id);
		string text = (player != null) ? player.GetBestName() : null;
		if (string.IsNullOrEmpty(text))
		{
			text = GameStrings.Get("GLOBAL_PLAYER_PLAYER");
		}
		return text;
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00014218 File Offset: 0x00012418
	public static bool HasPlayerBestNamePresence(BnetGameAccountId id)
	{
		BnetPlayer player = BnetUtils.GetPlayer(id);
		string text = (player != null) ? player.GetBestName() : null;
		return !string.IsNullOrEmpty(text);
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00014248 File Offset: 0x00012448
	public static string GetInviterBestName(PartyInvite invite)
	{
		if (invite != null && !string.IsNullOrEmpty(invite.InviterName))
		{
			return invite.InviterName;
		}
		BnetPlayer bnetPlayer = (invite != null) ? BnetUtils.GetPlayer(invite.InviterId) : null;
		string text = (bnetPlayer != null) ? bnetPlayer.GetBestName() : null;
		if (string.IsNullOrEmpty(text))
		{
			text = GameStrings.Get("GLOBAL_PLAYER_PLAYER");
		}
		return text;
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x000142B4 File Offset: 0x000124B4
	public static bool CanReceiveChallengeFrom(BnetGameAccountId id)
	{
		return BnetFriendMgr.Get().IsFriend(id) || BnetNearbyPlayerMgr.Get().IsNearbyStranger(id);
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x000142E8 File Offset: 0x000124E8
	public static bool CanReceiveWhisperFrom(BnetGameAccountId id)
	{
		BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
		return !myPlayer.IsBusy() && BnetFriendMgr.Get().IsFriend(id);
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x00014320 File Offset: 0x00012520
	public static BnetGameAccountId CreateGameAccountId(BnetId src)
	{
		BnetGameAccountId bnetGameAccountId = new BnetGameAccountId();
		bnetGameAccountId.SetHi(src.Hi);
		bnetGameAccountId.SetLo(src.Lo);
		return bnetGameAccountId;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x0001434C File Offset: 0x0001254C
	public static PartyId CreatePartyId(BnetId protoEntityId)
	{
		return new PartyId(protoEntityId.Hi, protoEntityId.Lo);
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x00014360 File Offset: 0x00012560
	public static BnetId CreatePegasusBnetId(PartyId partyId)
	{
		return new BnetId
		{
			Hi = partyId.Hi,
			Lo = partyId.Lo
		};
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001438C File Offset: 0x0001258C
	public static BnetId CreatePegasusBnetId(BnetEntityId src)
	{
		return new BnetId
		{
			Hi = src.GetHi(),
			Lo = src.GetLo()
		};
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x000143B8 File Offset: 0x000125B8
	public static string GetNameForProgramId(BnetProgramId programId)
	{
		string nameTag = BnetProgramId.GetNameTag(programId);
		if (nameTag != null)
		{
			return GameStrings.Get(nameTag);
		}
		return null;
	}
}
