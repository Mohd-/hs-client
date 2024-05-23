using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004E6 RID: 1254
public class CraftingTray : MonoBehaviour
{
	// Token: 0x06003AD3 RID: 15059 RVA: 0x0011C2C4 File Offset: 0x0011A4C4
	private void Awake()
	{
		CraftingTray.s_instance = this;
	}

	// Token: 0x06003AD4 RID: 15060 RVA: 0x0011C2CC File Offset: 0x0011A4CC
	private void Start()
	{
		this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDoneButtonReleased));
		this.m_massDisenchantButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnMassDisenchantButtonReleased));
		this.m_massDisenchantButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnMassDisenchantButtonOver));
		this.m_massDisenchantButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnMassDisenchantButtonOut));
		this.m_showGoldenCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleShowGolden));
		this.m_showGoldenCheckbox.SetChecked(false);
		this.m_showSoulboundCheckbox.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ToggleShowSoulbound));
		this.m_showSoulboundCheckbox.SetChecked(false);
		this.SetMassDisenchantAmount();
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x0011C38D File Offset: 0x0011A58D
	private void OnDestroy()
	{
		CraftingTray.s_instance = null;
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x0011C395 File Offset: 0x0011A595
	public static CraftingTray Get()
	{
		return CraftingTray.s_instance;
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x0011C39C File Offset: 0x0011A59C
	public void UpdateMassDisenchantAmount()
	{
		if (this.m_dustAmount > 0)
		{
			Material[] materials = this.m_massDisenchantMesh.GetComponent<Renderer>().materials;
			materials[CraftingTray.MASS_DISENCHANT_MATERIAL_TO_SWITCH] = this.m_massDisenchantMaterial;
			this.m_massDisenchantMesh.GetComponent<Renderer>().materials = materials;
			this.m_highlight.gameObject.SetActive(true);
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			this.m_massDisenchantButton.SetEnabled(true);
			this.m_massDisenchantText.gameObject.SetActive(true);
			this.m_potentialDustAmount.gameObject.SetActive(true);
		}
		else
		{
			Material[] materials2 = this.m_massDisenchantMesh.GetComponent<Renderer>().materials;
			materials2[CraftingTray.MASS_DISENCHANT_MATERIAL_TO_SWITCH] = this.m_massDisenchantDisabledMaterial;
			this.m_massDisenchantMesh.GetComponent<Renderer>().materials = materials2;
			this.m_highlight.gameObject.SetActive(false);
			this.m_massDisenchantButton.SetEnabled(false);
			this.m_massDisenchantText.gameObject.SetActive(false);
			this.m_potentialDustAmount.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x0011C4B0 File Offset: 0x0011A6B0
	public void SetMassDisenchantAmount()
	{
		if (base.gameObject.activeSelf)
		{
			base.StartCoroutine(this.SetMassDisenchantAmountWhenReady());
		}
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x0011C4DC File Offset: 0x0011A6DC
	private IEnumerator SetMassDisenchantAmountWhenReady()
	{
		while (MassDisenchant.Get() == null)
		{
			yield return null;
		}
		MassDisenchant.Get().UpdateContents(CollectionManager.Get().GetMassDisenchantCards());
		int amount = MassDisenchant.Get().GetTotalAmount();
		this.m_dustAmount = amount;
		this.m_potentialDustAmount.Text = amount.ToString();
		this.UpdateMassDisenchantAmount();
		yield break;
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x0011C4F8 File Offset: 0x0011A6F8
	public void Show(bool? overrideShowSoulbound = null, bool? overrideShowGolden = null, bool updatePage = true)
	{
		this.m_shown = true;
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.CRAFTING
		});
		if (overrideShowSoulbound != null)
		{
			this.m_showSoulboundCheckbox.SetChecked(overrideShowSoulbound.Value);
		}
		if (overrideShowGolden != null)
		{
			this.m_showGoldenCheckbox.SetChecked(overrideShowGolden.Value);
		}
		this.SetMassDisenchantAmount();
		CollectionManagerDisplay.Get().m_pageManager.ShowCraftingModeCards(!this.m_showSoulboundCheckbox.IsChecked(), this.m_showGoldenCheckbox.IsChecked(), updatePage);
	}

	// Token: 0x06003ADB RID: 15067 RVA: 0x0011C594 File Offset: 0x0011A794
	public void Hide()
	{
		this.m_shown = false;
		PresenceMgr.Get().SetPrevStatus();
		CollectionManagerDisplay.Get().HideCraftingTray();
		CollectionManagerDisplay.Get().m_pageManager.HideMassDisenchant();
	}

	// Token: 0x06003ADC RID: 15068 RVA: 0x0011C5CC File Offset: 0x0011A7CC
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06003ADD RID: 15069 RVA: 0x0011C5D4 File Offset: 0x0011A7D4
	private void OnDoneButtonReleased(UIEvent e)
	{
		this.Hide();
	}

	// Token: 0x06003ADE RID: 15070 RVA: 0x0011C5DC File Offset: 0x0011A7DC
	private void OnMassDisenchantButtonReleased(UIEvent e)
	{
		if (!CollectionManagerDisplay.Get().m_pageManager.ArePagesTurning())
		{
			if (CollectionManagerDisplay.Get().m_pageManager.IsShowingMassDisenchant())
			{
				CollectionManagerDisplay.Get().m_pageManager.HideMassDisenchant();
				this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			}
			else
			{
				CollectionManagerDisplay.Get().m_pageManager.ShowMassDisenchant();
				base.StartCoroutine(MassDisenchant.Get().StartHighlight());
			}
			SoundManager.Get().LoadAndPlay("Hub_Click");
		}
	}

	// Token: 0x06003ADF RID: 15071 RVA: 0x0011C662 File Offset: 0x0011A862
	private void OnMassDisenchantButtonOver(UIEvent e)
	{
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
		SoundManager.Get().LoadAndPlay("Hub_Mouseover");
	}

	// Token: 0x06003AE0 RID: 15072 RVA: 0x0011C684 File Offset: 0x0011A884
	private void OnMassDisenchantButtonOut(UIEvent e)
	{
		if (!CollectionManagerDisplay.Get().m_pageManager.IsShowingMassDisenchant())
		{
			if (int.Parse(this.m_potentialDustAmount.Text) > 0)
			{
				this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			else
			{
				this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			}
		}
	}

	// Token: 0x06003AE1 RID: 15073 RVA: 0x0011C6DC File Offset: 0x0011A8DC
	private void ToggleShowGolden(UIEvent e)
	{
		bool flag = this.m_showGoldenCheckbox.IsChecked();
		CollectionManagerDisplay.Get().m_pageManager.ShowCraftingModeCards(!this.m_showSoulboundCheckbox.IsChecked(), flag, true);
		if (flag)
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_on", base.gameObject);
		}
		else
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_off", base.gameObject);
		}
	}

	// Token: 0x06003AE2 RID: 15074 RVA: 0x0011C74C File Offset: 0x0011A94C
	private void ToggleShowSoulbound(UIEvent e)
	{
		bool flag = this.m_showSoulboundCheckbox.IsChecked();
		CollectionManagerDisplay.Get().m_pageManager.ShowCraftingModeCards(!flag, this.m_showGoldenCheckbox.IsChecked(), true);
		if (flag)
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_on", base.gameObject);
		}
		else
		{
			SoundManager.Get().LoadAndPlay("checkbox_toggle_off", base.gameObject);
		}
	}

	// Token: 0x0400257F RID: 9599
	public UIBButton m_doneButton;

	// Token: 0x04002580 RID: 9600
	public PegUIElement m_massDisenchantButton;

	// Token: 0x04002581 RID: 9601
	public UberText m_potentialDustAmount;

	// Token: 0x04002582 RID: 9602
	public UberText m_massDisenchantText;

	// Token: 0x04002583 RID: 9603
	public CheckBox m_showGoldenCheckbox;

	// Token: 0x04002584 RID: 9604
	public CheckBox m_showSoulboundCheckbox;

	// Token: 0x04002585 RID: 9605
	public HighlightState m_highlight;

	// Token: 0x04002586 RID: 9606
	public GameObject m_massDisenchantMesh;

	// Token: 0x04002587 RID: 9607
	public Material m_massDisenchantMaterial;

	// Token: 0x04002588 RID: 9608
	public Material m_massDisenchantDisabledMaterial;

	// Token: 0x04002589 RID: 9609
	private int m_dustAmount;

	// Token: 0x0400258A RID: 9610
	private bool m_shown;

	// Token: 0x0400258B RID: 9611
	private static CraftingTray s_instance;

	// Token: 0x0400258C RID: 9612
	private static PlatformDependentValue<int> MASS_DISENCHANT_MATERIAL_TO_SWITCH = new PlatformDependentValue<int>(PlatformCategory.Screen)
	{
		PC = 0,
		Phone = 1
	};
}
