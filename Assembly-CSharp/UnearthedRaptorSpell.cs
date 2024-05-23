using System;

// Token: 0x02000EA6 RID: 3750
public class UnearthedRaptorSpell : SuperSpell
{
	// Token: 0x06007114 RID: 28948 RVA: 0x002155E0 File Offset: 0x002137E0
	public override bool AddPowerTargets()
	{
		bool flag = base.AddPowerTargets();
		return flag && this.m_targets.Count > 0;
	}
}
