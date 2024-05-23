using System;
using bgs;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class PlayerIcon : PegUIElement
{
	// Token: 0x06003F1A RID: 16154 RVA: 0x00133143 File Offset: 0x00131343
	public void Hide()
	{
		this.m_hidden = true;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003F1B RID: 16155 RVA: 0x00133158 File Offset: 0x00131358
	public void Show()
	{
		this.m_hidden = false;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003F1C RID: 16156 RVA: 0x0013316D File Offset: 0x0013136D
	public BnetPlayer GetPlayer()
	{
		return this.m_player;
	}

	// Token: 0x06003F1D RID: 16157 RVA: 0x00133175 File Offset: 0x00131375
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

	// Token: 0x06003F1E RID: 16158 RVA: 0x00133194 File Offset: 0x00131394
	public void UpdateIcon()
	{
		if (this.m_player.IsOnline() && this.m_player.GetBestProgramId() != BnetProgramId.PHOENIX)
		{
			if (!this.m_hidden)
			{
				base.gameObject.SetActive(true);
			}
			BnetProgramId bestProgramId = this.m_player.GetBestProgramId();
			this.m_OnlinePortrait.SetProgramId(bestProgramId);
		}
		else
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x0400285D RID: 10333
	public GameObject m_OfflineIcon;

	// Token: 0x0400285E RID: 10334
	public GameObject m_OnlineIcon;

	// Token: 0x0400285F RID: 10335
	public PlayerPortrait m_OnlinePortrait;

	// Token: 0x04002860 RID: 10336
	private bool m_hidden;

	// Token: 0x04002861 RID: 10337
	private BnetPlayer m_player;
}
