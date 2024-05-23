using System;
using UnityEngine;

// Token: 0x020001C6 RID: 454
[CustomEditClass]
public class AdventureBossDef : MonoBehaviour
{
	// Token: 0x04000FA4 RID: 4004
	public Material m_CoinPortraitMaterial;

	// Token: 0x04000FA5 RID: 4005
	public AdventureBossDef.IntroLinePlayTime m_IntroLinePlayTime;

	// Token: 0x04000FA6 RID: 4006
	public string m_IntroLine;

	// Token: 0x04000FA7 RID: 4007
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_quotePrefabOverride;

	// Token: 0x04000FA8 RID: 4008
	public MusicPlaylistType m_MissionMusic;

	// Token: 0x020001C7 RID: 455
	public enum IntroLinePlayTime
	{
		// Token: 0x04000FAA RID: 4010
		MissionSelect,
		// Token: 0x04000FAB RID: 4011
		MissionStart
	}
}
