using System;
using UnityEngine;

// Token: 0x020001CB RID: 459
[CustomEditClass]
public class AdventureGenericButton : PegUIElement
{
	// Token: 0x06001D4F RID: 7503 RVA: 0x000899BE File Offset: 0x00087BBE
	public bool IsPortraitLoaded()
	{
		return this.m_PortraitLoaded;
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x000899C8 File Offset: 0x00087BC8
	public void SetDesaturate(bool desaturate)
	{
		if (!this.CheckValidMaterialProperties(this.m_MaterialProperties))
		{
			return;
		}
		if (!this.CheckValidMaterialProperties(this.m_BorderMaterialProperties))
		{
			return;
		}
		this.m_PortraitRenderer.materials[this.m_MaterialProperties.m_MaterialIndex].SetFloat("_Desaturate", (!desaturate) ? 0f : 1f);
		this.m_BorderRenderer.materials[this.m_BorderMaterialProperties.m_MaterialIndex].SetFloat("_Desaturate", (!desaturate) ? 0f : 1f);
		this.m_ButtonTextObject.TextColor = ((!desaturate) ? this.m_NormalTextColor : this.m_DisabledTextColor);
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x00089A87 File Offset: 0x00087C87
	public void SetButtonText(string str)
	{
		if (this.m_ButtonTextObject == null)
		{
			return;
		}
		this.m_ButtonTextObject.Text = str;
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x00089AA7 File Offset: 0x00087CA7
	public void SetPortraitTexture(string texturename)
	{
		this.SetPortraitTexture(texturename, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);
	}

	// Token: 0x06001D53 RID: 7507 RVA: 0x00089AC8 File Offset: 0x00087CC8
	public void SetPortraitTexture(string texturename, int index, string mattexprop)
	{
		if (texturename == null || texturename.Length == 0)
		{
			return;
		}
		texturename = FileUtils.GameAssetPathToName(texturename);
		AdventureGenericButton.MaterialProperties materialProperties = new AdventureGenericButton.MaterialProperties
		{
			m_MaterialIndex = index,
			m_MaterialPropertyName = mattexprop
		};
		if (!this.CheckValidMaterialProperties(materialProperties))
		{
			return;
		}
		this.m_PortraitLoaded = false;
		AssetLoader.Get().LoadTexture(texturename, new AssetLoader.ObjectCallback(this.ApplyPortraitTexture), materialProperties, false);
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x00089B33 File Offset: 0x00087D33
	public void SetPortraitTiling(Vector2 tiling)
	{
		this.SetPortraitTiling(tiling, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x00089B54 File Offset: 0x00087D54
	public void SetPortraitTiling(Vector2 tiling, int index, string mattexprop)
	{
		AdventureGenericButton.MaterialProperties materialProperties = new AdventureGenericButton.MaterialProperties
		{
			m_MaterialIndex = index,
			m_MaterialPropertyName = mattexprop
		};
		if (!this.CheckValidMaterialProperties(materialProperties))
		{
			return;
		}
		this.m_PortraitRenderer.materials[materialProperties.m_MaterialIndex].SetTextureScale(materialProperties.m_MaterialPropertyName, tiling);
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x00089BA2 File Offset: 0x00087DA2
	public void SetPortraitOffset(Vector2 offset)
	{
		this.SetPortraitOffset(offset, this.m_MaterialProperties.m_MaterialIndex, this.m_MaterialProperties.m_MaterialPropertyName);
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x00089BC4 File Offset: 0x00087DC4
	public void SetPortraitOffset(Vector2 offset, int index, string mattexprop)
	{
		AdventureGenericButton.MaterialProperties materialProperties = new AdventureGenericButton.MaterialProperties
		{
			m_MaterialIndex = index,
			m_MaterialPropertyName = mattexprop
		};
		if (!this.CheckValidMaterialProperties(materialProperties))
		{
			return;
		}
		this.m_PortraitRenderer.materials[materialProperties.m_MaterialIndex].SetTextureOffset(materialProperties.m_MaterialPropertyName, offset);
	}

	// Token: 0x06001D58 RID: 7512 RVA: 0x00089C14 File Offset: 0x00087E14
	private void ApplyPortraitTexture(string name, Object obj, object userdata)
	{
		this.m_PortraitLoaded = true;
		AdventureGenericButton.MaterialProperties materialProperties = userdata as AdventureGenericButton.MaterialProperties;
		Texture texture = obj as Texture;
		if (texture == null)
		{
			Debug.LogError(string.Format("Unable to load portrait texture {0}.", name), obj);
			return;
		}
		this.m_PortraitRenderer.materials[materialProperties.m_MaterialIndex].SetTexture(materialProperties.m_MaterialPropertyName, texture);
	}

	// Token: 0x06001D59 RID: 7513 RVA: 0x00089C74 File Offset: 0x00087E74
	private bool CheckValidMaterialProperties(AdventureGenericButton.MaterialProperties matprop)
	{
		if (this.m_PortraitRenderer == null)
		{
			Debug.LogError("No portrait mesh renderer set.");
			return false;
		}
		if (matprop.m_MaterialIndex >= this.m_PortraitRenderer.materials.Length)
		{
			Debug.LogError(string.Format("Unable to find material index {0}", matprop.m_MaterialIndex));
			return false;
		}
		return true;
	}

	// Token: 0x04000FF9 RID: 4089
	private const string s_DefaultPortraitMaterialTextureName = "_MainTex";

	// Token: 0x04000FFA RID: 4090
	private const int s_DefaultPortraitMaterialIndex = 1;

	// Token: 0x04000FFB RID: 4091
	[CustomEditField(Sections = "Portrait Settings")]
	public MeshRenderer m_PortraitRenderer;

	// Token: 0x04000FFC RID: 4092
	[CustomEditField(Sections = "Portrait Settings")]
	public AdventureGenericButton.MaterialProperties m_MaterialProperties = new AdventureGenericButton.MaterialProperties();

	// Token: 0x04000FFD RID: 4093
	[CustomEditField(Sections = "Border Settings")]
	public MeshRenderer m_BorderRenderer;

	// Token: 0x04000FFE RID: 4094
	[CustomEditField(Sections = "Border Settings")]
	public AdventureGenericButton.MaterialProperties m_BorderMaterialProperties = new AdventureGenericButton.MaterialProperties();

	// Token: 0x04000FFF RID: 4095
	[CustomEditField(Sections = "Text Settings")]
	public UberText m_ButtonTextObject;

	// Token: 0x04001000 RID: 4096
	[CustomEditField(Sections = "Text Settings")]
	public Color m_NormalTextColor = default(Color);

	// Token: 0x04001001 RID: 4097
	public Color m_DisabledTextColor = default(Color);

	// Token: 0x04001002 RID: 4098
	private bool m_PortraitLoaded = true;

	// Token: 0x020001D5 RID: 469
	[Serializable]
	public class MaterialProperties
	{
		// Token: 0x0400103D RID: 4157
		public int m_MaterialIndex = 1;

		// Token: 0x0400103E RID: 4158
		public string m_MaterialPropertyName = "_MainTex";
	}
}
