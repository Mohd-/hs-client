using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020002D6 RID: 726
public class LightFileInfo
{
	// Token: 0x06002634 RID: 9780 RVA: 0x000BA40C File Offset: 0x000B860C
	public LightFileInfo(string path)
	{
		this.FullName = path;
		string[] array = path.Split(FileUtils.FOLDER_SEPARATOR_CHARS);
		this.Name = array[array.Length - 1];
		this.DirectoryName = array[array.Length - 2];
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06002635 RID: 9781 RVA: 0x000BA44C File Offset: 0x000B864C
	public string Path
	{
		get
		{
			return this.FullName;
		}
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x000BA454 File Offset: 0x000B8654
	public static IEnumerable<LightFileInfo> Search(string path, string extension)
	{
		string[] paths = new string[]
		{
			path
		};
		return LightFileInfo.Search(paths, extension);
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x000BA474 File Offset: 0x000B8674
	public static IEnumerable<LightFileInfo> Search(string[] paths, string extension)
	{
		List<LightFileInfo> list = new List<LightFileInfo>();
		foreach (string text in paths)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			LightFileInfo._Search(list, text, extension);
			float realtimeSinceStartup2 = Time.realtimeSinceStartup;
			Log.Cameron.Print("PROFILE Search {0} {1} took {2}s", new object[]
			{
				text,
				extension,
				realtimeSinceStartup2 - realtimeSinceStartup
			});
		}
		return list;
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x000BA4E4 File Offset: 0x000B86E4
	public static void _Search(List<LightFileInfo> matches, string path, string extension)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		foreach (FileInfo fileInfo in directoryInfo.GetFiles(string.Format("*{0}", extension), 1))
		{
			matches.Add(new LightFileInfo(fileInfo.FullName));
		}
	}

	// Token: 0x040016D0 RID: 5840
	public readonly string FullName;

	// Token: 0x040016D1 RID: 5841
	public readonly string Name;

	// Token: 0x040016D2 RID: 5842
	public readonly string DirectoryName;
}
