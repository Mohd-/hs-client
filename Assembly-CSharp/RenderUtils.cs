using System;
using UnityEngine;

// Token: 0x02000336 RID: 822
public static class RenderUtils
{
	// Token: 0x06002A9B RID: 10907 RVA: 0x000D216A File Offset: 0x000D036A
	public static Material GetMaterial(Component c, int materialIndex)
	{
		return RenderUtils.GetMaterial(c.GetComponent<Renderer>(), materialIndex);
	}

	// Token: 0x06002A9C RID: 10908 RVA: 0x000D2178 File Offset: 0x000D0378
	public static Material GetMaterial(GameObject go, int materialIndex)
	{
		return RenderUtils.GetMaterial(go.GetComponent<Renderer>(), materialIndex);
	}

	// Token: 0x06002A9D RID: 10909 RVA: 0x000D2188 File Offset: 0x000D0388
	public static Material GetMaterial(Renderer renderer, int materialIndex)
	{
		if (materialIndex < 0)
		{
			return null;
		}
		Material[] materials = renderer.materials;
		if (materialIndex >= materials.Length)
		{
			return null;
		}
		Material material = materials[materialIndex];
		material.shaderKeywords = renderer.sharedMaterials[materialIndex].shaderKeywords;
		return material;
	}

	// Token: 0x06002A9E RID: 10910 RVA: 0x000D21C8 File Offset: 0x000D03C8
	public static Material GetSharedMaterial(Component c, int materialIndex)
	{
		return RenderUtils.GetSharedMaterial(c.GetComponent<Renderer>(), materialIndex);
	}

	// Token: 0x06002A9F RID: 10911 RVA: 0x000D21D6 File Offset: 0x000D03D6
	public static Material GetSharedMaterial(GameObject go, int materialIndex)
	{
		return RenderUtils.GetSharedMaterial(go.GetComponent<Renderer>(), materialIndex);
	}

	// Token: 0x06002AA0 RID: 10912 RVA: 0x000D21E4 File Offset: 0x000D03E4
	public static Material GetSharedMaterial(Renderer renderer, int materialIndex)
	{
		if (materialIndex < 0)
		{
			return null;
		}
		Material[] sharedMaterials = renderer.sharedMaterials;
		if (materialIndex >= sharedMaterials.Length)
		{
			return null;
		}
		return sharedMaterials[materialIndex];
	}

	// Token: 0x06002AA1 RID: 10913 RVA: 0x000D220F File Offset: 0x000D040F
	public static void SetMaterial(Component c, int materialIndex, Material material)
	{
		RenderUtils.SetMaterial(c.GetComponent<Renderer>(), materialIndex, material);
	}

	// Token: 0x06002AA2 RID: 10914 RVA: 0x000D221E File Offset: 0x000D041E
	public static void SetMaterial(GameObject go, int materialIndex, Material material)
	{
		RenderUtils.SetMaterial(go.GetComponent<Renderer>(), materialIndex, material);
	}

	// Token: 0x06002AA3 RID: 10915 RVA: 0x000D2230 File Offset: 0x000D0430
	public static void SetMaterial(Renderer renderer, int materialIndex, Material material)
	{
		if (materialIndex < 0)
		{
			return;
		}
		Material[] materials = renderer.materials;
		if (materialIndex >= materials.Length)
		{
			return;
		}
		materials[materialIndex] = material;
		renderer.materials = materials;
		renderer.materials[materialIndex].shaderKeywords = material.shaderKeywords;
	}

	// Token: 0x06002AA4 RID: 10916 RVA: 0x000D2274 File Offset: 0x000D0474
	public static void SetSharedMaterial(Component c, int materialIndex, Material material)
	{
		RenderUtils.SetSharedMaterial(c.GetComponent<Renderer>(), materialIndex, material);
	}

	// Token: 0x06002AA5 RID: 10917 RVA: 0x000D2283 File Offset: 0x000D0483
	public static void SetSharedMaterial(GameObject go, int materialIndex, Material material)
	{
		RenderUtils.SetSharedMaterial(go.GetComponent<Renderer>(), materialIndex, material);
	}

	// Token: 0x06002AA6 RID: 10918 RVA: 0x000D2294 File Offset: 0x000D0494
	public static void SetSharedMaterial(Renderer renderer, int materialIndex, Material material)
	{
		if (material == null)
		{
			return;
		}
		if (materialIndex < 0)
		{
			return;
		}
		Material[] sharedMaterials = renderer.sharedMaterials;
		if (materialIndex >= sharedMaterials.Length)
		{
			return;
		}
		sharedMaterials[materialIndex] = material;
		sharedMaterials[materialIndex].shaderKeywords = material.shaderKeywords;
		renderer.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06002AA7 RID: 10919 RVA: 0x000D22E0 File Offset: 0x000D04E0
	public static void SetAlpha(Component c, float alpha)
	{
		RenderUtils.SetAlpha(c.gameObject, alpha, false);
	}

	// Token: 0x06002AA8 RID: 10920 RVA: 0x000D22EF File Offset: 0x000D04EF
	public static void SetAlpha(Component c, float alpha, bool includeInactive)
	{
		RenderUtils.SetAlpha(c.gameObject, alpha, includeInactive);
	}

	// Token: 0x06002AA9 RID: 10921 RVA: 0x000D22FE File Offset: 0x000D04FE
	public static void SetAlpha(GameObject go, float alpha)
	{
		RenderUtils.SetAlpha(go, alpha, false);
	}

	// Token: 0x06002AAA RID: 10922 RVA: 0x000D2308 File Offset: 0x000D0508
	public static void SetAlpha(GameObject go, float alpha, bool includeInactive)
	{
		foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(includeInactive))
		{
			foreach (Material material in renderer.materials)
			{
				if (material.HasProperty("_Color"))
				{
					Color color = material.color;
					color.a = alpha;
					material.color = color;
				}
				else if (material.HasProperty("_TintColor"))
				{
					Color color2 = material.GetColor("_TintColor");
					color2.a = alpha;
					material.SetColor("_TintColor", color2);
				}
			}
			if (renderer.GetComponent<Light>() != null)
			{
				Color color3 = renderer.GetComponent<Light>().color;
				color3.a = alpha;
				renderer.GetComponent<Light>().color = color3;
			}
		}
		foreach (UberText uberText in go.GetComponentsInChildren<UberText>(includeInactive))
		{
			Color textColor = uberText.TextColor;
			uberText.TextColor = new Color(textColor.r, textColor.g, textColor.b, alpha);
		}
	}

	// Token: 0x06002AAB RID: 10923 RVA: 0x000D2442 File Offset: 0x000D0642
	public static float GetMainTextureScaleX(GameObject go)
	{
		return RenderUtils.GetMainTextureScaleX(go.GetComponent<Renderer>());
	}

	// Token: 0x06002AAC RID: 10924 RVA: 0x000D244F File Offset: 0x000D064F
	public static float GetMainTextureScaleX(Component c)
	{
		return RenderUtils.GetMainTextureScaleX(c.GetComponent<Renderer>());
	}

	// Token: 0x06002AAD RID: 10925 RVA: 0x000D245C File Offset: 0x000D065C
	public static float GetMainTextureScaleX(Renderer r)
	{
		return r.material.mainTextureScale.x;
	}

	// Token: 0x06002AAE RID: 10926 RVA: 0x000D247C File Offset: 0x000D067C
	public static void SetMainTextureScaleX(Component c, float x)
	{
		RenderUtils.SetMainTextureScaleX(c.GetComponent<Renderer>(), x);
	}

	// Token: 0x06002AAF RID: 10927 RVA: 0x000D248A File Offset: 0x000D068A
	public static void SetMainTextureScaleX(GameObject go, float x)
	{
		RenderUtils.SetMainTextureScaleX(go.GetComponent<Renderer>(), x);
	}

	// Token: 0x06002AB0 RID: 10928 RVA: 0x000D2498 File Offset: 0x000D0698
	public static void SetMainTextureScaleX(Renderer r, float x)
	{
		Vector2 mainTextureScale = r.material.mainTextureScale;
		mainTextureScale.x = x;
		r.material.mainTextureScale = mainTextureScale;
	}

	// Token: 0x06002AB1 RID: 10929 RVA: 0x000D24C5 File Offset: 0x000D06C5
	public static float GetMainTextureScaleY(GameObject go)
	{
		return RenderUtils.GetMainTextureScaleY(go.GetComponent<Renderer>());
	}

	// Token: 0x06002AB2 RID: 10930 RVA: 0x000D24D2 File Offset: 0x000D06D2
	public static float GetMainTextureScaleY(Component c)
	{
		return RenderUtils.GetMainTextureScaleY(c.GetComponent<Renderer>());
	}

	// Token: 0x06002AB3 RID: 10931 RVA: 0x000D24E0 File Offset: 0x000D06E0
	public static float GetMainTextureScaleY(Renderer r)
	{
		return r.material.mainTextureScale.y;
	}

	// Token: 0x06002AB4 RID: 10932 RVA: 0x000D2500 File Offset: 0x000D0700
	public static void SetMainTextureScaleY(Component c, float y)
	{
		RenderUtils.SetMainTextureScaleY(c.GetComponent<Renderer>(), y);
	}

	// Token: 0x06002AB5 RID: 10933 RVA: 0x000D250E File Offset: 0x000D070E
	public static void SetMainTextureScaleY(GameObject go, float y)
	{
		RenderUtils.SetMainTextureScaleY(go.GetComponent<Renderer>(), y);
	}

	// Token: 0x06002AB6 RID: 10934 RVA: 0x000D251C File Offset: 0x000D071C
	public static void SetMainTextureScaleY(Renderer r, float y)
	{
		Vector2 mainTextureScale = r.material.mainTextureScale;
		mainTextureScale.y = y;
		r.material.mainTextureScale = mainTextureScale;
	}

	// Token: 0x06002AB7 RID: 10935 RVA: 0x000D2549 File Offset: 0x000D0749
	public static float GetMainTextureOffsetX(GameObject go)
	{
		return RenderUtils.GetMainTextureOffsetX(go.GetComponent<Renderer>());
	}

	// Token: 0x06002AB8 RID: 10936 RVA: 0x000D2556 File Offset: 0x000D0756
	public static float GetMainTextureOffsetX(Component c)
	{
		return RenderUtils.GetMainTextureOffsetX(c.GetComponent<Renderer>());
	}

	// Token: 0x06002AB9 RID: 10937 RVA: 0x000D2564 File Offset: 0x000D0764
	public static float GetMainTextureOffsetX(Renderer r)
	{
		return r.material.mainTextureOffset.x;
	}

	// Token: 0x06002ABA RID: 10938 RVA: 0x000D2584 File Offset: 0x000D0784
	public static void SetMainTextureOffsetX(Component c, float x)
	{
		RenderUtils.SetMainTextureOffsetY(c.GetComponent<Renderer>(), x);
	}

	// Token: 0x06002ABB RID: 10939 RVA: 0x000D2592 File Offset: 0x000D0792
	public static void SetMainTextureOffsetX(GameObject go, float x)
	{
		RenderUtils.SetMainTextureOffsetY(go.GetComponent<Renderer>(), x);
	}

	// Token: 0x06002ABC RID: 10940 RVA: 0x000D25A0 File Offset: 0x000D07A0
	public static void SetMainTextureOffsetX(Renderer r, float x)
	{
		Vector2 mainTextureOffset = r.material.mainTextureOffset;
		mainTextureOffset.x = x;
		r.material.mainTextureOffset = mainTextureOffset;
	}

	// Token: 0x06002ABD RID: 10941 RVA: 0x000D25CD File Offset: 0x000D07CD
	public static float GetMainTextureOffsetY(GameObject go)
	{
		return RenderUtils.GetMainTextureOffsetY(go.GetComponent<Renderer>());
	}

	// Token: 0x06002ABE RID: 10942 RVA: 0x000D25DA File Offset: 0x000D07DA
	public static float GetMainTextureOffsetY(Component c)
	{
		return RenderUtils.GetMainTextureOffsetY(c.GetComponent<Renderer>());
	}

	// Token: 0x06002ABF RID: 10943 RVA: 0x000D25E8 File Offset: 0x000D07E8
	public static float GetMainTextureOffsetY(Renderer r)
	{
		return r.material.mainTextureOffset.y;
	}

	// Token: 0x06002AC0 RID: 10944 RVA: 0x000D2608 File Offset: 0x000D0808
	public static void SetMainTextureOffsetY(Component c, float y)
	{
		RenderUtils.SetMainTextureOffsetY(c.GetComponent<Renderer>(), y);
	}

	// Token: 0x06002AC1 RID: 10945 RVA: 0x000D2616 File Offset: 0x000D0816
	public static void SetMainTextureOffsetY(GameObject go, float y)
	{
		RenderUtils.SetMainTextureOffsetY(go.GetComponent<Renderer>(), y);
	}

	// Token: 0x06002AC2 RID: 10946 RVA: 0x000D2624 File Offset: 0x000D0824
	public static void SetMainTextureOffsetY(Renderer r, float y)
	{
		Vector2 mainTextureOffset = r.material.mainTextureOffset;
		mainTextureOffset.y = y;
		r.material.mainTextureOffset = mainTextureOffset;
	}
}
