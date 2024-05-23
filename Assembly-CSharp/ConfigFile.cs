using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

// Token: 0x020001AA RID: 426
public class ConfigFile
{
	// Token: 0x06001C04 RID: 7172 RVA: 0x00083DF2 File Offset: 0x00081FF2
	public string GetPath()
	{
		return this.m_path;
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x00083DFA File Offset: 0x00081FFA
	public bool LightLoad(string path)
	{
		return this.Load(path, true);
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x00083E04 File Offset: 0x00082004
	public bool FullLoad(string path)
	{
		return this.Load(path, false);
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x00083E10 File Offset: 0x00082010
	public bool Save(string path = null)
	{
		if (path == null)
		{
			path = this.m_path;
		}
		if (path == null)
		{
			Debug.LogError("ConfigFile.Save() - no path given");
			return false;
		}
		string text = this.GenerateText();
		try
		{
			FileUtils.SetFileWritableFlag(path, true);
			File.WriteAllText(path, text);
		}
		catch (Exception ex)
		{
			Debug.LogError(string.Format("ConfigFile.Save() - Failed to write file at {0}. Exception={1}", path, ex.Message));
			return false;
		}
		this.m_path = path;
		return true;
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x00083E98 File Offset: 0x00082098
	public bool Has(string key)
	{
		ConfigFile.Line line = this.FindEntry(key);
		return line != null;
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x00083EB4 File Offset: 0x000820B4
	public bool Delete(string key, bool removeEmptySections = true)
	{
		int num = this.FindEntryIndex(key);
		if (num < 0)
		{
			return false;
		}
		this.m_lines.RemoveAt(num);
		if (removeEmptySections)
		{
			int i;
			for (i = num - 1; i >= 0; i--)
			{
				ConfigFile.Line line = this.m_lines[i];
				if (line.m_type == ConfigFile.LineType.SECTION)
				{
					break;
				}
				string text = line.m_raw.Trim();
				if (!string.IsNullOrEmpty(text))
				{
					return true;
				}
			}
			int j;
			for (j = num; j < this.m_lines.Count; j++)
			{
				ConfigFile.Line line2 = this.m_lines[j];
				if (line2.m_type == ConfigFile.LineType.SECTION)
				{
					break;
				}
				string text2 = line2.m_raw.Trim();
				if (!string.IsNullOrEmpty(text2))
				{
					return true;
				}
			}
			int num2 = j - i;
			this.m_lines.RemoveRange(i, num2);
		}
		return true;
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x00083FA1 File Offset: 0x000821A1
	public void Clear()
	{
		this.m_lines.Clear();
	}

	// Token: 0x06001C0B RID: 7179 RVA: 0x00083FB0 File Offset: 0x000821B0
	public string Get(string key, string defaultVal = "")
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return line.m_value;
	}

	// Token: 0x06001C0C RID: 7180 RVA: 0x00083FD4 File Offset: 0x000821D4
	public bool Get(string key, bool defaultVal = false)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceBool(line.m_value);
	}

	// Token: 0x06001C0D RID: 7181 RVA: 0x00083FFC File Offset: 0x000821FC
	public int Get(string key, int defaultVal = 0)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceInt(line.m_value);
	}

	// Token: 0x06001C0E RID: 7182 RVA: 0x00084024 File Offset: 0x00082224
	public float Get(string key, float defaultVal = 0f)
	{
		ConfigFile.Line line = this.FindEntry(key);
		if (line == null)
		{
			return defaultVal;
		}
		return GeneralUtils.ForceFloat(line.m_value);
	}

	// Token: 0x06001C0F RID: 7183 RVA: 0x0008404C File Offset: 0x0008224C
	public bool Set(string key, object val)
	{
		string val2 = (val != null) ? val.ToString() : string.Empty;
		return this.Set(key, val2);
	}

	// Token: 0x06001C10 RID: 7184 RVA: 0x00084078 File Offset: 0x00082278
	public bool Set(string key, bool val)
	{
		string val2 = (!val) ? "false" : "true";
		return this.Set(key, val2);
	}

	// Token: 0x06001C11 RID: 7185 RVA: 0x000840A4 File Offset: 0x000822A4
	public bool Set(string key, string val)
	{
		ConfigFile.Line line = this.RegisterEntry(key);
		if (line == null)
		{
			return false;
		}
		line.m_value = val;
		return true;
	}

	// Token: 0x06001C12 RID: 7186 RVA: 0x000840C9 File Offset: 0x000822C9
	public List<ConfigFile.Line> GetLines()
	{
		return this.m_lines;
	}

	// Token: 0x06001C13 RID: 7187 RVA: 0x000840D4 File Offset: 0x000822D4
	public string GenerateText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line line = this.m_lines[i];
			ConfigFile.LineType type = line.m_type;
			if (type != ConfigFile.LineType.SECTION)
			{
				if (type != ConfigFile.LineType.ENTRY)
				{
					stringBuilder.Append(line.m_raw);
				}
				else if (line.m_quoteValue)
				{
					stringBuilder.AppendFormat("{0} = \"{1}\"", line.m_lineKey, line.m_value);
				}
				else
				{
					stringBuilder.AppendFormat("{0} = {1}", line.m_lineKey, line.m_value);
				}
			}
			else
			{
				stringBuilder.AppendFormat("[{0}]", line.m_sectionName);
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06001C14 RID: 7188 RVA: 0x000841A8 File Offset: 0x000823A8
	private bool Load(string path, bool ignoreUselessLines)
	{
		this.m_path = null;
		this.m_lines.Clear();
		if (!File.Exists(path))
		{
			Debug.LogError("Error loading config file " + path);
			return false;
		}
		int num = 1;
		using (StreamReader streamReader = File.OpenText(path))
		{
			string text = string.Empty;
			while (streamReader.Peek() != -1)
			{
				string text2 = streamReader.ReadLine();
				string text3 = text2.Trim();
				if (!ignoreUselessLines || text3.Length > 0)
				{
					bool flag = text3.Length > 0 && text3.get_Chars(0) == ';';
					if (!ignoreUselessLines || !flag)
					{
						ConfigFile.Line line = new ConfigFile.Line();
						line.m_raw = text2;
						line.m_sectionName = text;
						if (flag)
						{
							line.m_type = ConfigFile.LineType.COMMENT;
						}
						else if (text3.Length > 0)
						{
							if (text3.get_Chars(0) == '[')
							{
								if (text3.Length < 2 || text3.get_Chars(text3.Length - 1) != ']')
								{
									Debug.LogWarning(string.Format("ConfigFile.Load() - invalid section \"{0}\" on line {1} in file {2}", text2, num, path));
									if (!ignoreUselessLines)
									{
										this.m_lines.Add(line);
									}
									continue;
								}
								line.m_type = ConfigFile.LineType.SECTION;
								text = (line.m_sectionName = text3.Substring(1, text3.Length - 2));
								this.m_lines.Add(line);
								continue;
							}
							else
							{
								int num2 = text3.IndexOf('=');
								if (num2 < 0)
								{
									Debug.LogWarning(string.Format("ConfigFile.Load() - invalid entry \"{0}\" on line {1} in file {2}", text2, num, path));
									if (!ignoreUselessLines)
									{
										this.m_lines.Add(line);
									}
									continue;
								}
								string text4 = text3.Substring(0, num2).Trim();
								string text5 = text3.Substring(num2 + 1, text3.Length - num2 - 1).Trim();
								if (text5.Length > 2)
								{
									int num3 = text5.Length - 1;
									if ((text5.get_Chars(0) == '"' || text5.get_Chars(0) == '“' || text5.get_Chars(0) == '”') && (text5.get_Chars(num3) == '"' || text5.get_Chars(num3) == '“' || text5.get_Chars(num3) == '”'))
									{
										text5 = text5.Substring(1, text5.Length - 2);
										line.m_quoteValue = true;
									}
								}
								line.m_type = ConfigFile.LineType.ENTRY;
								line.m_fullKey = ((!string.IsNullOrEmpty(text)) ? string.Format("{0}.{1}", text, text4) : text4);
								line.m_lineKey = text4;
								line.m_value = text5;
							}
						}
						this.m_lines.Add(line);
					}
				}
			}
		}
		this.m_path = path;
		return true;
	}

	// Token: 0x06001C15 RID: 7189 RVA: 0x000844BC File Offset: 0x000826BC
	private int FindSectionIndex(string sectionName)
	{
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line line = this.m_lines[i];
			if (line.m_type == ConfigFile.LineType.SECTION)
			{
				if (line.m_sectionName.Equals(sectionName, 5))
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001C16 RID: 7190 RVA: 0x00084518 File Offset: 0x00082718
	private ConfigFile.Line FindEntry(string fullKey)
	{
		int num = this.FindEntryIndex(fullKey);
		if (num < 0)
		{
			return null;
		}
		return this.m_lines[num];
	}

	// Token: 0x06001C17 RID: 7191 RVA: 0x00084544 File Offset: 0x00082744
	private int FindEntryIndex(string fullKey)
	{
		for (int i = 0; i < this.m_lines.Count; i++)
		{
			ConfigFile.Line line = this.m_lines[i];
			if (line.m_type == ConfigFile.LineType.ENTRY)
			{
				if (line.m_fullKey.Equals(fullKey, 5))
				{
					return i;
				}
			}
		}
		return -1;
	}

	// Token: 0x06001C18 RID: 7192 RVA: 0x000845A0 File Offset: 0x000827A0
	private ConfigFile.Line RegisterEntry(string fullKey)
	{
		if (string.IsNullOrEmpty(fullKey))
		{
			return null;
		}
		int num = fullKey.IndexOf('.');
		if (num < 0)
		{
			return null;
		}
		string sectionName = fullKey.Substring(0, num);
		string text = string.Empty;
		if (fullKey.Length > num + 1)
		{
			text = fullKey.Substring(num + 1, fullKey.Length - num - 1);
		}
		ConfigFile.Line line = null;
		int num2 = this.FindSectionIndex(sectionName);
		if (num2 < 0)
		{
			ConfigFile.Line line2 = new ConfigFile.Line();
			if (this.m_lines.Count > 0)
			{
				line2.m_sectionName = this.m_lines[this.m_lines.Count - 1].m_sectionName;
			}
			this.m_lines.Add(line2);
			ConfigFile.Line line3 = new ConfigFile.Line();
			line3.m_type = ConfigFile.LineType.SECTION;
			line3.m_sectionName = sectionName;
			this.m_lines.Add(line3);
			line = new ConfigFile.Line();
			line.m_type = ConfigFile.LineType.ENTRY;
			line.m_sectionName = sectionName;
			line.m_lineKey = text;
			line.m_fullKey = fullKey;
			this.m_lines.Add(line);
		}
		else
		{
			int i;
			for (i = num2 + 1; i < this.m_lines.Count; i++)
			{
				ConfigFile.Line line4 = this.m_lines[i];
				if (line4.m_type == ConfigFile.LineType.SECTION)
				{
					break;
				}
				if (line4.m_type == ConfigFile.LineType.ENTRY)
				{
					if (line4.m_lineKey.Equals(text, 5))
					{
						line = line4;
						break;
					}
				}
			}
			if (line == null)
			{
				line = new ConfigFile.Line();
				line.m_type = ConfigFile.LineType.ENTRY;
				line.m_sectionName = sectionName;
				line.m_lineKey = text;
				line.m_fullKey = fullKey;
				this.m_lines.Insert(i, line);
			}
		}
		return line;
	}

	// Token: 0x04000EB1 RID: 3761
	private string m_path;

	// Token: 0x04000EB2 RID: 3762
	private List<ConfigFile.Line> m_lines = new List<ConfigFile.Line>();

	// Token: 0x020001AB RID: 427
	public class Line
	{
		// Token: 0x04000EB3 RID: 3763
		public string m_raw = string.Empty;

		// Token: 0x04000EB4 RID: 3764
		public ConfigFile.LineType m_type;

		// Token: 0x04000EB5 RID: 3765
		public string m_sectionName = string.Empty;

		// Token: 0x04000EB6 RID: 3766
		public string m_lineKey = string.Empty;

		// Token: 0x04000EB7 RID: 3767
		public string m_fullKey = string.Empty;

		// Token: 0x04000EB8 RID: 3768
		public string m_value = string.Empty;

		// Token: 0x04000EB9 RID: 3769
		public bool m_quoteValue;
	}

	// Token: 0x02000EDA RID: 3802
	public enum LineType
	{
		// Token: 0x04005C3A RID: 23610
		UNKNOWN,
		// Token: 0x04005C3B RID: 23611
		COMMENT,
		// Token: 0x04005C3C RID: 23612
		SECTION,
		// Token: 0x04005C3D RID: 23613
		ENTRY
	}
}
