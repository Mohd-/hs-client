using System;
using System.Collections.Generic;

// Token: 0x02000367 RID: 871
[Serializable]
public class SoundPlaybackLimitDef
{
	// Token: 0x04001B72 RID: 7026
	[CustomEditField(Label = "Playback Limit")]
	public int m_Limit = 1;

	// Token: 0x04001B73 RID: 7027
	[CustomEditField(Label = "Clip Group")]
	public List<SoundPlaybackLimitClipDef> m_ClipDefs = new List<SoundPlaybackLimitClipDef>();
}
