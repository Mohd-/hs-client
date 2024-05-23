using System;
using System.Collections.Generic;
using bgs;

// Token: 0x020004D7 RID: 1239
public class BnetEventMgr
{
	// Token: 0x06003A72 RID: 14962 RVA: 0x0011AC1D File Offset: 0x00118E1D
	public static BnetEventMgr Get()
	{
		if (BnetEventMgr.s_instance == null)
		{
			BnetEventMgr.s_instance = new BnetEventMgr();
			BnetEventMgr.s_instance.Initialize();
		}
		return BnetEventMgr.s_instance;
	}

	// Token: 0x06003A73 RID: 14963 RVA: 0x0011AC42 File Offset: 0x00118E42
	public void Initialize()
	{
		Network.Get().SetBnetStateHandler(new Network.BnetEventHandler(this.OnBnetEventsOccurred));
	}

	// Token: 0x06003A74 RID: 14964 RVA: 0x0011AC5A File Offset: 0x00118E5A
	public void Shutdown()
	{
	}

	// Token: 0x06003A75 RID: 14965 RVA: 0x0011AC5C File Offset: 0x00118E5C
	private void OnBnetEventsOccurred(BattleNet.BnetEvent[] bnetEvents)
	{
		foreach (BattleNet.BnetEvent stateChange in bnetEvents)
		{
			this.FireChangeEvent(stateChange);
		}
	}

	// Token: 0x06003A76 RID: 14966 RVA: 0x0011AC8A File Offset: 0x00118E8A
	public bool AddChangeListener(BnetEventMgr.ChangeCallback callback)
	{
		return this.AddChangeListener(callback, null);
	}

	// Token: 0x06003A77 RID: 14967 RVA: 0x0011AC94 File Offset: 0x00118E94
	public bool AddChangeListener(BnetEventMgr.ChangeCallback callback, object userData)
	{
		BnetEventMgr.ChangeListener changeListener = new BnetEventMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		if (this.m_changeListeners.Contains(changeListener))
		{
			return false;
		}
		this.m_changeListeners.Add(changeListener);
		return true;
	}

	// Token: 0x06003A78 RID: 14968 RVA: 0x0011ACD5 File Offset: 0x00118ED5
	public bool RemoveChangeListener(BnetEventMgr.ChangeCallback callback)
	{
		return this.RemoveChangeListener(callback, null);
	}

	// Token: 0x06003A79 RID: 14969 RVA: 0x0011ACE0 File Offset: 0x00118EE0
	public bool RemoveChangeListener(BnetEventMgr.ChangeCallback callback, object userData)
	{
		BnetEventMgr.ChangeListener changeListener = new BnetEventMgr.ChangeListener();
		changeListener.SetCallback(callback);
		changeListener.SetUserData(userData);
		return this.m_changeListeners.Remove(changeListener);
	}

	// Token: 0x06003A7A RID: 14970 RVA: 0x0011AD10 File Offset: 0x00118F10
	private void FireChangeEvent(BattleNet.BnetEvent stateChange)
	{
		foreach (BnetEventMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
		{
			changeListener.Fire(stateChange);
		}
	}

	// Token: 0x0400254C RID: 9548
	private static BnetEventMgr s_instance;

	// Token: 0x0400254D RID: 9549
	private List<BnetEventMgr.ChangeListener> m_changeListeners = new List<BnetEventMgr.ChangeListener>();

	// Token: 0x020004D8 RID: 1240
	// (Invoke) Token: 0x06003A7C RID: 14972
	public delegate void ChangeCallback(BattleNet.BnetEvent stateChange, object userData);

	// Token: 0x0200055E RID: 1374
	private class ChangeListener : EventListener<BnetEventMgr.ChangeCallback>
	{
		// Token: 0x06003F32 RID: 16178 RVA: 0x00133884 File Offset: 0x00131A84
		public void Fire(BattleNet.BnetEvent stateChange)
		{
			this.m_callback(stateChange, this.m_userData);
		}
	}
}
