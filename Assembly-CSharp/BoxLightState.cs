using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067A RID: 1658
[Serializable]
public class BoxLightState
{
	// Token: 0x04002DB7 RID: 11703
	public BoxLightStateType m_Type;

	// Token: 0x04002DB8 RID: 11704
	public float m_DelaySec;

	// Token: 0x04002DB9 RID: 11705
	public float m_TransitionSec = 0.5f;

	// Token: 0x04002DBA RID: 11706
	public iTween.EaseType m_TransitionEaseType = iTween.EaseType.linear;

	// Token: 0x04002DBB RID: 11707
	public Spell m_Spell;

	// Token: 0x04002DBC RID: 11708
	public Color m_AmbientColor = new Color(0.5058824f, 0.4745098f, 0.4745098f, 1f);

	// Token: 0x04002DBD RID: 11709
	public List<BoxLightInfo> m_LightInfos;
}
