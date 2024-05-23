using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006E8 RID: 1768
public class CollectionDeckTileActor : Actor
{
	// Token: 0x060048E3 RID: 18659 RVA: 0x0015C672 File Offset: 0x0015A872
	public override void Awake()
	{
		base.Awake();
		this.AssignSlider();
		this.AssignCardCount();
	}

	// Token: 0x060048E4 RID: 18660 RVA: 0x0015C688 File Offset: 0x0015A888
	public void UpdateDeckCardProperties(bool cardIsUnique, int numCards, bool useSliderAnimations)
	{
		if (cardIsUnique)
		{
			this.m_uniqueStar.SetActive(this.m_shown);
			this.m_countTextMesh.gameObject.SetActive(false);
		}
		else
		{
			this.m_uniqueStar.SetActive(false);
			this.m_countTextMesh.gameObject.SetActive(this.m_shown);
			this.m_countTextMesh.Text = Convert.ToString(numCards);
		}
		bool flag = cardIsUnique || numCards > 1;
		if (flag)
		{
			this.OpenSlider(useSliderAnimations);
		}
		else
		{
			this.CloseSlider(useSliderAnimations);
		}
	}

	// Token: 0x060048E5 RID: 18661 RVA: 0x0015C71C File Offset: 0x0015A91C
	public void UpdateMaterial(Material material)
	{
		if (material != null)
		{
			this.m_portraitMesh.GetComponent<MeshRenderer>().material = material;
		}
	}

	// Token: 0x060048E6 RID: 18662 RVA: 0x0015C73B File Offset: 0x0015A93B
	public void SetGhosted(CollectionDeckTileActor.GhostedState state)
	{
		this.m_ghosted = state;
	}

	// Token: 0x060048E7 RID: 18663 RVA: 0x0015C744 File Offset: 0x0015A944
	public override void SetPremium(TAG_PREMIUM premium)
	{
		base.SetPremium(premium);
		this.UpdateFrameMaterial();
	}

	// Token: 0x060048E8 RID: 18664 RVA: 0x0015C754 File Offset: 0x0015A954
	public void UpdateGhostTileEffect()
	{
		if (this.m_manaGem == null)
		{
			return;
		}
		this.UpdateFrameMaterial();
		CollectionDeckTileActor.DeckTileFrameColorSet deckTileFrameColorSet;
		Material material;
		if (this.m_ghosted == CollectionDeckTileActor.GhostedState.NONE)
		{
			deckTileFrameColorSet = this.m_normalColorSet;
			material = this.m_manaGemNormalMaterial;
		}
		else if (this.m_ghosted == CollectionDeckTileActor.GhostedState.BLUE)
		{
			deckTileFrameColorSet = this.m_ghostedColorSet;
			material = this.m_manaGemGhostedMaterial;
		}
		else
		{
			deckTileFrameColorSet = this.m_redColorSet;
			material = this.m_manaGemRedGhostedMaterial;
		}
		this.m_manaGem.material = material;
		this.m_countText.TextColor = deckTileFrameColorSet.m_countTextColor;
		this.m_nameTextMesh.TextColor = deckTileFrameColorSet.m_nameTextColor;
		this.m_costTextMesh.TextColor = deckTileFrameColorSet.m_costTextColor;
		if (this.m_countText.Outline)
		{
			this.m_countText.OutlineColor = deckTileFrameColorSet.m_outlineColor;
		}
		if (this.m_nameTextMesh.Outline)
		{
			this.m_nameTextMesh.OutlineColor = deckTileFrameColorSet.m_outlineColor;
		}
		if (this.m_costTextMesh.Outline)
		{
			this.m_costTextMesh.OutlineColor = deckTileFrameColorSet.m_outlineColor;
		}
		if (this.m_highlight && deckTileFrameColorSet.m_highlightMaterial)
		{
			this.m_highlight.GetComponent<Renderer>().material = deckTileFrameColorSet.m_highlightMaterial;
		}
		if (this.m_highlightGlow && deckTileFrameColorSet.m_highlightGlowMaterial)
		{
			this.m_highlightGlow.GetComponent<Renderer>().material = deckTileFrameColorSet.m_highlightGlowMaterial;
		}
		this.SetDesaturationAmount(this.GetPortraitMaterial(), deckTileFrameColorSet);
		this.SetDesaturationAmount(this.m_uniqueStar.GetComponent<MeshRenderer>().material, deckTileFrameColorSet);
	}

	// Token: 0x060048E9 RID: 18665 RVA: 0x0015C8FC File Offset: 0x0015AAFC
	protected override Material GetPortraitMaterial()
	{
		MeshRenderer component = this.m_portraitMesh.GetComponent<MeshRenderer>();
		return component.material;
	}

	// Token: 0x060048EA RID: 18666 RVA: 0x0015C91D File Offset: 0x0015AB1D
	private void SetDesaturationAmount(Material material, CollectionDeckTileActor.DeckTileFrameColorSet colorSet)
	{
		material.SetColor("_Color", colorSet.m_desatColor);
		material.SetFloat("_Desaturate", colorSet.m_desatAmount);
		material.SetFloat("_Contrast", colorSet.m_desatContrast);
	}

	// Token: 0x060048EB RID: 18667 RVA: 0x0015C954 File Offset: 0x0015AB54
	private void UpdateFrameMaterial()
	{
		Material material = this.m_standardFrameInteriorMaterial;
		Material material2;
		if (this.m_ghosted == CollectionDeckTileActor.GhostedState.BLUE)
		{
			material2 = this.m_ghostedFrameMaterial;
		}
		else if (this.m_ghosted == CollectionDeckTileActor.GhostedState.RED)
		{
			material2 = this.m_redFrameMaterial;
			material = this.m_redFrameInteriorMaterial;
		}
		else
		{
			TAG_PREMIUM premium = base.GetPremium();
			if (premium != TAG_PREMIUM.GOLDEN)
			{
				material2 = this.m_standardFrameMaterial;
			}
			else
			{
				material2 = this.m_premiumFrameMaterial;
			}
		}
		if (material2 != null)
		{
			this.m_frame.GetComponent<Renderer>().material = material2;
		}
		if (material != null)
		{
			this.m_frameInterior.GetComponent<Renderer>().material = material;
		}
	}

	// Token: 0x060048EC RID: 18668 RVA: 0x0015CA08 File Offset: 0x0015AC08
	private void AssignSlider()
	{
		this.m_originalSliderLocalPos = this.m_slider.transform.localPosition;
		this.m_openSliderLocalPos = this.m_rootObject.transform.FindChild("OpenSliderPosition").transform.localPosition;
	}

	// Token: 0x060048ED RID: 18669 RVA: 0x0015CA50 File Offset: 0x0015AC50
	private void AssignCardCount()
	{
		this.m_countTextMesh = this.m_rootObject.transform.FindChild("CardCountText").GetComponent<UberText>();
	}

	// Token: 0x060048EE RID: 18670 RVA: 0x0015CA74 File Offset: 0x0015AC74
	private void OpenSlider(bool useSliderAnimations)
	{
		if (this.m_sliderIsOpen)
		{
			return;
		}
		this.m_sliderIsOpen = true;
		iTween.StopByName(this.m_slider.gameObject, "position");
		if (useSliderAnimations)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_openSliderLocalPos,
				"isLocal",
				true,
				"time",
				0.35f,
				"easetype",
				iTween.EaseType.easeOutBounce,
				"name",
				"position"
			});
			iTween.MoveTo(this.m_slider.gameObject, args);
		}
		else
		{
			this.m_slider.transform.localPosition = this.m_openSliderLocalPos;
		}
	}

	// Token: 0x060048EF RID: 18671 RVA: 0x0015CB48 File Offset: 0x0015AD48
	private void CloseSlider(bool useSliderAnimations)
	{
		if (!this.m_sliderIsOpen)
		{
			return;
		}
		this.m_sliderIsOpen = false;
		iTween.StopByName(this.m_slider.gameObject, "position");
		if (useSliderAnimations)
		{
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_originalSliderLocalPos,
				"isLocal",
				true,
				"time",
				0.35f,
				"easetype",
				iTween.EaseType.easeOutBounce,
				"name",
				"position"
			});
			iTween.MoveTo(this.m_slider.gameObject, args);
		}
		else
		{
			this.m_slider.transform.localPosition = this.m_originalSliderLocalPos;
		}
	}

	// Token: 0x04003009 RID: 12297
	private const float SLIDER_ANIM_TIME = 0.35f;

	// Token: 0x0400300A RID: 12298
	public Material m_standardFrameMaterial;

	// Token: 0x0400300B RID: 12299
	public Material m_premiumFrameMaterial;

	// Token: 0x0400300C RID: 12300
	public Material m_standardFrameInteriorMaterial;

	// Token: 0x0400300D RID: 12301
	public GameObject m_frame;

	// Token: 0x0400300E RID: 12302
	public GameObject m_frameInterior;

	// Token: 0x0400300F RID: 12303
	public GameObject m_uniqueStar;

	// Token: 0x04003010 RID: 12304
	public GameObject m_highlight;

	// Token: 0x04003011 RID: 12305
	public GameObject m_highlightGlow;

	// Token: 0x04003012 RID: 12306
	public UberText m_countText;

	// Token: 0x04003013 RID: 12307
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_ghostedFrameMaterial;

	// Token: 0x04003014 RID: 12308
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_redFrameMaterial;

	// Token: 0x04003015 RID: 12309
	[CustomEditField(Sections = "Ghosting Effect")]
	public MeshRenderer m_manaGem;

	// Token: 0x04003016 RID: 12310
	[CustomEditField(Sections = "Ghosting Effect")]
	public MeshRenderer m_slider;

	// Token: 0x04003017 RID: 12311
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_manaGemNormalMaterial;

	// Token: 0x04003018 RID: 12312
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_manaGemGhostedMaterial;

	// Token: 0x04003019 RID: 12313
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_manaGemRedGhostedMaterial;

	// Token: 0x0400301A RID: 12314
	[CustomEditField(Sections = "Ghosting Effect")]
	public Material m_redFrameInteriorMaterial;

	// Token: 0x0400301B RID: 12315
	[CustomEditField(Sections = "Ghosting Effect")]
	public CollectionDeckTileActor.DeckTileFrameColorSet m_normalColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();

	// Token: 0x0400301C RID: 12316
	[CustomEditField(Sections = "Ghosting Effect")]
	public CollectionDeckTileActor.DeckTileFrameColorSet m_ghostedColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();

	// Token: 0x0400301D RID: 12317
	[CustomEditField(Sections = "Ghosting Effect")]
	public CollectionDeckTileActor.DeckTileFrameColorSet m_redColorSet = new CollectionDeckTileActor.DeckTileFrameColorSet();

	// Token: 0x0400301E RID: 12318
	private UberText m_countTextMesh;

	// Token: 0x0400301F RID: 12319
	private bool m_sliderIsOpen;

	// Token: 0x04003020 RID: 12320
	private Vector3 m_originalSliderLocalPos;

	// Token: 0x04003021 RID: 12321
	private Vector3 m_openSliderLocalPos;

	// Token: 0x04003022 RID: 12322
	private CollectionDeckTileActor.GhostedState m_ghosted;

	// Token: 0x02000737 RID: 1847
	public enum GhostedState
	{
		// Token: 0x04003208 RID: 12808
		NONE,
		// Token: 0x04003209 RID: 12809
		BLUE,
		// Token: 0x0400320A RID: 12810
		RED
	}

	// Token: 0x02000766 RID: 1894
	[Serializable]
	public class DeckTileFrameColorSet
	{
		// Token: 0x040032DE RID: 13022
		public Color m_desatColor = Color.white;

		// Token: 0x040032DF RID: 13023
		public float m_desatContrast;

		// Token: 0x040032E0 RID: 13024
		public float m_desatAmount;

		// Token: 0x040032E1 RID: 13025
		public Color m_costTextColor = Color.white;

		// Token: 0x040032E2 RID: 13026
		public Color m_countTextColor = new Color(1f, 0.9f, 0f, 1f);

		// Token: 0x040032E3 RID: 13027
		public Color m_nameTextColor = Color.white;

		// Token: 0x040032E4 RID: 13028
		public Color m_sliderColor = new Color(0.62f, 0.62f, 0.62f, 1f);

		// Token: 0x040032E5 RID: 13029
		public Color m_outlineColor = Color.black;

		// Token: 0x040032E6 RID: 13030
		public Material m_highlightMaterial;

		// Token: 0x040032E7 RID: 13031
		public Material m_highlightGlowMaterial;
	}
}
