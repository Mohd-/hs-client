using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000370 RID: 880
public class HeroPickerButton : PegUIElement
{
	// Token: 0x06002D2E RID: 11566 RVA: 0x000E25F9 File Offset: 0x000E07F9
	public void SetPreconDeckID(long preconDeckID)
	{
		this.m_preconDeckID = preconDeckID;
	}

	// Token: 0x06002D2F RID: 11567 RVA: 0x000E2602 File Offset: 0x000E0802
	public long GetPreconDeckID()
	{
		return this.m_preconDeckID;
	}

	// Token: 0x06002D30 RID: 11568 RVA: 0x000E260A File Offset: 0x000E080A
	public bool IsDeckValid()
	{
		return this.m_isDeckValid;
	}

	// Token: 0x06002D31 RID: 11569 RVA: 0x000E2614 File Offset: 0x000E0814
	public void SetIsDeckValid(bool isValid)
	{
		if (this.m_isDeckValid == isValid)
		{
			return;
		}
		this.m_isDeckValid = isValid;
		this.m_invalidDeckX.SetActive(!this.m_isDeckValid);
	}

	// Token: 0x06002D32 RID: 11570 RVA: 0x000E264C File Offset: 0x000E084C
	public void UpdateDisplay(FullDef def, TAG_PREMIUM premium)
	{
		this.m_heroClass = def.GetEntityDef().GetClass();
		this.SetFullDef(def);
		this.SetClassname(GameStrings.GetClassName(this.m_heroClass));
		this.SetClassIcon(this.GetClassIconMaterial(this.m_heroClass));
		this.SetBasicSetProgress(this.m_heroClass);
		this.SetPremium(premium);
	}

	// Token: 0x06002D33 RID: 11571 RVA: 0x000E26A8 File Offset: 0x000E08A8
	public void SetClassIcon(Material mat)
	{
		this.m_heroClassIcon.GetComponent<Renderer>().material = mat;
		this.m_heroClassIcon.GetComponent<Renderer>().material.renderQueue = 3007;
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x000E26E0 File Offset: 0x000E08E0
	public void SetClassname(string s)
	{
		this.m_classLabel.Text = s;
	}

	// Token: 0x06002D35 RID: 11573 RVA: 0x000E26EE File Offset: 0x000E08EE
	public void SetFullDef(FullDef def)
	{
		this.m_fullDef = def;
		this.UpdatePortrait();
	}

	// Token: 0x06002D36 RID: 11574 RVA: 0x000E26FD File Offset: 0x000E08FD
	public FullDef GetFullDef()
	{
		return this.m_fullDef;
	}

	// Token: 0x06002D37 RID: 11575 RVA: 0x000E2705 File Offset: 0x000E0905
	public void SetSelected(bool isSelected)
	{
		this.m_isSelected = isSelected;
		if (isSelected)
		{
			this.Lower();
		}
		else
		{
			this.Raise();
		}
	}

	// Token: 0x06002D38 RID: 11576 RVA: 0x000E2725 File Offset: 0x000E0925
	public bool IsSelected()
	{
		return this.m_isSelected;
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x000E2730 File Offset: 0x000E0930
	public void SetBasicSetProgress(TAG_CLASS classTag)
	{
		int basicCardsIOwn = CollectionManager.Get().GetBasicCardsIOwn(classTag);
		int num = 20;
		if (basicCardsIOwn == num)
		{
			this.m_classLabel.transform.position = this.m_bones.m_classLabelOneLine.position;
			this.m_labelGradient.transform.parent = this.m_bones.m_gradientOneLine;
			this.m_labelGradient.transform.localPosition = Vector3.zero;
			this.m_labelGradient.transform.localScale = Vector3.one;
			this.m_setProgressLabel.gameObject.SetActive(false);
		}
		else
		{
			this.m_classLabel.transform.position = this.m_bones.m_classLabelTwoLine.position;
			this.m_labelGradient.transform.parent = this.m_bones.m_gradientTwoLine;
			this.m_setProgressLabel.Text = GameStrings.Format((!UniversalInputManager.UsePhoneUI) ? "GLUE_BASIC_SET_PROGRESS" : "GLUE_BASIC_SET_PROGRESS_PHONE", new object[]
			{
				basicCardsIOwn,
				num
			});
			this.m_labelGradient.transform.localPosition = Vector3.zero;
			this.m_labelGradient.transform.localScale = Vector3.one;
			this.m_setProgressLabel.gameObject.SetActive(true);
			this.m_setProgressLabel.TextColor = HeroPickerButton.BASIC_SET_COLOR_IN_PROGRESS;
		}
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x000E289C File Offset: 0x000E0A9C
	public void Lower()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.Activate(false);
		}
		float num;
		if (this.m_locked)
		{
			num = 0.7f;
		}
		else
		{
			num = -0.7f;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			new Vector3(base.GetOriginalLocalPosition().x, base.GetOriginalLocalPosition().y + num, base.GetOriginalLocalPosition().z),
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x000E2970 File Offset: 0x000E0B70
	public void Raise()
	{
		if (this.m_isSelected)
		{
			return;
		}
		this.Activate(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			new Vector3(base.GetOriginalLocalPosition().x, base.GetOriginalLocalPosition().y, base.GetOriginalLocalPosition().z),
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06002D3C RID: 11580 RVA: 0x000E2A20 File Offset: 0x000E0C20
	public void SetHighlightState(ActorStateType stateType)
	{
		if (this.m_highlightState == null)
		{
			Transform parent = base.gameObject.transform.parent;
			this.m_highlightState = parent.GetComponentInChildren<HighlightState>();
		}
		if (this.m_highlightState != null)
		{
			this.m_highlightState.ChangeState(stateType);
		}
	}

	// Token: 0x06002D3D RID: 11581 RVA: 0x000E2A79 File Offset: 0x000E0C79
	public void Activate(bool enable)
	{
		this.SetEnabled(enable);
	}

	// Token: 0x06002D3E RID: 11582 RVA: 0x000E2A84 File Offset: 0x000E0C84
	public void Lock()
	{
		base.transform.parent.transform.localEulerAngles = new Vector3(0f, 180f, 180f);
		this.m_locked = true;
	}

	// Token: 0x06002D3F RID: 11583 RVA: 0x000E2AC1 File Offset: 0x000E0CC1
	public void SetProgress(int acknowledgedProgress, int currProgress, int maxProgress)
	{
		this.SetProgress(acknowledgedProgress, currProgress, maxProgress, true);
	}

	// Token: 0x06002D40 RID: 11584 RVA: 0x000E2AD0 File Offset: 0x000E0CD0
	public void SetProgress(int acknowledgedProgress, int currProgress, int maxProgress, bool shouldAnimate)
	{
		bool flag = acknowledgedProgress == currProgress;
		if (currProgress == maxProgress)
		{
			this.Unlock(!flag && shouldAnimate);
		}
	}

	// Token: 0x06002D41 RID: 11585 RVA: 0x000E2AFA File Offset: 0x000E0CFA
	public bool IsLocked()
	{
		return this.m_locked;
	}

	// Token: 0x06002D42 RID: 11586 RVA: 0x000E2B02 File Offset: 0x000E0D02
	public void SetUnlockedCallback(HeroPickerButton.UnlockedCallback unlockedCallback)
	{
		this.m_unlockedCallback = unlockedCallback;
	}

	// Token: 0x06002D43 RID: 11587 RVA: 0x000E2B0B File Offset: 0x000E0D0B
	public TAG_PREMIUM GetPremium()
	{
		return this.m_premium;
	}

	// Token: 0x06002D44 RID: 11588 RVA: 0x000E2B13 File Offset: 0x000E0D13
	public void SetPremium(TAG_PREMIUM premium)
	{
		this.m_premium = premium;
		this.UpdatePortrait();
	}

	// Token: 0x06002D45 RID: 11589 RVA: 0x000E2B24 File Offset: 0x000E0D24
	private void Unlock(bool animate)
	{
		base.transform.parent.localEulerAngles = new Vector3(0f, 180f, 0f);
		this.UnlockAfterAnimate();
	}

	// Token: 0x06002D46 RID: 11590 RVA: 0x000E2B5B File Offset: 0x000E0D5B
	private void UnlockAfterAnimate()
	{
		this.m_locked = false;
		if (this.m_unlockedCallback != null)
		{
			this.m_unlockedCallback(this);
		}
	}

	// Token: 0x06002D47 RID: 11591 RVA: 0x000E2B7C File Offset: 0x000E0D7C
	private Material GetClassIconMaterial(TAG_CLASS classTag)
	{
		int num = 0;
		switch (classTag)
		{
		case TAG_CLASS.INVALID:
			num = 9;
			break;
		case TAG_CLASS.DRUID:
			num = 5;
			break;
		case TAG_CLASS.HUNTER:
			num = 4;
			break;
		case TAG_CLASS.MAGE:
			num = 7;
			break;
		case TAG_CLASS.PALADIN:
			num = 3;
			break;
		case TAG_CLASS.PRIEST:
			num = 8;
			break;
		case TAG_CLASS.ROGUE:
			num = 2;
			break;
		case TAG_CLASS.SHAMAN:
			num = 1;
			break;
		case TAG_CLASS.WARLOCK:
			num = 6;
			break;
		case TAG_CLASS.WARRIOR:
			num = 0;
			break;
		}
		return this.CLASS_MATERIALS[num];
	}

	// Token: 0x06002D48 RID: 11592 RVA: 0x000E2C18 File Offset: 0x000E0E18
	private void UpdatePortrait()
	{
		if (this.m_fullDef == null)
		{
			return;
		}
		CardDef cardDef = this.m_fullDef.GetCardDef();
		if (cardDef == null)
		{
			return;
		}
		Material deckPickerPortrait = cardDef.GetDeckPickerPortrait();
		if (deckPickerPortrait == null)
		{
			return;
		}
		DeckPickerHero component = base.GetComponent<DeckPickerHero>();
		Material premiumPortraitMaterial = cardDef.GetPremiumPortraitMaterial();
		if (this.m_premium == TAG_PREMIUM.GOLDEN && premiumPortraitMaterial != null)
		{
			component.m_PortraitMesh.GetComponent<Renderer>().material = premiumPortraitMaterial;
			component.m_PortraitMesh.GetComponent<Renderer>().material.mainTextureOffset = deckPickerPortrait.mainTextureOffset;
			component.m_PortraitMesh.GetComponent<Renderer>().material.mainTextureScale = deckPickerPortrait.mainTextureScale;
			component.m_PortraitMesh.GetComponent<Renderer>().material.SetTexture("_ShadowTex", null);
			float? seed = this.m_seed;
			if (seed == null)
			{
				this.m_seed = new float?(Random.value);
			}
			if (component.m_PortraitMesh.GetComponent<Renderer>().material.HasProperty("_Seed"))
			{
				component.m_PortraitMesh.GetComponent<Renderer>().material.SetFloat("_Seed", this.m_seed.Value);
			}
		}
		else
		{
			component.m_PortraitMesh.GetComponent<Renderer>().sharedMaterial = deckPickerPortrait;
		}
	}

	// Token: 0x06002D49 RID: 11593 RVA: 0x000E2D67 File Offset: 0x000E0F67
	protected override void OnRelease()
	{
		if (this.m_isDeckValid)
		{
			this.Lower();
		}
	}

	// Token: 0x04001BFE RID: 7166
	public GameObject m_heroClassIcon;

	// Token: 0x04001BFF RID: 7167
	public UberText m_classLabel;

	// Token: 0x04001C00 RID: 7168
	public UberText m_setProgressLabel;

	// Token: 0x04001C01 RID: 7169
	public GameObject m_labelGradient;

	// Token: 0x04001C02 RID: 7170
	public GameObject m_buttonFrame;

	// Token: 0x04001C03 RID: 7171
	public TAG_CLASS m_heroClass;

	// Token: 0x04001C04 RID: 7172
	public List<Material> CLASS_MATERIALS = new List<Material>();

	// Token: 0x04001C05 RID: 7173
	public HeroPickerButtonBones m_bones;

	// Token: 0x04001C06 RID: 7174
	public GameObject m_invalidDeckX;

	// Token: 0x04001C07 RID: 7175
	private FullDef m_fullDef;

	// Token: 0x04001C08 RID: 7176
	private bool m_isSelected;

	// Token: 0x04001C09 RID: 7177
	private HighlightState m_highlightState;

	// Token: 0x04001C0A RID: 7178
	private bool m_locked;

	// Token: 0x04001C0B RID: 7179
	private long m_preconDeckID;

	// Token: 0x04001C0C RID: 7180
	private HeroPickerButton.UnlockedCallback m_unlockedCallback;

	// Token: 0x04001C0D RID: 7181
	private TAG_PREMIUM m_premium;

	// Token: 0x04001C0E RID: 7182
	private float? m_seed;

	// Token: 0x04001C0F RID: 7183
	private bool m_isDeckValid = true;

	// Token: 0x04001C10 RID: 7184
	private static readonly Color BASIC_SET_COLOR_IN_PROGRESS = new Color(0.97f, 0.82f, 0.22f);

	// Token: 0x020007D9 RID: 2009
	// (Invoke) Token: 0x06004E60 RID: 20064
	public delegate void UnlockedCallback(HeroPickerButton button);
}
