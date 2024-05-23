using System;
using System.Net;
using System.Net.Sockets;
using bgs;
using UnityEngine;

// Token: 0x020005AB RID: 1451
public class ServerConnection<PacketType> where PacketType : PacketFormat, new()
{
	// Token: 0x060040F4 RID: 16628 RVA: 0x00139040 File Offset: 0x00137240
	~ServerConnection()
	{
		this.Disconnect();
	}

	// Token: 0x060040F5 RID: 16629 RVA: 0x00139070 File Offset: 0x00137270
	public IPEndPoint GetLocalEndPoint()
	{
		if (this.m_socket == null)
		{
			return null;
		}
		return this.m_socket.LocalEndPoint as IPEndPoint;
	}

	// Token: 0x060040F6 RID: 16630 RVA: 0x00139090 File Offset: 0x00137290
	public bool Open(int port)
	{
		if (this.m_socket != null)
		{
			return false;
		}
		IPEndPoint ipendPoint = new IPEndPoint(IPAddress.Any, port);
		try
		{
			this.m_socket = new Socket(2, 1, 6);
			this.m_socket.Bind(ipendPoint);
			this.m_socket.Listen(16);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("SeverConnection: error opening inbound connection: " + ex.Message + " (this probably occurred because you have multiple game instances running)");
			this.m_socket = null;
			return false;
		}
		return this.Listen();
	}

	// Token: 0x060040F7 RID: 16631 RVA: 0x00139128 File Offset: 0x00137328
	public void Disconnect()
	{
		if (this.m_socket != null && this.m_socket.Connected)
		{
			this.m_socket.Shutdown(2);
			this.m_socket.Close();
		}
	}

	// Token: 0x060040F8 RID: 16632 RVA: 0x00139168 File Offset: 0x00137368
	public bool Listen()
	{
		object @lock = this.m_lock;
		lock (@lock)
		{
			if (this.m_listening)
			{
				return true;
			}
			this.m_listening = true;
		}
		if (this.m_socket == null)
		{
			return false;
		}
		try
		{
			this.m_socket.BeginAccept(new AsyncCallback(ServerConnection<PacketType>.OnAccept), this);
		}
		catch (Exception ex)
		{
			object lock2 = this.m_lock;
			lock (lock2)
			{
				this.m_listening = false;
			}
			Debug.LogError("error listening for incoming connections: " + ex.Message);
			this.m_socket = null;
			return false;
		}
		return true;
	}

	// Token: 0x060040F9 RID: 16633 RVA: 0x00139248 File Offset: 0x00137448
	private static void OnAccept(IAsyncResult ar)
	{
		ServerConnection<PacketType> serverConnection = (ServerConnection<PacketType>)ar.AsyncState;
		if (serverConnection == null || serverConnection.m_socket == null)
		{
			return;
		}
		try
		{
			Socket socket = serverConnection.m_socket.EndAccept(ar);
			serverConnection.m_currentConnection = new ClientConnection<PacketType>(socket);
		}
		catch (Exception ex)
		{
			Debug.LogError("error accepting connection: " + ex.Message);
		}
		serverConnection.m_listening = false;
	}

	// Token: 0x060040FA RID: 16634 RVA: 0x001392C4 File Offset: 0x001374C4
	public ClientConnection<PacketType> GetNextAcceptedConnection()
	{
		if (this.m_currentConnection != null)
		{
			ClientConnection<PacketType> currentConnection = this.m_currentConnection;
			this.m_currentConnection = null;
			return currentConnection;
		}
		this.Listen();
		return null;
	}

	// Token: 0x04002967 RID: 10599
	private Socket m_socket;

	// Token: 0x04002968 RID: 10600
	private int m_port;

	// Token: 0x04002969 RID: 10601
	private ClientConnection<PacketType> m_currentConnection;

	// Token: 0x0400296A RID: 10602
	private bool m_listening;

	// Token: 0x0400296B RID: 10603
	private object m_lock = new object();
}
