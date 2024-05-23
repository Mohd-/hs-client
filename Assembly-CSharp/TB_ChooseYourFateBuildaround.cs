using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200030F RID: 783
public class TB_ChooseYourFateBuildaround : MissionEntity
{
	// Token: 0x060028D4 RID: 10452 RVA: 0x000C663A File Offset: 0x000C483A
	public override void PreloadAssets()
	{
		base.PreloadSound("tutorial_mission_hero_coin_mouse_away");
	}

	// Token: 0x060028D5 RID: 10453 RVA: 0x000C6648 File Offset: 0x000C4848
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
		this.popUpPos = new Vector3(0f, 0f, 0f);
		switch (missionEvent)
		{
		case 1:
		case 2:
		case 3:
			if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
			{
				this.textID = this.friendlyFate;
				if (UniversalInputManager.UsePhoneUI)
				{
					this.popUpPos.x = -75f;
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
				this.textID = this.opposingFate;
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
		}
		yield break;
	}

	// Token: 0x040017EC RID: 6124
	private Notification ChooseYourFatePopup;

	// Token: 0x040017ED RID: 6125
	private Vector3 popUpPos;

	// Token: 0x040017EE RID: 6126
	private string textID;

	// Token: 0x040017EF RID: 6127
	private string friendlyFate = "TB_PICKYOURFATE_BUILDAROUND_NEWFATE";

	// Token: 0x040017F0 RID: 6128
	private string opposingFate = "TB_PICKYOURFATE_BUILDAROUND_OPPONENTFATE";

	// Token: 0x040017F1 RID: 6129
	private HashSet<int> seen = new HashSet<int>();
}
