using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200001D RID: 29
public abstract class RewardData
{
	// Token: 0x060002B0 RID: 688 RVA: 0x0000CD17 File Offset: 0x0000AF17
	protected RewardData(Reward.Type type)
	{
		this.m_type = type;
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060002B1 RID: 689 RVA: 0x0000CD38 File Offset: 0x0000AF38
	public Reward.Type RewardType
	{
		get
		{
			return this.m_type;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000CD40 File Offset: 0x0000AF40
	public NetCache.ProfileNotice.NoticeOrigin Origin
	{
		get
		{
			return this.m_origin;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002B3 RID: 691 RVA: 0x0000CD48 File Offset: 0x0000AF48
	public long OriginData
	{
		get
		{
			return this.m_originData;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060002B4 RID: 692 RVA: 0x0000CD50 File Offset: 0x0000AF50
	public bool IsDummyReward
	{
		get
		{
			return this.m_isDummyReward;
		}
	}

	// Token: 0x060002B5 RID: 693 RVA: 0x0000CD58 File Offset: 0x0000AF58
	public void LoadRewardObject(Reward.DelOnRewardLoaded callback)
	{
		this.LoadRewardObject(callback, null);
	}

	// Token: 0x060002B6 RID: 694 RVA: 0x0000CD64 File Offset: 0x0000AF64
	public void LoadRewardObject(Reward.DelOnRewardLoaded callback, object callbackData)
	{
		string gameObjectName = this.GetGameObjectName();
		if (string.IsNullOrEmpty(gameObjectName))
		{
			Debug.LogError(string.Format("Reward.LoadRewardObject(): Do not know how to load reward object for {0}.", this));
			return;
		}
		Reward.LoadRewardCallbackData callbackData2 = new Reward.LoadRewardCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData
		};
		AssetLoader.Get().LoadGameObject(gameObjectName, new AssetLoader.GameObjectCallback(this.OnRewardObjectLoaded), callbackData2, false);
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0000CDC4 File Offset: 0x0000AFC4
	public void SetOrigin(NetCache.ProfileNotice.NoticeOrigin origin, long originData)
	{
		this.m_origin = origin;
		this.m_originData = originData;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000CDD4 File Offset: 0x0000AFD4
	public void AddNoticeID(long noticeID)
	{
		if (this.m_noticeIDs.Contains(noticeID))
		{
			return;
		}
		this.m_noticeIDs.Add(noticeID);
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x0000CDF4 File Offset: 0x0000AFF4
	public List<long> GetNoticeIDs()
	{
		return this.m_noticeIDs;
	}

	// Token: 0x060002BA RID: 698 RVA: 0x0000CDFC File Offset: 0x0000AFFC
	public void AcknowledgeNotices()
	{
		long[] array = this.m_noticeIDs.ToArray();
		this.m_noticeIDs.Clear();
		foreach (long id in array)
		{
			Network.AckNotice(id);
		}
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0000CE40 File Offset: 0x0000B040
	public void MarkAsDummyReward()
	{
		this.m_isDummyReward = true;
	}

	// Token: 0x060002BC RID: 700
	protected abstract string GetGameObjectName();

	// Token: 0x060002BD RID: 701 RVA: 0x0000CE4C File Offset: 0x0000B04C
	private void OnRewardObjectLoaded(string name, GameObject go, object callbackData)
	{
		Reward component = go.GetComponent<Reward>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("Reward.OnRewardObjectLoaded() - game object loaded from {0} has no reward component", name));
		}
		else
		{
			component.SetData(this, true);
		}
		Reward.LoadRewardCallbackData loadRewardCallbackData = callbackData as Reward.LoadRewardCallbackData;
		component.NotifyLoadedWhenReady(loadRewardCallbackData);
	}

	// Token: 0x040000F6 RID: 246
	private Reward.Type m_type;

	// Token: 0x040000F7 RID: 247
	private NetCache.ProfileNotice.NoticeOrigin m_origin = NetCache.ProfileNotice.NoticeOrigin.UNKNOWN;

	// Token: 0x040000F8 RID: 248
	private long m_originData;

	// Token: 0x040000F9 RID: 249
	protected List<long> m_noticeIDs = new List<long>();

	// Token: 0x040000FA RID: 250
	private bool m_isDummyReward;
}
