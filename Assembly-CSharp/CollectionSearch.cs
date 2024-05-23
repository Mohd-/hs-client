using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BD RID: 1725
public class CollectionSearch : MonoBehaviour
{
	// Token: 0x060047DF RID: 18399 RVA: 0x001593A0 File Offset: 0x001575A0
	private void Start()
	{
		this.m_background.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackgroundReleased));
		this.m_clearButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClearReleased));
		W8Touch.Get().VirtualKeyboardDidShow += new Action(this.OnKeyboardShown);
		W8Touch.Get().VirtualKeyboardDidHide += new Action(this.OnKeyboardHidden);
		if (this.m_background.GetComponent<Renderer>() != null)
		{
			this.m_origSearchMaterial = this.m_background.GetComponent<Renderer>().material;
		}
		this.m_origSearchPos = base.transform.localPosition;
		this.UpdateSearchText();
	}

	// Token: 0x060047E0 RID: 18400 RVA: 0x00159450 File Offset: 0x00157650
	private void OnDestroy()
	{
		W8Touch.Get().VirtualKeyboardDidShow -= new Action(this.OnKeyboardShown);
		W8Touch.Get().VirtualKeyboardDidHide -= new Action(this.OnKeyboardHidden);
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		}
	}

	// Token: 0x060047E1 RID: 18401 RVA: 0x001594AA File Offset: 0x001576AA
	public bool IsActive()
	{
		return this.m_isActive;
	}

	// Token: 0x060047E2 RID: 18402 RVA: 0x001594B2 File Offset: 0x001576B2
	public void SetActiveLayer(GameLayer activeLayer)
	{
		if (activeLayer == this.m_activeLayer)
		{
			return;
		}
		this.m_activeLayer = activeLayer;
		if (!this.IsActive())
		{
			return;
		}
		this.MoveToActiveLayer(false);
	}

	// Token: 0x060047E3 RID: 18403 RVA: 0x001594DC File Offset: 0x001576DC
	public void Activate(bool ignoreTouchMode = false)
	{
		if (this.m_isActive)
		{
			return;
		}
		this.MoveToActiveLayer(true);
		this.m_isActive = true;
		this.m_prevText = this.m_text;
		CollectionSearch.ActivatedListener[] array = this.m_activatedListeners.ToArray();
		foreach (CollectionSearch.ActivatedListener activatedListener in array)
		{
			activatedListener();
		}
		if (!ignoreTouchMode && ((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible()))
		{
			this.TouchKeyboardSearchDisplay(true);
		}
		else
		{
			this.ShowInput(true);
		}
	}

	// Token: 0x060047E4 RID: 18404 RVA: 0x0015957C File Offset: 0x0015777C
	public void Deactivate()
	{
		if (!this.m_isActive)
		{
			return;
		}
		this.MoveToOriginalLayer();
		this.m_isActive = false;
		this.HideInput();
		this.ResetSearchDisplay();
		CollectionSearch.DeactivatedListener[] array = this.m_deactivatedListeners.ToArray();
		foreach (CollectionSearch.DeactivatedListener deactivatedListener in array)
		{
			deactivatedListener(this.m_prevText, this.m_text);
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			Navigation.GoBack();
		}
	}

	// Token: 0x060047E5 RID: 18405 RVA: 0x001595FC File Offset: 0x001577FC
	public void Cancel()
	{
		if (!this.m_isActive)
		{
			return;
		}
		this.m_text = this.m_prevText;
		this.UpdateSearchText();
		this.Deactivate();
	}

	// Token: 0x060047E6 RID: 18406 RVA: 0x0015962D File Offset: 0x0015782D
	public string GetText()
	{
		return this.m_text;
	}

	// Token: 0x060047E7 RID: 18407 RVA: 0x00159638 File Offset: 0x00157838
	public void ClearFilter(bool updateVisuals = true)
	{
		this.m_text = string.Empty;
		this.UpdateSearchText();
		this.ClearInput();
		CollectionSearch.ClearedListener[] array = this.m_clearedListeners.ToArray();
		foreach (CollectionSearch.ClearedListener clearedListener in array)
		{
			clearedListener(updateVisuals);
		}
		if ((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible())
		{
			this.Deactivate();
		}
	}

	// Token: 0x060047E8 RID: 18408 RVA: 0x001596B7 File Offset: 0x001578B7
	public void RegisterActivatedListener(CollectionSearch.ActivatedListener listener)
	{
		if (this.m_activatedListeners.Contains(listener))
		{
			return;
		}
		this.m_activatedListeners.Add(listener);
	}

	// Token: 0x060047E9 RID: 18409 RVA: 0x001596D7 File Offset: 0x001578D7
	public void RemoveActivatedListener(CollectionSearch.ActivatedListener listener)
	{
		this.m_activatedListeners.Remove(listener);
	}

	// Token: 0x060047EA RID: 18410 RVA: 0x001596E6 File Offset: 0x001578E6
	public void RegisterDeactivatedListener(CollectionSearch.DeactivatedListener listener)
	{
		if (this.m_deactivatedListeners.Contains(listener))
		{
			return;
		}
		this.m_deactivatedListeners.Add(listener);
	}

	// Token: 0x060047EB RID: 18411 RVA: 0x00159706 File Offset: 0x00157906
	public void RemoveDeactivatedListener(CollectionSearch.DeactivatedListener listener)
	{
		this.m_deactivatedListeners.Remove(listener);
	}

	// Token: 0x060047EC RID: 18412 RVA: 0x00159715 File Offset: 0x00157915
	public void RegisterClearedListener(CollectionSearch.ClearedListener listener)
	{
		if (this.m_clearedListeners.Contains(listener))
		{
			return;
		}
		this.m_clearedListeners.Add(listener);
	}

	// Token: 0x060047ED RID: 18413 RVA: 0x00159735 File Offset: 0x00157935
	public void RemoveClearedListener(CollectionSearch.ClearedListener listener)
	{
		this.m_clearedListeners.Remove(listener);
	}

	// Token: 0x060047EE RID: 18414 RVA: 0x00159744 File Offset: 0x00157944
	public void SetEnabled(bool enabled)
	{
		this.m_background.SetEnabled(enabled);
		this.m_clearButton.SetEnabled(enabled);
	}

	// Token: 0x060047EF RID: 18415 RVA: 0x0015975E File Offset: 0x0015795E
	private void OnBackgroundReleased(UIEvent e)
	{
		this.Activate(false);
	}

	// Token: 0x060047F0 RID: 18416 RVA: 0x00159767 File Offset: 0x00157967
	private void OnClearReleased(UIEvent e)
	{
		this.ClearFilter(true);
	}

	// Token: 0x060047F1 RID: 18417 RVA: 0x00159770 File Offset: 0x00157970
	private void OnActivateAnimComplete()
	{
		this.ShowInput(true);
	}

	// Token: 0x060047F2 RID: 18418 RVA: 0x0015977C File Offset: 0x0015797C
	private void OnDeactivateAnimComplete()
	{
		CollectionSearch.DeactivatedListener[] array = this.m_deactivatedListeners.ToArray();
		foreach (CollectionSearch.DeactivatedListener deactivatedListener in array)
		{
			deactivatedListener(this.m_prevText, this.m_text);
		}
	}

	// Token: 0x060047F3 RID: 18419 RVA: 0x001597C4 File Offset: 0x001579C4
	private void ShowInput(bool fromActivate = true)
	{
		Bounds bounds = this.m_searchText.GetBounds();
		this.m_searchText.gameObject.SetActive(false);
		Rect rect = CameraUtils.CreateGUIViewportRect(Box.Get().GetCamera(), bounds.min, bounds.max);
		Color? color = default(Color?);
		if (W8Touch.Get().IsVirtualKeyboardVisible())
		{
			color = new Color?(this.m_altSearchColor);
		}
		UniversalInputManager.TextInputParams textInputParams = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_rect = rect,
			m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
			m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
			m_font = this.m_searchText.GetLocalizedFont(),
			m_text = this.m_text,
			m_color = color
		};
		textInputParams.m_showVirtualKeyboard = fromActivate;
		UniversalInputManager.Get().UseTextInput(textInputParams, false);
	}

	// Token: 0x060047F4 RID: 18420 RVA: 0x001598C4 File Offset: 0x00157AC4
	private void HideInput()
	{
		UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		this.m_searchText.gameObject.SetActive(true);
	}

	// Token: 0x060047F5 RID: 18421 RVA: 0x001598F4 File Offset: 0x00157AF4
	private void ClearInput()
	{
		if (!this.m_isActive)
		{
			return;
		}
		SoundManager.Get().LoadAndPlay("text_box_delete_text");
		UniversalInputManager.Get().SetInputText(string.Empty);
	}

	// Token: 0x060047F6 RID: 18422 RVA: 0x0015992B File Offset: 0x00157B2B
	private void OnInputUpdated(string input)
	{
		this.m_text = input;
		this.UpdateSearchText();
	}

	// Token: 0x060047F7 RID: 18423 RVA: 0x0015993C File Offset: 0x00157B3C
	private void OnInputComplete(string input)
	{
		this.m_text = input;
		this.UpdateSearchText();
		SoundManager.Get().LoadAndPlay("text_commit");
		this.Deactivate();
	}

	// Token: 0x060047F8 RID: 18424 RVA: 0x0015996B File Offset: 0x00157B6B
	private void OnInputCanceled(bool userRequested, GameObject requester)
	{
		this.Cancel();
	}

	// Token: 0x060047F9 RID: 18425 RVA: 0x00159974 File Offset: 0x00157B74
	private void UpdateSearchText()
	{
		if (string.IsNullOrEmpty(this.m_text))
		{
			this.m_searchText.Text = GameStrings.Get("GLUE_COLLECTION_SEARCH");
			this.m_clearButton.gameObject.SetActive(false);
		}
		else
		{
			this.m_searchText.Text = this.m_text;
			this.m_clearButton.gameObject.SetActive(true);
		}
	}

	// Token: 0x060047FA RID: 18426 RVA: 0x001599E0 File Offset: 0x00157BE0
	private void MoveToActiveLayer(bool saveOriginalLayer)
	{
		if (saveOriginalLayer)
		{
			this.m_originalLayer = (GameLayer)base.gameObject.layer;
		}
		SceneUtils.SetLayer(base.gameObject, this.m_activeLayer);
	}

	// Token: 0x060047FB RID: 18427 RVA: 0x00159A15 File Offset: 0x00157C15
	private void MoveToOriginalLayer()
	{
		SceneUtils.SetLayer(base.gameObject, this.m_originalLayer);
	}

	// Token: 0x060047FC RID: 18428 RVA: 0x00159A28 File Offset: 0x00157C28
	private void TouchKeyboardSearchDisplay(bool fromActivate = false)
	{
		if (this.m_isTouchKeyboardDisplayMode)
		{
			return;
		}
		this.m_isTouchKeyboardDisplayMode = true;
		if (this.m_background.GetComponent<Renderer>() != null)
		{
			this.m_background.GetComponent<Renderer>().material = this.m_altSearchMaterial;
		}
		base.transform.localPosition = CollectionManagerDisplay.Get().m_activeSearchBone_Win8.transform.localPosition;
		this.HideInput();
		this.ShowInput(fromActivate || W8Touch.Get().IsVirtualKeyboardVisible());
		this.m_xMesh.GetComponent<Renderer>().material.SetColor("_Color", this.m_altSearchColor);
	}

	// Token: 0x060047FD RID: 18429 RVA: 0x00159AD4 File Offset: 0x00157CD4
	private void ResetSearchDisplay()
	{
		if (!this.m_isTouchKeyboardDisplayMode)
		{
			return;
		}
		this.m_isTouchKeyboardDisplayMode = false;
		this.m_background.GetComponent<Renderer>().material = this.m_origSearchMaterial;
		base.transform.localPosition = this.m_origSearchPos;
		this.HideInput();
		this.ShowInput(false);
		this.m_xMesh.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
	}

	// Token: 0x060047FE RID: 18430 RVA: 0x00159B47 File Offset: 0x00157D47
	private void OnKeyboardShown()
	{
		if (this.m_isActive && !this.m_isTouchKeyboardDisplayMode)
		{
			this.TouchKeyboardSearchDisplay(false);
		}
	}

	// Token: 0x060047FF RID: 18431 RVA: 0x00159B66 File Offset: 0x00157D66
	private void OnKeyboardHidden()
	{
		if (this.m_isActive && this.m_isTouchKeyboardDisplayMode)
		{
			this.ResetSearchDisplay();
		}
	}

	// Token: 0x04002F65 RID: 12133
	private const float ANIM_TIME = 0.1f;

	// Token: 0x04002F66 RID: 12134
	private const int MAX_SEARCH_LENGTH = 31;

	// Token: 0x04002F67 RID: 12135
	public UberText m_searchText;

	// Token: 0x04002F68 RID: 12136
	public PegUIElement m_background;

	// Token: 0x04002F69 RID: 12137
	public PegUIElement m_clearButton;

	// Token: 0x04002F6A RID: 12138
	public GameObject m_xMesh;

	// Token: 0x04002F6B RID: 12139
	public Material m_altSearchMaterial;

	// Token: 0x04002F6C RID: 12140
	public Color m_altSearchColor;

	// Token: 0x04002F6D RID: 12141
	private Material m_origSearchMaterial;

	// Token: 0x04002F6E RID: 12142
	private Vector3 m_origSearchPos;

	// Token: 0x04002F6F RID: 12143
	private bool m_isActive;

	// Token: 0x04002F70 RID: 12144
	private string m_prevText;

	// Token: 0x04002F71 RID: 12145
	private string m_text;

	// Token: 0x04002F72 RID: 12146
	private List<CollectionSearch.ActivatedListener> m_activatedListeners = new List<CollectionSearch.ActivatedListener>();

	// Token: 0x04002F73 RID: 12147
	private List<CollectionSearch.DeactivatedListener> m_deactivatedListeners = new List<CollectionSearch.DeactivatedListener>();

	// Token: 0x04002F74 RID: 12148
	private List<CollectionSearch.ClearedListener> m_clearedListeners = new List<CollectionSearch.ClearedListener>();

	// Token: 0x04002F75 RID: 12149
	private GameLayer m_originalLayer;

	// Token: 0x04002F76 RID: 12150
	private GameLayer m_activeLayer;

	// Token: 0x04002F77 RID: 12151
	private bool m_isTouchKeyboardDisplayMode;

	// Token: 0x020006C7 RID: 1735
	// (Invoke) Token: 0x06004845 RID: 18501
	public delegate void ActivatedListener();

	// Token: 0x020006C8 RID: 1736
	// (Invoke) Token: 0x06004849 RID: 18505
	public delegate void DeactivatedListener(string oldSearchText, string newSearchText);

	// Token: 0x020006C9 RID: 1737
	// (Invoke) Token: 0x0600484D RID: 18509
	public delegate void ClearedListener(bool updateVisuals);
}
