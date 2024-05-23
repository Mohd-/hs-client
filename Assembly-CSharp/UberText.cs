using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x02000121 RID: 289
[ExecuteInEditMode]
[CustomEditClass]
public class UberText : MonoBehaviour
{
	// Token: 0x06000DEF RID: 3567 RVA: 0x000389B0 File Offset: 0x00036BB0
	public static UberText[] EnableAllTextInObject(GameObject obj, bool enable)
	{
		UberText[] componentsInChildren = obj.GetComponentsInChildren<UberText>();
		UberText.EnableAllTextObjects(componentsInChildren, enable);
		return componentsInChildren;
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x000389CC File Offset: 0x00036BCC
	public static void EnableAllTextObjects(UberText[] objs, bool enable)
	{
		foreach (UberText uberText in objs)
		{
			uberText.gameObject.SetActive(enable);
		}
	}

	// Token: 0x1700020B RID: 523
	// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x000389FF File Offset: 0x00036BFF
	protected float Offset
	{
		get
		{
			if (this.m_Offset == 0f)
			{
				UberText.s_offset -= 100f;
				this.m_Offset = UberText.s_offset;
			}
			return this.m_Offset;
		}
	}

	// Token: 0x1700020C RID: 524
	// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x00038A34 File Offset: 0x00036C34
	protected Material TextMaterial
	{
		get
		{
			if (this.m_TextMaterial == null)
			{
				if (this.m_TextShader == null)
				{
					this.m_TextShader = ShaderUtils.FindShader(this.TEXT_SHADER_NAME);
					if (!this.m_TextShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.TEXT_SHADER_NAME);
					}
				}
				this.m_TextMaterial = new Material(this.m_TextShader);
				SceneUtils.SetHideFlags(this.m_TextMaterial, 52);
			}
			return this.m_TextMaterial;
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x06000DF3 RID: 3571 RVA: 0x00038AC0 File Offset: 0x00036CC0
	protected Material PlaneMaterial
	{
		get
		{
			if (this.m_PlaneMaterial == null)
			{
				if (this.m_PlaneShader == null)
				{
					this.m_PlaneShader = ShaderUtils.FindShader(this.PLANE_SHADER_NAME);
					if (!this.m_PlaneShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.PLANE_SHADER_NAME);
					}
				}
				this.m_PlaneMaterial = new Material(this.m_PlaneShader);
				SceneUtils.SetHideFlags(this.m_PlaneMaterial, 52);
			}
			return this.m_PlaneMaterial;
		}
	}

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x00038B4C File Offset: 0x00036D4C
	protected Material BoldMaterial
	{
		get
		{
			if (this.m_BoldMaterial == null)
			{
				if (this.m_BoldShader == null)
				{
					this.m_BoldShader = ShaderUtils.FindShader(this.BOLD_SHADER_NAME);
					if (!this.m_BoldShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.BOLD_SHADER_NAME);
					}
				}
				this.m_BoldMaterial = new Material(this.m_BoldShader);
				SceneUtils.SetHideFlags(this.m_BoldMaterial, 52);
			}
			return this.m_BoldMaterial;
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x06000DF5 RID: 3573 RVA: 0x00038BD8 File Offset: 0x00036DD8
	protected Material OutlineTextMaterial
	{
		get
		{
			string text = this.OUTLINE_TEXT_SHADER_NAME;
			if (Localization.GetLocale() == Locale.thTH)
			{
				text = this.OUTLINE_TEXT_2PASS_SHADER_NAME;
			}
			if (!this.m_RichText)
			{
				if (Localization.GetLocale() == Locale.thTH)
				{
					text = this.OUTLINE_NO_VERT_COLOR_TEXT_2PASS_SHADER_NAME;
				}
				else
				{
					text = this.OUTLINE_NO_VERT_COLOR_TEXT_SHADER_NAME;
				}
			}
			if (this.m_OutlineTextMaterial != null && text != this.m_OutlineTextMaterial.shader.name)
			{
				this.m_OutlineTextShader = null;
				Object.DestroyImmediate(this.m_OutlineTextMaterial);
				this.m_OutlineTextMaterial = null;
			}
			if (this.m_OutlineTextMaterial == null)
			{
				if (this.m_OutlineTextShader == null)
				{
					this.m_OutlineTextShader = ShaderUtils.FindShader(text);
					if (!this.m_OutlineTextShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + text);
					}
				}
				this.m_OutlineTextMaterial = new Material(this.m_OutlineTextShader);
				SceneUtils.SetHideFlags(this.m_OutlineTextMaterial, 52);
			}
			return this.m_OutlineTextMaterial;
		}
	}

	// Token: 0x17000210 RID: 528
	// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x00038CE0 File Offset: 0x00036EE0
	protected Material TextAntialiasingMaterial
	{
		get
		{
			if (this.m_TextAntialiasingMaterial == null)
			{
				if (this.m_AntialiasingTextShader == null)
				{
					this.m_AntialiasingTextShader = ShaderUtils.FindShader(this.TEXT_ANTIALAISING_SHADER_NAME);
					if (!this.m_AntialiasingTextShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.TEXT_ANTIALAISING_SHADER_NAME);
					}
				}
				this.m_TextAntialiasingMaterial = new Material(this.m_AntialiasingTextShader);
				SceneUtils.SetHideFlags(this.m_TextAntialiasingMaterial, 52);
			}
			return this.m_TextAntialiasingMaterial;
		}
	}

	// Token: 0x17000211 RID: 529
	// (get) Token: 0x06000DF7 RID: 3575 RVA: 0x00038D6C File Offset: 0x00036F6C
	protected Material ShadowMaterial
	{
		get
		{
			if (this.m_ShadowMaterial == null)
			{
				if (this.m_ShadowTextShader == null)
				{
					this.m_ShadowTextShader = ShaderUtils.FindShader(this.SHADOW_SHADER_NAME);
					if (!this.m_ShadowTextShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.SHADOW_SHADER_NAME);
					}
				}
				this.m_ShadowMaterial = new Material(this.m_ShadowTextShader);
				SceneUtils.SetHideFlags(this.m_ShadowMaterial, 52);
			}
			return this.m_ShadowMaterial;
		}
	}

	// Token: 0x17000212 RID: 530
	// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x00038DF8 File Offset: 0x00036FF8
	protected Material InlineImageMaterial
	{
		get
		{
			if (this.m_InlineImageMaterial == null)
			{
				if (this.m_InlineImageShader == null)
				{
					this.m_InlineImageShader = ShaderUtils.FindShader(this.INLINE_IMAGE_SHADER_NAME);
					if (!this.m_InlineImageShader)
					{
						Debug.LogError("UberText Failed to load Shader: " + this.INLINE_IMAGE_SHADER_NAME);
					}
				}
				this.m_InlineImageMaterial = new Material(this.m_InlineImageShader);
			}
			return this.m_InlineImageMaterial;
		}
	}

	// Token: 0x17000213 RID: 531
	// (get) Token: 0x06000DF9 RID: 3577 RVA: 0x00038E74 File Offset: 0x00037074
	// (set) Token: 0x06000DFA RID: 3578 RVA: 0x00038E7C File Offset: 0x0003707C
	[CustomEditField(Sections = "Text", T = EditType.TEXT_AREA)]
	public string Text
	{
		get
		{
			return this.m_Text;
		}
		set
		{
			this.m_TextSet = true;
			this.m_TextSet = true;
			if (value == this.m_Text)
			{
				return;
			}
			this.m_Text = (value ?? string.Empty);
			if (Enumerable.Any<char>(this.m_Text, (char c) => char.IsSurrogate(c)))
			{
				IEnumerable<char> enumerable = Enumerable.Select<char, char>(Enumerable.Where<char>(this.m_Text, (char c) => !char.IsLowSurrogate(c)), (char c) => (!char.IsHighSurrogate(c)) ? c : '�');
				this.m_Text = new string(Enumerable.ToArray<char>(enumerable));
			}
			if (this.m_Text == this.m_PreviousText)
			{
				return;
			}
			this.UpdateNow();
		}
	}

	// Token: 0x17000214 RID: 532
	// (get) Token: 0x06000DFB RID: 3579 RVA: 0x00038F62 File Offset: 0x00037162
	// (set) Token: 0x06000DFC RID: 3580 RVA: 0x00038F6A File Offset: 0x0003716A
	[CustomEditField(Sections = "Text")]
	public bool GameStringLookup
	{
		get
		{
			return this.m_GameStringLookup;
		}
		set
		{
			if (value == this.m_GameStringLookup)
			{
				return;
			}
			this.m_GameStringLookup = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x06000DFD RID: 3581 RVA: 0x00038F86 File Offset: 0x00037186
	// (set) Token: 0x06000DFE RID: 3582 RVA: 0x00038F8E File Offset: 0x0003718E
	[CustomEditField(Sections = "Text")]
	public bool UseEditorText
	{
		get
		{
			return this.m_UseEditorText;
		}
		set
		{
			if (value == this.m_UseEditorText)
			{
				return;
			}
			this.m_UseEditorText = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000216 RID: 534
	// (get) Token: 0x06000DFF RID: 3583 RVA: 0x00038FAA File Offset: 0x000371AA
	// (set) Token: 0x06000E00 RID: 3584 RVA: 0x00038FB2 File Offset: 0x000371B2
	[CustomEditField(Sections = "Text")]
	public bool Cache
	{
		get
		{
			return this.m_Cache;
		}
		set
		{
			this.m_Cache = value;
		}
	}

	// Token: 0x17000217 RID: 535
	// (get) Token: 0x06000E01 RID: 3585 RVA: 0x00038FBB File Offset: 0x000371BB
	// (set) Token: 0x06000E02 RID: 3586 RVA: 0x00038FC3 File Offset: 0x000371C3
	[CustomEditField(Sections = "Size")]
	public float Width
	{
		get
		{
			return this.m_Width;
		}
		set
		{
			if (value == this.m_Width)
			{
				return;
			}
			this.m_Width = value;
			if (this.m_Width < 0.01f)
			{
				this.m_Width = 0.01f;
			}
			this.UpdateText();
		}
	}

	// Token: 0x17000218 RID: 536
	// (get) Token: 0x06000E03 RID: 3587 RVA: 0x00038FFA File Offset: 0x000371FA
	// (set) Token: 0x06000E04 RID: 3588 RVA: 0x00039002 File Offset: 0x00037202
	[CustomEditField(Sections = "Size")]
	public float Height
	{
		get
		{
			return this.m_Height;
		}
		set
		{
			if (value == this.m_Height)
			{
				return;
			}
			this.m_Height = value;
			if (this.m_Height < 0.01f)
			{
				this.m_Height = 0.01f;
			}
			this.UpdateText();
		}
	}

	// Token: 0x17000219 RID: 537
	// (get) Token: 0x06000E05 RID: 3589 RVA: 0x00039039 File Offset: 0x00037239
	// (set) Token: 0x06000E06 RID: 3590 RVA: 0x00039041 File Offset: 0x00037241
	[CustomEditField(Sections = "Size")]
	public float LineSpacing
	{
		get
		{
			return this.m_LineSpacing;
		}
		set
		{
			if (value == this.m_LineSpacing)
			{
				return;
			}
			this.m_LineSpacing = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700021A RID: 538
	// (get) Token: 0x06000E07 RID: 3591 RVA: 0x0003905D File Offset: 0x0003725D
	// (set) Token: 0x06000E08 RID: 3592 RVA: 0x00039065 File Offset: 0x00037265
	[CustomEditField(Sections = "Style")]
	public Font TrueTypeFont
	{
		get
		{
			return this.m_Font;
		}
		set
		{
			if (value == this.m_Font)
			{
				return;
			}
			this.m_Font = value;
			this.SetFont(this.m_Font);
			this.UpdateText();
		}
	}

	// Token: 0x1700021B RID: 539
	// (get) Token: 0x06000E09 RID: 3593 RVA: 0x00039092 File Offset: 0x00037292
	// (set) Token: 0x06000E0A RID: 3594 RVA: 0x0003909C File Offset: 0x0003729C
	[CustomEditField(Sections = "Style")]
	public int FontSize
	{
		get
		{
			return this.m_FontSize;
		}
		set
		{
			if (value == this.m_FontSize)
			{
				return;
			}
			this.m_FontSize = value;
			if (this.m_FontSize < 1)
			{
				this.m_FontSize = 1;
			}
			if (this.m_FontSize > 120)
			{
				this.m_FontSize = 120;
			}
			this.UpdateText();
		}
	}

	// Token: 0x1700021C RID: 540
	// (get) Token: 0x06000E0B RID: 3595 RVA: 0x000390EB File Offset: 0x000372EB
	// (set) Token: 0x06000E0C RID: 3596 RVA: 0x000390F4 File Offset: 0x000372F4
	[CustomEditField(Sections = "Style")]
	public int MinFontSize
	{
		get
		{
			return this.m_MinFontSize;
		}
		set
		{
			if (value == this.m_MinFontSize)
			{
				return;
			}
			this.m_MinFontSize = value;
			if (this.m_MinFontSize < 1)
			{
				this.m_MinFontSize = 1;
			}
			if (this.m_MinFontSize > this.m_FontSize)
			{
				this.m_MinFontSize = this.m_FontSize;
			}
			this.UpdateText();
		}
	}

	// Token: 0x1700021D RID: 541
	// (get) Token: 0x06000E0D RID: 3597 RVA: 0x0003914B File Offset: 0x0003734B
	// (set) Token: 0x06000E0E RID: 3598 RVA: 0x00039153 File Offset: 0x00037353
	[CustomEditField(Sections = "Style")]
	public float CharacterSize
	{
		get
		{
			return this.m_CharacterSize;
		}
		set
		{
			if (value == this.m_CharacterSize)
			{
				return;
			}
			this.m_CharacterSize = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700021E RID: 542
	// (get) Token: 0x06000E0F RID: 3599 RVA: 0x0003916F File Offset: 0x0003736F
	// (set) Token: 0x06000E10 RID: 3600 RVA: 0x00039178 File Offset: 0x00037378
	[CustomEditField(Sections = "Style")]
	public float MinCharacterSize
	{
		get
		{
			return this.m_MinCharacterSize;
		}
		set
		{
			if (value == this.m_MinCharacterSize)
			{
				return;
			}
			this.m_MinCharacterSize = value;
			if (this.m_MinCharacterSize < 0.001f)
			{
				this.m_MinCharacterSize = 0.001f;
			}
			if (this.m_MinCharacterSize > this.m_CharacterSize)
			{
				this.m_MinCharacterSize = this.m_CharacterSize;
			}
			this.UpdateText();
		}
	}

	// Token: 0x1700021F RID: 543
	// (get) Token: 0x06000E11 RID: 3601 RVA: 0x000391D7 File Offset: 0x000373D7
	// (set) Token: 0x06000E12 RID: 3602 RVA: 0x000391DF File Offset: 0x000373DF
	[CustomEditField(Sections = "Style")]
	public bool RichText
	{
		get
		{
			return this.m_RichText;
		}
		set
		{
			if (value == this.m_RichText)
			{
				return;
			}
			this.m_RichText = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000220 RID: 544
	// (get) Token: 0x06000E13 RID: 3603 RVA: 0x000391FB File Offset: 0x000373FB
	// (set) Token: 0x06000E14 RID: 3604 RVA: 0x00039203 File Offset: 0x00037403
	[CustomEditField(Sections = "Style")]
	public Color TextColor
	{
		get
		{
			return this.m_TextColor;
		}
		set
		{
			if (value == this.m_TextColor)
			{
				return;
			}
			this.m_TextColor = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000221 RID: 545
	// (get) Token: 0x06000E15 RID: 3605 RVA: 0x00039224 File Offset: 0x00037424
	// (set) Token: 0x06000E16 RID: 3606 RVA: 0x00039234 File Offset: 0x00037434
	[CustomEditField(Hide = true)]
	public float TextAlpha
	{
		get
		{
			return this.m_TextColor.a;
		}
		set
		{
			if (value == this.m_TextColor.a)
			{
				return;
			}
			this.m_TextColor.a = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000222 RID: 546
	// (get) Token: 0x06000E17 RID: 3607 RVA: 0x00039265 File Offset: 0x00037465
	// (set) Token: 0x06000E18 RID: 3608 RVA: 0x0003926D File Offset: 0x0003746D
	[CustomEditField(Sections = "Style")]
	public float BoldSize
	{
		get
		{
			return this.m_BoldSize;
		}
		set
		{
			if (value == this.m_BoldSize)
			{
				return;
			}
			this.m_BoldSize = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000223 RID: 547
	// (get) Token: 0x06000E19 RID: 3609 RVA: 0x00039289 File Offset: 0x00037489
	// (set) Token: 0x06000E1A RID: 3610 RVA: 0x00039291 File Offset: 0x00037491
	[CustomEditField(Sections = "Paragraph")]
	public bool WordWrap
	{
		get
		{
			return this.m_WordWrap;
		}
		set
		{
			if (value == this.m_WordWrap)
			{
				return;
			}
			this.m_WordWrap = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000224 RID: 548
	// (get) Token: 0x06000E1B RID: 3611 RVA: 0x000392AD File Offset: 0x000374AD
	// (set) Token: 0x06000E1C RID: 3612 RVA: 0x000392B5 File Offset: 0x000374B5
	[CustomEditField(Sections = "Paragraph")]
	public bool ForceWrapLargeWords
	{
		get
		{
			return this.m_ForceWrapLargeWords;
		}
		set
		{
			if (value == this.m_ForceWrapLargeWords)
			{
				return;
			}
			this.m_ForceWrapLargeWords = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000225 RID: 549
	// (get) Token: 0x06000E1D RID: 3613 RVA: 0x000392D1 File Offset: 0x000374D1
	// (set) Token: 0x06000E1E RID: 3614 RVA: 0x000392D9 File Offset: 0x000374D9
	[CustomEditField(Sections = "Paragraph")]
	public bool ResizeToFit
	{
		get
		{
			return this.m_ResizeToFit;
		}
		set
		{
			if (value == this.m_ResizeToFit)
			{
				return;
			}
			this.m_ResizeToFit = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000226 RID: 550
	// (get) Token: 0x06000E1F RID: 3615 RVA: 0x000392F5 File Offset: 0x000374F5
	// (set) Token: 0x06000E20 RID: 3616 RVA: 0x000392FD File Offset: 0x000374FD
	[CustomEditField(Sections = "Underwear", Label = "Enable")]
	public bool Underwear
	{
		get
		{
			return this.m_Underwear;
		}
		set
		{
			if (value == this.m_Underwear)
			{
				return;
			}
			this.m_Underwear = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000227 RID: 551
	// (get) Token: 0x06000E21 RID: 3617 RVA: 0x00039319 File Offset: 0x00037519
	// (set) Token: 0x06000E22 RID: 3618 RVA: 0x00039321 File Offset: 0x00037521
	[CustomEditField(Parent = "Underwear", Label = "Flip")]
	public bool UnderwearFlip
	{
		get
		{
			return this.m_UnderwearFlip;
		}
		set
		{
			if (value == this.m_UnderwearFlip)
			{
				return;
			}
			this.m_UnderwearFlip = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000228 RID: 552
	// (get) Token: 0x06000E23 RID: 3619 RVA: 0x0003933D File Offset: 0x0003753D
	// (set) Token: 0x06000E24 RID: 3620 RVA: 0x00039345 File Offset: 0x00037545
	[CustomEditField(Parent = "Underwear", Label = "Width")]
	public float UnderwearWidth
	{
		get
		{
			return this.m_UnderwearWidth;
		}
		set
		{
			if (value == this.m_UnderwearWidth)
			{
				return;
			}
			this.m_UnderwearWidth = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000229 RID: 553
	// (get) Token: 0x06000E25 RID: 3621 RVA: 0x00039361 File Offset: 0x00037561
	// (set) Token: 0x06000E26 RID: 3622 RVA: 0x00039369 File Offset: 0x00037569
	[CustomEditField(Parent = "Underwear", Label = "Height")]
	public float UnderwearHeight
	{
		get
		{
			return this.m_UnderwearHeight;
		}
		set
		{
			if (value == this.m_UnderwearHeight)
			{
				return;
			}
			this.m_UnderwearHeight = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022A RID: 554
	// (get) Token: 0x06000E27 RID: 3623 RVA: 0x00039385 File Offset: 0x00037585
	// (set) Token: 0x06000E28 RID: 3624 RVA: 0x0003938D File Offset: 0x0003758D
	[CustomEditField(Sections = "Alignment", Label = "Enable")]
	public UberText.AlignmentOptions Alignment
	{
		get
		{
			return this.m_Alignment;
		}
		set
		{
			if (value == this.m_Alignment)
			{
				return;
			}
			this.m_Alignment = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022B RID: 555
	// (get) Token: 0x06000E29 RID: 3625 RVA: 0x000393A9 File Offset: 0x000375A9
	// (set) Token: 0x06000E2A RID: 3626 RVA: 0x000393B1 File Offset: 0x000375B1
	[CustomEditField(Parent = "Alignment")]
	public UberText.AnchorOptions Anchor
	{
		get
		{
			return this.m_Anchor;
		}
		set
		{
			if (value == this.m_Anchor)
			{
				return;
			}
			this.m_Anchor = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022C RID: 556
	// (get) Token: 0x06000E2B RID: 3627 RVA: 0x000393CD File Offset: 0x000375CD
	// (set) Token: 0x06000E2C RID: 3628 RVA: 0x000393D5 File Offset: 0x000375D5
	[CustomEditField(Sections = "Render/Bake")]
	public bool RenderToTexture
	{
		get
		{
			return this.m_RenderToTexture;
		}
		set
		{
			if (value == this.m_RenderToTexture)
			{
				return;
			}
			this.m_RenderToTexture = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022D RID: 557
	// (get) Token: 0x06000E2D RID: 3629 RVA: 0x000393F1 File Offset: 0x000375F1
	// (set) Token: 0x06000E2E RID: 3630 RVA: 0x000393F9 File Offset: 0x000375F9
	[CustomEditField(Sections = "Render/Bake")]
	public GameObject RenderOnObject
	{
		get
		{
			return this.m_RenderOnObject;
		}
		set
		{
			if (value == this.m_RenderOnObject)
			{
				return;
			}
			this.m_RenderOnObject = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06000E2F RID: 3631 RVA: 0x0003941A File Offset: 0x0003761A
	// (set) Token: 0x06000E30 RID: 3632 RVA: 0x00039422 File Offset: 0x00037622
	[CustomEditField(Parent = "RenderToTexture")]
	public int TextureResolution
	{
		get
		{
			return this.m_Resolution;
		}
		set
		{
			if (value == this.m_Resolution)
			{
				return;
			}
			this.m_Resolution = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700022F RID: 559
	// (get) Token: 0x06000E31 RID: 3633 RVA: 0x0003943E File Offset: 0x0003763E
	// (set) Token: 0x06000E32 RID: 3634 RVA: 0x00039446 File Offset: 0x00037646
	[CustomEditField(Sections = "Outline", Label = "Enable")]
	public bool Outline
	{
		get
		{
			return this.m_Outline;
		}
		set
		{
			if (value == this.m_Outline)
			{
				return;
			}
			this.m_Outline = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000230 RID: 560
	// (get) Token: 0x06000E33 RID: 3635 RVA: 0x00039462 File Offset: 0x00037662
	// (set) Token: 0x06000E34 RID: 3636 RVA: 0x0003946A File Offset: 0x0003766A
	[CustomEditField(Parent = "Outline", Label = "Size")]
	public float OutlineSize
	{
		get
		{
			return this.m_OutlineSize;
		}
		set
		{
			if (value == this.m_OutlineSize)
			{
				return;
			}
			this.m_OutlineSize = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000231 RID: 561
	// (get) Token: 0x06000E35 RID: 3637 RVA: 0x00039486 File Offset: 0x00037686
	// (set) Token: 0x06000E36 RID: 3638 RVA: 0x0003948E File Offset: 0x0003768E
	[CustomEditField(Parent = "Outline", Label = "Color")]
	public Color OutlineColor
	{
		get
		{
			return this.m_OutlineColor;
		}
		set
		{
			if (value == this.m_OutlineColor)
			{
				return;
			}
			this.m_OutlineColor = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000232 RID: 562
	// (get) Token: 0x06000E37 RID: 3639 RVA: 0x000394AF File Offset: 0x000376AF
	// (set) Token: 0x06000E38 RID: 3640 RVA: 0x000394BC File Offset: 0x000376BC
	[CustomEditField(Hide = true)]
	public float OutlineAlpha
	{
		get
		{
			return this.m_OutlineColor.a;
		}
		set
		{
			if (value == this.m_OutlineColor.a)
			{
				return;
			}
			this.m_OutlineColor.a = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000233 RID: 563
	// (get) Token: 0x06000E39 RID: 3641 RVA: 0x000394ED File Offset: 0x000376ED
	// (set) Token: 0x06000E3A RID: 3642 RVA: 0x000394F5 File Offset: 0x000376F5
	[CustomEditField(Parent = "RenderToTexture")]
	public bool AntiAlias
	{
		get
		{
			return this.m_AntiAlias;
		}
		set
		{
			if (value == this.m_AntiAlias)
			{
				return;
			}
			this.m_AntiAlias = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000234 RID: 564
	// (get) Token: 0x06000E3B RID: 3643 RVA: 0x00039511 File Offset: 0x00037711
	// (set) Token: 0x06000E3C RID: 3644 RVA: 0x00039519 File Offset: 0x00037719
	[CustomEditField(Parent = "AntiAlias")]
	public float AntiAliasAmount
	{
		get
		{
			return this.m_AntiAliasAmount;
		}
		set
		{
			if (value == this.m_AntiAliasAmount)
			{
				return;
			}
			this.m_AntiAliasAmount = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000235 RID: 565
	// (get) Token: 0x06000E3D RID: 3645 RVA: 0x00039535 File Offset: 0x00037735
	// (set) Token: 0x06000E3E RID: 3646 RVA: 0x0003953D File Offset: 0x0003773D
	[CustomEditField(Sections = "Localization")]
	public UberText.LocalizationSettings LocalizeSettings
	{
		get
		{
			return this.m_LocalizedSettings;
		}
		set
		{
			this.m_LocalizedSettings = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x06000E3F RID: 3647 RVA: 0x0003954C File Offset: 0x0003774C
	// (set) Token: 0x06000E40 RID: 3648 RVA: 0x00039554 File Offset: 0x00037754
	[CustomEditField(Parent = "AntiAlias")]
	public float AntiAliasEdge
	{
		get
		{
			return this.m_AntiAliasEdge;
		}
		set
		{
			if (value == this.m_AntiAliasEdge)
			{
				return;
			}
			this.m_AntiAliasEdge = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06000E41 RID: 3649 RVA: 0x00039570 File Offset: 0x00037770
	// (set) Token: 0x06000E42 RID: 3650 RVA: 0x00039578 File Offset: 0x00037778
	[CustomEditField(Sections = "Shadow", Label = "Enable")]
	public bool Shadow
	{
		get
		{
			return this.m_Shadow;
		}
		set
		{
			if (value == this.m_Shadow)
			{
				return;
			}
			this.m_Shadow = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06000E43 RID: 3651 RVA: 0x00039594 File Offset: 0x00037794
	// (set) Token: 0x06000E44 RID: 3652 RVA: 0x0003959C File Offset: 0x0003779C
	[CustomEditField(Parent = "Shadow")]
	public float ShadowOffset
	{
		get
		{
			return this.m_ShadowOffset;
		}
		set
		{
			if (value == this.m_ShadowOffset)
			{
				return;
			}
			this.m_ShadowOffset = value;
			this.UpdateText();
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06000E45 RID: 3653 RVA: 0x000395B8 File Offset: 0x000377B8
	// (set) Token: 0x06000E46 RID: 3654 RVA: 0x000395C0 File Offset: 0x000377C0
	[CustomEditField(Parent = "Shadow")]
	public float ShadowBlur
	{
		get
		{
			return this.m_ShadowBlur;
		}
		set
		{
			if (value == this.m_ShadowBlur)
			{
				return;
			}
			this.m_ShadowBlur = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06000E47 RID: 3655 RVA: 0x000395DC File Offset: 0x000377DC
	// (set) Token: 0x06000E48 RID: 3656 RVA: 0x000395E4 File Offset: 0x000377E4
	[CustomEditField(Parent = "Shadow")]
	public Color ShadowColor
	{
		get
		{
			return this.m_ShadowColor;
		}
		set
		{
			if (value == this.m_ShadowColor)
			{
				return;
			}
			this.m_ShadowColor = value;
			this.UpdateColor();
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06000E49 RID: 3657 RVA: 0x00039605 File Offset: 0x00037805
	// (set) Token: 0x06000E4A RID: 3658 RVA: 0x00039614 File Offset: 0x00037814
	[CustomEditField(Parent = "Shadow")]
	public float ShadowAlpha
	{
		get
		{
			return this.m_ShadowColor.a;
		}
		set
		{
			if (value == this.m_ShadowColor.a)
			{
				return;
			}
			this.m_ShadowColor.a = value;
			this.UpdateColor();
		}
	}

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06000E4B RID: 3659 RVA: 0x00039645 File Offset: 0x00037845
	// (set) Token: 0x06000E4C RID: 3660 RVA: 0x0003964D File Offset: 0x0003784D
	[CustomEditField(Sections = "Render")]
	public int RenderQueue
	{
		get
		{
			return this.m_RenderQueue;
		}
		set
		{
			if (value == this.m_RenderQueue)
			{
				return;
			}
			this.m_RenderQueue = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06000E4D RID: 3661 RVA: 0x00039669 File Offset: 0x00037869
	// (set) Token: 0x06000E4E RID: 3662 RVA: 0x00039671 File Offset: 0x00037871
	[CustomEditField(Sections = "Render")]
	public float AmbientLightBlend
	{
		get
		{
			return this.m_AmbientLightBlend;
		}
		set
		{
			if (value == this.m_AmbientLightBlend)
			{
				return;
			}
			this.m_AmbientLightBlend = value;
			this.UpdateText();
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06000E4F RID: 3663 RVA: 0x0003968D File Offset: 0x0003788D
	// (set) Token: 0x06000E50 RID: 3664 RVA: 0x00039695 File Offset: 0x00037895
	[CustomEditField(Parent = "RenderToTexture")]
	public Color GradientUpperColor
	{
		get
		{
			return this.m_GradientUpperColor;
		}
		set
		{
			if (value == this.m_GradientUpperColor)
			{
				return;
			}
			this.m_GradientUpperColor = value;
			this.UpdateColor();
		}
	}

	// Token: 0x1700023F RID: 575
	// (get) Token: 0x06000E51 RID: 3665 RVA: 0x000396B6 File Offset: 0x000378B6
	// (set) Token: 0x06000E52 RID: 3666 RVA: 0x000396C4 File Offset: 0x000378C4
	[CustomEditField(Hide = true)]
	public float GradientUpperAlpha
	{
		get
		{
			return this.m_GradientUpperColor.a;
		}
		set
		{
			if (value == this.m_GradientUpperColor.a)
			{
				return;
			}
			this.m_GradientUpperColor.a = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06000E53 RID: 3667 RVA: 0x000396F5 File Offset: 0x000378F5
	// (set) Token: 0x06000E54 RID: 3668 RVA: 0x000396FD File Offset: 0x000378FD
	[CustomEditField(Parent = "RenderToTexture")]
	public Color GradientLowerColor
	{
		get
		{
			return this.m_GradientLowerColor;
		}
		set
		{
			if (value == this.m_GradientLowerColor)
			{
				return;
			}
			this.m_GradientLowerColor = value;
			this.UpdateColor();
		}
	}

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x06000E55 RID: 3669 RVA: 0x0003971E File Offset: 0x0003791E
	// (set) Token: 0x06000E56 RID: 3670 RVA: 0x0003972C File Offset: 0x0003792C
	[CustomEditField(Hide = true)]
	public float GradientLowerAlpha
	{
		get
		{
			return this.m_GradientLowerColor.a;
		}
		set
		{
			if (value == this.m_GradientLowerColor.a)
			{
				return;
			}
			this.m_GradientLowerColor.a = value;
			this.UpdateColor();
		}
	}

	// Token: 0x06000E57 RID: 3671 RVA: 0x00039760 File Offset: 0x00037960
	private void Awake()
	{
		if (!this.m_GameStringLookup && !this.m_TextSet && !this.m_UseEditorText)
		{
			this.m_Text = string.Empty;
		}
		this.FindSupportedTextureFormat();
		if (!UberText.s_InlineImageTexture)
		{
			UberText.s_InlineImageTexture = new Texture2D(2, 2, 5, false);
			UberText.s_InlineImageTexture.SetPixel(0, 0, Color.clear);
			UberText.s_InlineImageTexture.SetPixel(1, 0, Color.clear);
			UberText.s_InlineImageTexture.SetPixel(0, 1, Color.clear);
			UberText.s_InlineImageTexture.SetPixel(1, 1, Color.clear);
			UberText.s_InlineImageTexture.Apply();
		}
		this.DestroyChildren();
	}

	// Token: 0x06000E58 RID: 3672 RVA: 0x00039810 File Offset: 0x00037A10
	private void Start()
	{
		this.m_updated = false;
	}

	// Token: 0x06000E59 RID: 3673 RVA: 0x0003981C File Offset: 0x00037A1C
	private void Update()
	{
		if (this.m_RenderToTexture && this.m_TextTexture && !this.m_TextTexture.IsCreated())
		{
			Log.Kyle.Print("UberText Texture lost 1. UpdateText Called", new object[0]);
			this.m_updated = false;
			this.RenderText();
			return;
		}
		this.RenderText();
		this.UpdateTexelSize();
	}

	// Token: 0x06000E5A RID: 3674 RVA: 0x00039883 File Offset: 0x00037A83
	private void OnDisable()
	{
		if (this.m_RenderOnObject)
		{
			this.m_RenderOnObject.GetComponent<Renderer>().enabled = false;
		}
	}

	// Token: 0x06000E5B RID: 3675 RVA: 0x000398A6 File Offset: 0x00037AA6
	private void OnDestroy()
	{
		this.CleanUp();
	}

	// Token: 0x06000E5C RID: 3676 RVA: 0x000398AE File Offset: 0x00037AAE
	private void OnEnable()
	{
		this.m_updated = false;
		this.SetFont(this.m_Font);
		this.UpdateNow();
	}

	// Token: 0x06000E5D RID: 3677 RVA: 0x000398CC File Offset: 0x00037ACC
	private void OnDrawGizmos()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = new Color(0.3f, 0.3f, 0.35f, 0.2f);
		Gizmos.DrawCube(Vector3.zero, new Vector3(this.m_Width + this.m_Width * 0.02f, this.m_Height + this.m_Height * 0.02f, 0f));
		Gizmos.color = Color.black;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.m_Width, this.m_Height, 0f));
		if (this.m_Underwear)
		{
			float num = this.m_Width * this.m_UnderwearWidth * 0.5f;
			float num2 = this.m_Height * this.m_UnderwearHeight;
			if (this.m_UnderwearFlip)
			{
				Vector3 vector;
				vector..ctor(-(this.m_Width * 0.5f - num * 0.5f), this.m_Height * 0.5f - num2 * 0.5f, 0f);
				Gizmos.DrawWireCube(vector, new Vector3(num, num2, 0f));
				Vector3 vector2;
				vector2..ctor(this.m_Width * 0.5f - num * 0.5f, this.m_Height * 0.5f - num2 * 0.5f, 0f);
				Gizmos.DrawWireCube(vector2, new Vector3(num, num2, 0f));
			}
			else
			{
				Vector3 vector3;
				vector3..ctor(-(this.m_Width * 0.5f - num * 0.5f), -(this.m_Height * 0.5f - num2 * 0.5f), 0f);
				Gizmos.DrawWireCube(vector3, new Vector3(num, num2, 0f));
				Vector3 vector4;
				vector4..ctor(this.m_Width * 0.5f - num * 0.5f, -(this.m_Height * 0.5f - num2 * 0.5f), 0f);
				Gizmos.DrawWireCube(vector4, new Vector3(num, num2, 0f));
			}
		}
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06000E5E RID: 3678 RVA: 0x00039AD0 File Offset: 0x00037CD0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(this.m_Width + this.m_Width * 0.04f, this.m_Height + this.m_Height * 0.04f, 0f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Token: 0x06000E5F RID: 3679 RVA: 0x00039B3B File Offset: 0x00037D3B
	public void Show()
	{
		this.m_Hidden = false;
		this.UpdateText();
	}

	// Token: 0x06000E60 RID: 3680 RVA: 0x00039B4A File Offset: 0x00037D4A
	public void Hide()
	{
		this.m_Hidden = true;
		this.UpdateText();
	}

	// Token: 0x06000E61 RID: 3681 RVA: 0x00039B59 File Offset: 0x00037D59
	public bool isHidden()
	{
		return this.m_Hidden;
	}

	// Token: 0x06000E62 RID: 3682 RVA: 0x00039B61 File Offset: 0x00037D61
	public void EditorAwake()
	{
		this.DestroyChildren();
		this.UpdateText();
	}

	// Token: 0x06000E63 RID: 3683 RVA: 0x00039B6F File Offset: 0x00037D6F
	public bool IsDone()
	{
		return this.m_updated;
	}

	// Token: 0x06000E64 RID: 3684 RVA: 0x00039B77 File Offset: 0x00037D77
	public void UpdateText()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.m_updated = false;
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x00039B91 File Offset: 0x00037D91
	public void UpdateNow()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.m_updated = false;
		this.RenderText();
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00039BB4 File Offset: 0x00037DB4
	public Bounds GetBounds()
	{
		Matrix4x4 localToWorldMatrix = base.transform.localToWorldMatrix;
		Vector3 vector = localToWorldMatrix.MultiplyVector(Vector3.up) * (this.m_Height * 0.5f);
		Vector3 vector2 = localToWorldMatrix.MultiplyVector(Vector3.right) * (this.m_Width * 0.5f);
		Bounds result = default(Bounds);
		result.min = base.transform.position - vector2 + vector;
		result.max = base.transform.position + vector2 - vector;
		return result;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00039C50 File Offset: 0x00037E50
	public Bounds GetTextBounds()
	{
		if (!this.m_updated)
		{
			this.UpdateNow();
		}
		Bounds bounds;
		bounds..ctor(Vector3.zero, Vector3.zero);
		if (this.m_TextMesh)
		{
			Quaternion rotation = base.transform.rotation;
			base.transform.rotation = Quaternion.identity;
			bounds = this.m_TextMesh.GetComponent<Renderer>().bounds;
			base.transform.rotation = rotation;
		}
		return bounds;
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x00039CCC File Offset: 0x00037ECC
	public Bounds GetTextWorldSpaceBounds()
	{
		if (!this.m_updated)
		{
			this.UpdateNow();
		}
		Bounds bounds;
		bounds..ctor(Vector3.zero, Vector3.zero);
		if (this.m_TextMesh)
		{
			bounds = this.m_TextMesh.GetComponent<Renderer>().bounds;
		}
		return bounds;
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x00039D20 File Offset: 0x00037F20
	public Vector3 GetLocalizationPositionOffset()
	{
		Vector3 result = Vector3.zero;
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				result = locale.m_PositionOffset;
			}
		}
		return result;
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x00039D5D File Offset: 0x00037F5D
	public int GetLineCount()
	{
		return this.m_LineCount;
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x00039D65 File Offset: 0x00037F65
	public float GetActualCharacterSize()
	{
		return this.m_TextMesh.characterSize / this.m_CharacterSizeModifier / 0.01f;
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x00039D7F File Offset: 0x00037F7F
	public bool IsMultiLine()
	{
		return this.m_LineCount > 1;
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x00039D8A File Offset: 0x00037F8A
	public bool IsEllipsized()
	{
		return this.m_Ellipsized;
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x00039D92 File Offset: 0x00037F92
	public void SetGameStringText(string gameStringTag)
	{
		this.Text = GameStrings.Get(gameStringTag);
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x00039DA0 File Offset: 0x00037FA0
	public Font GetLocalizedFont()
	{
		if (this.m_LocalizedFont)
		{
			return this.m_LocalizedFont;
		}
		return this.m_Font;
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x00039DBF File Offset: 0x00037FBF
	public UberText.LocalizationSettings.LocaleAdjustment AddLocaleAdjustment(Locale locale)
	{
		return this.m_LocalizedSettings.AddLocale(locale);
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x00039DCD File Offset: 0x00037FCD
	public UberText.LocalizationSettings.LocaleAdjustment GetLocaleAdjustment(Locale locale)
	{
		return this.m_LocalizedSettings.GetLocale(locale);
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x00039DDB File Offset: 0x00037FDB
	public void RemoveLocaleAdjustment(Locale locale)
	{
		this.m_LocalizedSettings.RemoveLocale(locale);
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x00039DE9 File Offset: 0x00037FE9
	public UberText.LocalizationSettings GetAllLocalizationSettings()
	{
		return this.m_LocalizedSettings;
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x00039DF4 File Offset: 0x00037FF4
	public void SetFontWithoutLocalization(FontDef fontDef)
	{
		Font font = fontDef.m_Font;
		if (font == null)
		{
			return;
		}
		if (this.m_TextMesh != null && this.m_TextMesh.font == font)
		{
			return;
		}
		this.m_Font = font;
		this.m_LocalizedFont = this.m_Font;
		this.m_LineSpaceModifier = fontDef.m_LineSpaceModifier;
		this.m_FontSizeModifier = fontDef.m_FontSizeModifier;
		this.m_SingleLineAdjustment = fontDef.m_SingleLineAdjustment;
		this.m_CharacterSizeModifier = fontDef.m_CharacterSizeModifier;
		this.m_OutlineModifier = fontDef.m_OutlineModifier;
		this.m_isFontDefLoaded = true;
		this.m_FontTexture = font.material.mainTexture;
		this.UpdateFontTextures();
		if (this.m_TextMesh != null)
		{
			Object.DestroyImmediate(this.m_TextMesh);
		}
		this.UpdateNow();
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00039ECC File Offset: 0x000380CC
	private void RenderText()
	{
		if (this.m_updated && (!this.m_TextTexture || this.m_TextTexture.IsCreated()))
		{
			return;
		}
		if (this.m_Font == null)
		{
			Debug.LogWarning(string.Format("UberText error: Font is null for {0}", base.gameObject.name));
			return;
		}
		if (this.m_Text == null || this.m_Text == string.Empty)
		{
			if (this.m_TextMesh)
			{
				this.m_TextMesh.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_PlaneGameObject)
			{
				this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_RenderOnObject)
			{
				this.m_RenderOnObject.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_ShadowPlaneGameObject)
			{
				this.m_ShadowPlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
			return;
		}
		if (this.m_TextMesh)
		{
			this.m_TextMesh.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_PlaneGameObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_RenderOnObject)
		{
			this.m_RenderOnObject.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_ShadowPlaneGameObject)
		{
			this.m_ShadowPlaneGameObject.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_Hidden)
		{
			if (this.m_TextMesh)
			{
				this.m_TextMesh.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_PlaneGameObject)
			{
				this.m_PlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_RenderOnObject)
			{
				this.m_RenderOnObject.GetComponent<Renderer>().enabled = false;
			}
			if (this.m_ShadowPlaneGameObject)
			{
				this.m_ShadowPlaneGameObject.GetComponent<Renderer>().enabled = false;
			}
			return;
		}
		if (this.m_TextMesh)
		{
			this.m_TextMesh.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_PlaneGameObject)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_RenderOnObject)
		{
			this.m_RenderOnObject.GetComponent<Renderer>().enabled = true;
		}
		if (this.m_ShadowPlaneGameObject)
		{
			this.m_ShadowPlaneGameObject.GetComponent<Renderer>().enabled = true;
		}
		Vector2 worldWidthAndHight = this.GetWorldWidthAndHight();
		this.m_WorldWidth = worldWidthAndHight.x;
		this.m_WorldHeight = worldWidthAndHight.y;
		this.CreateTextMesh();
		if (this.m_TextMesh == null)
		{
			return;
		}
		this.UpdateTextMesh();
		if (this.m_Outline)
		{
			this.OutlineRender();
		}
		if (this.m_RenderToTexture)
		{
			this.CreateCamera();
			this.CreateTexture();
			if (this.m_RenderOnObject)
			{
				this.SetupRenderOnObject();
			}
			else
			{
				this.CreateRenderPlane();
			}
			this.SetupForRender();
			if (!this.m_RenderOnObject)
			{
				this.ShadowRender();
			}
			this.UpdateTexelSize();
			if (this.m_TextTexture == null)
			{
				Debug.LogWarning("UberText Render to Texture m_TextTexture is null!");
				this.m_updated = false;
				return;
			}
			if (this.m_Camera.targetTexture != this.m_TextTexture)
			{
				this.m_Camera.targetTexture = this.m_TextTexture;
			}
			this.m_Camera.Render();
			if (!this.m_TextTexture.IsCreated())
			{
				Debug.LogWarning("UberText Render to Texture m_TextTexture.IsCreated() == false after render!");
				this.m_updated = false;
				return;
			}
			this.AntiAliasRender();
		}
		this.UpdateLayers();
		this.UpdateRenderQueue();
		this.UpdateColor();
		if (this.m_RenderOnObject)
		{
			this.m_RenderOnObject.GetComponent<Renderer>().enabled = true;
		}
		this.m_PreviousText = this.m_Text;
		this.m_updated = true;
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x0003A2E0 File Offset: 0x000384E0
	private void UpdateTextMesh()
	{
		string text = string.Empty;
		bool flag = false;
		bool flag2 = false;
		if (!flag)
		{
			this.m_CacheHash = new UberText.CachedTextKeyData
			{
				m_Text = this.m_Text,
				m_CharSize = this.m_CharacterSize,
				m_Font = this.m_Font,
				m_FontSize = this.m_FontSize,
				m_Height = this.m_Height,
				m_Width = this.m_Width,
				m_LineSpacing = this.m_LineSpacing
			}.GetHashCode();
			if (this.m_Cache && (this.m_WordWrap || this.m_ResizeToFit) && UberText.s_CachedText.ContainsKey(this.m_CacheHash))
			{
				UberText.CachedTextValues cachedTextValues = UberText.s_CachedText[this.m_CacheHash];
				if (cachedTextValues.m_OriginalTextHash == this.m_Text.GetHashCode())
				{
					text = cachedTextValues.m_Text;
					this.SetText(text);
					this.SetActualCharacterSize(cachedTextValues.m_CharSize);
					flag2 = true;
				}
			}
		}
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		if (!flag2)
		{
			string text2 = this.m_Text;
			text = string.Empty;
			if (this.m_GameStringLookup)
			{
				text2 = GameStrings.Get(text2.Trim());
			}
			if (Localization.GetLocale() != Locale.enUS)
			{
				text2 = this.LocalizationFixes(text2);
			}
			text = this.ProcessText(text2);
			this.m_Words = this.BreakStringIntoWords(text);
			this.m_LineCount = UberText.LineCount(text);
			this.m_Ellipsized = false;
			if (this.m_WordWrap && !this.m_ResizeToFit)
			{
				this.SetText(this.WordWrapString(text, this.m_WorldWidth));
			}
			else
			{
				this.SetText(text);
			}
			this.SetActualCharacterSize(this.m_CharacterSize * this.m_CharacterSizeModifier * 0.01f);
		}
		this.m_TextMesh.GetComponent<Renderer>().enabled = true;
		this.SetFont(this.m_Font);
		this.SetFontSize(this.m_FontSize);
		this.SetLineSpacing(this.m_LineSpacing);
		switch (this.m_Alignment)
		{
		case UberText.AlignmentOptions.Left:
			this.m_TextMesh.alignment = 0;
			switch (this.m_Anchor)
			{
			case UberText.AnchorOptions.Upper:
				this.m_TextMesh.transform.localPosition = new Vector3(-this.m_Width * 0.5f, this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 0;
				break;
			case UberText.AnchorOptions.Middle:
				this.m_TextMesh.transform.localPosition = new Vector3(-this.m_Width * 0.5f, 0f, 0f);
				this.m_TextMesh.anchor = 3;
				break;
			case UberText.AnchorOptions.Lower:
				this.m_TextMesh.transform.localPosition = new Vector3(-this.m_Width * 0.5f, -this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 6;
				break;
			}
			break;
		case UberText.AlignmentOptions.Center:
			this.m_TextMesh.alignment = 1;
			switch (this.m_Anchor)
			{
			case UberText.AnchorOptions.Upper:
				this.m_TextMesh.transform.localPosition = new Vector3(0f, this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 1;
				break;
			case UberText.AnchorOptions.Middle:
				this.m_TextMesh.transform.localPosition = new Vector3(0f, 0f, 0f);
				this.m_TextMesh.anchor = 4;
				break;
			case UberText.AnchorOptions.Lower:
				this.m_TextMesh.transform.localPosition = new Vector3(0f, -this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 7;
				break;
			}
			break;
		case UberText.AlignmentOptions.Right:
			this.m_TextMesh.alignment = 2;
			switch (this.m_Anchor)
			{
			case UberText.AnchorOptions.Upper:
				this.m_TextMesh.transform.localPosition = new Vector3(this.m_Width * 0.5f, this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 2;
				break;
			case UberText.AnchorOptions.Middle:
				this.m_TextMesh.transform.localPosition = new Vector3(this.m_Width * 0.5f, 0f, 0f);
				this.m_TextMesh.anchor = 5;
				break;
			case UberText.AnchorOptions.Lower:
				this.m_TextMesh.transform.localPosition = new Vector3(this.m_Width * 0.5f, -this.m_Height * 0.5f, 0f);
				this.m_TextMesh.anchor = 8;
				break;
			}
			break;
		}
		if (this.m_ResizeToFit && !flag2)
		{
			this.ResizeTextToFit(text);
		}
		base.transform.rotation = rotation;
		if (!flag && this.m_Cache && !flag2 && (this.m_WordWrap || this.m_ResizeToFit))
		{
			double num = 0.0;
			if (!double.TryParse(this.m_Text, ref num) && this.m_Text.Length > 3)
			{
				UberText.s_CachedText[this.m_CacheHash] = new UberText.CachedTextValues();
				UberText.s_CachedText[this.m_CacheHash].m_Text = this.m_TextMesh.text;
				UberText.s_CachedText[this.m_CacheHash].m_CharSize = this.m_TextMesh.characterSize;
				UberText.s_CachedText[this.m_CacheHash].m_OriginalTextHash = this.m_Text.GetHashCode();
			}
		}
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				this.m_TextMesh.transform.localPosition += locale.m_PositionOffset;
			}
		}
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x0003A910 File Offset: 0x00038B10
	private void UpdateColor()
	{
		if (this.m_Outline)
		{
			if (this.m_OutlineTextMaterial)
			{
				this.m_OutlineTextMaterial.SetColor("_Color", this.m_TextColor);
				this.m_OutlineTextMaterial.SetColor("_OutlineColor", this.m_OutlineColor);
				this.m_OutlineTextMaterial.SetFloat("_LightingBlend", this.m_AmbientLightBlend);
			}
			if (this.m_BoldMaterial)
			{
				this.m_BoldMaterial.SetColor("_Color", this.m_TextColor);
				this.m_BoldMaterial.SetColor("_OutlineColor", this.m_OutlineColor);
				this.m_BoldMaterial.SetFloat("_LightingBlend", this.m_AmbientLightBlend);
			}
		}
		else
		{
			if (this.m_TextMaterial)
			{
				this.m_TextMaterial.SetColor("_Color", this.m_TextColor);
				this.m_TextMaterial.SetFloat("_LightingBlend", this.m_AmbientLightBlend);
			}
			if (this.m_BoldMaterial)
			{
				this.m_BoldMaterial.SetColor("_Color", this.m_TextColor);
				this.m_BoldMaterial.SetFloat("_LightingBlend", this.m_AmbientLightBlend);
			}
		}
		if (this.m_Shadow && this.m_ShadowMaterial)
		{
			this.m_ShadowMaterial.SetColor("_Color", this.m_ShadowColor);
		}
		if (this.m_PlaneMesh)
		{
			this.m_PlaneMesh.colors = new Color[]
			{
				this.m_GradientLowerColor,
				this.m_GradientLowerColor,
				this.m_GradientUpperColor,
				this.m_GradientUpperColor
			};
		}
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x0003AAE4 File Offset: 0x00038CE4
	private void UpdateFontTextures()
	{
		if (this.m_TextMaterial)
		{
			this.m_TextMaterial.mainTexture = this.m_FontTexture;
		}
		if (this.m_OutlineTextMaterial)
		{
			this.m_OutlineTextMaterial.mainTexture = this.m_FontTexture;
		}
		if (this.m_BoldMaterial)
		{
			this.m_BoldMaterial.mainTexture = this.m_FontTexture;
		}
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x0003AB54 File Offset: 0x00038D54
	private void UpdateLayers()
	{
		if (this.m_RenderToTexture)
		{
			this.m_TextMeshGameObject.layer = 0;
			if (this.m_PlaneGameObject)
			{
				this.m_PlaneGameObject.layer = base.gameObject.layer;
			}
		}
		else if (this.m_TextMeshGameObject)
		{
			this.m_TextMeshGameObject.layer = base.gameObject.layer;
		}
		if (this.m_Shadow && this.m_ShadowPlaneGameObject)
		{
			this.m_ShadowPlaneGameObject.layer = base.gameObject.layer;
		}
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x0003ABFC File Offset: 0x00038DFC
	private void UpdateRenderQueue()
	{
		GameObject gameObject;
		if (this.m_RenderToTexture)
		{
			if (this.m_RenderOnObject)
			{
				gameObject = this.m_RenderOnObject;
			}
			else
			{
				gameObject = this.m_PlaneGameObject;
			}
		}
		else
		{
			gameObject = this.m_TextMeshGameObject;
		}
		if (gameObject == null)
		{
			return;
		}
		if (this.m_OrgRenderQueue == -9999)
		{
			this.m_OrgRenderQueue = gameObject.GetComponent<Renderer>().sharedMaterial.renderQueue;
		}
		foreach (Material material in gameObject.GetComponent<Renderer>().sharedMaterials)
		{
			material.renderQueue = this.m_OrgRenderQueue + this.m_RenderQueue;
		}
		if (this.m_Shadow && this.m_ShadowPlaneGameObject)
		{
			this.m_ShadowPlaneGameObject.GetComponent<Renderer>().sharedMaterial.renderQueue = gameObject.GetComponent<Renderer>().sharedMaterial.renderQueue - 1;
		}
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x0003ACF0 File Offset: 0x00038EF0
	private void UpdateTexelSize()
	{
		float num = this.m_OutlineSize * this.m_OutlineModifier;
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				num = this.m_OutlineSize * this.m_OutlineModifier * locale.m_OutlineModifier;
			}
		}
		if (this.m_FontTexture == null)
		{
			this.m_FontTexture = this.GetFontTexture();
		}
		if (this.m_FontTexture == null)
		{
			if (!Application.isPlaying)
			{
				return;
			}
			Debug.LogError(string.Format("UberText.UpdateTexelSize() - m_FontTexture == null!  text={0}", this.m_Text));
			return;
		}
		else
		{
			Vector2 vector = this.TexelSize(this.m_FontTexture);
			if (vector == this.m_PreviousTexelSize)
			{
				return;
			}
			if (this.m_BoldMaterial != null)
			{
				this.m_BoldMaterial.SetFloat("_BoldOffsetX", this.m_BoldSize * vector.x);
				this.m_BoldMaterial.SetFloat("_BoldOffsetY", this.m_BoldSize * vector.y);
			}
			if (this.m_Outline && !this.m_RenderToTexture)
			{
				if (this.m_OutlineTextMaterial != null)
				{
					this.m_OutlineTextMaterial.SetFloat("_OutlineOffsetX", vector.x * num);
					this.m_OutlineTextMaterial.SetFloat("_OutlineOffsetY", vector.y * num);
					this.m_OutlineTextMaterial.SetFloat("_TexelSizeX", vector.x);
					this.m_OutlineTextMaterial.SetFloat("_TexelSizeY", vector.y);
				}
				if (this.m_BoldMaterial != null)
				{
					this.m_BoldMaterial.SetFloat("_BoldOffsetX", this.m_BoldSize * vector.x);
					this.m_BoldMaterial.SetFloat("_BoldOffsetY", this.m_BoldSize * vector.y);
					this.m_BoldMaterial.SetFloat("_OutlineOffsetX", vector.x * num);
					this.m_BoldMaterial.SetFloat("_OutlineOffsetY", vector.y * num);
				}
			}
			if (this.m_Shadow && this.m_RenderToTexture && this.m_ShadowMaterial != null)
			{
				this.m_ShadowMaterial.SetFloat("_OffsetX", vector.x * this.m_ShadowBlur);
				this.m_ShadowMaterial.SetFloat("_OffsetY", vector.y * this.m_ShadowBlur);
			}
			if (this.m_AntiAlias && this.m_RenderToTexture && this.m_TextAntialiasingMaterial != null)
			{
				this.m_TextAntialiasingMaterial.SetFloat("_OffsetX", vector.x * this.m_AntiAliasAmount);
				this.m_TextAntialiasingMaterial.SetFloat("_OffsetY", vector.y * this.m_AntiAliasAmount);
			}
			this.m_PreviousTexelSize = vector;
			return;
		}
	}

	// Token: 0x06000E7C RID: 3708 RVA: 0x0003AFD0 File Offset: 0x000391D0
	private void CreateTextMesh()
	{
		if (!this.m_TextMeshGameObject)
		{
			this.m_TextMeshGameObject = new GameObject();
			this.m_TextMeshGameObject.name = "UberText_RenderObject_" + base.name;
			SceneUtils.SetHideFlags(this.m_TextMeshGameObject, 61);
		}
		else
		{
			TextMesh component = this.m_TextMeshGameObject.GetComponent<TextMesh>();
			if (component)
			{
				this.SetText(string.Empty);
			}
		}
		if (this.m_RenderToTexture)
		{
			Vector3 position;
			position..ctor(-3000f, 3000f, this.Offset);
			this.m_TextMeshGameObject.transform.parent = null;
			this.m_TextMeshGameObject.transform.position = position;
			this.m_TextMeshGameObject.transform.rotation = Quaternion.identity;
		}
		else
		{
			this.m_TextMeshGameObject.transform.parent = base.transform;
			this.m_TextMeshGameObject.transform.localPosition = Vector3.zero;
			this.m_TextMeshGameObject.transform.localRotation = Quaternion.identity;
			this.m_TextMeshGameObject.transform.localScale = Vector3.one;
		}
		if (!this.m_TextMesh)
		{
			this.m_TextMaterialIndices.Clear();
			if (this.m_TextMeshGameObject == null)
			{
				return;
			}
			MeshRenderer component2 = this.m_TextMeshGameObject.GetComponent<MeshRenderer>();
			if (component2 == null)
			{
				this.m_TextMeshGameObject.AddComponent<MeshRenderer>();
			}
			TextMesh component3 = this.m_TextMeshGameObject.GetComponent<TextMesh>();
			if (component3)
			{
				this.m_TextMesh = component3;
			}
			else
			{
				this.m_TextMesh = this.m_TextMeshGameObject.AddComponent<TextMesh>();
			}
			if (this.m_TextMesh == null)
			{
				Debug.LogError("UberText: Faild to create TextMesh");
				return;
			}
			this.SetRichText(this.m_RichText);
			Texture fontTexture = this.GetFontTexture();
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterial = this.TextMaterial;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterial.mainTexture = fontTexture;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterial.color = this.m_TextColor;
			if (this.m_RichText)
			{
				Material[] array = new Material[2];
				array[0] = this.m_TextMesh.GetComponent<Renderer>().sharedMaterial;
				this.m_TextMaterialIndices.Add(UberText.TextRenderMaterial.Text, 0);
				array[1] = this.BoldMaterial;
				array[1].mainTexture = fontTexture;
				this.m_TextMaterialIndices.Add(UberText.TextRenderMaterial.Bold, 1);
				this.m_TextMesh.GetComponent<Renderer>().sharedMaterials = array;
			}
			else
			{
				Material[] sharedMaterials = new Material[]
				{
					this.m_TextMesh.GetComponent<Renderer>().sharedMaterial
				};
				this.m_TextMaterialIndices.Add(UberText.TextRenderMaterial.Text, 0);
				this.m_TextMesh.GetComponent<Renderer>().sharedMaterials = sharedMaterials;
			}
		}
		if (!this.m_Outline && this.m_TextMesh.GetComponent<Renderer>().sharedMaterial == this.m_OutlineTextMaterial)
		{
			Texture mainTexture = this.m_TextMesh.GetComponent<Renderer>().sharedMaterial.mainTexture;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterial = this.TextMaterial;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterial.mainTexture = mainTexture;
		}
		this.SetFont(this.m_Font);
		this.SetFontSize(this.m_FontSize);
		this.SetLineSpacing(this.m_LineSpacing);
		this.SetActualCharacterSize(this.m_CharacterSize * this.m_CharacterSizeModifier * 0.01f);
		if (this.m_Text == null)
		{
			this.SetText(string.Empty);
		}
		else
		{
			this.SetText(this.m_Text);
		}
	}

	// Token: 0x06000E7D RID: 3709 RVA: 0x0003B374 File Offset: 0x00039574
	private void SetFont(Font font)
	{
		if (font == null)
		{
			return;
		}
		if (!this.m_isFontDefLoaded)
		{
			FontTable fontTable = FontTable.Get();
			if (fontTable != null)
			{
				FontDef fontDef = fontTable.GetFontDef(font);
				if (fontDef != null)
				{
					this.m_LocalizedFont = fontDef.m_Font;
					this.m_LineSpaceModifier = fontDef.m_LineSpaceModifier;
					this.m_FontSizeModifier = fontDef.m_FontSizeModifier;
					this.m_SingleLineAdjustment = fontDef.m_SingleLineAdjustment;
					this.m_CharacterSizeModifier = fontDef.m_CharacterSizeModifier;
					this.m_OutlineModifier = fontDef.m_OutlineModifier;
					this.m_isFontDefLoaded = true;
				}
				else
				{
					Debug.LogError("Error loading fontDef for:" + base.name);
				}
			}
		}
		if (this.m_TextMesh == null)
		{
			return;
		}
		if (this.m_TextMesh.font == font)
		{
			return;
		}
		if (this.m_LocalizedFont == null)
		{
			this.m_TextMesh.font = this.m_Font;
		}
		else
		{
			this.m_TextMesh.font = this.m_LocalizedFont;
		}
		this.m_FontTexture = this.m_TextMesh.font.material.mainTexture;
		this.UpdateFontTextures();
	}

	// Token: 0x06000E7E RID: 3710 RVA: 0x0003B4AC File Offset: 0x000396AC
	private void SetFontSize(int fontSize)
	{
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				fontSize = (int)(locale.m_FontSizeModifier * (float)fontSize);
			}
		}
		fontSize = (int)(this.m_FontSizeModifier * (float)fontSize);
		if (this.m_TextMesh.fontSize != fontSize)
		{
			this.m_TextMesh.fontSize = fontSize;
		}
	}

	// Token: 0x06000E7F RID: 3711 RVA: 0x0003B510 File Offset: 0x00039710
	private void SetLineSpacing(float lineSpacing)
	{
		int num = UberText.LineCount(this.m_TextMesh.text);
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				if (num == 1)
				{
					lineSpacing += locale.m_SingleLineAdjustment;
				}
				else
				{
					lineSpacing *= locale.m_LineSpaceModifier;
				}
			}
		}
		if (num == 1)
		{
			lineSpacing += this.m_SingleLineAdjustment;
		}
		else
		{
			lineSpacing *= this.m_LineSpaceModifier;
		}
		if (this.m_TextMesh.lineSpacing == lineSpacing)
		{
			return;
		}
		this.m_TextMesh.lineSpacing = lineSpacing;
	}

	// Token: 0x06000E80 RID: 3712 RVA: 0x0003B5AE File Offset: 0x000397AE
	private void SetActualCharacterSize(float characterSize)
	{
		if (this.m_TextMesh.characterSize != characterSize)
		{
			this.m_TextMesh.characterSize = characterSize;
		}
	}

	// Token: 0x06000E81 RID: 3713 RVA: 0x0003B5D0 File Offset: 0x000397D0
	private void SetText(string text)
	{
		if (this.m_TextMesh.text != text)
		{
			this.m_TextMesh.text = text;
		}
	}

	// Token: 0x06000E82 RID: 3714 RVA: 0x0003B5FF File Offset: 0x000397FF
	private void SetRichText(bool richText)
	{
		if (this.m_TextMesh.richText != richText)
		{
			this.m_TextMesh.richText = richText;
		}
	}

	// Token: 0x06000E83 RID: 3715 RVA: 0x0003B620 File Offset: 0x00039820
	private Texture GetFontTexture()
	{
		if (!(this.m_LocalizedFont == null))
		{
			return this.m_LocalizedFont.material.mainTexture;
		}
		if (this.m_Font == null)
		{
			return null;
		}
		return this.m_Font.material.mainTexture;
	}

	// Token: 0x06000E84 RID: 3716 RVA: 0x0003B672 File Offset: 0x00039872
	private void DestroyTextMesh()
	{
		if (this.m_TextMeshGameObject)
		{
			Object.DestroyImmediate(this.m_TextMeshGameObject);
		}
	}

	// Token: 0x06000E85 RID: 3717 RVA: 0x0003B68F File Offset: 0x0003988F
	private void CreateEditorRoot()
	{
	}

	// Token: 0x06000E86 RID: 3718 RVA: 0x0003B694 File Offset: 0x00039894
	private void CreateCamera()
	{
		if (this.m_Camera != null)
		{
			return;
		}
		this.m_CameraGO = new GameObject();
		this.m_Camera = this.m_CameraGO.AddComponent<Camera>();
		this.m_CameraGO.name = "UberText_RenderCamera_" + base.name;
		SceneUtils.SetHideFlags(this.m_CameraGO, 61);
		this.m_Camera.orthographic = true;
		this.m_CameraGO.transform.parent = this.m_TextMeshGameObject.transform;
		this.m_CameraGO.transform.rotation = Quaternion.identity;
		this.m_CameraGO.transform.position = this.m_TextMeshGameObject.transform.position;
		this.m_Camera.nearClipPlane = -0.1f;
		this.m_Camera.farClipPlane = 0.1f;
		if (Camera.main)
		{
			this.m_Camera.depth = Camera.main.depth - 50f;
		}
		Color color = this.m_TextColor;
		if (this.m_Outline)
		{
			color = this.m_OutlineColor;
		}
		this.m_Camera.backgroundColor = new Color(color.r, color.g, color.b, 0f);
		this.m_Camera.clearFlags = 2;
		this.m_Camera.depthTextureMode = 0;
		this.m_Camera.renderingPath = 1;
		this.m_Camera.cullingMask = UberText.RENDER_LAYER_BIT;
		this.m_Camera.enabled = false;
	}

	// Token: 0x06000E87 RID: 3719 RVA: 0x0003B820 File Offset: 0x00039A20
	private void DestroyCamera()
	{
		if (!this.m_CameraGO)
		{
			return;
		}
		this.m_Camera.targetTexture = null;
		Object.Destroy(this.m_CameraGO);
	}

	// Token: 0x06000E88 RID: 3720 RVA: 0x0003B858 File Offset: 0x00039A58
	private void CreateTexture()
	{
		Vector2 vector = this.CalcTextureSize();
		if (this.m_TextTexture != null)
		{
			if (this.m_Camera.targetTexture == null)
			{
				this.m_Camera.targetTexture = this.m_TextTexture;
			}
			if (this.m_TextTexture.width == (int)vector.x && this.m_TextTexture.height == (int)vector.y)
			{
				return;
			}
		}
		this.DestroyTexture();
		this.m_TextTexture = new RenderTexture((int)vector.x, (int)vector.y, 0, UberText.s_TextureFormat);
		SceneUtils.SetHideFlags(this.m_TextTexture, 61);
		if (this.m_Camera)
		{
			this.m_Camera.targetTexture = this.m_TextTexture;
		}
		if (this.m_PlaneGameObject && this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial)
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_TextTexture;
		}
		this.m_PreviousResolution = this.m_Resolution;
	}

	// Token: 0x06000E89 RID: 3721 RVA: 0x0003B97B File Offset: 0x00039B7B
	private void DestroyTexture()
	{
		if (this.m_TextTexture != null)
		{
			Object.Destroy(this.m_TextTexture);
		}
	}

	// Token: 0x06000E8A RID: 3722 RVA: 0x0003B99C File Offset: 0x00039B9C
	private void CreateRenderPlane()
	{
		if (this.m_PlaneMesh != null && this.m_Width == this.m_PreviousPlaneWidth && this.m_Height == this.m_PreviousPlaneHeight && this.m_PreviousResolution == this.m_Resolution)
		{
			return;
		}
		if (this.m_PlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_PlaneGameObject);
		}
		this.m_PlaneGameObject = new GameObject();
		this.m_PlaneGameObject.name = "UberText_RenderPlane_" + base.name;
		this.m_PlaneGameObject.AddComponent<MeshFilter>();
		this.m_PlaneGameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		SceneUtils.SetHideFlags(this.m_PlaneGameObject, 52);
		this.m_PlaneGameObject.transform.parent = base.transform;
		this.m_PlaneGameObject.transform.position = base.transform.position;
		this.m_PlaneGameObject.transform.rotation = base.transform.rotation;
		this.m_PlaneGameObject.transform.Rotate(-90f, 0f, 0f);
		this.m_PlaneGameObject.transform.localScale = Vector3.one;
		float num = this.m_Width * 0.5f;
		float num2 = this.m_Height * 0.5f;
		mesh.vertices = new Vector3[]
		{
			new Vector3(-num, 0f, -num2),
			new Vector3(num, 0f, -num2),
			new Vector3(-num, 0f, num2),
			new Vector3(num, 0f, num2)
		};
		mesh.colors = new Color[]
		{
			this.m_GradientLowerColor,
			this.m_GradientLowerColor,
			this.m_GradientUpperColor,
			this.m_GradientUpperColor
		};
		mesh.uv = this.PLANE_UVS;
		mesh.normals = this.PLANE_NORMALS;
		mesh.triangles = this.PLANE_TRIANGLES;
		Mesh mesh2 = mesh;
		this.m_PlaneGameObject.GetComponent<MeshFilter>().mesh = mesh2;
		this.m_PlaneMesh = mesh2;
		this.m_PlaneMesh.RecalculateBounds();
		this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.PlaneMaterial;
		this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_TextTexture;
		this.m_PreviousPlaneWidth = this.m_Width;
		this.m_PreviousPlaneHeight = this.m_Height;
	}

	// Token: 0x06000E8B RID: 3723 RVA: 0x0003BC4C File Offset: 0x00039E4C
	private void DestroyRenderPlane()
	{
		if (this.m_PlaneGameObject != null)
		{
			MeshFilter component = this.m_PlaneGameObject.GetComponent<MeshFilter>();
			if (component != null)
			{
				Object.DestroyImmediate(component.sharedMesh);
				Object.DestroyImmediate(component);
			}
			Object.DestroyImmediate(this.m_PlaneGameObject);
		}
	}

	// Token: 0x06000E8C RID: 3724 RVA: 0x0003BCA0 File Offset: 0x00039EA0
	private void SetupRenderOnObject()
	{
		if (!this.m_RenderOnObject)
		{
			return;
		}
		this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial = this.PlaneMaterial;
		this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_TextTexture;
	}

	// Token: 0x06000E8D RID: 3725 RVA: 0x0003BCF0 File Offset: 0x00039EF0
	private void ResizeTextToFit(string text)
	{
		if (text == null || text == string.Empty)
		{
			return;
		}
		Transform parent = this.m_TextMeshGameObject.transform.parent;
		Quaternion rotation = this.m_TextMeshGameObject.transform.rotation;
		Vector3 localScale = this.m_TextMeshGameObject.transform.localScale;
		this.m_TextMeshGameObject.transform.parent = null;
		this.m_TextMeshGameObject.transform.localScale = Vector3.one;
		this.m_TextMeshGameObject.transform.rotation = Quaternion.identity;
		float width = this.m_Width;
		string text2 = this.RemoveTagsFromWord(text);
		if (text2 == null)
		{
			text2 = string.Empty;
		}
		this.SetText(text2);
		if (this.m_WordWrap)
		{
			this.SetText(this.WordWrapString(text, width));
		}
		this.ReduceText_CharSize(text);
		this.m_TextMeshGameObject.transform.parent = parent;
		this.m_TextMeshGameObject.transform.localScale = localScale;
		this.m_TextMeshGameObject.transform.rotation = rotation;
		if (!this.m_WordWrap)
		{
			this.SetText(text);
		}
	}

	// Token: 0x06000E8E RID: 3726 RVA: 0x0003BE10 File Offset: 0x0003A010
	private void ReduceText(string text, int step, int newSize)
	{
		if (this.m_FontSize == 1)
		{
			return;
		}
		this.SetFontSize(newSize);
		float num = this.m_Height;
		float num2 = this.m_Width;
		if (!this.m_RenderToTexture)
		{
			num = this.m_WorldHeight;
			num2 = this.m_WorldWidth;
		}
		if (!this.IsMultiLine())
		{
			this.SetLineSpacing(0f);
		}
		float y = this.m_TextMesh.GetComponent<Renderer>().bounds.size.y;
		float x = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
		int num3 = 0;
		while (y > num || x > num2)
		{
			num3++;
			if (num3 > 40)
			{
				break;
			}
			newSize -= step;
			if (newSize < this.m_MinFontSize)
			{
				newSize = this.m_MinFontSize;
				break;
			}
			this.SetFontSize(newSize);
			if (this.m_WordWrap)
			{
				this.SetText(this.WordWrapString(text, num2));
			}
			y = this.m_TextMesh.GetComponent<Renderer>().bounds.size.y;
			x = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
		}
		if (!this.IsMultiLine())
		{
			this.SetLineSpacing(this.m_LineSpacing);
		}
		this.m_FontSize = newSize;
	}

	// Token: 0x06000E8F RID: 3727 RVA: 0x0003BF80 File Offset: 0x0003A180
	private void ReduceText_CharSize(string text)
	{
		float height = this.m_Height;
		float width = this.m_Width;
		float num = this.m_TextMesh.characterSize;
		if (!this.IsMultiLine())
		{
			this.SetLineSpacing(0f);
		}
		else
		{
			this.SetLineSpacing(this.m_LineSpacing);
		}
		float x = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
		float y = this.m_TextMesh.GetComponent<Renderer>().bounds.size.y;
		int num2 = 0;
		while (y > height || x > width)
		{
			num2++;
			if (num2 > 40)
			{
				break;
			}
			num *= 0.95f;
			if (num <= this.m_MinCharacterSize * 0.01f)
			{
				num = this.m_MinCharacterSize * 0.01f;
				this.SetActualCharacterSize(num);
				if (this.m_WordWrap)
				{
					this.SetText(this.WordWrapString(text, width, true));
				}
				break;
			}
			this.SetActualCharacterSize(num);
			if (this.m_WordWrap)
			{
				this.SetText(this.WordWrapString(text, width, false));
			}
			if (UberText.LineCount(this.m_TextMesh.text) > 1)
			{
				this.SetLineSpacing(this.m_LineSpacing);
			}
			else
			{
				this.SetLineSpacing(0f);
			}
			x = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
			y = this.m_TextMesh.GetComponent<Renderer>().bounds.size.y;
		}
		this.SetLineSpacing(this.m_LineSpacing);
	}

	// Token: 0x06000E90 RID: 3728 RVA: 0x0003C134 File Offset: 0x0003A334
	private void SetupForRender()
	{
		if (this.m_RenderToTexture)
		{
			Vector3 position;
			position..ctor(-3000f, 3000f, this.Offset);
			this.m_TextMeshGameObject.transform.parent = null;
			this.m_TextMeshGameObject.transform.position = position;
			this.m_TextMeshGameObject.transform.rotation = Quaternion.identity;
			this.m_TextMeshGameObject.transform.localScale = Vector3.one;
			this.m_TextMeshGameObject.layer = UberText.RENDER_LAYER;
			float num = -3000f;
			if (this.Alignment == UberText.AlignmentOptions.Left)
			{
				num += this.m_Width * 0.5f;
			}
			if (this.Alignment == UberText.AlignmentOptions.Right)
			{
				num -= this.m_Width * 0.5f;
			}
			float num2 = 0f;
			if (this.m_Anchor == UberText.AnchorOptions.Upper)
			{
				num2 += this.m_Height * 0.5f;
			}
			if (this.m_Anchor == UberText.AnchorOptions.Lower)
			{
				num2 -= this.m_Height * 0.5f;
			}
			Vector3 position2;
			position2..ctor(num, 3000f - num2, this.Offset);
			this.m_CameraGO.transform.parent = this.m_TextMeshGameObject.transform;
			this.m_CameraGO.transform.position = position2;
			Color color = this.m_TextColor;
			if (this.m_Outline)
			{
				color = this.m_OutlineColor;
			}
			this.m_Camera.backgroundColor = new Color(color.r, color.g, color.b, 0f);
			this.m_Camera.orthographicSize = this.m_Height * 0.5f;
			if (!this.RenderOnObject && this.m_PlaneGameObject != null)
			{
				this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.mainTexture = this.m_TextTexture;
			}
		}
		else
		{
			this.m_TextMeshGameObject.transform.parent = base.transform;
			this.m_TextMeshGameObject.transform.localPosition = Vector3.zero;
			this.m_CameraGO.transform.position = base.transform.position;
		}
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x0003C359 File Offset: 0x0003A559
	private string WordWrapString(string text, float width)
	{
		return this.WordWrapString(text, width, false);
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x0003C364 File Offset: 0x0003A564
	private string WordWrapString(string text, float width, bool ellipsis)
	{
		if (text == null || text == string.Empty)
		{
			return string.Empty;
		}
		float num = width;
		float num2 = 0f;
		float num3 = 0f;
		Bounds bounds = default(Bounds);
		Bounds bounds2 = default(Bounds);
		Quaternion rotation = this.m_TextMeshGameObject.transform.rotation;
		Vector3 position = this.m_TextMeshGameObject.transform.position;
		Vector3 localScale = this.m_TextMeshGameObject.transform.localScale;
		this.m_TextMeshGameObject.transform.rotation = Quaternion.identity;
		this.m_TextMeshGameObject.transform.position = new Vector3(0f, this.m_Height * 0.25f, 0f);
		this.m_TextMeshGameObject.transform.localScale = Vector3.one;
		if (this.m_Underwear)
		{
			num2 = this.m_UnderwearHeight;
			num3 = this.m_UnderwearWidth;
			if (this.m_LocalizedSettings != null)
			{
				UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
				if (locale != null)
				{
					num3 = locale.m_UnderwearWidth;
					num2 = locale.m_UnderwearHeight;
				}
			}
			if (this.m_UnderwearFlip)
			{
				num2 = this.m_Height * num2;
			}
			else
			{
				num2 = this.m_Height * (1f - num2);
			}
			num3 = width * (1f - num3);
			Vector3 size;
			size..ctor(this.m_Width * this.m_UnderwearWidth * 0.5f, this.m_Height * this.m_UnderwearHeight * 0.5f, 1f);
			float num4 = 0f;
			if (this.m_UnderwearFlip)
			{
				num4 = num4 + this.m_Height * 0.5f - this.m_Height * this.m_UnderwearHeight * 0.5f;
			}
			else
			{
				num4 = num4 - this.m_Height * 0.5f + this.m_Height * this.m_UnderwearHeight * 0.5f;
			}
			Vector3 zero = Vector3.zero;
			zero.x = zero.x + this.m_Width * 0.5f - this.m_Width * 0.5f * this.m_UnderwearWidth * 0.5f;
			zero.y = num4;
			bounds.center = zero;
			bounds.size = size;
			Vector3 zero2 = Vector3.zero;
			zero2.x = zero2.x - this.m_Width * 0.5f + this.m_Width * 0.5f * this.m_UnderwearWidth * 0.5f;
			zero2.y = num4;
			bounds2.center = zero2;
			bounds2.size = size;
		}
		TextAnchor anchor = this.m_TextMesh.anchor;
		if (this.m_Underwear)
		{
			this.m_TextMesh.anchor = 1;
		}
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		string[] array = this.m_Words;
		if (array == null)
		{
			return text;
		}
		if ((!this.m_ResizeToFit || ellipsis) && this.m_ForceWrapLargeWords)
		{
			List<string> list = new List<string>();
			foreach (string text2 in array)
			{
				this.SetText(text2);
				float x = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
				if (x < width)
				{
					list.Add(text2);
				}
				else
				{
					int num5 = Mathf.CeilToInt(x / width);
					int start = 0;
					int num6 = 1;
					for (int j = 0; j < num5; j++)
					{
						this.SetText(text2.Slice(start, num6));
						while (this.m_TextMesh.GetComponent<Renderer>().bounds.size.x < width && num6 < text2.Length)
						{
							num6++;
							this.SetText(text2.Slice(start, num6));
						}
						list.Add(text2.Slice(start, num6 - 1));
						start = num6 - 1;
					}
					list.Add(text2.Slice(start, text2.Length));
				}
			}
			array = list.ToArray();
		}
		int num7 = 0;
		if (text.Contains("\n"))
		{
			for (int k = 0; k < text.Length; k++)
			{
				byte b = (byte)text.get_Chars(k);
				if (b == 10)
				{
					num7++;
				}
			}
		}
		bool flag = false;
		if (this.m_Underwear && !this.m_UnderwearFlip)
		{
			flag = true;
		}
		if (this.m_Underwear && this.m_UnderwearFlip)
		{
			StringBuilder stringBuilder3 = new StringBuilder();
			StringBuilder stringBuilder4 = new StringBuilder();
			foreach (string text3 in array)
			{
				string text4 = this.RemoveTagsFromWord(text3);
				stringBuilder4.Append(text4);
				string text5 = stringBuilder4.ToString();
				if (text5 == null)
				{
					Debug.LogWarning("UberText: actualLine is null in WordWrapString!");
					text5 = string.Empty;
				}
				this.SetText(text5);
				float x2 = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
				if (x2 >= width)
				{
					flag = true;
					break;
				}
				stringBuilder3.Append(text3);
			}
			if (stringBuilder3.ToString().Contains("\n"))
			{
				flag = true;
			}
		}
		foreach (string text6 in array)
		{
			string text7 = this.RemoveTagsFromWord(text6);
			stringBuilder2.Append(text7);
			string text8 = stringBuilder2.ToString();
			if (text8 == null)
			{
				Debug.LogWarning("UberText: actualLine is null in WordWrapString!");
				text8 = string.Empty;
			}
			this.SetText(text8);
			float x3 = this.m_TextMesh.GetComponent<Renderer>().bounds.size.x;
			if (this.m_Underwear && flag)
			{
				this.SetText(stringBuilder.ToString());
				float y = this.m_TextMesh.GetComponent<Renderer>().bounds.size.y;
				if (this.m_UnderwearFlip)
				{
					if (y - (this.m_Height - y) * 0.2f < num2)
					{
						width = num3;
					}
					else
					{
						width = num;
					}
				}
				else if (this.m_TextMesh.GetComponent<Renderer>().bounds.Intersects(bounds) || this.m_TextMesh.GetComponent<Renderer>().bounds.Intersects(bounds2))
				{
					width = num3;
				}
			}
			if (x3 < width)
			{
				stringBuilder.Append(text6);
			}
			else
			{
				if (ellipsis)
				{
					this.SetText(stringBuilder.ToString() + '\n');
					if (this.m_TextMesh.GetComponent<Renderer>().bounds.size.y > this.m_Height)
					{
						this.m_Ellipsized = true;
						stringBuilder.Append(" ...");
						break;
					}
				}
				if (stringBuilder.Length > 2 && stringBuilder.ToString().get_Chars(stringBuilder.Length - 1) == ']' && stringBuilder.ToString().get_Chars(stringBuilder.Length - 2) == 'd' && stringBuilder.ToString().get_Chars(stringBuilder.Length - 3) == '[')
				{
					stringBuilder.Append('-');
				}
				num7++;
				stringBuilder.Append('\n');
				stringBuilder.Append(text6.TrimStart(new char[]
				{
					' '
				}));
				stringBuilder2 = new StringBuilder();
				for (int n = 0; n < this.m_LineCount; n++)
				{
					stringBuilder2.Append("\n");
				}
				stringBuilder2.Append(text7);
			}
		}
		this.m_TextMeshGameObject.transform.rotation = rotation;
		this.m_TextMeshGameObject.transform.position = position;
		this.m_TextMeshGameObject.transform.localScale = localScale;
		this.m_TextMesh.anchor = anchor;
		string text9 = this.RemoveLineBreakTagsHardSpace(stringBuilder.ToString());
		if (text9 == null)
		{
			Debug.LogWarning("UberText: Word Wrap returned a null string!");
			text9 = string.Empty;
		}
		this.m_LineCount = UberText.LineCount(text9);
		return text9;
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x0003CBD4 File Offset: 0x0003ADD4
	private string ProcessText(string text)
	{
		if (!this.m_RichText)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<material=1></material>");
		stringBuilder.Append(text);
		for (int i = 0; i < text.Length; i++)
		{
			if (text.get_Chars(i) == '<')
			{
				if (i <= text.Length - 2)
				{
					if (text.get_Chars(i + 1) == 'b')
					{
						if (i + 3 >= text.Length || text.get_Chars(i + 3) != '<')
						{
							this.Bold();
							if (this.m_TextMesh.GetComponent<Renderer>().sharedMaterials.Length < 1)
							{
								Debug.LogWarning("UberText: Tried to set Bold material, but material missing!");
							}
							else
							{
								stringBuilder.Replace("<b>", "<material=1>");
								stringBuilder.Replace("</b>", "</material>");
								i++;
							}
						}
					}
					else if (text.get_Chars(i + 1) == 'm')
					{
						if (i <= text.Length - 3)
						{
							if (text.get_Chars(i + 2) != 'a')
							{
								int num = text.Substring(i).IndexOf('>');
								if (num < 1)
								{
									i++;
								}
								else
								{
									string text2 = text.Substring(i, num + 1);
									stringBuilder.Replace(text2, this.InlineImage(text2));
								}
							}
						}
					}
				}
			}
		}
		string text3 = stringBuilder.ToString();
		if (text3 == null)
		{
			Debug.LogWarning("UberText: ProcessText returned a null string!");
			text3 = string.Empty;
		}
		return text3;
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x0003CD63 File Offset: 0x0003AF63
	private string LocalizationFixes(string text)
	{
		if (Localization.GetLocale() == Locale.thTH)
		{
			return this.FixThai(text);
		}
		return text;
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0003CD7C File Offset: 0x0003AF7C
	private void ShadowRender()
	{
		if (!this.m_Shadow)
		{
			this.DestroyShadow();
			return;
		}
		if (this.m_PlaneGameObject == null)
		{
			return;
		}
		if (this.m_ShadowPlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_ShadowPlaneGameObject);
		}
		this.m_ShadowPlaneGameObject = new GameObject();
		this.m_ShadowPlaneGameObject.name = "UberText_ShadowPlane_" + base.name;
		this.m_ShadowPlaneGameObject.AddComponent<MeshFilter>();
		this.m_ShadowPlaneGameObject.AddComponent<MeshRenderer>();
		Mesh mesh = new Mesh();
		SceneUtils.SetHideFlags(this.m_ShadowPlaneGameObject, 52);
		this.m_ShadowPlaneGameObject.transform.parent = this.m_PlaneGameObject.transform;
		this.m_ShadowPlaneGameObject.transform.localRotation = Quaternion.identity;
		this.m_ShadowPlaneGameObject.transform.localScale = Vector3.one;
		float num = -this.m_ShadowOffset * 0.01f;
		this.m_ShadowPlaneGameObject.transform.localPosition = new Vector3(num, 0f, num);
		float num2 = this.m_Width * 0.5f;
		float num3 = this.m_Height * 0.5f;
		mesh.vertices = new Vector3[]
		{
			new Vector3(-num2, 0f, -num3),
			new Vector3(num2, 0f, -num3),
			new Vector3(-num2, 0f, num3),
			new Vector3(num2, 0f, num3)
		};
		mesh.uv = this.PLANE_UVS;
		mesh.normals = this.PLANE_NORMALS;
		mesh.triangles = this.PLANE_TRIANGLES;
		Mesh mesh2 = mesh;
		this.m_ShadowPlaneGameObject.GetComponent<MeshFilter>().mesh = mesh2;
		Mesh mesh3 = mesh2;
		mesh3.RecalculateBounds();
		this.m_ShadowPlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.ShadowMaterial;
		this.m_ShadowMaterial.mainTexture = this.m_TextTexture;
		Vector2 vector = this.TexelSize(this.m_TextTexture);
		this.m_ShadowMaterial.SetColor("_Color", this.m_ShadowColor);
		this.m_ShadowMaterial.SetFloat("_OffsetX", vector.x * this.m_ShadowBlur);
		this.m_ShadowMaterial.SetFloat("_OffsetY", vector.y * this.m_ShadowBlur);
	}

	// Token: 0x06000E96 RID: 3734 RVA: 0x0003CFDE File Offset: 0x0003B1DE
	private void DestroyShadow()
	{
		if (this.m_ShadowPlaneGameObject != null)
		{
			Object.DestroyImmediate(this.m_ShadowPlaneGameObject);
		}
	}

	// Token: 0x06000E97 RID: 3735 RVA: 0x0003CFFC File Offset: 0x0003B1FC
	private void OutlineRender()
	{
		if (this.m_TextMesh == null)
		{
			Debug.LogError(string.Format("UberText OutlineRender ({0}, {1}): m_TextMesh == null", base.gameObject.name, this.m_Text));
			return;
		}
		Material sharedMaterial = this.m_TextMesh.GetComponent<Renderer>().sharedMaterial;
		if (sharedMaterial == null)
		{
			Debug.LogError(string.Format("UberText OutlineRender ({0}, {1}): m_TextMesh.renderer.sharedMaterial == null", base.gameObject.name, this.m_Text));
			return;
		}
		Texture mainTexture = sharedMaterial.mainTexture;
		if (mainTexture == null)
		{
			Debug.LogError(string.Format("UberText OutlineRender ({0}, {1}): textMat.mainTexture == null", base.gameObject.name, this.m_Text));
			return;
		}
		if (this.OutlineTextMaterial == null)
		{
			Debug.LogError(string.Format("UberText OutlineRender ({0}, {1}): OutlineTextMaterial == null", base.gameObject.name, this.m_Text));
			return;
		}
		this.m_TextMesh.GetComponent<Renderer>().sharedMaterial = this.OutlineTextMaterial;
		if (this.m_OutlineTextMaterial == null)
		{
			Debug.LogError(string.Format("UberText OutlineRender ({0}, {1}): m_OutlineTextMaterial == null", base.gameObject.name, this.m_Text));
			return;
		}
		this.m_OutlineTextMaterial.mainTexture = mainTexture;
		Vector2 vector = this.TexelSize(this.m_OutlineTextMaterial.mainTexture);
		float num = this.m_OutlineSize * this.m_OutlineModifier;
		if (this.m_LocalizedSettings != null)
		{
			UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
			if (locale != null)
			{
				num = this.m_OutlineSize * this.m_OutlineModifier * locale.m_OutlineModifier;
			}
		}
		this.m_OutlineTextMaterial.SetFloat("_OutlineOffsetX", vector.x * num);
		this.m_OutlineTextMaterial.SetFloat("_OutlineOffsetY", vector.y * num);
		this.m_OutlineTextMaterial.SetColor("_Color", this.m_TextColor);
		this.m_OutlineTextMaterial.SetColor("_OutlineColor", this.m_OutlineColor);
		this.m_OutlineTextMaterial.SetFloat("_TexelSizeX", vector.x);
		this.m_OutlineTextMaterial.SetFloat("_TexelSizeY", vector.y);
	}

	// Token: 0x06000E98 RID: 3736 RVA: 0x0003D21C File Offset: 0x0003B41C
	private void AntiAliasRender()
	{
		if (this.m_PlaneGameObject == null && this.m_RenderOnObject == null)
		{
			return;
		}
		if (this.m_AntiAlias)
		{
			Texture texture;
			if (this.m_RenderOnObject)
			{
				texture = this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
				this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial = this.TextAntialiasingMaterial;
				this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", texture);
			}
			else
			{
				texture = this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.GetTexture("_MainTex");
				this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.TextAntialiasingMaterial;
				this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", texture);
			}
			Vector2 vector = this.TexelSize(texture);
			this.m_TextAntialiasingMaterial.SetFloat("_OffsetX", vector.x * this.m_AntiAliasAmount);
			this.m_TextAntialiasingMaterial.SetFloat("_OffsetY", vector.y * this.m_AntiAliasAmount);
			this.m_TextAntialiasingMaterial.SetFloat("_Edge", this.m_AntiAliasEdge);
		}
		else if (this.m_RenderOnObject)
		{
			this.m_RenderOnObject.GetComponent<Renderer>().sharedMaterial = this.PlaneMaterial;
		}
		else
		{
			this.m_PlaneGameObject.GetComponent<Renderer>().sharedMaterial = this.PlaneMaterial;
		}
	}

	// Token: 0x06000E99 RID: 3737 RVA: 0x0003D3A4 File Offset: 0x0003B5A4
	private void Bold()
	{
		if (this.m_BoldSize > 10f)
		{
			this.m_BoldSize = 10f;
		}
		if (this.m_Outline)
		{
			if (this.m_BoldOutlineShader == null)
			{
				this.m_BoldOutlineShader = ShaderUtils.FindShader(this.BOLD_OUTLINE_TEXT_SHADER_NAME);
				if (!this.m_BoldOutlineShader)
				{
					Debug.LogError("UberText Failed to load Shader: " + this.BOLD_OUTLINE_TEXT_SHADER_NAME);
				}
			}
			float num = this.m_OutlineSize * this.m_OutlineModifier;
			if (this.m_LocalizedSettings != null)
			{
				UberText.LocalizationSettings.LocaleAdjustment locale = this.m_LocalizedSettings.GetLocale(Localization.GetLocale());
				if (locale != null)
				{
					num = this.m_OutlineSize * this.m_OutlineModifier * locale.m_OutlineModifier;
				}
			}
			this.m_BoldMaterial.shader = this.m_BoldOutlineShader;
			Vector2 vector = this.TexelSize(this.m_BoldMaterial.mainTexture);
			this.m_BoldMaterial.SetColor("_OutlineColor", this.m_OutlineColor);
			this.m_BoldMaterial.SetFloat("_BoldOffsetX", this.m_BoldSize * vector.x);
			this.m_BoldMaterial.SetFloat("_BoldOffsetY", this.m_BoldSize * vector.y);
			this.m_BoldMaterial.SetFloat("_OutlineOffsetX", vector.x * (num + this.m_BoldSize * 0.75f));
			this.m_BoldMaterial.SetFloat("_OutlineOffsetY", vector.y * (num + this.m_BoldSize * 0.75f));
		}
		else
		{
			this.m_BoldMaterial.shader = this.m_BoldShader;
			Vector2 vector2 = this.TexelSize(this.m_BoldMaterial.mainTexture);
			this.m_BoldMaterial.SetFloat("_BoldOffsetX", this.m_BoldSize * vector2.x);
			this.m_BoldMaterial.SetFloat("_BoldOffsetY", this.m_BoldSize * vector2.y);
			this.m_BoldMaterial.SetColor("_Color", this.m_TextColor);
		}
	}

	// Token: 0x06000E9A RID: 3738 RVA: 0x0003D5A0 File Offset: 0x0003B7A0
	private string InlineImage(string tag)
	{
		if (tag == string.Empty)
		{
			return string.Empty;
		}
		if (!UberText.s_InlineImageTextureLoaded)
		{
			AssetLoader.Get().LoadTexture("mana_in_line", new AssetLoader.ObjectCallback(this.SetManaTexture), null, false);
		}
		int num = this.TextEffectsMaterial(UberText.TextRenderMaterial.InlineImages, this.InlineImageMaterial);
		if (tag != null)
		{
			if (UberText.<>f__switch$map91 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(13);
				dictionary.Add("<m>", 0);
				dictionary.Add("<me>", 1);
				dictionary.Add("<m0>", 2);
				dictionary.Add("<m1>", 3);
				dictionary.Add("<m2>", 4);
				dictionary.Add("<m3>", 5);
				dictionary.Add("<m4>", 6);
				dictionary.Add("<m5>", 7);
				dictionary.Add("<m6>", 8);
				dictionary.Add("<m7>", 9);
				dictionary.Add("<m8>", 10);
				dictionary.Add("<m9>", 11);
				dictionary.Add("<m10>", 12);
				UberText.<>f__switch$map91 = dictionary;
			}
			int num2;
			if (UberText.<>f__switch$map91.TryGetValue(tag, ref num2))
			{
				float num3;
				float num4;
				switch (num2)
				{
				case 0:
					num3 = 0f;
					num4 = 0f;
					break;
				case 1:
					num3 = 0.75f;
					num4 = 0.25f;
					break;
				case 2:
					num3 = 0f;
					num4 = 0.75f;
					break;
				case 3:
					num3 = 0.25f;
					num4 = 0.75f;
					break;
				case 4:
					num3 = 0.5f;
					num4 = 0.75f;
					break;
				case 5:
					num3 = 0.75f;
					num4 = 0.75f;
					break;
				case 6:
					num3 = 0f;
					num4 = 0.5f;
					break;
				case 7:
					num3 = 0.25f;
					num4 = 0.5f;
					break;
				case 8:
					num3 = 0.5f;
					num4 = 0.5f;
					break;
				case 9:
					num3 = 0.75f;
					num4 = 0.5f;
					break;
				case 10:
					num3 = 0f;
					num4 = 0.25f;
					break;
				case 11:
					num3 = 0.25f;
					num4 = 0.25f;
					break;
				case 12:
					num3 = 0.5f;
					num4 = 0.25f;
					break;
				default:
					return tag;
				}
				string empty = string.Empty;
				this.m_TextMesh.GetComponent<Renderer>().sharedMaterials[num].mainTexture = UberText.s_InlineImageTexture;
				return string.Format("<quad material={0} size={1} x={2} y={3} width=0.25 height=0.25 />", new object[]
				{
					num,
					this.m_FontSize,
					num3,
					num4
				});
			}
		}
		return tag;
	}

	// Token: 0x06000E9B RID: 3739 RVA: 0x0003D868 File Offset: 0x0003BA68
	private int TextEffectsMaterial(UberText.TextRenderMaterial materialKey, Material material)
	{
		if (!this.m_TextMaterialIndices.ContainsKey(materialKey))
		{
			Material[] array = new Material[this.m_TextMesh.GetComponent<Renderer>().sharedMaterials.Length + 1];
			int num = array.Length - 1;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterials.CopyTo(array, 0);
			array[num] = material;
			this.m_TextMesh.GetComponent<Renderer>().sharedMaterials = array;
			this.m_TextMaterialIndices.Add(materialKey, num);
			return num;
		}
		return this.m_TextMaterialIndices[materialKey];
	}

	// Token: 0x06000E9C RID: 3740 RVA: 0x0003D8F0 File Offset: 0x0003BAF0
	private void SetManaTexture(string name, Object obj, object callbackData)
	{
		UberText.s_InlineImageTexture = (obj as Texture2D);
		UberText.s_InlineImageTextureLoaded = true;
		this.m_TextMesh.GetComponent<Renderer>().sharedMaterials[this.m_TextMaterialIndices[UberText.TextRenderMaterial.InlineImages]].mainTexture = UberText.s_InlineImageTexture;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x0003D935 File Offset: 0x0003BB35
	private void UpdateEditorText()
	{
	}

	// Token: 0x06000E9E RID: 3742 RVA: 0x0003D937 File Offset: 0x0003BB37
	public static void DisableCache()
	{
		UberText.s_disableCache = true;
		UberText.s_CachedText.Clear();
	}

	// Token: 0x06000E9F RID: 3743 RVA: 0x0003D94C File Offset: 0x0003BB4C
	private Vector2 GetWorldWidthAndHight()
	{
		Quaternion rotation = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		Vector3 lossyScale = base.transform.lossyScale;
		float num = this.m_Width;
		if (lossyScale.x > 0f)
		{
			num = this.m_Width * lossyScale.x;
		}
		float num2 = this.m_Height;
		if (lossyScale.y > 0f)
		{
			num2 = this.m_Height * lossyScale.y;
		}
		base.transform.rotation = rotation;
		return new Vector2(num, num2);
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0003D9E4 File Offset: 0x0003BBE4
	public static Vector3 GetWorldScale(Transform xform)
	{
		Vector3 localScale = xform.localScale;
		if (xform.parent != null)
		{
			Transform parent = xform.parent;
			while (parent != null)
			{
				localScale.Scale(parent.localScale);
				parent = parent.parent;
			}
		}
		return localScale;
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x0003DA38 File Offset: 0x0003BC38
	private Vector3 GetLossyWorldScale(Transform xform)
	{
		Quaternion rotation = xform.rotation;
		xform.rotation = Quaternion.identity;
		Vector3 lossyScale = base.transform.lossyScale;
		xform.rotation = rotation;
		return lossyScale;
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x0003DA6C File Offset: 0x0003BC6C
	private void FindSupportedTextureFormat()
	{
		if (UberText.s_TextureFormat == 9)
		{
			if (SystemInfo.SupportsRenderTextureFormat(5))
			{
				UberText.s_TextureFormat = 5;
			}
			else
			{
				UberText.s_TextureFormat = 7;
			}
		}
	}

	// Token: 0x06000EA3 RID: 3747 RVA: 0x0003DAA4 File Offset: 0x0003BCA4
	private Vector2 CalcTextureSize()
	{
		Vector2 result;
		result..ctor((float)this.m_Resolution, (float)this.m_Resolution);
		if (this.m_Width > this.m_Height)
		{
			result.x = (float)this.m_Resolution;
			result.y = (float)this.m_Resolution * (this.m_Height / this.m_Width);
		}
		else
		{
			result.x = (float)this.m_Resolution * (this.m_Width / this.m_Height);
			result.y = (float)this.m_Resolution;
		}
		if (GraphicsManager.Get() != null && GraphicsManager.Get().RenderQualityLevel == GraphicsQuality.Low)
		{
			result.x *= 0.75f;
			result.y *= 0.75f;
		}
		return result;
	}

	// Token: 0x06000EA4 RID: 3748 RVA: 0x0003DB78 File Offset: 0x0003BD78
	private string RemoveTagsFromWord(string word)
	{
		if (!this.m_RichText)
		{
			return word;
		}
		if (!word.Contains("<") && !word.Contains("["))
		{
			return word;
		}
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		for (int i = 0; i < word.Length; i++)
		{
			if (word.get_Chars(i) == '<')
			{
				if (i < word.Length - 1)
				{
					if (word.get_Chars(i + 1) == 'q')
					{
						if (!word.Substring(i).Contains(">"))
						{
							return stringBuilder.ToString();
						}
						int num = i + 1;
						while (word.get_Chars(num) != '>')
						{
							num++;
						}
						stringBuilder.Append("W");
						i = num;
					}
					else
					{
						flag = true;
					}
				}
			}
			else if (word.get_Chars(i) == '>')
			{
				flag = false;
			}
			else if (word.get_Chars(i) == '[' && i + 2 < word.Length && (word.get_Chars(i + 1) == 'b' || word.get_Chars(i + 1) == 'd' || word.get_Chars(i + 1) == 'x') && word.get_Chars(i + 2) == ']')
			{
				flag = true;
			}
			else
			{
				if (word.get_Chars(i) == ']')
				{
					if (i - 2 >= 0 && (word.get_Chars(i - 1) == 'b' || word.get_Chars(i - 1) == 'd' || word.get_Chars(i - 1) == 'x') && word.get_Chars(i - 2) == '[')
					{
						flag = false;
						goto IL_1B2;
					}
					flag = false;
				}
				if (!flag)
				{
					stringBuilder.Append(word.get_Chars(i));
				}
			}
			IL_1B2:;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000EA5 RID: 3749 RVA: 0x0003DD50 File Offset: 0x0003BF50
	private string RemoveLineBreakTagsHardSpace(string text)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		for (int i = 0; i < text.Length; i++)
		{
			if (text.get_Chars(i) == '[' && i + 2 < text.Length && (text.get_Chars(i + 1) == 'b' || text.get_Chars(i + 1) == 'd' || text.get_Chars(i + 1) == 'x') && text.get_Chars(i + 2) == ']')
			{
				flag = true;
			}
			else
			{
				if (text.get_Chars(i) == ']')
				{
					if (i - 2 >= 0 && (text.get_Chars(i - 1) == 'b' || text.get_Chars(i - 1) == 'd' || text.get_Chars(i - 1) == 'x') && text.get_Chars(i - 2) == '[')
					{
						flag = false;
						goto IL_107;
					}
					flag = false;
				}
				if (!flag)
				{
					if (text.get_Chars(i) == '_')
					{
						stringBuilder.Append(' ');
					}
					else
					{
						stringBuilder.Append(text.get_Chars(i));
					}
				}
			}
			IL_107:;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06000EA6 RID: 3750 RVA: 0x0003DE7C File Offset: 0x0003C07C
	private void DestroyChildren()
	{
		GameObject gameObject = new GameObject("UberTextDestroyDummy");
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (!(base.transform == transform))
			{
				if (!(transform == null))
				{
					GameObject gameObject2 = transform.gameObject;
					if (!(gameObject2 == null))
					{
						if (gameObject2.name.StartsWith("UberText_"))
						{
							gameObject2.transform.parent = gameObject.transform;
						}
					}
				}
			}
		}
		if (Application.isPlaying)
		{
			Object.Destroy(gameObject);
		}
		else
		{
			Object.DestroyImmediate(gameObject);
		}
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x0003DF44 File Offset: 0x0003C144
	private void CleanUp()
	{
		this.m_Offset = 0f;
		this.DestroyRenderPlane();
		this.DestroyCamera();
		this.DestroyTexture();
		this.DestroyShadow();
		this.DestroyTextMesh();
		this.m_updated = false;
		if (this.m_BoldMaterial)
		{
			Object.DestroyImmediate(this.m_BoldMaterial);
		}
		if (this.m_TextMaterial)
		{
			Object.DestroyImmediate(this.m_TextMaterial);
		}
		if (this.m_OutlineTextMaterial)
		{
			Object.DestroyImmediate(this.m_OutlineTextMaterial);
		}
		if (this.m_TextAntialiasingMaterial)
		{
			Object.DestroyImmediate(this.m_TextAntialiasingMaterial);
		}
		if (this.m_ShadowMaterial)
		{
			Object.DestroyImmediate(this.m_ShadowMaterial);
		}
		if (this.m_PlaneMaterial)
		{
			Object.DestroyImmediate(this.m_PlaneMaterial);
		}
		if (this.m_InlineImageMaterial)
		{
			Object.DestroyImmediate(this.m_InlineImageMaterial);
		}
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x0003E040 File Offset: 0x0003C240
	private static int LineCount(string s)
	{
		int num = 1;
		for (int i = 0; i < s.Length; i++)
		{
			if (s.get_Chars(i) == '\n')
			{
				num++;
			}
		}
		return num;
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x0003E07C File Offset: 0x0003C27C
	private string[] BreakStringIntoWords(string text)
	{
		if (text == null || text == string.Empty)
		{
			return null;
		}
		List<string> list = new List<string>();
		bool flag = false;
		TextElementEnumerator textElementEnumerator = StringInfo.GetTextElementEnumerator(text);
		int[] array = StringInfo.ParseCombiningCharacters(text);
		List<string> list2 = new List<string>(array.Length);
		while (textElementEnumerator.MoveNext())
		{
			list2.Add(textElementEnumerator.GetTextElement());
		}
		bool flag2 = false;
		StringBuilder stringBuilder = new StringBuilder(list2[0]);
		if (list2[0] == "<" && this.m_RichText)
		{
			flag = true;
		}
		int i = 1;
		while (i < list2.Count)
		{
			if (!(list2[i] == "]") || i - 2 <= 0)
			{
				goto IL_165;
			}
			if ((list2[i - 1] == "b" || list2[i - 1] == "d") && list2[i - 2] == "[")
			{
				stringBuilder.Append(list2[i]);
				list.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			else
			{
				if (list2[i - 1] == "x" && list2[i - 2] == "[")
				{
					flag2 = true;
					goto IL_165;
				}
				goto IL_165;
			}
			IL_286:
			i++;
			continue;
			IL_165:
			if (list2[i] == "<" && this.m_RichText)
			{
				flag = true;
				stringBuilder.Append(list2[i]);
				goto IL_286;
			}
			if (list2[i] == ">")
			{
				flag = false;
				stringBuilder.Append(list2[i]);
				goto IL_286;
			}
			if (flag)
			{
				stringBuilder.Append(list2[i]);
				goto IL_286;
			}
			string text2 = list2[i - 1];
			string text3 = list2[i];
			string empty = string.Empty;
			int lastChar = char.ConvertToUtf32(text2, 0);
			int wideChar = char.ConvertToUtf32(text3, 0);
			int nextChar = 0;
			if (i < list2.Count - 1 && !string.IsNullOrEmpty(empty))
			{
				nextChar = char.ConvertToUtf32(empty, 0);
			}
			if (!flag2 && this.CanWrapBetween(lastChar, wideChar, nextChar))
			{
				list.Add(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			stringBuilder.Append(list2[i]);
			goto IL_286;
		}
		list.Add(stringBuilder.ToString());
		return list.ToArray();
	}

	// Token: 0x06000EAA RID: 3754 RVA: 0x0003E338 File Offset: 0x0003C538
	public Vector2 TexelSize(Texture texture)
	{
		int frameCount = Time.frameCount;
		Font key = this.m_Font;
		if (this.m_LocalizedFont != null)
		{
			key = this.m_LocalizedFont;
		}
		if (UberText.s_TexelUpdateFrame.ContainsKey(key) && UberText.s_TexelUpdateFrame[key] == frameCount)
		{
			return UberText.s_TexelUpdateData[key];
		}
		Vector2 vector = default(Vector2);
		vector.x = 1f / (float)texture.width;
		vector.y = 1f / (float)texture.height;
		UberText.s_TexelUpdateFrame[key] = frameCount;
		UberText.s_TexelUpdateData[key] = vector;
		return vector;
	}

	// Token: 0x06000EAB RID: 3755 RVA: 0x0003E3E0 File Offset: 0x0003C5E0
	private static void DeleteOldCacheFiles()
	{
		foreach (object obj in Enum.GetValues(typeof(Locale)))
		{
			Locale locale = (Locale)((int)obj);
			string text = string.Format("{0}/text_{1}.cache", FileUtils.PersistentDataPath, locale);
			if (File.Exists(text))
			{
				try
				{
					File.Delete(text);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("UberText.DeleteOldCacheFiles() - Failed to delete {0}. Reason={1}", text, ex.Message));
				}
			}
		}
	}

	// Token: 0x06000EAC RID: 3756 RVA: 0x0003E4A4 File Offset: 0x0003C6A4
	private static string GetCacheFolderPath()
	{
		return string.Format("{0}/UberText", FileUtils.CachePath);
	}

	// Token: 0x06000EAD RID: 3757 RVA: 0x0003E4B5 File Offset: 0x0003C6B5
	private static string GetCacheFilePath()
	{
		return string.Format("{0}/text_{1}.cache", UberText.GetCacheFolderPath(), Localization.GetLocale());
	}

	// Token: 0x06000EAE RID: 3758 RVA: 0x0003E4D0 File Offset: 0x0003C6D0
	private static void CreateCacheFolder()
	{
		string cacheFolderPath = UberText.GetCacheFolderPath();
		if (Directory.Exists(cacheFolderPath))
		{
			return;
		}
		try
		{
			Directory.CreateDirectory(cacheFolderPath);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("UberText.CreateCacheFolder() - Failed to create {0}. Reason={1}", cacheFolderPath, ex.Message));
		}
	}

	// Token: 0x06000EAF RID: 3759 RVA: 0x0003E528 File Offset: 0x0003C728
	public static void StoreCachedData()
	{
		if (UberText.s_disableCache)
		{
			return;
		}
		UberText.CreateCacheFolder();
		string cacheFilePath = UberText.GetCacheFilePath();
		using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(cacheFilePath, 2)))
		{
			int num = 12574;
			binaryWriter.Write(num);
			foreach (KeyValuePair<int, UberText.CachedTextValues> keyValuePair in UberText.s_CachedText)
			{
				binaryWriter.Write(keyValuePair.Key);
				binaryWriter.Write(keyValuePair.Value.m_Text);
				binaryWriter.Write(keyValuePair.Value.m_CharSize);
				binaryWriter.Write(keyValuePair.Value.m_OriginalTextHash);
			}
		}
		Log.Kyle.Print("UberText Cache Stored: " + cacheFilePath, new object[0]);
	}

	// Token: 0x06000EB0 RID: 3760 RVA: 0x0003E62C File Offset: 0x0003C82C
	public static void LoadCachedData()
	{
		if (UberText.s_disableCache)
		{
			return;
		}
		UberText.s_CachedText.Clear();
		UberText.DeleteOldCacheFiles();
		UberText.CreateCacheFolder();
		string cacheFilePath = UberText.GetCacheFilePath();
		if (!File.Exists(cacheFilePath))
		{
			return;
		}
		int num = 12574;
		using (BinaryReader binaryReader = new BinaryReader(File.Open(cacheFilePath, 3)))
		{
			if (binaryReader.BaseStream.Length == 0L)
			{
				return;
			}
			int num2 = binaryReader.ReadInt32();
			if (num2 != num)
			{
				return;
			}
			if (binaryReader.PeekChar() == -1)
			{
				return;
			}
			try
			{
				while (binaryReader.PeekChar() != -1)
				{
					int key = binaryReader.ReadInt32();
					UberText.CachedTextValues cachedTextValues = new UberText.CachedTextValues();
					cachedTextValues.m_Text = binaryReader.ReadString();
					cachedTextValues.m_CharSize = binaryReader.ReadSingle();
					cachedTextValues.m_OriginalTextHash = binaryReader.ReadInt32();
					UberText.s_CachedText.Add(key, cachedTextValues);
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning(string.Format("UberText LoadCachedData() failed: {0}", ex.Message));
				UberText.s_CachedText.Clear();
			}
		}
		if (UberText.s_CachedText.Count > 50000)
		{
			UberText.s_CachedText.Clear();
		}
		Log.Kyle.Print("UberText Cache Loaded: " + UberText.s_CachedText.Count.ToString(), new object[0]);
	}

	// Token: 0x06000EB1 RID: 3761 RVA: 0x0003E7B4 File Offset: 0x0003C9B4
	private bool CanWrapBetween(int lastChar, int wideChar, int nextChar)
	{
		if (Localization.GetLocale() == Locale.frFR || Localization.GetLocale() == Locale.deDE)
		{
			if (char.IsWhiteSpace(char.ConvertFromUtf32(wideChar), 0))
			{
				switch (nextChar)
				{
				case 58:
				case 59:
				case 63:
					break;
				default:
					if (nextChar != 33 && nextChar != 46 && nextChar != 171 && nextChar != 187)
					{
						goto IL_77;
					}
					break;
				}
				return false;
			}
			IL_77:
			if (char.IsWhiteSpace(char.ConvertFromUtf32(wideChar), 0) && lastChar == 171)
			{
				return false;
			}
		}
		if (lastChar == 45)
		{
			return wideChar < 48 || wideChar > 57;
		}
		if (lastChar == 59)
		{
			return true;
		}
		if (wideChar == 124)
		{
			return true;
		}
		if (char.IsWhiteSpace(char.ConvertFromUtf32(lastChar), 0))
		{
			return false;
		}
		if (char.IsWhiteSpace(char.ConvertFromUtf32(wideChar), 0))
		{
			return true;
		}
		switch (lastChar)
		{
		case 12296:
		case 12298:
		case 12300:
		case 12302:
		case 12304:
			break;
		default:
			switch (lastChar)
			{
			case 65505:
			case 65509:
			case 65510:
				break;
			default:
				switch (lastChar)
				{
				case 65113:
				case 65115:
				case 65117:
					break;
				default:
					if (lastChar != 91 && lastChar != 92 && lastChar != 36 && lastChar != 40 && lastChar != 123 && lastChar != 8216 && lastChar != 8220 && lastChar != 8245 && lastChar != 12308 && lastChar != 12317 && lastChar != 65284 && lastChar != 65288 && lastChar != 65339 && lastChar != 65371)
					{
						switch (wideChar)
						{
						case 65104:
						case 65105:
						case 65106:
						case 65108:
						case 65109:
						case 65110:
						case 65111:
						case 65114:
						case 65116:
						case 65118:
							break;
						default:
							switch (wideChar)
							{
							case 12297:
							case 12299:
							case 12301:
							case 12303:
							case 12305:
								break;
							default:
								switch (wideChar)
								{
								case 41:
								case 44:
								case 46:
									break;
								default:
									switch (wideChar)
									{
									case 58:
									case 59:
									case 63:
										break;
									default:
										switch (wideChar)
										{
										case 8226:
										case 8230:
										case 8231:
											break;
										default:
											switch (wideChar)
											{
											case 65289:
											case 65292:
											case 65294:
												break;
											default:
												switch (wideChar)
												{
												case 65306:
												case 65307:
												case 65311:
													break;
												default:
													if (wideChar != 8211 && wideChar != 8212 && wideChar != 8242 && wideChar != 8243 && wideChar != 12289 && wideChar != 12290 && wideChar != 65438 && wideChar != 65439 && wideChar != 33 && wideChar != 37 && wideChar != 93 && wideChar != 125 && wideChar != 176 && wideChar != 183 && wideChar != 8217 && wideChar != 8221 && wideChar != 8451 && wideChar != 12309 && wideChar != 12318 && wideChar != 12540 && wideChar != 65072 && wideChar != 65281 && wideChar != 65285 && wideChar != 65341 && wideChar != 65373 && wideChar != 65392 && wideChar != 65504)
													{
														return lastChar == 12290 || lastChar == 65292 || ((Localization.GetLocale() != Locale.koKR || this.m_Alignment != UberText.AlignmentOptions.Center) && ((wideChar >= 4352 && wideChar <= 4607) || (wideChar >= 12288 && wideChar <= 55215) || (wideChar >= 63744 && wideChar <= 64255) || (wideChar >= 65280 && wideChar <= 65439) || (wideChar >= 65440 && wideChar <= 65500)));
													}
													break;
												}
												break;
											}
											break;
										}
										break;
									}
									break;
								}
								break;
							}
							break;
						}
						return false;
					}
					break;
				}
				break;
			}
			break;
		}
		return false;
	}

	// Token: 0x06000EB2 RID: 3762 RVA: 0x0003ECAC File Offset: 0x0003CEAC
	private string FixThai(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		char[] array = text.ToCharArray();
		UberText.ThaiGlyphType[] array2 = new UberText.ThaiGlyphType[Enumerable.Count<char>(array)];
		StringBuilder stringBuilder = new StringBuilder(text);
		for (int i = 0; i < Enumerable.Count<char>(array); i++)
		{
			char c = array[i];
			if ((c >= 'ก' && c <= 'ฯ') || c == 'ะ' || c == 'เ' || c == 'แ')
			{
				if (c == 'ฝ' || c == 'ฟ' || c == 'ป' || c == 'ฬ')
				{
					array2[i] = UberText.ThaiGlyphType.BASE_ASCENDER;
				}
				else if (c == 'ฏ' || c == 'ฎ')
				{
					array2[i] = UberText.ThaiGlyphType.BASE_DESCENDER;
				}
				else
				{
					array2[i] = UberText.ThaiGlyphType.BASE;
				}
			}
			else if (c >= '่' && c <= '์')
			{
				array2[i] = UberText.ThaiGlyphType.TONE_MARK;
			}
			else if (c == 'ั' || c == 'ิ' || c == 'ี' || c == 'ึ' || c == 'ื' || c == '็' || c == 'ํ')
			{
				array2[i] = UberText.ThaiGlyphType.UPPER;
			}
			else if (c == 'ุ' || c == 'ู' || c == 'ฺ')
			{
				array2[i] = UberText.ThaiGlyphType.LOWER;
			}
		}
		for (int j = 0; j < Enumerable.Count<char>(array); j++)
		{
			char c2 = array[j];
			UberText.ThaiGlyphType thaiGlyphType = array2[j];
			stringBuilder.set_Chars(j, c2);
			if (j >= 1)
			{
				UberText.ThaiGlyphType thaiGlyphType2 = array2[j - 1];
				char c3 = array[j - 1];
				if (thaiGlyphType == UberText.ThaiGlyphType.UPPER && thaiGlyphType2 == UberText.ThaiGlyphType.BASE_ASCENDER)
				{
					char c4 = c2;
					switch (c4)
					{
					case 'ั':
						stringBuilder.set_Chars(j, '');
						break;
					default:
						if (c4 != '็')
						{
							if (c4 == 'ํ')
							{
								stringBuilder.set_Chars(j, '');
							}
						}
						else
						{
							stringBuilder.set_Chars(j, '');
						}
						break;
					case 'ิ':
						stringBuilder.set_Chars(j, '');
						break;
					case 'ี':
						stringBuilder.set_Chars(j, '');
						break;
					case 'ึ':
						stringBuilder.set_Chars(j, '');
						break;
					case 'ื':
						stringBuilder.set_Chars(j, '');
						break;
					}
				}
				else if (thaiGlyphType == UberText.ThaiGlyphType.LOWER && thaiGlyphType2 == UberText.ThaiGlyphType.BASE_DESCENDER)
				{
					stringBuilder.set_Chars(j, c2 + '');
				}
				else
				{
					if (thaiGlyphType == UberText.ThaiGlyphType.LOWER)
					{
						if (c3 == 'ญ')
						{
							stringBuilder.set_Chars(j - 1, '');
							goto IL_441;
						}
						if (c3 == 'ฐ')
						{
							stringBuilder.set_Chars(j - 1, '');
							goto IL_441;
						}
					}
					if (thaiGlyphType == UberText.ThaiGlyphType.TONE_MARK)
					{
						if (j - 2 >= 0)
						{
							if (thaiGlyphType2 == UberText.ThaiGlyphType.UPPER && array2[j - 2] == UberText.ThaiGlyphType.BASE_ASCENDER)
							{
								stringBuilder.set_Chars(j, c2 + '');
							}
							if (thaiGlyphType2 == UberText.ThaiGlyphType.LOWER && j > 1)
							{
								thaiGlyphType2 = array2[j - 2];
								c3 = array[j - 2];
							}
						}
						if (j < Enumerable.Count<char>(array) - 1 && (array[j + 1] == 'ำ' || array[j + 1] == 'ํ'))
						{
							if (thaiGlyphType2 == UberText.ThaiGlyphType.BASE_ASCENDER)
							{
								stringBuilder.set_Chars(j, c2 + '');
								stringBuilder.Insert(j + 1, '');
								stringBuilder.Insert(j + 2, c2);
								if (array[j + 1] == 'ำ')
								{
									stringBuilder.set_Chars(j + 1, 'ำ');
								}
								j++;
								goto IL_441;
							}
						}
						else if (thaiGlyphType2 == UberText.ThaiGlyphType.BASE || thaiGlyphType2 == UberText.ThaiGlyphType.BASE_DESCENDER)
						{
							stringBuilder.set_Chars(j, c2 + '');
							goto IL_441;
						}
						if (thaiGlyphType2 == UberText.ThaiGlyphType.BASE_ASCENDER)
						{
							stringBuilder.set_Chars(j, c2 + '');
						}
					}
				}
			}
			IL_441:;
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0400074B RID: 1867
	private const int CACHE_FILE_VERSION_TEMP = 2;

	// Token: 0x0400074C RID: 1868
	private const int CACHE_FILE_MAX_SIZE = 50000;

	// Token: 0x0400074D RID: 1869
	private const string FONT_NAME_BLIZZARD_GLOBAL = "BlizzardGlobal";

	// Token: 0x0400074E RID: 1870
	private const string FONT_NAME_BELWE_OUTLINE = "Belwe_Outline";

	// Token: 0x0400074F RID: 1871
	private const string FONT_NAME_BELWE = "Belwe";

	// Token: 0x04000750 RID: 1872
	private const string FONT_NAME_FRANKLIN_GOTHIC = "FranklinGothic";

	// Token: 0x04000751 RID: 1873
	private const float CHARACTER_SIZE_SCALE = 0.01f;

	// Token: 0x04000752 RID: 1874
	private const float BOLD_MAX_SIZE = 10f;

	// Token: 0x04000753 RID: 1875
	private const int MAX_REDUCE_TEXT_COUNT = 40;

	// Token: 0x04000754 RID: 1876
	private readonly string TEXT_SHADER_NAME = "Hero/Text_Unlit";

	// Token: 0x04000755 RID: 1877
	private readonly string PLANE_SHADER_NAME = "Hidden/TextPlane";

	// Token: 0x04000756 RID: 1878
	private readonly string BOLD_SHADER_NAME = "Hidden/Text_Bold";

	// Token: 0x04000757 RID: 1879
	private readonly string BOLD_OUTLINE_TEXT_SHADER_NAME = "Hidden/TextBoldOutline_Unlit";

	// Token: 0x04000758 RID: 1880
	private readonly string OUTLINE_TEXT_SHADER_NAME = "Hidden/TextOutline_Unlit";

	// Token: 0x04000759 RID: 1881
	private readonly string OUTLINE_TEXT_2PASS_SHADER_NAME = "Hidden/TextOutline_Unlit_2pass";

	// Token: 0x0400075A RID: 1882
	private readonly string OUTLINE_NO_VERT_COLOR_TEXT_SHADER_NAME = "Hidden/TextOutline_Unlit_NoVertColor";

	// Token: 0x0400075B RID: 1883
	private readonly string OUTLINE_NO_VERT_COLOR_TEXT_2PASS_SHADER_NAME = "Hidden/TextOutline_Unlit_NoVertColor_2pass";

	// Token: 0x0400075C RID: 1884
	private readonly string TEXT_ANTIALAISING_SHADER_NAME = "Hidden/TextAntialiasing";

	// Token: 0x0400075D RID: 1885
	private readonly string INLINE_IMAGE_SHADER_NAME = "Hero/Unlit_Transparent";

	// Token: 0x0400075E RID: 1886
	private readonly string SHADOW_SHADER_NAME = "Hidden/TextShadow";

	// Token: 0x0400075F RID: 1887
	[SerializeField]
	private string m_Text = "Uber Text";

	// Token: 0x04000760 RID: 1888
	[SerializeField]
	private bool m_GameStringLookup;

	// Token: 0x04000761 RID: 1889
	[SerializeField]
	private bool m_UseEditorText;

	// Token: 0x04000762 RID: 1890
	[SerializeField]
	private float m_Width = 1f;

	// Token: 0x04000763 RID: 1891
	[SerializeField]
	private float m_Height = 1f;

	// Token: 0x04000764 RID: 1892
	[SerializeField]
	private float m_LineSpacing = 1f;

	// Token: 0x04000765 RID: 1893
	[SerializeField]
	private Font m_Font;

	// Token: 0x04000766 RID: 1894
	[SerializeField]
	private int m_FontSize = 35;

	// Token: 0x04000767 RID: 1895
	[SerializeField]
	private int m_MinFontSize = 10;

	// Token: 0x04000768 RID: 1896
	[SerializeField]
	private float m_CharacterSize = 5f;

	// Token: 0x04000769 RID: 1897
	[SerializeField]
	private float m_MinCharacterSize = 1f;

	// Token: 0x0400076A RID: 1898
	[SerializeField]
	private bool m_RichText = true;

	// Token: 0x0400076B RID: 1899
	[SerializeField]
	private Color m_TextColor = Color.white;

	// Token: 0x0400076C RID: 1900
	[SerializeField]
	private float m_BoldSize;

	// Token: 0x0400076D RID: 1901
	[SerializeField]
	private bool m_WordWrap;

	// Token: 0x0400076E RID: 1902
	[SerializeField]
	private bool m_ForceWrapLargeWords;

	// Token: 0x0400076F RID: 1903
	[SerializeField]
	private bool m_ResizeToFit;

	// Token: 0x04000770 RID: 1904
	[SerializeField]
	private bool m_Underwear;

	// Token: 0x04000771 RID: 1905
	[SerializeField]
	private bool m_UnderwearFlip;

	// Token: 0x04000772 RID: 1906
	[SerializeField]
	private float m_UnderwearWidth = 0.2f;

	// Token: 0x04000773 RID: 1907
	[SerializeField]
	private float m_UnderwearHeight = 0.2f;

	// Token: 0x04000774 RID: 1908
	[SerializeField]
	private UberText.AlignmentOptions m_Alignment = UberText.AlignmentOptions.Center;

	// Token: 0x04000775 RID: 1909
	[SerializeField]
	private UberText.AnchorOptions m_Anchor = UberText.AnchorOptions.Middle;

	// Token: 0x04000776 RID: 1910
	[SerializeField]
	private bool m_RenderToTexture;

	// Token: 0x04000777 RID: 1911
	[SerializeField]
	private GameObject m_RenderOnObject;

	// Token: 0x04000778 RID: 1912
	[SerializeField]
	private int m_Resolution = 256;

	// Token: 0x04000779 RID: 1913
	[SerializeField]
	private bool m_Outline;

	// Token: 0x0400077A RID: 1914
	[SerializeField]
	private float m_OutlineSize = 1f;

	// Token: 0x0400077B RID: 1915
	[SerializeField]
	private Color m_OutlineColor = Color.black;

	// Token: 0x0400077C RID: 1916
	[SerializeField]
	private bool m_AntiAlias;

	// Token: 0x0400077D RID: 1917
	[SerializeField]
	private float m_AntiAliasAmount = 0.5f;

	// Token: 0x0400077E RID: 1918
	[SerializeField]
	private float m_AntiAliasEdge = 0.5f;

	// Token: 0x0400077F RID: 1919
	[SerializeField]
	private bool m_Shadow;

	// Token: 0x04000780 RID: 1920
	[SerializeField]
	private float m_ShadowOffset = 1f;

	// Token: 0x04000781 RID: 1921
	[SerializeField]
	private Color m_ShadowColor = new Color(0.1f, 0.1f, 0.1f, 0.333f);

	// Token: 0x04000782 RID: 1922
	[SerializeField]
	private float m_ShadowBlur = 1.5f;

	// Token: 0x04000783 RID: 1923
	[SerializeField]
	private int m_RenderQueue;

	// Token: 0x04000784 RID: 1924
	[SerializeField]
	private float m_AmbientLightBlend;

	// Token: 0x04000785 RID: 1925
	[SerializeField]
	private Color m_GradientUpperColor = Color.white;

	// Token: 0x04000786 RID: 1926
	[SerializeField]
	private Color m_GradientLowerColor = Color.white;

	// Token: 0x04000787 RID: 1927
	[SerializeField]
	private bool m_Cache = true;

	// Token: 0x04000788 RID: 1928
	[SerializeField]
	private UberText.LocalizationSettings m_LocalizedSettings;

	// Token: 0x04000789 RID: 1929
	private bool m_isFontDefLoaded;

	// Token: 0x0400078A RID: 1930
	private Font m_LocalizedFont;

	// Token: 0x0400078B RID: 1931
	private Texture m_FontTexture;

	// Token: 0x0400078C RID: 1932
	private float m_LineSpaceModifier = 1f;

	// Token: 0x0400078D RID: 1933
	private float m_SingleLineAdjustment;

	// Token: 0x0400078E RID: 1934
	private float m_FontSizeModifier = 1f;

	// Token: 0x0400078F RID: 1935
	private float m_CharacterSizeModifier = 1f;

	// Token: 0x04000790 RID: 1936
	private float m_OutlineModifier = 1f;

	// Token: 0x04000791 RID: 1937
	private float m_WorldWidth;

	// Token: 0x04000792 RID: 1938
	private float m_WorldHeight;

	// Token: 0x04000793 RID: 1939
	private bool m_updated;

	// Token: 0x04000794 RID: 1940
	private string m_PreviousText = string.Empty;

	// Token: 0x04000795 RID: 1941
	private string[] m_Words;

	// Token: 0x04000796 RID: 1942
	private TextMesh m_TextMesh;

	// Token: 0x04000797 RID: 1943
	private GameObject m_TextMeshGameObject;

	// Token: 0x04000798 RID: 1944
	private Material TextMeshBaseMaterial;

	// Token: 0x04000799 RID: 1945
	private Map<UberText.TextRenderMaterial, int> m_TextMaterialIndices = new Map<UberText.TextRenderMaterial, int>();

	// Token: 0x0400079A RID: 1946
	private RenderTexture m_TextTexture;

	// Token: 0x0400079B RID: 1947
	private Camera m_Camera;

	// Token: 0x0400079C RID: 1948
	private GameObject m_CameraGO;

	// Token: 0x0400079D RID: 1949
	private Mesh m_PlaneMesh;

	// Token: 0x0400079E RID: 1950
	private GameObject m_PlaneGameObject;

	// Token: 0x0400079F RID: 1951
	private float m_PreviousPlaneWidth;

	// Token: 0x040007A0 RID: 1952
	private float m_PreviousPlaneHeight;

	// Token: 0x040007A1 RID: 1953
	private int m_PreviousResolution = 256;

	// Token: 0x040007A2 RID: 1954
	private int m_LineCount;

	// Token: 0x040007A3 RID: 1955
	private GameObject m_ShadowPlaneGameObject;

	// Token: 0x040007A4 RID: 1956
	private Vector2 m_PreviousTexelSize;

	// Token: 0x040007A5 RID: 1957
	private int m_OrgRenderQueue = -9999;

	// Token: 0x040007A6 RID: 1958
	private bool m_Ellipsized;

	// Token: 0x040007A7 RID: 1959
	private int m_CacheHash;

	// Token: 0x040007A8 RID: 1960
	private bool m_Hidden;

	// Token: 0x040007A9 RID: 1961
	private bool m_TextSet;

	// Token: 0x040007AA RID: 1962
	private Bounds m_UnderwearLeftBounds;

	// Token: 0x040007AB RID: 1963
	private Bounds m_UnderwearRightBounds;

	// Token: 0x040007AC RID: 1964
	private static Texture2D s_InlineImageTexture;

	// Token: 0x040007AD RID: 1965
	private static bool s_InlineImageTextureLoaded = false;

	// Token: 0x040007AE RID: 1966
	private static float s_offset = -3000f;

	// Token: 0x040007AF RID: 1967
	private float m_Offset;

	// Token: 0x040007B0 RID: 1968
	private readonly Vector2[] PLANE_UVS = new Vector2[]
	{
		new Vector2(0f, 0f),
		new Vector2(1f, 0f),
		new Vector2(0f, 1f),
		new Vector2(1f, 1f)
	};

	// Token: 0x040007B1 RID: 1969
	private readonly Vector3[] PLANE_NORMALS = new Vector3[]
	{
		Vector3.up,
		Vector3.up,
		Vector3.up,
		Vector3.up
	};

	// Token: 0x040007B2 RID: 1970
	private readonly int[] PLANE_TRIANGLES = new int[]
	{
		3,
		1,
		2,
		2,
		1,
		0
	};

	// Token: 0x040007B3 RID: 1971
	private static RenderTextureFormat s_TextureFormat = 9;

	// Token: 0x040007B4 RID: 1972
	private static bool s_disableCache = false;

	// Token: 0x040007B5 RID: 1973
	private static Map<int, UberText.CachedTextValues> s_CachedText = new Map<int, UberText.CachedTextValues>();

	// Token: 0x040007B6 RID: 1974
	private static Map<Font, int> s_TexelUpdateFrame = new Map<Font, int>();

	// Token: 0x040007B7 RID: 1975
	private static Map<Font, Vector2> s_TexelUpdateData = new Map<Font, Vector2>();

	// Token: 0x040007B8 RID: 1976
	private static int RENDER_LAYER = 28;

	// Token: 0x040007B9 RID: 1977
	private static int RENDER_LAYER_BIT = GameLayer.InvisibleRender.LayerBit();

	// Token: 0x040007BA RID: 1978
	private Shader m_TextShader;

	// Token: 0x040007BB RID: 1979
	private Material m_TextMaterial;

	// Token: 0x040007BC RID: 1980
	private Shader m_PlaneShader;

	// Token: 0x040007BD RID: 1981
	private Material m_PlaneMaterial;

	// Token: 0x040007BE RID: 1982
	private Shader m_BoldShader;

	// Token: 0x040007BF RID: 1983
	private Shader m_BoldOutlineShader;

	// Token: 0x040007C0 RID: 1984
	private Material m_BoldMaterial;

	// Token: 0x040007C1 RID: 1985
	private Shader m_OutlineTextShader;

	// Token: 0x040007C2 RID: 1986
	private Material m_OutlineTextMaterial;

	// Token: 0x040007C3 RID: 1987
	private Shader m_AntialiasingTextShader;

	// Token: 0x040007C4 RID: 1988
	private Material m_TextAntialiasingMaterial;

	// Token: 0x040007C5 RID: 1989
	private Shader m_ShadowTextShader;

	// Token: 0x040007C6 RID: 1990
	private Material m_ShadowMaterial;

	// Token: 0x040007C7 RID: 1991
	private Shader m_InlineImageShader;

	// Token: 0x040007C8 RID: 1992
	private Material m_InlineImageMaterial;

	// Token: 0x020001A2 RID: 418
	public enum AlignmentOptions
	{
		// Token: 0x04000E58 RID: 3672
		Left,
		// Token: 0x04000E59 RID: 3673
		Center,
		// Token: 0x04000E5A RID: 3674
		Right
	}

	// Token: 0x020001E3 RID: 483
	[Serializable]
	public class LocalizationSettings
	{
		// Token: 0x06001DB6 RID: 7606 RVA: 0x0008A965 File Offset: 0x00088B65
		public LocalizationSettings()
		{
			this.m_LocaleAdjustments = new List<UberText.LocalizationSettings.LocaleAdjustment>();
		}

		// Token: 0x06001DB7 RID: 7607 RVA: 0x0008A978 File Offset: 0x00088B78
		public bool HasLocale(Locale locale)
		{
			return this.GetLocale(locale) != null;
		}

		// Token: 0x06001DB8 RID: 7608 RVA: 0x0008A98C File Offset: 0x00088B8C
		public UberText.LocalizationSettings.LocaleAdjustment GetLocale(Locale locale)
		{
			foreach (UberText.LocalizationSettings.LocaleAdjustment localeAdjustment in this.m_LocaleAdjustments)
			{
				if (localeAdjustment.m_Locale == locale)
				{
					return localeAdjustment;
				}
			}
			return null;
		}

		// Token: 0x06001DB9 RID: 7609 RVA: 0x0008A9F8 File Offset: 0x00088BF8
		public UberText.LocalizationSettings.LocaleAdjustment AddLocale(Locale locale)
		{
			UberText.LocalizationSettings.LocaleAdjustment localeAdjustment = this.GetLocale(locale);
			if (localeAdjustment != null)
			{
				return localeAdjustment;
			}
			localeAdjustment = new UberText.LocalizationSettings.LocaleAdjustment(locale);
			this.m_LocaleAdjustments.Add(localeAdjustment);
			return localeAdjustment;
		}

		// Token: 0x06001DBA RID: 7610 RVA: 0x0008AA2C File Offset: 0x00088C2C
		public void RemoveLocale(Locale locale)
		{
			for (int i = 0; i < this.m_LocaleAdjustments.Count; i++)
			{
				if (this.m_LocaleAdjustments[i].m_Locale == locale)
				{
					this.m_LocaleAdjustments.RemoveAt(i);
					return;
				}
			}
		}

		// Token: 0x04001067 RID: 4199
		public List<UberText.LocalizationSettings.LocaleAdjustment> m_LocaleAdjustments;

		// Token: 0x020001E4 RID: 484
		[Serializable]
		public class LocaleAdjustment
		{
			// Token: 0x06001DBB RID: 7611 RVA: 0x0008AA79 File Offset: 0x00088C79
			public LocaleAdjustment()
			{
				this.m_Locale = Locale.enUS;
			}

			// Token: 0x06001DBC RID: 7612 RVA: 0x0008AAB4 File Offset: 0x00088CB4
			public LocaleAdjustment(Locale locale)
			{
				this.m_Locale = locale;
			}

			// Token: 0x04001068 RID: 4200
			public Locale m_Locale;

			// Token: 0x04001069 RID: 4201
			public float m_LineSpaceModifier = 1f;

			// Token: 0x0400106A RID: 4202
			public float m_SingleLineAdjustment;

			// Token: 0x0400106B RID: 4203
			public float m_FontSizeModifier = 1f;

			// Token: 0x0400106C RID: 4204
			public float m_UnderwearWidth;

			// Token: 0x0400106D RID: 4205
			public float m_UnderwearHeight;

			// Token: 0x0400106E RID: 4206
			public float m_OutlineModifier = 1f;

			// Token: 0x0400106F RID: 4207
			public Vector3 m_PositionOffset = Vector3.zero;
		}
	}

	// Token: 0x020001E5 RID: 485
	private class CachedTextKeyData
	{
		// Token: 0x06001DBE RID: 7614 RVA: 0x0008AAF8 File Offset: 0x00088CF8
		public override int GetHashCode()
		{
			int num = this.m_Text.Length + this.m_FontSize + this.m_Text.GetHashCode();
			num += this.m_FontSize.GetHashCode();
			num -= this.m_CharSize.GetHashCode();
			num += this.m_Width.GetHashCode();
			num -= this.m_Height.GetHashCode();
			num += this.m_Font.GetHashCode();
			return num - this.m_LineSpacing.GetHashCode();
		}

		// Token: 0x04001070 RID: 4208
		public string m_Text;

		// Token: 0x04001071 RID: 4209
		public int m_FontSize;

		// Token: 0x04001072 RID: 4210
		public float m_CharSize;

		// Token: 0x04001073 RID: 4211
		public float m_Width;

		// Token: 0x04001074 RID: 4212
		public float m_Height;

		// Token: 0x04001075 RID: 4213
		public Font m_Font;

		// Token: 0x04001076 RID: 4214
		public float m_LineSpacing;
	}

	// Token: 0x020001E6 RID: 486
	[Serializable]
	private class CachedTextValues
	{
		// Token: 0x04001077 RID: 4215
		public string m_Text;

		// Token: 0x04001078 RID: 4216
		public float m_CharSize;

		// Token: 0x04001079 RID: 4217
		public int m_OriginalTextHash;
	}

	// Token: 0x020001E7 RID: 487
	public enum AnchorOptions
	{
		// Token: 0x0400107B RID: 4219
		Upper,
		// Token: 0x0400107C RID: 4220
		Middle,
		// Token: 0x0400107D RID: 4221
		Lower
	}

	// Token: 0x020001E8 RID: 488
	private enum TextRenderMaterial
	{
		// Token: 0x0400107F RID: 4223
		Text,
		// Token: 0x04001080 RID: 4224
		Bold,
		// Token: 0x04001081 RID: 4225
		Outline,
		// Token: 0x04001082 RID: 4226
		InlineImages
	}

	// Token: 0x020001E9 RID: 489
	private enum Fonts
	{
		// Token: 0x04001084 RID: 4228
		BlizzardGlobal,
		// Token: 0x04001085 RID: 4229
		Belwe,
		// Token: 0x04001086 RID: 4230
		BelweOutline,
		// Token: 0x04001087 RID: 4231
		FranklinGothic
	}

	// Token: 0x020001EA RID: 490
	private enum ThaiGlyphType
	{
		// Token: 0x04001089 RID: 4233
		BASE,
		// Token: 0x0400108A RID: 4234
		BASE_ASCENDER,
		// Token: 0x0400108B RID: 4235
		BASE_DESCENDER,
		// Token: 0x0400108C RID: 4236
		TONE_MARK,
		// Token: 0x0400108D RID: 4237
		UPPER,
		// Token: 0x0400108E RID: 4238
		LOWER
	}
}
