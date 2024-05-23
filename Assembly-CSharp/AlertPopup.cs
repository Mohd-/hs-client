using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200017B RID: 379
[CustomEditClass]
public class AlertPopup : DialogBase
{
	// Token: 0x060015E7 RID: 5607 RVA: 0x00067A94 File Offset: 0x00065C94
	protected override void Awake()
	{
		base.Awake();
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ButtonPress(AlertPopup.Response.OK);
		});
		this.m_confirmButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ButtonPress(AlertPopup.Response.CONFIRM);
		});
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.ButtonPress(AlertPopup.Response.CANCEL);
		});
	}

	// Token: 0x060015E8 RID: 5608 RVA: 0x00067AF2 File Offset: 0x00065CF2
	private void Start()
	{
		this.m_alertText.Text = GameStrings.Get("GLOBAL_OKAY");
	}

	// Token: 0x060015E9 RID: 5609 RVA: 0x00067B09 File Offset: 0x00065D09
	private void OnDestroy()
	{
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().SetSystemDialogActive(false);
		}
	}

	// Token: 0x060015EA RID: 5610 RVA: 0x00067B28 File Offset: 0x00065D28
	public override bool HandleKeyboardInput()
	{
		if (Input.GetKeyUp(27) && this.m_popupInfo != null && this.m_popupInfo.m_keyboardEscIsCancel && this.m_cancelButton.enabled && this.m_cancelButton.gameObject.activeSelf)
		{
			this.GoBack();
			return true;
		}
		return false;
	}

	// Token: 0x060015EB RID: 5611 RVA: 0x00067B8A File Offset: 0x00065D8A
	public override void GoBack()
	{
		this.ButtonPress(AlertPopup.Response.CANCEL);
	}

	// Token: 0x060015EC RID: 5612 RVA: 0x00067B93 File Offset: 0x00065D93
	public void SetInfo(AlertPopup.PopupInfo info)
	{
		this.m_popupInfo = info;
	}

	// Token: 0x060015ED RID: 5613 RVA: 0x00067B9C File Offset: 0x00065D9C
	public AlertPopup.PopupInfo GetInfo()
	{
		return this.m_popupInfo;
	}

	// Token: 0x060015EE RID: 5614 RVA: 0x00067BA4 File Offset: 0x00065DA4
	public override void Show()
	{
		base.Show();
		this.InitInfo();
		this.UpdateAll(this.m_popupInfo);
		base.transform.localPosition += this.m_popupInfo.m_offset;
		if (this.m_popupInfo.m_layerToUse != null)
		{
			SceneUtils.SetLayer(this, this.m_popupInfo.m_layerToUse.Value);
		}
		if (this.m_popupInfo.m_disableBlocker)
		{
			this.m_clickCatcher.SetActive(false);
		}
		base.DoShowAnimation();
		bool systemDialogActive = this.m_popupInfo == null || this.m_popupInfo.m_layerToUse == null || this.m_popupInfo.m_layerToUse.Value == GameLayer.UI || this.m_popupInfo.m_layerToUse.Value == GameLayer.HighPriorityUI;
		UniversalInputManager.Get().SetSystemDialogActive(systemDialogActive);
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x00067C90 File Offset: 0x00065E90
	public void UpdateInfo(AlertPopup.PopupInfo info)
	{
		this.m_updateInfo = info;
		this.UpdateButtons(this.m_updateInfo.m_responseDisplay);
		if (this.m_showAnimState != DialogBase.ShowAnimState.IN_PROGRESS)
		{
			this.UpdateInfoAfterAnim();
		}
	}

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x060015F0 RID: 5616 RVA: 0x00067CC7 File Offset: 0x00065EC7
	// (set) Token: 0x060015F1 RID: 5617 RVA: 0x00067CD4 File Offset: 0x00065ED4
	public string BodyText
	{
		get
		{
			return this.m_alertText.Text;
		}
		set
		{
			this.m_alertText.Text = value;
			if (this.m_popupInfo != null)
			{
				this.UpdateLayout();
			}
		}
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x00067CF3 File Offset: 0x00065EF3
	protected override void OnHideAnimFinished()
	{
		UniversalInputManager.Get().SetSystemDialogActive(false);
		base.OnHideAnimFinished();
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00067D06 File Offset: 0x00065F06
	protected override void OnShowAnimFinished()
	{
		base.OnShowAnimFinished();
		if (this.m_updateInfo != null)
		{
			this.UpdateInfoAfterAnim();
		}
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00067D1F File Offset: 0x00065F1F
	private void InitInfo()
	{
		if (this.m_popupInfo == null)
		{
			this.m_popupInfo = new AlertPopup.PopupInfo();
		}
		if (this.m_popupInfo.m_headerText == null)
		{
			this.m_popupInfo.m_headerText = GameStrings.Get("GLOBAL_DEFAULT_ALERT_HEADER");
		}
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x00067D5C File Offset: 0x00065F5C
	private void UpdateButtons(AlertPopup.ResponseDisplay displayType)
	{
		this.m_confirmButton.gameObject.SetActive(false);
		this.m_cancelButton.gameObject.SetActive(false);
		this.m_okayButton.gameObject.SetActive(false);
		switch (displayType)
		{
		case AlertPopup.ResponseDisplay.OK:
			this.m_okayButton.gameObject.SetActive(true);
			break;
		case AlertPopup.ResponseDisplay.CONFIRM:
			this.m_confirmButton.gameObject.SetActive(true);
			break;
		case AlertPopup.ResponseDisplay.CANCEL:
			this.m_cancelButton.gameObject.SetActive(true);
			break;
		case AlertPopup.ResponseDisplay.CONFIRM_CANCEL:
			this.m_confirmButton.gameObject.SetActive(true);
			this.m_cancelButton.gameObject.SetActive(true);
			break;
		}
		this.m_buttonContainer.UpdateSlices();
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x00067E30 File Offset: 0x00066030
	private void UpdateTexts(AlertPopup.PopupInfo popupInfo)
	{
		this.m_alertText.RichText = this.m_popupInfo.m_richTextEnabled;
		this.m_alertText.Alignment = this.m_popupInfo.m_alertTextAlignment;
		if (popupInfo.m_headerText == null)
		{
			popupInfo.m_headerText = GameStrings.Get("GLOBAL_DEFAULT_ALERT_HEADER");
		}
		this.m_alertText.Text = popupInfo.m_text;
		this.m_okayButton.SetText((popupInfo.m_okText != null) ? popupInfo.m_okText : GameStrings.Get("GLOBAL_OKAY"));
		this.m_confirmButton.SetText((popupInfo.m_confirmText != null) ? popupInfo.m_confirmText : GameStrings.Get("GLOBAL_CONFIRM"));
		this.m_cancelButton.SetText((popupInfo.m_cancelText != null) ? popupInfo.m_cancelText : GameStrings.Get("GLOBAL_CANCEL"));
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x00067F16 File Offset: 0x00066116
	private void UpdateInfoAfterAnim()
	{
		this.m_popupInfo = this.m_updateInfo;
		this.m_updateInfo = null;
		this.UpdateAll(this.m_popupInfo);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x00067F38 File Offset: 0x00066138
	private void UpdateAll(AlertPopup.PopupInfo popupInfo)
	{
		this.m_alertIcon.SetActive(popupInfo.m_showAlertIcon);
		bool active = popupInfo.m_iconSet == AlertPopup.PopupInfo.IconSet.Default;
		bool active2 = popupInfo.m_iconSet == AlertPopup.PopupInfo.IconSet.Alternate;
		for (int i = 0; i < this.m_buttonIconsSet1.Count; i++)
		{
			this.m_buttonIconsSet1[i].SetActive(active);
		}
		for (int j = 0; j < this.m_buttonIconsSet2.Count; j++)
		{
			this.m_buttonIconsSet2[j].SetActive(active2);
		}
		this.UpdateHeaderText(popupInfo.m_headerText);
		this.UpdateTexts(popupInfo);
		this.UpdateLayout();
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x00067FE0 File Offset: 0x000661E0
	private void UpdateLayout()
	{
		bool activeSelf = this.m_alertIcon.activeSelf;
		Bounds textBounds = this.m_alertText.GetTextBounds();
		float num = textBounds.size.x;
		float num2 = textBounds.size.y + this.m_padding + this.m_popupInfo.m_padding;
		float num3 = 0f;
		float num4 = 0f;
		if (activeSelf)
		{
			OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_alertIcon, true);
			num3 = orientedBounds.Extents[0].magnitude * 2f;
			num4 = orientedBounds.Extents[2].magnitude * 2f;
		}
		this.UpdateButtons(this.m_popupInfo.m_responseDisplay);
		num = Mathf.Max(TransformUtil.GetBoundsOfChildren(this.m_confirmButton).size.x * 2f, num);
		this.m_body.SetSize(num + num3, Mathf.Max(num2, num4));
		Vector3 offset;
		offset..ctor(0f, 0.01f, 0f);
		TransformUtil.SetPoint(this.m_alertIcon, Anchor.TOP_LEFT_XZ, this.m_body.m_middle, Anchor.TOP_LEFT_XZ, offset);
		this.m_alertIcon.transform.localPosition += this.m_alertIconOffset;
		if (this.m_popupInfo.m_alertTextAlignment == UberText.AlignmentOptions.Center)
		{
			TransformUtil.SetPoint(this.m_alertText, Anchor.TOP_XZ, this.m_body.m_middle, Anchor.TOP_XZ, offset);
		}
		else
		{
			TransformUtil.SetPoint(this.m_alertText, Anchor.TOP_LEFT_XZ, this.m_body.m_middle, Anchor.TOP_LEFT_XZ, offset);
		}
		Vector3 position = this.m_alertText.transform.position;
		position.x += num3;
		this.m_alertText.transform.position = position;
		if (this.m_popupInfo.m_alertTextAlignment == UberText.AlignmentOptions.Center)
		{
			this.m_alertText.Width -= num3;
		}
		this.m_header.m_container.transform.position = this.m_body.m_top.m_slice.transform.position;
		this.m_buttonContainer.transform.position = this.m_body.m_bottom.m_slice.transform.position;
	}

	// Token: 0x060015FA RID: 5626 RVA: 0x00068244 File Offset: 0x00066444
	private void ButtonPress(AlertPopup.Response response)
	{
		if (this.m_popupInfo.m_responseCallback != null)
		{
			this.m_popupInfo.m_responseCallback(response, this.m_popupInfo.m_responseUserData);
		}
		this.Hide();
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x00068284 File Offset: 0x00066484
	private void UpdateHeaderText(string text)
	{
		bool flag = string.IsNullOrEmpty(text);
		this.m_header.m_container.gameObject.SetActive(!flag);
		if (flag)
		{
			return;
		}
		this.m_header.m_text.ResizeToFit = false;
		this.m_header.m_text.Text = text;
		this.m_header.m_text.UpdateNow();
		MeshRenderer component = this.m_body.m_middle.m_slice.GetComponent<MeshRenderer>();
		float x = this.m_header.m_text.GetTextBounds().size.x;
		float x2 = this.m_header.m_text.transform.worldToLocalMatrix.MultiplyVector(this.m_header.m_text.GetTextBounds().size).x;
		float num = 0.8f * this.m_header.m_text.transform.worldToLocalMatrix.MultiplyVector(component.GetComponent<Renderer>().bounds.size).x;
		if (x2 > num)
		{
			this.m_header.m_text.Width = num;
			this.m_header.m_text.ResizeToFit = true;
			this.m_header.m_text.UpdateNow();
			x = this.m_header.m_text.GetTextBounds().size.x;
		}
		else
		{
			this.m_header.m_text.Width = x2;
		}
		TransformUtil.SetLocalScaleToWorldDimension(this.m_header.m_middle, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(x, 0)
		});
		this.m_header.m_container.UpdateSlices();
	}

	// Token: 0x04000AB7 RID: 2743
	private const float BUTTON_FRAME_WIDTH = 80f;

	// Token: 0x04000AB8 RID: 2744
	public AlertPopup.Header m_header;

	// Token: 0x04000AB9 RID: 2745
	public NineSliceElement m_body;

	// Token: 0x04000ABA RID: 2746
	public GameObject m_alertIcon;

	// Token: 0x04000ABB RID: 2747
	public MultiSliceElement m_buttonContainer;

	// Token: 0x04000ABC RID: 2748
	public UIBButton m_okayButton;

	// Token: 0x04000ABD RID: 2749
	public UIBButton m_confirmButton;

	// Token: 0x04000ABE RID: 2750
	public UIBButton m_cancelButton;

	// Token: 0x04000ABF RID: 2751
	public GameObject m_clickCatcher;

	// Token: 0x04000AC0 RID: 2752
	public UberText m_alertText;

	// Token: 0x04000AC1 RID: 2753
	public Vector3 m_alertIconOffset;

	// Token: 0x04000AC2 RID: 2754
	public float m_padding;

	// Token: 0x04000AC3 RID: 2755
	public Vector3 m_loadPosition;

	// Token: 0x04000AC4 RID: 2756
	public Vector3 m_showPosition;

	// Token: 0x04000AC5 RID: 2757
	public List<GameObject> m_buttonIconsSet1 = new List<GameObject>();

	// Token: 0x04000AC6 RID: 2758
	public List<GameObject> m_buttonIconsSet2 = new List<GameObject>();

	// Token: 0x04000AC7 RID: 2759
	private AlertPopup.PopupInfo m_popupInfo;

	// Token: 0x04000AC8 RID: 2760
	private AlertPopup.PopupInfo m_updateInfo;

	// Token: 0x0200017C RID: 380
	public class PopupInfo
	{
		// Token: 0x04000AC9 RID: 2761
		public UserAttentionBlocker m_attentionCategory = UserAttentionBlocker.ALL_EXCEPT_FATAL_ERROR_SCENE;

		// Token: 0x04000ACA RID: 2762
		public string m_id;

		// Token: 0x04000ACB RID: 2763
		public string m_headerText;

		// Token: 0x04000ACC RID: 2764
		public string m_text;

		// Token: 0x04000ACD RID: 2765
		public string m_okText;

		// Token: 0x04000ACE RID: 2766
		public string m_confirmText;

		// Token: 0x04000ACF RID: 2767
		public string m_cancelText;

		// Token: 0x04000AD0 RID: 2768
		public bool m_showAlertIcon = true;

		// Token: 0x04000AD1 RID: 2769
		public AlertPopup.ResponseDisplay m_responseDisplay = AlertPopup.ResponseDisplay.OK;

		// Token: 0x04000AD2 RID: 2770
		public AlertPopup.ResponseCallback m_responseCallback;

		// Token: 0x04000AD3 RID: 2771
		public object m_responseUserData;

		// Token: 0x04000AD4 RID: 2772
		public Vector3 m_offset = Vector3.zero;

		// Token: 0x04000AD5 RID: 2773
		public float m_padding;

		// Token: 0x04000AD6 RID: 2774
		public Vector3? m_scaleOverride;

		// Token: 0x04000AD7 RID: 2775
		public bool m_richTextEnabled = true;

		// Token: 0x04000AD8 RID: 2776
		public bool m_disableBlocker;

		// Token: 0x04000AD9 RID: 2777
		public AlertPopup.PopupInfo.IconSet m_iconSet;

		// Token: 0x04000ADA RID: 2778
		public UberText.AlignmentOptions m_alertTextAlignment;

		// Token: 0x04000ADB RID: 2779
		public GameLayer? m_layerToUse;

		// Token: 0x04000ADC RID: 2780
		public bool m_keyboardEscIsCancel = true;

		// Token: 0x0200046E RID: 1134
		public enum IconSet
		{
			// Token: 0x04002364 RID: 9060
			Default,
			// Token: 0x04002365 RID: 9061
			Alternate,
			// Token: 0x04002366 RID: 9062
			None
		}
	}

	// Token: 0x0200017D RID: 381
	// (Invoke) Token: 0x06001601 RID: 5633
	public delegate void ResponseCallback(AlertPopup.Response response, object userData);

	// Token: 0x0200017E RID: 382
	public enum ResponseDisplay
	{
		// Token: 0x04000ADE RID: 2782
		NONE,
		// Token: 0x04000ADF RID: 2783
		OK,
		// Token: 0x04000AE0 RID: 2784
		CONFIRM,
		// Token: 0x04000AE1 RID: 2785
		CANCEL,
		// Token: 0x04000AE2 RID: 2786
		CONFIRM_CANCEL
	}

	// Token: 0x0200017F RID: 383
	public enum Response
	{
		// Token: 0x04000AE4 RID: 2788
		OK,
		// Token: 0x04000AE5 RID: 2789
		CONFIRM,
		// Token: 0x04000AE6 RID: 2790
		CANCEL
	}

	// Token: 0x0200046F RID: 1135
	[Serializable]
	public class Header
	{
		// Token: 0x04002367 RID: 9063
		public MultiSliceElement m_container;

		// Token: 0x04002368 RID: 9064
		public GameObject m_middle;

		// Token: 0x04002369 RID: 9065
		public UberText m_text;
	}
}
