using System;
using System.Collections.Generic;

// Token: 0x020000B2 RID: 178
public class FatalErrorMgr
{
	// Token: 0x0600085B RID: 2139 RVA: 0x000208A4 File Offset: 0x0001EAA4
	public static FatalErrorMgr Get()
	{
		if (FatalErrorMgr.s_instance == null)
		{
			FatalErrorMgr.s_instance = new FatalErrorMgr();
		}
		return FatalErrorMgr.s_instance;
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x000208BF File Offset: 0x0001EABF
	public void Add(FatalErrorMessage message)
	{
		this.m_messages.Add(message);
		this.FireErrorListeners(message);
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x000208D4 File Offset: 0x0001EAD4
	public bool AddUnique(FatalErrorMessage message)
	{
		if (!string.IsNullOrEmpty(message.m_id))
		{
			foreach (FatalErrorMessage fatalErrorMessage in this.m_messages)
			{
				if (fatalErrorMessage.m_id == message.m_id)
				{
					return false;
				}
			}
		}
		this.Add(message);
		return true;
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x00020960 File Offset: 0x0001EB60
	public void SetErrorCode(string prefixSource, string errorSubset1, string errorSubset2 = null, string errorSubset3 = null)
	{
		this.m_generatedErrorCode = prefixSource + ":" + errorSubset1;
		if (errorSubset2 != null)
		{
			this.m_generatedErrorCode = this.m_generatedErrorCode + ":" + errorSubset2;
		}
		if (errorSubset3 != null)
		{
			this.m_generatedErrorCode = this.m_generatedErrorCode + ":" + errorSubset3;
		}
		Log.Yim.Print("m_generatedErrorCode = " + this.m_generatedErrorCode, new object[0]);
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x000209DB File Offset: 0x0001EBDB
	public void ClearAllErrors()
	{
		this.m_messages.Clear();
		this.m_generatedErrorCode = null;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x000209EF File Offset: 0x0001EBEF
	public bool AddErrorListener(FatalErrorMgr.ErrorCallback callback)
	{
		return this.AddErrorListener(callback, null);
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x000209FC File Offset: 0x0001EBFC
	public bool AddErrorListener(FatalErrorMgr.ErrorCallback callback, object userData)
	{
		FatalErrorMgr.ErrorListener errorListener = new FatalErrorMgr.ErrorListener();
		errorListener.SetCallback(callback);
		errorListener.SetUserData(userData);
		if (this.m_errorListeners.Contains(errorListener))
		{
			return false;
		}
		this.m_errorListeners.Add(errorListener);
		return true;
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x00020A3D File Offset: 0x0001EC3D
	public bool RemoveErrorListener(FatalErrorMgr.ErrorCallback callback)
	{
		return this.RemoveErrorListener(callback, null);
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00020A48 File Offset: 0x0001EC48
	public bool RemoveErrorListener(FatalErrorMgr.ErrorCallback callback, object userData)
	{
		FatalErrorMgr.ErrorListener errorListener = new FatalErrorMgr.ErrorListener();
		errorListener.SetCallback(callback);
		errorListener.SetUserData(userData);
		return this.m_errorListeners.Remove(errorListener);
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x00020A75 File Offset: 0x0001EC75
	public List<FatalErrorMessage> GetMessages()
	{
		return this.m_messages;
	}

	// Token: 0x06000865 RID: 2149 RVA: 0x00020A7D File Offset: 0x0001EC7D
	public string GetFormattedErrorCode()
	{
		return this.m_generatedErrorCode;
	}

	// Token: 0x06000866 RID: 2150 RVA: 0x00020A85 File Offset: 0x0001EC85
	public bool HasError()
	{
		return this.m_messages.Count > 0;
	}

	// Token: 0x06000867 RID: 2151 RVA: 0x00020A98 File Offset: 0x0001EC98
	public void NotifyExitPressed()
	{
		Log.Mike.Print("FatalErrorDialog.NotifyExitPressed() - BEGIN", new object[0]);
		this.SendAcknowledgements();
		Log.Mike.Print("FatalErrorDialog.NotifyExitPressed() - calling ApplicationMgr.Get().Exit()", new object[0]);
		ApplicationMgr.Get().Exit();
		Log.Mike.Print("FatalErrorDialog.NotifyExitPressed() - END", new object[0]);
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00020AF4 File Offset: 0x0001ECF4
	private void SendAcknowledgements()
	{
		foreach (FatalErrorMessage fatalErrorMessage in this.m_messages.ToArray())
		{
			if (fatalErrorMessage.m_ackCallback != null)
			{
				fatalErrorMessage.m_ackCallback(fatalErrorMessage.m_ackUserData);
			}
		}
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00020B44 File Offset: 0x0001ED44
	protected void FireErrorListeners(FatalErrorMessage message)
	{
		FatalErrorMgr.ErrorListener[] array = this.m_errorListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(message);
		}
	}

	// Token: 0x04000449 RID: 1097
	private static FatalErrorMgr s_instance;

	// Token: 0x0400044A RID: 1098
	private List<FatalErrorMessage> m_messages = new List<FatalErrorMessage>();

	// Token: 0x0400044B RID: 1099
	private string m_text;

	// Token: 0x0400044C RID: 1100
	private List<FatalErrorMgr.ErrorListener> m_errorListeners = new List<FatalErrorMgr.ErrorListener>();

	// Token: 0x0400044D RID: 1101
	private string m_generatedErrorCode;

	// Token: 0x0200028B RID: 651
	// (Invoke) Token: 0x0600239D RID: 9117
	public delegate void ErrorCallback(FatalErrorMessage message, object userData);

	// Token: 0x020004ED RID: 1261
	protected class ErrorListener : EventListener<FatalErrorMgr.ErrorCallback>
	{
		// Token: 0x06003B45 RID: 15173 RVA: 0x0011F696 File Offset: 0x0011D896
		public void Fire(FatalErrorMessage message)
		{
			if (!GeneralUtils.IsCallbackValid(this.m_callback))
			{
				return;
			}
			this.m_callback(message, this.m_userData);
		}
	}
}
