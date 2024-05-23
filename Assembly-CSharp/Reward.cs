using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001F RID: 31
public abstract class Reward : MonoBehaviour
{
	// Token: 0x060002E7 RID: 743 RVA: 0x0000E3F9 File Offset: 0x0000C5F9
	protected Reward()
	{
		this.InitData();
	}

	// Token: 0x060002E8 RID: 744 RVA: 0x0000E424 File Offset: 0x0000C624
	protected virtual void Awake()
	{
		if (this.m_rewardBannerPrefab != null)
		{
			if (!UniversalInputManager.UsePhoneUI)
			{
				this.m_rewardBanner = Object.Instantiate<RewardBanner>(this.m_rewardBannerPrefab);
				this.m_rewardBanner.gameObject.SetActive(false);
				this.m_rewardBanner.transform.parent = this.m_rewardBannerBone.transform;
				this.m_rewardBanner.transform.localPosition = Vector3.zero;
			}
			else
			{
				this.m_rewardBanner = (RewardBanner)GameUtils.Instantiate(this.m_rewardBannerPrefab, this.m_rewardBannerBone, false);
			}
		}
		this.EnableClickCatcher(false);
		SoundManager.Get().Load("game_end_reward");
	}

	// Token: 0x060002E9 RID: 745 RVA: 0x0000E4DC File Offset: 0x0000C6DC
	private void Start()
	{
		if (this.m_clickCatcher != null)
		{
			this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnClickReleased));
		}
		this.Hide(false);
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060002EA RID: 746 RVA: 0x0000E50F File Offset: 0x0000C70F
	public Reward.Type RewardType
	{
		get
		{
			return this.Data.RewardType;
		}
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060002EB RID: 747 RVA: 0x0000E51C File Offset: 0x0000C71C
	public RewardData Data
	{
		get
		{
			return this.m_data;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060002EC RID: 748 RVA: 0x0000E524 File Offset: 0x0000C724
	public bool IsShown
	{
		get
		{
			return this.m_shown;
		}
	}

	// Token: 0x060002ED RID: 749 RVA: 0x0000E52C File Offset: 0x0000C72C
	public void Show(bool updateCacheValues)
	{
		this.Data.AcknowledgeNotices();
		if (this.m_rewardBanner != null)
		{
			this.m_rewardBanner.gameObject.SetActive(true);
		}
		this.ShowReward(updateCacheValues);
		this.m_shown = true;
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0000E574 File Offset: 0x0000C774
	public void Hide(bool animate = false)
	{
		if (!animate)
		{
			this.OnHideAnimateComplete();
			return;
		}
		iTween.FadeTo(base.gameObject, 0f, RewardUtils.REWARD_HIDE_TIME);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			RewardUtils.REWARD_HIDDEN_SCALE,
			"time",
			RewardUtils.REWARD_HIDE_TIME,
			"oncomplete",
			"OnHideAnimateComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.ScaleTo(base.gameObject, args);
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0000E606 File Offset: 0x0000C806
	private void OnHideAnimateComplete()
	{
		this.HideReward();
		this.m_shown = false;
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x0000E615 File Offset: 0x0000C815
	public void SetData(RewardData data, bool updateVisuals)
	{
		this.m_data = data;
		this.OnDataSet(updateVisuals);
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x0000E625 File Offset: 0x0000C825
	public void NotifyLoadedWhenReady(Reward.LoadRewardCallbackData loadRewardCallbackData)
	{
		base.StartCoroutine(this.WaitThenNotifyLoaded(loadRewardCallbackData));
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x0000E638 File Offset: 0x0000C838
	public void EnableClickCatcher(bool enabled)
	{
		if (this.m_clickCatcher != null)
		{
			this.m_clickCatcher.gameObject.SetActive(enabled);
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0000E667 File Offset: 0x0000C867
	public bool RegisterClickListener(Reward.OnClickedCallback callback)
	{
		return this.RegisterClickListener(callback, null);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0000E674 File Offset: 0x0000C874
	public bool RegisterClickListener(Reward.OnClickedCallback callback, object userData)
	{
		Reward.OnClickedListener onClickedListener = new Reward.OnClickedListener();
		onClickedListener.SetCallback(callback);
		onClickedListener.SetUserData(userData);
		if (this.m_clickListeners.Contains(onClickedListener))
		{
			return false;
		}
		this.m_clickListeners.Add(onClickedListener);
		return true;
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x0000E6B5 File Offset: 0x0000C8B5
	public bool RemoveClickListener(Reward.OnClickedCallback callback)
	{
		return this.RemoveClickListener(callback, null);
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0000E6C0 File Offset: 0x0000C8C0
	public bool RemoveClickListener(Reward.OnClickedCallback callback, object userData)
	{
		Reward.OnClickedListener onClickedListener = new Reward.OnClickedListener();
		onClickedListener.SetCallback(callback);
		onClickedListener.SetUserData(userData);
		return this.m_clickListeners.Remove(onClickedListener);
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x0000E6ED File Offset: 0x0000C8ED
	public bool RegisterHideListener(Reward.OnHideCallback callback)
	{
		return this.RegisterHideListener(callback, null);
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x0000E6F8 File Offset: 0x0000C8F8
	public bool RegisterHideListener(Reward.OnHideCallback callback, object userData)
	{
		Reward.OnHideListener onHideListener = new Reward.OnHideListener();
		onHideListener.SetCallback(callback);
		onHideListener.SetUserData(userData);
		if (this.m_hideListeners.Contains(onHideListener))
		{
			return false;
		}
		this.m_hideListeners.Add(onHideListener);
		return true;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x0000E73C File Offset: 0x0000C93C
	public void RemoveHideListener(Reward.OnHideCallback callback, object userData)
	{
		Reward.OnHideListener onHideListener = new Reward.OnHideListener();
		onHideListener.SetCallback(callback);
		onHideListener.SetUserData(userData);
		this.m_hideListeners.Remove(onHideListener);
	}

	// Token: 0x060002FA RID: 762
	protected abstract void InitData();

	// Token: 0x060002FB RID: 763 RVA: 0x0000E76A File Offset: 0x0000C96A
	protected virtual void ShowReward(bool updateCacheValues)
	{
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0000E76C File Offset: 0x0000C96C
	protected virtual void OnDataSet(bool updateVisuals)
	{
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0000E76E File Offset: 0x0000C96E
	protected virtual void HideReward()
	{
		this.OnHide();
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0000E776 File Offset: 0x0000C976
	protected void SetReady(bool ready)
	{
		this.m_ready = ready;
	}

	// Token: 0x060002FF RID: 767 RVA: 0x0000E780 File Offset: 0x0000C980
	protected void SetRewardText(string headline, string details, string source)
	{
		if (UniversalInputManager.UsePhoneUI && this.RewardType != Reward.Type.GOLD)
		{
			details = string.Empty;
		}
		if (this.m_rewardBanner != null)
		{
			this.m_rewardBanner.SetText(headline, details, source);
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0000E7D0 File Offset: 0x0000C9D0
	private IEnumerator WaitThenNotifyLoaded(Reward.LoadRewardCallbackData loadRewardCallbackData)
	{
		if (loadRewardCallbackData.m_callback == null)
		{
			yield break;
		}
		while (!this.m_ready)
		{
			yield return null;
		}
		loadRewardCallbackData.m_callback(this, loadRewardCallbackData.m_callbackData);
		yield break;
	}

	// Token: 0x06000301 RID: 769 RVA: 0x0000E7FC File Offset: 0x0000C9FC
	private void OnClickReleased(UIEvent e)
	{
		Reward.OnClickedListener[] array = this.m_clickListeners.ToArray();
		foreach (Reward.OnClickedListener onClickedListener in array)
		{
			onClickedListener.Fire(this);
		}
	}

	// Token: 0x06000302 RID: 770 RVA: 0x0000E838 File Offset: 0x0000CA38
	private void OnHide()
	{
		Reward.OnHideListener[] array = this.m_hideListeners.ToArray();
		foreach (Reward.OnHideListener onHideListener in array)
		{
			onHideListener.Fire();
		}
	}

	// Token: 0x04000106 RID: 262
	public GameObject m_root;

	// Token: 0x04000107 RID: 263
	public RewardBanner m_rewardBannerPrefab;

	// Token: 0x04000108 RID: 264
	public GameObject m_rewardBannerBone;

	// Token: 0x04000109 RID: 265
	public PegUIElement m_clickCatcher;

	// Token: 0x0400010A RID: 266
	protected RewardBanner m_rewardBanner;

	// Token: 0x0400010B RID: 267
	private RewardData m_data;

	// Token: 0x0400010C RID: 268
	private Reward.Type m_type;

	// Token: 0x0400010D RID: 269
	private bool m_ready = true;

	// Token: 0x0400010E RID: 270
	private bool m_shown;

	// Token: 0x0400010F RID: 271
	private List<Reward.OnClickedListener> m_clickListeners = new List<Reward.OnClickedListener>();

	// Token: 0x04000110 RID: 272
	private List<Reward.OnHideListener> m_hideListeners = new List<Reward.OnHideListener>();

	// Token: 0x02000020 RID: 32
	public enum Type
	{
		// Token: 0x04000112 RID: 274
		ARCANE_DUST,
		// Token: 0x04000113 RID: 275
		BOOSTER_PACK,
		// Token: 0x04000114 RID: 276
		CARD,
		// Token: 0x04000115 RID: 277
		CARD_BACK,
		// Token: 0x04000116 RID: 278
		CRAFTABLE_CARD,
		// Token: 0x04000117 RID: 279
		FORGE_TICKET,
		// Token: 0x04000118 RID: 280
		GOLD,
		// Token: 0x04000119 RID: 281
		MOUNT,
		// Token: 0x0400011A RID: 282
		CLASS_CHALLENGE
	}

	// Token: 0x0200014F RID: 335
	// (Invoke) Token: 0x060011DD RID: 4573
	public delegate void DelOnRewardLoaded(Reward reward, object callbackData);

	// Token: 0x02000150 RID: 336
	public class LoadRewardCallbackData
	{
		// Token: 0x04000962 RID: 2402
		public Reward.DelOnRewardLoaded m_callback;

		// Token: 0x04000963 RID: 2403
		public object m_callbackData;
	}

	// Token: 0x02000163 RID: 355
	// (Invoke) Token: 0x06001348 RID: 4936
	public delegate void OnClickedCallback(Reward reward, object userData);

	// Token: 0x0200016E RID: 366
	// (Invoke) Token: 0x060013D1 RID: 5073
	public delegate void OnHideCallback(object userData);

	// Token: 0x0200016F RID: 367
	private class OnClickedListener : EventListener<Reward.OnClickedCallback>
	{
		// Token: 0x060013D5 RID: 5077 RVA: 0x000582EE File Offset: 0x000564EE
		public void Fire(Reward reward)
		{
			this.m_callback(reward, this.m_userData);
		}
	}

	// Token: 0x02000170 RID: 368
	private class OnHideListener : EventListener<Reward.OnHideCallback>
	{
		// Token: 0x060013D7 RID: 5079 RVA: 0x0005830A File Offset: 0x0005650A
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
