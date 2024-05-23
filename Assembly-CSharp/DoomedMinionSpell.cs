using System;
using UnityEngine;

// Token: 0x02000E5B RID: 3675
public class DoomedMinionSpell : SuperSpell
{
	// Token: 0x06006F85 RID: 28549 RVA: 0x0020BB54 File Offset: 0x00209D54
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		foreach (GameObject gameObject in this.GetVisualTargets())
		{
			if (!(gameObject == null))
			{
				Card component = gameObject.GetComponent<Card>();
				component.ActivateActorSpell(this.m_SpellType);
			}
		}
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x040058A9 RID: 22697
	public SpellType m_SpellType;
}
