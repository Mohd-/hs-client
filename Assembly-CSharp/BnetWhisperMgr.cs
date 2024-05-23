using System;
using System.Collections.Generic;
using bgs;

// Token: 0x02000518 RID: 1304
public class BnetWhisperMgr
{
	// Token: 0x06003C87 RID: 15495 RVA: 0x00125440 File Offset: 0x00123640
	public static BnetWhisperMgr Get()
	{
		if (BnetWhisperMgr.s_instance == null)
		{
			BnetWhisperMgr.s_instance = new BnetWhisperMgr();
			ApplicationMgr.Get().WillReset += delegate()
			{
				BnetWhisperMgr.s_instance.m_whispers.Clear();
				BnetWhisperMgr.s_instance.m_whisperMap.Clear();
				BnetWhisperMgr.s_instance.m_firstPendingWhisperIndex = -1;
				BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(BnetWhisperMgr.Get().OnPlayersChanged));
			};
		}
		return BnetWhisperMgr.s_instance;
	}

	// Token: 0x06003C88 RID: 15496 RVA: 0x00125490 File Offset: 0x00123690
	public void Initialize()
	{
		Network.Get().SetWhisperHandler(new Network.WhisperHandler(this.OnWhispers));
		Network.Get().AddBnetErrorListener(6, new Network.BnetErrorCallback(this.OnBnetError));
	}

	// Token: 0x06003C89 RID: 15497 RVA: 0x001254CA File Offset: 0x001236CA
	public void Shutdown()
	{
		Network.Get().RemoveBnetErrorListener(6, new Network.BnetErrorCallback(this.OnBnetError));
		Network.Get().SetWhisperHandler(null);
	}

	// Token: 0x06003C8A RID: 15498 RVA: 0x001254F0 File Offset: 0x001236F0
	public List<BnetWhisper> GetWhispersWithPlayer(BnetPlayer player)
	{
		if (player == null)
		{
			return null;
		}
		List<BnetWhisper> list = new List<BnetWhisper>();
		Map<BnetGameAccountId, BnetGameAccount> gameAccounts = player.GetGameAccounts();
		foreach (BnetGameAccountId key in gameAccounts.Keys)
		{
			List<BnetWhisper> list2;
			if (this.m_whisperMap.TryGetValue(key, out list2))
			{
				list.AddRange(list2);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort(delegate(BnetWhisper a, BnetWhisper b)
		{
			ulong timestampMicrosec = a.GetTimestampMicrosec();
			ulong timestampMicrosec2 = b.GetTimestampMicrosec();
			if (timestampMicrosec < timestampMicrosec2)
			{
				return -1;
			}
			if (timestampMicrosec > timestampMicrosec2)
			{
				return 1;
			}
			return 0;
		});
		return list;
	}

	// Token: 0x06003C8B RID: 15499 RVA: 0x001255AC File Offset: 0x001237AC
	public bool SendWhisper(BnetPlayer player, string message)
	{
		if (player == null)
		{
			return false;
		}
		BnetGameAccount bestGameAccount = player.GetBestGameAccount();
		if (bestGameAccount == null)
		{
			return false;
		}
		Network.SendWhisper(bestGameAccount.GetId(), message);
		return true;
	}

	// Token: 0x06003C8C RID: 15500 RVA: 0x001255E3 File Offset: 0x001237E3
	public bool HavePendingWhispers()
	{
		return this.m_firstPendingWhisperIndex >= 0;
	}

	// Token: 0x06003C8D RID: 15501 RVA: 0x001255F1 File Offset: 0x001237F1
	public bool AddWhisperListener(BnetWhisperMgr.WhisperCallback callback)
	{
		return this.AddWhisperListener(callback, null);
	}

	// Token: 0x06003C8E RID: 15502 RVA: 0x001255FC File Offset: 0x001237FC
	public bool AddWhisperListener(BnetWhisperMgr.WhisperCallback callback, object userData)
	{
		BnetWhisperMgr.WhisperListener whisperListener = new BnetWhisperMgr.WhisperListener();
		whisperListener.SetCallback(callback);
		whisperListener.SetUserData(userData);
		if (this.m_whisperListeners.Contains(whisperListener))
		{
			return false;
		}
		this.m_whisperListeners.Add(whisperListener);
		return true;
	}

	// Token: 0x06003C8F RID: 15503 RVA: 0x0012563D File Offset: 0x0012383D
	public bool RemoveWhisperListener(BnetWhisperMgr.WhisperCallback callback)
	{
		return this.RemoveWhisperListener(callback, null);
	}

	// Token: 0x06003C90 RID: 15504 RVA: 0x00125648 File Offset: 0x00123848
	public bool RemoveWhisperListener(BnetWhisperMgr.WhisperCallback callback, object userData)
	{
		BnetWhisperMgr.WhisperListener whisperListener = new BnetWhisperMgr.WhisperListener();
		whisperListener.SetCallback(callback);
		whisperListener.SetUserData(userData);
		return this.m_whisperListeners.Remove(whisperListener);
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x00125678 File Offset: 0x00123878
	private void OnWhispers(BnetWhisper[] whispers)
	{
		for (int i = 0; i < whispers.Length; i++)
		{
			BnetWhisper bnetWhisper = whispers[i];
			this.m_whispers.Add(bnetWhisper);
			if (!this.HavePendingWhispers())
			{
				if (WhisperUtil.IsDisplayable(bnetWhisper))
				{
					this.ProcessWhisper(this.m_whispers.Count - 1);
				}
				else
				{
					this.m_firstPendingWhisperIndex = i;
					BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
				}
			}
		}
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x001256FC File Offset: 0x001238FC
	private bool OnBnetError(BnetErrorInfo info, object userData)
	{
		Log.Mike.Print("BnetWhisperMgr.OnBnetError() - event={0} error={1}", new object[]
		{
			info.GetFeatureEvent(),
			info.GetError()
		});
		return true;
	}

	// Token: 0x06003C93 RID: 15507 RVA: 0x0012573B File Offset: 0x0012393B
	private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
	{
		if (!this.CanProcessPendingWhispers())
		{
			return;
		}
		BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
		this.ProcessPendingWhispers();
	}

	// Token: 0x06003C94 RID: 15508 RVA: 0x00125768 File Offset: 0x00123968
	private void FireWhisperEvent(BnetWhisper whisper)
	{
		foreach (BnetWhisperMgr.WhisperListener whisperListener in this.m_whisperListeners.ToArray())
		{
			whisperListener.Fire(whisper);
		}
	}

	// Token: 0x06003C95 RID: 15509 RVA: 0x001257A0 File Offset: 0x001239A0
	private bool CanProcessPendingWhispers()
	{
		if (this.m_firstPendingWhisperIndex < 0)
		{
			return true;
		}
		for (int i = this.m_firstPendingWhisperIndex; i < this.m_whispers.Count; i++)
		{
			BnetWhisper whisper = this.m_whispers[i];
			if (!WhisperUtil.IsDisplayable(whisper))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003C96 RID: 15510 RVA: 0x001257F8 File Offset: 0x001239F8
	private void ProcessPendingWhispers()
	{
		if (this.m_firstPendingWhisperIndex < 0)
		{
			return;
		}
		for (int i = this.m_firstPendingWhisperIndex; i < this.m_whispers.Count; i++)
		{
			this.ProcessWhisper(i);
		}
		this.m_firstPendingWhisperIndex = -1;
	}

	// Token: 0x06003C97 RID: 15511 RVA: 0x00125844 File Offset: 0x00123A44
	private void ProcessWhisper(int index)
	{
		BnetWhisper bnetWhisper = this.m_whispers[index];
		BnetGameAccountId theirGameAccountId = WhisperUtil.GetTheirGameAccountId(bnetWhisper);
		if (!BnetUtils.CanReceiveWhisperFrom(theirGameAccountId))
		{
			this.m_whispers.RemoveAt(index);
			return;
		}
		List<BnetWhisper> list;
		if (!this.m_whisperMap.TryGetValue(theirGameAccountId, out list))
		{
			list = new List<BnetWhisper>();
			this.m_whisperMap.Add(theirGameAccountId, list);
		}
		else if (list.Count == 100)
		{
			this.RemoveOldestWhisper(list);
		}
		list.Add(bnetWhisper);
		this.FireWhisperEvent(bnetWhisper);
	}

	// Token: 0x06003C98 RID: 15512 RVA: 0x001258CC File Offset: 0x00123ACC
	private void RemoveOldestWhisper(List<BnetWhisper> whispers)
	{
		BnetWhisper bnetWhisper = whispers[0];
		whispers.RemoveAt(0);
		this.m_whispers.Remove(bnetWhisper);
	}

	// Token: 0x0400269B RID: 9883
	private const int MAX_WHISPERS_PER_PLAYER = 100;

	// Token: 0x0400269C RID: 9884
	private static BnetWhisperMgr s_instance;

	// Token: 0x0400269D RID: 9885
	private List<BnetWhisper> m_whispers = new List<BnetWhisper>();

	// Token: 0x0400269E RID: 9886
	private Map<BnetGameAccountId, List<BnetWhisper>> m_whisperMap = new Map<BnetGameAccountId, List<BnetWhisper>>();

	// Token: 0x0400269F RID: 9887
	private int m_firstPendingWhisperIndex = -1;

	// Token: 0x040026A0 RID: 9888
	private List<BnetWhisperMgr.WhisperListener> m_whisperListeners = new List<BnetWhisperMgr.WhisperListener>();

	// Token: 0x02000519 RID: 1305
	// (Invoke) Token: 0x06003C9C RID: 15516
	public delegate void WhisperCallback(BnetWhisper whisper, object userData);

	// Token: 0x0200059B RID: 1435
	private class WhisperListener : EventListener<BnetWhisperMgr.WhisperCallback>
	{
		// Token: 0x060040A9 RID: 16553 RVA: 0x00137D52 File Offset: 0x00135F52
		public void Fire(BnetWhisper whisper)
		{
			this.m_callback(whisper, this.m_userData);
		}
	}
}
