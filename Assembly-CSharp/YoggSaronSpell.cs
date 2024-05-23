using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000EA9 RID: 3753
public class YoggSaronSpell : Spell
{
	// Token: 0x06007121 RID: 28961 RVA: 0x00215A89 File Offset: 0x00213C89
	public override bool CanPurge()
	{
		return this.m_mistSpellInstance == null;
	}

	// Token: 0x06007122 RID: 28962 RVA: 0x00215A97 File Offset: 0x00213C97
	public override bool AddPowerTargets()
	{
		return !this.m_mistSpellInstance || this.m_taskList.IsEndOfBlock();
	}

	// Token: 0x06007123 RID: 28963 RVA: 0x00215ABC File Offset: 0x00213CBC
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		base.StartCoroutine(this.DoEffectsWithTiming());
	}

	// Token: 0x06007124 RID: 28964 RVA: 0x00215AD4 File Offset: 0x00213CD4
	private IEnumerator DoEffectsWithTiming()
	{
		if (!this.m_mistSpellInstance)
		{
			this.m_mistSpellInstance = Object.Instantiate<Spell>(this.m_MistSpellPrefab);
			if (this.m_mistSpellInstance)
			{
				this.m_mistSpellInstance.ActivateState(SpellStateType.BIRTH);
				while (this.m_mistSpellInstance.GetActiveState() != SpellStateType.IDLE)
				{
					yield return null;
				}
			}
		}
		if (this.m_mistSpellInstance && this.m_taskList.IsEndOfBlock())
		{
			this.m_mistSpellInstance.ActivateState(SpellStateType.DEATH);
			while (!this.m_mistSpellInstance.IsFinished())
			{
				yield return null;
			}
			this.OnSpellFinished();
			while (this.m_mistSpellInstance.GetActiveState() != SpellStateType.NONE)
			{
				yield return null;
			}
			Object.Destroy(this.m_mistSpellInstance.gameObject);
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x04005AC0 RID: 23232
	public Spell m_MistSpellPrefab;

	// Token: 0x04005AC1 RID: 23233
	private Spell m_mistSpellInstance;
}
