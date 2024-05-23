using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C6 RID: 2246
public class SecretSpellController : SpellController
{
	// Token: 0x060054A1 RID: 21665 RVA: 0x00194F0C File Offset: 0x0019310C
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		bool flag = false;
		if (taskList.IsStartOfBlock() && this.InitBannerSpell(sourceEntity))
		{
			flag = true;
		}
		Spell triggerSpell = this.GetTriggerSpell(card);
		if (triggerSpell != null && this.InitTriggerSpell(card, triggerSpell))
		{
			flag = true;
		}
		if (!flag)
		{
			return false;
		}
		base.SetSource(card);
		return true;
	}

	// Token: 0x060054A2 RID: 21666 RVA: 0x00194F84 File Offset: 0x00193184
	protected override void OnProcessTaskList()
	{
		Card source = base.GetSource();
		source.SetSecretTriggered(true);
		if (this.m_taskList.IsStartOfBlock())
		{
			this.FireSecretActorSpell();
			if (this.FireBannerSpell())
			{
				return;
			}
		}
		if (this.FireTriggerSpell())
		{
			return;
		}
		base.OnProcessTaskList();
	}

	// Token: 0x060054A3 RID: 21667 RVA: 0x00194FD4 File Offset: 0x001931D4
	private bool FireSecretActorSpell()
	{
		Card source = base.GetSource();
		if (!source.CanShowSecretTrigger())
		{
			return false;
		}
		source.ShowSecretTrigger();
		return true;
	}

	// Token: 0x060054A4 RID: 21668 RVA: 0x00194FFC File Offset: 0x001931FC
	private Spell GetTriggerSpell(Card card)
	{
		Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
		return card.GetTriggerSpell(blockStart.EffectIndex, true);
	}

	// Token: 0x060054A5 RID: 21669 RVA: 0x00195024 File Offset: 0x00193224
	private bool InitTriggerSpell(Card card, Spell triggerSpell)
	{
		if (!triggerSpell.AttachPowerTaskList(this.m_taskList))
		{
			Network.HistBlockStart blockStart = this.m_taskList.GetBlockStart();
			Log.Power.Print(string.Format("{0}.InitTriggerSpell() - FAILED to attach task list to trigger spell {1} for {2}", this, blockStart.EffectIndex, card), new object[0]);
			return false;
		}
		return true;
	}

	// Token: 0x060054A6 RID: 21670 RVA: 0x00195078 File Offset: 0x00193278
	private bool FireTriggerSpell()
	{
		Card source = base.GetSource();
		Spell triggerSpell = this.GetTriggerSpell(source);
		if (triggerSpell == null)
		{
			return false;
		}
		if (triggerSpell.GetPowerTaskList() != this.m_taskList && !this.InitTriggerSpell(source, triggerSpell))
		{
			return false;
		}
		triggerSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnTriggerSpellFinished));
		triggerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnTriggerSpellStateFinished));
		triggerSpell.SafeActivateState(SpellStateType.ACTION);
		return true;
	}

	// Token: 0x060054A7 RID: 21671 RVA: 0x001950EE File Offset: 0x001932EE
	private void OnTriggerSpellFinished(Spell triggerSpell, object userData)
	{
		this.OnFinishedTaskList();
	}

	// Token: 0x060054A8 RID: 21672 RVA: 0x001950F6 File Offset: 0x001932F6
	private void OnTriggerSpellStateFinished(Spell triggerSpell, SpellStateType prevStateType, object userData)
	{
		if (triggerSpell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		this.OnFinished();
	}

	// Token: 0x060054A9 RID: 21673 RVA: 0x0019510C File Offset: 0x0019330C
	private Spell DetermineBannerSpellPrefab(Entity sourceEntity)
	{
		if (this.m_BannerDefs == null)
		{
			return null;
		}
		TAG_CLASS @class = sourceEntity.GetClass();
		SpellClassTag spellClassTag = SpellUtils.ConvertClassTagToSpellEnum(@class);
		if (spellClassTag == SpellClassTag.NONE)
		{
			Debug.LogWarning(string.Format("{0}.DetermineBannerSpellPrefab() - entity {1} has unknown class tag {2}. Using default banner.", this, sourceEntity, @class));
		}
		else if (this.m_BannerDefs != null && this.m_BannerDefs.Count > 0)
		{
			for (int i = 0; i < this.m_BannerDefs.Count; i++)
			{
				SecretBannerDef secretBannerDef = this.m_BannerDefs[i];
				if (spellClassTag == secretBannerDef.m_HeroClass)
				{
					return secretBannerDef.m_SpellPrefab;
				}
			}
			Log.Asset.Print(string.Format("{0}.DetermineBannerSpellPrefab() - class type {1} has no Banner Def. Using default banner.", this, spellClassTag), new object[0]);
		}
		return this.m_DefaultBannerSpellPrefab;
	}

	// Token: 0x060054AA RID: 21674 RVA: 0x001951D8 File Offset: 0x001933D8
	private bool InitBannerSpell(Entity sourceEntity)
	{
		Spell spell = this.DetermineBannerSpellPrefab(sourceEntity);
		if (spell == null)
		{
			return false;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(spell.gameObject);
		this.m_bannerSpell = gameObject.GetComponent<Spell>();
		return true;
	}

	// Token: 0x060054AB RID: 21675 RVA: 0x00195214 File Offset: 0x00193414
	private bool FireBannerSpell()
	{
		if (this.m_bannerSpell == null)
		{
			return false;
		}
		base.StartCoroutine(this.ContinueWithSecretEvents());
		this.m_bannerSpell.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnBannerSpellStateFinished));
		this.m_bannerSpell.Activate();
		return true;
	}

	// Token: 0x060054AC RID: 21676 RVA: 0x00195264 File Offset: 0x00193464
	private IEnumerator ContinueWithSecretEvents()
	{
		yield return new WaitForSeconds(1f);
		while (!HistoryManager.Get().HasBigCard())
		{
			yield return null;
		}
		HistoryManager.Get().NotifyOfSecretSpellFinished();
		yield return new WaitForSeconds(1f);
		if (this.FireTriggerSpell())
		{
			yield break;
		}
		base.OnProcessTaskList();
		yield break;
	}

	// Token: 0x060054AD RID: 21677 RVA: 0x00195280 File Offset: 0x00193480
	private void OnBannerSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (this.m_bannerSpell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(this.m_bannerSpell.gameObject);
		this.m_bannerSpell = null;
	}

	// Token: 0x04003AF2 RID: 15090
	public List<SecretBannerDef> m_BannerDefs;

	// Token: 0x04003AF3 RID: 15091
	public Spell m_DefaultBannerSpellPrefab;

	// Token: 0x04003AF4 RID: 15092
	private Spell m_bannerSpell;

	// Token: 0x04003AF5 RID: 15093
	private Spell m_triggerSpell;
}
