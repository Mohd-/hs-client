using System;
using System.Collections.Generic;

// Token: 0x020001A5 RID: 421
[Serializable]
public class CardEffectDef
{
	// Token: 0x04000E69 RID: 3689
	[CustomEditField(T = EditType.SPELL)]
	public string m_SpellPath;

	// Token: 0x04000E6A RID: 3690
	[CustomEditField(T = EditType.CARD_SOUND_SPELL)]
	public List<string> m_SoundSpellPaths = new List<string>();
}
