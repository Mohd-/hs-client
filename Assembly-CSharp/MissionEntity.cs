using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007CB RID: 1995
public class MissionEntity : GameEntity
{
	// Token: 0x06004DD8 RID: 19928 RVA: 0x001728A8 File Offset: 0x00170AA8
	public MissionEntity()
	{
		this.InitEmoteResponses();
	}

	// Token: 0x06004DD9 RID: 19929 RVA: 0x001728C4 File Offset: 0x00170AC4
	// Note: this type is marked as 'beforefieldinit'.
	static MissionEntity()
	{
		List<EmoteType> list = new List<EmoteType>();
		list.Add(EmoteType.GREETINGS);
		list.Add(EmoteType.WELL_PLAYED);
		list.Add(EmoteType.OOPS);
		list.Add(EmoteType.SORRY);
		list.Add(EmoteType.THANKS);
		list.Add(EmoteType.THREATEN);
		list.Add(EmoteType.WOW);
		MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS = list;
	}

	// Token: 0x06004DDA RID: 19930 RVA: 0x00172910 File Offset: 0x00170B10
	public override void OnTagChanged(TagDelta change)
	{
		GAME_TAG tag = (GAME_TAG)change.tag;
		if (tag != GAME_TAG.MISSION_EVENT)
		{
			if (tag != GAME_TAG.STEP)
			{
				if (tag == GAME_TAG.NEXT_STEP)
				{
					if (change.newValue == 6)
					{
						if (GameState.Get().IsMulliganManagerActive())
						{
							GameState.Get().SetMulliganBusy(true);
						}
					}
					else if (change.oldValue == 9 && change.newValue == 10 && GameState.Get().IsFriendlySidePlayerTurn())
					{
						TurnStartManager.Get().BeginPlayingTurnEvents();
					}
				}
			}
			else if (change.newValue == 4)
			{
				this.HandleMulliganTagChange();
			}
			else if (change.oldValue == 9 && change.newValue == 10 && !GameState.Get().IsFriendlySidePlayerTurn())
			{
				this.HandleStartOfTurn(base.GetTag(GAME_TAG.TURN));
			}
		}
		else
		{
			this.HandleMissionEvent(change.newValue);
		}
		base.OnTagChanged(change);
	}

	// Token: 0x06004DDB RID: 19931 RVA: 0x00172A16 File Offset: 0x00170C16
	public override void NotifyOfStartOfTurnEventsFinished()
	{
		this.HandleStartOfTurn(base.GetTag(GAME_TAG.TURN));
	}

	// Token: 0x06004DDC RID: 19932 RVA: 0x00172A26 File Offset: 0x00170C26
	public override void SendCustomEvent(int eventID)
	{
		this.HandleMissionEvent(eventID);
	}

	// Token: 0x06004DDD RID: 19933 RVA: 0x00172A2F File Offset: 0x00170C2F
	public override void NotifyOfOpponentWillPlayCard(string cardId)
	{
		base.NotifyOfOpponentWillPlayCard(cardId);
		Gameplay.Get().StartCoroutine(this.RespondToWillPlayCardWithTiming(cardId));
	}

	// Token: 0x06004DDE RID: 19934 RVA: 0x00172A4A File Offset: 0x00170C4A
	public override void NotifyOfOpponentPlayedCard(Entity entity)
	{
		base.NotifyOfOpponentPlayedCard(entity);
		Gameplay.Get().StartCoroutine(this.RespondToPlayedCardWithTiming(entity));
	}

	// Token: 0x06004DDF RID: 19935 RVA: 0x00172A65 File Offset: 0x00170C65
	public override void NotifyOfFriendlyPlayedCard(Entity entity)
	{
		base.NotifyOfFriendlyPlayedCard(entity);
		Gameplay.Get().StartCoroutine(this.RespondToFriendlyPlayedCardWithTiming(entity));
	}

	// Token: 0x06004DE0 RID: 19936 RVA: 0x00172A80 File Offset: 0x00170C80
	public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
	{
		base.NotifyOfGameOver(gameResult);
		Gameplay.Get().StartCoroutine(this.HandleGameOverWithTiming(gameResult));
	}

	// Token: 0x06004DE1 RID: 19937 RVA: 0x00172A9B File Offset: 0x00170C9B
	public override bool ShouldUseSecretClassNames()
	{
		return true;
	}

	// Token: 0x06004DE2 RID: 19938 RVA: 0x00172AA0 File Offset: 0x00170CA0
	public override void OnEmotePlayed(Card card, EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		if (card.GetEntity().IsControlledByLocalUser())
		{
			Gameplay.Get().StartCoroutine(this.HandlePlayerEmoteWithTiming(emoteType, emoteSpell));
		}
	}

	// Token: 0x06004DE3 RID: 19939 RVA: 0x00172AD0 File Offset: 0x00170CD0
	public override bool IsEnemySpeaking()
	{
		return this.m_enemySpeaking;
	}

	// Token: 0x06004DE4 RID: 19940 RVA: 0x00172AD8 File Offset: 0x00170CD8
	public override bool ShouldDelayCardSoundSpells()
	{
		return this.m_delayCardSoundSpells;
	}

	// Token: 0x06004DE5 RID: 19941 RVA: 0x00172AE0 File Offset: 0x00170CE0
	public override bool DoAlternateMulliganIntro()
	{
		if (!this.ShouldDoAlternateMulliganIntro())
		{
			return false;
		}
		Gameplay.Get().StartCoroutine(this.DoAlternateMulliganIntroWithTiming());
		return true;
	}

	// Token: 0x06004DE6 RID: 19942 RVA: 0x00172B0C File Offset: 0x00170D0C
	protected virtual void HandleMulliganTagChange()
	{
		MulliganManager.Get().BeginMulligan();
	}

	// Token: 0x06004DE7 RID: 19943 RVA: 0x00172B18 File Offset: 0x00170D18
	protected void HandleStartOfTurn(int turn)
	{
		Gameplay.Get().StartCoroutine(this.HandleStartOfTurnWithTiming(turn));
	}

	// Token: 0x06004DE8 RID: 19944 RVA: 0x00172B2C File Offset: 0x00170D2C
	protected virtual IEnumerator HandleStartOfTurnWithTiming(int turn)
	{
		yield break;
	}

	// Token: 0x06004DE9 RID: 19945 RVA: 0x00172B40 File Offset: 0x00170D40
	protected void HandleMissionEvent(int missionEvent)
	{
		Gameplay.Get().StartCoroutine(this.HandleMissionEventWithTiming(missionEvent));
	}

	// Token: 0x06004DEA RID: 19946 RVA: 0x00172B54 File Offset: 0x00170D54
	protected virtual IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		yield break;
	}

	// Token: 0x06004DEB RID: 19947 RVA: 0x00172B68 File Offset: 0x00170D68
	protected virtual IEnumerator RespondToWillPlayCardWithTiming(string cardId)
	{
		yield break;
	}

	// Token: 0x06004DEC RID: 19948 RVA: 0x00172B7C File Offset: 0x00170D7C
	protected virtual IEnumerator RespondToPlayedCardWithTiming(Entity entity)
	{
		yield break;
	}

	// Token: 0x06004DED RID: 19949 RVA: 0x00172B90 File Offset: 0x00170D90
	protected virtual IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
	{
		yield break;
	}

	// Token: 0x06004DEE RID: 19950 RVA: 0x00172BA4 File Offset: 0x00170DA4
	protected virtual IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
	{
		yield break;
	}

	// Token: 0x06004DEF RID: 19951 RVA: 0x00172BB8 File Offset: 0x00170DB8
	protected IEnumerator DoAlternateMulliganIntroWithTiming()
	{
		Board.Get().RaiseTheLights();
		SceneMgr.Get().NotifySceneLoaded();
		while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
		{
			yield return null;
		}
		GameMgr.Get().UpdatePresence();
		MulliganManager.Get().SkipMulligan();
		yield break;
	}

	// Token: 0x06004DF0 RID: 19952 RVA: 0x00172BCC File Offset: 0x00170DCC
	protected void PlaySound(string audioName, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioName, null, Notification.SpeechBubbleDirection.None, null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, 3f));
	}

	// Token: 0x06004DF1 RID: 19953 RVA: 0x00172BF8 File Offset: 0x00170DF8
	protected IEnumerator PlaySoundAndBlockSpeech(string audioName, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		this.m_enemySpeaking = true;
		yield return Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioName, null, Notification.SpeechBubbleDirection.None, null, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, 3f));
		this.m_enemySpeaking = false;
		yield break;
	}

	// Token: 0x06004DF2 RID: 19954 RVA: 0x00172C50 File Offset: 0x00170E50
	protected IEnumerator PlaySoundAndBlockSpeech(string audioName, string stringName, Notification.SpeechBubbleDirection direction, Actor actor, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		this.m_enemySpeaking = true;
		yield return Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioName, stringName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, 3f));
		this.m_enemySpeaking = false;
		yield break;
	}

	// Token: 0x06004DF3 RID: 19955 RVA: 0x00172CD8 File Offset: 0x00170ED8
	protected IEnumerator PlaySoundAndBlockSpeech(string audioID, Notification.SpeechBubbleDirection direction, Actor actor, float testingDuration = 3f, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		this.m_enemySpeaking = true;
		yield return Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioID, audioID, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration));
		this.m_enemySpeaking = false;
		yield break;
	}

	// Token: 0x06004DF4 RID: 19956 RVA: 0x00172D60 File Offset: 0x00170F60
	protected IEnumerator PlaySoundAndBlockSpeechOnce(string audioName, string stringName, Notification.SpeechBubbleDirection direction, Actor actor, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		if (NotificationManager.Get().HasSoundPlayedThisSession(audioName))
		{
			yield break;
		}
		NotificationManager.Get().ForceAddSoundToPlayedList(audioName);
		this.m_enemySpeaking = true;
		yield return Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioName, stringName, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, 3f));
		this.m_enemySpeaking = false;
		yield break;
	}

	// Token: 0x06004DF5 RID: 19957 RVA: 0x00172DE8 File Offset: 0x00170FE8
	protected IEnumerator PlaySoundAndBlockSpeechOnce(string audioID, Notification.SpeechBubbleDirection direction, Actor actor, float testingDuration = 3f, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false)
	{
		if (NotificationManager.Get().HasSoundPlayedThisSession(audioID))
		{
			yield break;
		}
		NotificationManager.Get().ForceAddSoundToPlayedList(audioID);
		this.m_enemySpeaking = true;
		yield return Gameplay.Get().StartCoroutine(this.PlaySoundAndWait(audioID, audioID, direction, actor, waitTimeScale, parentBubbleToActor, delayCardSoundSpells, testingDuration));
		this.m_enemySpeaking = false;
		yield break;
	}

	// Token: 0x06004DF6 RID: 19958 RVA: 0x00172E70 File Offset: 0x00171070
	protected IEnumerator PlaySoundAndWait(string audioName, string stringName, Notification.SpeechBubbleDirection direction, Actor actor, float waitTimeScale = 1f, bool parentBubbleToActor = true, bool delayCardSoundSpells = false, float testingDuration = 3f)
	{
		AudioSource sound = null;
		bool isJustTesting = false;
		if (string.IsNullOrEmpty(audioName) || !base.CheckPreloadedSound(audioName))
		{
			isJustTesting = true;
		}
		else
		{
			sound = base.GetPreloadedSound(audioName);
		}
		if (!isJustTesting && (sound == null || sound.clip == null))
		{
			if (base.CheckPreloadedSound(audioName))
			{
				base.RemovePreloadedSound(audioName);
				base.PreloadSound(audioName);
				while (base.IsPreloadingAssets())
				{
					yield return null;
				}
				sound = base.GetPreloadedSound(audioName);
			}
			if (sound == null || sound.clip == null)
			{
				Debug.Log("MissionEntity.PlaySoundAndWait() - sound error - " + audioName);
				yield break;
			}
		}
		float clipLength = testingDuration;
		if (!isJustTesting)
		{
			clipLength = sound.clip.length;
		}
		float waitTime = clipLength * waitTimeScale;
		if (!isJustTesting)
		{
			SoundManager.Get().PlayPreloaded(sound);
		}
		if (delayCardSoundSpells)
		{
			Gameplay.Get().StartCoroutine(this.WaitForCardSoundSpellDelay(clipLength));
		}
		if (direction != Notification.SpeechBubbleDirection.None)
		{
			this.ShowBubble(stringName, direction, actor, false, clipLength, parentBubbleToActor);
			waitTime += 0.5f;
		}
		yield return new WaitForSeconds(waitTime);
		yield break;
	}

	// Token: 0x06004DF7 RID: 19959 RVA: 0x00172F08 File Offset: 0x00171108
	protected IEnumerator PlayCharacterQuoteAndWait(string prefabName, string audioID, float testingDuration = 0f, bool allowRepeatDuringSession = true, bool delayCardSoundSpells = false)
	{
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioID, audioID, NotificationManager.DEFAULT_CHARACTER_POS, 1f, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, false));
		yield break;
	}

	// Token: 0x06004DF8 RID: 19960 RVA: 0x00172F70 File Offset: 0x00171170
	protected IEnumerator PlayCharacterQuoteAndWait(string prefabName, string audioName, string stringName, float testingDuration = 0f, bool allowRepeatDuringSession = true, bool delayCardSoundSpells = false)
	{
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioName, stringName, NotificationManager.DEFAULT_CHARACTER_POS, 1f, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, false));
		yield break;
	}

	// Token: 0x06004DF9 RID: 19961 RVA: 0x00172FE8 File Offset: 0x001711E8
	protected IEnumerator PlayCharacterQuoteAndWait(string prefabName, string audioName, string stringName, Vector3 position, float waitTimeScale = 1f, float testingDuration = 0f, bool allowRepeatDuringSession = true, bool delayCardSoundSpells = false, bool isBig = false)
	{
		AudioSource sound = null;
		bool isJustTesting = false;
		if (string.IsNullOrEmpty(audioName) || !base.CheckPreloadedSound(audioName))
		{
			isJustTesting = true;
		}
		else
		{
			sound = base.GetPreloadedSound(audioName);
		}
		if (!isJustTesting && (sound == null || sound.clip == null))
		{
			if (base.CheckPreloadedSound(audioName))
			{
				base.RemovePreloadedSound(audioName);
				base.PreloadSound(audioName);
				while (base.IsPreloadingAssets())
				{
					yield return null;
				}
				sound = base.GetPreloadedSound(audioName);
			}
			if (sound == null || sound.clip == null)
			{
				Debug.Log("MissionEntity.PlaySoundAndWait() - sound error - " + audioName);
				yield break;
			}
		}
		float clipLength;
		if (isJustTesting)
		{
			clipLength = testingDuration;
		}
		else
		{
			clipLength = sound.clip.length;
		}
		float waitTime = clipLength * waitTimeScale;
		if (delayCardSoundSpells)
		{
			Gameplay.Get().StartCoroutine(this.WaitForCardSoundSpellDelay(clipLength));
		}
		if (isBig)
		{
			NotificationManager.Get().CreateBigCharacterQuote(prefabName, audioName, stringName, allowRepeatDuringSession, testingDuration, null);
		}
		else
		{
			NotificationManager.Get().CreateCharacterQuote(prefabName, GameStrings.Get(stringName), audioName, allowRepeatDuringSession, testingDuration * 2f, CanvasAnchor.BOTTOM_LEFT);
		}
		waitTime += 0.5f;
		yield return new WaitForSeconds(waitTime);
		NotificationManager.Get().DestroyActiveQuote(0f);
		yield break;
	}

	// Token: 0x06004DFA RID: 19962 RVA: 0x00173080 File Offset: 0x00171280
	protected IEnumerator PlayBigCharacterQuoteAndWait(string prefabName, string audioName, float testingDuration = 3f, float waitTimeScale = 1f, bool allowRepeatDuringSession = true, bool delayCardSoundSpells = false)
	{
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioName, audioName, Vector3.zero, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true));
		yield break;
	}

	// Token: 0x06004DFB RID: 19963 RVA: 0x001730F8 File Offset: 0x001712F8
	protected IEnumerator PlayBigCharacterQuoteAndWait(string prefabName, string audioName, string textID, float testingDuration = 3f, float waitTimeScale = 1f, bool allowRepeatDuringSession = true, bool delayCardSoundSpells = false)
	{
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioName, textID, Vector3.zero, waitTimeScale, testingDuration, allowRepeatDuringSession, delayCardSoundSpells, true));
		yield break;
	}

	// Token: 0x06004DFC RID: 19964 RVA: 0x00173180 File Offset: 0x00171380
	protected IEnumerator PlayBigCharacterQuoteAndWaitOnce(string prefabName, string audioName, float testingDuration = 3f, float waitTimeScale = 1f, bool delayCardSoundSpells = false)
	{
		bool allowRepeat = DemoMgr.Get().IsExpoDemo();
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioName, audioName, Vector3.zero, waitTimeScale, testingDuration, allowRepeat, delayCardSoundSpells, true));
		yield break;
	}

	// Token: 0x06004DFD RID: 19965 RVA: 0x001731E8 File Offset: 0x001713E8
	protected IEnumerator PlayBigCharacterQuoteAndWaitOnce(string prefabName, string audioName, string textID, float testingDuration = 3f, float waitTimeScale = 1f, bool delayCardSoundSpells = false)
	{
		bool allowRepeat = DemoMgr.Get().IsExpoDemo();
		yield return Gameplay.Get().StartCoroutine(this.PlayCharacterQuoteAndWait(prefabName, audioName, textID, Vector3.zero, waitTimeScale, testingDuration, allowRepeat, delayCardSoundSpells, true));
		yield break;
	}

	// Token: 0x06004DFE RID: 19966 RVA: 0x00173260 File Offset: 0x00171460
	protected IEnumerator WaitForCardSoundSpellDelay(float sec)
	{
		this.m_delayCardSoundSpells = true;
		yield return new WaitForSeconds(sec);
		this.m_delayCardSoundSpells = false;
		yield break;
	}

	// Token: 0x06004DFF RID: 19967 RVA: 0x0017328C File Offset: 0x0017148C
	protected void ShowBubble(string textKey, Notification.SpeechBubbleDirection direction, Actor speakingActor, bool destroyOnNewNotification, float duration, bool parentToActor)
	{
		NotificationManager notificationManager = NotificationManager.Get();
		Notification notification;
		if (SoundUtils.CanDetectVolume() && SoundUtils.IsVoiceAudible() && GameMgr.Get().IsTutorial())
		{
			notification = notificationManager.CreateSpeechBubble(string.Empty, direction, speakingActor, destroyOnNewNotification, parentToActor);
			float num = 0.25f;
			Vector3 localScale;
			localScale..ctor(notification.transform.localScale.x * num, notification.transform.localScale.y * num, notification.transform.localScale.z * num);
			notification.transform.localScale = localScale;
		}
		else
		{
			notification = notificationManager.CreateSpeechBubble(GameStrings.Get(textKey), direction, speakingActor, destroyOnNewNotification, parentToActor);
		}
		if (duration > 0f)
		{
			notificationManager.DestroyNotification(notification, duration);
		}
	}

	// Token: 0x06004E00 RID: 19968 RVA: 0x0017335C File Offset: 0x0017155C
	protected virtual void InitEmoteResponses()
	{
	}

	// Token: 0x06004E01 RID: 19969 RVA: 0x00173360 File Offset: 0x00171560
	protected IEnumerator HandlePlayerEmoteWithTiming(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		while (emoteSpell.IsActive())
		{
			yield return null;
		}
		if (this.m_enemySpeaking)
		{
			yield break;
		}
		this.PlayEmoteResponse(emoteType, emoteSpell);
		yield break;
	}

	// Token: 0x06004E02 RID: 19970 RVA: 0x00173398 File Offset: 0x00171598
	protected virtual void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
	{
		Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
		foreach (MissionEntity.EmoteResponseGroup emoteResponseGroup in this.m_emoteResponseGroups)
		{
			if (emoteResponseGroup.m_responses.Count != 0)
			{
				if (emoteResponseGroup.m_triggers.Contains(emoteType))
				{
					int responseIndex = emoteResponseGroup.m_responseIndex;
					MissionEntity.EmoteResponse emoteResponse = emoteResponseGroup.m_responses[responseIndex];
					Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(emoteResponse.m_soundName, emoteResponse.m_stringTag, Notification.SpeechBubbleDirection.TopRight, actor, 1f, true, false));
					this.CycleNextResponseGroupIndex(emoteResponseGroup);
				}
			}
		}
	}

	// Token: 0x06004E03 RID: 19971 RVA: 0x00173470 File Offset: 0x00171670
	protected virtual void CycleNextResponseGroupIndex(MissionEntity.EmoteResponseGroup responseGroup)
	{
		if (responseGroup.m_responseIndex == responseGroup.m_responses.Count - 1)
		{
			responseGroup.m_responseIndex = 0;
		}
		else
		{
			responseGroup.m_responseIndex++;
		}
	}

	// Token: 0x0400350A RID: 13578
	protected const float TIME_TO_WAIT_BEFORE_ENDING_QUOTE = 5f;

	// Token: 0x0400350B RID: 13579
	protected static readonly List<EmoteType> STANDARD_EMOTE_RESPONSE_TRIGGERS;

	// Token: 0x0400350C RID: 13580
	protected bool m_enemySpeaking;

	// Token: 0x0400350D RID: 13581
	protected bool m_delayCardSoundSpells;

	// Token: 0x0400350E RID: 13582
	protected List<MissionEntity.EmoteResponseGroup> m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>();

	// Token: 0x02000991 RID: 2449
	protected class EmoteResponseGroup
	{
		// Token: 0x04003FB0 RID: 16304
		public List<EmoteType> m_triggers = new List<EmoteType>();

		// Token: 0x04003FB1 RID: 16305
		public List<MissionEntity.EmoteResponse> m_responses = new List<MissionEntity.EmoteResponse>();

		// Token: 0x04003FB2 RID: 16306
		public int m_responseIndex;
	}

	// Token: 0x02000992 RID: 2450
	protected class EmoteResponse
	{
		// Token: 0x04003FB3 RID: 16307
		public string m_soundName;

		// Token: 0x04003FB4 RID: 16308
		public string m_stringTag;
	}
}
