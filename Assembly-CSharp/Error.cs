using System;
using UnityEngine;

// Token: 0x0200009A RID: 154
public static class Error
{
	// Token: 0x0600074D RID: 1869 RVA: 0x0001CCF8 File Offset: 0x0001AEF8
	public static void AddWarning(string header, string message, params object[] messageArgs)
	{
		Error.AddWarning(new ErrorParams
		{
			m_header = header,
			m_message = string.Format(message, messageArgs)
		});
	}

	// Token: 0x0600074E RID: 1870 RVA: 0x0001CD28 File Offset: 0x0001AF28
	public static void AddWarningLoc(string headerKey, string messageKey, params object[] messageArgs)
	{
		Error.AddWarning(new ErrorParams
		{
			m_header = GameStrings.Get(headerKey),
			m_message = GameStrings.Format(messageKey, messageArgs)
		});
	}

	// Token: 0x0600074F RID: 1871 RVA: 0x0001CD5C File Offset: 0x0001AF5C
	public static void AddDevWarning(string header, string message, params object[] messageArgs)
	{
		string text = string.Format(message, messageArgs);
		if (!ApplicationMgr.IsInternal())
		{
			Debug.LogWarning(string.Format("Error.AddDevWarning() - header={0} message={1}", header, text));
			return;
		}
		Error.AddWarning(new ErrorParams
		{
			m_header = header,
			m_message = text
		});
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x0001CDA8 File Offset: 0x0001AFA8
	public static void AddWarning(ErrorParams parms)
	{
		if (!DialogManager.Get())
		{
			Error.AddFatal(parms);
			return;
		}
		Debug.LogWarning(string.Format("Error.AddWarning() - header={0} message={1}", parms.m_header, parms.m_message));
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().CancelTextInput(null, true);
		}
		Error.ShowWarningDialog(parms);
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0001CE08 File Offset: 0x0001B008
	public static void AddFatal(string message)
	{
		Error.AddFatal(new ErrorParams
		{
			m_message = message
		});
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x0001CE28 File Offset: 0x0001B028
	public static void AddFatalLoc(string messageKey, params object[] messageArgs)
	{
		Error.AddFatal(new ErrorParams
		{
			m_message = GameStrings.Format(messageKey, messageArgs)
		});
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0001CE50 File Offset: 0x0001B050
	public static void AddDevFatal(string message, params object[] messageArgs)
	{
		string text = string.Format(message, messageArgs);
		if (!ApplicationMgr.IsInternal())
		{
			Debug.LogError(string.Format("Error.AddDevFatal() - message={0}", text));
			return;
		}
		Error.AddFatal(new ErrorParams
		{
			m_message = text
		});
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x0001CE94 File Offset: 0x0001B094
	public static void AddFatal(ErrorParams parms)
	{
		Debug.LogError(string.Format("Error.AddFatal() - message={0}", parms.m_message));
		if (UniversalInputManager.Get() != null)
		{
			UniversalInputManager.Get().CancelTextInput(null, true);
		}
		if (Error.ShouldUseWarningDialogForFatalError())
		{
			if (string.IsNullOrEmpty(parms.m_header))
			{
				parms.m_header = "Fatal Error as Warning";
			}
			Error.ShowWarningDialog(parms);
			return;
		}
		parms.m_type = ErrorType.FATAL;
		FatalErrorMessage fatalErrorMessage = new FatalErrorMessage();
		fatalErrorMessage.m_id = (parms.m_header ?? string.Empty) + parms.m_message;
		fatalErrorMessage.m_text = parms.m_message;
		fatalErrorMessage.m_ackCallback = parms.m_ackCallback;
		fatalErrorMessage.m_ackUserData = parms.m_ackUserData;
		fatalErrorMessage.m_allowClick = parms.m_allowClick;
		fatalErrorMessage.m_redirectToStore = parms.m_redirectToStore;
		fatalErrorMessage.m_delayBeforeNextReset = parms.m_delayBeforeNextReset;
		FatalErrorMgr.Get().Add(fatalErrorMessage);
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x0001CF84 File Offset: 0x0001B184
	private static bool ShouldUseWarningDialogForFatalError()
	{
		return !ApplicationMgr.IsPublic() && DialogManager.Get() && !Options.Get().GetBool(Option.ERROR_SCREEN);
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x0001CFC0 File Offset: 0x0001B1C0
	private static void ShowWarningDialog(ErrorParams parms)
	{
		parms.m_type = ErrorType.WARNING;
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_id = parms.m_header + parms.m_message;
		popupInfo.m_headerText = parms.m_header;
		popupInfo.m_text = parms.m_message;
		popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(Error.OnWarningPopupResponse);
		popupInfo.m_responseUserData = parms;
		popupInfo.m_showAlertIcon = true;
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x0001D034 File Offset: 0x0001B234
	private static void OnWarningPopupResponse(AlertPopup.Response response, object userData)
	{
		ErrorParams errorParams = (ErrorParams)userData;
		if (errorParams.m_ackCallback != null)
		{
			errorParams.m_ackCallback(errorParams.m_ackUserData);
		}
	}

	// Token: 0x040003DB RID: 987
	public static readonly PlatformDependentValue<bool> HAS_APP_STORE = new PlatformDependentValue<bool>(PlatformCategory.OS)
	{
		PC = false,
		Mac = false,
		iOS = true,
		Android = true
	};

	// Token: 0x02000293 RID: 659
	// (Invoke) Token: 0x0600240E RID: 9230
	public delegate void AcknowledgeCallback(object userData);
}
