using System;
using UnityEngine;

// Token: 0x02000959 RID: 2393
public class AudioSourceSettings
{
	// Token: 0x06005785 RID: 22405 RVA: 0x001A35CE File Offset: 0x001A17CE
	public AudioSourceSettings()
	{
		this.LoadDefaults();
	}

	// Token: 0x06005786 RID: 22406 RVA: 0x001A35DC File Offset: 0x001A17DC
	public void LoadDefaults()
	{
		this.m_bypassEffects = false;
		this.m_loop = false;
		this.m_priority = 128;
		this.m_volume = 1f;
		this.m_pitch = 1f;
		this.m_stereoPan = 0f;
		this.m_spatialBlend = 1f;
		this.m_reverbZoneMix = 1f;
		this.m_rolloffMode = 1;
		this.m_dopplerLevel = 1f;
		this.m_minDistance = 100f;
		this.m_maxDistance = 500f;
		this.m_spread = 0f;
	}

	// Token: 0x04003E5F RID: 15967
	public const bool DEFAULT_BYPASS_EFFECTS = false;

	// Token: 0x04003E60 RID: 15968
	public const bool DEFAULT_LOOP = false;

	// Token: 0x04003E61 RID: 15969
	public const float MIN_VOLUME = 0f;

	// Token: 0x04003E62 RID: 15970
	public const float MAX_VOLUME = 1f;

	// Token: 0x04003E63 RID: 15971
	public const float DEFAULT_VOLUME = 1f;

	// Token: 0x04003E64 RID: 15972
	public const int MIN_PRIORITY = 0;

	// Token: 0x04003E65 RID: 15973
	public const int MAX_PRIORITY = 256;

	// Token: 0x04003E66 RID: 15974
	public const int DEFAULT_PRIORITY = 128;

	// Token: 0x04003E67 RID: 15975
	public const float MIN_PITCH = -3f;

	// Token: 0x04003E68 RID: 15976
	public const float MAX_PITCH = 3f;

	// Token: 0x04003E69 RID: 15977
	public const float DEFAULT_PITCH = 1f;

	// Token: 0x04003E6A RID: 15978
	public const float MIN_STEREO_PAN = -1f;

	// Token: 0x04003E6B RID: 15979
	public const float MAX_STEREO_PAN = 1f;

	// Token: 0x04003E6C RID: 15980
	public const float DEFAULT_STEREO_PAN = 0f;

	// Token: 0x04003E6D RID: 15981
	public const float MIN_SPATIAL_BLEND = 0f;

	// Token: 0x04003E6E RID: 15982
	public const float MAX_SPATIAL_BLEND = 1f;

	// Token: 0x04003E6F RID: 15983
	public const float DEFAULT_SPATIAL_BLEND = 1f;

	// Token: 0x04003E70 RID: 15984
	public const float MIN_REVERB_ZONE_MIX = 0f;

	// Token: 0x04003E71 RID: 15985
	public const float MAX_REVERB_ZONE_MIX = 1.1f;

	// Token: 0x04003E72 RID: 15986
	public const float DEFAULT_REVERB_ZONE_MIX = 1f;

	// Token: 0x04003E73 RID: 15987
	public const AudioRolloffMode DEFAULT_ROLLOFF_MODE = 1;

	// Token: 0x04003E74 RID: 15988
	public const float MIN_DOPPLER_LEVEL = 0f;

	// Token: 0x04003E75 RID: 15989
	public const float MAX_DOPPLER_LEVEL = 5f;

	// Token: 0x04003E76 RID: 15990
	public const float DEFAULT_DOPPLER_LEVEL = 1f;

	// Token: 0x04003E77 RID: 15991
	public const float DEFAULT_MIN_DISTANCE = 100f;

	// Token: 0x04003E78 RID: 15992
	public const float DEFAULT_MAX_DISTANCE = 500f;

	// Token: 0x04003E79 RID: 15993
	public const float MIN_SPREAD = 0f;

	// Token: 0x04003E7A RID: 15994
	public const float MAX_SPREAD = 360f;

	// Token: 0x04003E7B RID: 15995
	public const float DEFAULT_SPREAD = 0f;

	// Token: 0x04003E7C RID: 15996
	public bool m_bypassEffects;

	// Token: 0x04003E7D RID: 15997
	public bool m_loop;

	// Token: 0x04003E7E RID: 15998
	public int m_priority;

	// Token: 0x04003E7F RID: 15999
	public float m_volume;

	// Token: 0x04003E80 RID: 16000
	public float m_pitch;

	// Token: 0x04003E81 RID: 16001
	public float m_stereoPan;

	// Token: 0x04003E82 RID: 16002
	public float m_spatialBlend;

	// Token: 0x04003E83 RID: 16003
	public float m_reverbZoneMix;

	// Token: 0x04003E84 RID: 16004
	public AudioRolloffMode m_rolloffMode;

	// Token: 0x04003E85 RID: 16005
	public float m_dopplerLevel;

	// Token: 0x04003E86 RID: 16006
	public float m_minDistance;

	// Token: 0x04003E87 RID: 16007
	public float m_maxDistance;

	// Token: 0x04003E88 RID: 16008
	public float m_spread;
}
