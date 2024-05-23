using System;
using UnityEngine;

// Token: 0x0200056A RID: 1386
public class RecruitAFriendFrame : MonoBehaviour
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06003F7F RID: 16255 RVA: 0x0013480B File Offset: 0x00132A0B
	// (remove) Token: 0x06003F80 RID: 16256 RVA: 0x00134824 File Offset: 0x00132A24
	public event Action Closed;

	// Token: 0x06003F81 RID: 16257 RVA: 0x0013483D File Offset: 0x00132A3D
	private void Awake()
	{
		this.InitItems();
		this.Layout();
		this.InitInput();
		this.InitInputTextField();
	}

	// Token: 0x06003F82 RID: 16258 RVA: 0x00134858 File Offset: 0x00132A58
	private void Start()
	{
		this.m_InputTextField.SetInputFont(this.m_localizedInputFont);
		this.m_InputTextField.Text = this.m_inputText;
		this.UpdateInstructions();
	}

	// Token: 0x06003F83 RID: 16259 RVA: 0x00134890 File Offset: 0x00132A90
	private void InitInput()
	{
		FontDef fontDef = FontTable.Get().GetFontDef(this.m_InputFont);
		if (fontDef == null)
		{
			this.m_localizedInputFont = this.m_InputFont;
		}
		else
		{
			this.m_localizedInputFont = fontDef.m_Font;
		}
	}

	// Token: 0x06003F84 RID: 16260 RVA: 0x001348D7 File Offset: 0x00132AD7
	public void UpdateLayout()
	{
		this.Layout();
	}

	// Token: 0x06003F85 RID: 16261 RVA: 0x001348DF File Offset: 0x00132ADF
	public void Close()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003F86 RID: 16262 RVA: 0x001348EC File Offset: 0x00132AEC
	private void InitItems()
	{
		this.InitInputBlocker();
	}

	// Token: 0x06003F87 RID: 16263 RVA: 0x001348F4 File Offset: 0x00132AF4
	private void Layout()
	{
		base.transform.parent = BaseUI.Get().transform;
		base.transform.position = BaseUI.Get().GetRecruitAFriendBone().position;
	}

	// Token: 0x06003F88 RID: 16264 RVA: 0x00134930 File Offset: 0x00132B30
	private void UpdateInstructions()
	{
		this.m_InstructionText.gameObject.SetActive(string.IsNullOrEmpty(this.m_inputText) && string.IsNullOrEmpty(Input.compositionString));
	}

	// Token: 0x06003F89 RID: 16265 RVA: 0x0013496C File Offset: 0x00132B6C
	private void InitInputBlocker()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		GameObject gameObject = CameraUtils.CreateInputBlocker(camera, "RecruitAFriendInputBlocker", this);
		gameObject.transform.position = this.m_Bones.m_InputBlocker.position;
		this.m_inputBlocker = gameObject.AddComponent<PegUIElement>();
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerReleased));
	}

	// Token: 0x06003F8A RID: 16266 RVA: 0x001349D7 File Offset: 0x00132BD7
	private void OnInputBlockerReleased(UIEvent e)
	{
		this.FireClosedEvent();
	}

	// Token: 0x06003F8B RID: 16267 RVA: 0x001349E0 File Offset: 0x00132BE0
	private void InitInputTextField()
	{
		this.m_InputTextField.Preprocess += new Action<Event>(this.OnInputPreprocess);
		this.m_InputTextField.Changed += new Action<string>(this.OnInputChanged);
		this.m_InputTextField.Submitted += new Action<string>(this.OnInputSubmitted);
		this.m_InputTextField.Canceled += new Action(this.OnInputCanceled);
		this.m_InstructionText.gameObject.SetActive(true);
	}

	// Token: 0x06003F8C RID: 16268 RVA: 0x00134A5A File Offset: 0x00132C5A
	private void OnInputPreprocess(Event e)
	{
		if (Input.imeIsSelected)
		{
			this.UpdateInstructions();
		}
	}

	// Token: 0x06003F8D RID: 16269 RVA: 0x00134A6C File Offset: 0x00132C6C
	private void OnInputChanged(string text)
	{
		this.m_inputText = text;
		this.UpdateInstructions();
	}

	// Token: 0x06003F8E RID: 16270 RVA: 0x00134A7C File Offset: 0x00132C7C
	private void OnInputSubmitted(string input)
	{
		if (!RecruitListMgr.Get().CanAddMoreRecruits())
		{
			this.ShowAlertPopup("GLOBAL_FRIENDLIST_TOO_MANY_RECRUITS_ALERT_MESSAGE");
			return;
		}
		if (!RecruitListMgr.IsValidRecruitInput(input))
		{
			this.ShowAlertPopup("GLOBAL_FRIENDLIST_INVALID_EMAIL");
			return;
		}
		RecruitListMgr.Get().SendRecruitAFriendInvite(input);
		this.FireClosedEvent();
	}

	// Token: 0x06003F8F RID: 16271 RVA: 0x00134ACC File Offset: 0x00132CCC
	private void OnInputCanceled()
	{
		this.FireClosedEvent();
	}

	// Token: 0x06003F90 RID: 16272 RVA: 0x00134AD4 File Offset: 0x00132CD4
	private void FireClosedEvent()
	{
		if (this.Closed != null)
		{
			this.Closed.Invoke();
		}
	}

	// Token: 0x06003F91 RID: 16273 RVA: 0x00134AEC File Offset: 0x00132CEC
	private void ShowAlertPopup(string msgGameStringKey)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_text = GameStrings.Format(msgGameStringKey, new object[0]);
		popupInfo.m_showAlertIcon = true;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_responseCallback = null;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x040028B5 RID: 10421
	public RecruitAFriendFrameBones m_Bones;

	// Token: 0x040028B6 RID: 10422
	public UberText m_TitleText;

	// Token: 0x040028B7 RID: 10423
	public UberText m_DescriptionText;

	// Token: 0x040028B8 RID: 10424
	public UberText m_DisclaimerText;

	// Token: 0x040028B9 RID: 10425
	public UberText m_InstructionText;

	// Token: 0x040028BA RID: 10426
	public TextField m_InputTextField;

	// Token: 0x040028BB RID: 10427
	public Font m_InputFont;

	// Token: 0x040028BC RID: 10428
	private PegUIElement m_inputBlocker;

	// Token: 0x040028BD RID: 10429
	private string m_inputText = string.Empty;

	// Token: 0x040028BE RID: 10430
	private Font m_localizedInputFont;
}
