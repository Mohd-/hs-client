using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000456 RID: 1110
[CustomEditClass]
public class StoreAdventureDef : MonoBehaviour
{
	// Token: 0x04002235 RID: 8757
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_storeButtonPrefab;

	// Token: 0x04002236 RID: 8758
	[CustomEditField(T = EditType.TEXTURE)]
	public string m_logoTextureName;

	// Token: 0x04002237 RID: 8759
	public Material m_keyArt;

	// Token: 0x04002238 RID: 8760
	public int m_preorderCardBackId;

	// Token: 0x04002239 RID: 8761
	[CustomEditField(T = EditType.TEXTURE)]
	public string m_accentTextureName;

	// Token: 0x0400223A RID: 8762
	public MusicPlaylistType m_playlist;

	// Token: 0x0400223B RID: 8763
	public List<string> m_previewCards = new List<string>();
}
