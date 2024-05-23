using System;
using System.Collections.Generic;
using bgs;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class UnityUrlDownloader : IUrlDownloader
{
	// Token: 0x06000832 RID: 2098 RVA: 0x000204F8 File Offset: 0x0001E6F8
	public void Process()
	{
		foreach (UnityUrlDownloader.DownloadState downloadState in this.m_downloadsToStart)
		{
			downloadState.startTime = Time.realtimeSinceStartup;
			downloadState.handle = new WWW(downloadState.url);
			this.m_downloadsRunning.Add(downloadState);
		}
		this.m_downloadsToStart.Clear();
		if (this.m_downloadsRunning.Count > 0)
		{
			HashSet<UnityUrlDownloader.DownloadState> hashSet = null;
			foreach (UnityUrlDownloader.DownloadState downloadState2 in this.m_downloadsRunning)
			{
				bool flag = false;
				if (downloadState2.handle.isDone)
				{
					downloadState2.success = string.IsNullOrEmpty(downloadState2.handle.error);
					flag = true;
				}
				else if (downloadState2.timeoutMs >= 0)
				{
					float num = Time.realtimeSinceStartup - downloadState2.startTime;
					if (num > (float)downloadState2.timeoutMs / 1000f)
					{
						downloadState2.success = false;
						flag = true;
					}
				}
				if (flag)
				{
					if (hashSet == null)
					{
						hashSet = new HashSet<UnityUrlDownloader.DownloadState>();
					}
					hashSet.Add(downloadState2);
				}
			}
			if (hashSet != null)
			{
				foreach (UnityUrlDownloader.DownloadState downloadState3 in hashSet)
				{
					this.m_downloadsRunning.Remove(downloadState3);
					this.m_downloadsDone.Add(downloadState3);
				}
			}
			foreach (UnityUrlDownloader.DownloadState downloadState4 in this.m_downloadsDone)
			{
				if (!downloadState4.success && downloadState4.numRetriesLeft > 0)
				{
					downloadState4.numRetriesLeft--;
					this.m_downloadsToStart.Add(downloadState4);
				}
				else if (downloadState4.cb != null)
				{
					downloadState4.cb.Invoke(downloadState4.success, downloadState4.handle.bytes);
				}
			}
			this.m_downloadsDone.Clear();
		}
	}

	// Token: 0x06000833 RID: 2099 RVA: 0x00020774 File Offset: 0x0001E974
	public void Download(string url, UrlDownloadCompletedCallback cb)
	{
		UrlDownloaderConfig config = new UrlDownloaderConfig();
		this.Download(url, cb, config);
	}

	// Token: 0x06000834 RID: 2100 RVA: 0x00020790 File Offset: 0x0001E990
	public void Download(string url, UrlDownloadCompletedCallback cb, UrlDownloaderConfig config)
	{
		UnityUrlDownloader.DownloadState downloadState = new UnityUrlDownloader.DownloadState();
		downloadState.url = url;
		downloadState.timeoutMs = config.timeoutMs;
		downloadState.numRetriesLeft = config.numRetries;
		downloadState.cb = cb;
		this.m_downloadsToStart.Add(downloadState);
	}

	// Token: 0x0400043E RID: 1086
	private HashSet<UnityUrlDownloader.DownloadState> m_downloadsToStart = new HashSet<UnityUrlDownloader.DownloadState>();

	// Token: 0x0400043F RID: 1087
	private HashSet<UnityUrlDownloader.DownloadState> m_downloadsRunning = new HashSet<UnityUrlDownloader.DownloadState>();

	// Token: 0x04000440 RID: 1088
	private HashSet<UnityUrlDownloader.DownloadState> m_downloadsDone = new HashSet<UnityUrlDownloader.DownloadState>();

	// Token: 0x02000A7F RID: 2687
	internal class DownloadState
	{
		// Token: 0x04004554 RID: 17748
		public string url;

		// Token: 0x04004555 RID: 17749
		public int numRetriesLeft;

		// Token: 0x04004556 RID: 17750
		public int timeoutMs = -1;

		// Token: 0x04004557 RID: 17751
		public WWW handle;

		// Token: 0x04004558 RID: 17752
		public bool success;

		// Token: 0x04004559 RID: 17753
		public UrlDownloadCompletedCallback cb;

		// Token: 0x0400455A RID: 17754
		public float startTime;
	}
}
