using System;

// Token: 0x02000361 RID: 865
[Serializable]
public class MusicTrack
{
	// Token: 0x06002C40 RID: 11328 RVA: 0x000DBEA8 File Offset: 0x000DA0A8
	public MusicTrack Clone()
	{
		return (MusicTrack)base.MemberwiseClone();
	}

	// Token: 0x04001B5D RID: 7005
	[CustomEditField(ListSortable = true)]
	public MusicTrackType m_trackType;

	// Token: 0x04001B5E RID: 7006
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_name;

	// Token: 0x04001B5F RID: 7007
	public float m_volume = 1f;

	// Token: 0x04001B60 RID: 7008
	public bool m_shuffle = true;
}
