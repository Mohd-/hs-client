using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200016C RID: 364
public class QuestToast : MonoBehaviour
{
	// Token: 0x060013B8 RID: 5048 RVA: 0x00057C48 File Offset: 0x00055E48
	public void Awake()
	{
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x00057C68 File Offset: 0x00055E68
	public void OnDestroy()
	{
		if (this == QuestToast.m_activeQuest)
		{
			if (QuestToast.m_questActive)
			{
				this.FadeEffectsOut();
				QuestToast.m_questActive = false;
			}
			QuestToast.m_activeQuest = null;
		}
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x00057CA1 File Offset: 0x00055EA1
	public static void ShowQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, bool updateCacheValues, Achievement quest)
	{
		QuestToast.ShowQuestToast(blocker, onClosedCallback, updateCacheValues, quest, true);
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x00057CAD File Offset: 0x00055EAD
	public static void ShowQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, bool updateCacheValues, Achievement quest, bool fullScreenEffects)
	{
		QuestToast.ShowQuestToast(blocker, onClosedCallback, null, updateCacheValues, quest, fullScreenEffects);
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x00057CBB File Offset: 0x00055EBB
	public static void ShowQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, object callbackUserData, bool updateCacheValues, Achievement quest)
	{
		QuestToast.ShowQuestToast(blocker, onClosedCallback, callbackUserData, updateCacheValues, quest, true);
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x00057CCC File Offset: 0x00055ECC
	public static void ShowQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, object callbackUserData, bool updateCacheValues, Achievement quest, bool fullscreenEffects)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "ShowQuestToast:" + ((quest != null) ? quest.ID.ToString() : "null")))
		{
			if (onClosedCallback != null)
			{
				onClosedCallback(callbackUserData);
			}
			return;
		}
		quest.AckCurrentProgressAndRewardNotices();
		if (quest.ID == 56)
		{
			return;
		}
		QuestToast.m_showFullscreenEffects = fullscreenEffects;
		QuestToast.m_questActive = true;
		QuestToast.ToastCallbackData callbackData = new QuestToast.ToastCallbackData
		{
			m_toastRewards = quest.Rewards,
			m_toastName = quest.Name,
			m_toastDescription = quest.Description,
			m_onCloseCallback = onClosedCallback,
			m_onCloseCallbackData = callbackUserData,
			m_updateCacheValues = updateCacheValues
		};
		AssetLoader.Get().LoadActor("QuestToast", true, new AssetLoader.GameObjectCallback(QuestToast.PositionActor), callbackData, false);
		SoundManager.Get().LoadAndPlay("Quest_Complete_Jingle");
		SoundManager.Get().LoadAndPlay("quest_complete_pop_up");
		SoundManager.Get().LoadAndPlay("tavern_crowd_play_reaction_positive_random");
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x00057DCF File Offset: 0x00055FCF
	public static void ShowFixedRewardQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, RewardData rewardData, string name, string description)
	{
		QuestToast.ShowFixedRewardQuestToast(blocker, onClosedCallback, null, rewardData, name, description, true);
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x00057DDE File Offset: 0x00055FDE
	public static void ShowFixedRewardQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, RewardData rewardData, string name, string description, bool fullscreenEffects)
	{
		QuestToast.ShowFixedRewardQuestToast(blocker, onClosedCallback, null, rewardData, name, description, fullscreenEffects);
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x00057DEE File Offset: 0x00055FEE
	public static void ShowFixedRewardQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, object callbackUserData, RewardData rewardData, string name, string description)
	{
		QuestToast.ShowFixedRewardQuestToast(blocker, onClosedCallback, null, rewardData, name, description, true);
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x00057E00 File Offset: 0x00056000
	public static void ShowFixedRewardQuestToast(UserAttentionBlocker blocker, QuestToast.DelOnCloseQuestToast onClosedCallback, object callbackUserData, RewardData rewardData, string name, string description, bool fullscreenEffects)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "ShowFixedRewardQuestToast:" + ((rewardData != null) ? string.Concat(new object[]
		{
			rewardData.Origin,
			":",
			rewardData.OriginData,
			":",
			rewardData.RewardType
		}) : "null")))
		{
			return;
		}
		QuestToast.m_showFullscreenEffects = fullscreenEffects;
		QuestToast.m_questActive = true;
		List<RewardData> list = new List<RewardData>();
		list.Add(rewardData);
		QuestToast.ToastCallbackData callbackData = new QuestToast.ToastCallbackData
		{
			m_toastRewards = list,
			m_toastName = name,
			m_toastDescription = description,
			m_onCloseCallback = onClosedCallback,
			m_onCloseCallbackData = callbackUserData,
			m_updateCacheValues = true
		};
		AssetLoader.Get().LoadActor("QuestToast", true, new AssetLoader.GameObjectCallback(QuestToast.PositionActor), callbackData, false);
		SoundManager.Get().LoadAndPlay("Quest_Complete_Jingle");
		SoundManager.Get().LoadAndPlay("quest_complete_pop_up");
		SoundManager.Get().LoadAndPlay("tavern_crowd_play_reaction_positive_random");
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x00057F14 File Offset: 0x00056114
	private static void PositionActor(string actorName, GameObject actorObject, object c)
	{
		actorObject.transform.localPosition = new Vector3(6f, 5f, 3f);
		Vector3 localScale = actorObject.transform.localScale;
		actorObject.transform.localScale = 0.01f * Vector3.one;
		actorObject.SetActive(true);
		iTween.ScaleTo(actorObject, localScale, 0.5f);
		QuestToast component = actorObject.GetComponent<QuestToast>();
		if (component == null)
		{
			Debug.LogWarning("QuestToast.PositionActor(): actor has no QuestToast component");
			QuestToast.m_questActive = false;
		}
		else
		{
			QuestToast.m_activeQuest = component;
			QuestToast.ToastCallbackData toastCallbackData = c as QuestToast.ToastCallbackData;
			component.m_onCloseCallback = toastCallbackData.m_onCloseCallback;
			component.m_toastRewards = toastCallbackData.m_toastRewards;
			component.m_toastName = toastCallbackData.m_toastName;
			component.m_toastDescription = toastCallbackData.m_toastDescription;
			component.SetUpToast(toastCallbackData.m_updateCacheValues);
		}
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x00057FEA File Offset: 0x000561EA
	private void CloseQuestToast(UIEvent e)
	{
		this.CloseQuestToast();
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x00057FF4 File Offset: 0x000561F4
	public void CloseQuestToast()
	{
		if (base.gameObject == null)
		{
			return;
		}
		QuestToast.m_questActive = false;
		this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseQuestToast));
		this.FadeEffectsOut();
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.5f,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"DestroyQuestToast"
		}));
		if (this.m_onCloseCallback == null)
		{
			return;
		}
		this.m_onCloseCallback(this.m_onCloseCallbackData);
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x000580B6 File Offset: 0x000562B6
	public static bool IsQuestActive()
	{
		return QuestToast.m_questActive && QuestToast.m_activeQuest != null;
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x000580D0 File Offset: 0x000562D0
	public static QuestToast GetCurrentToast()
	{
		return QuestToast.m_activeQuest;
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x000580D7 File Offset: 0x000562D7
	private void DestroyQuestToast()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x000580E4 File Offset: 0x000562E4
	public void SetUpToast(bool updateCacheValues)
	{
		this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseQuestToast));
		this.m_questName.Text = this.m_toastName;
		this.m_requirement.Text = this.m_toastDescription;
		RewardData rewardData = null;
		if (this.m_toastRewards != null)
		{
			using (List<RewardData>.Enumerator enumerator = this.m_toastRewards.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					RewardData rewardData2 = enumerator.Current;
					rewardData = rewardData2;
				}
			}
		}
		if (rewardData != null)
		{
			rewardData.LoadRewardObject(new Reward.DelOnRewardLoaded(this.RewardObjectLoaded), updateCacheValues);
		}
		this.FadeEffectsIn();
	}

	// Token: 0x060013C9 RID: 5065 RVA: 0x000581AC File Offset: 0x000563AC
	private void RewardObjectLoaded(Reward reward, object callbackData)
	{
		bool updateCacheValues = (bool)callbackData;
		reward.Hide(false);
		reward.transform.parent = this.m_rewardBone;
		reward.transform.localEulerAngles = Vector3.zero;
		reward.transform.localScale = QuestToast.REWARD_SCALE;
		reward.transform.localPosition = Vector3.zero;
		BoosterPackReward componentInChildren = reward.gameObject.GetComponentInChildren<BoosterPackReward>();
		if (componentInChildren != null)
		{
			componentInChildren.m_Layer = (GameLayer)base.gameObject.layer;
		}
		SceneUtils.SetLayer(reward.gameObject, base.gameObject.layer);
		reward.Show(updateCacheValues);
	}

	// Token: 0x060013CA RID: 5066 RVA: 0x00058250 File Offset: 0x00056450
	private void FadeEffectsIn()
	{
		if (!QuestToast.m_showFullscreenEffects)
		{
			return;
		}
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x060013CB RID: 5067 RVA: 0x000582AC File Offset: 0x000564AC
	private void FadeEffectsOut()
	{
		if (!QuestToast.m_showFullscreenEffects)
		{
			return;
		}
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x04000A2C RID: 2604
	public UberText m_questName;

	// Token: 0x04000A2D RID: 2605
	public GameObject m_nameLine;

	// Token: 0x04000A2E RID: 2606
	public UberText m_requirement;

	// Token: 0x04000A2F RID: 2607
	public Transform m_rewardBone;

	// Token: 0x04000A30 RID: 2608
	public PegUIElement m_clickCatcher;

	// Token: 0x04000A31 RID: 2609
	private QuestToast.DelOnCloseQuestToast m_onCloseCallback;

	// Token: 0x04000A32 RID: 2610
	private object m_onCloseCallbackData;

	// Token: 0x04000A33 RID: 2611
	private List<RewardData> m_toastRewards;

	// Token: 0x04000A34 RID: 2612
	private string m_toastName = string.Empty;

	// Token: 0x04000A35 RID: 2613
	private string m_toastDescription = string.Empty;

	// Token: 0x04000A36 RID: 2614
	private static bool m_showFullscreenEffects = true;

	// Token: 0x04000A37 RID: 2615
	private static bool m_questActive;

	// Token: 0x04000A38 RID: 2616
	private static QuestToast m_activeQuest;

	// Token: 0x04000A39 RID: 2617
	private static readonly Vector3 REWARD_SCALE = new Vector3(1.42f, 1.42f, 1.42f);

	// Token: 0x0200016D RID: 365
	// (Invoke) Token: 0x060013CD RID: 5069
	public delegate void DelOnCloseQuestToast(object userData);

	// Token: 0x0200069D RID: 1693
	private class ToastCallbackData
	{
		// Token: 0x04002E58 RID: 11864
		public QuestToast.DelOnCloseQuestToast m_onCloseCallback;

		// Token: 0x04002E59 RID: 11865
		public object m_onCloseCallbackData;

		// Token: 0x04002E5A RID: 11866
		public List<RewardData> m_toastRewards;

		// Token: 0x04002E5B RID: 11867
		public string m_toastName = string.Empty;

		// Token: 0x04002E5C RID: 11868
		public string m_toastDescription = string.Empty;

		// Token: 0x04002E5D RID: 11869
		public bool m_updateCacheValues;
	}
}
