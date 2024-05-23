using System;
using UnityEngine;

// Token: 0x02000F35 RID: 3893
[ExecuteInEditMode]
public class ScreenEffectGlow : ScreenEffect
{
	// Token: 0x060073C5 RID: 29637 RVA: 0x0022197C File Offset: 0x0021FB7C
	private void Awake()
	{
		this.m_PreviousLayer = base.gameObject.layer;
	}

	// Token: 0x060073C6 RID: 29638 RVA: 0x0022198F File Offset: 0x0021FB8F
	private void Start()
	{
		this.SetLayer();
	}

	// Token: 0x060073C7 RID: 29639 RVA: 0x00221997 File Offset: 0x0021FB97
	private void Update()
	{
	}

	// Token: 0x060073C8 RID: 29640 RVA: 0x0022199C File Offset: 0x0021FB9C
	private void SetLayer()
	{
		if (this.m_PreviousRenderGlowOnly != this.m_RenderGlowOnly)
		{
			this.m_PreviousRenderGlowOnly = this.m_RenderGlowOnly;
			if (this.m_RenderGlowOnly)
			{
				this.m_PreviousLayer = base.gameObject.layer;
				SceneUtils.SetLayer(base.gameObject, GameLayer.ScreenEffects);
			}
			else
			{
				SceneUtils.SetLayer(base.gameObject, this.m_PreviousLayer);
			}
		}
	}

	// Token: 0x04005E64 RID: 24164
	public bool m_RenderGlowOnly;

	// Token: 0x04005E65 RID: 24165
	private bool m_PreviousRenderGlowOnly;

	// Token: 0x04005E66 RID: 24166
	private int m_PreviousLayer;
}
