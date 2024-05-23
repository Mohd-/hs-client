using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000298 RID: 664
public class GameStringTable
{
	// Token: 0x06002415 RID: 9237 RVA: 0x000B0DB0 File Offset: 0x000AEFB0
	public bool Load(GameStringCategory cat)
	{
		string filePathWithLoadOrder = GameStringTable.GetFilePathWithLoadOrder(cat, new GameStringTable.FilePathFromCategoryCallback(GameStringTable.GetFilePathFromCategory));
		string filePathWithLoadOrder2 = GameStringTable.GetFilePathWithLoadOrder(cat, new GameStringTable.FilePathFromCategoryCallback(GameStringTable.GetAudioFilePathFromCategory));
		return this.Load(cat, filePathWithLoadOrder, filePathWithLoadOrder2);
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000B0DEC File Offset: 0x000AEFEC
	public bool Load(GameStringCategory cat, Locale locale)
	{
		string filePathFromCategory = GameStringTable.GetFilePathFromCategory(cat, locale);
		string audioFilePathFromCategory = GameStringTable.GetAudioFilePathFromCategory(cat, locale);
		return this.Load(cat, filePathFromCategory, audioFilePathFromCategory);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000B0E14 File Offset: 0x000AF014
	public bool Load(GameStringCategory cat, string path, string audioPath)
	{
		this.m_category = GameStringCategory.INVALID;
		this.m_table.Clear();
		List<GameStringTable.Entry> list = null;
		List<GameStringTable.Entry> list2 = null;
		GameStringTable.Header header;
		if (File.Exists(path) && !GameStringTable.LoadFile(path, out header, out list))
		{
			Error.AddDevWarning("GameStrings Error", "GameStringTable.Load() - Failed to load {0} for cat {1}.", new object[]
			{
				path,
				cat
			});
			return false;
		}
		GameStringTable.Header header2;
		if (File.Exists(audioPath) && !GameStringTable.LoadFile(audioPath, out header2, out list2))
		{
			Error.AddDevWarning("GameStrings Error", "GameStringTable.Load() - Failed to load {0} for cat {1}.", new object[]
			{
				audioPath,
				cat
			});
			return false;
		}
		if (list != null && list2 != null)
		{
			this.BuildTable(path, list, audioPath, list2);
		}
		else if (list != null)
		{
			this.BuildTable(path, list);
		}
		else
		{
			if (list2 == null)
			{
				Error.AddDevWarning("GameStrings Error", "GameStringTable.Load() - There are no entries for cat {0}.", new object[]
				{
					cat
				});
				return false;
			}
			this.BuildTable(audioPath, list2);
		}
		this.m_category = cat;
		return true;
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000B0F1C File Offset: 0x000AF11C
	public string Get(string key)
	{
		string result;
		this.m_table.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000B0F39 File Offset: 0x000AF139
	public Map<string, string> GetAll()
	{
		return this.m_table;
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000B0F41 File Offset: 0x000AF141
	public GameStringCategory GetCategory()
	{
		return this.m_category;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000B0F4C File Offset: 0x000AF14C
	public static bool LoadFile(string path, out GameStringTable.Header header, out List<GameStringTable.Entry> entries)
	{
		header = null;
		entries = null;
		string[] lines = null;
		try
		{
			lines = File.ReadAllLines(path);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(string.Format("GameStringTable.LoadFile() - Failed to read \"{0}\".\n\nException: {1}", path, ex.Message));
			return false;
		}
		header = GameStringTable.LoadFileHeader(lines);
		if (header == null)
		{
			Debug.LogWarning(string.Format("GameStringTable.LoadFile() - \"{0}\" had a malformed header.", path));
			return false;
		}
		entries = GameStringTable.LoadFileEntries(path, header, lines);
		return true;
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000B0FD0 File Offset: 0x000AF1D0
	private static string GetFilePathWithLoadOrder(GameStringCategory cat, GameStringTable.FilePathFromCategoryCallback pathCallback)
	{
		Locale[] loadOrder = Localization.GetLoadOrder(false);
		for (int i = 0; i < loadOrder.Length; i++)
		{
			string text = pathCallback(cat, loadOrder[i]);
			if (File.Exists(text))
			{
				return text;
			}
		}
		return null;
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000B1014 File Offset: 0x000AF214
	private static string GetFilePathFromCategory(GameStringCategory cat, Locale locale)
	{
		string fileName = string.Format("{0}.txt", cat);
		return GameStrings.GetAssetPath(locale, fileName);
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000B103C File Offset: 0x000AF23C
	private static string GetAudioFilePathFromCategory(GameStringCategory cat, Locale locale)
	{
		string fileName = string.Format("{0}_AUDIO.txt", cat);
		return GameStrings.GetAssetPath(locale, fileName);
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000B1064 File Offset: 0x000AF264
	private static GameStringTable.Header LoadFileHeader(string[] lines)
	{
		GameStringTable.Header header = new GameStringTable.Header();
		for (int i = 0; i < lines.Length; i++)
		{
			string text = lines[i];
			if (text.Length != 0)
			{
				if (!text.StartsWith("#"))
				{
					string[] array = text.Split(new char[]
					{
						'\t'
					});
					for (int j = 0; j < array.Length; j++)
					{
						string text2 = array[j];
						if (text2 == "TAG")
						{
							header.m_keyIndex = j;
							if (header.m_valueIndex >= 0)
							{
								break;
							}
						}
						else if (text2 == "TEXT")
						{
							header.m_valueIndex = j;
							if (header.m_keyIndex >= 0)
							{
								break;
							}
						}
					}
					if (header.m_keyIndex < 0 && header.m_valueIndex < 0)
					{
						return null;
					}
					header.m_entryStartIndex = i + 1;
					return header;
				}
			}
		}
		return null;
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000B1164 File Offset: 0x000AF364
	private static List<GameStringTable.Entry> LoadFileEntries(string path, GameStringTable.Header header, string[] lines)
	{
		List<GameStringTable.Entry> list = new List<GameStringTable.Entry>(lines.Length);
		int num = Mathf.Max(header.m_keyIndex, header.m_valueIndex);
		for (int i = header.m_entryStartIndex; i < lines.Length; i++)
		{
			string text = lines[i];
			if (text.Length != 0)
			{
				if (!text.StartsWith("#"))
				{
					string[] array = text.Split(new char[]
					{
						'\t'
					});
					if (array.Length <= num)
					{
						Error.AddDevWarning("GameStrings Error", "GameStringTable.LoadFileEntries() - line {0} in \"{1}\" is malformed", new object[]
						{
							i + 1,
							path
						});
					}
					else
					{
						list.Add(new GameStringTable.Entry
						{
							m_key = array[header.m_keyIndex],
							m_value = TextUtils.DecodeWhitespaces(array[header.m_valueIndex])
						});
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000B124C File Offset: 0x000AF44C
	private void BuildTable(string path, List<GameStringTable.Entry> entries)
	{
		int count = entries.Count;
		this.m_table = new Map<string, string>(count);
		if (count == 0)
		{
			return;
		}
		if (ApplicationMgr.IsInternal())
		{
			GameStringTable.CheckConflicts(path, entries);
		}
		foreach (GameStringTable.Entry entry in entries)
		{
			this.m_table[entry.m_key] = entry.m_value;
		}
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000B12DC File Offset: 0x000AF4DC
	private void BuildTable(string path, List<GameStringTable.Entry> entries, string audioPath, List<GameStringTable.Entry> audioEntries)
	{
		int num = entries.Count + audioEntries.Count;
		this.m_table = new Map<string, string>(num);
		if (num == 0)
		{
			return;
		}
		if (ApplicationMgr.IsInternal())
		{
			GameStringTable.CheckConflicts(path, entries);
		}
		foreach (GameStringTable.Entry entry in entries)
		{
			this.m_table[entry.m_key] = entry.m_value;
		}
		foreach (GameStringTable.Entry entry2 in audioEntries)
		{
			this.m_table[entry2.m_key] = entry2.m_value;
		}
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000B13CC File Offset: 0x000AF5CC
	private static void CheckConflicts(string path, List<GameStringTable.Entry> entries)
	{
		if (entries.Count == 0)
		{
			return;
		}
		for (int i = 0; i < entries.Count; i++)
		{
			string key = entries[i].m_key;
			for (int j = i + 1; j < entries.Count; j++)
			{
				string key2 = entries[j].m_key;
				if (string.Equals(key, key2, 5))
				{
					string message = string.Format("GameStringTable.CheckConflicts() - Tag {0} appears more than once in {1}. All tags must be unique.", key, path);
					Error.AddDevWarning("GameStrings Error", message, new object[0]);
				}
			}
		}
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000B1464 File Offset: 0x000AF664
	private static void CheckConflicts(string path1, List<GameStringTable.Entry> entries1, string path2, List<GameStringTable.Entry> entries2)
	{
		if (entries1.Count == 0)
		{
			return;
		}
		GameStringTable.CheckConflicts(path1, entries1);
		if (entries2.Count == 0)
		{
			return;
		}
		GameStringTable.CheckConflicts(path2, entries2);
		for (int i = 0; i < entries1.Count; i++)
		{
			string key = entries1[i].m_key;
			for (int j = 0; j < entries2.Count; j++)
			{
				string key2 = entries2[j].m_key;
				if (string.Equals(key, key2, 5))
				{
					string message = string.Format("GameStringTable.CheckConflicts() - Tag {0} is used in {1} and {2}. All tags must be unique.", key, path1, path2);
					Error.AddDevWarning("GameStrings Error", message, new object[0]);
				}
			}
		}
	}

	// Token: 0x04001511 RID: 5393
	public const string KEY_FIELD_NAME = "TAG";

	// Token: 0x04001512 RID: 5394
	public const string VALUE_FIELD_NAME = "TEXT";

	// Token: 0x04001513 RID: 5395
	private GameStringCategory m_category;

	// Token: 0x04001514 RID: 5396
	private Map<string, string> m_table = new Map<string, string>();

	// Token: 0x02000EC2 RID: 3778
	public class Entry
	{
		// Token: 0x04005BC2 RID: 23490
		public string m_key;

		// Token: 0x04005BC3 RID: 23491
		public string m_value;
	}

	// Token: 0x02000EC3 RID: 3779
	public class Header
	{
		// Token: 0x04005BC4 RID: 23492
		public int m_entryStartIndex = -1;

		// Token: 0x04005BC5 RID: 23493
		public int m_keyIndex = -1;

		// Token: 0x04005BC6 RID: 23494
		public int m_valueIndex = -1;
	}

	// Token: 0x02000EC4 RID: 3780
	// (Invoke) Token: 0x06007195 RID: 29077
	private delegate string FilePathFromCategoryCallback(GameStringCategory cat, Locale locale);
}
