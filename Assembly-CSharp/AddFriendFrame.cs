using System;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class AddFriendFrame : MonoBehaviour
{
	// Token: 0x14000012 RID: 18
	// (add) Token: 0x06003891 RID: 14481 RVA: 0x00114995 File Offset: 0x00112B95
	// (remove) Token: 0x06003892 RID: 14482 RVA: 0x001149AE File Offset: 0x00112BAE
	public event Action Closed;

	// Token: 0x06003893 RID: 14483 RVA: 0x001149C7 File Offset: 0x00112BC7
	private void Awake()
	{
		this.InitItems();
		this.Layout();
		this.InitInput();
		this.InitInputTextField();
	}

	// Token: 0x06003894 RID: 14484 RVA: 0x001149E1 File Offset: 0x00112BE1
	private void Start()
	{
		this.m_InputTextField.SetInputFont(this.m_localizedInputFont);
		this.m_InputTextField.Activate();
		this.m_InputTextField.Text = this.m_inputText;
		this.UpdateInstructions();
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x00114A18 File Offset: 0x00112C18
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

	// Token: 0x06003896 RID: 14486 RVA: 0x00114A5F File Offset: 0x00112C5F
	public void UpdateLayout()
	{
		this.Layout();
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x00114A67 File Offset: 0x00112C67
	public void Close()
	{
		if (this.m_inputBlocker != null)
		{
			Object.Destroy(this.m_inputBlocker.gameObject);
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x00114A98 File Offset: 0x00112C98
	public void SetPlayer(BnetPlayer player)
	{
		this.m_player = player;
		if (player == null)
		{
			this.m_usePlayer = false;
			this.m_playerDisplayName = null;
		}
		else
		{
			this.m_usePlayer = true;
			this.m_playerDisplayName = FriendUtils.GetUniqueName(this.m_player);
		}
		this.m_inputText = this.m_playerDisplayName;
		this.m_InputTextField.Text = this.m_inputText;
		this.UpdateInstructions();
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x00114B00 File Offset: 0x00112D00
	private void InitItems()
	{
		this.m_HeaderText.Text = GameStrings.Get("GLOBAL_ADDFRIEND_HEADER");
		this.m_InstructionText.Text = GameStrings.Get("GLOBAL_ADDFRIEND_INSTRUCTION");
		this.InitInputBlocker();
	}

	// Token: 0x0600389A RID: 14490 RVA: 0x00114B34 File Offset: 0x00112D34
	private void Layout()
	{
		base.transform.parent = BaseUI.Get().transform;
		base.transform.position = BaseUI.Get().GetAddFriendBone().position;
		if ((UniversalInputManager.Get().IsTouchMode() && W8Touch.s_isWindows8OrGreater) || W8Touch.Get().IsVirtualKeyboardVisible())
		{
			Vector3 position;
			position..ctor(base.transform.position.x, base.transform.position.y + 100f, base.transform.position.z);
			base.transform.position = position;
		}
	}

	// Token: 0x0600389B RID: 14491 RVA: 0x00114BEC File Offset: 0x00112DEC
	private void UpdateInstructions()
	{
		this.m_InstructionText.gameObject.SetActive(string.IsNullOrEmpty(this.m_inputText) && string.IsNullOrEmpty(Input.compositionString));
	}

	// Token: 0x0600389C RID: 14492 RVA: 0x00114C28 File Offset: 0x00112E28
	private void InitInputBlocker()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		GameObject gameObject = CameraUtils.CreateInputBlocker(camera, "AddFriendInputBlocker");
		gameObject.transform.parent = base.transform.parent;
		this.m_inputBlocker = gameObject.AddComponent<PegUIElement>();
		this.m_inputBlocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInputBlockerReleased));
		TransformUtil.SetPosZ(this.m_inputBlocker, base.transform.position.z + 1f);
	}

	// Token: 0x0600389D RID: 14493 RVA: 0x00114CB1 File Offset: 0x00112EB1
	private void OnInputBlockerReleased(UIEvent e)
	{
		this.OnClosed();
	}

	// Token: 0x0600389E RID: 14494 RVA: 0x00114CBC File Offset: 0x00112EBC
	private void InitInputTextField()
	{
		this.m_InputTextField.Preprocess += new Action<Event>(this.OnInputPreprocess);
		this.m_InputTextField.Changed += new Action<string>(this.OnInputChanged);
		this.m_InputTextField.Submitted += new Action<string>(this.OnInputSubmitted);
		this.m_InputTextField.Canceled += new Action(this.OnInputCanceled);
		this.m_InstructionText.gameObject.SetActive(true);
	}

	// Token: 0x0600389F RID: 14495 RVA: 0x00114D36 File Offset: 0x00112F36
	private void OnInputPreprocess(Event e)
	{
		if (Input.imeIsSelected)
		{
			this.UpdateInstructions();
		}
	}

	// Token: 0x060038A0 RID: 14496 RVA: 0x00114D48 File Offset: 0x00112F48
	private void OnInputChanged(string text)
	{
		this.m_inputText = text;
		this.UpdateInstructions();
		this.m_usePlayer = (string.Compare(this.m_playerDisplayName, text.Trim(), true) == 0);
	}

	// Token: 0x060038A1 RID: 14497 RVA: 0x00114D80 File Offset: 0x00112F80
	private void OnInputSubmitted(string input)
	{
		string text = (!this.m_usePlayer) ? input.Trim() : this.m_player.GetBattleTag().ToString();
		if (text.Contains("@"))
		{
			BnetFriendMgr.Get().SendInviteByEmail(text);
		}
		else if (text.Contains("#"))
		{
			BnetFriendMgr.Get().SendInviteByBattleTag(text);
		}
		else
		{
			string message = GameStrings.Get("GLOBAL_ADDFRIEND_ERROR_MALFORMED");
			UIStatus.Get().AddError(message);
		}
		this.OnClosed();
	}

	// Token: 0x060038A2 RID: 14498 RVA: 0x00114E10 File Offset: 0x00113010
	private void OnInputCanceled()
	{
		this.OnClosed();
	}

	// Token: 0x060038A3 RID: 14499 RVA: 0x00114E18 File Offset: 0x00113018
	private void OnClosed()
	{
		if (this.Closed != null)
		{
			this.Closed.Invoke();
		}
	}

	// Token: 0x04002463 RID: 9315
	public AddFriendFrameBones m_Bones;

	// Token: 0x04002464 RID: 9316
	public UberText m_HeaderText;

	// Token: 0x04002465 RID: 9317
	public UberText m_InstructionText;

	// Token: 0x04002466 RID: 9318
	public TextField m_InputTextField;

	// Token: 0x04002467 RID: 9319
	public Font m_InputFont;

	// Token: 0x04002468 RID: 9320
	private PegUIElement m_inputBlocker;

	// Token: 0x04002469 RID: 9321
	private string m_inputText = string.Empty;

	// Token: 0x0400246A RID: 9322
	private BnetPlayer m_player;

	// Token: 0x0400246B RID: 9323
	private bool m_usePlayer;

	// Token: 0x0400246C RID: 9324
	private string m_playerDisplayName;

	// Token: 0x0400246D RID: 9325
	private Font m_localizedInputFont;
}
