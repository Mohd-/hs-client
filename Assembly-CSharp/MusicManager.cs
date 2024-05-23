using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000236 RID: 566
public class MusicManager : MonoBehaviour
{
	// Token: 0x06002155 RID: 8533 RVA: 0x000A340A File Offset: 0x000A160A
	private void Awake()
	{
		MusicManager.s_instance = this;
	}

	// Token: 0x06002156 RID: 8534 RVA: 0x000A3414 File Offset: 0x000A1614
	private void Start()
	{
		if (ApplicationMgr.Get() != null)
		{
			ApplicationMgr.Get().WillReset += new Action(this.WillReset);
		}
	}

	// Token: 0x06002157 RID: 8535 RVA: 0x000A3447 File Offset: 0x000A1647
	private void OnDestroy()
	{
		MusicManager.s_instance = null;
	}

	// Token: 0x06002158 RID: 8536 RVA: 0x000A344F File Offset: 0x000A164F
	public static MusicManager Get()
	{
		return MusicManager.s_instance;
	}

	// Token: 0x06002159 RID: 8537 RVA: 0x000A3458 File Offset: 0x000A1658
	public bool StartPlaylist(MusicPlaylistType type)
	{
		if (this.m_currentPlaylist == type)
		{
			return true;
		}
		SoundManager soundManager = SoundManager.Get();
		if (soundManager == null)
		{
			Debug.LogError("MusicManager.StartPlaylist() - SoundManager does not exist.");
			return false;
		}
		MusicPlaylist musicPlaylist = this.FindPlaylist(type);
		if (musicPlaylist == null)
		{
			Debug.LogWarning(string.Format("MusicManager.StartPlaylist() - failed to find playlist for type {0}", type));
			return false;
		}
		List<MusicTrack> musicTracks = musicPlaylist.GetMusicTracks();
		List<MusicTrack> currentMusicTracks = soundManager.GetCurrentMusicTracks();
		if (!this.AreTracksEqual(musicTracks, currentMusicTracks))
		{
			soundManager.NukeMusicAndStopPlayingCurrentTrack();
			if (musicTracks != null && musicTracks.Count > 0)
			{
				soundManager.AddMusicTracks(musicTracks);
			}
		}
		List<MusicTrack> ambienceTracks = musicPlaylist.GetAmbienceTracks();
		List<MusicTrack> currentAmbienceTracks = soundManager.GetCurrentAmbienceTracks();
		if (!this.AreTracksEqual(ambienceTracks, currentAmbienceTracks))
		{
			soundManager.NukeAmbienceAndStopPlayingCurrentTrack();
			if (ambienceTracks != null && ambienceTracks.Count > 0)
			{
				soundManager.AddAmbienceTracks(ambienceTracks);
			}
		}
		this.m_currentPlaylist = musicPlaylist.m_type;
		return true;
	}

	// Token: 0x0600215A RID: 8538 RVA: 0x000A3540 File Offset: 0x000A1740
	public bool StopPlaylist()
	{
		SoundManager soundManager = SoundManager.Get();
		if (soundManager == null)
		{
			Debug.LogError("MusicManager.StopPlaylist() - SoundManager does not exist.");
			return false;
		}
		if (this.m_currentPlaylist == MusicPlaylistType.Invalid)
		{
			return false;
		}
		this.m_currentPlaylist = MusicPlaylistType.Invalid;
		soundManager.NukePlaylistsAndStopPlayingCurrentTracks();
		return true;
	}

	// Token: 0x0600215B RID: 8539 RVA: 0x000A3586 File Offset: 0x000A1786
	public MusicPlaylistType GetCurrentPlaylist()
	{
		return this.m_currentPlaylist;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000A3590 File Offset: 0x000A1790
	private void WillReset()
	{
		SoundManager soundManager = SoundManager.Get();
		if (soundManager == null)
		{
			Debug.LogError("MusicManager.WillReset() - SoundManager does not exist.");
			return;
		}
		this.m_currentPlaylist = MusicPlaylistType.Invalid;
		soundManager.ImmediatelyKillMusicAndAmbience();
	}

	// Token: 0x0600215D RID: 8541 RVA: 0x000A35C8 File Offset: 0x000A17C8
	private MusicPlaylist FindPlaylist(MusicPlaylistType type)
	{
		MusicConfig musicConfig = MusicConfig.Get();
		if (musicConfig == null)
		{
			Debug.LogError("MusicManager.FindPlaylist() - MusicConfig does not exist.");
			return null;
		}
		MusicPlaylist musicPlaylist = musicConfig.FindPlaylist(type);
		if (musicPlaylist == null)
		{
			Debug.LogWarning(string.Format("MusicManager.FindPlaylist() - {0} playlist is not defined.", type));
			return null;
		}
		return musicPlaylist;
	}

	// Token: 0x0600215E RID: 8542 RVA: 0x000A361C File Offset: 0x000A181C
	private bool AreTracksEqual(List<MusicTrack> lhsTracks, List<MusicTrack> rhsTracks)
	{
		if (lhsTracks.Count != rhsTracks.Count)
		{
			return false;
		}
		MusicTrack lhs;
		foreach (MusicTrack lhs2 in lhsTracks)
		{
			lhs = lhs2;
			if (rhsTracks.Find((MusicTrack rhs) => rhs.m_name == lhs.m_name) == null)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040012A1 RID: 4769
	private static MusicManager s_instance;

	// Token: 0x040012A2 RID: 4770
	private MusicPlaylistType m_currentPlaylist;
}
