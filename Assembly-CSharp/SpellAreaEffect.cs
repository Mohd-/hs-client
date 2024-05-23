using System;
using UnityEngine;

// Token: 0x02000ED0 RID: 3792
public class SpellAreaEffect : Spell
{
	// Token: 0x060071C4 RID: 29124 RVA: 0x00217590 File Offset: 0x00215790
	public override bool AddPowerTargets()
	{
		return base.CanAddPowerTargets() && base.AddMultiplePowerTargets() && base.GetTargets().Count > 0;
	}

	// Token: 0x060071C5 RID: 29125 RVA: 0x002175CC File Offset: 0x002157CC
	protected override void OnDeath(SpellStateType prevStateType)
	{
		base.OnDeath(prevStateType);
		if (this.m_ImpactSpellPrefab == null)
		{
			return;
		}
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			this.SpawnImpactSpell(this.m_targets[i]);
		}
	}

	// Token: 0x060071C6 RID: 29126 RVA: 0x00217620 File Offset: 0x00215820
	private void SpawnImpactSpell(GameObject targetObject)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(this.m_ImpactSpellPrefab.gameObject, targetObject.transform.position, Quaternion.identity);
		Spell component = gameObject.GetComponent<Spell>();
		component.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnImpactSpellStateFinished));
		component.Activate();
	}

	// Token: 0x060071C7 RID: 29127 RVA: 0x00217672 File Offset: 0x00215872
	private void OnImpactSpellStateFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x04005BFF RID: 23551
	public Spell m_ImpactSpellPrefab;
}
