using System;

// Token: 0x02000008 RID: 8
public class EventListener<Delegate>
{
	// Token: 0x06000047 RID: 71 RVA: 0x00003A42 File Offset: 0x00001C42
	public EventListener()
	{
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003A4A File Offset: 0x00001C4A
	public EventListener(Delegate callback, object userData)
	{
		this.m_callback = callback;
		this.m_userData = userData;
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00003A60 File Offset: 0x00001C60
	public override bool Equals(object obj)
	{
		EventListener<Delegate> eventListener = obj as EventListener<Delegate>;
		if (eventListener == null)
		{
			return base.Equals(obj);
		}
		return this.m_callback.Equals(eventListener.m_callback) && this.m_userData == eventListener.m_userData;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003AB4 File Offset: 0x00001CB4
	public override int GetHashCode()
	{
		int num = 23;
		if (this.m_callback != null)
		{
			num = num * 17 + this.m_callback.GetHashCode();
		}
		if (this.m_userData != null)
		{
			num = num * 17 + this.m_userData.GetHashCode();
		}
		return num;
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00003B08 File Offset: 0x00001D08
	public Delegate GetCallback()
	{
		return this.m_callback;
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00003B10 File Offset: 0x00001D10
	public void SetCallback(Delegate callback)
	{
		this.m_callback = callback;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00003B19 File Offset: 0x00001D19
	public object GetUserData()
	{
		return this.m_userData;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x00003B21 File Offset: 0x00001D21
	public void SetUserData(object userData)
	{
		this.m_userData = userData;
	}

	// Token: 0x0400001A RID: 26
	protected Delegate m_callback;

	// Token: 0x0400001B RID: 27
	protected object m_userData;
}
