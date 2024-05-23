using System;
using UnityEngine;

// Token: 0x02000E53 RID: 3667
public class OverrideCustomDeathSpell : SuperSpell
{
	// Token: 0x06006F5A RID: 28506 RVA: 0x0020B01C File Offset: 0x0020921C
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		foreach (GameObject gameObject in this.GetVisualTargets())
		{
			if (!(gameObject == null))
			{
				Card component = gameObject.GetComponent<Card>();
				component.OverrideCustomDeathSpell(Object.Instantiate<Spell>(this.m_CustomDeathSpell));
				component.SuppressKeywordDeaths(this.m_SuppressKeywordDeaths);
				component.SetKeywordDeathDelaySec(this.m_KeywordDeathDelay);
			}
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x04005887 RID: 22663
	public Spell m_CustomDeathSpell;

	// Token: 0x04005888 RID: 22664
	public bool m_SuppressKeywordDeaths = true;

	// Token: 0x04005889 RID: 22665
	public float m_KeywordDeathDelay = 0.6f;
}
