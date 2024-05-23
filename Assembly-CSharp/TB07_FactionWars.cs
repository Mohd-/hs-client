using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000A41 RID: 2625
public class TB07_FactionWars : MissionEntity
{
	// Token: 0x06005C82 RID: 23682 RVA: 0x001B948A File Offset: 0x001B768A
	public override void PreloadAssets()
	{
	}

	// Token: 0x06005C83 RID: 23683 RVA: 0x001B948C File Offset: 0x001B768C
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		this.textID = string.Empty;
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		if (missionEvent == 1)
		{
			this.textID = "TB_SINGLEPLAYERTRIAL_PLAYERDIED";
		}
		if (this.textID.Length > 0)
		{
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(0.5f);
			GameState.Get().SetBusy(false);
			this.popUpPos = new Vector3(1.27f, 0f, 19f);
			this.GameOverPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.25f, GameStrings.Get(this.textID), false);
			NotificationManager.Get().DestroyNotification(this.GameOverPopup, 4f);
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(0.5f);
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x040043F2 RID: 17394
	private Notification GameOverPopup;

	// Token: 0x040043F3 RID: 17395
	private string textID;

	// Token: 0x040043F4 RID: 17396
	private Vector3 popUpPos;
}
