using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028E RID: 654
[CustomEditClass]
public class SoundConfig : MonoBehaviour
{
	// Token: 0x040014EE RID: 5358
	[CustomEditField(Sections = "Music")]
	public float m_SecondsBetweenMusicTracks = 10f;

	// Token: 0x040014EF RID: 5359
	[CustomEditField(Sections = "System Audio Sources")]
	public AudioSource m_PlayClipTemplate;

	// Token: 0x040014F0 RID: 5360
	[CustomEditField(Sections = "System Audio Sources")]
	public AudioSource m_PlaceholderSound;

	// Token: 0x040014F1 RID: 5361
	[CustomEditField(Sections = "Playback Limiting")]
	public List<SoundPlaybackLimitDef> m_PlaybackLimitDefs = new List<SoundPlaybackLimitDef>();

	// Token: 0x040014F2 RID: 5362
	[CustomEditField(Sections = "Ducking")]
	public List<SoundDuckingDef> m_DuckingDefs = new List<SoundDuckingDef>();
}
