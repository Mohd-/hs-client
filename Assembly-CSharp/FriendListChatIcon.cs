using System;
using System.Collections.Generic;
using System.Linq;
using bgs;
using UnityEngine;

// Token: 0x02000634 RID: 1588
public class FriendListChatIcon : MonoBehaviour
{
	// Token: 0x06004515 RID: 17685 RVA: 0x0014BC7B File Offset: 0x00149E7B
	public BnetPlayer GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x06004516 RID: 17686 RVA: 0x0014BC83 File Offset: 0x00149E83
	public bool SetPlayer(BnetPlayer player)
	{
		if (this.m_player == player)
		{
			return false;
		}
		this.m_player = player;
		this.UpdateIcon();
		return true;
	}

	// Token: 0x06004517 RID: 17687 RVA: 0x0014BCA4 File Offset: 0x00149EA4
	public void UpdateIcon()
	{
		if (this.m_player == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		List<BnetWhisper> whispersWithPlayer = BnetWhisperMgr.Get().GetWhispersWithPlayer(this.m_player);
		if (whispersWithPlayer == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (WhisperUtil.IsSpeaker(BnetPresenceMgr.Get().GetMyPlayer(), whispersWithPlayer[whispersWithPlayer.Count - 1]))
		{
			base.gameObject.SetActive(false);
			return;
		}
		PlayerChatInfo playerChatInfo = ChatMgr.Get().GetPlayerChatInfo(this.m_player);
		if (playerChatInfo != null)
		{
			BnetWhisper bnetWhisper = Enumerable.LastOrDefault<BnetWhisper>(whispersWithPlayer, (BnetWhisper whisper) => WhisperUtil.IsSpeaker(this.m_player, whisper));
			if (bnetWhisper == playerChatInfo.GetLastSeenWhisper())
			{
				base.gameObject.SetActive(false);
				return;
			}
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x04002BF4 RID: 11252
	private BnetPlayer m_player;
}
