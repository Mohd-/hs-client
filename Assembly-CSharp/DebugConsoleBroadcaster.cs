using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using UnityEngine;

// Token: 0x02000A76 RID: 2678
public class DebugConsoleBroadcaster
{
	// Token: 0x06005D6B RID: 23915 RVA: 0x001C0680 File Offset: 0x001BE880
	public void Start(int destinationPort, string broadCastResponse)
	{
		this.m_Socket = new Socket(2, 2, 17);
		this.m_Socket.EnableBroadcast = true;
		ASCIIEncoding asciiencoding = new ASCIIEncoding();
		this.m_RequestBytes = asciiencoding.GetBytes(broadCastResponse);
		this.m_RemoteEndPoint = new IPEndPoint(IPAddress.Broadcast, destinationPort);
		this.m_Timer = new Timer(DebugConsoleBroadcaster.Interval.TotalMilliseconds);
		this.m_Timer.Elapsed += new ElapsedEventHandler(this.OnTimerTick);
		this.m_Timer.Start();
		this.m_started = true;
	}

	// Token: 0x06005D6C RID: 23916 RVA: 0x001C0710 File Offset: 0x001BE910
	public void Stop()
	{
		if (!this.m_started)
		{
			return;
		}
		this.m_Timer.Stop();
		this.m_Socket.Close();
	}

	// Token: 0x06005D6D RID: 23917 RVA: 0x001C0740 File Offset: 0x001BE940
	private void OnTimerTick(object sender, ElapsedEventArgs args)
	{
		this.m_Socket.BeginSendTo(this.m_RequestBytes, 0, this.m_RequestBytes.Length, 0, this.m_RemoteEndPoint, new AsyncCallback(this.OnSendTo), this);
	}

	// Token: 0x06005D6E RID: 23918 RVA: 0x001C077C File Offset: 0x001BE97C
	private void OnSendTo(IAsyncResult ar)
	{
		try
		{
			this.m_Socket.EndSendTo(ar);
		}
		catch (Exception ex)
		{
			Debug.LogError("error debug broadcast: " + ex.Message);
		}
	}

	// Token: 0x0400452F RID: 17711
	private Socket m_Socket;

	// Token: 0x04004530 RID: 17712
	private IPEndPoint m_RemoteEndPoint;

	// Token: 0x04004531 RID: 17713
	private byte[] m_RequestBytes;

	// Token: 0x04004532 RID: 17714
	private Timer m_Timer;

	// Token: 0x04004533 RID: 17715
	private bool m_started;

	// Token: 0x04004534 RID: 17716
	private static readonly TimeSpan Interval = new TimeSpan(0, 0, Random.Range(7, 10));
}
