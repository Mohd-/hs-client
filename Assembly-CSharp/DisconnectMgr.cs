using System;
using UnityEngine;

// Token: 0x0200088B RID: 2187
public class DisconnectMgr : MonoBehaviour
{
	// Token: 0x06005374 RID: 21364 RVA: 0x0018E58B File Offset: 0x0018C78B
	private void Awake()
	{
		DisconnectMgr.s_instance = this;
	}

	// Token: 0x06005375 RID: 21365 RVA: 0x0018E593 File Offset: 0x0018C793
	private void OnDestroy()
	{
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
		DisconnectMgr.s_instance = null;
	}

	// Token: 0x06005376 RID: 21366 RVA: 0x0018E5C2 File Offset: 0x0018C7C2
	public static DisconnectMgr Get()
	{
		return DisconnectMgr.s_instance;
	}

	// Token: 0x06005377 RID: 21367 RVA: 0x0018E5CC File Offset: 0x0018C7CC
	public void DisconnectFromGameplay()
	{
		SceneMgr.Mode postDisconnectSceneMode = GameMgr.Get().GetPostDisconnectSceneMode();
		GameMgr.Get().PreparePostGameSceneMode(postDisconnectSceneMode);
		if (postDisconnectSceneMode == SceneMgr.Mode.INVALID)
		{
			Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION", 0f);
		}
		else if (Network.WasDisconnectRequested())
		{
			SceneMgr.Get().SetNextMode(postDisconnectSceneMode);
		}
		else
		{
			this.ShowGameplayDialog(postDisconnectSceneMode);
		}
	}

	// Token: 0x06005378 RID: 21368 RVA: 0x0018E630 File Offset: 0x0018C830
	private void ShowGameplayDialog(SceneMgr.Mode nextMode)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLOBAL_ERROR_NETWORK_TITLE");
		popupInfo.m_text = GameStrings.Get("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION");
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
		DialogManager.Get().ShowPopup(popupInfo, new DialogManager.DialogProcessCallback(this.OnGameplayDialogProcessed), nextMode);
	}

	// Token: 0x06005379 RID: 21369 RVA: 0x0018E688 File Offset: 0x0018C888
	private bool OnGameplayDialogProcessed(DialogBase dialog, object userData)
	{
		this.m_dialog = (AlertPopup)dialog;
		SceneMgr.Mode nextMode = (SceneMgr.Mode)((int)userData);
		SceneMgr.Get().SetNextMode(nextMode);
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		return true;
	}

	// Token: 0x0600537A RID: 21370 RVA: 0x0018E6CA File Offset: 0x0018C8CA
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded), userData);
		this.UpdateGameplayDialog();
	}

	// Token: 0x0600537B RID: 21371 RVA: 0x0018E6EC File Offset: 0x0018C8EC
	private void UpdateGameplayDialog()
	{
		AlertPopup.PopupInfo info = this.m_dialog.GetInfo();
		info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnGameplayDialogResponse);
		this.m_dialog.UpdateInfo(info);
	}

	// Token: 0x0600537C RID: 21372 RVA: 0x0018E72A File Offset: 0x0018C92A
	private void OnGameplayDialogResponse(AlertPopup.Response response, object userData)
	{
		this.m_dialog = null;
	}

	// Token: 0x040039AB RID: 14763
	private static DisconnectMgr s_instance;

	// Token: 0x040039AC RID: 14764
	private AlertPopup m_dialog;
}
