using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028F RID: 655
[CustomEditClass]
public class MusicConfig : MonoBehaviour
{
	// Token: 0x060023CC RID: 9164 RVA: 0x000AFAD2 File Offset: 0x000ADCD2
	private void Awake()
	{
		MusicConfig.s_instance = this;
	}

	// Token: 0x060023CD RID: 9165 RVA: 0x000AFADA File Offset: 0x000ADCDA
	private void OnDestroy()
	{
		MusicConfig.s_instance = null;
	}

	// Token: 0x060023CE RID: 9166 RVA: 0x000AFAE2 File Offset: 0x000ADCE2
	public static MusicConfig Get()
	{
		return MusicConfig.s_instance;
	}

	// Token: 0x060023CF RID: 9167 RVA: 0x000AFAEC File Offset: 0x000ADCEC
	public MusicPlaylist GetPlaylist(MusicPlaylistType type)
	{
		MusicPlaylist musicPlaylist = this.FindPlaylist(type);
		return musicPlaylist ?? new MusicPlaylist();
	}

	// Token: 0x060023D0 RID: 9168 RVA: 0x000AFB10 File Offset: 0x000ADD10
	public MusicPlaylist FindPlaylist(MusicPlaylistType type)
	{
		for (int i = 0; i < this.m_playlists.Count; i++)
		{
			MusicPlaylist musicPlaylist = this.m_playlists[i];
			if (musicPlaylist.m_type == type)
			{
				return musicPlaylist;
			}
		}
		return null;
	}

	// Token: 0x040014F3 RID: 5363
	[CustomEditField(Sections = "Playlists")]
	public List<MusicPlaylist> m_playlists = new List<MusicPlaylist>();

	// Token: 0x040014F4 RID: 5364
	private static MusicConfig s_instance;
}
