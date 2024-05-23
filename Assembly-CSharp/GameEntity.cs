using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E4 RID: 740
public class GameEntity : Entity
{
	// Token: 0x06002718 RID: 10008 RVA: 0x000BEA39 File Offset: 0x000BCC39
	public GameEntity()
	{
		this.PreloadAssets();
	}

	// Token: 0x06002719 RID: 10009 RVA: 0x000BEA54 File Offset: 0x000BCC54
	public void FadeOutHeroActor(Actor actorToFade)
	{
		this.ToggleSpotLight(actorToFade.GetHeroSpotlight(), false);
		Material heroMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitMatIdx];
		Material heroFrameMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitFrameMatIdx];
		float @float = heroMat.GetFloat("_LightingBlend");
		Action<object> action = delegate(object amount)
		{
			heroMat.SetFloat("_LightingBlend", (float)amount);
			heroFrameMat.SetFloat("_LightingBlend", (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			0.25f,
			"from",
			@float,
			"to",
			1f,
			"onupdate",
			action,
			"onupdatetarget",
			actorToFade.gameObject
		});
		iTween.ValueTo(actorToFade.gameObject, args);
	}

	// Token: 0x0600271A RID: 10010 RVA: 0x000BEB40 File Offset: 0x000BCD40
	public void FadeOutActor(Actor actorToFade)
	{
		Material mat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitMatIdx];
		Material frameMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitFrameMatIdx];
		float @float = mat.GetFloat("_LightingBlend");
		Action<object> action = delegate(object amount)
		{
			mat.SetFloat("_LightingBlend", (float)amount);
			frameMat.SetFloat("_LightingBlend", (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			0.25f,
			"from",
			@float,
			"to",
			1f,
			"onupdate",
			action,
			"onupdatetarget",
			actorToFade.gameObject
		});
		iTween.ValueTo(actorToFade.gameObject, args);
	}

	// Token: 0x0600271B RID: 10011 RVA: 0x000BEC20 File Offset: 0x000BCE20
	private void ToggleSpotLight(Light light, bool bOn)
	{
		float num = 0.1f;
		float num2 = 1.3f;
		Action<object> action = delegate(object amount)
		{
			light.intensity = (float)amount;
		};
		Action<object> action2 = delegate(object args)
		{
			light.enabled = false;
		};
		if (bOn)
		{
			light.enabled = true;
			light.intensity = 0f;
			Hashtable args3 = iTween.Hash(new object[]
			{
				"time",
				num,
				"from",
				0f,
				"to",
				num2,
				"onupdate",
				action,
				"onupdatetarget",
				light.gameObject
			});
			iTween.ValueTo(light.gameObject, args3);
		}
		else
		{
			Hashtable args2 = iTween.Hash(new object[]
			{
				"time",
				num,
				"from",
				light.intensity,
				"to",
				0f,
				"onupdate",
				action,
				"onupdatetarget",
				light.gameObject,
				"oncomplete",
				action2
			});
			iTween.ValueTo(light.gameObject, args2);
		}
	}

	// Token: 0x0600271C RID: 10012 RVA: 0x000BED9E File Offset: 0x000BCF9E
	public void FadeInHeroActor(Actor actorToFade)
	{
		this.FadeInHeroActor(actorToFade, 0f);
	}

	// Token: 0x0600271D RID: 10013 RVA: 0x000BEDAC File Offset: 0x000BCFAC
	public void FadeInHeroActor(Actor actorToFade, float lightBlendAmount)
	{
		this.ToggleSpotLight(actorToFade.GetHeroSpotlight(), true);
		Material heroMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitMatIdx];
		Material heroFrameMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitFrameMatIdx];
		float @float = heroMat.GetFloat("_LightingBlend");
		Action<object> action = delegate(object amount)
		{
			heroMat.SetFloat("_LightingBlend", (float)amount);
			heroFrameMat.SetFloat("_LightingBlend", (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			0.25f,
			"from",
			@float,
			"to",
			lightBlendAmount,
			"onupdate",
			action,
			"onupdatetarget",
			actorToFade.gameObject
		});
		iTween.ValueTo(actorToFade.gameObject, args);
	}

	// Token: 0x0600271E RID: 10014 RVA: 0x000BEE92 File Offset: 0x000BD092
	public void FadeInActor(Actor actorToFade)
	{
		this.FadeInActor(actorToFade, 0f);
	}

	// Token: 0x0600271F RID: 10015 RVA: 0x000BEEA0 File Offset: 0x000BD0A0
	public void FadeInActor(Actor actorToFade, float lightBlendAmount)
	{
		Material mat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitMatIdx];
		Material frameMat = actorToFade.m_portraitMesh.GetComponent<Renderer>().materials[actorToFade.m_portraitFrameMatIdx];
		float @float = mat.GetFloat("_LightingBlend");
		Action<object> action = delegate(object amount)
		{
			mat.SetFloat("_LightingBlend", (float)amount);
			frameMat.SetFloat("_LightingBlend", (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			0.25f,
			"from",
			@float,
			"to",
			lightBlendAmount,
			"onupdate",
			action,
			"onupdatetarget",
			actorToFade.gameObject
		});
		iTween.ValueTo(actorToFade.gameObject, args);
	}

	// Token: 0x06002720 RID: 10016 RVA: 0x000BEF7C File Offset: 0x000BD17C
	public void PreloadSound(string soundName)
	{
		this.m_preloadsNeeded++;
		AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnSoundLoaded), null, false, SoundManager.Get().GetPlaceholderSound());
	}

	// Token: 0x06002721 RID: 10017 RVA: 0x000BEFBC File Offset: 0x000BD1BC
	private void OnSoundLoaded(string name, GameObject go, object callbackData)
	{
		this.m_preloadsNeeded--;
		if (go == null)
		{
			Debug.LogWarning(string.Format("GameEntity.OnSoundLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("GameEntity.OnSoundLoaded() - ERROR \"{0}\" has no Spell component", name));
			return;
		}
		this.m_preloadedSounds.Add(name, component);
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000BF025 File Offset: 0x000BD225
	public void RemovePreloadedSound(string name)
	{
		this.m_preloadedSounds.Remove(name);
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000BF034 File Offset: 0x000BD234
	public bool CheckPreloadedSound(string name)
	{
		AudioSource audioSource;
		return this.m_preloadedSounds.TryGetValue(name, out audioSource);
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x000BF050 File Offset: 0x000BD250
	public AudioSource GetPreloadedSound(string name)
	{
		AudioSource result;
		if (this.m_preloadedSounds.TryGetValue(name, out result))
		{
			return result;
		}
		Debug.LogError(string.Format("GameEntity.GetPreloadedSound() - \"{0}\" was not preloaded", name));
		return null;
	}

	// Token: 0x06002725 RID: 10021 RVA: 0x000BF083 File Offset: 0x000BD283
	public bool IsPreloadingAssets()
	{
		return this.m_preloadsNeeded > 0;
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000BF08E File Offset: 0x000BD28E
	public override string GetName()
	{
		return "GameEntity";
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000BF095 File Offset: 0x000BD295
	public override string GetDebugName()
	{
		return "GameEntity";
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000BF09C File Offset: 0x000BD29C
	public override void OnTagsChanged(TagDeltaSet changeSet)
	{
		for (int i = 0; i < changeSet.Size(); i++)
		{
			TagDelta change = changeSet[i];
			this.OnTagChanged(change);
		}
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000BF0D0 File Offset: 0x000BD2D0
	public override void OnRealTimeTagChanged(Network.HistTagChange change)
	{
		GAME_TAG tag = (GAME_TAG)change.Tag;
		if (tag == GAME_TAG.MISSION_EVENT)
		{
			this.HandleRealTimeMissionEvent(change.Value);
		}
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x000BF106 File Offset: 0x000BD306
	public virtual void PreloadAssets()
	{
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x000BF108 File Offset: 0x000BD308
	public virtual void NotifyOfStartOfTurnEventsFinished()
	{
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000BF10A File Offset: 0x000BD30A
	public virtual bool NotifyOfEndTurnButtonPushed()
	{
		return true;
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000BF10D File Offset: 0x000BD30D
	public virtual bool NotifyOfBattlefieldCardClicked(Entity clickedEntity, bool wasInTargetMode)
	{
		return true;
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x000BF110 File Offset: 0x000BD310
	public virtual void NotifyOfCardMousedOver(Entity mousedOverEntity)
	{
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000BF112 File Offset: 0x000BD312
	public virtual void NotifyOfCardMousedOff(Entity mousedOffEntity)
	{
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000BF114 File Offset: 0x000BD314
	public virtual bool NotifyOfCardTooltipDisplayShow(Card card)
	{
		return true;
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000BF117 File Offset: 0x000BD317
	public virtual void NotifyOfCardTooltipDisplayHide(Card card)
	{
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000BF119 File Offset: 0x000BD319
	public virtual void NotifyOfCoinFlipResult()
	{
	}

	// Token: 0x06002733 RID: 10035 RVA: 0x000BF11B File Offset: 0x000BD31B
	public virtual bool NotifyOfPlayError(PlayErrors.ErrorType error, Entity errorSource)
	{
		return false;
	}

	// Token: 0x06002734 RID: 10036 RVA: 0x000BF11E File Offset: 0x000BD31E
	public virtual string[] NotifyOfKeywordHelpPanelDisplay(Entity entity)
	{
		return null;
	}

	// Token: 0x06002735 RID: 10037 RVA: 0x000BF121 File Offset: 0x000BD321
	public virtual void NotifyOfCardGrabbed(Entity entity)
	{
	}

	// Token: 0x06002736 RID: 10038 RVA: 0x000BF123 File Offset: 0x000BD323
	public virtual void NotifyOfCardDropped(Entity entity)
	{
	}

	// Token: 0x06002737 RID: 10039 RVA: 0x000BF125 File Offset: 0x000BD325
	public virtual void NotifyOfTargetModeCancelled()
	{
	}

	// Token: 0x06002738 RID: 10040 RVA: 0x000BF127 File Offset: 0x000BD327
	public virtual void NotifyOfHelpPanelDisplay(int numPanels)
	{
	}

	// Token: 0x06002739 RID: 10041 RVA: 0x000BF129 File Offset: 0x000BD329
	public virtual void NotifyOfDebugCommand(int command)
	{
	}

	// Token: 0x0600273A RID: 10042 RVA: 0x000BF12B File Offset: 0x000BD32B
	public virtual void NotifyOfManaCrystalSpawned()
	{
	}

	// Token: 0x0600273B RID: 10043 RVA: 0x000BF12D File Offset: 0x000BD32D
	public virtual void NotifyOfEnemyManaCrystalSpawned()
	{
	}

	// Token: 0x0600273C RID: 10044 RVA: 0x000BF12F File Offset: 0x000BD32F
	public virtual void NotifyOfTooltipZoneMouseOver(TooltipZone tooltip)
	{
	}

	// Token: 0x0600273D RID: 10045 RVA: 0x000BF131 File Offset: 0x000BD331
	public virtual void NotifyOfHistoryTokenMousedOver(GameObject mousedOverTile)
	{
	}

	// Token: 0x0600273E RID: 10046 RVA: 0x000BF133 File Offset: 0x000BD333
	public virtual void NotifyOfHistoryTokenMousedOut()
	{
	}

	// Token: 0x0600273F RID: 10047 RVA: 0x000BF135 File Offset: 0x000BD335
	public virtual void NotifyOfCustomIntroFinished()
	{
	}

	// Token: 0x06002740 RID: 10048 RVA: 0x000BF138 File Offset: 0x000BD338
	public virtual void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_EndGameScreen);
		Spell enemyBlowUpSpell;
		Spell friendlyBlowUpSpell;
		this.PlayHeroBlowUpSpells(gameResult, out enemyBlowUpSpell, out friendlyBlowUpSpell);
		this.ShowEndScreen(gameResult, enemyBlowUpSpell, friendlyBlowUpSpell);
	}

	// Token: 0x06002741 RID: 10049 RVA: 0x000BF171 File Offset: 0x000BD371
	public virtual void NotifyOfHeroesFinishedAnimatingInMulligan()
	{
	}

	// Token: 0x06002742 RID: 10050 RVA: 0x000BF173 File Offset: 0x000BD373
	public virtual bool NotifyOfTooltipDisplay(TooltipZone tooltip)
	{
		return false;
	}

	// Token: 0x06002743 RID: 10051 RVA: 0x000BF178 File Offset: 0x000BD378
	public virtual void NotifyOfMulliganInitialized()
	{
		if (GameMgr.Get().IsTutorial())
		{
			return;
		}
		AssetLoader.Get().LoadActor("EmoteHandler", new AssetLoader.GameObjectCallback(this.EmoteHandlerDoneLoadingCallback), null, false);
		if (GameMgr.Get().IsAI())
		{
			return;
		}
		AssetLoader.Get().LoadActor("EnemyEmoteHandler", new AssetLoader.GameObjectCallback(this.EnemyEmoteHandlerDoneLoadingCallback), null, false);
	}

	// Token: 0x06002744 RID: 10052 RVA: 0x000BF1E1 File Offset: 0x000BD3E1
	public virtual void NotifyOfMulliganEnded()
	{
	}

	// Token: 0x06002745 RID: 10053 RVA: 0x000BF1E4 File Offset: 0x000BD3E4
	private void EmoteHandlerDoneLoadingCallback(string actorName, GameObject actorObject, object callbackData)
	{
		actorObject.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.FRIENDLY).transform.position;
	}

	// Token: 0x06002746 RID: 10054 RVA: 0x000BF214 File Offset: 0x000BD414
	private void EnemyEmoteHandlerDoneLoadingCallback(string actorName, GameObject actorObject, object callbackData)
	{
		actorObject.transform.position = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position;
	}

	// Token: 0x06002747 RID: 10055 RVA: 0x000BF241 File Offset: 0x000BD441
	public virtual void NotifyOfGamePackOpened()
	{
	}

	// Token: 0x06002748 RID: 10056 RVA: 0x000BF243 File Offset: 0x000BD443
	public virtual void NotifyOfDefeatCoinAnimation()
	{
	}

	// Token: 0x06002749 RID: 10057 RVA: 0x000BF245 File Offset: 0x000BD445
	public virtual void SendCustomEvent(int eventID)
	{
	}

	// Token: 0x0600274A RID: 10058 RVA: 0x000BF247 File Offset: 0x000BD447
	public virtual string GetTurnStartReminderText()
	{
		return string.Empty;
	}

	// Token: 0x0600274B RID: 10059 RVA: 0x000BF24E File Offset: 0x000BD44E
	public virtual bool ShouldDoAlternateMulliganIntro()
	{
		return false;
	}

	// Token: 0x0600274C RID: 10060 RVA: 0x000BF251 File Offset: 0x000BD451
	public virtual bool DoAlternateMulliganIntro()
	{
		return false;
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000BF254 File Offset: 0x000BD454
	public virtual float GetAdditionalTimeToWaitForSpells()
	{
		return 0f;
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000BF25B File Offset: 0x000BD45B
	public virtual bool IsKeywordHelpDelayOverridden()
	{
		return false;
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000BF25E File Offset: 0x000BD45E
	public virtual bool IsMouseOverDelayOverriden()
	{
		return false;
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000BF261 File Offset: 0x000BD461
	public virtual bool AreTooltipsDisabled()
	{
		return false;
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000BF264 File Offset: 0x000BD464
	public virtual bool ShouldShowBigCard()
	{
		return true;
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000BF267 File Offset: 0x000BD467
	public virtual bool ShouldShowCrazyKeywordTooltip()
	{
		return false;
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000BF26A File Offset: 0x000BD46A
	public virtual bool ShouldShowHeroTooltips()
	{
		return false;
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000BF26D File Offset: 0x000BD46D
	public virtual bool ShouldDoOpeningTaunts()
	{
		return true;
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000BF270 File Offset: 0x000BD470
	public virtual bool ShouldHandleCoin()
	{
		return true;
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000BF273 File Offset: 0x000BD473
	public virtual List<RewardData> GetCustomRewards()
	{
		return null;
	}

	// Token: 0x06002757 RID: 10071 RVA: 0x000BF276 File Offset: 0x000BD476
	public virtual void HandleRealTimeMissionEvent(int missionEvent)
	{
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000BF278 File Offset: 0x000BD478
	public virtual void OnPlayThinkEmote()
	{
		if (GameMgr.Get().IsAI())
		{
			return;
		}
		EmoteType emoteType = EmoteType.THINK1;
		switch (Random.Range(1, 4))
		{
		case 1:
			emoteType = EmoteType.THINK1;
			break;
		case 2:
			emoteType = EmoteType.THINK2;
			break;
		case 3:
			emoteType = EmoteType.THINK3;
			break;
		}
		GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000BF2E9 File Offset: 0x000BD4E9
	public virtual void OnEmotePlayed(Card card, EmoteType emoteType, CardSoundSpell emoteSpell)
	{
	}

	// Token: 0x0600275A RID: 10074 RVA: 0x000BF2EB File Offset: 0x000BD4EB
	public virtual void NotifyOfOpponentWillPlayCard(string cardId)
	{
	}

	// Token: 0x0600275B RID: 10075 RVA: 0x000BF2ED File Offset: 0x000BD4ED
	public virtual void NotifyOfOpponentPlayedCard(Entity entity)
	{
	}

	// Token: 0x0600275C RID: 10076 RVA: 0x000BF2EF File Offset: 0x000BD4EF
	public virtual void NotifyOfFriendlyPlayedCard(Entity entity)
	{
	}

	// Token: 0x0600275D RID: 10077 RVA: 0x000BF2F1 File Offset: 0x000BD4F1
	public virtual string UpdateCardText(Card card, Actor bigCardActor, string text)
	{
		return text;
	}

	// Token: 0x0600275E RID: 10078 RVA: 0x000BF2F4 File Offset: 0x000BD4F4
	public virtual bool ShouldUseSecretClassNames()
	{
		return false;
	}

	// Token: 0x0600275F RID: 10079 RVA: 0x000BF2F8 File Offset: 0x000BD4F8
	public virtual void StartGameplaySoundtracks()
	{
		Board board = Board.Get();
		MusicPlaylistType type = (!(board == null)) ? board.m_BoardMusic : MusicPlaylistType.InGame_Default;
		MusicManager.Get().StartPlaylist(type);
	}

	// Token: 0x06002760 RID: 10080 RVA: 0x000BF334 File Offset: 0x000BD534
	public virtual string GetAlternatePlayerName()
	{
		return string.Empty;
	}

	// Token: 0x06002761 RID: 10081 RVA: 0x000BF33C File Offset: 0x000BD53C
	public virtual IEnumerator PlayMissionIntroLineAndWait()
	{
		yield break;
	}

	// Token: 0x06002762 RID: 10082 RVA: 0x000BF350 File Offset: 0x000BD550
	public virtual IEnumerator DoActionsAfterIntroBeforeMulligan()
	{
		yield break;
	}

	// Token: 0x06002763 RID: 10083 RVA: 0x000BF364 File Offset: 0x000BD564
	public virtual bool IsEnemySpeaking()
	{
		return false;
	}

	// Token: 0x06002764 RID: 10084 RVA: 0x000BF367 File Offset: 0x000BD567
	public virtual bool ShouldDelayCardSoundSpells()
	{
		return false;
	}

	// Token: 0x06002765 RID: 10085 RVA: 0x000BF36A File Offset: 0x000BD56A
	public virtual bool ShouldAllowCardGrab(Entity entity)
	{
		return true;
	}

	// Token: 0x06002766 RID: 10086 RVA: 0x000BF36D File Offset: 0x000BD56D
	public virtual string CustomChoiceBannerText()
	{
		return null;
	}

	// Token: 0x06002767 RID: 10087 RVA: 0x000BF370 File Offset: 0x000BD570
	protected virtual Spell BlowUpHero(Card card, SpellType spellType)
	{
		Spell result = card.ActivateActorSpell(spellType);
		Gameplay.Get().StartCoroutine(this.HideOtherElements(card));
		return result;
	}

	// Token: 0x06002768 RID: 10088 RVA: 0x000BF398 File Offset: 0x000BD598
	protected IEnumerator HideOtherElements(Card card)
	{
		yield return new WaitForSeconds(0.5f);
		Player controller = card.GetEntity().GetController();
		if (controller.GetHeroPowerCard() != null)
		{
			controller.GetHeroPowerCard().HideCard();
			controller.GetHeroPowerCard().GetActor().ToggleForceIdle(true);
			controller.GetHeroPowerCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
			controller.GetHeroPowerCard().GetActor().DeactivateAllPreDeathSpells();
		}
		if (controller.GetWeaponCard() != null)
		{
			controller.GetWeaponCard().HideCard();
			controller.GetWeaponCard().GetActor().ToggleForceIdle(true);
			controller.GetWeaponCard().GetActor().SetActorState(ActorStateType.CARD_IDLE);
			controller.GetWeaponCard().GetActor().DeactivateAllPreDeathSpells();
		}
		card.GetActor().HideArmorSpell();
		card.GetActor().GetHealthObject().Hide();
		card.GetActor().GetAttackObject().Hide();
		card.GetActor().ToggleForceIdle(true);
		card.GetActor().SetActorState(ActorStateType.CARD_IDLE);
		yield break;
	}

	// Token: 0x06002769 RID: 10089 RVA: 0x000BF3BC File Offset: 0x000BD5BC
	private void PlayHeroBlowUpSpells(TAG_PLAYSTATE gameResult, out Spell enemyBlowUpSpell, out Spell friendlyBlowUpSpell)
	{
		enemyBlowUpSpell = null;
		friendlyBlowUpSpell = null;
		Card heroCard = GameState.Get().GetOpposingSidePlayer().GetHeroCard();
		Card heroCard2 = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			enemyBlowUpSpell = this.BlowUpHero(heroCard, SpellType.ENDGAME_WIN);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			enemyBlowUpSpell = this.BlowUpHero(heroCard, SpellType.ENDGAME_DRAW);
			friendlyBlowUpSpell = this.BlowUpHero(heroCard2, SpellType.ENDGAME_LOSE);
		}
	}

	// Token: 0x0600276A RID: 10090 RVA: 0x000BF43C File Offset: 0x000BD63C
	private void ShowEndScreen(TAG_PLAYSTATE gameResult, Spell enemyBlowUpSpell, Spell friendlyBlowUpSpell)
	{
		GameEntity.EndGameScreenContext endGameScreenContext = new GameEntity.EndGameScreenContext();
		endGameScreenContext.m_enemyBlowUpSpell = enemyBlowUpSpell;
		endGameScreenContext.m_friendlyBlowUpSpell = friendlyBlowUpSpell;
		if (gameResult == TAG_PLAYSTATE.WON)
		{
			SoundManager.Get().LoadAndPlay("victory_jingle");
			if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.DRAFT && DraftManager.Get() != null && DraftManager.Get().GetWins() == 11)
			{
				DraftManager.Get().NotifyOfFinalGame(true);
			}
			AssetLoader.Get().LoadActor("VictoryTwoScoop", new AssetLoader.GameObjectCallback(this.OnEndGameScreenLoaded), endGameScreenContext, false);
		}
		else if (gameResult == TAG_PLAYSTATE.LOST)
		{
			SoundManager.Get().LoadAndPlay("defeat_jingle");
			if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.DRAFT && DraftManager.Get() != null && DraftManager.Get().GetLosses() == 2)
			{
				DraftManager.Get().NotifyOfFinalGame(false);
			}
			AssetLoader.Get().LoadActor("DefeatTwoScoop", new AssetLoader.GameObjectCallback(this.OnEndGameScreenLoaded), endGameScreenContext, false);
		}
		else if (gameResult == TAG_PLAYSTATE.TIED)
		{
			SoundManager.Get().LoadAndPlay("defeat_jingle");
			if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.DRAFT && DraftManager.Get() != null && DraftManager.Get().GetLosses() == 2)
			{
				DraftManager.Get().NotifyOfFinalGame(false);
			}
			AssetLoader.Get().LoadActor("DefeatTwoScoop", new AssetLoader.GameObjectCallback(this.OnEndGameScreenLoaded), endGameScreenContext, false);
		}
	}

	// Token: 0x0600276B RID: 10091 RVA: 0x000BF5A8 File Offset: 0x000BD7A8
	protected void OnEndGameScreenLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			Debug.LogError(string.Format("GameEntity.OnEndGameScreenLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		EndGameScreen component = go.GetComponent<EndGameScreen>();
		GameEntity.EndGameScreenContext endGameScreenContext = (GameEntity.EndGameScreenContext)callbackData;
		endGameScreenContext.ShowScreen(component);
	}

	// Token: 0x04001733 RID: 5939
	private Map<string, AudioSource> m_preloadedSounds = new Map<string, AudioSource>();

	// Token: 0x04001734 RID: 5940
	private int m_preloadsNeeded;

	// Token: 0x0200086D RID: 2157
	protected class EndGameScreenContext
	{
		// Token: 0x060052CB RID: 21195 RVA: 0x0018B22C File Offset: 0x0018942C
		public void ShowScreen(EndGameScreen screen)
		{
			bool flag = false;
			if (this.m_enemyBlowUpSpell != null && !this.m_enemyBlowUpSpell.IsFinished())
			{
				flag = true;
				this.m_enemyBlowUpSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBlowUpSpellFinished), screen);
			}
			if (this.m_friendlyBlowUpSpell != null && !this.m_friendlyBlowUpSpell.IsFinished())
			{
				flag = true;
				this.m_friendlyBlowUpSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnBlowUpSpellFinished), screen);
			}
			if (!flag)
			{
				screen.Show();
			}
		}

		// Token: 0x060052CC RID: 21196 RVA: 0x0018B2C0 File Offset: 0x001894C0
		private void OnBlowUpSpellFinished(Spell spell, object userData)
		{
			EndGameScreen endGameScreen = (EndGameScreen)userData;
			if (this.AreBlowUpSpellsFinished())
			{
				endGameScreen.Show();
			}
		}

		// Token: 0x060052CD RID: 21197 RVA: 0x0018B2E8 File Offset: 0x001894E8
		private bool AreBlowUpSpellsFinished()
		{
			return (!(this.m_enemyBlowUpSpell != null) || this.m_enemyBlowUpSpell.IsFinished()) && (!(this.m_friendlyBlowUpSpell != null) || this.m_friendlyBlowUpSpell.IsFinished());
		}

		// Token: 0x04003909 RID: 14601
		public Spell m_enemyBlowUpSpell;

		// Token: 0x0400390A RID: 14602
		public Spell m_friendlyBlowUpSpell;
	}
}
