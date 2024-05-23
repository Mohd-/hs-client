using System;
using System.Collections.Generic;

// Token: 0x02000432 RID: 1074
[Serializable]
public class MusicPlaylist
{
	// Token: 0x0600361B RID: 13851 RVA: 0x0010B406 File Offset: 0x00109606
	public List<MusicTrack> GetMusicTracks()
	{
		return this.GetRandomizedTracks(this.m_tracks, MusicTrackType.Music);
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x0010B415 File Offset: 0x00109615
	public List<MusicTrack> GetAmbienceTracks()
	{
		return this.GetRandomizedTracks(this.m_tracks, MusicTrackType.Ambience);
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x0010B424 File Offset: 0x00109624
	private List<MusicTrack> GetRandomizedTracks(List<MusicTrack> trackList, MusicTrackType type)
	{
		List<MusicTrack> list = new List<MusicTrack>();
		List<MusicTrack> list2 = new List<MusicTrack>();
		foreach (MusicTrack musicTrack in trackList)
		{
			if (type == musicTrack.m_trackType && !string.IsNullOrEmpty(musicTrack.m_name))
			{
				if (musicTrack.m_shuffle)
				{
					list2.Add(musicTrack.Clone());
				}
				else
				{
					list.Add(musicTrack.Clone());
				}
			}
		}
		Random random = new Random();
		while (list2.Count > 0)
		{
			int num = random.Next(0, list2.Count);
			list.Add(list2[num]);
			list2.RemoveAt(num);
		}
		return list;
	}

	// Token: 0x0400219D RID: 8605
	[CustomEditField(ListSortable = true)]
	public MusicPlaylistType m_type;

	// Token: 0x0400219E RID: 8606
	[CustomEditField(ListTable = true)]
	public List<MusicTrack> m_tracks = new List<MusicTrack>();
}
