using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E6C RID: 3692
public class KelThuzad_StealTurn : Spell
{
	// Token: 0x06006FEA RID: 28650 RVA: 0x0020DDE3 File Offset: 0x0020BFE3
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.SpellEffect(prevStateType));
		base.OnAction(prevStateType);
	}

	// Token: 0x06006FEB RID: 28651 RVA: 0x0020DDFC File Offset: 0x0020BFFC
	private IEnumerator SpellEffect(SpellStateType prevStateType)
	{
		yield return new WaitForSeconds(0.25f);
		if (TurnTimer.Get() != null)
		{
			TurnTimer.Get().OnEndTurnRequested();
		}
		EndTurnButton.Get().m_EndTurnButtonMesh.GetComponent<Animation>()["ENDTURN_YOUR_TIMER_DONE"].speed = 0.7f;
		EndTurnButton.Get().OnTurnTimerEnded(true);
		yield return new WaitForSeconds(1f);
		EndTurnButton.Get().m_EndTurnButtonMesh.GetComponent<Animation>()["ENDTURN_YOUR_TIMER_DONE"].speed = 1f;
		yield break;
	}

	// Token: 0x04005919 RID: 22809
	public GameObject m_Lightning;
}
