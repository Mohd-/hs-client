using System;

// Token: 0x02000337 RID: 823
[Serializable]
public class SpellTableEntry
{
	// Token: 0x040019A0 RID: 6560
	public SpellType m_Type;

	// Token: 0x040019A1 RID: 6561
	[CustomEditField(Hide = true)]
	public Spell m_Spell;

	// Token: 0x040019A2 RID: 6562
	[CustomEditField(T = EditType.SPELL)]
	public string m_SpellPrefabName = string.Empty;
}
