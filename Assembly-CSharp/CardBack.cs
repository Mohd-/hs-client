using System;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class CardBack : MonoBehaviour
{
	// Token: 0x0400223C RID: 8764
	public Mesh m_CardBackMesh;

	// Token: 0x0400223D RID: 8765
	public Material m_CardBackMaterial;

	// Token: 0x0400223E RID: 8766
	public Material m_CardBackMaterial1;

	// Token: 0x0400223F RID: 8767
	public Texture2D m_CardBackTexture;

	// Token: 0x04002240 RID: 8768
	public Texture2D m_HiddenCardEchoTexture;

	// Token: 0x04002241 RID: 8769
	public GameObject m_DragEffect;

	// Token: 0x04002242 RID: 8770
	public float m_EffectMinVelocity = 2f;

	// Token: 0x04002243 RID: 8771
	public float m_EffectMaxVelocity = 40f;
}
