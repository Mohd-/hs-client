using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using bgs;
using UnityEngine;
using WTCG.BI;

// Token: 0x020002DA RID: 730
public class UpdateManager : MonoBehaviour
{
	// Token: 0x06002649 RID: 9801 RVA: 0x000BADCB File Offset: 0x000B8FCB
	private void Awake()
	{
		UpdateManager.s_instance = this;
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x000BADD3 File Offset: 0x000B8FD3
	private void OnDestroy()
	{
		UpdateManager.s_instance = null;
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x000BADDB File Offset: 0x000B8FDB
	public static UpdateManager Get()
	{
		return UpdateManager.s_instance;
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x000BADE2 File Offset: 0x000B8FE2
	public void StartInitialize(int version, UpdateManager.InitCallback callback)
	{
		this.m_callback = callback;
		this.m_assetsVersion = version;
		base.StartCoroutine(this.Initialize());
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x000BAE00 File Offset: 0x000B9000
	public IEnumerator Initialize()
	{
		BIReport.Get().Report_DataOnlyPatching(DataOnlyPatching.Status.STARTED, Localization.GetLocale(), 12574, this.m_assetsVersion);
		this.SetDownloadURI();
		this.InitValues();
		for (int tryCount = 0; tryCount < this.GetTryCount(); tryCount++)
		{
			Log.UpdateManager.Print("Download {0}", new object[]
			{
				this.DownloadURI("update.txt")
			});
			WWW manifestFile = new WWW(this.DownloadURI("update.txt"));
			yield return manifestFile;
			this.m_error = manifestFile.error;
			if (!string.IsNullOrEmpty(manifestFile.error))
			{
				this.m_reportStatus = DataOnlyPatching.Status.FAILED_DOWNLOADING;
			}
			else
			{
				this.m_reportStatus = DataOnlyPatching.Status.SUCCEED_WITH_CACHE;
				List<UpdateManager.UpdateItem> list = new List<UpdateManager.UpdateItem>();
				this.LoadManifest(manifestFile.bytes, ref list);
				this.m_updateProgress.numFilesToDownload = list.Count;
				if (string.IsNullOrEmpty(this.m_error))
				{
					int num;
					foreach (UpdateManager.UpdateItem item in list)
					{
						string extension = Path.GetExtension(item.fileName);
						string text = extension;
						if (text == null)
						{
							goto IL_25A;
						}
						if (UpdateManager.<>f__switch$map8C == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
							dictionary.Add(".unity3d", 0);
							UpdateManager.<>f__switch$map8C = dictionary;
						}
						if (!UpdateManager.<>f__switch$map8C.TryGetValue(text, ref num))
						{
							goto IL_25A;
						}
						if (num != 0)
						{
							goto IL_25A;
						}
						yield return base.StartCoroutine(this.DownloadToCache(item));
						this.m_updateProgress.numFilesDownloaded++;
						this.m_currentDownload = null;
						IL_278:
						if (!string.IsNullOrEmpty(this.m_error))
						{
							break;
						}
						continue;
						IL_25A:
						Error.AddDevFatal("UpdateManager: {0} is unsupported", new object[]
						{
							extension
						});
						goto IL_278;
					}
					if (this.m_skipLoadingAssetBundle)
					{
						if (this.m_reportStatus == DataOnlyPatching.Status.SUCCEED)
						{
							this.m_reportStatus = DataOnlyPatching.Status.SUCCEED_WITH_TIMEOVER;
						}
						foreach (UpdateManager.UpdateItem item2 in list)
						{
							item2.bytes = null;
						}
						BIReport.Get().Report_DataOnlyPatching(this.m_reportStatus, Localization.GetLocale(), 12574, this.m_assetsVersion);
						yield break;
					}
					if (string.IsNullOrEmpty(this.m_error))
					{
						AssetLoader.Get().UnloadUpdatableBundles();
						foreach (UpdateManager.UpdateItem item3 in list)
						{
							string extension2 = Path.GetExtension(item3.fileName);
							string text = extension2;
							if (text != null)
							{
								if (UpdateManager.<>f__switch$map8D == null)
								{
									Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
									dictionary.Add(".unity3d", 0);
									UpdateManager.<>f__switch$map8D = dictionary;
								}
								if (UpdateManager.<>f__switch$map8D.TryGetValue(text, ref num))
								{
									if (num == 0)
									{
										yield return base.StartCoroutine(this.LoadFromCache(item3));
									}
								}
							}
							if (!string.IsNullOrEmpty(this.m_error))
							{
								break;
							}
						}
						AssetLoader.Get().ReloadUpdatableBundles();
					}
					break;
				}
			}
			if (!string.IsNullOrEmpty(this.m_error))
			{
				this.MoveToNextCDN();
			}
		}
		bool success = string.IsNullOrEmpty(this.m_error);
		Log.UpdateManager.Print("Set LastFailedDOPVersion: {0}", new object[]
		{
			(!success) ? this.m_assetsVersion : -1
		});
		Options.Get().SetInt(Option.LAST_FAILED_DOP_VERSION, (!success) ? this.m_assetsVersion : -1);
		BIReport.Get().Report_DataOnlyPatching(this.m_reportStatus, Localization.GetLocale(), 12574, this.m_assetsVersion);
		this.m_currentDownload = null;
		this.CallCallback();
		yield break;
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x000BAE1B File Offset: 0x000B901B
	public bool ContainsFile(string fileName)
	{
		return this.m_assetBundles.ContainsKey(fileName);
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x000BAE2C File Offset: 0x000B902C
	public AssetBundle GetAssetBundle(string fileName)
	{
		AssetBundle result = null;
		this.m_assetBundles.TryGetValue(fileName, out result);
		return result;
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x000BAE4B File Offset: 0x000B904B
	public string GetError()
	{
		return this.m_error;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x000BAE53 File Offset: 0x000B9053
	public bool UpdateIsRequired()
	{
		return this.m_updateIsRequired;
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x000BAE5C File Offset: 0x000B905C
	public void SetLastFailedDOPVersion(bool success)
	{
		Log.UpdateManager.Print("Set LastFailedDOPVersion: {0}", new object[]
		{
			(!success) ? this.m_assetsVersion : -1
		});
		Options.Get().SetInt(Option.LAST_FAILED_DOP_VERSION, (!success) ? this.m_assetsVersion : -1);
	}

	// Token: 0x06002653 RID: 9811 RVA: 0x000BAEB6 File Offset: 0x000B90B6
	public bool StopWaitingForUpdate()
	{
		if (this.m_updateIsRequired)
		{
			Debug.LogWarning("Cannot stop waiting for update, update is required!");
			return false;
		}
		this.m_skipLoadingAssetBundle = true;
		this.CallCallback();
		return true;
	}

	// Token: 0x06002654 RID: 9812 RVA: 0x000BAEDD File Offset: 0x000B90DD
	private bool CurrentRegionIsCN()
	{
		return BattleNet.GetCurrentRegion() == 5;
	}

	// Token: 0x06002655 RID: 9813 RVA: 0x000BAEE7 File Offset: 0x000B90E7
	private int GetTryCount()
	{
		return this.m_remoteUri.Length;
	}

	// Token: 0x06002656 RID: 9814 RVA: 0x000BAEF4 File Offset: 0x000B90F4
	private string DownloadURI(string fileName)
	{
		string text = this.m_remoteUri[this.m_remoteUriIndex];
		text += "win/";
		text += string.Format("{0}/", this.m_assetsVersion);
		return string.Format("{0}{1}", text, fileName);
	}

	// Token: 0x06002657 RID: 9815 RVA: 0x000BAF43 File Offset: 0x000B9143
	private void MoveToNextCDN()
	{
		this.m_remoteUriIndex = (this.m_remoteUriIndex + 1) % this.GetTryCount();
		Options.Get().SetInt(Option.PREFERRED_CDN_INDEX, this.m_remoteUriIndex);
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000BAF6C File Offset: 0x000B916C
	private string CachedFileFolder()
	{
		return string.Format("Updates/{0}", this.m_assetsVersion);
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000BAF83 File Offset: 0x000B9183
	private string CachedFilePath(string fileName)
	{
		return string.Format("{0}/{1}", this.CachedFileFolder(), fileName);
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000BAF98 File Offset: 0x000B9198
	private bool CalculateAndCompareMD5(byte[] buf, string md5)
	{
		MD5CryptoServiceProvider md5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = md5CryptoServiceProvider.ComputeHash(buf);
		string text = BitConverter.ToString(array).Replace("-", string.Empty);
		Log.UpdateManager.Print("md5 expected {0} current {1}", new object[]
		{
			md5,
			text
		});
		return md5 == "0" || text == md5;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000BB000 File Offset: 0x000B9200
	private void SetDownloadURI()
	{
		if (ApplicationMgr.IsPublic())
		{
			if (this.CurrentRegionIsCN())
			{
				this.m_remoteUri = new string[]
				{
					"http://client01.pdl.battlenet.com.cn/hs-pod/update/",
					"http://client02.pdl.battlenet.com.cn/hs-pod/update/",
					"http://client03.pdl.battlenet.com.cn/hs-pod/update/",
					"http://dist.blizzard.com/hs-pod/update/"
				};
			}
			else
			{
				this.m_remoteUri = new string[]
				{
					"http://dist.blizzard.com/hs-pod/update/",
					"http://llnw.blizzard.com/hs-pod/update/"
				};
			}
		}
		else
		{
			this.m_remoteUri = Vars.Key("Application.CDN").GetStr("http://streaming-t5.corp.blizzard.net/hearthstone/update/").Split(new char[]
			{
				';'
			});
		}
		foreach (string text in this.m_remoteUri)
		{
			Log.UpdateManager.Print("CDN: {0}", new object[]
			{
				text
			});
		}
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000BB0D8 File Offset: 0x000B92D8
	private void LoadManifest(byte[] buf, ref List<UpdateManager.UpdateItem> list)
	{
		try
		{
			using (StreamReader streamReader = new StreamReader(new MemoryStream(buf)))
			{
				string text;
				while ((text = streamReader.ReadLine()) != null)
				{
					if (!string.IsNullOrEmpty(text))
					{
						string[] array = text.Split(new char[]
						{
							';'
						});
						if (array.Length != 3)
						{
							Log.UpdateManager.Print("Bad update manifest", new object[0]);
							this.m_error = "Bad update manifest";
							this.m_reportStatus = DataOnlyPatching.Status.FAILED_BAD_DATA;
							return;
						}
						if (array[0].ToLower() == "flag")
						{
							string text2 = array[1].ToLower();
							if (text2 != null)
							{
								if (UpdateManager.<>f__switch$map8E == null)
								{
									Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
									dictionary.Add("required", 0);
									dictionary.Add("skip", 1);
									UpdateManager.<>f__switch$map8E = dictionary;
								}
								int num;
								if (UpdateManager.<>f__switch$map8E.TryGetValue(text2, ref num))
								{
									if (num == 0)
									{
										this.m_updateIsRequired = (array[2].ToLower() == "true");
										continue;
									}
									if (num == 1)
									{
										this.m_updateProgress.maxPatchingBarDisplayTime = (float)Convert.ToInt32(array[2]);
										continue;
									}
								}
							}
							Error.AddDevFatal("UpdateManager: {0} is unsupported", new object[]
							{
								array[1]
							});
						}
						else
						{
							list.Add(new UpdateManager.UpdateItem
							{
								fileName = array[0],
								size = Convert.ToInt32(array[1]),
								md5 = array[2]
							});
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			this.m_error = string.Format("LoadManifest Error: {0}", ex.Message);
			this.m_reportStatus = DataOnlyPatching.Status.FAILED_BAD_DATA;
		}
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000BB2D4 File Offset: 0x000B94D4
	private IEnumerator DownloadToCache(UpdateManager.UpdateItem item)
	{
		bool needToDownload = true;
		string filePath = this.CachedFilePath(item.fileName);
		if (File.Exists(filePath))
		{
			try
			{
				byte[] buf = null;
				FileStream fileRead = new FileStream(filePath, 3, 1);
				Log.UpdateManager.Print("Check cached {0}", new object[]
				{
					filePath
				});
				buf = new byte[fileRead.Length];
				fileRead.Read(buf, 0, Convert.ToInt32(fileRead.Length));
				fileRead.Close();
				item.bytes = buf;
				needToDownload = !this.CalculateAndCompareMD5(buf, item.md5);
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				Log.UpdateManager.Print("FileStream Error: {0}", new object[]
				{
					ex.Message
				});
				needToDownload = true;
			}
		}
		if (needToDownload)
		{
			this.m_reportStatus = DataOnlyPatching.Status.SUCCEED;
			int lastFailedDopVersion = Options.Get().GetInt(Option.LAST_FAILED_DOP_VERSION, -1);
			Log.UpdateManager.Print("LastFailedDOPVerion: {0}", new object[]
			{
				lastFailedDopVersion
			});
			if (lastFailedDopVersion == this.m_assetsVersion)
			{
				this.StopWaitingForUpdate();
			}
			else
			{
				this.m_updateProgress.showProgressBar = true;
			}
			for (int tryCount = 0; tryCount < this.GetTryCount(); tryCount++)
			{
				Log.UpdateManager.Print("Download {0}", new object[]
				{
					this.DownloadURI(item.fileName)
				});
				WWW file = new WWW(this.DownloadURI(item.fileName));
				this.m_currentDownload = file;
				yield return file;
				this.m_error = file.error;
				if (!file.isDone || !string.IsNullOrEmpty(file.error))
				{
					this.m_reportStatus = DataOnlyPatching.Status.FAILED_DOWNLOADING;
				}
				else
				{
					this.m_reportStatus = DataOnlyPatching.Status.SUCCEED;
					Log.UpdateManager.Print("Finished downloading {0}", new object[]
					{
						item.fileName
					});
					Directory.CreateDirectory(this.CachedFileFolder());
					FileStream fileWrite = new FileStream(filePath, 2, 2);
					fileWrite.Write(file.bytes, 0, file.size);
					fileWrite.Close();
					item.bytes = file.bytes;
					if (this.CalculateAndCompareMD5(file.bytes, item.md5))
					{
						break;
					}
					this.m_error = string.Format("MD5 mismatch {0} {1}", item.fileName, item.md5);
					this.m_reportStatus = DataOnlyPatching.Status.FAILED_MD5_MISMATCH;
				}
				if (!string.IsNullOrEmpty(this.m_error))
				{
					this.MoveToNextCDN();
				}
			}
		}
		yield break;
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000BB300 File Offset: 0x000B9500
	private IEnumerator LoadFromCache(UpdateManager.UpdateItem item)
	{
		if (item.bytes == null)
		{
			this.m_error = "No asset bundle in memory";
			this.m_reportStatus = DataOnlyPatching.Status.FAILED_BAD_DATA;
			yield break;
		}
		AssetBundleCreateRequest request = AssetBundle.CreateFromMemory(item.bytes);
		yield return request;
		if (request == null)
		{
			this.m_error = "Bad Asset Bundle";
			this.m_reportStatus = DataOnlyPatching.Status.FAILED_BAD_ASSETBUNDLE;
			yield break;
		}
		if (item.fileName.IndexOf(".xml.unity3d") != -1)
		{
			string xmlName = Path.GetFileNameWithoutExtension(item.fileName);
			TextAsset xml = (TextAsset)request.assetBundle.LoadAsset(xmlName);
			Log.UpdateManager.Print("Dbf.Reload - Use new {0}", new object[]
			{
				xmlName
			});
			string name = Path.GetFileNameWithoutExtension(xmlName);
			GameDbf.Reload(name, xml.text);
			request.assetBundle.Unload(true);
		}
		else
		{
			this.m_assetBundles.Add(item.fileName, request.assetBundle);
		}
		yield break;
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000BB329 File Offset: 0x000B9529
	private void CallCallback()
	{
		if (this.m_callback != null)
		{
			this.m_callback();
			this.m_callback = null;
		}
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000BB348 File Offset: 0x000B9548
	public UpdateManager.UpdateProgress GetProgressForCurrentFile()
	{
		this.m_updateProgress.downloadPercentage = 0f;
		if (this.m_currentDownload != null)
		{
			this.m_updateProgress.downloadPercentage = this.m_currentDownload.progress;
		}
		return this.m_updateProgress;
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000BB384 File Offset: 0x000B9584
	private void InitValues()
	{
		this.m_assetBundles.Clear();
		this.m_updateProgress = new UpdateManager.UpdateProgress();
		this.m_currentDownload = null;
		this.m_error = null;
		this.m_updateIsRequired = false;
		this.m_skipLoadingAssetBundle = false;
		this.m_remoteUriIndex = Options.Get().GetInt(Option.PREFERRED_CDN_INDEX, 0) % this.m_remoteUri.Length;
		this.m_reportStatus = DataOnlyPatching.Status.SUCCEED_WITH_CACHE;
	}

	// Token: 0x040016DE RID: 5854
	private Map<string, AssetBundle> m_assetBundles = new Map<string, AssetBundle>();

	// Token: 0x040016DF RID: 5855
	private string m_error;

	// Token: 0x040016E0 RID: 5856
	private string[] m_remoteUri;

	// Token: 0x040016E1 RID: 5857
	private int m_remoteUriIndex;

	// Token: 0x040016E2 RID: 5858
	private int m_assetsVersion;

	// Token: 0x040016E3 RID: 5859
	private UpdateManager.UpdateProgress m_updateProgress;

	// Token: 0x040016E4 RID: 5860
	private WWW m_currentDownload;

	// Token: 0x040016E5 RID: 5861
	private bool m_updateIsRequired;

	// Token: 0x040016E6 RID: 5862
	private bool m_skipLoadingAssetBundle;

	// Token: 0x040016E7 RID: 5863
	private UpdateManager.InitCallback m_callback;

	// Token: 0x040016E8 RID: 5864
	private DataOnlyPatching.Status m_reportStatus;

	// Token: 0x040016E9 RID: 5865
	private static UpdateManager s_instance;

	// Token: 0x0200069C RID: 1692
	// (Invoke) Token: 0x0600475A RID: 18266
	public delegate void InitCallback();

	// Token: 0x020008A1 RID: 2209
	public class UpdateProgress
	{
		// Token: 0x04003A21 RID: 14881
		public int numFilesToDownload = 4;

		// Token: 0x04003A22 RID: 14882
		public int numFilesDownloaded;

		// Token: 0x04003A23 RID: 14883
		public float downloadPercentage;

		// Token: 0x04003A24 RID: 14884
		public bool showProgressBar;

		// Token: 0x04003A25 RID: 14885
		public float maxPatchingBarDisplayTime = 20f;
	}

	// Token: 0x02000980 RID: 2432
	private class UpdateItem
	{
		// Token: 0x04003F48 RID: 16200
		public string fileName;

		// Token: 0x04003F49 RID: 16201
		public int size;

		// Token: 0x04003F4A RID: 16202
		public string md5;

		// Token: 0x04003F4B RID: 16203
		public byte[] bytes;
	}
}
