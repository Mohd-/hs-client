using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002FF RID: 767
public class BRM06_Majordomo : BRM_MissionEntity
{
	// Token: 0x06002876 RID: 10358 RVA: 0x000C50B0 File Offset: 0x000C32B0
	public override void PreloadAssets()
	{
		base.PreloadSound("VO_BRMA06_1_RESPONSE_03");
		base.PreloadSound("VO_BRMA06_3_RESPONSE_03");
		base.PreloadSound("VO_BRMA06_1_DEATH_04");
		base.PreloadSound("VO_BRMA06_1_TURN1_02_ALT");
		base.PreloadSound("VO_BRMA06_1_SUMMON_RAG_05");
		base.PreloadSound("VO_BRMA06_3_INTRO_01");
		base.PreloadSound("VO_BRMA06_3_TURN1_02");
		base.PreloadSound("VO_NEFARIAN_MAJORDOMO_41");
	}

	// Token: 0x06002877 RID: 10359 RVA: 0x000C5118 File Offset: 0x000C3318
	protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		switch (emoteType)
		{
		case EmoteType.GREETINGS:
		case EmoteType.WELL_PLAYED:
		case EmoteType.OOPS:
		case EmoteType.THREATEN:
		case EmoteType.THANKS:
		case EmoteType.SORRY:
		{
			Entity hero = GameState.Get().GetOpposingSidePlayer().GetHero();
			if (hero.GetCardId() == "BRMA06_1" || hero.GetCardId() == "BRMA06_1H")
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA06_1_RESPONSE_03", "VO_BRMA06_1_RESPONSE_03", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			}
			else if (hero.GetCardId() == "BRMA06_3" || hero.GetCardId() == "BRMA06_3H")
			{
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA06_3_RESPONSE_03", "VO_BRMA06_3_RESPONSE_03", Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
			}
			break;
		}
		}
	}

	// Token: 0x06002878 RID: 10360 RVA: 0x000C5218 File Offset: 0x000C3418
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (missionEvent == 1)
		{
			EntityDef ragDef = DefLoader.Get().GetEntityDef("BRMA06_3");
			Gameplay.Get().UpdateEnemySideNameBannerName(ragDef.GetName());
			NotificationManager.Get().CreateCharacterQuote("Majordomo_Quote", new Vector3(157.6f, NotificationManager.DEPTH, 84.5f), GameStrings.Get("VO_BRMA06_1_SUMMON_RAG_05"), string.Empty, true, 30f, null, CanvasAnchor.BOTTOM_LEFT);
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA06_1_SUMMON_RAG_05", 1f, true, false));
			NotificationManager.Get().DestroyActiveQuote(0f);
			enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
			yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA06_3_TURN1_02", "VO_BRMA06_3_TURN1_02", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x06002879 RID: 10361 RVA: 0x000C5244 File Offset: 0x000C3444
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("VO_BRMA06_1_TURN1_02_ALT", "VO_BRMA06_1_TURN1_02_ALT", Notification.SpeechBubbleDirection.TopRight, enemyActor, 1f, true, false));
		}
		yield break;
	}

	// Token: 0x0600287A RID: 10362 RVA: 0x000C5270 File Offset: 0x000C3470
	protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
		{
			yield return new WaitForSeconds(5f);
			NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", GameStrings.Get("VO_NEFARIAN_MAJORDOMO_DEAD_42"), "VO_NEFARIAN_MAJORDOMO_DEAD_42", true, 0f, CanvasAnchor.BOTTOM_LEFT);
		}
		yield break;
	}

	// Token: 0x0600287B RID: 10363 RVA: 0x000C5294 File Offset: 0x000C3494
	public override IEnumerator PlayMissionIntroLineAndWait()
	{
		if (NotificationManager.Get().HasSoundPlayedThisSession("VO_NEFARIAN_MAJORDOMO_41"))
		{
			yield break;
		}
		NotificationManager.Get().ForceAddSoundToPlayedList("VO_NEFARIAN_MAJORDOMO_41");
		NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote", new Vector3(97.7f, NotificationManager.DEPTH, 27.8f), GameStrings.Get("VO_NEFARIAN_MAJORDOMO_41"), string.Empty, false, 30f, null, CanvasAnchor.BOTTOM_LEFT);
		yield return Gameplay.Get().StartCoroutine(base.PlaySoundAndWait("VO_NEFARIAN_MAJORDOMO_41", string.Empty, Notification.SpeechBubbleDirection.None, null, 1f, true, false, 3f));
		NotificationManager.Get().DestroyActiveQuote(0f);
		yield break;
	}
}
