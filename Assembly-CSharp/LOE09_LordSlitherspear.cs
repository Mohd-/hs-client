using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000314 RID: 788
public class LOE09_LordSlitherspear : LOE_MissionEntity
{
	// Token: 0x060028E9 RID: 10473 RVA: 0x000C6A6D File Offset: 0x000C4C6D
	public override void StartGameplaySoundtracks()
	{
		MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOE_Wing3);
	}

	// Token: 0x060028EA RID: 10474 RVA: 0x000C6A80 File Offset: 0x000C4C80
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_LOEA09_1_RESPONSE");
		base.PreloadSound("VO_LOEA09_UNSTABLE");
		base.PreloadSound("VO_LOEA09_HERO_POWER");
		base.PreloadSound("FX_MinionSummon_Cast");
		base.PreloadSound("VO_LOEA09_QUOTE1");
		base.PreloadSound("VO_LOEA09_FINLEY_DEATH");
		base.PreloadSound("VO_LOEA09_HERO_POWER1");
		base.PreloadSound("VO_LOEA09_HERO_POWER2");
		base.PreloadSound("VO_LOEA09_HERO_POWER3");
		base.PreloadSound("VO_LOEA09_HERO_POWER4");
		base.PreloadSound("VO_LOEA09_HERO_POWER5");
		base.PreloadSound("VO_LOEA09_HERO_POWER6");
		base.PreloadSound("VO_LOEA09_WIN");
	}

	// Token: 0x060028EB RID: 10475 RVA: 0x000C6B1C File Offset: 0x000C4D1C
	protected override void InitEmoteResponses()
	{
		List<MissionEntity.EmoteResponseGroup> list = new List<MissionEntity.EmoteResponseGroup>();
		List<MissionEntity.EmoteResponseGroup> list2 = list;
		MissionEntity.EmoteResponseGroup emoteResponseGroup = new MissionEntity.EmoteResponseGroup();
		emoteResponseGroup.m_triggers = new List<EmoteType>(MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS);
		MissionEntity.EmoteResponseGroup emoteResponseGroup2 = emoteResponseGroup;
		List<MissionEntity.EmoteResponse> list3 = new List<MissionEntity.EmoteResponse>();
		list3.Add(new MissionEntity.EmoteResponse
		{
			m_soundName = "VO_LOEA09_1_RESPONSE",
			m_stringTag = "VO_LOEA09_1_RESPONSE"
		});
		emoteResponseGroup2.m_responses = list3;
		list2.Add(emoteResponseGroup);
		this.m_emoteResponseGroups = list;
	}

	// Token: 0x060028EC RID: 10476 RVA: 0x000C6B84 File Offset: 0x000C4D84
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		switch (missionEvent)
		{
		case 1:
			this.m_finley_saved = true;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOEA09_QUOTE1", 3f, 1f, false));
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_UNSTABLE", "VO_LOEA09_UNSTABLE", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			GameState.Get().SetBusy(false);
			break;
		case 3:
			if (this.m_finley_death_line)
			{
				yield break;
			}
			this.m_finley_death_line = true;
			GameState.Get().SetBusy(true);
			yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOEA09_FINLEY_DEATH", 3f, 1f, false));
			GameState.Get().SetBusy(false);
			break;
		}
		yield break;
	}

	// Token: 0x060028ED RID: 10477 RVA: 0x000C6BB0 File Offset: 0x000C4DB0
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		if (this.m_finley_saved)
		{
			yield break;
		}
		if (this.m_cauldronCard == null)
		{
			int cauldronId = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
			Entity cauldron = GameState.Get().GetEntity(cauldronId);
			if (cauldron != null)
			{
				this.m_cauldronCard = cauldron.GetCard();
			}
		}
		if (this.m_cauldronCard == null && turn > 1)
		{
			yield break;
		}
		if (this.m_cauldronCard != null && this.m_cauldronCard.GetEntity().GetZone() != TAG_ZONE.PLAY)
		{
			yield break;
		}
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		switch (turn)
		{
		case 1:
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER", "VO_LOEA09_HERO_POWER", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
			if (!(this.m_cauldronCard == null))
			{
				yield return Gameplay.Get().StartCoroutine(base.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote", "VO_LOEA09_HERO_POWER1", "VO_LOEA09_HERO_POWER1", 3f, 1f, false));
			}
			break;
		case 4:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER2", Notification.SpeechBubbleDirection.TopRight, this.m_cauldronCard.GetActor(), 3f, 1f, true, false));
			break;
		case 6:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER3", Notification.SpeechBubbleDirection.TopRight, this.m_cauldronCard.GetActor(), 3f, 1f, true, false));
			break;
		case 8:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER4", Notification.SpeechBubbleDirection.TopRight, this.m_cauldronCard.GetActor(), 3f, 1f, true, false));
			break;
		case 10:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER5", Notification.SpeechBubbleDirection.TopRight, this.m_cauldronCard.GetActor(), 3f, 1f, true, false));
			break;
		case 12:
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeechOnce("VO_LOEA09_HERO_POWER6", Notification.SpeechBubbleDirection.TopRight, this.m_cauldronCard.GetActor(), 3f, 1f, true, false));
			break;
		}
		yield break;
	}

	// Token: 0x060028EE RID: 10478 RVA: 0x000C6BDC File Offset: 0x000C4DDC
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			yield return Gameplay.Get().StartCoroutine(base.PlayCharacterQuoteAndWait("Blaggh_Quote", "VO_LOEA09_WIN", "VO_LOEA09_WIN", 0f, false, false));
		}
		yield break;
	}

	// Token: 0x04001818 RID: 6168
	private bool m_finley_death_line;

	// Token: 0x04001819 RID: 6169
	private bool m_finley_saved;

	// Token: 0x0400181A RID: 6170
	private Card m_cauldronCard;
}
