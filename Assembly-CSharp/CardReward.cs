using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000272 RID: 626
public class CardReward : Reward
{
	// Token: 0x06002336 RID: 9014 RVA: 0x000AD9C0 File Offset: 0x000ABBC0
	public void MakeActorsUnlit()
	{
		foreach (Actor actor in this.m_actors)
		{
			actor.SetUnlit();
		}
	}

	// Token: 0x06002337 RID: 9015 RVA: 0x000ADA1C File Offset: 0x000ABC1C
	protected override void InitData()
	{
		base.SetData(new CardRewardData(), false);
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000ADA2C File Offset: 0x000ABC2C
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		CardRewardData cardRewardData = base.Data as CardRewardData;
		if (cardRewardData == null)
		{
			Debug.LogWarning(string.Format("CardReward.SetData() - data {0} is not CardRewardData", base.Data));
			return;
		}
		if (string.IsNullOrEmpty(cardRewardData.CardID))
		{
			Debug.LogWarning(string.Format("CardReward.SetData() - data {0} has invalid cardID", cardRewardData));
			return;
		}
		base.SetReady(false);
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
		if (entityDef.IsHero())
		{
			AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnHeroActorLoaded), entityDef, false);
			this.m_goToRotate = this.m_heroCardRoot;
			this.m_cardCount.Hide();
			if (cardRewardData.Premium == TAG_PREMIUM.GOLDEN)
			{
				this.SetUpGoldenHeroAchieves();
			}
			else
			{
				this.SetupHeroAchieves();
			}
		}
		else
		{
			if (UniversalInputManager.UsePhoneUI)
			{
				this.m_cardCount.Hide();
			}
			string handActor = ActorNames.GetHandActor(entityDef, cardRewardData.Premium);
			AssetLoader.Get().LoadActor(handActor, new AssetLoader.GameObjectCallback(this.OnActorLoaded), entityDef, false);
			this.m_goToRotate = this.m_nonHeroCardsRoot;
		}
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000ADB50 File Offset: 0x000ABD50
	protected override void ShowReward(bool updateCacheValues)
	{
		CardRewardData cardRewardData = base.Data as CardRewardData;
		if (!cardRewardData.IsDummyReward && updateCacheValues)
		{
			CollectionManager.Get().AddCardReward(cardRewardData, true);
		}
		this.InitRewardText();
		if (cardRewardData.FixedReward != null && cardRewardData.FixedReward.UseQuestToast && DefLoader.Get().GetEntityDef(cardRewardData.CardID).IsHero() && this.m_rewardBanner != null)
		{
			this.m_rewardBanner.gameObject.SetActive(false);
		}
		this.m_root.SetActive(true);
		this.m_goToRotate.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, 540f),
			"time",
			1.5f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1
		});
		iTween.RotateAdd(this.m_goToRotate.gameObject, args);
		SoundManager.Get().LoadAndPlay("game_end_reward");
		this.PlayHeroEmote();
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000ADCA1 File Offset: 0x000ABEA1
	protected override void HideReward()
	{
		base.HideReward();
		this.m_root.SetActive(false);
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000ADCB8 File Offset: 0x000ABEB8
	private void OnCardDefLoaded(string cardID, CardDef cardDef, object callbackData)
	{
		if (DefLoader.Get().GetEntityDef(cardID) == null)
		{
			Debug.LogWarning(string.Format("OnCardDefLoaded() - entityDef for CardID {0} is null", cardID));
			return;
		}
		foreach (Actor actor in this.m_actors)
		{
			this.FinishSettingUpActor(actor, cardDef);
		}
		foreach (EmoteEntryDef emoteEntryDef in cardDef.m_EmoteDefs)
		{
			if (emoteEntryDef.m_emoteType == EmoteType.START)
			{
				AssetLoader.Get().LoadSpell(emoteEntryDef.m_emoteSoundSpellPath, new AssetLoader.GameObjectCallback(this.OnStartEmoteLoaded), null, false);
			}
		}
		base.SetReady(true);
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000ADDAC File Offset: 0x000ABFAC
	private void OnStartEmoteLoaded(string name, GameObject go, object callbackData)
	{
		if (go == null)
		{
			return;
		}
		CardSoundSpell component = go.GetComponent<CardSoundSpell>();
		if (component == null)
		{
			return;
		}
		this.m_emote = component;
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000ADDE1 File Offset: 0x000ABFE1
	private void PlayHeroEmote()
	{
		if (this.m_emote == null)
		{
			return;
		}
		this.m_emote.Reactivate();
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000ADE00 File Offset: 0x000AC000
	private void OnHeroActorLoaded(string name, GameObject go, object callbackData)
	{
		EntityDef entityDef = (EntityDef)callbackData;
		Actor component = go.GetComponent<Actor>();
		component.SetEntityDef(entityDef);
		component.transform.parent = this.m_heroCardRoot.transform;
		component.transform.localScale = Vector3.one;
		component.transform.localPosition = Vector3.zero;
		component.transform.localRotation = Quaternion.identity;
		component.TurnOffCollider();
		component.m_healthObject.SetActive(false);
		CardRewardData cardRewardData = base.Data as CardRewardData;
		if (cardRewardData.FixedReward != null && cardRewardData.FixedReward.UseQuestToast)
		{
			PlatformDependentValue<Vector3> val = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
			{
				PC = new Vector3(2f, 2f, 2f),
				Phone = new Vector3(1.3f, 1.3f, 1.3f)
			};
			PlatformDependentValue<Vector3> val2 = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
			{
				PC = new Vector3(0f, 0f, -0.45f),
				Phone = new Vector3(0f, 0f, -0.3f)
			};
			component.transform.localScale = val;
			component.transform.localPosition = val2;
		}
		SceneUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.m_actors.Add(component);
		DefLoader.Get().LoadCardDef(entityDef.GetCardId(), new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), new CardPortraitQuality(3, true), null);
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000ADF84 File Offset: 0x000AC184
	private void OnActorLoaded(string name, GameObject go, object callbackData)
	{
		EntityDef entityDef = (EntityDef)callbackData;
		Actor component = go.GetComponent<Actor>();
		this.StartSettingUpNonHeroActor(component, entityDef, this.m_cardParent.transform);
		CardRewardData cardRewardData = base.Data as CardRewardData;
		this.m_cardCount.SetCount(cardRewardData.Count);
		if (cardRewardData.Count > 1)
		{
			Actor actor = Object.Instantiate<Actor>(component);
			this.StartSettingUpNonHeroActor(actor, entityDef, this.m_duplicateCardParent.transform);
		}
		DefLoader.Get().LoadCardDef(entityDef.GetCardId(), new DefLoader.LoadDefCallback<CardDef>(this.OnCardDefLoaded), entityDef, new CardPortraitQuality(3, true));
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000AE01C File Offset: 0x000AC21C
	private void StartSettingUpNonHeroActor(Actor actor, EntityDef entityDef, Transform parentTransform)
	{
		actor.SetEntityDef(entityDef);
		actor.transform.parent = parentTransform;
		actor.transform.localScale = CardReward.CARD_SCALE[entityDef.GetCardType()];
		actor.transform.localPosition = Vector3.zero;
		actor.transform.localRotation = Quaternion.identity;
		actor.TurnOffCollider();
		if (base.Data.Origin != NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
		{
			SceneUtils.SetLayer(actor.gameObject, GameLayer.IgnoreFullScreenEffects);
		}
		this.m_actors.Add(actor);
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000AE0A8 File Offset: 0x000AC2A8
	private void FinishSettingUpActor(Actor actor, CardDef cardDef)
	{
		CardRewardData cardRewardData = base.Data as CardRewardData;
		actor.SetCardDef(cardDef);
		actor.SetPremium(cardRewardData.Premium);
		actor.UpdateAllComponents();
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000AE0DC File Offset: 0x000AC2DC
	private void SetupHeroAchieves()
	{
		List<Achievement> achievesInGroup = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO);
		List<Achievement> achievesInGroup2 = AchieveManager.Get().GetAchievesInGroup(Achievement.AchType.UNLOCK_HERO, true);
		int count = achievesInGroup.Count;
		int count2 = achievesInGroup2.Count;
		CardRewardData cardRewardData = base.Data as CardRewardData;
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
		TAG_CLASS @class = entityDef.GetClass();
		string className = GameStrings.GetClassName(@class);
		string headline = GameStrings.Format("GLOBAL_REWARD_HERO_HEADLINE", new object[]
		{
			className
		});
		string details = GameStrings.Format("GLOBAL_REWARD_HERO_DETAILS", new object[]
		{
			count2,
			count
		});
		string source = GameStrings.Format("GLOBAL_REWARD_HERO_SOURCE", new object[]
		{
			className
		});
		base.SetRewardText(headline, details, source);
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000AE1A0 File Offset: 0x000AC3A0
	private void SetUpGoldenHeroAchieves()
	{
		string headline = GameStrings.Get("GLOBAL_REWARD_GOLDEN_HERO_HEADLINE");
		base.SetRewardText(headline, string.Empty, string.Empty);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000AE1CC File Offset: 0x000AC3CC
	private void InitRewardText()
	{
		CardRewardData cardRewardData = base.Data as CardRewardData;
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardRewardData.CardID);
		if (!entityDef.IsHero())
		{
			string headline = GameStrings.Get("GLOBAL_REWARD_CARD_HEADLINE");
			string details = string.Empty;
			string source = string.Empty;
			TAG_CARD_SET cardSet = entityDef.GetCardSet();
			TAG_CLASS @class = entityDef.GetClass();
			string className = GameStrings.GetClassName(@class);
			if (GameMgr.Get().IsTutorial())
			{
				details = GameUtils.GetCurrentTutorialCardRewardDetails();
			}
			else if (cardSet == TAG_CARD_SET.CORE)
			{
				int num = 20;
				int basicCardsIOwn = CollectionManager.Get().GetBasicCardsIOwn(@class);
				if (cardRewardData.Premium == TAG_PREMIUM.GOLDEN)
				{
					details = string.Empty;
				}
				else
				{
					if (num == basicCardsIOwn)
					{
						cardRewardData.InnKeeperLine = CardRewardData.InnKeeperTrigger.CORE_CLASS_SET_COMPLETE;
					}
					else if (basicCardsIOwn == 4)
					{
						cardRewardData.InnKeeperLine = CardRewardData.InnKeeperTrigger.SECOND_REWARD_EVER;
					}
					details = GameStrings.Format("GLOBAL_REWARD_CORE_CARD_DETAILS", new object[]
					{
						basicCardsIOwn,
						num,
						className
					});
				}
			}
			if (base.Data.Origin == NetCache.ProfileNotice.NoticeOrigin.LEVEL_UP)
			{
				TAG_CLASS tag_CLASS = (TAG_CLASS)base.Data.OriginData;
				NetCache.HeroLevel heroLevel = GameUtils.GetHeroLevel(tag_CLASS);
				source = GameStrings.Format("GLOBAL_REWARD_CARD_LEVEL_UP", new object[]
				{
					heroLevel.CurrentLevel.Level.ToString(),
					GameStrings.GetClassName(tag_CLASS)
				});
			}
			else
			{
				source = string.Empty;
			}
			base.SetRewardText(headline, details, source);
		}
	}

	// Token: 0x0400145C RID: 5212
	public GameObject m_nonHeroCardsRoot;

	// Token: 0x0400145D RID: 5213
	public GameObject m_heroCardRoot;

	// Token: 0x0400145E RID: 5214
	public GameObject m_cardParent;

	// Token: 0x0400145F RID: 5215
	public GameObject m_duplicateCardParent;

	// Token: 0x04001460 RID: 5216
	public CardRewardCount m_cardCount;

	// Token: 0x04001461 RID: 5217
	private static readonly Map<TAG_CARDTYPE, Vector3> CARD_SCALE = new Map<TAG_CARDTYPE, Vector3>
	{
		{
			TAG_CARDTYPE.SPELL,
			new Vector3(1f, 1f, 1f)
		},
		{
			TAG_CARDTYPE.MINION,
			new Vector3(1f, 1f, 1f)
		},
		{
			TAG_CARDTYPE.WEAPON,
			new Vector3(1f, 0.5f, 1f)
		}
	};

	// Token: 0x04001462 RID: 5218
	private List<Actor> m_actors = new List<Actor>();

	// Token: 0x04001463 RID: 5219
	private GameObject m_goToRotate;

	// Token: 0x04001464 RID: 5220
	private CardSoundSpell m_emote;
}
