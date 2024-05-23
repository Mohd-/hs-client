using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CA RID: 458
[CustomEditClass]
public class AdventureChooserButton : AdventureGenericButton
{
	// Token: 0x17000324 RID: 804
	// (get) Token: 0x06001D2A RID: 7466 RVA: 0x0008900D File Offset: 0x0008720D
	// (set) Token: 0x06001D2B RID: 7467 RVA: 0x00089015 File Offset: 0x00087215
	[CustomEditField(Sections = "Button Settings")]
	public float ButtonBottomPadding
	{
		get
		{
			return this.m_ButtonBottomPadding;
		}
		set
		{
			this.m_ButtonBottomPadding = value;
			this.UpdateButtonPositions();
		}
	}

	// Token: 0x17000325 RID: 805
	// (get) Token: 0x06001D2C RID: 7468 RVA: 0x00089024 File Offset: 0x00087224
	// (set) Token: 0x06001D2D RID: 7469 RVA: 0x0008902C File Offset: 0x0008722C
	[CustomEditField(Sections = "Sub Button Settings")]
	public float SubButtonHeight
	{
		get
		{
			return this.m_SubButtonHeight;
		}
		set
		{
			this.m_SubButtonHeight = value;
			this.UpdateButtonPositions();
		}
	}

	// Token: 0x17000326 RID: 806
	// (get) Token: 0x06001D2E RID: 7470 RVA: 0x0008903B File Offset: 0x0008723B
	// (set) Token: 0x06001D2F RID: 7471 RVA: 0x00089043 File Offset: 0x00087243
	[CustomEditField(Sections = "Sub Button Settings")]
	public float SubButtonContainerBtmPadding
	{
		get
		{
			return this.m_SubButtonContainerBtmPadding;
		}
		set
		{
			this.m_SubButtonContainerBtmPadding = value;
			this.UpdateButtonPositions();
		}
	}

	// Token: 0x17000327 RID: 807
	// (get) Token: 0x06001D30 RID: 7472 RVA: 0x00089052 File Offset: 0x00087252
	// (set) Token: 0x06001D31 RID: 7473 RVA: 0x0008905A File Offset: 0x0008725A
	[CustomEditField(Sections = "Button Settings")]
	public bool Toggle
	{
		get
		{
			return this.m_Toggled;
		}
		set
		{
			this.ToggleButton(value);
		}
	}

	// Token: 0x06001D32 RID: 7474 RVA: 0x00089064 File Offset: 0x00087264
	protected override void Awake()
	{
		base.Awake();
		this.m_SubButtonContainer.SetActive(this.m_Toggled);
		this.m_SubButtonContainer.transform.localPosition = this.GetHiddenPosition();
		this.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ToggleButton(!this.m_Toggled);
		});
		if (this.m_PortraitRenderer != null)
		{
			this.m_MainButtonExtents = this.m_PortraitRenderer.bounds.extents;
		}
	}

	// Token: 0x06001D33 RID: 7475 RVA: 0x000890DC File Offset: 0x000872DC
	public AdventureChooserSubButton[] GetSubButtons()
	{
		return this.m_SubButtons.ToArray();
	}

	// Token: 0x06001D34 RID: 7476 RVA: 0x000890EC File Offset: 0x000872EC
	public AdventureChooserSubButton GetSubButtonFromMode(AdventureModeDbId mode)
	{
		foreach (AdventureChooserSubButton adventureChooserSubButton in this.m_SubButtons)
		{
			if (adventureChooserSubButton.GetMode() == mode)
			{
				return adventureChooserSubButton;
			}
		}
		return null;
	}

	// Token: 0x06001D35 RID: 7477 RVA: 0x00089158 File Offset: 0x00087358
	public void SetAdventure(AdventureDbId id)
	{
		this.m_AdventureId = id;
	}

	// Token: 0x06001D36 RID: 7478 RVA: 0x00089161 File Offset: 0x00087361
	public AdventureDbId GetAdventure()
	{
		return this.m_AdventureId;
	}

	// Token: 0x06001D37 RID: 7479 RVA: 0x00089169 File Offset: 0x00087369
	public void SetSelectSubButtonOnToggle(bool flag)
	{
		this.m_SelectSubButtonOnToggle = flag;
	}

	// Token: 0x06001D38 RID: 7480 RVA: 0x00089174 File Offset: 0x00087374
	public AdventureChooserSubButton CreateSubButton(AdventureModeDbId id, AdventureSubDef subDef, string subButtonPrefab, bool useAsLastSelected)
	{
		if (this.m_SubButtonContainer == null)
		{
			Debug.LogError("m_SubButtonContainer cannot be null. Unable to create subbutton.", this);
			return null;
		}
		AdventureChooserSubButton newsubbutton = GameUtils.LoadGameObjectWithComponent<AdventureChooserSubButton>(subButtonPrefab);
		if (newsubbutton == null)
		{
			return null;
		}
		GameUtils.SetParent(newsubbutton, this.m_SubButtonContainer, false);
		if (useAsLastSelected || this.m_LastSelectedSubButton == null)
		{
			this.m_LastSelectedSubButton = newsubbutton;
		}
		GameUtils.SetAutomationName(newsubbutton.gameObject, new object[]
		{
			id
		});
		newsubbutton.SetAdventure(this.m_AdventureId, id);
		newsubbutton.SetButtonText(subDef.GetShortName());
		newsubbutton.SetPortraitTexture(subDef.m_Texture);
		newsubbutton.SetPortraitTiling(subDef.m_TextureTiling);
		newsubbutton.SetPortraitOffset(subDef.m_TextureOffset);
		newsubbutton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.OnSubButtonClicked(newsubbutton);
		});
		this.m_SubButtons.Add(newsubbutton);
		this.UpdateButtonPositions();
		this.m_SubButtonContainer.transform.localPosition = this.GetHiddenPosition();
		return newsubbutton;
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x000892C4 File Offset: 0x000874C4
	public void UpdateButtonPositions()
	{
		float subButtonHeight = this.m_SubButtonHeight;
		for (int i = 1; i < this.m_SubButtons.Count; i++)
		{
			AdventureChooserSubButton component = this.m_SubButtons[i];
			TransformUtil.SetLocalPosZ(component, subButtonHeight * (float)i);
		}
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0008930B File Offset: 0x0008750B
	public void AddVisualUpdatedListener(AdventureChooserButton.VisualUpdated dlg)
	{
		this.m_VisualUpdatedEventList.Add(dlg);
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x00089319 File Offset: 0x00087519
	public void AddToggleListener(AdventureChooserButton.Toggled dlg)
	{
		this.m_ToggleEventList.Add(dlg);
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x00089327 File Offset: 0x00087527
	public void AddModeSelectionListener(AdventureChooserButton.ModeSelection dlg)
	{
		this.m_ModeSelectionEventList.Add(dlg);
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x00089335 File Offset: 0x00087535
	public void AddExpandedListener(AdventureChooserButton.Expanded dlg)
	{
		this.m_ExpandedEventList.Add(dlg);
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x00089344 File Offset: 0x00087544
	public float GetFullButtonHeight()
	{
		if (this.m_PortraitRenderer == null || this.m_SubButtonContainer == null)
		{
			return TransformUtil.GetBoundsOfChildren(base.gameObject).size.z;
		}
		float num = this.m_SubButtonContainer.transform.localPosition.z + this.m_SubButtonHeight * (float)this.m_SubButtons.Count + this.m_SubButtonContainerBtmPadding;
		float num2 = this.m_PortraitRenderer.transform.localPosition.z - this.m_MainButtonExtents.z;
		float num3 = this.m_PortraitRenderer.transform.localPosition.z + this.m_MainButtonExtents.z;
		return Math.Max(num3, num) - num2 - this.m_ButtonBottomPadding;
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x00089424 File Offset: 0x00087624
	public void DisableSubButtonHighlights()
	{
		foreach (AdventureChooserSubButton adventureChooserSubButton in this.m_SubButtons)
		{
			adventureChooserSubButton.SetHighlight(false);
		}
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x00089480 File Offset: 0x00087680
	public bool ContainsSubButton(AdventureChooserSubButton btn)
	{
		return this.m_SubButtons.Exists((AdventureChooserSubButton x) => x == btn);
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x000894B4 File Offset: 0x000876B4
	public void ToggleButton(bool toggle)
	{
		if (toggle == this.m_Toggled)
		{
			return;
		}
		this.m_Toggled = toggle;
		this.m_ButtonStateTable.CancelQueuedStates();
		this.m_ButtonStateTable.TriggerState((!this.m_Toggled) ? "Contract" : "Expand", true, null);
		if (this.m_Toggled)
		{
			this.m_SubButtonContainer.SetActive(true);
		}
		Vector3 hiddenPosition = this.GetHiddenPosition();
		Vector3 showPosition = this.GetShowPosition();
		Vector3 vector = (!this.m_Toggled) ? showPosition : hiddenPosition;
		Vector3 vector2 = (!this.m_Toggled) ? hiddenPosition : showPosition;
		this.m_SubButtonContainer.transform.localPosition = vector;
		this.UpdateSubButtonsVisibility(vector, this.m_SubButtonShowPosZ);
		Hashtable args = iTween.Hash(new object[]
		{
			"islocal",
			true,
			"from",
			vector,
			"to",
			vector2,
			"time",
			this.m_SubButtonAnimationTime,
			"easeType",
			(!this.m_Toggled) ? this.m_DeactivateEaseType : this.m_ActivateEaseType,
			"oncomplete",
			"OnExpandAnimationComplete",
			"oncompletetarget",
			base.gameObject,
			"onupdate",
			delegate(object newVal)
			{
				this.OnButtonAnimating((Vector3)newVal, this.m_SubButtonShowPosZ);
			},
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(base.gameObject, args);
		this.FireToggleEvent();
		if (this.m_Toggled && this.m_SelectSubButtonOnToggle && this.m_LastSelectedSubButton != null)
		{
			this.OnSubButtonClicked(this.m_LastSelectedSubButton);
		}
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x0008968C File Offset: 0x0008788C
	private Vector3 GetHiddenPosition()
	{
		Vector3 localPosition = this.m_SubButtonContainer.transform.localPosition;
		return new Vector3(localPosition.x, localPosition.y, this.m_SubButtonShowPosZ - this.m_SubButtonHeight * (float)this.m_SubButtons.Count - this.m_SubButtonContainerBtmPadding);
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x000896E0 File Offset: 0x000878E0
	private Vector3 GetShowPosition()
	{
		Vector3 localPosition = this.m_SubButtonContainer.transform.localPosition;
		return new Vector3(localPosition.x, localPosition.y, this.m_SubButtonShowPosZ);
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x00089717 File Offset: 0x00087917
	private void OnButtonAnimating(Vector3 curr, float zposshowlimit)
	{
		this.m_SubButtonContainer.transform.localPosition = curr;
		this.UpdateSubButtonsVisibility(curr, zposshowlimit);
		this.FireVisualUpdatedEvent();
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x00089738 File Offset: 0x00087938
	private void UpdateSubButtonsVisibility(Vector3 curr, float zposshowlimit)
	{
		float subButtonHeight = this.m_SubButtonHeight;
		for (int i = 0; i < this.m_SubButtons.Count; i++)
		{
			float num = subButtonHeight * (float)(i + 1) + curr.z;
			bool flag = zposshowlimit - num <= this.m_SubButtonVisibilityPadding;
			GameObject gameObject = this.m_SubButtons[i].gameObject;
			if (gameObject.activeSelf != flag)
			{
				gameObject.SetActive(flag);
			}
		}
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x000897AE File Offset: 0x000879AE
	private void OnExpandAnimationComplete()
	{
		if (this.m_SubButtonContainer.activeSelf != this.m_Toggled)
		{
			this.m_SubButtonContainer.SetActive(this.m_Toggled);
		}
		this.FireExpandedEvent(this.m_Toggled);
	}

	// Token: 0x06001D47 RID: 7495 RVA: 0x000897E4 File Offset: 0x000879E4
	private void FireVisualUpdatedEvent()
	{
		AdventureChooserButton.VisualUpdated[] array = this.m_VisualUpdatedEventList.ToArray();
		foreach (AdventureChooserButton.VisualUpdated visualUpdated in array)
		{
			visualUpdated();
		}
	}

	// Token: 0x06001D48 RID: 7496 RVA: 0x00089820 File Offset: 0x00087A20
	private void FireToggleEvent()
	{
		AdventureChooserButton.Toggled[] array = this.m_ToggleEventList.ToArray();
		foreach (AdventureChooserButton.Toggled toggled in array)
		{
			toggled(this.m_Toggled);
		}
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x00089860 File Offset: 0x00087A60
	private void FireModeSelectedEvent(AdventureChooserSubButton btn)
	{
		AdventureChooserButton.ModeSelection[] array = this.m_ModeSelectionEventList.ToArray();
		foreach (AdventureChooserButton.ModeSelection modeSelection in array)
		{
			modeSelection(btn);
		}
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x0008989C File Offset: 0x00087A9C
	private void FireExpandedEvent(bool expand)
	{
		AdventureChooserButton.Expanded[] array = this.m_ExpandedEventList.ToArray();
		foreach (AdventureChooserButton.Expanded expanded in array)
		{
			expanded(this, expand);
		}
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x000898D8 File Offset: 0x00087AD8
	private void OnSubButtonClicked(AdventureChooserSubButton btn)
	{
		this.m_LastSelectedSubButton = btn;
		this.FireModeSelectedEvent(btn);
		foreach (AdventureChooserSubButton adventureChooserSubButton in this.m_SubButtons)
		{
			adventureChooserSubButton.SetHighlight(adventureChooserSubButton == btn);
		}
	}

	// Token: 0x04000FE3 RID: 4067
	private const string s_EventButtonExpand = "Expand";

	// Token: 0x04000FE4 RID: 4068
	private const string s_EventButtonContract = "Contract";

	// Token: 0x04000FE5 RID: 4069
	[CustomEditField(Sections = "Button State Table")]
	[SerializeField]
	public StateEventTable m_ButtonStateTable;

	// Token: 0x04000FE6 RID: 4070
	[SerializeField]
	private float m_ButtonBottomPadding;

	// Token: 0x04000FE7 RID: 4071
	[CustomEditField(Sections = "Sub Button Settings")]
	[SerializeField]
	public GameObject m_SubButtonContainer;

	// Token: 0x04000FE8 RID: 4072
	[SerializeField]
	private float m_SubButtonHeight = 3.75f;

	// Token: 0x04000FE9 RID: 4073
	[SerializeField]
	private float m_SubButtonContainerBtmPadding = 0.1f;

	// Token: 0x04000FEA RID: 4074
	[SerializeField]
	[CustomEditField(Sections = "Sub Button Settings")]
	public iTween.EaseType m_ActivateEaseType = iTween.EaseType.easeOutBounce;

	// Token: 0x04000FEB RID: 4075
	[CustomEditField(Sections = "Sub Button Settings")]
	[SerializeField]
	public iTween.EaseType m_DeactivateEaseType = iTween.EaseType.easeOutSine;

	// Token: 0x04000FEC RID: 4076
	[SerializeField]
	[CustomEditField(Sections = "Sub Button Settings")]
	public float m_SubButtonVisibilityPadding = 5f;

	// Token: 0x04000FED RID: 4077
	[SerializeField]
	[CustomEditField(Sections = "Sub Button Settings")]
	public float m_SubButtonAnimationTime = 0.25f;

	// Token: 0x04000FEE RID: 4078
	[SerializeField]
	[CustomEditField(Sections = "Sub Button Settings")]
	public float m_SubButtonShowPosZ;

	// Token: 0x04000FEF RID: 4079
	private bool m_Toggled;

	// Token: 0x04000FF0 RID: 4080
	private bool m_SelectSubButtonOnToggle;

	// Token: 0x04000FF1 RID: 4081
	private Vector3 m_MainButtonExtents = Vector3.zero;

	// Token: 0x04000FF2 RID: 4082
	private AdventureDbId m_AdventureId;

	// Token: 0x04000FF3 RID: 4083
	private List<AdventureChooserSubButton> m_SubButtons = new List<AdventureChooserSubButton>();

	// Token: 0x04000FF4 RID: 4084
	private List<AdventureChooserButton.VisualUpdated> m_VisualUpdatedEventList = new List<AdventureChooserButton.VisualUpdated>();

	// Token: 0x04000FF5 RID: 4085
	private List<AdventureChooserButton.Toggled> m_ToggleEventList = new List<AdventureChooserButton.Toggled>();

	// Token: 0x04000FF6 RID: 4086
	private List<AdventureChooserButton.ModeSelection> m_ModeSelectionEventList = new List<AdventureChooserButton.ModeSelection>();

	// Token: 0x04000FF7 RID: 4087
	private List<AdventureChooserButton.Expanded> m_ExpandedEventList = new List<AdventureChooserButton.Expanded>();

	// Token: 0x04000FF8 RID: 4088
	private AdventureChooserSubButton m_LastSelectedSubButton;

	// Token: 0x020001CC RID: 460
	// (Invoke) Token: 0x06001D5B RID: 7515
	public delegate void VisualUpdated();

	// Token: 0x020001CD RID: 461
	// (Invoke) Token: 0x06001D5F RID: 7519
	public delegate void Toggled(bool toggle);

	// Token: 0x020001CE RID: 462
	// (Invoke) Token: 0x06001D63 RID: 7523
	public delegate void ModeSelection(AdventureChooserSubButton btn);

	// Token: 0x020001D0 RID: 464
	// (Invoke) Token: 0x06001D70 RID: 7536
	public delegate void Expanded(AdventureChooserButton button, bool expand);
}
