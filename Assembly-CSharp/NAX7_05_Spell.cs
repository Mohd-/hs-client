using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006AB RID: 1707
public class NAX7_05_Spell : Spell
{
	// Token: 0x0600478F RID: 18319 RVA: 0x00157564 File Offset: 0x00155764
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.StartCoroutine(this.SpellEffect(prevStateType));
	}

	// Token: 0x06004790 RID: 18320 RVA: 0x00157574 File Offset: 0x00155774
	private IEnumerator SpellEffect(SpellStateType prevStateType)
	{
		Board board = Board.Get();
		Transform crystal = board.transform.FindChild("Board_NAX").FindChild("NAX_Crystal_Skinned");
		PlayMakerFSM fsm = crystal.GetComponent<PlayMakerFSM>();
		if (fsm == null)
		{
			Debug.LogWarning("NAX7_05_Spell unable to get playmaker fsm");
			yield break;
		}
		fsm.SendEvent("ClickTop");
		base.OnBirth(prevStateType);
		this.OnSpellFinished();
		yield break;
	}
}
