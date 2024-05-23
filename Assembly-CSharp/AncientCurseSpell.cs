using System;
using UnityEngine;

// Token: 0x02000E21 RID: 3617
public class AncientCurseSpell : SuperSpell
{
	// Token: 0x06006E9B RID: 28315 RVA: 0x00206F0C File Offset: 0x0020510C
	public void DoHeroDamage()
	{
		Log.JMac.Print("AncientCurseSpell - DoHeroDamage()!", new object[0]);
		PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
		if (currentTaskList == null)
		{
			Debug.LogWarning("AncientCurseSpell.DoHeroDamage() called when there was no current PowerTaskList!");
		}
		else
		{
			GameUtils.DoDamageTasks(currentTaskList, base.GetSourceCard(), this.GetVisualTargetCard());
		}
	}
}
