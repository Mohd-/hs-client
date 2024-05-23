using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class LOE03_AncientTemple : LOE_MissionEntity
{
	// Token: 0x0600290D RID: 10509 RVA: 0x000C72BC File Offset: 0x000C54BC
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_03_TURN_5_4");
		base.PreloadSound("VO_LOE_03_TURN_6");
		base.PreloadSound("VO_LOE_03_TURN_9");
		base.PreloadSound("VO_LOE_03_TURN_5");
		base.PreloadSound("VO_LOE_03_TURN_4_GOOD");
		base.PreloadSound("VO_LOE_03_TURN_4_BAD");
		base.PreloadSound("VO_LOE_03_TURN_6_2");
		base.PreloadSound("VO_LOE_03_TURN_4_NEITHER");
		base.PreloadSound("VO_LOE_03_TURN_3_WARNING");
		base.PreloadSound("VO_LOE_03_TURN_2");
		base.PreloadSound("VO_LOE_03_TURN_2_2");
		base.PreloadSound("VO_LOE_03_TURN_4");
		base.PreloadSound("VO_LOE_03_TURN_7");
		base.PreloadSound("VO_LOE_03_TURN_7_2");
		base.PreloadSound("VO_LOE_03_TURN_3_BOULDER");
		base.PreloadSound("VO_LOE_03_TURN_1");
		base.PreloadSound("VO_LOE_03_TURN_8");
		base.PreloadSound("VO_LOE_03_TURN_10");
		base.PreloadSound("VO_LOE_03_WIN");
		base.PreloadSound("VO_LOE_WING_1_WIN_2");
		base.PreloadSound("VO_LOE_03_RESPONSE");
	}

	// Token: 0x0600290E RID: 10510 RVA: 0x000C73B0 File Offset: 0x000C55B0
	public override bool ShouldHandleCoin()
	{
		return false;
	}

	// Token: 0x0600290F RID: 10511 RVA: 0x000C73B3 File Offset: 0x000C55B3
	public override void NotifyOfMulliganInitialized()
	{
		base.NotifyOfMulliganInitialized();
		this.m_mostRecentMissionEvent = base.GetTag(GAME_TAG.MISSION_EVENT);
		this.InitVisuals();
	}

	// Token: 0x06002910 RID: 10512 RVA: 0x000C73D0 File Offset: 0x000C55D0
	public override void NotifyOfMulliganEnded()
	{
		base.NotifyOfMulliganEnded();
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		actor.GetHealthObject().Hide();
	}

	// Token: 0x06002911 RID: 10513 RVA: 0x000C7404 File Offset: 0x000C5604
	public override void OnTagChanged(TagDelta change)
	{
		base.OnTagChanged(change);
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag == GAME_TAG.COST)
		{
			this.UpdateVisuals(change.newValue);
		}
	}

	// Token: 0x06002912 RID: 10514 RVA: 0x000C7440 File Offset: 0x000C5640
	public override string CustomChoiceBannerText()
	{
		if (base.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.MAIN_START)
		{
			string text = null;
			int mostRecentMissionEvent = this.m_mostRecentMissionEvent;
			switch (mostRecentMissionEvent)
			{
			case 10:
				text = "MISSION_GLOWING_POOL";
				break;
			case 11:
				text = "MISSION_PIT_OF_SPIKES";
				break;
			case 12:
				text = "MISSION_TAKE_THE_SHORTCUT";
				break;
			default:
				if (mostRecentMissionEvent == 4)
				{
					text = "MISSION_STATUES_EYE";
				}
				break;
			}
			if (text != null)
			{
				return GameStrings.Get(text);
			}
		}
		return null;
	}

	// Token: 0x06002913 RID: 10515 RVA: 0x000C74C0 File Offset: 0x000C56C0
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		this.m_mostRecentMissionEvent = missionEvent;
		yield return Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
		switch (missionEvent)
		{
		case 1:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_5_4", 3f, 1f, true, false));
			break;
		case 2:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_6", 3f, 1f, false));
			GameState.Get().SetBusy(false);
			break;
		case 3:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_9", 3f, 1f, false));
			break;
		case 4:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_5", 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 5:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_4_GOOD", 3f, 1f, true, false));
			break;
		case 6:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_4_BAD", 3f, 1f, false));
			break;
		case 7:
			while (GameState.Get().IsBusy())
			{
				yield return null;
			}
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_6_2", 3f, 1f, false));
			break;
		case 8:
			GameState.Get().SetBusy(true);
			while (this.m_enemySpeaking)
			{
				yield return null;
			}
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_4_NEITHER", 3f, 1f, false));
			break;
		case 9:
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_3_WARNING", 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 10:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_2", 3f, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_03_TURN_2_2", 3f, 1f, false));
			break;
		case 11:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_4", 3f, 1f, true, false));
			break;
		case 12:
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_7", 3f, 1f, true, false));
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote", "VO_LOE_03_TURN_7_2", 3f, 1f, false));
			break;
		case 13:
			GameState.Get().SetBusy(false);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_TURN_3_BOULDER", 3f, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		}
		yield break;
	}

	// Token: 0x06002914 RID: 10516 RVA: 0x000C74EC File Offset: 0x000C56EC
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor mActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		mActor.DeactivateSpell(SpellType.IMMUNE);
		if (turn == 1)
		{
			int cost = base.GetCost();
			this.InitTurnCounter(cost);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_1", 3f, 1f, false));
		}
		if (turn % 2 == 0)
		{
			switch (base.GetCost())
			{
			case 1:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_10", 3f, 1f, false));
				break;
			case 3:
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote", "VO_LOE_03_TURN_8", 3f, 1f, false));
				break;
			}
		}
		yield break;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000C7518 File Offset: 0x000C5718
	protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		if (MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
		{
			Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWait("Reno_BigQuote", "VO_LOE_03_RESPONSE", 3f, 1f, true, false));
		}
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000C755C File Offset: 0x000C575C
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Reno_Quote", "VO_LOE_03_WIN", "VO_LOE_03_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x000C7588 File Offset: 0x000C5788
	private void InitVisuals()
	{
		int cost = base.GetCost();
		int tag = base.GetTag(GAME_TAG.TURN);
		this.InitTempleArt(cost);
		if (tag >= 1 && GameState.Get().IsPastBeginPhase())
		{
			this.InitTurnCounter(cost);
		}
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x000C75CC File Offset: 0x000C57CC
	private void InitTempleArt(int cost)
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject("TempleArt", true, false);
		this.m_templeArt = gameObject.GetComponent<TempleArt>();
		this.UpdateTempleArt(cost);
	}

	// Token: 0x06002919 RID: 10521 RVA: 0x000C7600 File Offset: 0x000C5800
	private void InitTurnCounter(int cost)
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject("LOE_Turn_Timer", true, false);
		this.m_turnCounter = gameObject.GetComponent<Notification>();
		PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
		component.FsmVariables.GetFsmBool("RunningMan").Value = true;
		component.FsmVariables.GetFsmBool("MineCart").Value = false;
		component.SendEvent("Birth");
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		this.m_turnCounter.transform.parent = actor.gameObject.transform;
		this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
		this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
		this.UpdateTurnCounterText(cost);
	}

	// Token: 0x0600291A RID: 10522 RVA: 0x000C76E8 File Offset: 0x000C58E8
	private void UpdateVisuals(int cost)
	{
		this.UpdateTempleArt(cost);
		if (this.m_turnCounter)
		{
			this.UpdateTurnCounter(cost);
		}
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000C7708 File Offset: 0x000C5908
	private void UpdateTempleArt(int cost)
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		this.m_templeArt.DoPortraitSwap(actor, cost);
	}

	// Token: 0x0600291C RID: 10524 RVA: 0x000C7738 File Offset: 0x000C5938
	private void UpdateTurnCounter(int cost)
	{
		PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
		component.SendEvent("Action");
		this.UpdateTurnCounterText(cost);
	}

	// Token: 0x0600291D RID: 10525 RVA: 0x000C7764 File Offset: 0x000C5964
	private void UpdateTurnCounterText(int cost)
	{
		GameStrings.PluralNumber[] pluralNumbers = new GameStrings.PluralNumber[]
		{
			new GameStrings.PluralNumber
			{
				m_number = cost
			}
		};
		string headlineString = GameStrings.FormatPlurals("MISSION_DEFAULTCOUNTERNAME", pluralNumbers, new object[0]);
		this.m_turnCounter.ChangeDialogText(headlineString, cost.ToString(), string.Empty, string.Empty);
	}

	// Token: 0x04001821 RID: 6177
	private Notification m_turnCounter;

	// Token: 0x04001822 RID: 6178
	private TempleArt m_templeArt;

	// Token: 0x04001823 RID: 6179
	private int m_mostRecentMissionEvent;
}
