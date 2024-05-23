using System;

// Token: 0x02000189 RID: 393
[Serializable]
public class EmoteEntryDef
{
	// Token: 0x04000B3A RID: 2874
	public EmoteType m_emoteType;

	// Token: 0x04000B3B RID: 2875
	[CustomEditField(T = EditType.CARD_SOUND_SPELL)]
	public string m_emoteSoundSpellPath;

	// Token: 0x04000B3C RID: 2876
	public string m_emoteGameStringKey;
}
