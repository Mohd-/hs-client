using System;
using System.Collections;

// Token: 0x0200030D RID: 781
public class TB02_CoOp : MissionEntity
{
	// Token: 0x060028C9 RID: 10441 RVA: 0x000C6480 File Offset: 0x000C4680
	private void SetUpBossCard()
	{
		if (this.m_bossCard == null)
		{
			int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_ENT_1);
			Entity entity = GameState.Get().GetEntity(tag);
			if (entity != null)
			{
				this.m_bossCard = entity.GetCard();
			}
		}
	}

	// Token: 0x060028CA RID: 10442 RVA: 0x000C64D0 File Offset: 0x000C46D0
	public override void PreloadAssets()
	{
		base.PreloadSound("FX_MinionSummon_Cast");
		base.PreloadSound("CleanMechSmall_Trigger_Underlay");
		base.PreloadSound("CleanMechLarge_Play_Underlay");
		base.PreloadSound("CleanMechLarge_Death_Underlay");
	}

	// Token: 0x060028CB RID: 10443 RVA: 0x000C650C File Offset: 0x000C470C
	protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		this.SetUpBossCard();
		if (this.m_bossCard == null)
		{
			yield break;
		}
		if (turn == 1)
		{
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("CleanMechSmall_Trigger_Underlay", "VO_COOP02_00", Notification.SpeechBubbleDirection.TopRight, this.m_bossCard.GetActor(), 1f, true, false));
		}
		yield break;
	}

	// Token: 0x060028CC RID: 10444 RVA: 0x000C6538 File Offset: 0x000C4738
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		while (this.m_enemySpeaking)
		{
			yield return null;
		}
		this.SetUpBossCard();
		if (this.m_bossCard == null)
		{
			yield break;
		}
		if (missionEvent != 5)
		{
			if (missionEvent == 6)
			{
				GameState.Get().SetBusy(true);
				Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("CleanMechLarge_Death_Underlay", "VO_COOP02_ABILITY_06", Notification.SpeechBubbleDirection.TopRight, this.m_bossCard.GetActor(), 1f, true, false));
				GameState.Get().SetBusy(false);
			}
		}
		else
		{
			GameState.Get().SetBusy(true);
			Gameplay.Get().StartCoroutine(base.PlaySoundAndBlockSpeech("CleanMechLarge_Play_Underlay", "VO_COOP02_ABILITY_05", Notification.SpeechBubbleDirection.TopRight, this.m_bossCard.GetActor(), 1f, true, false));
			GameState.Get().SetBusy(false);
		}
		yield break;
	}

	// Token: 0x060028CD RID: 10445 RVA: 0x000C6564 File Offset: 0x000C4764
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		if (gameResult != TAG_PLAYSTATE.WON)
		{
			base.NotifyOfGameOver(gameResult);
			return;
		}
		PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_EndGameScreen);
		GameEntity.EndGameScreenContext callbackData = new GameEntity.EndGameScreenContext();
		SoundManager.Get().LoadAndPlay("victory_jingle");
		AssetLoader.Get().LoadActor("VictoryTwoScoop", new AssetLoader.GameObjectCallback(base.OnEndGameScreenLoaded), callbackData, false);
	}

	// Token: 0x040017E5 RID: 6117
	private Card m_bossCard;
}
