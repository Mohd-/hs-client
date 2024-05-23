using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class TB04_DeckBuilding : MissionEntity
{
	// Token: 0x060028CF RID: 10447 RVA: 0x000C65D3 File Offset: 0x000C47D3
	public override void PreloadAssets()
	{
		base.PreloadSound("tutorial_mission_hero_coin_mouse_away");
	}

	// Token: 0x060028D0 RID: 10448 RVA: 0x000C65E0 File Offset: 0x000C47E0
	public override bool ShouldDoAlternateMulliganIntro()
	{
		return true;
	}

	// Token: 0x060028D1 RID: 10449 RVA: 0x000C65E3 File Offset: 0x000C47E3
	public override bool ShouldHandleCoin()
	{
		return false;
	}

	// Token: 0x060028D2 RID: 10450 RVA: 0x000C65E8 File Offset: 0x000C47E8
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		this.popUpPos = new Vector3(0f, 0f, 0f);
		switch (missionEvent)
		{
		case 1:
			if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
			{
				yield break;
			}
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(2f);
			GameState.Get().SetBusy(false);
			this.textID = "TB_DECKBUILDING_FIRSTPICKTHREE";
			if (UniversalInputManager.UsePhoneUI)
			{
				this.popUpPos.z = -66f;
			}
			else
			{
				this.popUpPos.z = -44f;
			}
			this.PickThreePopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.25f, GameStrings.Get(this.textID), false);
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.PickThreePopup, 12f);
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(1f);
			GameState.Get().SetBusy(false);
			break;
		case 2:
			if (this.PickThreePopup)
			{
				NotificationManager.Get().DestroyNotification(this.PickThreePopup, 0.25f);
			}
			break;
		case 3:
			if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
			{
				yield break;
			}
			this.textID = "TB_DECKBUILDING_FIRSTENDTURN";
			if (UniversalInputManager.UsePhoneUI)
			{
				this.popUpPos.x = 82f;
				this.popUpPos.z = -28f;
			}
			else
			{
				this.popUpPos.z = -36f;
			}
			GameState.Get().SetBusy(true);
			this.EndOfTurnPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.5f, GameStrings.Get(this.textID), false);
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.EndOfTurnPopup, 5f);
			this.EndOfTurnPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
			NotificationManager.Get().DestroyNotification(this.CardPlayedPopup, 0f);
			yield return new WaitForSeconds(3.5f);
			GameState.Get().SetBusy(false);
			break;
		case 4:
			if (GameState.Get().GetFriendlySidePlayer() != GameState.Get().GetCurrentPlayer())
			{
				yield break;
			}
			this.textID = "TB_DECKBUILDING_FIRSTCARDPLAYED";
			if (UniversalInputManager.UsePhoneUI)
			{
				this.popUpPos.x = 82f;
				this.popUpPos.y = 0f;
				this.popUpPos.z = -28f;
			}
			else
			{
				this.popUpPos.x = 51f;
				this.popUpPos.y = 0f;
				this.popUpPos.z = -15.5f;
			}
			GameState.Get().SetBusy(true);
			this.CardPlayedPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.5f, GameStrings.Get(this.textID), false);
			this.CardPlayedPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Right);
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.CardPlayedPopup, 10f);
			yield return new WaitForSeconds(3f);
			GameState.Get().SetBusy(false);
			if (this.CardPlayedPopup != null)
			{
				iTween.PunchScale(this.CardPlayedPopup.gameObject, iTween.Hash(new object[]
				{
					"amount",
					new Vector3(2f, 2f, 2f),
					"time",
					1f
				}));
			}
			break;
		case 10:
			if (GameState.Get().IsFriendlySidePlayerTurn())
			{
				TurnStartManager.Get().BeginListeningForTurnEvents();
			}
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(2f);
			GameState.Get().SetBusy(false);
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(2f);
			GameState.Get().SetBusy(false);
			this.textID = "TB_DECKBUILDING_STARTOFGAME";
			if (UniversalInputManager.UsePhoneUI)
			{
				this.popUpPos.x = 0f;
				this.popUpPos.y = 0f;
				this.popUpPos.z = 0f;
			}
			else
			{
				this.popUpPos.x = 0f;
				this.popUpPos.y = 0f;
				this.popUpPos.z = 0f;
			}
			GameState.Get().SetBusy(true);
			this.StartOfTurnPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 2f, GameStrings.Get(this.textID), false);
			base.PlaySound("tutorial_mission_hero_coin_mouse_away", 1f, true, false);
			NotificationManager.Get().DestroyNotification(this.StartOfTurnPopup, 3f);
			yield return new WaitForSeconds(3f);
			GameState.Get().SetBusy(false);
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(3f);
			GameState.Get().SetBusy(false);
			break;
		case 11:
			NotificationManager.Get().DestroyNotification(this.StartOfTurnPopup, 0f);
			NotificationManager.Get().DestroyNotification(this.EndOfTurnPopup, 0f);
			NotificationManager.Get().DestroyNotification(this.CardPlayedPopup, 0f);
			NotificationManager.Get().DestroyNotification(this.PickThreePopup, 0f);
			break;
		}
		yield break;
	}

	// Token: 0x040017E6 RID: 6118
	private Notification PickThreePopup;

	// Token: 0x040017E7 RID: 6119
	private Notification EndOfTurnPopup;

	// Token: 0x040017E8 RID: 6120
	private Notification StartOfTurnPopup;

	// Token: 0x040017E9 RID: 6121
	private Notification CardPlayedPopup;

	// Token: 0x040017EA RID: 6122
	private Vector3 popUpPos;

	// Token: 0x040017EB RID: 6123
	private string textID;
}
