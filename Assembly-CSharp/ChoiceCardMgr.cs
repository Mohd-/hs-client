using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200084F RID: 2127
[CustomEditClass]
public class ChoiceCardMgr : MonoBehaviour
{
	// Token: 0x06005199 RID: 20889 RVA: 0x001861DC File Offset: 0x001843DC
	private void Awake()
	{
		ChoiceCardMgr.s_instance = this;
	}

	// Token: 0x0600519A RID: 20890 RVA: 0x001861E4 File Offset: 0x001843E4
	private void OnDestroy()
	{
		ChoiceCardMgr.s_instance = null;
	}

	// Token: 0x0600519B RID: 20891 RVA: 0x001861EC File Offset: 0x001843EC
	private void Start()
	{
		GameState.Get().RegisterEntityChoicesReceivedListener(new GameState.EntityChoicesReceivedCallback(this.OnEntityChoicesReceived));
		GameState.Get().RegisterEntitiesChosenReceivedListener(new GameState.EntitiesChosenReceivedCallback(this.OnEntitiesChosenReceived));
		GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
	}

	// Token: 0x0600519C RID: 20892 RVA: 0x0018623F File Offset: 0x0018443F
	public static ChoiceCardMgr Get()
	{
		return ChoiceCardMgr.s_instance;
	}

	// Token: 0x0600519D RID: 20893 RVA: 0x00186248 File Offset: 0x00184448
	public List<Card> GetFriendlyCards()
	{
		if (this.m_subOptionState != null)
		{
			return this.m_subOptionState.m_cards;
		}
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		ChoiceCardMgr.ChoiceState choiceState;
		if (this.m_choiceStateMap.TryGetValue(friendlyPlayerId, out choiceState))
		{
			return choiceState.m_cards;
		}
		return null;
	}

	// Token: 0x0600519E RID: 20894 RVA: 0x00186292 File Offset: 0x00184492
	public bool IsShown()
	{
		return this.m_subOptionState != null || this.m_choiceStateMap.Count > 0;
	}

	// Token: 0x0600519F RID: 20895 RVA: 0x001862B8 File Offset: 0x001844B8
	public bool IsFriendlyShown()
	{
		if (this.m_subOptionState != null)
		{
			return true;
		}
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		return this.m_choiceStateMap.ContainsKey(friendlyPlayerId);
	}

	// Token: 0x060051A0 RID: 20896 RVA: 0x001862F1 File Offset: 0x001844F1
	public bool HasSubOption()
	{
		return this.m_subOptionState != null;
	}

	// Token: 0x060051A1 RID: 20897 RVA: 0x001862FF File Offset: 0x001844FF
	public Card GetSubOptionParentCard()
	{
		return (this.m_subOptionState != null) ? this.m_subOptionState.m_parentCard : null;
	}

	// Token: 0x060051A2 RID: 20898 RVA: 0x0018631D File Offset: 0x0018451D
	public void ClearSubOptions()
	{
		this.m_subOptionState = null;
	}

	// Token: 0x060051A3 RID: 20899 RVA: 0x00186328 File Offset: 0x00184528
	public void ShowSubOptions(Card parentCard)
	{
		this.m_subOptionState = new ChoiceCardMgr.SubOptionState();
		this.m_subOptionState.m_parentCard = parentCard;
		base.StartCoroutine(this.WaitThenShowSubOptions());
	}

	// Token: 0x060051A4 RID: 20900 RVA: 0x0018635C File Offset: 0x0018455C
	public bool IsWaitingToShowSubOptions()
	{
		if (!this.HasSubOption())
		{
			return false;
		}
		Entity entity = this.m_subOptionState.m_parentCard.GetEntity();
		if (entity.IsMinion())
		{
			Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
			ZonePlay battlefieldZone = friendlySidePlayer.GetBattlefieldZone();
			if (this.m_subOptionState.m_parentCard.GetZone() != battlefieldZone)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060051A5 RID: 20901 RVA: 0x001863C4 File Offset: 0x001845C4
	public void CancelSubOptions()
	{
		if (!this.HasSubOption())
		{
			return;
		}
		Entity entity = this.m_subOptionState.m_parentCard.GetEntity();
		Card card = entity.GetCard();
		for (int i = 0; i < this.m_subOptionState.m_cards.Count; i++)
		{
			Spell subOptionSpell = card.GetSubOptionSpell(i, false);
			if (subOptionSpell)
			{
				SpellStateType activeState = subOptionSpell.GetActiveState();
				if (activeState != SpellStateType.NONE && activeState != SpellStateType.CANCEL)
				{
					subOptionSpell.ActivateState(SpellStateType.CANCEL);
				}
			}
		}
		card.ActivateHandStateSpells();
		if (entity.IsHeroPower())
		{
			entity.SetTagAndHandleChange<int>(GAME_TAG.EXHAUSTED, 0);
		}
		this.HideSubOptions(null);
	}

	// Token: 0x060051A6 RID: 20902 RVA: 0x00186469 File Offset: 0x00184669
	public void OnSubOptionClicked(Entity chosenEntity)
	{
		if (!this.HasSubOption())
		{
			return;
		}
		this.HideSubOptions(chosenEntity);
	}

	// Token: 0x060051A7 RID: 20903 RVA: 0x0018647E File Offset: 0x0018467E
	public bool HasChoices()
	{
		return this.m_choiceStateMap.Count > 0;
	}

	// Token: 0x060051A8 RID: 20904 RVA: 0x0018648E File Offset: 0x0018468E
	public bool HasChoices(int playerId)
	{
		return this.m_choiceStateMap.ContainsKey(playerId);
	}

	// Token: 0x060051A9 RID: 20905 RVA: 0x0018649C File Offset: 0x0018469C
	public bool HasFriendlyChoices()
	{
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		return this.HasChoices(friendlyPlayerId);
	}

	// Token: 0x060051AA RID: 20906 RVA: 0x001864BC File Offset: 0x001846BC
	public PowerTaskList GetPreChoiceTaskList(int playerId)
	{
		ChoiceCardMgr.ChoiceState choiceState;
		if (this.m_choiceStateMap.TryGetValue(playerId, out choiceState))
		{
			return choiceState.m_preTaskList;
		}
		return null;
	}

	// Token: 0x060051AB RID: 20907 RVA: 0x001864E4 File Offset: 0x001846E4
	public PowerTaskList GetFriendlyPreChoiceTaskList()
	{
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		return this.GetPreChoiceTaskList(friendlyPlayerId);
	}

	// Token: 0x060051AC RID: 20908 RVA: 0x00186504 File Offset: 0x00184704
	public bool IsWaitingToShowChoices(int playerId)
	{
		ChoiceCardMgr.ChoiceState choiceState;
		return this.m_choiceStateMap.TryGetValue(playerId, out choiceState) && choiceState.m_waitingToShow;
	}

	// Token: 0x060051AD RID: 20909 RVA: 0x0018652C File Offset: 0x0018472C
	public bool IsFriendlyWaitingToShowChoices()
	{
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		return this.IsWaitingToShowChoices(friendlyPlayerId);
	}

	// Token: 0x060051AE RID: 20910 RVA: 0x0018654B File Offset: 0x0018474B
	public NormalButton GetChoiceButton()
	{
		return this.m_choiceButton;
	}

	// Token: 0x060051AF RID: 20911 RVA: 0x00186554 File Offset: 0x00184754
	public void OnSendChoices(Network.EntityChoices choicePacket, List<Entity> chosenEntities)
	{
		if (choicePacket.ChoiceType != CHOICE_TYPE.GENERAL)
		{
			return;
		}
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		ChoiceCardMgr.ChoiceState state;
		if (!this.m_choiceStateMap.TryGetValue(friendlyPlayerId, out state))
		{
			Error.AddDevFatal("ChoiceCardMgr.OnSendChoices() - there is no ChoiceState for friendly player {0}", new object[]
			{
				friendlyPlayerId
			});
		}
		this.HideChoicesFromInput(friendlyPlayerId, state, chosenEntities);
	}

	// Token: 0x060051B0 RID: 20912 RVA: 0x001865AE File Offset: 0x001847AE
	private void OnEntityChoicesReceived(Network.EntityChoices choices, PowerTaskList preChoiceTaskList, object userData)
	{
		if (choices.ChoiceType != CHOICE_TYPE.GENERAL)
		{
			return;
		}
		base.StartCoroutine(this.WaitThenShowChoices(choices, preChoiceTaskList));
	}

	// Token: 0x060051B1 RID: 20913 RVA: 0x001865CC File Offset: 0x001847CC
	private bool OnEntitiesChosenReceived(Network.EntitiesChosen chosen, Network.EntityChoices choices, object userData)
	{
		if (choices.ChoiceType != CHOICE_TYPE.GENERAL)
		{
			return false;
		}
		base.StartCoroutine(this.WaitThenHideChoices(chosen, choices));
		return true;
	}

	// Token: 0x060051B2 RID: 20914 RVA: 0x001865EC File Offset: 0x001847EC
	private void OnGameOver(object userData)
	{
		base.StopAllCoroutines();
		this.CancelSubOptions();
		this.CancelChoices();
	}

	// Token: 0x060051B3 RID: 20915 RVA: 0x00186600 File Offset: 0x00184800
	private IEnumerator WaitThenShowChoices(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
	{
		int playerId = choices.PlayerId;
		ChoiceCardMgr.ChoiceState state = new ChoiceCardMgr.ChoiceState();
		this.m_choiceStateMap.Add(playerId, state);
		state.m_waitingToShow = true;
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		if (powerProcessor.HasTaskList(state.m_preTaskList))
		{
			Log.Power.Print("ChoiceCardMgr.WaitThenShowChoices() - id={0} WAIT for taskList {1}", new object[]
			{
				choices.ID,
				preChoiceTaskList.GetId()
			});
		}
		state.m_preTaskList = preChoiceTaskList;
		while (powerProcessor.HasTaskList(state.m_preTaskList))
		{
			yield return null;
		}
		Log.Power.Print("ChoiceCardMgr.WaitThenShowChoices() - id={0} BEGIN", new object[]
		{
			choices.ID
		});
		for (int i = 0; i < choices.Entities.Count; i++)
		{
			int entityId = choices.Entities[i];
			Entity entity = GameState.Get().GetEntity(entityId);
			Card card = entity.GetCard();
			if (card == null)
			{
				Error.AddDevFatal("ChoiceCardMgr.WaitThenShowChoices() - Entity {0} (option {1}) has no Card", new object[]
				{
					entity,
					i
				});
			}
			else
			{
				state.m_cards.Add(card);
				base.StartCoroutine(this.LoadChoiceCardActors(entity, card));
			}
		}
		for (int j = 0; j < state.m_cards.Count; j++)
		{
			Card card2 = state.m_cards[j];
			while (!this.IsChoiceCardReady(card2))
			{
				yield return null;
			}
		}
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		bool friendly = playerId == friendlyPlayerId;
		if (friendly)
		{
			while (GameState.Get().IsTurnStartManagerBlockingInput())
			{
				yield return null;
			}
		}
		state.m_waitingToShow = false;
		this.ShowChoices(state, friendly);
		yield break;
	}

	// Token: 0x060051B4 RID: 20916 RVA: 0x00186638 File Offset: 0x00184838
	private IEnumerator LoadChoiceCardActors(Entity entity, Card card)
	{
		while (!this.IsEntityReady(entity))
		{
			yield return null;
		}
		card.HideCard();
		while (!this.IsCardReady(card))
		{
			yield return null;
		}
		card.ForceLoadHandActor();
		yield break;
	}

	// Token: 0x060051B5 RID: 20917 RVA: 0x00186670 File Offset: 0x00184870
	private bool IsChoiceCardReady(Card card)
	{
		Entity entity = card.GetEntity();
		return this.IsEntityReady(entity) && this.IsCardReady(card) && this.IsCardActorReady(card);
	}

	// Token: 0x060051B6 RID: 20918 RVA: 0x001866B0 File Offset: 0x001848B0
	private void ShowChoices(ChoiceCardMgr.ChoiceState state, bool friendly)
	{
		List<Card> cards = state.m_cards;
		if (friendly)
		{
			this.ShowChoiceUi(cards);
		}
		int count = cards.Count;
		string text = (!friendly) ? this.m_ChoiceData.m_OpponentBoneName : this.m_ChoiceData.m_FriendlyBoneName;
		if (UniversalInputManager.UsePhoneUI)
		{
			text += "_phone";
		}
		Transform transform = Board.Get().FindBone(text);
		Vector3 position = transform.position;
		float num = this.m_ChoiceData.m_HorizontalPadding;
		if (UniversalInputManager.UsePhoneUI && count > this.m_CommonData.m_PhoneMaxCardsBeforeAdjusting)
		{
			num = this.m_ChoiceData.m_PhoneMaxHorizontalPadding;
		}
		float num2 = (!friendly) ? this.m_CommonData.m_OpponentCardWidth : this.m_CommonData.m_FriendlyCardWidth;
		float num3 = 0.5f * num2;
		float num4 = num2 * (float)count + num * (float)(count - 1);
		float num5 = 0.5f * num4;
		float num6 = position.x - num5 + num3;
		for (int i = 0; i < count; i++)
		{
			Card card = cards[i];
			Vector3 position2 = default(Vector3);
			position2.x = num6;
			position2.y = position.y;
			position2.z = position.z;
			card.transform.position = position2;
			num6 += num2 + num;
		}
		this.ShowChoiceCards(state, friendly, true);
	}

	// Token: 0x060051B7 RID: 20919 RVA: 0x00186828 File Offset: 0x00184A28
	private void ShowChoiceCards(ChoiceCardMgr.ChoiceState state, bool friendly, bool playEffects)
	{
		string text = (!friendly) ? this.m_ChoiceData.m_OpponentBoneName : this.m_ChoiceData.m_FriendlyBoneName;
		if (UniversalInputManager.UsePhoneUI)
		{
			text += "_phone";
		}
		int count = state.m_cards.Count;
		Transform transform = Board.Get().FindBone(text);
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		Vector3 localScale = transform.localScale;
		if (UniversalInputManager.UsePhoneUI && count > this.m_CommonData.m_PhoneMaxCardsBeforeAdjusting)
		{
			localScale.x *= this.m_CommonData.m_PhoneMaxCardScale;
			localScale.z *= this.m_CommonData.m_PhoneMaxCardScale;
		}
		for (int i = 0; i < count; i++)
		{
			Card card = state.m_cards[i];
			card.ShowCard();
			card.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
			iTween.Stop(card.gameObject);
			iTween.RotateTo(card.gameObject, eulerAngles, this.m_ChoiceData.m_CardShowTime);
			iTween.ScaleTo(card.gameObject, localScale, this.m_ChoiceData.m_CardShowTime);
		}
		if (playEffects)
		{
			this.PlayChoiceEffects(state, friendly);
		}
	}

	// Token: 0x060051B8 RID: 20920 RVA: 0x0018697C File Offset: 0x00184B7C
	private void PlayChoiceEffects(ChoiceCardMgr.ChoiceState state, bool friendly)
	{
		if (!friendly)
		{
			return;
		}
		GameState gameState = GameState.Get();
		Entity entity = gameState.GetEntity(gameState.GetFriendlyEntityChoices().Source);
		if (entity == null)
		{
			return;
		}
		foreach (Card card in state.m_cards)
		{
			Spell choiceEffectForCard = this.GetChoiceEffectForCard(entity, card);
			if (choiceEffectForCard != null)
			{
				Spell spell2 = Object.Instantiate<Spell>(choiceEffectForCard);
				TransformUtil.AttachAndPreserveLocalTransform(spell2.transform, card.GetActor().transform);
				spell2.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
				{
					if (spell.GetActiveState() == SpellStateType.NONE)
					{
						Object.Destroy(spell.gameObject);
					}
				});
				spell2.Activate();
			}
		}
	}

	// Token: 0x060051B9 RID: 20921 RVA: 0x00186A58 File Offset: 0x00184C58
	private Spell GetChoiceEffectForCard(Entity sourceEntity, Card card)
	{
		if (sourceEntity.HasReferencedTag(GAME_TAG.TREASURE))
		{
			return this.m_ChoiceEffectData.m_DiscoverCardEffect;
		}
		return null;
	}

	// Token: 0x060051BA RID: 20922 RVA: 0x00186A78 File Offset: 0x00184C78
	private IEnumerator WaitThenHideChoices(Network.EntitiesChosen chosen, Network.EntityChoices choices)
	{
		int playerId = chosen.PlayerId;
		ChoiceCardMgr.ChoiceState state = this.m_choiceStateMap[playerId];
		if (state.m_waitingToShow)
		{
			Log.Power.Print("ChoiceCardMgr.WaitThenHideChoices() - id={0} WAIT for EntityChoice", new object[]
			{
				chosen.ID
			});
			while (state.m_waitingToShow)
			{
				yield return null;
			}
			yield return new WaitForSeconds(this.m_ChoiceData.m_MinShowTime);
		}
		Log.Power.Print("ChoiceCardMgr.WaitThenHideChoices() - id={0} BEGIN", new object[]
		{
			chosen.ID
		});
		this.HideChoicesFromPacket(playerId, state, chosen);
		yield break;
	}

	// Token: 0x060051BB RID: 20923 RVA: 0x00186AA4 File Offset: 0x00184CA4
	private void HideChoicesFromPacket(int playerId, ChoiceCardMgr.ChoiceState state, Network.EntitiesChosen chosen)
	{
		for (int i = 0; i < state.m_cards.Count; i++)
		{
			Card card = state.m_cards[i];
			if (!this.WasCardChosen(card, chosen.Entities))
			{
				card.HideCard();
			}
		}
		this.DoCommonHideChoicesWork(playerId);
		GameState.Get().OnEntitiesChosenProcessed(chosen);
	}

	// Token: 0x060051BC RID: 20924 RVA: 0x00186B0C File Offset: 0x00184D0C
	private bool WasCardChosen(Card card, List<int> chosenEntityIds)
	{
		Entity entity = card.GetEntity();
		int entityId = entity.GetEntityId();
		int num = chosenEntityIds.FindIndex((int currEntityId) => entityId == currEntityId);
		return num >= 0;
	}

	// Token: 0x060051BD RID: 20925 RVA: 0x00186B4C File Offset: 0x00184D4C
	private void HideChoicesFromInput(int playerId, ChoiceCardMgr.ChoiceState state, List<Entity> chosenEntities)
	{
		for (int i = 0; i < state.m_cards.Count; i++)
		{
			Card card = state.m_cards[i];
			Entity entity = card.GetEntity();
			if (!chosenEntities.Contains(entity))
			{
				card.HideCard();
			}
		}
		this.DoCommonHideChoicesWork(playerId);
	}

	// Token: 0x060051BE RID: 20926 RVA: 0x00186BA8 File Offset: 0x00184DA8
	private void DoCommonHideChoicesWork(int playerId)
	{
		bool flag = playerId == GameState.Get().GetFriendlyPlayerId();
		if (flag)
		{
			this.HideChoiceUi();
		}
		this.m_choiceStateMap.Remove(playerId);
	}

	// Token: 0x060051BF RID: 20927 RVA: 0x00186BDC File Offset: 0x00184DDC
	private void HideChoiceCards(ChoiceCardMgr.ChoiceState state)
	{
		for (int i = 0; i < state.m_cards.Count; i++)
		{
			Card card = state.m_cards[i];
			this.HideChoiceCard(card);
		}
	}

	// Token: 0x060051C0 RID: 20928 RVA: 0x00186C1C File Offset: 0x00184E1C
	private void HideChoiceCard(Card card)
	{
		Action<object> action = delegate(object userData)
		{
			Card card2 = (Card)userData;
			card2.HideCard();
		};
		iTween.Stop(card.gameObject);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			ChoiceCardMgr.INVISIBLE_SCALE,
			"time",
			this.m_ChoiceData.m_CardHideTime,
			"oncomplete",
			action,
			"oncompleteparams",
			card,
			"oncompletetarget",
			base.gameObject
		});
		iTween.ScaleTo(card.gameObject, args);
	}

	// Token: 0x060051C1 RID: 20929 RVA: 0x00186CC5 File Offset: 0x00184EC5
	private void ShowChoiceUi(List<Card> cards)
	{
		this.ShowChoiceBanner(cards);
		this.ShowChoiceButton();
	}

	// Token: 0x060051C2 RID: 20930 RVA: 0x00186CD4 File Offset: 0x00184ED4
	private void HideChoiceUi()
	{
		this.HideChoiceBanner();
		this.HideChoiceButton();
	}

	// Token: 0x060051C3 RID: 20931 RVA: 0x00186CE4 File Offset: 0x00184EE4
	private void ShowChoiceBanner(List<Card> cards)
	{
		this.HideChoiceBanner();
		Network.EntityChoices friendlyEntityChoices = GameState.Get().GetFriendlyEntityChoices();
		Transform transform = Board.Get().FindBone(this.m_ChoiceData.m_BannerBoneName);
		this.m_choiceBanner = (Banner)Object.Instantiate(this.m_ChoiceData.m_BannerPrefab, transform.position, transform.rotation);
		string text = GameState.Get().GetGameEntity().CustomChoiceBannerText();
		if (text == null)
		{
			if (friendlyEntityChoices.CountMax == 1)
			{
				text = GameStrings.Get("GAMEPLAY_CHOOSE_ONE");
				foreach (Card card in cards)
				{
					if (null != card && card.GetEntity().IsHeroPower())
					{
						text = GameStrings.Get("GAMEPLAY_CHOOSE_ONE_HERO_POWER");
						break;
					}
				}
			}
			else
			{
				text = string.Format("[PH] Choose {0} to {1}", friendlyEntityChoices.CountMin, friendlyEntityChoices.CountMax);
			}
		}
		this.m_choiceBanner.SetText(text);
		Vector3 localScale = this.m_choiceBanner.transform.localScale;
		this.m_choiceBanner.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"time",
			this.m_ChoiceData.m_UiShowTime
		});
		iTween.ScaleTo(this.m_choiceBanner.gameObject, args);
	}

	// Token: 0x060051C4 RID: 20932 RVA: 0x00186E80 File Offset: 0x00185080
	private void HideChoiceBanner()
	{
		if (!this.m_choiceBanner)
		{
			return;
		}
		Object.Destroy(this.m_choiceBanner.gameObject);
	}

	// Token: 0x060051C5 RID: 20933 RVA: 0x00186EA4 File Offset: 0x001850A4
	private void ShowChoiceButton()
	{
		this.HideChoiceButton();
		string name = FileUtils.GameAssetPathToName(this.m_ChoiceData.m_ButtonPrefab);
		GameObject gameObject = AssetLoader.Get().LoadActor(name, false, false);
		this.m_choiceButton = gameObject.GetComponent<NormalButton>();
		UberText buttonUberText = this.m_choiceButton.GetButtonUberText();
		buttonUberText.TextAlpha = 1f;
		string text = this.m_ChoiceData.m_ButtonBoneName;
		if (UniversalInputManager.UsePhoneUI)
		{
			text += "_phone";
		}
		Transform source = Board.Get().FindBone(text);
		TransformUtil.CopyWorld(this.m_choiceButton, source);
		this.m_friendlyChoicesShown = true;
		this.m_choiceButton.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ChoiceButton_OnPress));
		this.m_choiceButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ChoiceButton_OnRelease));
		this.m_choiceButton.SetText(GameStrings.Get("GLOBAL_HIDE"));
		Spell component = this.m_choiceButton.m_button.GetComponent<Spell>();
		component.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x060051C6 RID: 20934 RVA: 0x00186FA2 File Offset: 0x001851A2
	private void HideChoiceButton()
	{
		if (!this.m_choiceButton)
		{
			return;
		}
		Object.Destroy(this.m_choiceButton.gameObject);
	}

	// Token: 0x060051C7 RID: 20935 RVA: 0x00186FC5 File Offset: 0x001851C5
	private void ChoiceButton_OnPress(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("UI_MouseClick_01");
	}

	// Token: 0x060051C8 RID: 20936 RVA: 0x00186FD8 File Offset: 0x001851D8
	private void ChoiceButton_OnRelease(UIEvent e)
	{
		int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
		ChoiceCardMgr.ChoiceState state = this.m_choiceStateMap[friendlyPlayerId];
		if (this.m_friendlyChoicesShown)
		{
			this.m_choiceButton.SetText(GameStrings.Get("GLOBAL_SHOW"));
			this.HideChoiceCards(state);
			this.m_friendlyChoicesShown = false;
		}
		else
		{
			this.m_choiceButton.SetText(GameStrings.Get("GLOBAL_HIDE"));
			this.ShowChoiceCards(state, true, this.m_ChoiceEffectData.m_AlwaysPlayChoiceEffects);
			this.m_friendlyChoicesShown = true;
		}
	}

	// Token: 0x060051C9 RID: 20937 RVA: 0x00187060 File Offset: 0x00185260
	private void CancelChoices()
	{
		this.HideChoiceUi();
		foreach (ChoiceCardMgr.ChoiceState choiceState in this.m_choiceStateMap.Values)
		{
			for (int i = 0; i < choiceState.m_cards.Count; i++)
			{
				Card card = choiceState.m_cards[i];
				card.HideCard();
			}
		}
		this.m_choiceStateMap.Clear();
	}

	// Token: 0x060051CA RID: 20938 RVA: 0x001870F8 File Offset: 0x001852F8
	private IEnumerator WaitThenShowSubOptions()
	{
		int currentPacketId = GameState.Get().GetOptionsPacket().ID;
		while (this.IsWaitingToShowSubOptions())
		{
			yield return null;
			if (this.m_subOptionState == null)
			{
				yield break;
			}
			if (GameMgr.Get().IsSpectator())
			{
				Network.Options options = GameState.Get().GetOptionsPacket();
				if (options == null || options.ID != currentPacketId)
				{
					InputManager.Get().DropSubOptionParentCard();
					yield break;
				}
			}
		}
		this.ShowSubOptions();
		yield break;
	}

	// Token: 0x060051CB RID: 20939 RVA: 0x00187114 File Offset: 0x00185314
	private void ShowSubOptions()
	{
		GameState gameState = GameState.Get();
		Card parentCard = this.m_subOptionState.m_parentCard;
		Entity entity = this.m_subOptionState.m_parentCard.GetEntity();
		string text = this.m_SubOptionData.m_BoneName;
		if (UniversalInputManager.UsePhoneUI)
		{
			text += "_phone";
		}
		Transform transform = Board.Get().FindBone(text);
		float num = this.m_CommonData.m_FriendlyCardWidth;
		float num2 = transform.position.x;
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		ZonePlay battlefieldZone = friendlySidePlayer.GetBattlefieldZone();
		List<int> subCardIDs = entity.GetSubCardIDs();
		if (entity.IsMinion() && !UniversalInputManager.UsePhoneUI)
		{
			int zonePosition = parentCard.GetZonePosition();
			num2 = battlefieldZone.GetCardPosition(parentCard).x;
			if (zonePosition > 5)
			{
				num += this.m_SubOptionData.m_AdjacentCardXOffset;
				num2 -= this.m_CommonData.m_FriendlyCardWidth * 1.5f + this.m_SubOptionData.m_AdjacentCardXOffset + this.m_SubOptionData.m_MinionParentXOffset;
			}
			else if (zonePosition == 1 && battlefieldZone.GetCards().Count > 6)
			{
				num += this.m_SubOptionData.m_AdjacentCardXOffset;
				num2 += this.m_CommonData.m_FriendlyCardWidth / 2f + this.m_SubOptionData.m_MinionParentXOffset;
			}
			else
			{
				num += this.m_SubOptionData.m_MinionParentXOffset * 2f;
				num2 -= this.m_CommonData.m_FriendlyCardWidth / 2f + this.m_SubOptionData.m_MinionParentXOffset;
			}
		}
		else
		{
			int count = subCardIDs.Count;
			num += ((count <= this.m_CommonData.m_PhoneMaxCardsBeforeAdjusting) ? this.m_SubOptionData.m_AdjacentCardXOffset : this.m_SubOptionData.m_PhoneMaxAdjacentCardXOffset);
			num2 -= num / 2f * (float)(count - 1);
		}
		for (int i = 0; i < subCardIDs.Count; i++)
		{
			int id = subCardIDs[i];
			Entity entity2 = gameState.GetEntity(id);
			Card card = entity2.GetCard();
			if (!(card == null))
			{
				this.m_subOptionState.m_cards.Add(card);
				card.ForceLoadHandActor();
				card.transform.position = parentCard.transform.position;
				card.transform.localScale = ChoiceCardMgr.INVISIBLE_SCALE;
				Vector3 position = default(Vector3);
				position.x = num2 + (float)i * num;
				position.y = transform.position.y;
				position.z = transform.position.z;
				iTween.MoveTo(card.gameObject, position, this.m_SubOptionData.m_CardShowTime);
				Vector3 localScale = transform.localScale;
				if (UniversalInputManager.UsePhoneUI && subCardIDs.Count > this.m_CommonData.m_PhoneMaxCardsBeforeAdjusting)
				{
					localScale.x *= this.m_CommonData.m_PhoneMaxCardScale;
					localScale.z *= this.m_CommonData.m_PhoneMaxCardScale;
				}
				iTween.ScaleTo(card.gameObject, localScale, this.m_SubOptionData.m_CardShowTime);
				card.ActivateHandStateSpells();
			}
		}
	}

	// Token: 0x060051CC RID: 20940 RVA: 0x00187480 File Offset: 0x00185680
	private void HideSubOptions(Entity chosenEntity = null)
	{
		for (int i = 0; i < this.m_subOptionState.m_cards.Count; i++)
		{
			Card card = this.m_subOptionState.m_cards[i];
			card.DeactivateHandStateSpells();
			Entity entity = card.GetEntity();
			if (entity != chosenEntity)
			{
				card.HideCard();
			}
		}
	}

	// Token: 0x060051CD RID: 20941 RVA: 0x001874DF File Offset: 0x001856DF
	private bool IsEntityReady(Entity entity)
	{
		return entity.GetZone() == TAG_ZONE.SETASIDE && !entity.IsBusy();
	}

	// Token: 0x060051CE RID: 20942 RVA: 0x001874FD File Offset: 0x001856FD
	private bool IsCardReady(Card card)
	{
		return !(card.GetCardDef() == null);
	}

	// Token: 0x060051CF RID: 20943 RVA: 0x00187513 File Offset: 0x00185713
	private bool IsCardActorReady(Card card)
	{
		return card.IsActorReady();
	}

	// Token: 0x0400383B RID: 14395
	public ChoiceCardMgr.CommonData m_CommonData = new ChoiceCardMgr.CommonData();

	// Token: 0x0400383C RID: 14396
	public ChoiceCardMgr.ChoiceData m_ChoiceData = new ChoiceCardMgr.ChoiceData();

	// Token: 0x0400383D RID: 14397
	public ChoiceCardMgr.SubOptionData m_SubOptionData = new ChoiceCardMgr.SubOptionData();

	// Token: 0x0400383E RID: 14398
	public ChoiceCardMgr.ChoiceEffectData m_ChoiceEffectData = new ChoiceCardMgr.ChoiceEffectData();

	// Token: 0x0400383F RID: 14399
	private static readonly Vector3 INVISIBLE_SCALE = new Vector3(0.0001f, 0.0001f, 0.0001f);

	// Token: 0x04003840 RID: 14400
	private static ChoiceCardMgr s_instance;

	// Token: 0x04003841 RID: 14401
	private ChoiceCardMgr.SubOptionState m_subOptionState;

	// Token: 0x04003842 RID: 14402
	private Map<int, ChoiceCardMgr.ChoiceState> m_choiceStateMap = new Map<int, ChoiceCardMgr.ChoiceState>();

	// Token: 0x04003843 RID: 14403
	private Banner m_choiceBanner;

	// Token: 0x04003844 RID: 14404
	private NormalButton m_choiceButton;

	// Token: 0x04003845 RID: 14405
	private bool m_friendlyChoicesShown;

	// Token: 0x020008B3 RID: 2227
	[Serializable]
	public class CommonData
	{
		// Token: 0x04003A73 RID: 14963
		public float m_FriendlyCardWidth = 2.85f;

		// Token: 0x04003A74 RID: 14964
		public float m_OpponentCardWidth = 1.5f;

		// Token: 0x04003A75 RID: 14965
		public int m_PhoneMaxCardsBeforeAdjusting = 3;

		// Token: 0x04003A76 RID: 14966
		public float m_PhoneMaxCardScale = 0.85f;
	}

	// Token: 0x020008B4 RID: 2228
	[Serializable]
	public class ChoiceData
	{
		// Token: 0x04003A77 RID: 14967
		public string m_FriendlyBoneName = "FriendlyChoice";

		// Token: 0x04003A78 RID: 14968
		public string m_OpponentBoneName = "OpponentChoice";

		// Token: 0x04003A79 RID: 14969
		public string m_BannerBoneName = "ChoiceBanner";

		// Token: 0x04003A7A RID: 14970
		public string m_ButtonBoneName = "ChoiceButton";

		// Token: 0x04003A7B RID: 14971
		public float m_MinShowTime = 1f;

		// Token: 0x04003A7C RID: 14972
		public Banner m_BannerPrefab;

		// Token: 0x04003A7D RID: 14973
		[CustomEditField(T = EditType.GAME_OBJECT)]
		public string m_ButtonPrefab;

		// Token: 0x04003A7E RID: 14974
		public float m_CardShowTime = 0.2f;

		// Token: 0x04003A7F RID: 14975
		public float m_CardHideTime = 0.2f;

		// Token: 0x04003A80 RID: 14976
		public float m_UiShowTime = 0.5f;

		// Token: 0x04003A81 RID: 14977
		public float m_HorizontalPadding = 0.75f;

		// Token: 0x04003A82 RID: 14978
		public float m_PhoneMaxHorizontalPadding = 0.1f;
	}

	// Token: 0x020008B5 RID: 2229
	[Serializable]
	public class SubOptionData
	{
		// Token: 0x04003A83 RID: 14979
		public string m_BoneName = "SubOption";

		// Token: 0x04003A84 RID: 14980
		public float m_AdjacentCardXOffset = 0.75f;

		// Token: 0x04003A85 RID: 14981
		public float m_PhoneMaxAdjacentCardXOffset = 0.1f;

		// Token: 0x04003A86 RID: 14982
		public float m_MinionParentXOffset = 0.9f;

		// Token: 0x04003A87 RID: 14983
		public float m_CardShowTime = 0.2f;
	}

	// Token: 0x020008B6 RID: 2230
	[Serializable]
	public class ChoiceEffectData
	{
		// Token: 0x04003A88 RID: 14984
		public bool m_AlwaysPlayChoiceEffects;

		// Token: 0x04003A89 RID: 14985
		public Spell m_DiscoverCardEffect;
	}

	// Token: 0x020008B7 RID: 2231
	private class SubOptionState
	{
		// Token: 0x04003A8A RID: 14986
		public List<Card> m_cards = new List<Card>();

		// Token: 0x04003A8B RID: 14987
		public Card m_parentCard;
	}

	// Token: 0x020008B8 RID: 2232
	private class ChoiceState
	{
		// Token: 0x04003A8C RID: 14988
		public List<Card> m_cards = new List<Card>();

		// Token: 0x04003A8D RID: 14989
		public bool m_waitingToShow;

		// Token: 0x04003A8E RID: 14990
		public PowerTaskList m_preTaskList;
	}
}
