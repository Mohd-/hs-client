using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000660 RID: 1632
public class BoardEvents : MonoBehaviour
{
	// Token: 0x060045E7 RID: 17895 RVA: 0x001505AB File Offset: 0x0014E7AB
	private void Awake()
	{
		BoardEvents.s_instance = this;
	}

	// Token: 0x060045E8 RID: 17896 RVA: 0x001505B3 File Offset: 0x0014E7B3
	private void Start()
	{
	}

	// Token: 0x060045E9 RID: 17897 RVA: 0x001505B8 File Offset: 0x0014E7B8
	private void Update()
	{
		if (Time.timeSinceLevelLoad > this.m_nextFastProcessTime)
		{
			this.m_nextFastProcessTime = Time.timeSinceLevelLoad + 0.15f;
			this.ProcessImmediateEvents();
			return;
		}
		if (Time.timeSinceLevelLoad > this.m_nextProcessTime)
		{
			this.m_nextProcessTime = Time.timeSinceLevelLoad + 1.25f;
			this.ProcessEvents();
		}
	}

	// Token: 0x060045EA RID: 17898 RVA: 0x00150614 File Offset: 0x0014E814
	private void OnDestroy()
	{
		BoardEvents.s_instance = null;
	}

	// Token: 0x060045EB RID: 17899 RVA: 0x0015061C File Offset: 0x0014E81C
	public static BoardEvents Get()
	{
		if (BoardEvents.s_instance == null)
		{
			Board board = Board.Get();
			if (board == null)
			{
				return null;
			}
			BoardEvents.s_instance = board.gameObject.AddComponent<BoardEvents>();
		}
		return BoardEvents.s_instance;
	}

	// Token: 0x060045EC RID: 17900 RVA: 0x00150664 File Offset: 0x0014E864
	public void RegisterLargeShakeEvent(BoardEvents.LargeShakeEventDelegate callback)
	{
		if (this.m_largeShakeEventCallbacks == null)
		{
			this.m_largeShakeEventCallbacks = new List<BoardEvents.LargeShakeEventDelegate>();
		}
		this.m_largeShakeEventCallbacks.Add(callback);
	}

	// Token: 0x060045ED RID: 17901 RVA: 0x00150693 File Offset: 0x0014E893
	public void RegisterFriendlyHeroDamageEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_friendlyHeroDamageCallacks = this.RegisterEvent(this.m_friendlyHeroDamageCallacks, callback, minimumWeight);
	}

	// Token: 0x060045EE RID: 17902 RVA: 0x001506A9 File Offset: 0x0014E8A9
	public void RegisterOpponentHeroDamageEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentHeroDamageCallacks = this.RegisterEvent(this.m_opponentHeroDamageCallacks, callback, minimumWeight);
	}

	// Token: 0x060045EF RID: 17903 RVA: 0x001506BF File Offset: 0x0014E8BF
	public void RegisterFriendlyMinionDamageEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_friendlyMinionDamageCallacks = this.RegisterEvent(this.m_friendlyMinionDamageCallacks, callback, minimumWeight);
	}

	// Token: 0x060045F0 RID: 17904 RVA: 0x001506D5 File Offset: 0x0014E8D5
	public void RegisterOpponentMinionDamageEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentMinionDamageCallacks = this.RegisterEvent(this.m_opponentMinionDamageCallacks, callback, minimumWeight);
	}

	// Token: 0x060045F1 RID: 17905 RVA: 0x001506EB File Offset: 0x0014E8EB
	public void RegisterFriendlyHeroHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_friendlyHeroHealCallbacks = this.RegisterEvent(this.m_friendlyHeroHealCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F2 RID: 17906 RVA: 0x00150701 File Offset: 0x0014E901
	public void RegisterOpponentHeroHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentHeroHealCallbacks = this.RegisterEvent(this.m_opponentHeroHealCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F3 RID: 17907 RVA: 0x00150717 File Offset: 0x0014E917
	public void RegisterFriendlyMinionHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_friendlyMinionHealCallbacks = this.RegisterEvent(this.m_friendlyMinionHealCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F4 RID: 17908 RVA: 0x0015072D File Offset: 0x0014E92D
	public void RegisterOpponentMinionHealEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentMinionHealCallbacks = this.RegisterEvent(this.m_opponentMinionHealCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F5 RID: 17909 RVA: 0x00150743 File Offset: 0x0014E943
	public void RegisterFriendlyLegendaryMinionSpawnEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_frindlyLegendaryMinionSpawnCallbacks = this.RegisterEvent(this.m_frindlyLegendaryMinionSpawnCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F6 RID: 17910 RVA: 0x00150759 File Offset: 0x0014E959
	public void RegisterOppenentLegendaryMinionSpawnEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentLegendaryMinionSpawnCallbacks = this.RegisterEvent(this.m_opponentLegendaryMinionSpawnCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F7 RID: 17911 RVA: 0x0015076F File Offset: 0x0014E96F
	public void RegisterFriendlyMinionSpawnEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_frindlyMinionSpawnCallbacks = this.RegisterEvent(this.m_frindlyMinionSpawnCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F8 RID: 17912 RVA: 0x00150785 File Offset: 0x0014E985
	public void RegisterOppenentMinionSpawnEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentMinionSpawnCallbacks = this.RegisterEvent(this.m_opponentMinionSpawnCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045F9 RID: 17913 RVA: 0x0015079B File Offset: 0x0014E99B
	public void RegisterFriendlyLegendaryMinionDeathEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_frindlyLegendaryMinionDeathCallbacks = this.RegisterEvent(this.m_frindlyLegendaryMinionDeathCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045FA RID: 17914 RVA: 0x001507B1 File Offset: 0x0014E9B1
	public void RegisterOppenentLegendaryMinionDeathEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentLegendaryMinionDeathCallbacks = this.RegisterEvent(this.m_opponentLegendaryMinionDeathCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045FB RID: 17915 RVA: 0x001507C7 File Offset: 0x0014E9C7
	public void RegisterFriendlyMinionDeathEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_frindlyMinionDeathCallbacks = this.RegisterEvent(this.m_frindlyMinionDeathCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045FC RID: 17916 RVA: 0x001507DD File Offset: 0x0014E9DD
	public void RegisterOppenentMinionDeathEvent(BoardEvents.EventDelegate callback, float minimumWeight = 1f)
	{
		this.m_opponentMinionDeathCallbacks = this.RegisterEvent(this.m_opponentMinionDeathCallbacks, callback, minimumWeight);
	}

	// Token: 0x060045FD RID: 17917 RVA: 0x001507F4 File Offset: 0x0014E9F4
	private List<BoardEvents.EventCallback> RegisterEvent(List<BoardEvents.EventCallback> eventList, BoardEvents.EventDelegate callback, float minimumWeight)
	{
		if (eventList == null)
		{
			eventList = new List<BoardEvents.EventCallback>();
		}
		eventList.Add(new BoardEvents.EventCallback
		{
			callback = callback,
			minimumWeight = minimumWeight
		});
		return eventList;
	}

	// Token: 0x060045FE RID: 17918 RVA: 0x0015082C File Offset: 0x0014EA2C
	public void MinionShakeEvent(ShakeMinionIntensity shakeIntensity, float customIntensity)
	{
		if (shakeIntensity != ShakeMinionIntensity.LargeShake)
		{
			return;
		}
		BoardEvents.EventData eventData = new BoardEvents.EventData();
		eventData.m_timeStamp = Time.timeSinceLevelLoad;
		eventData.m_eventType = BoardEvents.EVENT_TYPE.LargeMinionShake;
		this.m_fastEvents.AddLast(eventData);
	}

	// Token: 0x060045FF RID: 17919 RVA: 0x00150868 File Offset: 0x0014EA68
	public void DamageEvent(Card targetCard, float damage)
	{
		Entity entity = targetCard.GetEntity();
		if (entity == null)
		{
			return;
		}
		BoardEvents.EventData eventData = new BoardEvents.EventData();
		eventData.m_card = targetCard;
		eventData.m_timeStamp = Time.timeSinceLevelLoad;
		if (entity.IsHero())
		{
			if (targetCard.GetControllerSide() == Player.Side.FRIENDLY)
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyHeroDamage;
			}
			else
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentHeroDamage;
			}
		}
		else if (targetCard.GetControllerSide() == Player.Side.FRIENDLY)
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyMinionDamage;
		}
		else
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentMinionDamage;
		}
		eventData.m_value = damage;
		eventData.m_rarity = entity.GetRarity();
		this.m_events.AddLast(eventData);
	}

	// Token: 0x06004600 RID: 17920 RVA: 0x0015090C File Offset: 0x0014EB0C
	public void HealEvent(Card targetCard, float health)
	{
		Entity entity = targetCard.GetEntity();
		if (entity == null)
		{
			return;
		}
		BoardEvents.EventData eventData = new BoardEvents.EventData();
		eventData.m_card = targetCard;
		eventData.m_timeStamp = Time.timeSinceLevelLoad;
		if (entity.IsHero())
		{
			if (targetCard.GetControllerSide() == Player.Side.FRIENDLY)
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyHeroHeal;
			}
			else
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentHeroHeal;
			}
		}
		else if (targetCard.GetControllerSide() == Player.Side.FRIENDLY)
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyMinionHeal;
		}
		else
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentMinionHeal;
		}
		eventData.m_value = health;
		eventData.m_rarity = entity.GetRarity();
		this.m_events.AddLast(eventData);
	}

	// Token: 0x06004601 RID: 17921 RVA: 0x001509B0 File Offset: 0x0014EBB0
	public void SummonedEvent(Card minionCard)
	{
		Entity entity = minionCard.GetEntity();
		if (entity == null)
		{
			return;
		}
		BoardEvents.EventData eventData = new BoardEvents.EventData();
		eventData.m_card = minionCard;
		eventData.m_timeStamp = Time.timeSinceLevelLoad;
		if (entity.GetRarity() == TAG_RARITY.LEGENDARY)
		{
			if (minionCard.GetControllerSide() == Player.Side.FRIENDLY)
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionSpawn;
			}
			else
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentLegendaryMinionSpawn;
			}
		}
		else if (minionCard.GetControllerSide() == Player.Side.FRIENDLY)
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyMinionSpawn;
		}
		else
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentMinionSpawn;
		}
		eventData.m_value = (float)entity.GetOriginalCost();
		eventData.m_rarity = entity.GetRarity();
		this.m_events.AddLast(eventData);
	}

	// Token: 0x06004602 RID: 17922 RVA: 0x00150A5C File Offset: 0x0014EC5C
	public void DeathEvent(Card card)
	{
		Entity entity = card.GetEntity();
		if (entity == null)
		{
			return;
		}
		BoardEvents.EventData eventData = new BoardEvents.EventData();
		eventData.m_card = card;
		eventData.m_timeStamp = Time.timeSinceLevelLoad;
		if (entity.GetRarity() == TAG_RARITY.LEGENDARY)
		{
			if (card.GetControllerSide() == Player.Side.FRIENDLY)
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionDeath;
			}
			else
			{
				eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentLegendaryMinionDeath;
			}
		}
		else if (card.GetControllerSide() == Player.Side.FRIENDLY)
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.FriendlyMinionDeath;
		}
		else
		{
			eventData.m_eventType = BoardEvents.EVENT_TYPE.OpponentMinionDeath;
		}
		eventData.m_value = (float)entity.GetOriginalCost();
		eventData.m_rarity = entity.GetRarity();
		this.m_events.AddLast(eventData);
	}

	// Token: 0x06004603 RID: 17923 RVA: 0x00150B08 File Offset: 0x0014ED08
	private void ProcessImmediateEvents()
	{
		if (this.m_fastEvents.Count == 0)
		{
			return;
		}
		if (this.m_largeShakeEventCallbacks == null)
		{
			return;
		}
		LinkedListNode<BoardEvents.EventData> linkedListNode = this.m_fastEvents.First;
		while (linkedListNode != null)
		{
			BoardEvents.EventData value = linkedListNode.Value;
			LinkedListNode<BoardEvents.EventData> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode.Next;
			if (value.m_timeStamp + 0.15f < Time.timeSinceLevelLoad)
			{
				this.m_removeEvents.Add(linkedListNode2);
			}
			else if (value.m_eventType == BoardEvents.EVENT_TYPE.LargeMinionShake)
			{
				this.AddWeight(BoardEvents.EVENT_TYPE.LargeMinionShake, 1f);
				this.m_removeEvents.Add(linkedListNode2);
			}
		}
		for (int i = 0; i < this.m_removeEvents.Count; i++)
		{
			LinkedListNode<BoardEvents.EventData> linkedListNode3 = this.m_removeEvents[i];
			if (linkedListNode3 != null)
			{
				this.m_fastEvents.Remove(linkedListNode3);
			}
		}
		this.m_removeEvents.Clear();
		if (this.m_weights.ContainsKey(BoardEvents.EVENT_TYPE.LargeMinionShake) && this.m_weights[BoardEvents.EVENT_TYPE.LargeMinionShake] > 0f)
		{
			this.LargeShakeEvent();
		}
		this.m_weights.Clear();
	}

	// Token: 0x06004604 RID: 17924 RVA: 0x00150C34 File Offset: 0x0014EE34
	private void ProcessEvents()
	{
		if (this.m_events.Count == 0)
		{
			return;
		}
		LinkedListNode<BoardEvents.EventData> linkedListNode = this.m_events.First;
		while (linkedListNode != null)
		{
			BoardEvents.EventData value = linkedListNode.Value;
			LinkedListNode<BoardEvents.EventData> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode.Next;
			if (value.m_timeStamp + 3.5f < Time.timeSinceLevelLoad)
			{
				this.m_removeEvents.Add(linkedListNode2);
			}
			else
			{
				this.AddWeight(value.m_eventType, value.m_value);
			}
		}
		for (int i = 0; i < this.m_removeEvents.Count; i++)
		{
			LinkedListNode<BoardEvents.EventData> linkedListNode3 = this.m_removeEvents[i];
			if (linkedListNode3 != null)
			{
				this.m_events.Remove(linkedListNode3);
			}
		}
		this.m_removeEvents.Clear();
		if (this.m_weights.Count == 0)
		{
			return;
		}
		BoardEvents.EVENT_TYPE? event_TYPE = default(BoardEvents.EVENT_TYPE?);
		float num = -1f;
		foreach (BoardEvents.EVENT_TYPE event_TYPE2 in this.m_weights.Keys)
		{
			if (this.m_weights[event_TYPE2] >= num)
			{
				event_TYPE = new BoardEvents.EVENT_TYPE?(event_TYPE2);
				num = this.m_weights[event_TYPE2];
			}
		}
		if (event_TYPE == null)
		{
			return;
		}
		if (event_TYPE != null)
		{
			switch (event_TYPE.Value)
			{
			case BoardEvents.EVENT_TYPE.FriendlyHeroDamage:
				this.CallbackEvent(this.m_friendlyHeroDamageCallacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentHeroDamage:
				this.CallbackEvent(this.m_opponentHeroDamageCallacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyHeroHeal:
				this.CallbackEvent(this.m_friendlyHeroHealCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentHeroHeal:
				this.CallbackEvent(this.m_opponentHeroHealCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionSpawn:
				this.CallbackEvent(this.m_frindlyLegendaryMinionSpawnCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentLegendaryMinionSpawn:
				this.CallbackEvent(this.m_opponentLegendaryMinionSpawnCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyLegendaryMinionDeath:
				this.CallbackEvent(this.m_frindlyLegendaryMinionDeathCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentLegendaryMinionDeath:
				this.CallbackEvent(this.m_opponentLegendaryMinionDeathCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyMinionSpawn:
				this.CallbackEvent(this.m_frindlyMinionSpawnCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentMinionSpawn:
				this.CallbackEvent(this.m_opponentMinionSpawnCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyMinionDeath:
				this.CallbackEvent(this.m_frindlyMinionDeathCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentMinionDeath:
				this.CallbackEvent(this.m_opponentMinionDeathCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyMinionDamage:
				this.CallbackEvent(this.m_friendlyMinionDamageCallacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentMinionDamage:
				this.CallbackEvent(this.m_opponentMinionDamageCallacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.FriendlyMinionHeal:
				this.CallbackEvent(this.m_friendlyMinionHealCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			case BoardEvents.EVENT_TYPE.OpponentMinionHeal:
				this.CallbackEvent(this.m_opponentMinionHealCallbacks, num);
				this.m_events.Clear();
				goto IL_3BE;
			}
		}
		Debug.LogWarning(string.Format("BoardEvents: Event type unknown when processing event weights: {0}", event_TYPE));
		IL_3BE:
		this.m_weights.Clear();
	}

	// Token: 0x06004605 RID: 17925 RVA: 0x0015101C File Offset: 0x0014F21C
	private void LargeShakeEvent()
	{
		if (this.m_largeShakeEventCallbacks == null)
		{
			return;
		}
		for (int i = this.m_largeShakeEventCallbacks.Count - 1; i >= 0; i--)
		{
			if (this.m_largeShakeEventCallbacks[i] == null)
			{
				this.m_largeShakeEventCallbacks.RemoveAt(i);
			}
			else
			{
				this.m_largeShakeEventCallbacks[i]();
			}
		}
	}

	// Token: 0x06004606 RID: 17926 RVA: 0x00151088 File Offset: 0x0014F288
	private void CallbackEvent(List<BoardEvents.EventCallback> eventList, float weight)
	{
		if (eventList == null)
		{
			return;
		}
		for (int i = eventList.Count - 1; i >= 0; i--)
		{
			if (eventList[i] == null)
			{
				eventList.RemoveAt(i);
			}
			else if (weight >= eventList[i].minimumWeight)
			{
				eventList[i].callback(weight);
			}
		}
	}

	// Token: 0x06004607 RID: 17927 RVA: 0x001510F4 File Offset: 0x0014F2F4
	private void AddWeight(BoardEvents.EVENT_TYPE eventType, float weight)
	{
		if (this.m_weights.ContainsKey(eventType))
		{
			Dictionary<BoardEvents.EVENT_TYPE, float> weights;
			Dictionary<BoardEvents.EVENT_TYPE, float> dictionary = weights = this.m_weights;
			float num = weights[eventType];
			dictionary[eventType] = num + weight;
		}
		else
		{
			this.m_weights.Add(eventType, weight);
		}
	}

	// Token: 0x04002CEC RID: 11500
	private const float AI_PROCESS_INTERVAL = 3.5f;

	// Token: 0x04002CED RID: 11501
	private const float PROCESS_INTERVAL = 1.25f;

	// Token: 0x04002CEE RID: 11502
	private const float FAST_PROCESS_INTERVAL = 0.15f;

	// Token: 0x04002CEF RID: 11503
	private float m_nextProcessTime;

	// Token: 0x04002CF0 RID: 11504
	private float m_nextFastProcessTime;

	// Token: 0x04002CF1 RID: 11505
	private LinkedList<BoardEvents.EventData> m_events = new LinkedList<BoardEvents.EventData>();

	// Token: 0x04002CF2 RID: 11506
	private LinkedList<BoardEvents.EventData> m_fastEvents = new LinkedList<BoardEvents.EventData>();

	// Token: 0x04002CF3 RID: 11507
	private List<LinkedListNode<BoardEvents.EventData>> m_removeEvents = new List<LinkedListNode<BoardEvents.EventData>>();

	// Token: 0x04002CF4 RID: 11508
	private Dictionary<BoardEvents.EVENT_TYPE, float> m_weights = new Dictionary<BoardEvents.EVENT_TYPE, float>();

	// Token: 0x04002CF5 RID: 11509
	private List<BoardEvents.LargeShakeEventDelegate> m_largeShakeEventCallbacks;

	// Token: 0x04002CF6 RID: 11510
	private List<BoardEvents.EventCallback> m_friendlyHeroDamageCallacks;

	// Token: 0x04002CF7 RID: 11511
	private List<BoardEvents.EventCallback> m_opponentHeroDamageCallacks;

	// Token: 0x04002CF8 RID: 11512
	private List<BoardEvents.EventCallback> m_opponentMinionDamageCallacks;

	// Token: 0x04002CF9 RID: 11513
	private List<BoardEvents.EventCallback> m_friendlyMinionDamageCallacks;

	// Token: 0x04002CFA RID: 11514
	private List<BoardEvents.EventCallback> m_friendlyHeroHealCallbacks;

	// Token: 0x04002CFB RID: 11515
	private List<BoardEvents.EventCallback> m_opponentHeroHealCallbacks;

	// Token: 0x04002CFC RID: 11516
	private List<BoardEvents.EventCallback> m_friendlyMinionHealCallbacks;

	// Token: 0x04002CFD RID: 11517
	private List<BoardEvents.EventCallback> m_opponentMinionHealCallbacks;

	// Token: 0x04002CFE RID: 11518
	private List<BoardEvents.EventCallback> m_frindlyLegendaryMinionSpawnCallbacks;

	// Token: 0x04002CFF RID: 11519
	private List<BoardEvents.EventCallback> m_opponentLegendaryMinionSpawnCallbacks;

	// Token: 0x04002D00 RID: 11520
	private List<BoardEvents.EventCallback> m_frindlyMinionSpawnCallbacks;

	// Token: 0x04002D01 RID: 11521
	private List<BoardEvents.EventCallback> m_opponentMinionSpawnCallbacks;

	// Token: 0x04002D02 RID: 11522
	private List<BoardEvents.EventCallback> m_frindlyLegendaryMinionDeathCallbacks;

	// Token: 0x04002D03 RID: 11523
	private List<BoardEvents.EventCallback> m_opponentLegendaryMinionDeathCallbacks;

	// Token: 0x04002D04 RID: 11524
	private List<BoardEvents.EventCallback> m_frindlyMinionDeathCallbacks;

	// Token: 0x04002D05 RID: 11525
	private List<BoardEvents.EventCallback> m_opponentMinionDeathCallbacks;

	// Token: 0x04002D06 RID: 11526
	private static BoardEvents s_instance;

	// Token: 0x02000661 RID: 1633
	// (Invoke) Token: 0x06004609 RID: 17929
	public delegate void LargeShakeEventDelegate();

	// Token: 0x0200066E RID: 1646
	// (Invoke) Token: 0x06004670 RID: 18032
	public delegate void EventDelegate(float weight);

	// Token: 0x0200066F RID: 1647
	public enum EVENT_TYPE
	{
		// Token: 0x04002D6E RID: 11630
		FriendlyHeroDamage,
		// Token: 0x04002D6F RID: 11631
		OpponentHeroDamage,
		// Token: 0x04002D70 RID: 11632
		FriendlyHeroHeal,
		// Token: 0x04002D71 RID: 11633
		OpponentHeroHeal,
		// Token: 0x04002D72 RID: 11634
		FriendlyLegendaryMinionSpawn,
		// Token: 0x04002D73 RID: 11635
		OpponentLegendaryMinionSpawn,
		// Token: 0x04002D74 RID: 11636
		FriendlyLegendaryMinionDeath,
		// Token: 0x04002D75 RID: 11637
		OpponentLegendaryMinionDeath,
		// Token: 0x04002D76 RID: 11638
		FriendlyMinionSpawn,
		// Token: 0x04002D77 RID: 11639
		OpponentMinionSpawn,
		// Token: 0x04002D78 RID: 11640
		FriendlyMinionDeath,
		// Token: 0x04002D79 RID: 11641
		OpponentMinionDeath,
		// Token: 0x04002D7A RID: 11642
		FriendlyMinionDamage,
		// Token: 0x04002D7B RID: 11643
		OpponentMinionDamage,
		// Token: 0x04002D7C RID: 11644
		FriendlyMinionHeal,
		// Token: 0x04002D7D RID: 11645
		OpponentMinionHeal,
		// Token: 0x04002D7E RID: 11646
		LargeMinionShake
	}

	// Token: 0x02000670 RID: 1648
	public class EventData
	{
		// Token: 0x04002D7F RID: 11647
		public BoardEvents.EVENT_TYPE m_eventType;

		// Token: 0x04002D80 RID: 11648
		public float m_timeStamp;

		// Token: 0x04002D81 RID: 11649
		public Card m_card;

		// Token: 0x04002D82 RID: 11650
		public float m_value;

		// Token: 0x04002D83 RID: 11651
		public TAG_RARITY m_rarity;
	}

	// Token: 0x02000671 RID: 1649
	public class EventCallback
	{
		// Token: 0x04002D84 RID: 11652
		public BoardEvents.EventDelegate callback;

		// Token: 0x04002D85 RID: 11653
		public float minimumWeight;
	}
}
