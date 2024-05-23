using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E3B RID: 3643
public class SpawnToHandSpell : SuperSpell
{
	// Token: 0x06006EEF RID: 28399 RVA: 0x00208953 File Offset: 0x00206B53
	public override void RemoveAllTargets()
	{
		base.RemoveAllTargets();
		if (this.m_targetToOriginMap != null)
		{
			this.m_targetToOriginMap.Clear();
		}
	}

	// Token: 0x06006EF0 RID: 28400 RVA: 0x00208974 File Offset: 0x00206B74
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.FULL_ENTITY)
		{
			return null;
		}
		Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
		Network.Entity entity = histFullEntity.Entity;
		Entity entity2 = GameState.Get().GetEntity(entity.ID);
		if (entity2 == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, entity.ID);
			Debug.LogWarning(text);
			return null;
		}
		if (entity2.GetZone() != TAG_ZONE.HAND)
		{
			return null;
		}
		return entity2.GetCard();
	}

	// Token: 0x06006EF1 RID: 28401 RVA: 0x002089F0 File Offset: 0x00206BF0
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		base.StartCoroutine(this.DoEffectWithTiming());
	}

	// Token: 0x06006EF2 RID: 28402 RVA: 0x00208A20 File Offset: 0x00206C20
	protected virtual Vector3 GetOriginForTarget(int targetIndex = 0)
	{
		if (this.m_targetToOriginMap == null)
		{
			return base.transform.position;
		}
		Card card;
		if (!this.m_targetToOriginMap.TryGetValue(targetIndex, out card))
		{
			return base.transform.position;
		}
		return card.transform.position;
	}

	// Token: 0x06006EF3 RID: 28403 RVA: 0x00208A6E File Offset: 0x00206C6E
	protected void AddOriginForTarget(int targetIndex, Card card)
	{
		if (this.m_targetToOriginMap == null)
		{
			this.m_targetToOriginMap = new Map<int, Card>();
		}
		this.m_targetToOriginMap[targetIndex] = card;
	}

	// Token: 0x06006EF4 RID: 28404 RVA: 0x00208A93 File Offset: 0x00206C93
	protected bool AddUniqueOriginForTarget(int targetIndex, Card card)
	{
		if (this.m_targetToOriginMap != null && this.m_targetToOriginMap.ContainsValue(card))
		{
			return false;
		}
		this.AddOriginForTarget(targetIndex, card);
		return true;
	}

	// Token: 0x06006EF5 RID: 28405 RVA: 0x00208ABC File Offset: 0x00206CBC
	protected virtual IEnumerator DoEffectWithTiming()
	{
		GameObject sourceObject = base.GetSource();
		Card sourceCard = sourceObject.GetComponent<Card>();
		Actor sourceActor = sourceCard.GetActor();
		if (sourceActor && this.m_Shake)
		{
			GameObject sourceActorObject = sourceActor.gameObject;
			MinionShake.ShakeObject(sourceActorObject, ShakeMinionType.RandomDirection, sourceActorObject.transform.position, this.m_ShakeIntensity, 0.1f, 0f, this.m_ShakeDelay, true);
		}
		yield return new WaitForSeconds(this.m_CardDelay);
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			GameObject targetObject = this.m_targets[i];
			Card targetCard = targetObject.GetComponent<Card>();
			targetCard.transform.position = this.GetOriginForTarget(i);
			if (this.m_SpellPrefab != null)
			{
				Spell startSpell = base.CloneSpell(this.m_SpellPrefab);
				startSpell.SetSource(sourceObject);
				startSpell.AddTarget(targetObject);
				startSpell.SetPosition(targetCard.transform.position);
				startSpell.Activate();
			}
			targetCard.transform.localScale = new Vector3(this.m_CardStartScale, this.m_CardStartScale, this.m_CardStartScale);
			targetCard.SetTransitionStyle(ZoneTransitionStyle.VERY_SLOW);
			targetCard.SetDoNotWarpToNewZone(true);
		}
		this.AddTransitionDelays();
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x06006EF6 RID: 28406 RVA: 0x00208AD8 File Offset: 0x00206CD8
	protected string GetCardIdForTarget(int targetIndex)
	{
		GameObject gameObject = this.m_targets[targetIndex];
		Card component = gameObject.GetComponent<Card>();
		Entity entity = component.GetEntity();
		return entity.GetCardId();
	}

	// Token: 0x06006EF7 RID: 28407 RVA: 0x00208B08 File Offset: 0x00206D08
	private void AddTransitionDelays()
	{
		if (this.m_CardStaggerMin <= 0f && this.m_CardStaggerMax <= 0f)
		{
			return;
		}
		if (this.m_AccumulateStagger)
		{
			float num = 0f;
			for (int i = 0; i < this.m_targets.Count; i++)
			{
				GameObject gameObject = this.m_targets[i];
				Card component = gameObject.GetComponent<Card>();
				float num2 = Random.Range(this.m_CardStaggerMin, this.m_CardStaggerMax);
				num += num2;
				component.SetTransitionDelay(num);
			}
		}
		else
		{
			for (int j = 0; j < this.m_targets.Count; j++)
			{
				GameObject gameObject2 = this.m_targets[j];
				Card component2 = gameObject2.GetComponent<Card>();
				float transitionDelay = Random.Range(this.m_CardStaggerMin, this.m_CardStaggerMax);
				component2.SetTransitionDelay(transitionDelay);
			}
		}
	}

	// Token: 0x040057E5 RID: 22501
	public float m_CardStartScale = 0.1f;

	// Token: 0x040057E6 RID: 22502
	public float m_CardDelay = 1f;

	// Token: 0x040057E7 RID: 22503
	public float m_CardStaggerMin;

	// Token: 0x040057E8 RID: 22504
	public float m_CardStaggerMax;

	// Token: 0x040057E9 RID: 22505
	public bool m_AccumulateStagger = true;

	// Token: 0x040057EA RID: 22506
	public bool m_Shake = true;

	// Token: 0x040057EB RID: 22507
	public float m_ShakeDelay;

	// Token: 0x040057EC RID: 22508
	public ShakeMinionIntensity m_ShakeIntensity = ShakeMinionIntensity.MediumShake;

	// Token: 0x040057ED RID: 22509
	public Spell m_SpellPrefab;

	// Token: 0x040057EE RID: 22510
	protected Map<int, Card> m_targetToOriginMap;
}
