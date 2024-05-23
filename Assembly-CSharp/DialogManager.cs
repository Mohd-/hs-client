using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PegasusShared;
using UnityEngine;

// Token: 0x0200012A RID: 298
[CustomEditClass]
public class DialogManager : MonoBehaviour
{
	// Token: 0x06000F4E RID: 3918 RVA: 0x00042DFC File Offset: 0x00040FFC
	private void Awake()
	{
		DialogManager.s_instance = this;
		NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
		if (netObject != null)
		{
			this.MaybeShowSeasonEndDialog(netObject.Notices, false);
		}
		NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
	}

	// Token: 0x06000F4F RID: 3919 RVA: 0x00042E43 File Offset: 0x00041043
	private void OnDestroy()
	{
		NetCache.Get().RemoveNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
		DialogManager.s_instance = null;
	}

	// Token: 0x06000F50 RID: 3920 RVA: 0x00042E61 File Offset: 0x00041061
	public static DialogManager Get()
	{
		return DialogManager.s_instance;
	}

	// Token: 0x06000F51 RID: 3921 RVA: 0x00042E68 File Offset: 0x00041068
	public void GoBack()
	{
		if (this.m_currentDialog)
		{
			this.m_currentDialog.GoBack();
		}
	}

	// Token: 0x06000F52 RID: 3922 RVA: 0x00042E85 File Offset: 0x00041085
	public void ReadyForSeasonEndPopup(bool ready)
	{
		this.m_isReadyForSeasonEndPopup = ready;
	}

	// Token: 0x06000F53 RID: 3923 RVA: 0x00042E8E File Offset: 0x0004108E
	public bool HandleKeyboardInput()
	{
		return Input.GetKeyUp(27) && this.m_currentDialog && this.m_currentDialog.HandleKeyboardInput();
	}

	// Token: 0x06000F54 RID: 3924 RVA: 0x00042EC0 File Offset: 0x000410C0
	public void AddToQueue(DialogManager.DialogRequest request)
	{
		if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(request.m_attentionCategory, "DialogManager.AddToQueue:" + ((request != null) ? request.m_type.ToString() : "null")))
		{
			return;
		}
		this.m_dialogRequests.Enqueue(request);
		this.UpdateQueue();
	}

	// Token: 0x06000F55 RID: 3925 RVA: 0x00042F28 File Offset: 0x00041128
	private void UpdateQueue()
	{
		if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE))
		{
			return;
		}
		if (this.m_currentDialog != null)
		{
			return;
		}
		if (this.m_loadingDialog)
		{
			return;
		}
		if (this.m_dialogRequests.Count == 0)
		{
			this.DestroyPopupAssetsIfPossible();
			return;
		}
		DialogManager.DialogRequest dialogRequest = this.m_dialogRequests.Peek();
		if (!UserAttentionManager.CanShowAttentionGrabber(dialogRequest.m_attentionCategory, "DialogManager.UpdateQueue:" + dialogRequest.m_attentionCategory))
		{
			ApplicationMgr.Get().ScheduleCallback(0.5f, false, delegate(object userData)
			{
				this.UpdateQueue();
			}, null);
			return;
		}
		this.LoadPopup(dialogRequest);
	}

	// Token: 0x06000F56 RID: 3926 RVA: 0x00042FD0 File Offset: 0x000411D0
	public void ShowPopup(AlertPopup.PopupInfo info, DialogManager.DialogProcessCallback callback, object userData)
	{
		if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(info.m_attentionCategory, "DialogManager.ShowPopup:" + ((info != null) ? (info.m_id + ":" + info.m_attentionCategory.ToString()) : "null")))
		{
			return;
		}
		this.AddToQueue(new DialogManager.DialogRequest
		{
			m_type = DialogManager.DialogType.ALERT,
			m_attentionCategory = info.m_attentionCategory,
			m_info = info,
			m_callback = callback,
			m_userData = userData
		});
	}

	// Token: 0x06000F57 RID: 3927 RVA: 0x00043068 File Offset: 0x00041268
	public void ShowPopup(AlertPopup.PopupInfo info, DialogManager.DialogProcessCallback callback)
	{
		this.ShowPopup(info, callback, null);
	}

	// Token: 0x06000F58 RID: 3928 RVA: 0x00043073 File Offset: 0x00041273
	public void ShowPopup(AlertPopup.PopupInfo info)
	{
		this.ShowPopup(info, null, null);
	}

	// Token: 0x06000F59 RID: 3929 RVA: 0x00043080 File Offset: 0x00041280
	public bool ShowUniquePopup(AlertPopup.PopupInfo info, DialogManager.DialogProcessCallback callback, object userData)
	{
		if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(info.m_attentionCategory, "DialogManager.ShowUniquePopup:" + ((info != null) ? (info.m_id + ":" + info.m_attentionCategory.ToString()) : "null")))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(info.m_id))
		{
			foreach (DialogManager.DialogRequest dialogRequest in this.m_dialogRequests)
			{
				if (dialogRequest.m_type == DialogManager.DialogType.ALERT)
				{
					AlertPopup.PopupInfo popupInfo = (AlertPopup.PopupInfo)dialogRequest.m_info;
					if (popupInfo.m_id == info.m_id)
					{
						return false;
					}
				}
			}
		}
		this.ShowPopup(info, callback, userData);
		return true;
	}

	// Token: 0x06000F5A RID: 3930 RVA: 0x00043180 File Offset: 0x00041380
	public bool ShowUniquePopup(AlertPopup.PopupInfo info, DialogManager.DialogProcessCallback callback)
	{
		return this.ShowUniquePopup(info, callback, null);
	}

	// Token: 0x06000F5B RID: 3931 RVA: 0x0004318B File Offset: 0x0004138B
	public bool ShowUniquePopup(AlertPopup.PopupInfo info)
	{
		return this.ShowUniquePopup(info, null, null);
	}

	// Token: 0x06000F5C RID: 3932 RVA: 0x00043198 File Offset: 0x00041398
	public void ShowMessageOfTheDay(string message)
	{
		this.ShowPopup(new AlertPopup.PopupInfo
		{
			m_text = message
		});
	}

	// Token: 0x06000F5D RID: 3933 RVA: 0x000431BC File Offset: 0x000413BC
	public void RemoveUniquePopupRequestFromQueue(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return;
		}
		foreach (DialogManager.DialogRequest dialogRequest in this.m_dialogRequests)
		{
			if (dialogRequest.m_type == DialogManager.DialogType.ALERT)
			{
				AlertPopup.PopupInfo popupInfo = (AlertPopup.PopupInfo)dialogRequest.m_info;
				if (popupInfo.m_id == id)
				{
					this.m_dialogRequests = new Queue<DialogManager.DialogRequest>(Enumerable.Where<DialogManager.DialogRequest>(this.m_dialogRequests, (DialogManager.DialogRequest r) => r.m_info != null && r.m_info.GetType() == typeof(AlertPopup.PopupInfo) && ((AlertPopup.PopupInfo)r.m_info).m_id != id));
					break;
				}
			}
		}
	}

	// Token: 0x06000F5E RID: 3934 RVA: 0x00043288 File Offset: 0x00041488
	public bool WaitingToShowSeasonEndDialog()
	{
		if (this.m_waitingToShowSeasonEndDialog || (this.m_currentDialog != null && this.m_currentDialog is SeasonEndDialog))
		{
			return true;
		}
		DialogManager.DialogRequest dialogRequest = Enumerable.FirstOrDefault<DialogManager.DialogRequest>(this.m_dialogRequests, (DialogManager.DialogRequest obj) => obj.m_type == DialogManager.DialogType.SEASON_END);
		return dialogRequest != null;
	}

	// Token: 0x06000F5F RID: 3935 RVA: 0x000432F4 File Offset: 0x000414F4
	public void ShowFriendlyChallenge(FormatType formatType, BnetPlayer challenger, bool challengeIsTavernBrawl, FriendlyChallengeDialog.ResponseCallback responseCallback, DialogManager.DialogProcessCallback callback)
	{
		this.AddToQueue(new DialogManager.DialogRequest
		{
			m_type = ((!challengeIsTavernBrawl) ? DialogManager.DialogType.FRIENDLY_CHALLENGE : DialogManager.DialogType.TAVERN_BRAWL_CHALLENGE),
			m_info = new FriendlyChallengeDialog.Info
			{
				m_formatType = formatType,
				m_challenger = challenger,
				m_callback = responseCallback
			},
			m_callback = callback
		});
	}

	// Token: 0x06000F60 RID: 3936 RVA: 0x0004334C File Offset: 0x0004154C
	public void ShowExistingAccountPopup(ExistingAccountPopup.ResponseCallback responseCallback, DialogManager.DialogProcessCallback callback)
	{
		this.AddToQueue(new DialogManager.DialogRequest
		{
			m_type = DialogManager.DialogType.EXISTING_ACCOUNT,
			m_info = new ExistingAccountPopup.Info
			{
				m_callback = responseCallback
			},
			m_callback = callback
		});
	}

	// Token: 0x06000F61 RID: 3937 RVA: 0x00043388 File Offset: 0x00041588
	public void ShowCardListPopup(UserAttentionBlocker attentionCategory, CardListPopup.Info info)
	{
		this.AddToQueue(new DialogManager.DialogRequest
		{
			m_type = DialogManager.DialogType.CARD_LIST,
			m_attentionCategory = attentionCategory,
			m_info = info
		});
	}

	// Token: 0x06000F62 RID: 3938 RVA: 0x000433B7 File Offset: 0x000415B7
	public void ClearAllImmediately()
	{
		if (this.m_currentDialog != null)
		{
			Object.DestroyImmediate(this.m_currentDialog.gameObject);
			this.m_currentDialog = null;
		}
		this.m_dialogRequests.Clear();
		this.DestroyPopupAssetsIfPossible();
	}

	// Token: 0x06000F63 RID: 3939 RVA: 0x000433F4 File Offset: 0x000415F4
	public bool ShowingDialog()
	{
		return this.m_currentDialog != null || this.m_dialogRequests.Count > 0;
	}

	// Token: 0x06000F64 RID: 3940 RVA: 0x00043423 File Offset: 0x00041623
	private void OnNewNotices(List<NetCache.ProfileNotice> newNotices)
	{
		this.MaybeShowSeasonEndDialog(newNotices, true);
	}

	// Token: 0x06000F65 RID: 3941 RVA: 0x00043430 File Offset: 0x00041630
	private void MaybeShowSeasonEndDialog(List<NetCache.ProfileNotice> newNotices, bool fromNewNotices)
	{
		NetCache.ProfileNoticeMedal profileNoticeMedal = (NetCache.ProfileNoticeMedal)newNotices.Find((NetCache.ProfileNotice obj) => obj.Type == NetCache.ProfileNotice.NoticeType.GAINED_MEDAL);
		if (profileNoticeMedal == null)
		{
			return;
		}
		if (this.m_handledMedalNoticeIDs.Contains(profileNoticeMedal.NoticeID))
		{
			return;
		}
		if (UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber("DialogManager.MaybeShowSeasonEndDialog"))
		{
			return;
		}
		this.m_handledMedalNoticeIDs.Add(profileNoticeMedal.NoticeID);
		long seasonID = profileNoticeMedal.OriginData;
		NetCache.ProfileNoticeRewardCardBack noticeCardBack = (NetCache.ProfileNoticeRewardCardBack)newNotices.Find((NetCache.ProfileNotice notice) => notice.Type == NetCache.ProfileNotice.NoticeType.REWARD_CARD_BACK && notice.OriginData == seasonID);
		NetCache.ProfileNoticeBonusStars noticeBonusStars = (NetCache.ProfileNoticeBonusStars)newNotices.Find((NetCache.ProfileNotice notice) => notice.Type == NetCache.ProfileNotice.NoticeType.BONUS_STARS && notice.OriginData == seasonID);
		if (fromNewNotices)
		{
			NetCache.Get().RefreshNetObject<NetCache.NetCacheMedalInfo>();
			NetCache.Get().ReloadNetObject<NetCache.NetCacheRewardProgress>();
		}
		DialogManager.SeasonEndDialogRequestInfo seasonEndDialogRequestInfo = new DialogManager.SeasonEndDialogRequestInfo();
		seasonEndDialogRequestInfo.m_noticeMedal = profileNoticeMedal;
		seasonEndDialogRequestInfo.m_noticeBonusStars = noticeBonusStars;
		seasonEndDialogRequestInfo.m_noticeCardBack = noticeCardBack;
		base.StartCoroutine(this.ShowSeasonEndDialogWhenReady(new DialogManager.DialogRequest
		{
			m_type = DialogManager.DialogType.SEASON_END,
			m_info = seasonEndDialogRequestInfo
		}));
	}

	// Token: 0x06000F66 RID: 3942 RVA: 0x00043554 File Offset: 0x00041754
	private void LoadPopup(DialogManager.DialogRequest request)
	{
		DialogManager.DialogTypeMapping dialogTypeMapping = this.m_typeMapping.Find((DialogManager.DialogTypeMapping x) => x.m_type == request.m_type);
		if (dialogTypeMapping == null || dialogTypeMapping.m_prefabName == null)
		{
			Error.AddDevFatal("DialogManager.LoadPopup() - unhandled dialog type {0}", new object[]
			{
				request.m_type
			});
			return;
		}
		this.m_loadingDialog = true;
		AssetLoader.Get().LoadGameObject(FileUtils.GameAssetPathToName(dialogTypeMapping.m_prefabName), new AssetLoader.GameObjectCallback(this.OnPopupLoaded), null, true);
	}

	// Token: 0x06000F67 RID: 3943 RVA: 0x000435E8 File Offset: 0x000417E8
	private void OnPopupLoaded(string name, GameObject go, object callbackData)
	{
		this.m_loadingDialog = false;
		DialogManager.DialogRequest dialogRequest = this.m_dialogRequests.Peek();
		UserAttentionBlocker attentionCategory = (dialogRequest != null) ? dialogRequest.m_attentionCategory : UserAttentionBlocker.NONE;
		if (this.m_dialogRequests.Count == 0 || UserAttentionManager.IsBlockedBy(UserAttentionBlocker.FATAL_ERROR_SCENE) || !UserAttentionManager.CanShowAttentionGrabber(attentionCategory, "DialogManager.OnPopupLoaded:" + ((dialogRequest != null) ? dialogRequest.m_type.ToString() : "null")))
		{
			Object.DestroyImmediate(go);
			this.DestroyPopupAssetsIfPossible();
			return;
		}
		dialogRequest = this.m_dialogRequests.Dequeue();
		DialogBase component = go.GetComponent<DialogBase>();
		if (component == null)
		{
			Debug.LogError(string.Format("DialogManager.OnPopupLoaded() - game object {0} has no {1} component", go, dialogRequest.m_type));
			Object.DestroyImmediate(go);
			this.UpdateQueue();
			return;
		}
		this.ProcessRequest(dialogRequest, component);
	}

	// Token: 0x06000F68 RID: 3944 RVA: 0x000436C8 File Offset: 0x000418C8
	private void DestroyPopupAssetsIfPossible()
	{
		if (this.m_loadingDialog)
		{
			return;
		}
		foreach (DialogManager.DialogTypeMapping dialogTypeMapping in this.m_typeMapping)
		{
			AssetCache.ClearGameObject(FileUtils.GameAssetPathToName(dialogTypeMapping.m_prefabName));
		}
	}

	// Token: 0x06000F69 RID: 3945 RVA: 0x00043738 File Offset: 0x00041938
	private void ProcessRequest(DialogManager.DialogRequest request, DialogBase dialog)
	{
		if (request.m_callback != null && !request.m_callback(dialog, request.m_userData))
		{
			this.UpdateQueue();
			return;
		}
		this.m_currentDialog = dialog;
		this.m_currentDialog.AddHideListener(new DialogBase.HideCallback(this.OnCurrentDialogHidden));
		if (request.m_type == DialogManager.DialogType.ALERT)
		{
			this.ProcessAlertRequest(request, (AlertPopup)dialog);
		}
		else if (request.m_type == DialogManager.DialogType.SEASON_END)
		{
			this.ProcessMedalRequest(request, (SeasonEndDialog)dialog);
		}
		else if (request.m_type == DialogManager.DialogType.FRIENDLY_CHALLENGE || request.m_type == DialogManager.DialogType.TAVERN_BRAWL_CHALLENGE)
		{
			this.ProcessFriendlyChallengeRequest(request, (FriendlyChallengeDialog)dialog);
		}
		else if (request.m_type == DialogManager.DialogType.EXISTING_ACCOUNT)
		{
			this.ProcessExistingAccountRequest(request, (ExistingAccountPopup)dialog);
		}
		else if (request.m_type == DialogManager.DialogType.CARD_LIST)
		{
			this.ProcessCardListRequest(request, (CardListPopup)dialog);
		}
	}

	// Token: 0x06000F6A RID: 3946 RVA: 0x00043829 File Offset: 0x00041A29
	private void ProcessExistingAccountRequest(DialogManager.DialogRequest request, ExistingAccountPopup exAcctPopup)
	{
		exAcctPopup.SetInfo((ExistingAccountPopup.Info)request.m_info);
		exAcctPopup.Show();
	}

	// Token: 0x06000F6B RID: 3947 RVA: 0x00043844 File Offset: 0x00041A44
	private void ProcessAlertRequest(DialogManager.DialogRequest request, AlertPopup alertPopup)
	{
		AlertPopup.PopupInfo info = (AlertPopup.PopupInfo)request.m_info;
		alertPopup.SetInfo(info);
		alertPopup.Show();
	}

	// Token: 0x06000F6C RID: 3948 RVA: 0x0004386C File Offset: 0x00041A6C
	private void ProcessMedalRequest(DialogManager.DialogRequest request, SeasonEndDialog seasonEndDialog)
	{
		SeasonEndDialog.SeasonEndInfo seasonEndInfo;
		if (request.m_isFake)
		{
			seasonEndInfo = (request.m_info as SeasonEndDialog.SeasonEndInfo);
			if (seasonEndInfo == null)
			{
				return;
			}
		}
		else
		{
			DialogManager.SeasonEndDialogRequestInfo seasonEndDialogRequestInfo = request.m_info as DialogManager.SeasonEndDialogRequestInfo;
			seasonEndInfo = new SeasonEndDialog.SeasonEndInfo();
			seasonEndInfo.m_noticesToAck.Add(seasonEndDialogRequestInfo.m_noticeMedal.NoticeID);
			seasonEndInfo.m_seasonID = (int)seasonEndDialogRequestInfo.m_noticeMedal.OriginData;
			seasonEndInfo.m_rank = 26 - seasonEndDialogRequestInfo.m_noticeMedal.StarLevel;
			seasonEndInfo.m_chestRank = 26 - seasonEndDialogRequestInfo.m_noticeMedal.BestStarLevel;
			seasonEndInfo.m_legendIndex = seasonEndDialogRequestInfo.m_noticeMedal.LegendRank;
			seasonEndInfo.m_rankedRewards = seasonEndDialogRequestInfo.m_noticeMedal.Chest.Rewards;
			seasonEndInfo.m_isWild = seasonEndDialogRequestInfo.m_noticeMedal.IsWild;
			if (seasonEndDialogRequestInfo.m_noticeBonusStars != null)
			{
				seasonEndInfo.m_boostedRank = 26 - seasonEndDialogRequestInfo.m_noticeBonusStars.StarLevel;
				seasonEndInfo.m_bonusStars = seasonEndDialogRequestInfo.m_noticeBonusStars.Stars;
				seasonEndInfo.m_noticesToAck.Add(seasonEndDialogRequestInfo.m_noticeBonusStars.NoticeID);
			}
		}
		seasonEndDialog.Init(seasonEndInfo);
		seasonEndDialog.Show();
	}

	// Token: 0x06000F6D RID: 3949 RVA: 0x0004398B File Offset: 0x00041B8B
	private void ProcessFriendlyChallengeRequest(DialogManager.DialogRequest request, FriendlyChallengeDialog friendlyChallengeDialog)
	{
		friendlyChallengeDialog.SetInfo((FriendlyChallengeDialog.Info)request.m_info);
		friendlyChallengeDialog.Show();
	}

	// Token: 0x06000F6E RID: 3950 RVA: 0x000439A4 File Offset: 0x00041BA4
	private void ProcessCardListRequest(DialogManager.DialogRequest request, CardListPopup cardListPopup)
	{
		CardListPopup.Info info = (CardListPopup.Info)request.m_info;
		cardListPopup.SetInfo(info);
		cardListPopup.Show();
	}

	// Token: 0x06000F6F RID: 3951 RVA: 0x000439CA File Offset: 0x00041BCA
	private void OnCurrentDialogHidden(DialogBase dialog, object userData)
	{
		if (dialog != this.m_currentDialog)
		{
			return;
		}
		Object.Destroy(this.m_currentDialog.gameObject);
		this.m_currentDialog = null;
		this.UpdateQueue();
	}

	// Token: 0x06000F70 RID: 3952 RVA: 0x000439FC File Offset: 0x00041BFC
	private IEnumerator ShowSeasonEndDialogWhenReady(DialogManager.DialogRequest request)
	{
		this.m_waitingToShowSeasonEndDialog = true;
		while (NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>() == null || !this.m_isReadyForSeasonEndPopup)
		{
			yield return null;
		}
		while (SceneMgr.Get().IsTransitioning())
		{
			yield return null;
		}
		while (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB && SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN)
		{
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && !SceneMgr.Get().IsTransitioning())
			{
				SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
				break;
			}
			yield return null;
		}
		while (SceneMgr.Get().IsTransitioning())
		{
			yield return null;
		}
		this.AddToQueue(request);
		this.m_waitingToShowSeasonEndDialog = false;
		yield break;
	}

	// Token: 0x04000831 RID: 2097
	private static DialogManager s_instance;

	// Token: 0x04000832 RID: 2098
	private Queue<DialogManager.DialogRequest> m_dialogRequests = new Queue<DialogManager.DialogRequest>();

	// Token: 0x04000833 RID: 2099
	private DialogBase m_currentDialog;

	// Token: 0x04000834 RID: 2100
	private bool m_loadingDialog;

	// Token: 0x04000835 RID: 2101
	private bool m_isReadyForSeasonEndPopup;

	// Token: 0x04000836 RID: 2102
	private bool m_waitingToShowSeasonEndDialog;

	// Token: 0x04000837 RID: 2103
	private List<long> m_handledMedalNoticeIDs = new List<long>();

	// Token: 0x04000838 RID: 2104
	public List<DialogManager.DialogTypeMapping> m_typeMapping = new List<DialogManager.DialogTypeMapping>();

	// Token: 0x02000474 RID: 1140
	// (Invoke) Token: 0x060037AF RID: 14255
	public delegate bool DialogProcessCallback(DialogBase dialog, object userData);

	// Token: 0x02000475 RID: 1141
	public class DialogRequest
	{
		// Token: 0x04002392 RID: 9106
		public DialogManager.DialogType m_type;

		// Token: 0x04002393 RID: 9107
		public UserAttentionBlocker m_attentionCategory;

		// Token: 0x04002394 RID: 9108
		public object m_info;

		// Token: 0x04002395 RID: 9109
		public DialogManager.DialogProcessCallback m_callback;

		// Token: 0x04002396 RID: 9110
		public object m_userData;

		// Token: 0x04002397 RID: 9111
		public bool m_isFake;
	}

	// Token: 0x02000476 RID: 1142
	public enum DialogType
	{
		// Token: 0x04002399 RID: 9113
		ALERT,
		// Token: 0x0400239A RID: 9114
		SEASON_END,
		// Token: 0x0400239B RID: 9115
		FRIENDLY_CHALLENGE,
		// Token: 0x0400239C RID: 9116
		TAVERN_BRAWL_CHALLENGE,
		// Token: 0x0400239D RID: 9117
		EXISTING_ACCOUNT,
		// Token: 0x0400239E RID: 9118
		CARD_LIST
	}

	// Token: 0x02000477 RID: 1143
	[Serializable]
	public class DialogTypeMapping
	{
		// Token: 0x0400239F RID: 9119
		public DialogManager.DialogType m_type;

		// Token: 0x040023A0 RID: 9120
		[CustomEditField(T = EditType.GAME_OBJECT)]
		public string m_prefabName;
	}

	// Token: 0x02000478 RID: 1144
	private class SeasonEndDialogRequestInfo
	{
		// Token: 0x040023A1 RID: 9121
		public NetCache.ProfileNoticeMedal m_noticeMedal;

		// Token: 0x040023A2 RID: 9122
		public NetCache.ProfileNoticeBonusStars m_noticeBonusStars;

		// Token: 0x040023A3 RID: 9123
		public NetCache.ProfileNoticeRewardCardBack m_noticeCardBack;
	}
}
