using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000127 RID: 295
public class LocalOptions
{
	// Token: 0x06000F2B RID: 3883 RVA: 0x0004197B File Offset: 0x0003FB7B
	public static LocalOptions Get()
	{
		if (LocalOptions.s_instance == null)
		{
			LocalOptions.s_instance = new LocalOptions();
		}
		return LocalOptions.s_instance;
	}

	// Token: 0x06000F2C RID: 3884 RVA: 0x00041998 File Offset: 0x0003FB98
	public void Initialize()
	{
		this.m_path = string.Format("{0}/{1}", FileUtils.PersistentDataPath, "options.txt");
		foreach (LocalOptions.LineParser lineParser in this.m_lineParsers)
		{
			lineParser.m_regex = new Regex(lineParser.m_pattern, 1);
		}
		this.m_currentLineVersion = this.m_lineParsers.Length;
		if (this.Load())
		{
			OptionsMigration.UpgradeClientOptions();
		}
	}

	// Token: 0x06000F2D RID: 3885 RVA: 0x00041A0F File Offset: 0x0003FC0F
	public void Clear()
	{
		this.m_dirty = false;
		this.m_options.Clear();
		this.m_sortedKeys.Clear();
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x00041A2E File Offset: 0x0003FC2E
	public bool Has(string key)
	{
		return this.m_options.ContainsKey(key);
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x00041A3C File Offset: 0x0003FC3C
	public void Delete(string key)
	{
		if (!this.m_options.Remove(key))
		{
			return;
		}
		this.m_sortedKeys.Remove(key);
		this.m_dirty = true;
		this.Save(key);
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x00041A78 File Offset: 0x0003FC78
	public T Get<T>(string key)
	{
		object obj;
		if (!this.m_options.TryGetValue(key, out obj))
		{
			return default(T);
		}
		return (T)((object)obj);
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x00041AA8 File Offset: 0x0003FCA8
	public bool GetBool(string key)
	{
		return this.Get<bool>(key);
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x00041AB1 File Offset: 0x0003FCB1
	public int GetInt(string key)
	{
		return this.Get<int>(key);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x00041ABA File Offset: 0x0003FCBA
	public long GetLong(string key)
	{
		return this.Get<long>(key);
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x00041AC3 File Offset: 0x0003FCC3
	public ulong GetULong(string key)
	{
		return this.Get<ulong>(key);
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x00041ACC File Offset: 0x0003FCCC
	public float GetFloat(string key)
	{
		return this.Get<float>(key);
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x00041AD5 File Offset: 0x0003FCD5
	public string GetString(string key)
	{
		return this.Get<string>(key);
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x00041AE0 File Offset: 0x0003FCE0
	public void Set(string key, object val)
	{
		object obj;
		if (this.m_options.TryGetValue(key, out obj))
		{
			if (obj == val)
			{
				return;
			}
			if (obj != null && obj.Equals(val))
			{
				return;
			}
		}
		else
		{
			this.m_sortedKeys.Add(key);
			this.SortKeys();
		}
		this.m_options[key] = val;
		this.m_dirty = true;
		this.Save(key);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x00041B50 File Offset: 0x0003FD50
	private bool Load()
	{
		this.Clear();
		if (!File.Exists(this.m_path))
		{
			this.m_loadResult = LocalOptions.LoadResult.SUCCESS;
			return true;
		}
		string[] lines;
		if (!this.LoadFile(out lines))
		{
			this.m_loadResult = LocalOptions.LoadResult.FAIL;
			return false;
		}
		bool flag = false;
		if (!this.LoadAllLines(lines, out flag))
		{
			this.m_loadResult = LocalOptions.LoadResult.FAIL;
			return false;
		}
		foreach (string text in this.m_options.Keys)
		{
			this.m_sortedKeys.Add(text);
		}
		this.SortKeys();
		this.m_loadResult = LocalOptions.LoadResult.SUCCESS;
		if (flag)
		{
			this.m_dirty = true;
			this.Save();
		}
		return true;
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x00041C24 File Offset: 0x0003FE24
	private bool LoadFile(out string[] lines)
	{
		bool result;
		try
		{
			lines = File.ReadAllLines(this.m_path);
			result = true;
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("LocalOptions.LoadFile() - Failed to read {0}. Exception={1}", this.m_path, ex.Message));
			lines = null;
			result = false;
		}
		return result;
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x00041C88 File Offset: 0x0003FE88
	private bool LoadAllLines(string[] lines, out bool formatChanged)
	{
		formatChanged = false;
		int num = 0;
		for (int i = 0; i < lines.Length; i++)
		{
			string text = lines[i];
			text = text.Trim();
			if (text.Length != 0)
			{
				if (!text.StartsWith("#"))
				{
					int num2;
					string key;
					object value;
					bool flag;
					if (!this.LoadLine(text, out num2, out key, out value, out flag))
					{
						Debug.LogError(string.Format("LocalOptions.LoadAllLines() - Failed to load line {0}.", i + 1));
						num++;
						if (num > 4)
						{
							this.m_loadResult = LocalOptions.LoadResult.FAIL;
							return false;
						}
					}
					else
					{
						this.m_options[key] = value;
						formatChanged = (formatChanged || num2 != this.m_currentLineVersion || flag);
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x00041D4C File Offset: 0x0003FF4C
	private bool LoadLine(string line, out int version, out string key, out object val, out bool formatChanged)
	{
		version = 0;
		key = null;
		val = null;
		formatChanged = false;
		int num = 0;
		string text = null;
		string text2 = null;
		for (int i = 0; i < this.m_lineParsers.Length; i++)
		{
			LocalOptions.LineParser lineParser = this.m_lineParsers[i];
			if (lineParser.m_callback(lineParser.m_regex, line, out text, out text2))
			{
				num = i + 1;
				break;
			}
		}
		if (num == 0)
		{
			return false;
		}
		Option option = Option.INVALID;
		try
		{
			option = EnumUtils.GetEnum<Option>(text, 5);
		}
		catch (ArgumentException)
		{
			version = num;
			key = text;
			val = text2;
			return true;
		}
		bool flag = false;
		int num2;
		Locale locale;
		if (option == Option.LOCALE && GeneralUtils.TryParseInt(text2, out num2) && EnumUtils.TryCast<Locale>(num2, out locale))
		{
			text2 = locale.ToString();
			flag = true;
		}
		Type type = OptionDataTables.s_typeMap[option];
		if (type == typeof(bool))
		{
			val = GeneralUtils.ForceBool(text2);
		}
		else if (type == typeof(int))
		{
			val = GeneralUtils.ForceInt(text2);
		}
		else if (type == typeof(long))
		{
			val = GeneralUtils.ForceLong(text2);
		}
		else if (type == typeof(ulong))
		{
			val = GeneralUtils.ForceULong(text2);
		}
		else if (type == typeof(float))
		{
			val = GeneralUtils.ForceFloat(text2);
		}
		else
		{
			if (type != typeof(string))
			{
				return false;
			}
			val = text2;
		}
		version = num;
		key = text;
		formatChanged = flag;
		return true;
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x00041F20 File Offset: 0x00040120
	private static bool ParseLine_V1(Regex regex, string line, out string key, out string val)
	{
		key = null;
		val = null;
		Match match = regex.Match(line);
		if (!match.Success)
		{
			return false;
		}
		GroupCollection groups = match.Groups;
		key = groups["key"].Value;
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		string value = groups["type"].Value;
		if (string.IsNullOrEmpty(value))
		{
			return false;
		}
		string text = value;
		if (text != null)
		{
			if (LocalOptions.<>f__switch$map8F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
				dictionary.Add("b", 0);
				dictionary.Add("i", 0);
				dictionary.Add("l", 0);
				dictionary.Add("d", 0);
				dictionary.Add("s", 0);
				LocalOptions.<>f__switch$map8F = dictionary;
			}
			int num;
			if (LocalOptions.<>f__switch$map8F.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					val = groups["value"].Value;
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x00042024 File Offset: 0x00040224
	private static bool ParseLine_V2(Regex regex, string line, out string key, out string val)
	{
		key = null;
		val = null;
		Match match = regex.Match(line);
		if (!match.Success)
		{
			return false;
		}
		GroupCollection groups = match.Groups;
		key = groups["key"].Value;
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		val = groups["value"].Value;
		return true;
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x00042088 File Offset: 0x00040288
	private bool Save(string triggerKey)
	{
		switch (this.m_loadResult)
		{
		case LocalOptions.LoadResult.INVALID:
		case LocalOptions.LoadResult.FAIL:
			return false;
		}
		return this.Save();
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x000420BC File Offset: 0x000402BC
	private bool Save()
	{
		if (!this.m_dirty)
		{
			return true;
		}
		List<string> list = new List<string>();
		for (int i = 0; i < this.m_sortedKeys.Count; i++)
		{
			string text = this.m_sortedKeys[i];
			object obj = this.m_options[text];
			string text2 = string.Format("{0}={1}", text, obj);
			list.Add(text2);
		}
		bool result;
		try
		{
			File.WriteAllLines(this.m_path, list.ToArray(), new UTF8Encoding());
			this.m_dirty = false;
			result = true;
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("LocalOptions.Save() - Failed to save {0}. Exception={1}", this.m_path, ex.Message));
			result = false;
		}
		return result;
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x00042190 File Offset: 0x00040390
	private void SortKeys()
	{
		this.m_sortedKeys.Sort(new Comparison<string>(this.KeyComparison));
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x000421A9 File Offset: 0x000403A9
	private int KeyComparison(string key1, string key2)
	{
		return string.Compare(key1, key2, true);
	}

	// Token: 0x04000820 RID: 2080
	private const string OPTIONS_FILE_NAME = "options.txt";

	// Token: 0x04000821 RID: 2081
	private const int LOAD_LINE_FAIL_THRESHOLD = 4;

	// Token: 0x04000822 RID: 2082
	private LocalOptions.LineParser[] m_lineParsers = new LocalOptions.LineParser[]
	{
		new LocalOptions.LineParser
		{
			m_pattern = "(?<key>[^\\:]+):(?<type>[bilds])=(?<value>.*)",
			m_callback = new LocalOptions.ParseLineCallback(LocalOptions.ParseLine_V1)
		},
		new LocalOptions.LineParser
		{
			m_pattern = "(?<key>[^=]+)=(?<value>.*)",
			m_callback = new LocalOptions.ParseLineCallback(LocalOptions.ParseLine_V2)
		}
	};

	// Token: 0x04000823 RID: 2083
	private static LocalOptions s_instance;

	// Token: 0x04000824 RID: 2084
	private string m_path;

	// Token: 0x04000825 RID: 2085
	private LocalOptions.LoadResult m_loadResult;

	// Token: 0x04000826 RID: 2086
	private int m_currentLineVersion;

	// Token: 0x04000827 RID: 2087
	private Map<string, object> m_options = new Map<string, object>();

	// Token: 0x04000828 RID: 2088
	private List<string> m_sortedKeys = new List<string>();

	// Token: 0x04000829 RID: 2089
	private bool m_dirty;

	// Token: 0x02000EAE RID: 3758
	private enum LoadResult
	{
		// Token: 0x04005AD2 RID: 23250
		INVALID,
		// Token: 0x04005AD3 RID: 23251
		SUCCESS,
		// Token: 0x04005AD4 RID: 23252
		FAIL
	}

	// Token: 0x02000EAF RID: 3759
	private class LineParser
	{
		// Token: 0x04005AD5 RID: 23253
		public string m_pattern;

		// Token: 0x04005AD6 RID: 23254
		public Regex m_regex;

		// Token: 0x04005AD7 RID: 23255
		public LocalOptions.ParseLineCallback m_callback;
	}

	// Token: 0x02000EB0 RID: 3760
	// (Invoke) Token: 0x0600713B RID: 28987
	private delegate bool ParseLineCallback(Regex regex, string line, out string key, out string val);
}
