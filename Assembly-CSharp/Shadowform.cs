using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000E8F RID: 3727
public class Shadowform : SuperSpell
{
	// Token: 0x060070AE RID: 28846 RVA: 0x00212F54 File Offset: 0x00211154
	protected override void OnBirth(SpellStateType prevStateType)
	{
		base.OnBirth(prevStateType);
		if (this.m_ShadowformMaterial == null)
		{
			return;
		}
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(this);
		actor.SetShadowform(true);
		this.m_MaterialInstance = new Material(this.m_ShadowformMaterial);
		Texture portraitTexture = actor.GetPortraitTexture();
		this.m_MaterialInstance.mainTexture = portraitTexture;
		actor.SetPortraitMaterial(this.m_MaterialInstance);
		this.OnSpellFinished();
		GameObject portraitMesh = actor.GetPortraitMesh();
		Material mat = portraitMesh.GetComponent<Renderer>().materials[actor.m_portraitMatIdx];
		Action<object> action = delegate(object desat)
		{
			mat.SetFloat("_Desaturate", (float)desat);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInTime,
			"from",
			0f,
			"to",
			this.m_Desaturate,
			"onupdate",
			action,
			"onupdatetarget",
			actor.gameObject
		});
		iTween.ValueTo(actor.gameObject, args);
		Action<object> action2 = delegate(object col)
		{
			mat.SetColor("_Color", (Color)col);
		};
		Hashtable args2 = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInTime,
			"from",
			Color.white,
			"to",
			this.m_Tint,
			"onupdate",
			action2,
			"onupdatetarget",
			actor.gameObject
		});
		iTween.ValueTo(actor.gameObject, args2);
		Action<object> action3 = delegate(object desat)
		{
			mat.SetFloat("_Contrast", (float)desat);
		};
		Hashtable args3 = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInTime,
			"from",
			0f,
			"to",
			this.m_Contrast,
			"onupdate",
			action3,
			"onupdatetarget",
			actor.gameObject
		});
		iTween.ValueTo(actor.gameObject, args3);
		Action<object> action4 = delegate(object desat)
		{
			mat.SetFloat("_Intensity", (float)desat);
		};
		Hashtable args4 = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInTime,
			"from",
			1f,
			"to",
			this.m_Intensity,
			"onupdate",
			action4,
			"onupdatetarget",
			actor.gameObject
		});
		iTween.ValueTo(actor.gameObject, args4);
		Action<object> action5 = delegate(object desat)
		{
			mat.SetFloat("_FxIntensity", (float)desat);
		};
		Hashtable args5 = iTween.Hash(new object[]
		{
			"time",
			this.m_FadeInTime,
			"from",
			0f,
			"to",
			this.m_FxIntensity,
			"onupdate",
			action5,
			"onupdatetarget",
			actor.gameObject
		});
		iTween.ValueTo(actor.gameObject, args5);
	}

	// Token: 0x060070AF RID: 28847 RVA: 0x00213298 File Offset: 0x00211498
	protected override void OnDeath(SpellStateType prevStateType)
	{
		base.OnDeath(prevStateType);
		Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(this);
		actor.SetShadowform(false);
		actor.UpdateAllComponents();
	}

	// Token: 0x040059F9 RID: 23033
	public Material m_ShadowformMaterial;

	// Token: 0x040059FA RID: 23034
	public int m_MaterialIndex = 1;

	// Token: 0x040059FB RID: 23035
	public float m_FadeInTime = 1f;

	// Token: 0x040059FC RID: 23036
	public float m_Desaturate = 0.8f;

	// Token: 0x040059FD RID: 23037
	public Color m_Tint = new Color(0.69140625f, 0.328125f, 0.8046875f, 1f);

	// Token: 0x040059FE RID: 23038
	public float m_Contrast = -0.29f;

	// Token: 0x040059FF RID: 23039
	public float m_Intensity = 0.85f;

	// Token: 0x04005A00 RID: 23040
	public float m_FxIntensity = 4f;

	// Token: 0x04005A01 RID: 23041
	private Material m_MaterialInstance;
}
