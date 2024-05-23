using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000920 RID: 2336
public class HistoryTileInitInfo : HistoryItemInitInfo
{
	// Token: 0x04003D53 RID: 15699
	public HistoryInfoType m_type;

	// Token: 0x04003D54 RID: 15700
	public List<HistoryInfo> m_childInfos;

	// Token: 0x04003D55 RID: 15701
	public Texture m_fatigueTexture;

	// Token: 0x04003D56 RID: 15702
	public Material m_fullTileMaterial;

	// Token: 0x04003D57 RID: 15703
	public Material m_halfTileMaterial;

	// Token: 0x04003D58 RID: 15704
	public bool m_dead;

	// Token: 0x04003D59 RID: 15705
	public int m_splatAmount;
}
