using System;
using UnityEngine;

// Token: 0x02000E20 RID: 3616
public class OverrideCustomSpawnSpell : SuperSpell
{
	// Token: 0x06006E99 RID: 28313 RVA: 0x00206E5C File Offset: 0x0020505C
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		foreach (GameObject gameObject in this.GetVisualTargets())
		{
			if (!(gameObject == null))
			{
				Card component = gameObject.GetComponent<Card>();
				component.OverrideCustomSpawnSpell(Object.Instantiate<Spell>(this.m_CustomSpawnSpell));
			}
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x0400571A RID: 22298
	public Spell m_CustomSpawnSpell;
}
