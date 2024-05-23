using System;
using System.Collections.Generic;

// Token: 0x02000E1F RID: 3615
public class AmbushSpell : OverrideCustomSpawnSpell
{
	// Token: 0x06006E97 RID: 28311 RVA: 0x00206E1C File Offset: 0x0020501C
	public override bool AddPowerTargets()
	{
		if (!this.m_taskList.IsStartOfBlock())
		{
			return false;
		}
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		base.AddMultiplePowerTargets_FromMetaData(taskList);
		return true;
	}
}
