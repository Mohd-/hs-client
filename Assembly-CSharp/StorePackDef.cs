using System;
using UnityEngine;

// Token: 0x02000AE2 RID: 2786
[CustomEditClass]
public class StorePackDef : MonoBehaviour
{
	// Token: 0x04004798 RID: 18328
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_buttonPrefab;

	// Token: 0x04004799 RID: 18329
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_lowPolyPrefab;

	// Token: 0x0400479A RID: 18330
	[CustomEditField(T = EditType.TEXTURE)]
	public string m_logoTextureName;

	// Token: 0x0400479B RID: 18331
	[CustomEditField(T = EditType.TEXTURE)]
	public string m_logoTextureGlowName;

	// Token: 0x0400479C RID: 18332
	[CustomEditField(T = EditType.TEXTURE)]
	public string m_accentTextureName;

	// Token: 0x0400479D RID: 18333
	public Material m_background;

	// Token: 0x0400479E RID: 18334
	public MusicPlaylistType m_playlist;

	// Token: 0x0400479F RID: 18335
	public string m_preorderAvailableDateString;
}
