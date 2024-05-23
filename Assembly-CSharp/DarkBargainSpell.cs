using System;
using System.Collections.Generic;

// Token: 0x02000E52 RID: 3666
public class DarkBargainSpell : OverrideCustomDeathSpell
{
	// Token: 0x06006F58 RID: 28504 RVA: 0x0020AFE0 File Offset: 0x002091E0
	public override bool AddPowerTargets()
	{
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		base.AddMultiplePowerTargets_FromMetaData(taskList);
		return true;
	}
}
