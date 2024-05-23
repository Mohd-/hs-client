using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003E7 RID: 999
public class ZoneMgr : MonoBehaviour
{
	// Token: 0x0600339E RID: 13214 RVA: 0x001010C8 File Offset: 0x000FF2C8
	private void Awake()
	{
		ZoneMgr.s_instance = this;
		Zone[] componentsInChildren = base.gameObject.GetComponentsInChildren<Zone>();
		foreach (Zone zone in componentsInChildren)
		{
			this.m_zones.Add(zone);
		}
		if (GameState.Get() != null)
		{
			GameState.Get().RegisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
			GameState.Get().RegisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected), null);
		}
	}

	// Token: 0x0600339F RID: 13215 RVA: 0x00101148 File Offset: 0x000FF348
	private void Start()
	{
		InputManager inputManager = InputManager.Get();
		if (inputManager != null)
		{
			inputManager.StartWatchingForInput();
		}
	}

	// Token: 0x060033A0 RID: 13216 RVA: 0x0010116D File Offset: 0x000FF36D
	private void Update()
	{
		this.UpdateLocalChangeLists();
		this.UpdateServerChangeLists();
	}

	// Token: 0x060033A1 RID: 13217 RVA: 0x0010117C File Offset: 0x000FF37C
	private void OnDestroy()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().UnregisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
			GameState.Get().UnregisterOptionRejectedListener(new GameState.OptionRejectedCallback(this.OnOptionRejected), null);
		}
		ZoneMgr.s_instance = null;
	}

	// Token: 0x060033A2 RID: 13218 RVA: 0x001011C8 File Offset: 0x000FF3C8
	public static ZoneMgr Get()
	{
		return ZoneMgr.s_instance;
	}

	// Token: 0x060033A3 RID: 13219 RVA: 0x001011CF File Offset: 0x000FF3CF
	public List<Zone> GetZones()
	{
		return this.m_zones;
	}

	// Token: 0x060033A4 RID: 13220 RVA: 0x001011D8 File Offset: 0x000FF3D8
	public Zone FindZoneForTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		if (controllerId == 0)
		{
			return null;
		}
		if (zoneTag == TAG_ZONE.INVALID)
		{
			return null;
		}
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(controllerId, zoneTag, cardType))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033A5 RID: 13221 RVA: 0x00101254 File Offset: 0x000FF454
	public Zone FindZoneForEntity(Entity entity)
	{
		if (entity.GetZone() == TAG_ZONE.INVALID)
		{
			return null;
		}
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(entity.GetControllerId(), entity.GetZone(), entity.GetCardType()))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033A6 RID: 13222 RVA: 0x001012E0 File Offset: 0x000FF4E0
	public Zone FindZoneForEntityAndZoneTag(Entity entity, TAG_ZONE zoneTag)
	{
		if (zoneTag == TAG_ZONE.INVALID)
		{
			return null;
		}
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(entity.GetControllerId(), zoneTag, entity.GetCardType()))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033A7 RID: 13223 RVA: 0x00101360 File Offset: 0x000FF560
	public Zone FindZoneForEntityAndController(Entity entity, int controllerId)
	{
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(controllerId, entity.GetZone(), entity.GetCardType()))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033A8 RID: 13224 RVA: 0x001013D8 File Offset: 0x000FF5D8
	public Zone FindZoneForFullEntity(Network.HistFullEntity fullEntity)
	{
		int controllerId = 0;
		TAG_ZONE zoneTag = TAG_ZONE.INVALID;
		TAG_CARDTYPE cardType = TAG_CARDTYPE.INVALID;
		Network.Entity entity = fullEntity.Entity;
		foreach (Network.Entity.Tag tag in entity.Tags)
		{
			GAME_TAG name = (GAME_TAG)tag.Name;
			if (name != GAME_TAG.ZONE)
			{
				if (name != GAME_TAG.CONTROLLER)
				{
					if (name == GAME_TAG.CARDTYPE)
					{
						cardType = (TAG_CARDTYPE)tag.Value;
					}
				}
				else
				{
					controllerId = tag.Value;
				}
			}
			else
			{
				zoneTag = (TAG_ZONE)tag.Value;
			}
		}
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(controllerId, zoneTag, cardType))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033A9 RID: 13225 RVA: 0x001014F4 File Offset: 0x000FF6F4
	public Zone FindZoneForShowEntity(Entity entity, Network.HistShowEntity showEntity)
	{
		int controllerId = entity.GetControllerId();
		TAG_ZONE zoneTag = entity.GetZone();
		TAG_CARDTYPE cardType = entity.GetCardType();
		Network.Entity entity2 = showEntity.Entity;
		foreach (Network.Entity.Tag tag in entity2.Tags)
		{
			GAME_TAG name = (GAME_TAG)tag.Name;
			if (name != GAME_TAG.ZONE)
			{
				if (name != GAME_TAG.CONTROLLER)
				{
					if (name == GAME_TAG.CARDTYPE)
					{
						cardType = (TAG_CARDTYPE)tag.Value;
					}
				}
				else
				{
					controllerId = tag.Value;
				}
			}
			else
			{
				zoneTag = (TAG_ZONE)tag.Value;
			}
		}
		foreach (Zone zone in this.m_zones)
		{
			if (zone.CanAcceptTags(controllerId, zoneTag, cardType))
			{
				return zone;
			}
		}
		return null;
	}

	// Token: 0x060033AA RID: 13226 RVA: 0x00101620 File Offset: 0x000FF820
	public T FindZoneOfType<T>(Player.Side side) where T : Zone
	{
		Type typeFromHandle = typeof(T);
		foreach (Zone zone in this.m_zones)
		{
			if (zone.GetType() == typeFromHandle)
			{
				if (zone.m_Side == side)
				{
					return (T)((object)zone);
				}
			}
		}
		return (T)((object)null);
	}

	// Token: 0x060033AB RID: 13227 RVA: 0x001016B4 File Offset: 0x000FF8B4
	public List<T> FindZonesOfType<T>() where T : Zone
	{
		return this.FindZonesOfType<T, T>();
	}

	// Token: 0x060033AC RID: 13228 RVA: 0x001016BC File Offset: 0x000FF8BC
	public List<ReturnType> FindZonesOfType<ReturnType, ArgType>() where ReturnType : Zone where ArgType : Zone
	{
		List<ReturnType> list = new List<ReturnType>();
		Type typeFromHandle = typeof(ArgType);
		foreach (Zone zone in this.m_zones)
		{
			if (zone.GetType() == typeFromHandle)
			{
				list.Add((ReturnType)((object)zone));
			}
		}
		return list;
	}

	// Token: 0x060033AD RID: 13229 RVA: 0x00101740 File Offset: 0x000FF940
	public List<T> FindZonesOfType<T>(Player.Side side) where T : Zone
	{
		return this.FindZonesOfType<T, T>(side);
	}

	// Token: 0x060033AE RID: 13230 RVA: 0x0010174C File Offset: 0x000FF94C
	public List<ReturnType> FindZonesOfType<ReturnType, ArgType>(Player.Side side) where ReturnType : Zone where ArgType : Zone
	{
		List<ReturnType> list = new List<ReturnType>();
		foreach (Zone zone in this.m_zones)
		{
			if (zone is ArgType)
			{
				if (zone.m_Side == side)
				{
					list.Add((ReturnType)((object)zone));
				}
			}
		}
		return list;
	}

	// Token: 0x060033AF RID: 13231 RVA: 0x001017D4 File Offset: 0x000FF9D4
	public List<Zone> FindZonesForTag(TAG_ZONE zoneTag)
	{
		List<Zone> list = new List<Zone>();
		foreach (Zone zone in this.m_zones)
		{
			if (zone.m_ServerTag == zoneTag)
			{
				list.Add(zone);
			}
		}
		return list;
	}

	// Token: 0x060033B0 RID: 13232 RVA: 0x00101848 File Offset: 0x000FFA48
	public Map<Type, string> GetTweenNames()
	{
		return this.m_tweenNames;
	}

	// Token: 0x060033B1 RID: 13233 RVA: 0x00101850 File Offset: 0x000FFA50
	public string GetTweenName<T>() where T : Zone
	{
		Type typeFromHandle = typeof(T);
		string empty = string.Empty;
		this.m_tweenNames.TryGetValue(typeFromHandle, out empty);
		return empty;
	}

	// Token: 0x060033B2 RID: 13234 RVA: 0x0010187E File Offset: 0x000FFA7E
	public void RequestNextDeathBlockLayoutDelaySec(float sec)
	{
		this.m_nextDeathBlockLayoutDelaySec = Mathf.Max(this.m_nextDeathBlockLayoutDelaySec, sec);
	}

	// Token: 0x060033B3 RID: 13235 RVA: 0x00101894 File Offset: 0x000FFA94
	public float RemoveNextDeathBlockLayoutDelaySec()
	{
		float nextDeathBlockLayoutDelaySec = this.m_nextDeathBlockLayoutDelaySec;
		this.m_nextDeathBlockLayoutDelaySec = 0f;
		return nextDeathBlockLayoutDelaySec;
	}

	// Token: 0x060033B4 RID: 13236 RVA: 0x001018B4 File Offset: 0x000FFAB4
	public int PredictZonePosition(Zone zone, int pos)
	{
		ZoneMgr.TempZone tempZone = this.BuildTempZone(zone);
		this.PredictZoneFromPowerProcessor(tempZone);
		int result = this.FindBestInsertionPosition(tempZone, pos - 1, pos);
		this.m_tempZoneMap.Clear();
		this.m_tempEntityMap.Clear();
		return result;
	}

	// Token: 0x060033B5 RID: 13237 RVA: 0x001018F4 File Offset: 0x000FFAF4
	public bool HasPredictedCards()
	{
		return this.HasPredictedPositions() || this.HasPredictedWeapons() || this.HasPredictedSecrets();
	}

	// Token: 0x060033B6 RID: 13238 RVA: 0x0010192C File Offset: 0x000FFB2C
	public bool HasPredictedPositions()
	{
		List<Zone> list = this.FindZonesOfType<Zone>(Player.Side.FRIENDLY);
		foreach (Zone zone in list)
		{
			foreach (Card card in zone.GetCards())
			{
				if (card.GetPredictedZonePosition() != 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060033B7 RID: 13239 RVA: 0x001019E0 File Offset: 0x000FFBE0
	public bool HasPredictedWeapons()
	{
		List<ZoneWeapon> list = this.FindZonesOfType<ZoneWeapon>(Player.Side.FRIENDLY);
		foreach (ZoneWeapon zoneWeapon in list)
		{
			foreach (Card card in zoneWeapon.GetCards())
			{
				Entity entity = card.GetEntity();
				if (entity.GetZone() != TAG_ZONE.PLAY)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060033B8 RID: 13240 RVA: 0x00101A9C File Offset: 0x000FFC9C
	public bool HasPredictedSecrets()
	{
		List<ZoneSecret> list = this.FindZonesOfType<ZoneSecret>(Player.Side.FRIENDLY);
		foreach (ZoneSecret zoneSecret in list)
		{
			foreach (Card card in zoneSecret.GetCards())
			{
				Entity entity = card.GetEntity();
				if (entity.GetZone() != TAG_ZONE.SECRET)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060033B9 RID: 13241 RVA: 0x00101B58 File Offset: 0x000FFD58
	public bool HasActiveLocalChange()
	{
		return this.m_activeLocalChangeLists.Count > 0;
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x00101B68 File Offset: 0x000FFD68
	public bool HasPendingLocalChange()
	{
		return this.m_pendingLocalChangeLists.Count > 0;
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x00101B78 File Offset: 0x000FFD78
	public bool HasTriggeredActiveLocalChange(Entity entity)
	{
		return this.HasTriggeredActiveLocalChange(entity.GetCard());
	}

	// Token: 0x060033BC RID: 13244 RVA: 0x00101B88 File Offset: 0x000FFD88
	public bool HasTriggeredActiveLocalChange(Card card)
	{
		int num = this.FindTriggeredActiveLocalChangeIndex(card);
		return num >= 0;
	}

	// Token: 0x060033BD RID: 13245 RVA: 0x00101BA4 File Offset: 0x000FFDA4
	public ZoneChangeList FindTriggeredActiveLocalChange(Entity entity)
	{
		return this.FindTriggeredActiveLocalChange(entity.GetCard());
	}

	// Token: 0x060033BE RID: 13246 RVA: 0x00101BB4 File Offset: 0x000FFDB4
	public ZoneChangeList FindTriggeredActiveLocalChange(Card card)
	{
		int num = this.FindTriggeredActiveLocalChangeIndex(card);
		if (num < 0)
		{
			return null;
		}
		return this.m_pendingLocalChangeLists[num];
	}

	// Token: 0x060033BF RID: 13247 RVA: 0x00101BDE File Offset: 0x000FFDDE
	public bool HasTriggeredPendingLocalChange(Entity entity)
	{
		return this.HasTriggeredPendingLocalChange(entity.GetCard());
	}

	// Token: 0x060033C0 RID: 13248 RVA: 0x00101BEC File Offset: 0x000FFDEC
	public bool HasTriggeredPendingLocalChange(Card card)
	{
		int num = this.FindTriggeredPendingLocalChangeIndex(card);
		return num >= 0;
	}

	// Token: 0x060033C1 RID: 13249 RVA: 0x00101C08 File Offset: 0x000FFE08
	public ZoneChangeList FindTriggeredPendingLocalChange(Entity entity)
	{
		return this.FindTriggeredPendingLocalChange(entity.GetCard());
	}

	// Token: 0x060033C2 RID: 13250 RVA: 0x00101C18 File Offset: 0x000FFE18
	public ZoneChangeList FindTriggeredPendingLocalChange(Card card)
	{
		int num = this.FindTriggeredPendingLocalChangeIndex(card);
		if (num < 0)
		{
			return null;
		}
		return this.m_pendingLocalChangeLists[num];
	}

	// Token: 0x060033C3 RID: 13251 RVA: 0x00101C42 File Offset: 0x000FFE42
	public bool IsChangeInLocalHistory(ZoneChangeList changeList)
	{
		return this.m_localChangeListHistory.GetList().Contains(changeList);
	}

	// Token: 0x060033C4 RID: 13252 RVA: 0x00101C58 File Offset: 0x000FFE58
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, TAG_ZONE zoneTag)
	{
		Entity entity = triggerCard.GetEntity();
		Zone destinationZone = this.FindZoneForEntityAndZoneTag(entity, zoneTag);
		return this.AddLocalZoneChange(triggerCard, destinationZone, zoneTag, 0, null, null);
	}

	// Token: 0x060033C5 RID: 13253 RVA: 0x00101C84 File Offset: 0x000FFE84
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, TAG_ZONE zoneTag, int destinationPos)
	{
		Zone destinationZone = this.FindZoneForEntityAndZoneTag(triggerCard.GetEntity(), zoneTag);
		return this.AddLocalZoneChange(triggerCard, destinationZone, zoneTag, destinationPos, null, null);
	}

	// Token: 0x060033C6 RID: 13254 RVA: 0x00101CAC File Offset: 0x000FFEAC
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, TAG_ZONE zoneTag, int destinationPos, ZoneMgr.ChangeCompleteCallback callback, object userData)
	{
		Zone destinationZone = this.FindZoneForEntityAndZoneTag(triggerCard.GetEntity(), zoneTag);
		return this.AddLocalZoneChange(triggerCard, destinationZone, zoneTag, destinationPos, callback, userData);
	}

	// Token: 0x060033C7 RID: 13255 RVA: 0x00101CD8 File Offset: 0x000FFED8
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, Zone destinationZone, int destinationPos)
	{
		if (destinationZone == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.AddLocalZoneChange() - illegal zone change to null zone for card {0}", triggerCard));
			return null;
		}
		return this.AddLocalZoneChange(triggerCard, destinationZone, destinationZone.m_ServerTag, destinationPos, null, null);
	}

	// Token: 0x060033C8 RID: 13256 RVA: 0x00101D14 File Offset: 0x000FFF14
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, Zone destinationZone, int destinationPos, ZoneMgr.ChangeCompleteCallback callback, object userData)
	{
		if (destinationZone == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.AddLocalZoneChange() - illegal zone change to null zone for card {0}", triggerCard));
			return null;
		}
		return this.AddLocalZoneChange(triggerCard, destinationZone, destinationZone.m_ServerTag, destinationPos, callback, userData);
	}

	// Token: 0x060033C9 RID: 13257 RVA: 0x00101D54 File Offset: 0x000FFF54
	public ZoneChangeList AddLocalZoneChange(Card triggerCard, Zone destinationZone, TAG_ZONE destinationZoneTag, int destinationPos, ZoneMgr.ChangeCompleteCallback callback, object userData)
	{
		if (destinationZoneTag == TAG_ZONE.INVALID)
		{
			Debug.LogWarning(string.Format("ZoneMgr.AddLocalZoneChange() - illegal zone change to {0} for card {1}", destinationZoneTag, triggerCard));
			return null;
		}
		bool flag = destinationZone is ZonePlay || destinationZone is ZoneHand;
		if (flag && destinationPos <= 0)
		{
			Debug.LogWarning(string.Format("ZoneMgr.AddLocalZoneChange() - destinationPos {0} is too small for zone {1}, min is 1", destinationPos, destinationZone));
			return null;
		}
		ZoneChangeList zoneChangeList = this.CreateLocalChangeList(triggerCard, destinationZone, destinationZoneTag, destinationPos, callback, userData);
		this.ProcessOrEnqueueLocalChangeList(zoneChangeList);
		this.m_localChangeListHistory.Enqueue(zoneChangeList);
		return zoneChangeList;
	}

	// Token: 0x060033CA RID: 13258 RVA: 0x00101DE4 File Offset: 0x000FFFE4
	public ZoneChangeList AddPredictedLocalZoneChange(Card triggerCard, Zone destinationZone, int destinationPos, int predictedPos)
	{
		if (triggerCard == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.AddPredictedLocalZoneChange() - triggerCard is null", new object[0]));
			return null;
		}
		ZoneChangeList zoneChangeList = this.AddLocalZoneChange(triggerCard, destinationZone, destinationPos);
		if (zoneChangeList == null)
		{
			return null;
		}
		triggerCard.SetPredictedZonePosition(predictedPos);
		zoneChangeList.SetPredictedPosition(predictedPos);
		return zoneChangeList;
	}

	// Token: 0x060033CB RID: 13259 RVA: 0x00101E38 File Offset: 0x00100038
	public ZoneChangeList CancelLocalZoneChange(ZoneChangeList changeList, ZoneMgr.ChangeCompleteCallback callback = null, object userData = null)
	{
		if (changeList == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.CancelLocalZoneChange() - changeList is null", new object[0]));
			return null;
		}
		if (!this.m_localChangeListHistory.Remove(changeList))
		{
			Debug.LogWarning(string.Format("ZoneMgr.CancelLocalZoneChange() - changeList {0} is not in history", changeList.GetId()));
			return null;
		}
		ZoneChange localTriggerChange = changeList.GetLocalTriggerChange();
		Entity entity = localTriggerChange.GetEntity();
		Card card = entity.GetCard();
		Zone sourceZone = localTriggerChange.GetSourceZone();
		int sourcePosition = localTriggerChange.GetSourcePosition();
		ZoneChangeList zoneChangeList = this.CreateLocalChangeList(card, sourceZone, sourceZone.m_ServerTag, sourcePosition, callback, userData);
		zoneChangeList.SetCanceledChangeList(true);
		zoneChangeList.SetZoneInputBlocking(true);
		this.ProcessOrEnqueueLocalChangeList(zoneChangeList);
		return zoneChangeList;
	}

	// Token: 0x060033CC RID: 13260 RVA: 0x00101EE4 File Offset: 0x001000E4
	public static bool IsHandledPower(Network.PowerHistory power)
	{
		switch (power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
		{
			Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
			bool result = false;
			foreach (Network.Entity.Tag tag in histFullEntity.Entity.Tags)
			{
				if (tag.Name == 202)
				{
					if (tag.Value == 1)
					{
						return false;
					}
					if (tag.Value == 2)
					{
						return false;
					}
				}
				else if (tag.Name == 49 || tag.Name == 263 || tag.Name == 50)
				{
					result = true;
				}
			}
			return result;
		}
		case Network.PowerType.SHOW_ENTITY:
			return true;
		case Network.PowerType.HIDE_ENTITY:
			return true;
		case Network.PowerType.TAG_CHANGE:
		{
			Network.HistTagChange histTagChange = power as Network.HistTagChange;
			if (histTagChange.Tag == 49 || histTagChange.Tag == 263 || histTagChange.Tag == 50)
			{
				Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
				if (entity != null)
				{
					if (entity.IsPlayer())
					{
						return false;
					}
					if (entity.IsGame())
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
		default:
			return false;
		}
	}

	// Token: 0x060033CD RID: 13261 RVA: 0x00102050 File Offset: 0x00100250
	public bool HasActiveServerChange()
	{
		return this.m_activeServerChangeList != null;
	}

	// Token: 0x060033CE RID: 13262 RVA: 0x0010205E File Offset: 0x0010025E
	public bool HasPendingServerChange()
	{
		return this.m_pendingServerChangeLists.Count > 0;
	}

	// Token: 0x060033CF RID: 13263 RVA: 0x00102070 File Offset: 0x00100270
	public ZoneChangeList AddServerZoneChanges(PowerTaskList taskList, int taskStartIndex, int taskEndIndex, ZoneMgr.ChangeCompleteCallback callback, object userData)
	{
		int nextServerChangeListId = this.GetNextServerChangeListId();
		ZoneChangeList zoneChangeList = new ZoneChangeList();
		zoneChangeList.SetId(nextServerChangeListId);
		zoneChangeList.SetTaskList(taskList);
		zoneChangeList.SetCompleteCallback(callback);
		zoneChangeList.SetCompleteCallbackUserData(userData);
		Log.Zone.Print("ZoneMgr.AddServerZoneChanges() - taskListId={0} changeListId={1} taskStart={2} taskEnd={3}", new object[]
		{
			taskList.GetId(),
			nextServerChangeListId,
			taskStartIndex,
			taskEndIndex
		});
		List<PowerTask> taskList2 = taskList.GetTaskList();
		int i = taskStartIndex;
		while (i <= taskEndIndex)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.PowerType type = power.Type;
			ZoneChange zoneChange;
			switch (type)
			{
			case Network.PowerType.FULL_ENTITY:
			{
				Network.HistFullEntity fullEntity = (Network.HistFullEntity)power;
				zoneChange = this.CreateZoneChangeFromFullEntity(fullEntity);
				break;
			}
			case Network.PowerType.SHOW_ENTITY:
			{
				Network.HistShowEntity histShowEntity = (Network.HistShowEntity)power;
				zoneChange = this.CreateZoneChangeFromEntity(histShowEntity.Entity);
				break;
			}
			case Network.PowerType.HIDE_ENTITY:
			{
				Network.HistHideEntity hideEntity = (Network.HistHideEntity)power;
				zoneChange = this.CreateZoneChangeFromHideEntity(hideEntity);
				break;
			}
			case Network.PowerType.TAG_CHANGE:
			{
				Network.HistTagChange tagChange = (Network.HistTagChange)power;
				zoneChange = this.CreateZoneChangeFromTagChange(tagChange);
				break;
			}
			case Network.PowerType.BLOCK_START:
			case Network.PowerType.BLOCK_END:
				goto IL_17D;
			case Network.PowerType.CREATE_GAME:
			{
				Network.HistCreateGame createGame = (Network.HistCreateGame)power;
				zoneChange = this.CreateZoneChangeFromCreateGame(createGame);
				break;
			}
			case Network.PowerType.META_DATA:
			{
				Network.HistMetaData metaData = (Network.HistMetaData)power;
				zoneChange = this.CreateZoneChangeFromMetaData(metaData);
				break;
			}
			case Network.PowerType.CHANGE_ENTITY:
			{
				Network.HistChangeEntity histChangeEntity = (Network.HistChangeEntity)power;
				zoneChange = this.CreateZoneChangeFromEntity(histChangeEntity.Entity);
				break;
			}
			default:
				goto IL_17D;
			}
			if (zoneChange != null)
			{
				zoneChange.SetParentList(zoneChangeList);
				zoneChange.SetPowerTask(powerTask);
				zoneChangeList.AddChange(zoneChange);
			}
			i++;
			continue;
			IL_17D:
			Debug.LogError(string.Format("ZoneMgr.AddServerZoneChanges() - id={0} received unhandled power of type {1}", zoneChangeList.GetId(), type));
			return null;
		}
		this.m_tempEntityMap.Clear();
		this.m_pendingServerChangeLists.Enqueue(zoneChangeList);
		return zoneChangeList;
	}

	// Token: 0x060033D0 RID: 13264 RVA: 0x00102268 File Offset: 0x00100468
	private void UpdateLocalChangeLists()
	{
		List<ZoneChangeList> list = null;
		int i = 0;
		while (i < this.m_activeLocalChangeLists.Count)
		{
			ZoneChangeList zoneChangeList = this.m_activeLocalChangeLists[i];
			if (!zoneChangeList.IsComplete())
			{
				i++;
			}
			else
			{
				zoneChangeList.FireCompleteCallback();
				this.m_activeLocalChangeLists.RemoveAt(i);
				if (list == null)
				{
					list = new List<ZoneChangeList>();
				}
				list.Add(zoneChangeList);
			}
		}
		if (list == null)
		{
			return;
		}
		bool flag = false;
		for (int j = 0; j < list.Count; j++)
		{
			ZoneChangeList zoneChangeList2 = list[j];
			ZoneChange localTriggerChange = zoneChangeList2.GetLocalTriggerChange();
			Entity entity = localTriggerChange.GetEntity();
			Card card = entity.GetCard();
			if (zoneChangeList2.IsCanceledChangeList())
			{
				flag = true;
				card.SetPredictedZonePosition(0);
			}
			int num = this.FindTriggeredPendingLocalChangeIndex(entity);
			if (num >= 0)
			{
				ZoneChangeList zoneChangeList3 = this.m_pendingLocalChangeLists[num];
				this.m_pendingLocalChangeLists.RemoveAt(num);
				this.CreateLocalChangesFromTrigger(zoneChangeList3, zoneChangeList3.GetLocalTriggerChange());
				this.ProcessLocalChangeList(zoneChangeList3);
			}
		}
		if (flag)
		{
			this.AutoCorrectZonesAfterLocalChange();
		}
	}

	// Token: 0x060033D1 RID: 13265 RVA: 0x0010238C File Offset: 0x0010058C
	private void UpdateServerChangeLists()
	{
		if (this.m_activeServerChangeList != null && this.m_activeServerChangeList.IsComplete())
		{
			this.m_activeServerChangeList.FireCompleteCallback();
			this.m_activeServerChangeList = null;
			this.AutoCorrectZonesAfterServerChange();
		}
		if (this.HasPendingServerChange() && !this.HasActiveServerChange())
		{
			this.m_activeServerChangeList = this.m_pendingServerChangeLists.Dequeue();
			this.PostProcessServerChangeList(this.m_activeServerChangeList);
			base.StartCoroutine(this.m_activeServerChangeList.ProcessChanges());
		}
	}

	// Token: 0x060033D2 RID: 13266 RVA: 0x00102411 File Offset: 0x00100611
	private bool HasLocalChangeExitingZone(Entity entity, Zone zone)
	{
		return this.HasLocalChangeExitingZone(entity, zone, this.m_activeLocalChangeLists) || this.HasLocalChangeExitingZone(entity, zone, this.m_pendingLocalChangeLists);
	}

	// Token: 0x060033D3 RID: 13267 RVA: 0x00102440 File Offset: 0x00100640
	private bool HasLocalChangeExitingZone(Entity entity, Zone zone, List<ZoneChangeList> changeLists)
	{
		TAG_ZONE serverTag = zone.m_ServerTag;
		foreach (ZoneChangeList zoneChangeList in changeLists)
		{
			foreach (ZoneChange zoneChange in zoneChangeList.GetChanges())
			{
				if (entity == zoneChange.GetEntity())
				{
					if (serverTag == zoneChange.GetSourceZoneTag())
					{
						if (serverTag != zoneChange.GetDestinationZoneTag())
						{
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	// Token: 0x060033D4 RID: 13268 RVA: 0x00102518 File Offset: 0x00100718
	private void PredictZoneFromPowerProcessor(ZoneMgr.TempZone tempZone)
	{
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		tempZone.PreprocessChanges();
		powerProcessor.ForEachTaskList(delegate(int queueIndex, PowerTaskList taskList)
		{
			this.PredictZoneFromPowerTaskList(tempZone, taskList);
		});
		tempZone.Sort();
		tempZone.PostprocessChanges();
	}

	// Token: 0x060033D5 RID: 13269 RVA: 0x00102578 File Offset: 0x00100778
	private void PredictZoneFromPowerTaskList(ZoneMgr.TempZone tempZone, PowerTaskList taskList)
	{
		List<PowerTask> taskList2 = taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			this.PredictZoneFromPower(tempZone, power);
		}
	}

	// Token: 0x060033D6 RID: 13270 RVA: 0x001025BC File Offset: 0x001007BC
	private void PredictZoneFromPower(ZoneMgr.TempZone tempZone, Network.PowerHistory power)
	{
		switch (power.Type)
		{
		case Network.PowerType.FULL_ENTITY:
			this.PredictZoneFromFullEntity(tempZone, (Network.HistFullEntity)power);
			break;
		case Network.PowerType.SHOW_ENTITY:
			this.PredictZoneFromShowEntity(tempZone, (Network.HistShowEntity)power);
			break;
		case Network.PowerType.HIDE_ENTITY:
			this.PredictZoneFromHideEntity(tempZone, (Network.HistHideEntity)power);
			break;
		case Network.PowerType.TAG_CHANGE:
			this.PredictZoneFromTagChange(tempZone, (Network.HistTagChange)power);
			break;
		}
	}

	// Token: 0x060033D7 RID: 13271 RVA: 0x00102638 File Offset: 0x00100838
	private void PredictZoneFromFullEntity(ZoneMgr.TempZone tempZone, Network.HistFullEntity fullEntity)
	{
		Entity entity = this.RegisterTempEntity(fullEntity.Entity);
		Zone zone = tempZone.GetZone();
		bool flag = entity.GetZone() == zone.m_ServerTag;
		bool flag2 = entity.GetControllerId() == zone.GetControllerId();
		if (!flag)
		{
			return;
		}
		if (!flag2)
		{
			return;
		}
		tempZone.AddEntity(entity);
	}

	// Token: 0x060033D8 RID: 13272 RVA: 0x0010268C File Offset: 0x0010088C
	private void PredictZoneFromShowEntity(ZoneMgr.TempZone tempZone, Network.HistShowEntity showEntity)
	{
		Entity tempEntity = this.RegisterTempEntity(showEntity.Entity);
		foreach (Network.Entity.Tag tag in showEntity.Entity.Tags)
		{
			this.PredictZoneByApplyingTag(tempZone, tempEntity, (GAME_TAG)tag.Name, tag.Value);
		}
	}

	// Token: 0x060033D9 RID: 13273 RVA: 0x00102708 File Offset: 0x00100908
	private void PredictZoneFromHideEntity(ZoneMgr.TempZone tempZone, Network.HistHideEntity hideEntity)
	{
		Entity tempEntity = this.RegisterTempEntity(hideEntity.Entity);
		this.PredictZoneByApplyingTag(tempZone, tempEntity, GAME_TAG.ZONE, hideEntity.Zone);
	}

	// Token: 0x060033DA RID: 13274 RVA: 0x00102734 File Offset: 0x00100934
	private void PredictZoneFromTagChange(ZoneMgr.TempZone tempZone, Network.HistTagChange tagChange)
	{
		Entity tempEntity = this.RegisterTempEntity(tagChange.Entity);
		this.PredictZoneByApplyingTag(tempZone, tempEntity, (GAME_TAG)tagChange.Tag, tagChange.Value);
	}

	// Token: 0x060033DB RID: 13275 RVA: 0x00102764 File Offset: 0x00100964
	private void PredictZoneByApplyingTag(ZoneMgr.TempZone tempZone, Entity tempEntity, GAME_TAG tag, int val)
	{
		if (tag != GAME_TAG.ZONE && tag != GAME_TAG.CONTROLLER)
		{
			tempEntity.SetTag(tag, val);
			return;
		}
		Zone zone = tempZone.GetZone();
		bool flag = tempEntity.GetZone() == zone.m_ServerTag;
		bool flag2 = tempEntity.GetControllerId() == zone.GetControllerId();
		if (flag && flag2)
		{
			tempZone.RemoveEntity(tempEntity);
		}
		tempEntity.SetTag(tag, val);
		flag = (tempEntity.GetZone() == zone.m_ServerTag);
		flag2 = (tempEntity.GetControllerId() == zone.GetControllerId());
		if (flag && flag2)
		{
			tempZone.AddEntity(tempEntity);
		}
	}

	// Token: 0x060033DC RID: 13276 RVA: 0x00102808 File Offset: 0x00100A08
	private ZoneChangeList CreateLocalChangeList(Card triggerCard, Zone destinationZone, TAG_ZONE destinationZoneTag, int destinationPos, ZoneMgr.ChangeCompleteCallback callback, object userData)
	{
		int nextLocalChangeListId = this.GetNextLocalChangeListId();
		Log.Zone.Print("ZoneMgr.CreateLocalChangeList() - changeListId={0}", new object[]
		{
			nextLocalChangeListId
		});
		ZoneChangeList zoneChangeList = new ZoneChangeList();
		zoneChangeList.SetId(nextLocalChangeListId);
		zoneChangeList.SetCompleteCallback(callback);
		zoneChangeList.SetCompleteCallbackUserData(userData);
		Entity entity = triggerCard.GetEntity();
		Zone zone = triggerCard.GetZone();
		TAG_ZONE sourceZoneTag = (!(zone == null)) ? zone.m_ServerTag : TAG_ZONE.INVALID;
		int zonePosition = triggerCard.GetZonePosition();
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetParentList(zoneChangeList);
		zoneChange.SetEntity(entity);
		zoneChange.SetSourceZone(zone);
		zoneChange.SetSourceZoneTag(sourceZoneTag);
		zoneChange.SetSourcePosition(zonePosition);
		zoneChange.SetDestinationZone(destinationZone);
		zoneChange.SetDestinationZoneTag(destinationZoneTag);
		zoneChange.SetDestinationPosition(destinationPos);
		zoneChangeList.AddChange(zoneChange);
		return zoneChangeList;
	}

	// Token: 0x060033DD RID: 13277 RVA: 0x001028DC File Offset: 0x00100ADC
	private void ProcessOrEnqueueLocalChangeList(ZoneChangeList changeList)
	{
		ZoneChange localTriggerChange = changeList.GetLocalTriggerChange();
		Card card = localTriggerChange.GetEntity().GetCard();
		if (this.HasTriggeredActiveLocalChange(card))
		{
			this.m_pendingLocalChangeLists.Add(changeList);
		}
		else
		{
			this.CreateLocalChangesFromTrigger(changeList, localTriggerChange);
			this.ProcessLocalChangeList(changeList);
		}
	}

	// Token: 0x060033DE RID: 13278 RVA: 0x00102928 File Offset: 0x00100B28
	private void CreateLocalChangesFromTrigger(ZoneChangeList changeList, ZoneChange triggerChange)
	{
		Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - {0}", new object[]
		{
			changeList
		});
		Entity entity = triggerChange.GetEntity();
		Zone sourceZone = triggerChange.GetSourceZone();
		TAG_ZONE sourceZoneTag = triggerChange.GetSourceZoneTag();
		int sourcePosition = triggerChange.GetSourcePosition();
		Zone destinationZone = triggerChange.GetDestinationZone();
		TAG_ZONE destinationZoneTag = triggerChange.GetDestinationZoneTag();
		int destinationPosition = triggerChange.GetDestinationPosition();
		if (sourceZoneTag != destinationZoneTag)
		{
			this.CreateLocalChangesFromTrigger(changeList, entity, sourceZone, sourceZoneTag, sourcePosition, destinationZone, destinationZoneTag, destinationPosition);
		}
		else if (sourcePosition != destinationPosition)
		{
			this.CreateLocalPosOnlyChangesFromTrigger(changeList, entity, sourceZone, sourcePosition, destinationPosition);
		}
	}

	// Token: 0x060033DF RID: 13279 RVA: 0x001029B4 File Offset: 0x00100BB4
	private void CreateLocalChangesFromTrigger(ZoneChangeList changeList, Entity triggerEntity, Zone sourceZone, TAG_ZONE sourceZoneTag, int sourcePos, Zone destinationZone, TAG_ZONE destinationZoneTag, int destinationPos)
	{
		Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - triggerEntity={0} srcZone={1} srcPos={2} dstZone={3} dstPos={4}", new object[]
		{
			triggerEntity,
			sourceZoneTag,
			sourcePos,
			destinationZoneTag,
			destinationPos
		});
		if (sourcePos != destinationPos)
		{
			Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - srcPos={0} destPos={1}", new object[]
			{
				sourcePos,
				destinationPos
			});
		}
		if (sourceZone != null)
		{
			List<Card> cards = sourceZone.GetCards();
			for (int i = sourcePos; i < cards.Count; i++)
			{
				Card card = cards[i];
				Entity entity = card.GetEntity();
				ZoneChange zoneChange = new ZoneChange();
				zoneChange.SetParentList(changeList);
				zoneChange.SetEntity(entity);
				int num = i;
				zoneChange.SetSourcePosition(card.GetZonePosition());
				zoneChange.SetDestinationPosition(num);
				Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - srcZone card {0} zonePos {1} -> {2}", new object[]
				{
					card,
					card.GetZonePosition(),
					num
				});
				changeList.AddChange(zoneChange);
			}
		}
		if (destinationZone != null)
		{
			if (!(destinationZone is ZoneSecret))
			{
				if (destinationZone is ZoneWeapon)
				{
					List<Card> cards2 = destinationZone.GetCards();
					if (cards2.Count > 0)
					{
						Card card2 = cards2[0];
						Entity entity2 = card2.GetEntity();
						ZoneChange zoneChange2 = new ZoneChange();
						zoneChange2.SetParentList(changeList);
						zoneChange2.SetEntity(entity2);
						zoneChange2.SetDestinationZone(this.FindZoneOfType<ZoneGraveyard>(destinationZone.m_Side));
						zoneChange2.SetDestinationZoneTag(TAG_ZONE.GRAVEYARD);
						changeList.AddChange(zoneChange2);
					}
				}
				else if (destinationZone is ZonePlay || destinationZone is ZoneHand)
				{
					List<Card> cards3 = destinationZone.GetCards();
					for (int j = destinationPos - 1; j < cards3.Count; j++)
					{
						Card card3 = cards3[j];
						Entity entity3 = card3.GetEntity();
						int num2 = j + 2;
						ZoneChange zoneChange3 = new ZoneChange();
						zoneChange3.SetParentList(changeList);
						zoneChange3.SetEntity(entity3);
						zoneChange3.SetDestinationPosition(num2);
						Log.Zone.Print("ZoneMgr.CreateLocalChangesFromTrigger() - dstZone card {0} zonePos {1} -> {2}", new object[]
						{
							card3,
							entity3.GetZonePosition(),
							num2
						});
						changeList.AddChange(zoneChange3);
					}
				}
				else
				{
					Debug.LogError(string.Format("ZoneMgr.CreateLocalChangesFromTrigger() - don't know how to predict zone position changes for zone {0}", destinationZone));
				}
			}
		}
	}

	// Token: 0x060033E0 RID: 13280 RVA: 0x00102C38 File Offset: 0x00100E38
	private void CreateLocalPosOnlyChangesFromTrigger(ZoneChangeList changeList, Entity triggerEntity, Zone sourceZone, int sourcePos, int destinationPos)
	{
		List<Card> cards = sourceZone.GetCards();
		if (sourcePos < destinationPos)
		{
			int num = sourcePos;
			while (num < destinationPos && num < cards.Count)
			{
				Card card = cards[num];
				Entity entity = card.GetEntity();
				ZoneChange zoneChange = new ZoneChange();
				zoneChange.SetParentList(changeList);
				zoneChange.SetEntity(entity);
				int destinationPosition = num;
				zoneChange.SetSourcePosition(card.GetZonePosition());
				zoneChange.SetDestinationPosition(destinationPosition);
				changeList.AddChange(zoneChange);
				num++;
			}
		}
		else
		{
			for (int i = destinationPos - 1; i < sourcePos - 1; i++)
			{
				Card card2 = cards[i];
				Entity entity2 = card2.GetEntity();
				ZoneChange zoneChange2 = new ZoneChange();
				zoneChange2.SetParentList(changeList);
				zoneChange2.SetEntity(entity2);
				int destinationPosition2 = i + 2;
				zoneChange2.SetSourcePosition(card2.GetZonePosition());
				zoneChange2.SetDestinationPosition(destinationPosition2);
				changeList.AddChange(zoneChange2);
			}
		}
	}

	// Token: 0x060033E1 RID: 13281 RVA: 0x00102D30 File Offset: 0x00100F30
	private void ProcessLocalChangeList(ZoneChangeList changeList)
	{
		Log.Zone.Print("ZoneMgr.ProcessLocalChangeList() - [{0}]", new object[]
		{
			changeList
		});
		this.m_activeLocalChangeLists.Add(changeList);
		base.StartCoroutine(changeList.ProcessChanges());
	}

	// Token: 0x060033E2 RID: 13282 RVA: 0x00102D6F File Offset: 0x00100F6F
	private void OnCurrentPlayerChanged(Player player, object userData)
	{
		if (player.IsLocalUser())
		{
			this.m_localChangeListHistory.Clear();
		}
	}

	// Token: 0x060033E3 RID: 13283 RVA: 0x00102D88 File Offset: 0x00100F88
	private void OnOptionRejected(Network.Options.Option option, object userData)
	{
		if (option.Type != Network.Options.Option.OptionType.POWER)
		{
			return;
		}
		Entity entity = GameState.Get().GetEntity(option.Main.ID);
		ZoneChangeList zoneChangeList = this.FindRejectedLocalZoneChange(entity);
		if (zoneChangeList == null)
		{
			Log.Zone.Print("ZoneMgr.RejectLocalZoneChange() - did not find a zone change to reject for {0}", new object[]
			{
				entity
			});
			return;
		}
		Card card = entity.GetCard();
		card.SetPredictedZonePosition(0);
		this.CancelLocalZoneChange(zoneChangeList, null, null);
	}

	// Token: 0x060033E4 RID: 13284 RVA: 0x00102DF8 File Offset: 0x00100FF8
	private ZoneChangeList FindRejectedLocalZoneChange(Entity triggerEntity)
	{
		List<ZoneChangeList> list = this.m_localChangeListHistory.GetList();
		for (int i = 0; i < list.Count; i++)
		{
			ZoneChangeList zoneChangeList = list[i];
			List<ZoneChange> changes = zoneChangeList.GetChanges();
			for (int j = 0; j < changes.Count; j++)
			{
				ZoneChange zoneChange = changes[j];
				if (zoneChange.GetEntity() == triggerEntity)
				{
					if (zoneChange.GetDestinationZoneTag() == TAG_ZONE.PLAY)
					{
						return zoneChangeList;
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060033E5 RID: 13285 RVA: 0x00102E84 File Offset: 0x00101084
	private ZoneChange CreateZoneChangeFromCreateGame(Network.HistCreateGame createGame)
	{
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(GameState.Get().GetGameEntity());
		return zoneChange;
	}

	// Token: 0x060033E6 RID: 13286 RVA: 0x00102EA8 File Offset: 0x001010A8
	private ZoneChange CreateZoneChangeFromFullEntity(Network.HistFullEntity fullEntity)
	{
		Network.Entity entity = fullEntity.Entity;
		Entity entity2 = GameState.Get().GetEntity(entity.ID);
		if (entity2 == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.CreateZoneChangeFromFullEntity() - WARNING entity {0} DOES NOT EXIST!", entity.ID));
			return null;
		}
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(entity2);
		Card card = entity2.GetCard();
		if (card == null)
		{
			return zoneChange;
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (Network.Entity.Tag tag in entity.Tags)
		{
			if (tag.Name == 49)
			{
				zoneChange.SetDestinationZoneTag((TAG_ZONE)tag.Value);
				flag = true;
				if (flag2 && flag3)
				{
					break;
				}
			}
			else if (tag.Name == 263)
			{
				zoneChange.SetDestinationPosition(tag.Value);
				flag2 = true;
				if (flag && flag3)
				{
					break;
				}
			}
			else if (tag.Name == 50)
			{
				zoneChange.SetDestinationControllerId(tag.Value);
				flag3 = true;
				if (flag && flag2)
				{
					break;
				}
			}
		}
		if (flag || flag3)
		{
			zoneChange.SetDestinationZone(this.FindZoneForEntity(entity2));
		}
		return zoneChange;
	}

	// Token: 0x060033E7 RID: 13287 RVA: 0x00103020 File Offset: 0x00101220
	private ZoneChange CreateZoneChangeFromEntity(Network.Entity netEnt)
	{
		Entity entity = GameState.Get().GetEntity(netEnt.ID);
		if (entity == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.CreateZoneChangeFromEntity() - WARNING entity {0} DOES NOT EXIST!", netEnt.ID));
			return null;
		}
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(entity);
		Card card = entity.GetCard();
		if (card == null)
		{
			return zoneChange;
		}
		Entity entity2 = this.RegisterTempEntity(netEnt.ID, entity);
		foreach (Network.Entity.Tag tag in netEnt.Tags)
		{
			entity2.SetTag(tag.Name, tag.Value);
		}
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		foreach (Network.Entity.Tag tag2 in netEnt.Tags)
		{
			entity2.SetTag(tag2.Name, tag2.Value);
			if (tag2.Name == 49)
			{
				zoneChange.SetDestinationZoneTag((TAG_ZONE)tag2.Value);
				flag = true;
				if (flag2 && flag3)
				{
					break;
				}
			}
			else if (tag2.Name == 263)
			{
				zoneChange.SetDestinationPosition(tag2.Value);
				flag2 = true;
				if (flag && flag3)
				{
					break;
				}
			}
			else if (tag2.Name == 50)
			{
				zoneChange.SetDestinationControllerId(tag2.Value);
				flag3 = true;
				if (flag && flag2)
				{
					break;
				}
			}
		}
		if (flag || flag3)
		{
			zoneChange.SetDestinationZone(this.FindZoneForEntity(entity2));
		}
		return zoneChange;
	}

	// Token: 0x060033E8 RID: 13288 RVA: 0x0010320C File Offset: 0x0010140C
	private ZoneChange CreateZoneChangeFromHideEntity(Network.HistHideEntity hideEntity)
	{
		Entity entity = GameState.Get().GetEntity(hideEntity.Entity);
		if (entity == null)
		{
			Debug.LogWarning(string.Format("ZoneMgr.CreateZoneChangeFromHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", hideEntity.Entity, hideEntity.Zone));
			return null;
		}
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(entity);
		Card card = entity.GetCard();
		if (card == null)
		{
			return zoneChange;
		}
		Entity entity2 = this.RegisterTempEntity(hideEntity.Entity, entity);
		entity2.SetTag(GAME_TAG.ZONE, hideEntity.Zone);
		TAG_ZONE zone = (TAG_ZONE)hideEntity.Zone;
		zoneChange.SetDestinationZoneTag(zone);
		zoneChange.SetDestinationZone(this.FindZoneForEntity(entity2));
		return zoneChange;
	}

	// Token: 0x060033E9 RID: 13289 RVA: 0x001032B4 File Offset: 0x001014B4
	private ZoneChange CreateZoneChangeFromTagChange(Network.HistTagChange tagChange)
	{
		Entity entity = GameState.Get().GetEntity(tagChange.Entity);
		if (entity == null)
		{
			Debug.LogError(string.Format("ZoneMgr.CreateZoneChangeFromTagChange() - Entity {0} does not exist", tagChange.Entity));
			return null;
		}
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(entity);
		Card card = entity.GetCard();
		if (card == null)
		{
			return zoneChange;
		}
		Entity entity2 = this.RegisterTempEntity(tagChange.Entity, entity);
		entity2.SetTag(tagChange.Tag, tagChange.Value);
		if (tagChange.Tag == 49)
		{
			if (card == null)
			{
				Debug.LogError(string.Format("ZoneMgr.CreateZoneChangeFromTagChange() - {0} does not have a card visual", entity));
				return null;
			}
			TAG_ZONE value = (TAG_ZONE)tagChange.Value;
			zoneChange.SetDestinationZoneTag(value);
			zoneChange.SetDestinationZone(this.FindZoneForEntity(entity2));
		}
		else if (tagChange.Tag == 263)
		{
			if (card == null)
			{
				Debug.LogError(string.Format("ZoneMgr.CreateZoneChangeFromTagChange() - {0} does not have a card visual", entity));
				return null;
			}
			zoneChange.SetDestinationPosition(tagChange.Value);
		}
		else if (tagChange.Tag == 50)
		{
			if (card == null)
			{
				Debug.LogError(string.Format("ZoneMgr.CreateZoneChangeFromTagChange() - {0} does not have a card visual", entity));
				return null;
			}
			int value2 = tagChange.Value;
			zoneChange.SetDestinationControllerId(value2);
			zoneChange.SetDestinationZone(this.FindZoneForEntity(entity2));
		}
		return zoneChange;
	}

	// Token: 0x060033EA RID: 13290 RVA: 0x0010340C File Offset: 0x0010160C
	private ZoneChange CreateZoneChangeFromMetaData(Network.HistMetaData metaData)
	{
		if (metaData.Info.Count <= 0)
		{
			return null;
		}
		Entity entity = GameState.Get().GetEntity(metaData.Info[0]);
		if (entity == null)
		{
			Debug.LogError(string.Format("ZoneMgr.CreateZoneChangeFromMetaData() - Entity {0} does not exist", metaData.Info[0]));
			return null;
		}
		ZoneChange zoneChange = new ZoneChange();
		zoneChange.SetEntity(entity);
		return zoneChange;
	}

	// Token: 0x060033EB RID: 13291 RVA: 0x0010347C File Offset: 0x0010167C
	private Entity RegisterTempEntity(int id)
	{
		Entity entity = GameState.Get().GetEntity(id);
		return this.RegisterTempEntity(id, entity);
	}

	// Token: 0x060033EC RID: 13292 RVA: 0x001034A0 File Offset: 0x001016A0
	private Entity RegisterTempEntity(Network.Entity netEnt)
	{
		Entity entity = GameState.Get().GetEntity(netEnt.ID);
		return this.RegisterTempEntity(netEnt.ID, entity);
	}

	// Token: 0x060033ED RID: 13293 RVA: 0x001034CB File Offset: 0x001016CB
	private Entity RegisterTempEntity(Entity entity)
	{
		return this.RegisterTempEntity(entity.GetEntityId(), entity);
	}

	// Token: 0x060033EE RID: 13294 RVA: 0x001034DC File Offset: 0x001016DC
	private Entity RegisterTempEntity(int id, Entity entity)
	{
		Entity entity2;
		if (!this.m_tempEntityMap.TryGetValue(id, out entity2))
		{
			entity2 = entity.CloneForZoneMgr();
			this.m_tempEntityMap.Add(id, entity2);
		}
		return entity2;
	}

	// Token: 0x060033EF RID: 13295 RVA: 0x00103514 File Offset: 0x00101714
	private void PostProcessServerChangeList(ZoneChangeList serverChangeList)
	{
		if (!this.ShouldPostProcessServerChangeList(serverChangeList))
		{
			return;
		}
		if (this.CheckAndIgnoreServerChangeList(serverChangeList))
		{
			return;
		}
		if (this.ReplaceRemoteWeaponInServerChangeList(serverChangeList))
		{
			return;
		}
		this.MergeServerChangeList(serverChangeList);
	}

	// Token: 0x060033F0 RID: 13296 RVA: 0x00103550 File Offset: 0x00101750
	private bool ShouldPostProcessServerChangeList(ZoneChangeList changeList)
	{
		List<ZoneChange> changes = changeList.GetChanges();
		for (int i = 0; i < changes.Count; i++)
		{
			ZoneChange zoneChange = changes[i];
			if (zoneChange.HasDestinationData())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060033F1 RID: 13297 RVA: 0x00103594 File Offset: 0x00101794
	private bool CheckAndIgnoreServerChangeList(ZoneChangeList serverChangeList)
	{
		PowerTaskList taskList = serverChangeList.GetTaskList();
		Network.HistBlockStart blockStart = taskList.GetBlockStart();
		if (blockStart == null)
		{
			return false;
		}
		if (blockStart.BlockType != 7)
		{
			return false;
		}
		ZoneChangeList zoneChangeList = this.FindLocalChangeListMatchingServerChangeList(serverChangeList);
		if (zoneChangeList == null)
		{
			return false;
		}
		serverChangeList.SetIgnoreCardZoneChanges(true);
		while (this.m_localChangeListHistory.Count > 0)
		{
			ZoneChangeList zoneChangeList2 = this.m_localChangeListHistory.Dequeue();
			if (zoneChangeList == zoneChangeList2)
			{
				Card localTriggerCard = zoneChangeList.GetLocalTriggerCard();
				localTriggerCard.SetPredictedZonePosition(0);
				break;
			}
		}
		return true;
	}

	// Token: 0x060033F2 RID: 13298 RVA: 0x0010361C File Offset: 0x0010181C
	private ZoneChangeList FindLocalChangeListMatchingServerChangeList(ZoneChangeList serverChangeList)
	{
		foreach (ZoneChangeList zoneChangeList in this.m_localChangeListHistory)
		{
			int predictedPosition = zoneChangeList.GetPredictedPosition();
			foreach (ZoneChange zoneChange in zoneChangeList.GetChanges())
			{
				Entity entity = zoneChange.GetEntity();
				TAG_ZONE destinationZoneTag = zoneChange.GetDestinationZoneTag();
				if (destinationZoneTag != TAG_ZONE.INVALID)
				{
					List<ZoneChange> changes = serverChangeList.GetChanges();
					for (int i = 0; i < changes.Count; i++)
					{
						ZoneChange zoneChange2 = changes[i];
						Entity entity2 = zoneChange2.GetEntity();
						if (entity == entity2)
						{
							TAG_ZONE destinationZoneTag2 = zoneChange2.GetDestinationZoneTag();
							if (destinationZoneTag == destinationZoneTag2)
							{
								ZoneChange zoneChange3 = this.FindNextDstPosChange(serverChangeList, i, entity2);
								int num = (zoneChange3 != null) ? zoneChange3.GetDestinationPosition() : entity2.GetZonePosition();
								if (predictedPosition == num)
								{
									return zoneChangeList;
								}
							}
						}
					}
				}
			}
		}
		return null;
	}

	// Token: 0x060033F3 RID: 13299 RVA: 0x0010378C File Offset: 0x0010198C
	private ZoneChange FindNextDstPosChange(ZoneChangeList changeList, int index, Entity entity)
	{
		List<ZoneChange> changes = changeList.GetChanges();
		int i = index;
		while (i < changes.Count)
		{
			ZoneChange zoneChange = changes[i];
			if (zoneChange.HasDestinationZoneChange() && i != index)
			{
				return null;
			}
			if (zoneChange.HasDestinationPosition())
			{
				if (zoneChange.GetEntity() != entity)
				{
					return null;
				}
				return zoneChange;
			}
			else
			{
				i++;
			}
		}
		return null;
	}

	// Token: 0x060033F4 RID: 13300 RVA: 0x001037F0 File Offset: 0x001019F0
	private bool ReplaceRemoteWeaponInServerChangeList(ZoneChangeList serverChangeList)
	{
		List<ZoneChange> changes = serverChangeList.GetChanges();
		ZoneChange zoneChange = changes.Find(delegate(ZoneChange change)
		{
			Zone destinationZone3 = change.GetDestinationZone();
			if (!(destinationZone3 is ZoneWeapon))
			{
				return false;
			}
			Player controller = destinationZone3.GetController();
			return !controller.IsFriendlySide();
		});
		if (zoneChange == null)
		{
			return false;
		}
		Zone destinationZone = zoneChange.GetDestinationZone();
		if (destinationZone.GetCardCount() == 0)
		{
			return false;
		}
		Card cardAtIndex = destinationZone.GetCardAtIndex(0);
		Entity entity = cardAtIndex.GetEntity();
		int controllerId = entity.GetControllerId();
		Zone destinationZone2 = this.FindZoneForTags(controllerId, TAG_ZONE.GRAVEYARD, TAG_CARDTYPE.WEAPON);
		ZoneChange zoneChange2 = new ZoneChange();
		zoneChange2.SetEntity(entity);
		zoneChange2.SetDestinationZone(destinationZone2);
		zoneChange2.SetDestinationZoneTag(TAG_ZONE.GRAVEYARD);
		zoneChange2.SetDestinationPosition(0);
		serverChangeList.AddChange(zoneChange2);
		return true;
	}

	// Token: 0x060033F5 RID: 13301 RVA: 0x0010389C File Offset: 0x00101A9C
	private bool MergeServerChangeList(ZoneChangeList serverChangeList)
	{
		foreach (Zone zone in this.m_zones)
		{
			if (this.IsZoneInLocalHistory(zone))
			{
				ZoneMgr.TempZone tempZone = this.BuildTempZone(zone);
				this.m_tempZoneMap[zone] = tempZone;
				tempZone.PreprocessChanges();
			}
		}
		List<ZoneChange> changes = serverChangeList.GetChanges();
		for (int i = 0; i < changes.Count; i++)
		{
			ZoneChange change = changes[i];
			this.TempApplyZoneChange(change);
		}
		bool result = false;
		foreach (ZoneMgr.TempZone tempZone2 in this.m_tempZoneMap.Values)
		{
			tempZone2.Sort();
			tempZone2.PostprocessChanges();
			Zone zone2 = tempZone2.GetZone();
			for (int j = 1; j < zone2.GetLastPos(); j++)
			{
				Card cardAtPos = zone2.GetCardAtPos(j);
				Entity entity = cardAtPos.GetEntity();
				if (cardAtPos.GetPredictedZonePosition() != 0)
				{
					int pos = this.FindBestInsertionPosition(tempZone2, j - 1, j + 1);
					tempZone2.InsertEntityAtPos(pos, entity);
				}
			}
			if (tempZone2.IsModified())
			{
				result = true;
				for (int k = 1; k < tempZone2.GetLastPos(); k++)
				{
					Entity entityAtPos = tempZone2.GetEntityAtPos(k);
					Card card = entityAtPos.GetCard();
					Entity entity2 = card.GetEntity();
					ZoneChange zoneChange = new ZoneChange();
					zoneChange.SetEntity(entity2);
					zoneChange.SetDestinationZone(zone2);
					zoneChange.SetDestinationZoneTag(zone2.m_ServerTag);
					zoneChange.SetDestinationPosition(k);
					serverChangeList.AddChange(zoneChange);
				}
			}
		}
		this.m_tempZoneMap.Clear();
		this.m_tempEntityMap.Clear();
		return result;
	}

	// Token: 0x060033F6 RID: 13302 RVA: 0x00103AC8 File Offset: 0x00101CC8
	private bool IsZoneInLocalHistory(Zone zone)
	{
		foreach (ZoneChangeList zoneChangeList in this.m_localChangeListHistory)
		{
			foreach (ZoneChange zoneChange in zoneChangeList.GetChanges())
			{
				Zone sourceZone = zoneChange.GetSourceZone();
				Zone destinationZone = zoneChange.GetDestinationZone();
				if (zone == sourceZone || zone == destinationZone)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060033F7 RID: 13303 RVA: 0x00103B90 File Offset: 0x00101D90
	private void TempApplyZoneChange(ZoneChange change)
	{
		PowerTask powerTask = change.GetPowerTask();
		Network.PowerHistory power = powerTask.GetPower();
		Entity entity = change.GetEntity();
		Entity entity2 = this.RegisterTempEntity(entity);
		if (!change.HasDestinationZoneChange())
		{
			GameUtils.ApplyPower(entity2, power);
			return;
		}
		Zone zone = this.FindZoneForEntity(entity2);
		ZoneMgr.TempZone tempZone = this.FindTempZoneForZone(zone);
		if (tempZone != null)
		{
			tempZone.RemoveEntity(entity2);
		}
		GameUtils.ApplyPower(entity2, power);
		Zone destinationZone = change.GetDestinationZone();
		ZoneMgr.TempZone tempZone2 = this.FindTempZoneForZone(destinationZone);
		if (tempZone2 != null)
		{
			tempZone2.AddEntity(entity2);
		}
	}

	// Token: 0x060033F8 RID: 13304 RVA: 0x00103C18 File Offset: 0x00101E18
	private ZoneMgr.TempZone BuildTempZone(Zone zone)
	{
		ZoneMgr.TempZone tempZone = new ZoneMgr.TempZone();
		tempZone.SetZone(zone);
		List<Card> cards = zone.GetCards();
		for (int i = 0; i < cards.Count; i++)
		{
			Card card = cards[i];
			if (card.GetPredictedZonePosition() == 0)
			{
				Entity entity = card.GetEntity();
				Entity entity2 = this.RegisterTempEntity(entity);
				tempZone.AddInitialEntity(entity2);
			}
		}
		return tempZone;
	}

	// Token: 0x060033F9 RID: 13305 RVA: 0x00103C84 File Offset: 0x00101E84
	private ZoneMgr.TempZone FindTempZoneForZone(Zone zone)
	{
		if (zone == null)
		{
			return null;
		}
		ZoneMgr.TempZone result = null;
		this.m_tempZoneMap.TryGetValue(zone, out result);
		return result;
	}

	// Token: 0x060033FA RID: 13306 RVA: 0x00103CB4 File Offset: 0x00101EB4
	private int FindBestInsertionPosition(ZoneMgr.TempZone tempZone, int leftPos, int rightPos)
	{
		Zone zone = tempZone.GetZone();
		int num = 0;
		for (int i = leftPos - 1; i >= 0; i--)
		{
			Card cardAtIndex = zone.GetCardAtIndex(i);
			Entity entity = cardAtIndex.GetEntity();
			num = tempZone.FindEntityPosWithReplacements(entity.GetEntityId());
			if (num != 0)
			{
				break;
			}
		}
		int j;
		if (num == 0)
		{
			j = 1;
		}
		else
		{
			Entity entityAtPos = tempZone.GetEntityAtPos(num);
			int entityId = entityAtPos.GetEntityId();
			for (j = num + 1; j < tempZone.GetLastPos(); j++)
			{
				Entity entityAtPos2 = tempZone.GetEntityAtPos(j);
				if (entityAtPos2.GetCreatorId() != entityId)
				{
					break;
				}
				if (zone.ContainsCard(entityAtPos2.GetCard()))
				{
					break;
				}
			}
		}
		int num2 = 0;
		for (int k = rightPos - 1; k < zone.GetCardCount(); k++)
		{
			Card cardAtIndex2 = zone.GetCardAtIndex(k);
			Entity entity2 = cardAtIndex2.GetEntity();
			num2 = tempZone.FindEntityPosWithReplacements(entity2.GetEntityId());
			if (num2 != 0)
			{
				break;
			}
		}
		int l;
		if (num2 == 0)
		{
			l = tempZone.GetLastPos();
		}
		else
		{
			Entity entityAtPos3 = tempZone.GetEntityAtPos(num2);
			int entityId2 = entityAtPos3.GetEntityId();
			for (l = num2 - 1; l > 0; l--)
			{
				Entity entityAtPos4 = tempZone.GetEntityAtPos(l);
				if (entityAtPos4.GetCreatorId() != entityId2)
				{
					break;
				}
				if (zone.ContainsCard(entityAtPos4.GetCard()))
				{
					break;
				}
			}
			l++;
		}
		return Mathf.CeilToInt(0.5f * (float)(j + l));
	}

	// Token: 0x060033FB RID: 13307 RVA: 0x00103E58 File Offset: 0x00102058
	private int GetNextLocalChangeListId()
	{
		int nextLocalChangeListId = this.m_nextLocalChangeListId;
		this.m_nextLocalChangeListId = ((this.m_nextLocalChangeListId != int.MaxValue) ? (this.m_nextLocalChangeListId + 1) : 1);
		return nextLocalChangeListId;
	}

	// Token: 0x060033FC RID: 13308 RVA: 0x00103E94 File Offset: 0x00102094
	private int GetNextServerChangeListId()
	{
		int nextServerChangeListId = this.m_nextServerChangeListId;
		this.m_nextServerChangeListId = ((this.m_nextServerChangeListId != int.MaxValue) ? (this.m_nextServerChangeListId + 1) : 1);
		return nextServerChangeListId;
	}

	// Token: 0x060033FD RID: 13309 RVA: 0x00103ECD File Offset: 0x001020CD
	private int FindTriggeredActiveLocalChangeIndex(Entity entity)
	{
		return this.FindTriggeredActiveLocalChangeIndex(entity.GetCard());
	}

	// Token: 0x060033FE RID: 13310 RVA: 0x00103EDC File Offset: 0x001020DC
	private int FindTriggeredActiveLocalChangeIndex(Card card)
	{
		for (int i = 0; i < this.m_activeLocalChangeLists.Count; i++)
		{
			ZoneChangeList zoneChangeList = this.m_activeLocalChangeLists[i];
			Card localTriggerCard = zoneChangeList.GetLocalTriggerCard();
			if (localTriggerCard == card)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060033FF RID: 13311 RVA: 0x00103F28 File Offset: 0x00102128
	private int FindTriggeredPendingLocalChangeIndex(Entity entity)
	{
		return this.FindTriggeredPendingLocalChangeIndex(entity.GetCard());
	}

	// Token: 0x06003400 RID: 13312 RVA: 0x00103F38 File Offset: 0x00102138
	private int FindTriggeredPendingLocalChangeIndex(Card card)
	{
		for (int i = 0; i < this.m_pendingLocalChangeLists.Count; i++)
		{
			ZoneChangeList zoneChangeList = this.m_pendingLocalChangeLists[i];
			Card localTriggerCard = zoneChangeList.GetLocalTriggerCard();
			if (localTriggerCard == card)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003401 RID: 13313 RVA: 0x00103F84 File Offset: 0x00102184
	private void AutoCorrectZonesAfterServerChange()
	{
		if (this.HasActiveLocalChange())
		{
			return;
		}
		if (this.HasPendingLocalChange())
		{
			return;
		}
		if (this.HasActiveServerChange())
		{
			return;
		}
		if (this.HasPendingServerChange())
		{
			return;
		}
		if (this.HasPredictedCards())
		{
			return;
		}
		this.AutoCorrectZones();
	}

	// Token: 0x06003402 RID: 13314 RVA: 0x00103FD4 File Offset: 0x001021D4
	private void AutoCorrectZonesAfterLocalChange()
	{
		if (this.HasActiveLocalChange())
		{
			return;
		}
		if (this.HasPendingLocalChange())
		{
			return;
		}
		if (this.HasActiveServerChange())
		{
			return;
		}
		if (this.HasPendingServerChange())
		{
			return;
		}
		if (this.HasPredictedSecrets())
		{
			return;
		}
		if (this.HasPredictedWeapons())
		{
			return;
		}
		if (InputManager.Get().GetBattlecrySourceCard() != null)
		{
			return;
		}
		this.AutoCorrectZones();
	}

	// Token: 0x06003403 RID: 13315 RVA: 0x00104048 File Offset: 0x00102248
	private void AutoCorrectZones()
	{
		ZoneChangeList zoneChangeList = null;
		List<Zone> list = this.FindZonesOfType<Zone>(Player.Side.FRIENDLY);
		foreach (Zone zone in list)
		{
			foreach (Card card in zone.GetCards())
			{
				Entity entity = card.GetEntity();
				TAG_ZONE zone2 = entity.GetZone();
				int controllerId = entity.GetControllerId();
				int zonePosition = entity.GetZonePosition();
				TAG_ZONE serverTag = zone.m_ServerTag;
				int controllerId2 = zone.GetControllerId();
				int zonePosition2 = card.GetZonePosition();
				bool flag = zone2 == serverTag;
				bool flag2 = controllerId == controllerId2;
				bool flag3 = zonePosition == 0 || zonePosition == zonePosition2;
				if (!flag || !flag2 || !flag3)
				{
					if (zoneChangeList == null)
					{
						int nextLocalChangeListId = this.GetNextLocalChangeListId();
						zoneChangeList = new ZoneChangeList();
						zoneChangeList.SetId(nextLocalChangeListId);
					}
					ZoneChange zoneChange = new ZoneChange();
					zoneChange.SetEntity(entity);
					zoneChange.SetDestinationZoneTag(zone2);
					zoneChange.SetDestinationZone(this.FindZoneForEntity(entity));
					zoneChange.SetDestinationControllerId(controllerId);
					zoneChange.SetDestinationPosition(zonePosition);
					zoneChangeList.AddChange(zoneChange);
				}
			}
		}
		if (zoneChangeList == null)
		{
			return;
		}
		this.ProcessLocalChangeList(zoneChangeList);
	}

	// Token: 0x04002010 RID: 8208
	private Map<Type, string> m_tweenNames = new Map<Type, string>
	{
		{
			typeof(ZoneHand),
			"ZoneHandUpdateLayout"
		},
		{
			typeof(ZonePlay),
			"ZonePlayUpdateLayout"
		},
		{
			typeof(ZoneWeapon),
			"ZoneWeaponUpdateLayout"
		}
	};

	// Token: 0x04002011 RID: 8209
	private static ZoneMgr s_instance;

	// Token: 0x04002012 RID: 8210
	private List<Zone> m_zones = new List<Zone>();

	// Token: 0x04002013 RID: 8211
	private int m_nextLocalChangeListId = 1;

	// Token: 0x04002014 RID: 8212
	private int m_nextServerChangeListId = 1;

	// Token: 0x04002015 RID: 8213
	private Queue<ZoneChangeList> m_pendingServerChangeLists = new Queue<ZoneChangeList>();

	// Token: 0x04002016 RID: 8214
	private ZoneChangeList m_activeServerChangeList;

	// Token: 0x04002017 RID: 8215
	private Map<int, Entity> m_tempEntityMap = new Map<int, Entity>();

	// Token: 0x04002018 RID: 8216
	private Map<Zone, ZoneMgr.TempZone> m_tempZoneMap = new Map<Zone, ZoneMgr.TempZone>();

	// Token: 0x04002019 RID: 8217
	private List<ZoneChangeList> m_activeLocalChangeLists = new List<ZoneChangeList>();

	// Token: 0x0400201A RID: 8218
	private List<ZoneChangeList> m_pendingLocalChangeLists = new List<ZoneChangeList>();

	// Token: 0x0400201B RID: 8219
	private QueueList<ZoneChangeList> m_localChangeListHistory = new QueueList<ZoneChangeList>();

	// Token: 0x0400201C RID: 8220
	private float m_nextDeathBlockLayoutDelaySec;

	// Token: 0x02000858 RID: 2136
	// (Invoke) Token: 0x06005243 RID: 21059
	public delegate void ChangeCompleteCallback(ZoneChangeList changeList, object userData);

	// Token: 0x020008D3 RID: 2259
	private class TempZone
	{
		// Token: 0x06005528 RID: 21800 RVA: 0x00197CD0 File Offset: 0x00195ED0
		public Zone GetZone()
		{
			return this.m_zone;
		}

		// Token: 0x06005529 RID: 21801 RVA: 0x00197CD8 File Offset: 0x00195ED8
		public void SetZone(Zone zone)
		{
			this.m_zone = zone;
		}

		// Token: 0x0600552A RID: 21802 RVA: 0x00197CE1 File Offset: 0x00195EE1
		public bool IsModified()
		{
			return this.m_modified;
		}

		// Token: 0x0600552B RID: 21803 RVA: 0x00197CE9 File Offset: 0x00195EE9
		public int GetEntityCount()
		{
			return this.m_entities.Count;
		}

		// Token: 0x0600552C RID: 21804 RVA: 0x00197CF6 File Offset: 0x00195EF6
		public List<Entity> GetEntities()
		{
			return this.m_entities;
		}

		// Token: 0x0600552D RID: 21805 RVA: 0x00197D00 File Offset: 0x00195F00
		public Entity GetEntityAtIndex(int index)
		{
			if (index < 0)
			{
				return null;
			}
			if (index >= this.m_entities.Count)
			{
				return null;
			}
			return this.m_entities[index];
		}

		// Token: 0x0600552E RID: 21806 RVA: 0x00197D35 File Offset: 0x00195F35
		public Entity GetEntityAtPos(int pos)
		{
			return this.GetEntityAtIndex(pos - 1);
		}

		// Token: 0x0600552F RID: 21807 RVA: 0x00197D40 File Offset: 0x00195F40
		public void ClearEntities()
		{
			this.m_entities.Clear();
		}

		// Token: 0x06005530 RID: 21808 RVA: 0x00197D4D File Offset: 0x00195F4D
		public void AddInitialEntity(Entity entity)
		{
			this.m_entities.Add(entity);
		}

		// Token: 0x06005531 RID: 21809 RVA: 0x00197D5C File Offset: 0x00195F5C
		public bool CanAcceptEntity(Entity entity)
		{
			Zone zone = ZoneMgr.Get().FindZoneForEntityAndZoneTag(entity, this.m_zone.m_ServerTag);
			return zone == this.m_zone;
		}

		// Token: 0x06005532 RID: 21810 RVA: 0x00197D8C File Offset: 0x00195F8C
		public void AddEntity(Entity entity)
		{
			if (!this.CanAcceptEntity(entity))
			{
				return;
			}
			if (this.m_entities.Contains(entity))
			{
				return;
			}
			this.m_entities.Add(entity);
			this.m_modified = true;
		}

		// Token: 0x06005533 RID: 21811 RVA: 0x00197DC0 File Offset: 0x00195FC0
		public void InsertEntityAtIndex(int index, Entity entity)
		{
			if (!this.CanAcceptEntity(entity))
			{
				return;
			}
			if (index < 0)
			{
				return;
			}
			if (index > this.m_entities.Count)
			{
				return;
			}
			if (index < this.m_entities.Count && this.m_entities[index] == entity)
			{
				return;
			}
			this.m_entities.Insert(index, entity);
			this.m_modified = true;
		}

		// Token: 0x06005534 RID: 21812 RVA: 0x00197E2C File Offset: 0x0019602C
		public void InsertEntityAtPos(int pos, Entity entity)
		{
			int index = pos - 1;
			this.InsertEntityAtIndex(index, entity);
		}

		// Token: 0x06005535 RID: 21813 RVA: 0x00197E45 File Offset: 0x00196045
		public bool RemoveEntity(Entity entity)
		{
			if (!this.m_entities.Remove(entity))
			{
				return false;
			}
			this.m_modified = true;
			return true;
		}

		// Token: 0x06005536 RID: 21814 RVA: 0x00197E62 File Offset: 0x00196062
		public int GetLastPos()
		{
			return this.m_entities.Count + 1;
		}

		// Token: 0x06005537 RID: 21815 RVA: 0x00197E74 File Offset: 0x00196074
		public int FindEntityPos(Entity entity)
		{
			return 1 + this.m_entities.FindIndex((Entity currEntity) => currEntity == entity);
		}

		// Token: 0x06005538 RID: 21816 RVA: 0x00197EAC File Offset: 0x001960AC
		public bool ContainsEntity(Entity entity)
		{
			int num = this.FindEntityPos(entity);
			return num > 0;
		}

		// Token: 0x06005539 RID: 21817 RVA: 0x00197EC8 File Offset: 0x001960C8
		public int FindEntityPos(int entityId)
		{
			return 1 + this.m_entities.FindIndex((Entity currEntity) => currEntity.GetEntityId() == entityId);
		}

		// Token: 0x0600553A RID: 21818 RVA: 0x00197F00 File Offset: 0x00196100
		public bool ContainsEntity(int entityId)
		{
			int num = this.FindEntityPos(entityId);
			return num > 0;
		}

		// Token: 0x0600553B RID: 21819 RVA: 0x00197F1C File Offset: 0x0019611C
		public int FindEntityPosWithReplacements(int entityId)
		{
			while (entityId != 0)
			{
				int num = 1 + this.m_entities.FindIndex((Entity currEntity) => currEntity.GetEntityId() == entityId);
				if (num > 0)
				{
					return num;
				}
				this.m_replacedEntities.TryGetValue(entityId, out entityId);
			}
			return 0;
		}

		// Token: 0x0600553C RID: 21820 RVA: 0x00197F84 File Offset: 0x00196184
		public void Sort()
		{
			if (this.m_modified)
			{
				this.m_entities.Sort(new Comparison<Entity>(this.SortComparison));
			}
			else
			{
				Entity[] array = this.m_entities.ToArray();
				this.m_entities.Sort(new Comparison<Entity>(this.SortComparison));
				for (int i = 0; i < this.m_entities.Count; i++)
				{
					if (array[i] != this.m_entities[i])
					{
						this.m_modified = true;
						break;
					}
				}
			}
		}

		// Token: 0x0600553D RID: 21821 RVA: 0x00198018 File Offset: 0x00196218
		public void PreprocessChanges()
		{
			this.m_prevEntities.Clear();
			for (int i = 0; i < this.m_entities.Count; i++)
			{
				this.m_prevEntities.Add(this.m_entities[i]);
			}
		}

		// Token: 0x0600553E RID: 21822 RVA: 0x00198064 File Offset: 0x00196264
		public void PostprocessChanges()
		{
			for (int i = 0; i < this.m_prevEntities.Count; i++)
			{
				if (i >= this.m_entities.Count)
				{
					break;
				}
				Entity prevEntity = this.m_prevEntities[i];
				int num = this.m_entities.FindIndex((Entity currEntity) => currEntity == prevEntity);
				if (num < 0)
				{
					Entity entity = this.m_entities[i];
					if (!this.m_prevEntities.Contains(entity))
					{
						this.m_replacedEntities[prevEntity.GetEntityId()] = entity.GetEntityId();
					}
				}
			}
		}

		// Token: 0x0600553F RID: 21823 RVA: 0x00198120 File Offset: 0x00196320
		public override string ToString()
		{
			return string.Format("{0} ({1} entities)", this.m_zone, this.m_entities.Count);
		}

		// Token: 0x06005540 RID: 21824 RVA: 0x00198150 File Offset: 0x00196350
		private int SortComparison(Entity entity1, Entity entity2)
		{
			int zonePosition = entity1.GetZonePosition();
			int zonePosition2 = entity2.GetZonePosition();
			return zonePosition - zonePosition2;
		}

		// Token: 0x04003B47 RID: 15175
		private Zone m_zone;

		// Token: 0x04003B48 RID: 15176
		private bool m_modified;

		// Token: 0x04003B49 RID: 15177
		private List<Entity> m_prevEntities = new List<Entity>();

		// Token: 0x04003B4A RID: 15178
		private List<Entity> m_entities = new List<Entity>();

		// Token: 0x04003B4B RID: 15179
		private Map<int, int> m_replacedEntities = new Map<int, int>();
	}
}
