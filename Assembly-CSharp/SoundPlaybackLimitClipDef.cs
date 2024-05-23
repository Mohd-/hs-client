using System;

// Token: 0x02000368 RID: 872
[Serializable]
public class SoundPlaybackLimitClipDef
{
	// Token: 0x04001B74 RID: 7028
	[CustomEditField(Label = "Clip", T = EditType.AUDIO_CLIP)]
	public string m_Path;

	// Token: 0x04001B75 RID: 7029
	public int m_Priority;

	// Token: 0x04001B76 RID: 7030
	[CustomEditField(Range = "0.0-1.0")]
	public float m_ExclusivePlaybackThreshold = 0.1f;
}
