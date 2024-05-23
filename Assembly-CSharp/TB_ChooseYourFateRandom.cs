using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000310 RID: 784
public class TB_ChooseYourFateRandom : MissionEntity
{
	// Token: 0x060028D7 RID: 10455 RVA: 0x000C66B0 File Offset: 0x000C48B0
	public override void PreloadAssets()
	{
		base.PreloadSound("tutorial_mission_hero_coin_mouse_away");
	}

	// Token: 0x060028D8 RID: 10456 RVA: 0x000C66C0 File Offset: 0x000C48C0
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
		this.popUpPos = new Vector3(-46f, 0f, 0f);
		int EntityId = 0;
		int friendlyPlayerID = GameState.Get().GetFriendlySidePlayer().GetEntityId();
		int opposingPlayerID = GameState.Get().GetOpposingSidePlayer().GetEntityId();
		if (missionEvent > 1000)
		{
			EntityId = missionEvent - 1000;
			missionEvent -= EntityId;
			if (EntityId == friendlyPlayerID)
			{
				this.popUpPos.z = -44f;
				this.textID = this.newFate;
				if (UniversalInputManager.UsePhoneUI)
				{
					this.popUpPos.x = -51f;
					this.popUpPos.z = -62f;
				}
			}
			if (EntityId == opposingPlayerID)
			{
				this.popUpPos.z = 44f;
				this.textID = this.opponentFate;
				if (UniversalInputManager.UsePhoneUI)
				{
					this.popUpPos.x = -51f;
					this.popUpPos.z = 53f;
				}
			}
		}
		int num = missionEvent;
		switch (num)
		{
		case 1:
		case 2:
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
			if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
			{
				this.textID = this.firstFate;
				if (UniversalInputManager.UsePhoneUI)
				{
					this.popUpPos.x = -77f;
					this.popUpPos.z = 30.5f;
				}
				else
				{
					this.popUpPos.x = -50.5f;
					this.popUpPos.z = 29f;
				}
			}
			else
			{
				this.textID = this.firstOpponenentFate;
				if (UniversalInputManager.UsePhoneUI)
				{
					this.popUpPos.x = -34f;
					this.popUpPos.z = 12f;
				}
				else
				{
					this.popUpPos.x = -7f;
					this.popUpPos.z = 9f;
				}
			}
			yield return new WaitForSeconds(1f);
			this.ChooseYourFatePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.5f, GameStrings.Get(this.textID), false);
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.ChooseYourFatePopup, 3f);
			this.ChooseYourFatePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
			break;
		default:
			if (num == 1000)
			{
				this.ChooseYourFatePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.5f, GameStrings.Get(this.textID), false);
				base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
				NotificationManager.Get().DestroyNotification(this.ChooseYourFatePopup, 5f);
				this.ChooseYourFatePopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			}
			break;
		}
		yield break;
	}

	// Token: 0x040017F2 RID: 6130
	private Notification ChooseYourFatePopup;

	// Token: 0x040017F3 RID: 6131
	private Vector3 popUpPos;

	// Token: 0x040017F4 RID: 6132
	private string textID;

	// Token: 0x040017F5 RID: 6133
	private string newFate = "TB_PICKYOURFATE_RANDOM_NEWFATE";

	// Token: 0x040017F6 RID: 6134
	private string opponentFate = "TB_PICKYOURFATE_RANDOM_OPPONENTFATE";

	// Token: 0x040017F7 RID: 6135
	private string firstFate = "TB_PICKYOURFATE_RANDOM_FIRSTFATE";

	// Token: 0x040017F8 RID: 6136
	private string firstOpponenentFate = "TB_PICKYOURFATE_BUILDAROUND_OPPONENT_FIRSTFATE";

	// Token: 0x040017F9 RID: 6137
	private HashSet<int> seen = new HashSet<int>();
}
