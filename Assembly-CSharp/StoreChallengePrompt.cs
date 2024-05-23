using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PegasusUtil;
using SimpleJSON;
using UnityEngine;

// Token: 0x0200040B RID: 1035
public class StoreChallengePrompt : UIBPopup
{
	// Token: 0x14000010 RID: 16
	// (add) Token: 0x060034AD RID: 13485 RVA: 0x0010682E File Offset: 0x00104A2E
	// (remove) Token: 0x060034AE RID: 13486 RVA: 0x00106847 File Offset: 0x00104A47
	public event StoreChallengePrompt.CancelListener OnCancel;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x060034AF RID: 13487 RVA: 0x00106860 File Offset: 0x00104A60
	// (remove) Token: 0x060034B0 RID: 13488 RVA: 0x00106879 File Offset: 0x00104A79
	public event StoreChallengePrompt.CompleteListener OnChallengeComplete;

	// Token: 0x060034B1 RID: 13489 RVA: 0x00106894 File Offset: 0x00104A94
	private void Awake()
	{
		this.m_inputText.RichText = false;
		this.m_submitButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSubmitPressed));
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
		this.m_infoButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnInfoPressed));
	}

	// Token: 0x060034B2 RID: 13490 RVA: 0x001068F8 File Offset: 0x00104AF8
	public IEnumerator Show(string challengeUrl)
	{
		this.m_challengeJson = null;
		this.m_challengeUrl = challengeUrl;
		if (this.IsShown())
		{
			yield break;
		}
		this.m_shown = true;
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers["Accept"] = "application/json;charset=UTF-8";
		headers["Accept-Language"] = Localization.GetBnetLocaleName();
		WWW challenge = new WWW(this.m_challengeUrl, null, headers);
		bool isTimeout = false;
		string error = null;
		float timeout = Time.realtimeSinceStartup + 15f;
		while (!challenge.isDone && challenge.error == null)
		{
			if (Time.realtimeSinceStartup >= timeout)
			{
				isTimeout = true;
				break;
			}
			yield return 0;
		}
		if (!isTimeout)
		{
			if (!string.IsNullOrEmpty(challenge.error))
			{
				error = string.Format("WWW error: {0}", challenge.error);
			}
			else if (string.IsNullOrEmpty(challenge.text))
			{
				error = "response text is empty";
			}
			else
			{
				if (ApplicationMgr.IsInternal())
				{
					Log.BattleNet.PrintInfo("Challenge json received: {0}", new object[]
					{
						challenge.text
					});
				}
				try
				{
					this.m_challengeJson = JSON.Parse(challenge.text);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.LogException(e);
					error = string.Format("{0}: {1}", e.GetType().Name, e.Message);
				}
			}
		}
		if (isTimeout || error != null)
		{
			Debug.LogError("Tassadar Challenge Retrieval Failed: " + ((error != null) ? error : "url request timed out"));
			if (challenge.isDone)
			{
				Debug.LogError(challenge.text);
			}
			this.Hide(false);
			string header = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
			string message = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
			CancelPurchase.CancelReason? reason = default(CancelPurchase.CancelReason?);
			if (isTimeout)
			{
				reason = new CancelPurchase.CancelReason?(5);
			}
			this.DisplayError(header, message, false, reason, error);
			yield break;
		}
		JSONNode challengeNode = this.m_challengeJson["challenge"];
		this.m_challengeID = this.m_challengeJson["challenge_id"];
		string challengePrompt = challengeNode["prompt"];
		this.m_challengeType = challengeNode["type"];
		this.m_challengeInput = challengeNode["inputs"][0];
		JSONArray errorsNode = challengeNode["errors"].AsArray;
		if (errorsNode != null && errorsNode.Count > 0)
		{
			IEnumerable<JSONNode> list = Enumerable.Cast<JSONNode>(errorsNode.AsArray);
			string message2 = string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<JSONNode, string>(list, (JSONNode n) => n.Value)));
			this.DisplayError(this.m_challengeInput["label"], message2, false, new CancelPurchase.CancelReason?(7), message2);
			yield break;
		}
		bool showInfoButton = false;
		if (this.m_challengeType == "cvv")
		{
			showInfoButton = true;
		}
		this.m_messageText.Text = challengePrompt;
		if (string.IsNullOrEmpty(this.m_messageText.Text))
		{
			Debug.LogErrorFormat("Challenge has no prompt text, json received: {0}", new object[]
			{
				challenge.text
			});
		}
		this.m_infoButtonFrame.SetActive(showInfoButton);
		this.m_input = string.Empty;
		this.UpdateInputText();
		base.DoShowAnimation(new UIBPopup.OnAnimationComplete(this.OnShown));
		yield break;
	}

	// Token: 0x060034B3 RID: 13491 RVA: 0x00106924 File Offset: 0x00104B24
	public string HideChallenge()
	{
		string challengeID = this.m_challengeID;
		this.Hide(false);
		return challengeID;
	}

	// Token: 0x060034B4 RID: 13492 RVA: 0x00106940 File Offset: 0x00104B40
	private void OnShown()
	{
		if (!this.IsShown())
		{
			return;
		}
		this.ShowInput();
	}

	// Token: 0x060034B5 RID: 13493 RVA: 0x00106954 File Offset: 0x00104B54
	protected override void Hide(bool animate)
	{
		if (!this.IsShown())
		{
			return;
		}
		this.m_shown = false;
		this.HideInput();
		base.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(this.OnHidden));
	}

	// Token: 0x060034B6 RID: 13494 RVA: 0x00106991 File Offset: 0x00104B91
	protected override void OnHidden()
	{
		this.m_challengeID = null;
	}

	// Token: 0x060034B7 RID: 13495 RVA: 0x0010699C File Offset: 0x00104B9C
	private void Cancel()
	{
		string challengeID = this.m_challengeID;
		this.Hide(true);
		if (this.OnCancel != null)
		{
			this.OnCancel(challengeID);
		}
	}

	// Token: 0x060034B8 RID: 13496 RVA: 0x001069CE File Offset: 0x00104BCE
	private void OnSubmitPressed(UIEvent e)
	{
		base.StartCoroutine(this.SubmitChallenge());
	}

	// Token: 0x060034B9 RID: 13497 RVA: 0x001069E0 File Offset: 0x00104BE0
	private IEnumerator SubmitChallenge()
	{
		this.HideInput();
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers["Accept"] = "application/json;charset=UTF-8";
		headers["Accept-Language"] = Localization.GetBnetLocaleName();
		headers["Content-Type"] = "application/json;charset=UTF-8";
		string inputId = (!(this.m_challengeInput == null)) ? this.m_challengeInput["input_id"] : null;
		if (inputId == null)
		{
			inputId = string.Empty;
		}
		string inputResponse = (this.m_input != null) ? this.m_input : string.Empty;
		JSONClass challengeResponseJson = new JSONClass();
		JSONArray inputs = new JSONArray();
		JSONClass inputContents = new JSONClass();
		inputContents.Add("input_id", new JSONData(inputId));
		inputContents.Add("value", new JSONData(inputResponse));
		inputs.Add(inputContents);
		challengeResponseJson.Add("inputs", inputs);
		string responseString = challengeResponseJson.ToString();
		WWW challengeResponse = new WWW(this.m_challengeUrl, Encoding.UTF8.GetBytes(responseString), headers);
		bool isTimeout = false;
		string error = null;
		float timeout = Time.realtimeSinceStartup + 15f;
		while (!challengeResponse.isDone && challengeResponse.error == null)
		{
			if (Time.realtimeSinceStartup >= timeout)
			{
				isTimeout = true;
				break;
			}
			yield return 0;
		}
		JSONNode responseResult = null;
		if (!isTimeout)
		{
			if (!string.IsNullOrEmpty(challengeResponse.error))
			{
				error = string.Format("WWW error: {0}", challengeResponse.error);
			}
			else if (string.IsNullOrEmpty(challengeResponse.text))
			{
				error = "response text is empty";
			}
			else
			{
				if (ApplicationMgr.IsInternal())
				{
					Log.BattleNet.PrintInfo("Submit challenge response json received: {0}", new object[]
					{
						challengeResponse.text
					});
				}
				try
				{
					responseResult = JSON.Parse(challengeResponse.text);
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.LogException(e);
					error = string.Format("{0}: {1}", e.GetType().Name, e.Message);
				}
			}
		}
		if (isTimeout || error != null)
		{
			Debug.LogError("Tassadar Challenge Submission Failed: " + ((error != null) ? error : "url request timed out"));
			if (challengeResponse.isDone)
			{
				Debug.LogError(challengeResponse.text);
			}
			this.Hide(false);
			string header = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
			string message = GameStrings.Get("GLUE_STORE_FAIL_CHALLENGE_TIMEOUT");
			CancelPurchase.CancelReason? reason = default(CancelPurchase.CancelReason?);
			if (isTimeout)
			{
				reason = new CancelPurchase.CancelReason?(5);
			}
			this.DisplayError(header, message, false, reason, error);
			yield break;
		}
		bool isDone = responseResult["done"].AsBool;
		string activeChallengeID = this.m_challengeID;
		if (!isDone)
		{
			IEnumerable<JSONNode> list = Enumerable.Cast<JSONNode>(responseResult["challenge"]["errors"].AsArray);
			string message2 = string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<JSONNode, string>(list, (JSONNode n) => n.Value)));
			this.DisplayError(this.m_challengeInput["label"], message2, true, default(CancelPurchase.CancelReason?), null);
		}
		else
		{
			bool success = true;
			JSONNode errorNode = responseResult["error_code"];
			if (errorNode != null && !string.IsNullOrEmpty(errorNode.Value))
			{
				success = false;
				error = errorNode.Value;
			}
			if (success)
			{
				this.Hide(true);
				this.FireComplete(activeChallengeID, success, default(CancelPurchase.CancelReason?), error);
			}
			else
			{
				string header2 = GameStrings.Get("GLUE_STORE_GENERIC_BP_FAIL_HEADLINE");
				string message3 = GameStrings.Get("GLUE_STORE_FAIL_THROTTLED");
				CancelPurchase.CancelReason reason2 = 7;
				string text = error;
				if (text != null)
				{
					if (StoreChallengePrompt.<>f__switch$map79 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
						dictionary.Add("DENIED", 0);
						StoreChallengePrompt.<>f__switch$map79 = dictionary;
					}
					int num;
					if (StoreChallengePrompt.<>f__switch$map79.TryGetValue(text, ref num))
					{
						if (num == 0)
						{
							reason2 = 6;
							error = null;
						}
					}
				}
				this.DisplayError(header2, message3, false, new CancelPurchase.CancelReason?(reason2), error);
			}
		}
		yield break;
	}

	// Token: 0x060034BA RID: 13498 RVA: 0x001069FC File Offset: 0x00104BFC
	private void DisplayError(string header, string message, bool allowInputAgain, CancelPurchase.CancelReason? reason, string internalErrorInfo)
	{
		this.ClearInput();
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_headerText = header;
		popupInfo.m_text = message;
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
		if (allowInputAgain)
		{
			popupInfo.m_responseCallback = delegate(AlertPopup.Response response, object userData)
			{
				this.ShowInput();
			};
		}
		else
		{
			string challengeID = this.HideChallenge();
			this.FireComplete(challengeID, false, reason, internalErrorInfo);
		}
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x060034BB RID: 13499 RVA: 0x00106A73 File Offset: 0x00104C73
	private void FireComplete(string challengeID, bool isSuccess, CancelPurchase.CancelReason? reason, string internalErrorInfo)
	{
		if (this.OnChallengeComplete != null)
		{
			this.OnChallengeComplete(challengeID, isSuccess, reason, internalErrorInfo);
		}
	}

	// Token: 0x060034BC RID: 13500 RVA: 0x00106A90 File Offset: 0x00104C90
	private void OnCancelPressed(UIEvent e)
	{
		this.Cancel();
	}

	// Token: 0x060034BD RID: 13501 RVA: 0x00106A98 File Offset: 0x00104C98
	private void OnInfoPressed(UIEvent e)
	{
		Application.OpenURL(StoreChallengePrompt.s_cvvURL.GetURL());
	}

	// Token: 0x060034BE RID: 13502 RVA: 0x00106AAC File Offset: 0x00104CAC
	private void ShowInput()
	{
		this.m_inputText.gameObject.SetActive(false);
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		Bounds bounds = this.m_inputText.GetBounds();
		Rect rect = CameraUtils.CreateGUIViewportRect(camera, bounds.min, bounds.max);
		UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_password = true,
			m_rect = rect,
			m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
			m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
			m_font = this.m_inputText.TrueTypeFont,
			m_alignment = new TextAnchor?(4),
			m_maxCharacters = ((!(this.m_challengeInput != null)) ? 0 : this.m_challengeInput["max_length"].AsInt)
		};
		UniversalInputManager.Get().UseTextInput(parms, false);
		this.m_submitButton.SetEnabled(true);
	}

	// Token: 0x060034BF RID: 13503 RVA: 0x00106BC8 File Offset: 0x00104DC8
	private void HideInput()
	{
		UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		this.m_inputText.gameObject.SetActive(true);
		this.m_submitButton.SetEnabled(false);
	}

	// Token: 0x060034C0 RID: 13504 RVA: 0x00106C03 File Offset: 0x00104E03
	private void ClearInput()
	{
		UniversalInputManager.Get().SetInputText(string.Empty);
	}

	// Token: 0x060034C1 RID: 13505 RVA: 0x00106C14 File Offset: 0x00104E14
	private void OnInputUpdated(string input)
	{
		this.m_input = input;
		this.UpdateInputText();
	}

	// Token: 0x060034C2 RID: 13506 RVA: 0x00106C23 File Offset: 0x00104E23
	private void OnInputComplete(string input)
	{
		this.m_input = input;
		this.UpdateInputText();
		base.StartCoroutine(this.SubmitChallenge());
	}

	// Token: 0x060034C3 RID: 13507 RVA: 0x00106C3F File Offset: 0x00104E3F
	private void OnInputCanceled(bool userRequested, GameObject requester)
	{
		this.m_input = string.Empty;
		this.UpdateInputText();
		this.Cancel();
	}

	// Token: 0x060034C4 RID: 13508 RVA: 0x00106C58 File Offset: 0x00104E58
	private void UpdateInputText()
	{
		StringBuilder stringBuilder = new StringBuilder(this.m_input.Length);
		for (int i = 0; i < this.m_input.Length; i++)
		{
			stringBuilder.Append('*');
		}
		this.m_inputText.Text = stringBuilder.ToString();
	}

	// Token: 0x040020D2 RID: 8402
	private const string FMT_URL_CVV_INFO = "https://nydus.battle.net/WTCG/{0}/client/support/cvv?targetRegion={1}";

	// Token: 0x040020D3 RID: 8403
	private const float TASSADAR_CHALLENGE_TIMEOUT_SECONDS = 15f;

	// Token: 0x040020D4 RID: 8404
	public UIBButton m_submitButton;

	// Token: 0x040020D5 RID: 8405
	public UIBButton m_cancelButton;

	// Token: 0x040020D6 RID: 8406
	public UberText m_messageText;

	// Token: 0x040020D7 RID: 8407
	public UberText m_inputText;

	// Token: 0x040020D8 RID: 8408
	public GameObject m_infoButtonFrame;

	// Token: 0x040020D9 RID: 8409
	public UIBButton m_infoButton;

	// Token: 0x040020DA RID: 8410
	private static readonly StoreURL s_cvvURL = new StoreURL("https://nydus.battle.net/WTCG/{0}/client/support/cvv?targetRegion={1}", StoreURL.Param.LOCALE, StoreURL.Param.REGION);

	// Token: 0x040020DB RID: 8411
	private string m_input = string.Empty;

	// Token: 0x040020DC RID: 8412
	private string m_challengeID;

	// Token: 0x040020DD RID: 8413
	private string m_challengeUrl;

	// Token: 0x040020DE RID: 8414
	private JSONNode m_challengeJson;

	// Token: 0x040020DF RID: 8415
	private JSONNode m_challengeInput;

	// Token: 0x040020E0 RID: 8416
	private string m_challengeType;

	// Token: 0x0200042A RID: 1066
	// (Invoke) Token: 0x06003603 RID: 13827
	public delegate void CompleteListener(string challengeID, bool isSuccess, CancelPurchase.CancelReason? reason, string internalErrorInfo);

	// Token: 0x0200042B RID: 1067
	// (Invoke) Token: 0x06003607 RID: 13831
	public delegate void CancelListener(string challengeID);
}
