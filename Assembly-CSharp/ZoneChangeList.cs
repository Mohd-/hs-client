using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000850 RID: 2128
public class ZoneChangeList
{
	// Token: 0x060051D3 RID: 20947 RVA: 0x00187574 File Offset: 0x00185774
	public int GetId()
	{
		return this.m_id;
	}

	// Token: 0x060051D4 RID: 20948 RVA: 0x0018757C File Offset: 0x0018577C
	public void SetId(int id)
	{
		this.m_id = id;
	}

	// Token: 0x060051D5 RID: 20949 RVA: 0x00187585 File Offset: 0x00185785
	public bool IsLocal()
	{
		return this.m_taskList == null;
	}

	// Token: 0x060051D6 RID: 20950 RVA: 0x00187590 File Offset: 0x00185790
	public int GetPredictedPosition()
	{
		return this.m_predictedPosition;
	}

	// Token: 0x060051D7 RID: 20951 RVA: 0x00187598 File Offset: 0x00185798
	public void SetPredictedPosition(int pos)
	{
		this.m_predictedPosition = pos;
	}

	// Token: 0x060051D8 RID: 20952 RVA: 0x001875A1 File Offset: 0x001857A1
	public bool DoesIgnoreCardZoneChanges()
	{
		return this.m_ignoreCardZoneChanges;
	}

	// Token: 0x060051D9 RID: 20953 RVA: 0x001875A9 File Offset: 0x001857A9
	public void SetIgnoreCardZoneChanges(bool ignore)
	{
		this.m_ignoreCardZoneChanges = ignore;
	}

	// Token: 0x060051DA RID: 20954 RVA: 0x001875B2 File Offset: 0x001857B2
	public bool IsCanceledChangeList()
	{
		return this.m_canceledChangeList;
	}

	// Token: 0x060051DB RID: 20955 RVA: 0x001875BA File Offset: 0x001857BA
	public void SetCanceledChangeList(bool canceledChangeList)
	{
		this.m_canceledChangeList = canceledChangeList;
	}

	// Token: 0x060051DC RID: 20956 RVA: 0x001875C4 File Offset: 0x001857C4
	public void SetZoneInputBlocking(bool block)
	{
		for (int i = 0; i < this.m_changes.Count; i++)
		{
			ZoneChange zoneChange = this.m_changes[i];
			Zone sourceZone = zoneChange.GetSourceZone();
			if (sourceZone != null)
			{
				sourceZone.BlockInput(block);
			}
			Zone destinationZone = zoneChange.GetDestinationZone();
			if (destinationZone != null)
			{
				destinationZone.BlockInput(block);
			}
		}
	}

	// Token: 0x060051DD RID: 20957 RVA: 0x0018762E File Offset: 0x0018582E
	public bool IsComplete()
	{
		return this.m_complete;
	}

	// Token: 0x060051DE RID: 20958 RVA: 0x00187636 File Offset: 0x00185836
	public ZoneMgr.ChangeCompleteCallback GetCompleteCallback()
	{
		return this.m_completeCallback;
	}

	// Token: 0x060051DF RID: 20959 RVA: 0x0018763E File Offset: 0x0018583E
	public void SetCompleteCallback(ZoneMgr.ChangeCompleteCallback callback)
	{
		this.m_completeCallback = callback;
	}

	// Token: 0x060051E0 RID: 20960 RVA: 0x00187647 File Offset: 0x00185847
	public object GetCompleteCallbackUserData()
	{
		return this.m_completeCallbackUserData;
	}

	// Token: 0x060051E1 RID: 20961 RVA: 0x0018764F File Offset: 0x0018584F
	public void SetCompleteCallbackUserData(object userData)
	{
		this.m_completeCallbackUserData = userData;
	}

	// Token: 0x060051E2 RID: 20962 RVA: 0x00187658 File Offset: 0x00185858
	public void FireCompleteCallback()
	{
		this.DebugPrint("ZoneChangeList.FireCompleteCallback() - m_id={0} m_taskList={1} m_changes.Count={2} m_complete={3} m_completeCallback={4}", new object[]
		{
			this.m_id,
			(this.m_taskList != null) ? this.m_taskList.GetId().ToString() : "(null)",
			this.m_changes.Count,
			this.m_complete,
			(this.m_completeCallback != null) ? "(not null)" : "(null)"
		});
		if (this.m_completeCallback == null)
		{
			return;
		}
		this.m_completeCallback(this, this.m_completeCallbackUserData);
	}

	// Token: 0x060051E3 RID: 20963 RVA: 0x0018770B File Offset: 0x0018590B
	public PowerTaskList GetTaskList()
	{
		return this.m_taskList;
	}

	// Token: 0x060051E4 RID: 20964 RVA: 0x00187713 File Offset: 0x00185913
	public void SetTaskList(PowerTaskList taskList)
	{
		this.m_taskList = taskList;
	}

	// Token: 0x060051E5 RID: 20965 RVA: 0x0018771C File Offset: 0x0018591C
	public List<ZoneChange> GetChanges()
	{
		return this.m_changes;
	}

	// Token: 0x060051E6 RID: 20966 RVA: 0x00187724 File Offset: 0x00185924
	public ZoneChange GetChange(int index)
	{
		return this.m_changes[index];
	}

	// Token: 0x060051E7 RID: 20967 RVA: 0x00187734 File Offset: 0x00185934
	public ZoneChange GetLocalTriggerChange()
	{
		if (!this.IsLocal())
		{
			return null;
		}
		return (this.m_changes.Count <= 0) ? null : this.m_changes[0];
	}

	// Token: 0x060051E8 RID: 20968 RVA: 0x00187774 File Offset: 0x00185974
	public Card GetLocalTriggerCard()
	{
		ZoneChange localTriggerChange = this.GetLocalTriggerChange();
		if (localTriggerChange == null)
		{
			return null;
		}
		Entity entity = localTriggerChange.GetEntity();
		return entity.GetCard();
	}

	// Token: 0x060051E9 RID: 20969 RVA: 0x0018779F File Offset: 0x0018599F
	public void AddChange(ZoneChange change)
	{
		this.m_changes.Add(change);
	}

	// Token: 0x060051EA RID: 20970 RVA: 0x001877AD File Offset: 0x001859AD
	public void InsertChange(int index, ZoneChange change)
	{
		this.m_changes.Insert(index, change);
	}

	// Token: 0x060051EB RID: 20971 RVA: 0x001877BC File Offset: 0x001859BC
	public void ClearChanges()
	{
		this.m_changes.Clear();
	}

	// Token: 0x060051EC RID: 20972 RVA: 0x001877CC File Offset: 0x001859CC
	public IEnumerator ProcessChanges()
	{
		this.DebugPrint("ZoneChangeList.ProcessChanges() - m_id={0} m_taskList={1} m_changes.Count={2}", new object[]
		{
			this.m_id,
			(this.m_taskList != null) ? this.m_taskList.GetId().ToString() : "(null)",
			this.m_changes.Count
		});
		while (this.MustWaitForChoices())
		{
			yield return null;
		}
		HashSet<Entity> loadingEntities = new HashSet<Entity>();
		Map<Player, DyingSecretGroup> dyingSecretMap = null;
		int i = 0;
		while (i < this.m_changes.Count)
		{
			ZoneChange change = this.m_changes[i];
			this.DebugPrint("ZoneChangeList.ProcessChanges() - processing index={0} change={1}", new object[]
			{
				i,
				change
			});
			Entity entity = change.GetEntity();
			Card card = entity.GetCard();
			PowerTask powerTask = change.GetPowerTask();
			int srcControllerId = entity.GetControllerId();
			int srcPos = 0;
			Zone srcZone = null;
			if (card != null)
			{
				srcPos = card.GetZonePosition();
				srcZone = card.GetZone();
			}
			int dstControllerId = change.GetDestinationControllerId();
			int dstPos = change.GetDestinationPosition();
			Zone dstZone = change.GetDestinationZone();
			TAG_ZONE dstZoneTag = change.GetDestinationZoneTag();
			if (powerTask == null)
			{
				goto IL_2D5;
			}
			if (!powerTask.IsCompleted())
			{
				if (loadingEntities.Contains(entity))
				{
					this.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to load (powerTask=(not null))", new object[]
					{
						card
					});
					yield return ZoneMgr.Get().StartCoroutine(this.WaitForAndRemoveLoadingEntity(loadingEntities, entity, card));
					this.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to load (powerTask=(not null))", new object[]
					{
						card
					});
				}
				powerTask.DoTask();
				if (entity.IsLoadingAssets())
				{
					loadingEntities.Add(entity);
					goto IL_2D5;
				}
				goto IL_2D5;
			}
			IL_91F:
			i++;
			continue;
			IL_2D5:
			if (card == null)
			{
				goto IL_91F;
			}
			if (this.m_ignoreCardZoneChanges)
			{
				goto IL_91F;
			}
			bool zoneChanged = dstZoneTag != TAG_ZONE.INVALID && srcZone != dstZone;
			bool controllerChanged = dstControllerId != 0 && srcControllerId != dstControllerId;
			bool posChanged = dstPos != 0 && srcPos != dstPos;
			bool revealed = powerTask != null && powerTask.GetPower().Type == Network.PowerType.SHOW_ENTITY;
			bool forceUpdateActor = this.ShouldForceUpdateActor(change);
			if (UniversalInputManager.UsePhoneUI && this.IsDisplayableDyingSecret(entity, card, srcZone, dstZone))
			{
				if (dyingSecretMap == null)
				{
					dyingSecretMap = new Map<Player, DyingSecretGroup>();
				}
				Player controller = card.GetController();
				DyingSecretGroup dyingSecretGroup;
				if (!dyingSecretMap.TryGetValue(controller, out dyingSecretGroup))
				{
					dyingSecretGroup = new DyingSecretGroup();
					dyingSecretMap.Add(controller, dyingSecretGroup);
				}
				dyingSecretGroup.AddCard(card);
			}
			if (zoneChanged || controllerChanged || revealed || forceUpdateActor)
			{
				bool transitionedZones = zoneChanged || controllerChanged;
				bool revealedSecret = revealed && entity.GetZone() == TAG_ZONE.SECRET;
				if (transitionedZones || !revealedSecret)
				{
					if (srcZone != null)
					{
						this.m_dirtyZones.Add(srcZone);
					}
					if (dstZone != null)
					{
						this.m_dirtyZones.Add(dstZone);
					}
					this.DebugPrint("ZoneChangeList.ProcessChanges() - TRANSITIONING card {0} to {1}", new object[]
					{
						card,
						dstZone
					});
				}
				if (loadingEntities.Contains(entity))
				{
					this.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to load (zoneChanged={1} controllerChanged={2} powerTask=(not null))", new object[]
					{
						card,
						zoneChanged,
						controllerChanged
					});
					yield return ZoneMgr.Get().StartCoroutine(this.WaitForAndRemoveLoadingEntity(loadingEntities, entity, card));
					this.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to load (zoneChanged={1} controllerChanged={2} powerTask=(not null))", new object[]
					{
						card,
						zoneChanged,
						controllerChanged
					});
				}
				if (!card.IsActorReady() || card.IsBeingDrawnByOpponent())
				{
					this.DebugPrint("ZoneChangeList.ProcessChanges() - START waiting for {0} to become ready (zoneChanged={1} controllerChanged={2} powerTask=(not null))", new object[]
					{
						card,
						zoneChanged,
						controllerChanged
					});
					while (!card.IsActorReady() || card.IsBeingDrawnByOpponent())
					{
						yield return null;
					}
					this.DebugPrint("ZoneChangeList.ProcessChanges() - END waiting for {0} to become ready (zoneChanged={1} controllerChanged={2} powerTask=(not null))", new object[]
					{
						card,
						zoneChanged,
						controllerChanged
					});
				}
				Log.Zone.Print("ZoneChangeList.ProcessChanges() - id={0} local={1} {2} zone from {3} -> {4}", new object[]
				{
					this.m_id,
					this.IsLocal(),
					card,
					srcZone,
					dstZone
				});
				if (transitionedZones)
				{
					if (srcZone is ZonePlay && srcZone.m_Side == Player.Side.OPPOSING && dstZone is ZoneHand && dstZone.m_Side == Player.Side.OPPOSING)
					{
						Log.FaceDownCard.Print("ZoneChangeList.ProcessChanges() - id={0} {1}.TransitionToZone(): {2} -> {3}", new object[]
						{
							this.m_id,
							card,
							srcZone,
							dstZone
						});
						this.m_taskList.DebugDump(Log.FaceDownCard);
					}
					card.TransitionToZone(dstZone);
				}
				else if (revealed || forceUpdateActor)
				{
					card.UpdateActor(forceUpdateActor);
				}
				if (card.IsActorLoading())
				{
					loadingEntities.Add(entity);
				}
			}
			if (posChanged)
			{
				if (srcZone != null && !zoneChanged && !controllerChanged)
				{
					this.m_dirtyZones.Add(srcZone);
				}
				if (dstZone != null)
				{
					this.m_dirtyZones.Add(dstZone);
				}
				Log.Zone.Print("ZoneChangeList.ProcessChanges() - id={0} local={1} {2} pos from {3} -> {4}", new object[]
				{
					this.m_id,
					this.IsLocal(),
					card,
					srcPos,
					dstPos
				});
				card.SetZonePosition(dstPos);
				goto IL_91F;
			}
			goto IL_91F;
		}
		if (this.IsCanceledChangeList())
		{
			this.SetZoneInputBlocking(false);
		}
		this.ProcessDyingSecrets(dyingSecretMap);
		ZoneMgr.Get().StartCoroutine(this.UpdateDirtyZones(loadingEntities));
		yield break;
	}

	// Token: 0x060051ED RID: 20973 RVA: 0x001877E8 File Offset: 0x001859E8
	public override string ToString()
	{
		return string.Format("id={0} changes={1} complete={2} local={3} localTrigger=[{4}]", new object[]
		{
			this.m_id,
			this.m_changes.Count,
			this.m_complete,
			this.IsLocal(),
			this.GetLocalTriggerChange()
		});
	}

	// Token: 0x060051EE RID: 20974 RVA: 0x0018784B File Offset: 0x00185A4B
	private bool IsDisplayableDyingSecret(Entity entity, Card card, Zone srcZone, Zone dstZone)
	{
		return entity.IsSecret() && srcZone is ZoneSecret && dstZone is ZoneGraveyard;
	}

	// Token: 0x060051EF RID: 20975 RVA: 0x00187878 File Offset: 0x00185A78
	private void ProcessDyingSecrets(Map<Player, DyingSecretGroup> dyingSecretMap)
	{
		if (dyingSecretMap == null)
		{
			return;
		}
		Map<Player, DeadSecretGroup> map = null;
		foreach (KeyValuePair<Player, DyingSecretGroup> keyValuePair in dyingSecretMap)
		{
			Player key = keyValuePair.Key;
			DyingSecretGroup value = keyValuePair.Value;
			Card mainCard = value.GetMainCard();
			List<Card> cards = value.GetCards();
			List<Actor> actors = value.GetActors();
			for (int i = 0; i < cards.Count; i++)
			{
				Card card = cards[i];
				Actor actor = actors[i];
				if (card.WasSecretTriggered())
				{
					actor.Destroy();
				}
				else
				{
					if (card == mainCard && card.CanShowSecretDeath())
					{
						card.ShowSecretDeath(actor);
					}
					else
					{
						actor.Destroy();
					}
					if (map == null)
					{
						map = new Map<Player, DeadSecretGroup>();
					}
					DeadSecretGroup deadSecretGroup;
					if (!map.TryGetValue(key, out deadSecretGroup))
					{
						deadSecretGroup = new DeadSecretGroup();
						deadSecretGroup.SetMainCard(mainCard);
						map.Add(key, deadSecretGroup);
					}
					deadSecretGroup.AddCard(card);
				}
			}
		}
		BigCard.Get().ShowSecretDeaths(map);
	}

	// Token: 0x060051F0 RID: 20976 RVA: 0x001879BC File Offset: 0x00185BBC
	private bool MustWaitForChoices()
	{
		if (!ChoiceCardMgr.Get().HasChoices())
		{
			return false;
		}
		PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
		if (powerProcessor.HasGameOverTaskList())
		{
			return false;
		}
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (int playerId in playerMap.Keys)
		{
			PowerTaskList preChoiceTaskList = ChoiceCardMgr.Get().GetPreChoiceTaskList(playerId);
			if (preChoiceTaskList != null)
			{
				if (!powerProcessor.HasTaskList(preChoiceTaskList))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060051F1 RID: 20977 RVA: 0x00187A78 File Offset: 0x00185C78
	private IEnumerator WaitForAndRemoveLoadingEntity(HashSet<Entity> loadingEntities, Entity entity, Card card)
	{
		while (this.IsEntityLoading(entity, card))
		{
			yield return null;
		}
		loadingEntities.Remove(entity);
		yield break;
	}

	// Token: 0x060051F2 RID: 20978 RVA: 0x00187ABD File Offset: 0x00185CBD
	private bool IsEntityLoading(Entity entity, Card card)
	{
		return entity.IsLoadingAssets() || (card != null && card.IsActorLoading());
	}

	// Token: 0x060051F3 RID: 20979 RVA: 0x00187AE8 File Offset: 0x00185CE8
	private bool ShouldForceUpdateActor(ZoneChange change)
	{
		Entity entity = change.GetEntity();
		Zone sourceZone = change.GetSourceZone();
		TAG_ZONE destinationZoneTag = change.GetDestinationZoneTag();
		return entity.GetCardType() == TAG_CARDTYPE.SPELL && sourceZone == null && destinationZoneTag == TAG_ZONE.PLAY;
	}

	// Token: 0x060051F4 RID: 20980 RVA: 0x00187B2C File Offset: 0x00185D2C
	private IEnumerator UpdateDirtyZones(HashSet<Entity> loadingEntities)
	{
		this.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} loadingEntities.Count={1} m_dirtyZones.Count={2}", new object[]
		{
			this.m_id,
			loadingEntities.Count,
			this.m_dirtyZones.Count
		});
		foreach (Entity entity in loadingEntities)
		{
			Card card = entity.GetCard();
			this.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} START waiting for {1} to load (card={2})", new object[]
			{
				this.m_id,
				entity,
				card
			});
			while (this.IsEntityLoading(entity, card))
			{
				yield return null;
			}
			this.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} END waiting for {1} to load (card={2})", new object[]
			{
				this.m_id,
				entity,
				card
			});
		}
		if (this.IsDeathBlock())
		{
			float layoutDelaySec = ZoneMgr.Get().RemoveNextDeathBlockLayoutDelaySec();
			if (layoutDelaySec >= 0f)
			{
				yield return new WaitForSeconds(layoutDelaySec);
			}
			foreach (Zone dirtyZone in this.m_dirtyZones)
			{
				dirtyZone.UpdateLayout();
			}
			this.m_dirtyZones.Clear();
		}
		else
		{
			Zone[] dirtyZones = new Zone[this.m_dirtyZones.Count];
			this.m_dirtyZones.CopyTo(dirtyZones);
			foreach (Zone dirtyZone2 in dirtyZones)
			{
				this.DebugPrint("ZoneChangeList.UpdateDirtyZones() - m_id={0} START waiting for zone {1}", new object[]
				{
					this.m_id,
					dirtyZone2
				});
				if (dirtyZone2 is ZoneHand)
				{
					ZoneMgr.Get().StartCoroutine(this.ZoneHand_UpdateLayout((ZoneHand)dirtyZone2));
				}
				else
				{
					dirtyZone2.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnUpdateLayoutComplete));
					dirtyZone2.UpdateLayout();
				}
			}
		}
		ZoneMgr.Get().StartCoroutine(this.FinishWhenPossible());
		yield break;
	}

	// Token: 0x060051F5 RID: 20981 RVA: 0x00187B55 File Offset: 0x00185D55
	private bool IsDeathBlock()
	{
		return this.m_taskList != null && this.m_taskList.IsDeathBlock();
	}

	// Token: 0x060051F6 RID: 20982 RVA: 0x00187B70 File Offset: 0x00185D70
	private IEnumerator ZoneHand_UpdateLayout(ZoneHand zoneHand)
	{
		for (;;)
		{
			Card busyCard = zoneHand.GetCards().Find((Card card) => (!(TurnStartManager.Get() != null) || !TurnStartManager.Get().IsCardDrawHandled(card)) && !card.IsActorReady());
			if (busyCard == null)
			{
				break;
			}
			yield return null;
		}
		zoneHand.AddUpdateLayoutCompleteCallback(new Zone.UpdateLayoutCompleteCallback(this.OnUpdateLayoutComplete));
		zoneHand.UpdateLayout();
		yield break;
	}

	// Token: 0x060051F7 RID: 20983 RVA: 0x00187B9C File Offset: 0x00185D9C
	private void OnUpdateLayoutComplete(Zone zone, object userData)
	{
		this.DebugPrint("ZoneChangeList.OnUpdateLayoutComplete() - m_id={0} END waiting for zone {1}", new object[]
		{
			this.m_id,
			zone
		});
		this.m_dirtyZones.Remove(zone);
	}

	// Token: 0x060051F8 RID: 20984 RVA: 0x00187BDC File Offset: 0x00185DDC
	private IEnumerator FinishWhenPossible()
	{
		while (this.m_dirtyZones.Count > 0)
		{
			yield return null;
		}
		while (GameState.Get().IsBusy())
		{
			yield return null;
		}
		this.Finish();
		yield break;
	}

	// Token: 0x060051F9 RID: 20985 RVA: 0x00187BF8 File Offset: 0x00185DF8
	private void Finish()
	{
		this.m_complete = true;
		Log.Zone.Print("ZoneChangeList.Finish() - {0}", new object[]
		{
			this
		});
	}

	// Token: 0x060051FA RID: 20986 RVA: 0x00187C28 File Offset: 0x00185E28
	[Conditional("ZONE_CHANGE_DEBUG")]
	private void DebugPrint(string format, params object[] args)
	{
		string format2 = string.Format(format, args);
		Log.Zone.Print(format2, new object[0]);
	}

	// Token: 0x04003848 RID: 14408
	private int m_id;

	// Token: 0x04003849 RID: 14409
	private int m_predictedPosition;

	// Token: 0x0400384A RID: 14410
	private bool m_ignoreCardZoneChanges;

	// Token: 0x0400384B RID: 14411
	private bool m_canceledChangeList;

	// Token: 0x0400384C RID: 14412
	private PowerTaskList m_taskList;

	// Token: 0x0400384D RID: 14413
	private List<ZoneChange> m_changes = new List<ZoneChange>();

	// Token: 0x0400384E RID: 14414
	private HashSet<Zone> m_dirtyZones = new HashSet<Zone>();

	// Token: 0x0400384F RID: 14415
	private bool m_complete;

	// Token: 0x04003850 RID: 14416
	private ZoneMgr.ChangeCompleteCallback m_completeCallback;

	// Token: 0x04003851 RID: 14417
	private object m_completeCallbackUserData;
}
