using System;
using System.Collections.Generic;

// Token: 0x0200036A RID: 874
[Serializable]
public class SoundDuckingDef
{
	// Token: 0x06002C77 RID: 11383 RVA: 0x000DC9E4 File Offset: 0x000DABE4
	public override string ToString()
	{
		return string.Format("[SoundDuckingDef: {0}]", this.m_TriggerCategory);
	}

	// Token: 0x04001B7B RID: 7035
	public SoundCategory m_TriggerCategory;

	// Token: 0x04001B7C RID: 7036
	public List<SoundDuckedCategoryDef> m_DuckedCategoryDefs;
}
