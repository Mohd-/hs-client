using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class LOE07_MineCart : LOE_MissionEntity
{
	// Token: 0x0600291F RID: 10527 RVA: 0x000C77CB File Offset: 0x000C59CB
	public override void StartGameplaySoundtracks()
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOE_Minecart);
	}

	// Token: 0x06002920 RID: 10528 RVA: 0x000C77E0 File Offset: 0x000C59E0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOE_07_START");
		base.PreloadSound("VO_LOE_07_WIN");
		base.PreloadSound("VO_LOEA07_MINE_ARCHAEDAS");
		base.PreloadSound("VO_LOEA07_MINE_DETONATE");
		base.PreloadSound("VO_LOEA07_MINE_RAMMING");
		base.PreloadSound("VO_LOEA07_MINE_PARROT");
		base.PreloadSound("VO_LOEA07_MINE_BOOMZOOKA");
		base.PreloadSound("VO_LOEA07_MINE_DYNAMITE");
		base.PreloadSound("VO_LOEA07_MINE_BOOM");
		base.PreloadSound("VO_LOEA07_MINE_BARREL_FORWARD");
		base.PreloadSound("VO_LOEA07_MINE_HUNKER_DOWN");
		base.PreloadSound("VO_LOEA07_MINE_SPIKED_DECOY");
		base.PreloadSound("VO_LOEA07_MINE_REPAIRS");
		base.PreloadSound("VO_BRANN_WIN_07_ALT_07");
		base.PreloadSound("VO_LOE_07_RESPONSE");
		base.PreloadSound("Mine_response");
	}

	// Token: 0x06002921 RID: 10529 RVA: 0x000C789D File Offset: 0x000C5A9D
	public override bool ShouldDoAlternateMulliganIntro()
	{
		return true;
	}

	// Token: 0x06002922 RID: 10530 RVA: 0x000C78A0 File Offset: 0x000C5AA0
	public override bool ShouldHandleCoin()
	{
		return false;
	}

	// Token: 0x06002923 RID: 10531 RVA: 0x000C78A3 File Offset: 0x000C5AA3
	public override void NotifyOfMulliganInitialized()
	{
		base.NotifyOfMulliganInitialized();
		this.InitVisuals();
	}

	// Token: 0x06002924 RID: 10532 RVA: 0x000C78B4 File Offset: 0x000C5AB4
	public override void NotifyOfMulliganEnded()
	{
		base.NotifyOfMulliganEnded();
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		actor.GetHealthObject().Hide();
	}

	// Token: 0x06002925 RID: 10533 RVA: 0x000C78E8 File Offset: 0x000C5AE8
	public override void OnTagChanged(TagDelta change)
	{
		base.OnTagChanged(change);
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag == GAME_TAG.COST)
		{
			if (change.newValue != change.oldValue)
			{
				this.UpdateVisuals(change.newValue);
			}
		}
	}

	// Token: 0x06002926 RID: 10534 RVA: 0x000C7934 File Offset: 0x000C5B34
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (turn != 1)
		{
			if (turn == 11)
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_07_WIN", 3f, 1f, false));
			}
		}
		else
		{
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOE_07_START", 3f, 1f, false));
		}
		yield break;
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000C7960 File Offset: 0x000C5B60
	protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		if (MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
		{
			Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
			SoundManager.Get().LoadAndPlay("Mine_response", actor.gameObject);
		}
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000C79AC File Offset: 0x000C5BAC
	protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
		{
			yield return null;
		}
		if (this.m_playedLines.Contains(entity.GetCardId()))
		{
			yield break;
		}
		string cardID = entity.GetCardId();
		string text = cardID;
		if (text != null)
		{
			if (LOE07_MineCart.<>f__switch$map60 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
				dictionary.Add("LOEA07_16", 0);
				LOE07_MineCart.<>f__switch$map60 = dictionary;
			}
			int num;
			if (LOE07_MineCart.<>f__switch$map60.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_ARCHAEDAS", 3f, 1f, false));
				}
			}
		}
		yield break;
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000C79D8 File Offset: 0x000C5BD8
	protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
		{
			yield return null;
		}
		if (this.m_playedLines.Contains(entity.GetCardId()))
		{
			yield break;
		}
		string cardID = entity.GetCardId();
		string text = cardID;
		if (text != null)
		{
			if (LOE07_MineCart.<>f__switch$map61 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
				dictionary.Add("LOEA07_23", 0);
				dictionary.Add("LOEA07_19", 1);
				dictionary.Add("LOEA07_25", 2);
				dictionary.Add("LOEA07_32", 3);
				dictionary.Add("LOEA07_18", 4);
				dictionary.Add("LOEA07_20", 5);
				dictionary.Add("LOEA07_21", 6);
				dictionary.Add("LOEA07_22", 7);
				dictionary.Add("LOEA07_24", 8);
				dictionary.Add("LOEA07_28", 9);
				LOE07_MineCart.<>f__switch$map61 = dictionary;
			}
			int num;
			if (LOE07_MineCart.<>f__switch$map61.TryGetValue(text, ref num))
			{
				switch (num)
				{
				case 0:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_DETONATE", 3f, 1f, false));
					break;
				case 1:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_RAMMING", 3f, 1f, false));
					break;
				case 2:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_PARROT", 3f, 1f, false));
					break;
				case 3:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_BOOMZOOKA", 3f, 1f, false));
					break;
				case 4:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_DYNAMITE", 3f, 1f, false));
					break;
				case 5:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_BOOM", 3f, 1f, false));
					break;
				case 6:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_BARREL_FORWARD", 3f, 1f, false));
					break;
				case 7:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_HUNKER_DOWN", 3f, 1f, false));
					break;
				case 8:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_SPIKED_DECOY", 3f, 1f, false));
					break;
				case 9:
					this.m_playedLines.Add(cardID);
					yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote", "VO_LOEA07_MINE_REPAIRS", 3f, 1f, false));
					break;
				}
			}
		}
		yield break;
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000C7A04 File Offset: 0x000C5C04
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Brann_Quote", "VO_BRANN_WIN_07_ALT_07", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x000C7A30 File Offset: 0x000C5C30
	private void InitVisuals()
	{
		int cost = base.GetCost();
		this.InitMineCartArt();
		this.InitTurnCounter(cost);
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x000C7A54 File Offset: 0x000C5C54
	private void InitMineCartArt()
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject("MineCartRushArt", true, false);
		this.m_mineCartArt = gameObject.GetComponent<MineCartRushArt>();
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000C7A80 File Offset: 0x000C5C80
	private void InitTurnCounter(int cost)
	{
		GameObject gameObject = AssetLoader.Get().LoadGameObject("LOE_Turn_Timer", true, false);
		this.m_turnCounter = gameObject.GetComponent<Notification>();
		PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
		component.FsmVariables.GetFsmBool("RunningMan").Value = false;
		component.FsmVariables.GetFsmBool("MineCart").Value = true;
		component.SendEvent("Birth");
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		this.m_turnCounter.transform.parent = actor.gameObject.transform;
		this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
		this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
		this.UpdateTurnCounterText(cost);
	}

	// Token: 0x0600292E RID: 10542 RVA: 0x000C7B68 File Offset: 0x000C5D68
	private void UpdateVisuals(int cost)
	{
		this.UpdateMineCartArt();
		this.UpdateTurnCounter(cost);
	}

	// Token: 0x0600292F RID: 10543 RVA: 0x000C7B78 File Offset: 0x000C5D78
	private void UpdateMineCartArt()
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		this.m_mineCartArt.DoPortraitSwap(actor);
	}

	// Token: 0x06002930 RID: 10544 RVA: 0x000C7BA8 File Offset: 0x000C5DA8
	private void UpdateTurnCounter(int cost)
	{
		PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
		component.SendEvent("Action");
		this.UpdateTurnCounterText(cost);
	}

	// Token: 0x06002931 RID: 10545 RVA: 0x000C7BD4 File Offset: 0x000C5DD4
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

	// Token: 0x04001824 RID: 6180
	private Notification m_turnCounter;

	// Token: 0x04001825 RID: 6181
	private HashSet<string> m_playedLines = new HashSet<string>();

	// Token: 0x04001826 RID: 6182
	private MineCartRushArt m_mineCartArt;
}
