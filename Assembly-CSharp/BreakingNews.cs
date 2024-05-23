using System;
using System.Collections;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class BreakingNews : MonoBehaviour
{
	// Token: 0x06000A84 RID: 2692 RVA: 0x0002E9DB File Offset: 0x0002CBDB
	public static BreakingNews Get()
	{
		return BreakingNews.s_instance;
	}

	// Token: 0x06000A85 RID: 2693 RVA: 0x0002E9E2 File Offset: 0x0002CBE2
	public void Awake()
	{
		BreakingNews.SHOWS_BREAKING_NEWS = Network.TUTORIALS_WITHOUT_ACCOUNT;
		BreakingNews.s_instance = this;
	}

	// Token: 0x06000A86 RID: 2694 RVA: 0x0002E9F9 File Offset: 0x0002CBF9
	public void OnDestroy()
	{
		BreakingNews.s_instance = null;
	}

	// Token: 0x06000A87 RID: 2695 RVA: 0x0002EA04 File Offset: 0x0002CC04
	public static void FetchBreakingNews(string url, BreakingNews.BreakingNewsRecievedDelegate callback)
	{
		WWW request = new WWW(url);
		BreakingNews.s_instance.StartCoroutine(BreakingNews.s_instance.FetchBreakingNewsProgress(request, callback));
	}

	// Token: 0x06000A88 RID: 2696 RVA: 0x0002EA30 File Offset: 0x0002CC30
	public IEnumerator FetchBreakingNewsProgress(WWW request, BreakingNews.BreakingNewsRecievedDelegate callback)
	{
		while (request.error == null || !(request.error != string.Empty))
		{
			if (request.isDone)
			{
				callback(request.text, false);
				yield break;
			}
			yield return new WaitForSeconds(0.1f);
		}
		callback(request.error, true);
		yield break;
		yield break;
	}

	// Token: 0x06000A89 RID: 2697 RVA: 0x0002EA60 File Offset: 0x0002CC60
	public BreakingNews.Status GetStatus()
	{
		if (!BreakingNews.SHOWS_BREAKING_NEWS)
		{
			return BreakingNews.Status.Available;
		}
		if (this.m_status == BreakingNews.Status.Fetching && Time.realtimeSinceStartup - this.m_timeFetched > 15f)
		{
			this.m_status = BreakingNews.Status.TimedOut;
		}
		return this.m_status;
	}

	// Token: 0x06000A8A RID: 2698 RVA: 0x0002EAA8 File Offset: 0x0002CCA8
	public void Fetch()
	{
		if (!BreakingNews.SHOWS_BREAKING_NEWS)
		{
			return;
		}
		this.m_error = null;
		this.m_status = BreakingNews.Status.Fetching;
		this.m_text = string.Empty;
		this.m_timeFetched = Time.realtimeSinceStartup;
		BreakingNews.FetchBreakingNews(NydusLink.GetBreakingNewsLink(), delegate(string response, bool error)
		{
			if (error)
			{
				BreakingNews.s_instance.OnBreakingNewsError(response);
			}
			else
			{
				BreakingNews.s_instance.OnBreakingNewsResponse(response);
			}
		});
	}

	// Token: 0x06000A8B RID: 2699 RVA: 0x0002EB0C File Offset: 0x0002CD0C
	public void OnBreakingNewsResponse(string response)
	{
		Log.JMac.Print("Breaking News response received: {0}", new object[]
		{
			response
		});
		this.m_text = response;
		if (this.m_text.Length <= 2 || this.m_text.ToLowerInvariant().Contains("<html>"))
		{
			this.m_text = string.Empty;
		}
		this.m_status = BreakingNews.Status.Available;
	}

	// Token: 0x06000A8C RID: 2700 RVA: 0x0002EB78 File Offset: 0x0002CD78
	public void OnBreakingNewsError(string error)
	{
		this.m_error = error;
		Log.JMac.Print("Breaking News error received: {0}", new object[]
		{
			error
		});
	}

	// Token: 0x06000A8D RID: 2701 RVA: 0x0002EBA8 File Offset: 0x0002CDA8
	public string GetText()
	{
		if (!BreakingNews.SHOWS_BREAKING_NEWS)
		{
			return string.Empty;
		}
		if (this.m_status == BreakingNews.Status.Fetching || this.m_status == BreakingNews.Status.TimedOut)
		{
			Debug.LogError(string.Format("Fetched breaking news when it was unavailable, status={0}", this.m_status));
			return string.Empty;
		}
		return this.m_text;
	}

	// Token: 0x06000A8E RID: 2702 RVA: 0x0002EC02 File Offset: 0x0002CE02
	public string GetError()
	{
		return this.m_error;
	}

	// Token: 0x0400050A RID: 1290
	private const float TIMEOUT = 15f;

	// Token: 0x0400050B RID: 1291
	public static bool SHOWS_BREAKING_NEWS;

	// Token: 0x0400050C RID: 1292
	private static BreakingNews s_instance;

	// Token: 0x0400050D RID: 1293
	private BreakingNews.Status m_status;

	// Token: 0x0400050E RID: 1294
	private string m_text = string.Empty;

	// Token: 0x0400050F RID: 1295
	private string m_error;

	// Token: 0x04000510 RID: 1296
	private float m_timeFetched;

	// Token: 0x020000C2 RID: 194
	public enum Status
	{
		// Token: 0x04000513 RID: 1299
		Fetching,
		// Token: 0x04000514 RID: 1300
		Available,
		// Token: 0x04000515 RID: 1301
		TimedOut
	}

	// Token: 0x020008A3 RID: 2211
	// (Invoke) Token: 0x06005413 RID: 21523
	public delegate void BreakingNewsRecievedDelegate(string response, bool error);
}
