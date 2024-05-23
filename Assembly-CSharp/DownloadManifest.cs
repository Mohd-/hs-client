using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020002D9 RID: 729
public class DownloadManifest
{
	// Token: 0x0600263C RID: 9788 RVA: 0x000BA8C3 File Offset: 0x000B8AC3
	private DownloadManifest()
	{
		this.m_fileSet = new HashSet<string>();
		this.m_hashNames = new Map<string, string>();
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x000BA8F2 File Offset: 0x000B8AF2
	public static DownloadManifest Get()
	{
		if (DownloadManifest.s_downloadManifest == null)
		{
			DownloadManifest.s_downloadManifest = new DownloadManifest();
		}
		return DownloadManifest.s_downloadManifest;
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x000BA90D File Offset: 0x000B8B0D
	public bool ContainsFile(string filePath)
	{
		return this.m_fileSet.Contains(filePath);
	}

	// Token: 0x06002640 RID: 9792 RVA: 0x000BA91C File Offset: 0x000B8B1C
	public string HashForBundle(string bundleName)
	{
		string result;
		this.m_hashNames.TryGetValue(bundleName, out result);
		return result;
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x000BA93C File Offset: 0x000B8B3C
	public string DownloadableBundleFileName(string bundleName)
	{
		if (this.HashForBundle(bundleName) == null)
		{
			return null;
		}
		return bundleName;
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x000BA95C File Offset: 0x000B8B5C
	public static void ClearDataToWrite()
	{
		if (DownloadManifest.s_filesToWrite != null)
		{
			DownloadManifest.s_filesToWrite.Clear();
		}
		if (DownloadManifest.s_hashesToWrite != null)
		{
			DownloadManifest.s_hashesToWrite.Clear();
		}
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x000BA994 File Offset: 0x000B8B94
	public static void AddFileToWrite(string filePath)
	{
		if (DownloadManifest.s_filesToWrite == null)
		{
			DownloadManifest.s_filesToWrite = new List<string>();
		}
		List<string> list = DownloadManifest.s_filesToWrite;
		lock (list)
		{
			if (!DownloadManifest.s_filesToWrite.Contains(filePath))
			{
				DownloadManifest.s_filesToWrite.Add(filePath);
			}
		}
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x000BA9F8 File Offset: 0x000B8BF8
	public static void AddHashNameForBundle(string name, string hash)
	{
		if (DownloadManifest.s_hashesToWrite == null)
		{
			DownloadManifest.s_hashesToWrite = new Map<string, string>();
		}
		Map<string, string> map = DownloadManifest.s_hashesToWrite;
		lock (map)
		{
			try
			{
				DownloadManifest.s_hashesToWrite.Add(name, hash);
			}
			catch (ArgumentException ex)
			{
				Debug.LogError(string.Format("Exception adding key {0} with value {1} to hashesToWrite dict. Exception: {2}", name, hash, ex.Message));
			}
		}
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x000BAA7C File Offset: 0x000B8C7C
	public static void WriteToFile(string path)
	{
		if (DownloadManifest.s_filesToWrite == null)
		{
			return;
		}
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		using (StreamWriter streamWriter = new StreamWriter(path))
		{
			if (DownloadManifest.s_hashesToWrite != null)
			{
				Map<string, string> map = DownloadManifest.s_hashesToWrite;
				lock (map)
				{
					foreach (KeyValuePair<string, string> keyValuePair in DownloadManifest.s_hashesToWrite)
					{
						streamWriter.Write(keyValuePair.Value);
						streamWriter.Write(";");
						streamWriter.WriteLine(keyValuePair.Key);
					}
				}
			}
			streamWriter.WriteLine("<END OF HASHES>");
			List<string> list = DownloadManifest.s_filesToWrite;
			lock (list)
			{
				foreach (string text in DownloadManifest.s_filesToWrite)
				{
					streamWriter.WriteLine(text);
				}
			}
		}
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x000BABF4 File Offset: 0x000B8DF4
	private void Load()
	{
		string text = DownloadManifest.s_downloadManifestFilePath;
		int num = 0;
		try
		{
			using (StreamReader streamReader = new StreamReader(text))
			{
				bool flag = true;
				string text2;
				while ((text2 = streamReader.ReadLine()) != null)
				{
					num++;
					if (!string.IsNullOrEmpty(text2))
					{
						if (flag)
						{
							if (text2.Equals("<END OF HASHES>"))
							{
								flag = false;
							}
							else
							{
								this.ParseAndAddHashName(text2);
							}
						}
						else
						{
							this.m_fileSet.Add(text2);
						}
					}
				}
			}
		}
		catch (FileNotFoundException ex)
		{
			string message = string.Format("Failed to find download manifest at '{0}': {1}", text, ex.Message);
			Error.AddDevFatal(message, new object[0]);
		}
		catch (IOException ex2)
		{
			string message2 = string.Format("Failed to read download manifest at '{0}': {1}", text, ex2.Message);
			Error.AddDevFatal(message2, new object[0]);
		}
		catch (NullReferenceException ex3)
		{
			string message3 = string.Format("Failed to read from download manifest '{0}' line {1}: {2}", text, num, ex3.Message);
			Error.AddDevFatal(message3, new object[0]);
		}
		catch (Exception ex4)
		{
			string message4 = string.Format("An unknown error occurred loading download manifest '{0}' line {1}: {2}", text, num, ex4.Message);
			Error.AddDevFatal(message4, new object[0]);
		}
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x000BAD78 File Offset: 0x000B8F78
	private bool ParseAndAddHashName(string line)
	{
		string[] array = line.Split(new char[]
		{
			';'
		});
		if (array.Length != 2)
		{
			return false;
		}
		string value = array[0];
		string key = array[1];
		this.m_hashNames.Add(key, value);
		return true;
	}

	// Token: 0x040016D7 RID: 5847
	private const string MANIFEST_DIVIDER = "<END OF HASHES>";

	// Token: 0x040016D8 RID: 5848
	private static string s_downloadManifestFilePath = FileUtils.GetAssetPath("manifest-downloads.csv");

	// Token: 0x040016D9 RID: 5849
	private static DownloadManifest s_downloadManifest;

	// Token: 0x040016DA RID: 5850
	private Map<string, string> m_hashNames;

	// Token: 0x040016DB RID: 5851
	private HashSet<string> m_fileSet;

	// Token: 0x040016DC RID: 5852
	private static List<string> s_filesToWrite;

	// Token: 0x040016DD RID: 5853
	private static Map<string, string> s_hashesToWrite;
}
