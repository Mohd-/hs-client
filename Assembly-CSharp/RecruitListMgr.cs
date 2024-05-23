using System;
using System.Collections.Generic;
using bgs;

// Token: 0x02000583 RID: 1411
public class RecruitListMgr
{
	// Token: 0x06004014 RID: 16404 RVA: 0x00136A90 File Offset: 0x00134C90
	private RecruitListMgr()
	{
	}

	// Token: 0x06004016 RID: 16406 RVA: 0x00136AD1 File Offset: 0x00134CD1
	public static RecruitListMgr Get()
	{
		if (RecruitListMgr.s_instance == null)
		{
			RecruitListMgr.s_instance = new RecruitListMgr();
		}
		return RecruitListMgr.s_instance;
	}

	// Token: 0x06004017 RID: 16407 RVA: 0x00136AEC File Offset: 0x00134CEC
	public void Init()
	{
		if (RecruitListMgr.s_instance == null)
		{
			RecruitListMgr.s_instance = new RecruitListMgr();
		}
	}

	// Token: 0x06004018 RID: 16408 RVA: 0x00136B04 File Offset: 0x00134D04
	private int secondsSinceEpoch()
	{
		return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
	}

	// Token: 0x06004019 RID: 16409 RVA: 0x00136B34 File Offset: 0x00134D34
	public void SendRecruitAFriendInvite(string email)
	{
		char[] array = new char[]
		{
			':'
		};
		string[] array2 = email.Split(array);
		ulong lo = 0UL;
		Network.RecruitInfo recruitInfo;
		if (ulong.TryParse(array2[0], ref lo))
		{
			BnetAccountId bnetAccountId = new BnetAccountId();
			bnetAccountId.SetHi(72057594037927936UL);
			bnetAccountId.SetLo(lo);
			recruitInfo = new Network.RecruitInfo
			{
				RecruitID = bnetAccountId
			};
			Network.RecruitInfo recruitInfoFromAccountId = this.GetRecruitInfoFromAccountId(bnetAccountId);
			if (recruitInfoFromAccountId != null)
			{
				this.m_recruits.Remove(recruitInfoFromAccountId);
			}
		}
		else
		{
			Network.RecruitInfo recruitInfo2 = new Network.RecruitInfo();
			Network.RecruitInfo recruitInfo3 = recruitInfo2;
			ulong id;
			this.s_id = (id = this.s_id) + 1UL;
			recruitInfo3.ID = id;
			recruitInfo = recruitInfo2;
		}
		recruitInfo.Nickname = array2[0];
		if (array2.Length > 1)
		{
			recruitInfo.Status = int.Parse(array2[1]);
		}
		if (array2.Length > 2)
		{
			recruitInfo.Level = int.Parse(array2[2]);
		}
		recruitInfo.CreationTimeMicrosec = (ulong)((long)this.secondsSinceEpoch() * 1000L * 1000L);
		this.m_recruits.Add(recruitInfo);
		this.OnRecruitListResponse();
	}

	// Token: 0x0600401A RID: 16410 RVA: 0x00136C48 File Offset: 0x00134E48
	public void RecruitFriendCancel(ulong uniqueID)
	{
		Network.RecruitInfo recruitInfoFromId = this.GetRecruitInfoFromId(uniqueID);
		if (recruitInfoFromId != null)
		{
			this.m_recruits.Remove(recruitInfoFromId);
			this.FireRecruitsChangedEvent();
		}
	}

	// Token: 0x0600401B RID: 16411 RVA: 0x00136C78 File Offset: 0x00134E78
	public bool CanAddMoreRecruits()
	{
		int num = 0;
		foreach (Network.RecruitInfo recruitInfo in this.m_recruits)
		{
			if (recruitInfo.RecruitID.IsEmpty())
			{
				num++;
			}
		}
		return num < 5;
	}

	// Token: 0x0600401C RID: 16412 RVA: 0x00136CE8 File Offset: 0x00134EE8
	public Network.RecruitInfo GetRecruitInfoFromAccountId(BnetAccountId gameAccountID)
	{
		foreach (Network.RecruitInfo recruitInfo in this.m_recruits)
		{
			if (recruitInfo.RecruitID == gameAccountID)
			{
				return recruitInfo;
			}
		}
		return null;
	}

	// Token: 0x0600401D RID: 16413 RVA: 0x00136D58 File Offset: 0x00134F58
	public Network.RecruitInfo GetRecruitInfoFromId(ulong uniqueID)
	{
		foreach (Network.RecruitInfo recruitInfo in this.m_recruits)
		{
			if (recruitInfo.ID == uniqueID)
			{
				return recruitInfo;
			}
		}
		return null;
	}

	// Token: 0x0600401E RID: 16414 RVA: 0x00136DC4 File Offset: 0x00134FC4
	public List<Network.RecruitInfo> GetRecruitList()
	{
		return this.m_recruits;
	}

	// Token: 0x0600401F RID: 16415 RVA: 0x00136DCC File Offset: 0x00134FCC
	public void RefreshRecruitList()
	{
		this.OnRecruitListResponse();
	}

	// Token: 0x06004020 RID: 16416 RVA: 0x00136DD4 File Offset: 0x00134FD4
	public static bool IsValidRecruitInput(string email)
	{
		return !FriendUtils.IsValidEmail(email) || true;
	}

	// Token: 0x06004021 RID: 16417 RVA: 0x00136DE4 File Offset: 0x00134FE4
	private void OnReset()
	{
		this.m_recruits.Clear();
	}

	// Token: 0x06004022 RID: 16418 RVA: 0x00136DF4 File Offset: 0x00134FF4
	private void OnRecruitListResponse()
	{
		Log.Cameron.Print("recruit list response!", new object[0]);
		this.FireRecruitsChangedEvent();
		this.FireRecruitAcceptedEvent();
		this.m_lastRecruits = new List<Network.RecruitInfo>(this.m_recruits);
	}

	// Token: 0x06004023 RID: 16419 RVA: 0x00136E34 File Offset: 0x00135034
	private void FireRecruitsChangedEvent()
	{
		RecruitListMgr.RecruitsChangedListener[] array = this.m_recruitsChangedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire();
		}
	}

	// Token: 0x06004024 RID: 16420 RVA: 0x00136E6C File Offset: 0x0013506C
	private void FireRecruitAcceptedEvent()
	{
		foreach (Network.RecruitInfo recruitInfo in this.m_lastRecruits)
		{
			Network.RecruitInfo recruitInfoFromId = this.GetRecruitInfoFromId(recruitInfo.ID);
			if (recruitInfoFromId != null && recruitInfo.Status != 4 && recruitInfoFromId.Status == 4)
			{
				Log.Cameron.Print("comparing recruits", new object[0]);
				Log.Cameron.Print("old recruit " + recruitInfo, new object[0]);
				Log.Cameron.Print("new recruit " + recruitInfoFromId, new object[0]);
				RecruitListMgr.RecruitAcceptedListener[] array = this.m_recruitAcceptedListeners.ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					Log.Cameron.Print("recruit accepted " + recruitInfoFromId.Nickname, new object[0]);
					array[i].Fire(recruitInfoFromId);
				}
			}
		}
	}

	// Token: 0x06004025 RID: 16421 RVA: 0x00136F84 File Offset: 0x00135184
	public bool AddRecruitsChangedListener(RecruitListMgr.RecruitsChangedCallback callback)
	{
		RecruitListMgr.RecruitsChangedListener recruitsChangedListener = new RecruitListMgr.RecruitsChangedListener();
		recruitsChangedListener.SetCallback(callback);
		if (this.m_recruitsChangedListeners.Contains(recruitsChangedListener))
		{
			return false;
		}
		this.m_recruitsChangedListeners.Add(recruitsChangedListener);
		return true;
	}

	// Token: 0x06004026 RID: 16422 RVA: 0x00136FC0 File Offset: 0x001351C0
	public bool RemoveRecruitsChangedListener(RecruitListMgr.RecruitsChangedCallback callback)
	{
		RecruitListMgr.RecruitsChangedListener recruitsChangedListener = new RecruitListMgr.RecruitsChangedListener();
		recruitsChangedListener.SetCallback(callback);
		return this.m_recruitsChangedListeners.Remove(recruitsChangedListener);
	}

	// Token: 0x06004027 RID: 16423 RVA: 0x00136FE8 File Offset: 0x001351E8
	public bool AddRecruitAcceptedListener(RecruitListMgr.RecruitAcceptedCallback callback)
	{
		RecruitListMgr.RecruitAcceptedListener recruitAcceptedListener = new RecruitListMgr.RecruitAcceptedListener();
		recruitAcceptedListener.SetCallback(callback);
		if (this.m_recruitAcceptedListeners.Contains(recruitAcceptedListener))
		{
			return false;
		}
		this.m_recruitAcceptedListeners.Add(recruitAcceptedListener);
		return true;
	}

	// Token: 0x04002906 RID: 10502
	private ulong s_id;

	// Token: 0x04002907 RID: 10503
	private List<RecruitListMgr.RecruitsChangedListener> m_recruitsChangedListeners = new List<RecruitListMgr.RecruitsChangedListener>();

	// Token: 0x04002908 RID: 10504
	private List<RecruitListMgr.RecruitAcceptedListener> m_recruitAcceptedListeners = new List<RecruitListMgr.RecruitAcceptedListener>();

	// Token: 0x04002909 RID: 10505
	private static RecruitListMgr s_instance;

	// Token: 0x0400290A RID: 10506
	private List<Network.RecruitInfo> m_recruits = new List<Network.RecruitInfo>();

	// Token: 0x0400290B RID: 10507
	private List<Network.RecruitInfo> m_lastRecruits = new List<Network.RecruitInfo>();

	// Token: 0x02000589 RID: 1417
	// (Invoke) Token: 0x06004063 RID: 16483
	public delegate void RecruitsChangedCallback();

	// Token: 0x0200058A RID: 1418
	// (Invoke) Token: 0x06004067 RID: 16487
	public delegate void RecruitAcceptedCallback(Network.RecruitInfo recruit);

	// Token: 0x02000633 RID: 1587
	public enum RecruitStatus
	{
		// Token: 0x04002BEF RID: 11247
		NOTHING,
		// Token: 0x04002BF0 RID: 11248
		PENDING,
		// Token: 0x04002BF1 RID: 11249
		INELIGIBLE,
		// Token: 0x04002BF2 RID: 11250
		FAILED,
		// Token: 0x04002BF3 RID: 11251
		ACCEPTED
	}

	// Token: 0x02000636 RID: 1590
	private class RecruitsChangedListener : EventListener<RecruitListMgr.RecruitsChangedCallback>
	{
		// Token: 0x0600451A RID: 17690 RVA: 0x0014BD81 File Offset: 0x00149F81
		public void Fire()
		{
			this.m_callback();
		}
	}

	// Token: 0x02000637 RID: 1591
	private class RecruitAcceptedListener : EventListener<RecruitListMgr.RecruitAcceptedCallback>
	{
		// Token: 0x0600451C RID: 17692 RVA: 0x0014BD96 File Offset: 0x00149F96
		public void Fire(Network.RecruitInfo info)
		{
			this.m_callback(info);
		}
	}
}
