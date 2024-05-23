using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class TB09_ShadowTowers : MissionEntity
{
	// Token: 0x060028DD RID: 10461 RVA: 0x000C6850 File Offset: 0x000C4A50
	// Note: this type is marked as 'beforefieldinit'.
	static TB09_ShadowTowers()
	{
		Dictionary<int, string> dictionary = new Dictionary<int, string>();
		dictionary.Add(1, "TB_SHADOWTOWERS_SHADOWSPAWNED");
		dictionary.Add(2, "TB_SHADOWTOWERS_SHADOWSPAWNED");
		dictionary.Add(3, "TB_SHADOWTOWERS_ADJACENTMINIONS");
		dictionary.Add(4, "TB_SHADOWTOWERS_SHADOWSPAWNEDNEXT");
		TB09_ShadowTowers.minionMsgs = dictionary;
	}

	// Token: 0x060028DE RID: 10462 RVA: 0x000C6899 File Offset: 0x000C4A99
	public override void PreloadAssets()
	{
		base.PreloadSound("tutorial_mission_hero_coin_mouse_away");
	}

	// Token: 0x060028DF RID: 10463 RVA: 0x000C68A8 File Offset: 0x000C4AA8
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		if (this.seen.Contains(missionEvent))
		{
			yield break;
		}
		this.seen.Add(missionEvent);
		this.doPopup = false;
		this.doLeftArrow = false;
		this.doUpArrow = false;
		this.doDownArrow = false;
		switch (missionEvent)
		{
		case 1:
		case 2:
			if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
			{
				yield break;
			}
			this.doPopup = true;
			this.textID = TB09_ShadowTowers.minionMsgs[missionEvent];
			this.doLeftArrow = true;
			this.delayTime = 3f;
			this.popUpPos.x = 46f;
			this.popUpPos.z = -9f;
			this.popupDuration = 4f;
			if (UniversalInputManager.UsePhoneUI)
			{
			}
			break;
		case 3:
		case 4:
			this.doPopup = true;
			this.textID = TB09_ShadowTowers.minionMsgs[missionEvent];
			this.delayTime = 0f;
			this.popUpPos.x = 0f;
			this.popUpPos.z = 20f;
			this.popupDuration = 3f;
			if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
			{
				this.popUpPos.z = -11f;
				if (missionEvent == 3)
				{
					this.doDownArrow = true;
				}
			}
			else if (missionEvent == 3)
			{
				this.doUpArrow = true;
			}
			if (UniversalInputManager.UsePhoneUI)
			{
			}
			break;
		default:
			if (missionEvent == 11)
			{
				NotificationManager.Get().DestroyNotification(this.ShadowTowerPopup, 0f);
				this.doPopup = false;
			}
			break;
		}
		if (this.doPopup)
		{
			yield return new WaitForSeconds(this.delayTime);
			float popupScale = 1.5f;
			this.ShadowTowerPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * popupScale, GameStrings.Get(this.textID), false);
			if (this.doLeftArrow)
			{
				this.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
			}
			if (this.doUpArrow)
			{
				this.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
			}
			if (this.doDownArrow)
			{
				this.ShadowTowerPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
			}
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.ShadowTowerPopup, this.popupDuration);
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(5f);
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x04001807 RID: 6151
	private Notification ShadowTowerPopup;

	// Token: 0x04001808 RID: 6152
	private Notification MinionPopup;

	// Token: 0x04001809 RID: 6153
	private Vector3 popUpPos;

	// Token: 0x0400180A RID: 6154
	private string textID;

	// Token: 0x0400180B RID: 6155
	private bool doPopup;

	// Token: 0x0400180C RID: 6156
	private bool doLeftArrow;

	// Token: 0x0400180D RID: 6157
	private bool doUpArrow;

	// Token: 0x0400180E RID: 6158
	private bool doDownArrow;

	// Token: 0x0400180F RID: 6159
	private float delayTime;

	// Token: 0x04001810 RID: 6160
	private float popupDuration;

	// Token: 0x04001811 RID: 6161
	private HashSet<int> seen = new HashSet<int>();

	// Token: 0x04001812 RID: 6162
	private static readonly Dictionary<int, string> minionMsgs;
}
