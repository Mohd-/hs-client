using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class TB05_GiftExchange : MissionEntity
{
	// Token: 0x060028DA RID: 10458 RVA: 0x000C678C File Offset: 0x000C498C
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT1");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT2");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT3");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_GIFT4");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG1");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG2");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG3");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG4");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG5");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_LONG6");
		base.PreloadSound("VO_TB_1503_FATHER_WINTER_START");
	}

	// Token: 0x060028DB RID: 10459 RVA: 0x000C6814 File Offset: 0x000C4A14
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		this.VOChoice = string.Empty;
		this.delayTime = 0f;
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		if (missionEvent != 1)
		{
			if (missionEvent != 2)
			{
				if (missionEvent != 10)
				{
					if (missionEvent == 11)
					{
						if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
						{
							if (this.FirstStolenVO.Length > 0)
							{
								this.VOChoice = this.FirstStolenVO;
								this.FirstStolenVO = string.Empty;
								yield return new WaitForSeconds(1.5f);
								this.delayTime = 4f;
								this.textID = "TB_GIFTEXCHANGE_GIFTSTOLEN";
								this.popUpPos = new Vector3(22.2f, 0f, -44.6f);
								if (UniversalInputManager.UsePhoneUI)
								{
									this.popUpPos.x = 61f;
									this.popUpPos.z = -29f;
								}
								this.GiftStolenPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.25f, GameStrings.Get(this.textID), false);
								NotificationManager.Get().DestroyNotification(this.GiftStolenPopup, 4f);
							}
						}
						else if (this.NextStolenVO.Length > 0)
						{
							this.VOChoice = this.NextStolenVO;
							this.NextStolenVO = string.Empty;
						}
					}
				}
				else
				{
					this.VOChoice = this.StartVO;
					this.delayTime = 5f;
					this.textID = "TB_GIFTEXCHANGE_START";
					this.popUpPos = new Vector3(22.2f, 0f, -44.6f);
					this.popUpPos = new Vector3(0f, 0f, 0f);
					this.GameStartPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * 1.75f, GameStrings.Get(this.textID), false);
					NotificationManager.Get().DestroyNotification(this.GameStartPopup, 3f);
				}
			}
			else
			{
				this.VOChoice = this.PissedVOList[Random.Range(1, this.PissedVOList.Length)];
				this.delayTime = 2f;
			}
		}
		else if (this.FirstGiftVO.Length > 0)
		{
			this.VOChoice = this.FirstGiftVO;
			this.FirstGiftVO = string.Empty;
			this.delayTime = 4f;
			GameState.Get().SetBusy(true);
			yield return new WaitForSeconds(0.5f);
			GameState.Get().SetBusy(false);
			this.textID = "TB_GIFTEXCHANGE_GIFTSPAWNED";
			this.popUpPos = new Vector3(1.27f, 0f, -9.32f);
			if (GameState.Get().GetFriendlySidePlayer() == GameState.Get().GetCurrentPlayer())
			{
				this.popUpPos.z = 19f;
			}
			float popupScale = 1.25f;
			if (UniversalInputManager.UsePhoneUI)
			{
				popupScale = 1.75f;
			}
			this.GiftSpawnedPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.popUpPos, TutorialEntity.HELP_POPUP_SCALE * popupScale, GameStrings.Get(this.textID), false);
			NotificationManager.Get().DestroyNotification(this.GiftSpawnedPopup, 4f);
		}
		else
		{
			this.VOChoice = this.GiftVOList[Random.Range(1, this.GiftVOList.Length)];
			this.delayTime = 3f;
		}
		base.PlaySound(this.VOChoice, 1f, true, false);
		GameState.Get().SetBusy(true);
		yield return new WaitForSeconds(this.delayTime);
		GameState.Get().SetBusy(false);
		yield break;
	}

	// Token: 0x040017FA RID: 6138
	private string[] GiftVOList = new string[]
	{
		"VO_TB_1503_FATHER_WINTER_GIFT1",
		"VO_TB_1503_FATHER_WINTER_GIFT2",
		"VO_TB_1503_FATHER_WINTER_GIFT3",
		"VO_TB_1503_FATHER_WINTER_GIFT4"
	};

	// Token: 0x040017FB RID: 6139
	private string[] PissedVOList = new string[]
	{
		"VO_TB_1503_FATHER_WINTER_LONG2",
		"VO_TB_1503_FATHER_WINTER_LONG3",
		"VO_TB_1503_FATHER_WINTER_LONG4",
		"VO_TB_1503_FATHER_WINTER_LONG5",
		"VO_TB_1503_FATHER_WINTER_LONG6"
	};

	// Token: 0x040017FC RID: 6140
	private string FirstGiftVO = "VO_TB_1503_FATHER_WINTER_GIFT1";

	// Token: 0x040017FD RID: 6141
	private string StartVO = "VO_TB_1503_FATHER_WINTER_LONG6";

	// Token: 0x040017FE RID: 6142
	private string FirstStolenVO = "VO_TB_1503_FATHER_WINTER_START";

	// Token: 0x040017FF RID: 6143
	private string NextStolenVO = "VO_TB_1503_FATHER_WINTER_LONG1";

	// Token: 0x04001800 RID: 6144
	private string VOChoice;

	// Token: 0x04001801 RID: 6145
	private float delayTime;

	// Token: 0x04001802 RID: 6146
	private Notification GiftStolenPopup;

	// Token: 0x04001803 RID: 6147
	private Notification GiftSpawnedPopup;

	// Token: 0x04001804 RID: 6148
	private Notification GameStartPopup;

	// Token: 0x04001805 RID: 6149
	private string textID;

	// Token: 0x04001806 RID: 6150
	private Vector3 popUpPos;
}
