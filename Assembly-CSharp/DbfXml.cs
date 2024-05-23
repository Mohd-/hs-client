using System;
using System.Xml;
using UnityEngine;

// Token: 0x02000132 RID: 306
public class DbfXml
{
	// Token: 0x06000F87 RID: 3975 RVA: 0x00043AD8 File Offset: 0x00041CD8
	public static bool Load(string xmlFile, IDbf dbf)
	{
		using (XmlReader xmlReader = XmlReader.Create(xmlFile))
		{
			while (xmlReader.Read())
			{
				if (xmlReader.NodeType == 1)
				{
					if (xmlReader.Name == "Record")
					{
						DbfXml.LoadRecord(xmlReader.ReadSubtree(), dbf, false);
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06000F88 RID: 3976 RVA: 0x00043B54 File Offset: 0x00041D54
	public static void LoadRecord(XmlReader reader, IDbf dbf, bool hideDbfLocDebugInfo = false)
	{
		DbfRecord dbfRecord = dbf.CreateNewRecord();
		while (reader.Read())
		{
			if (reader.NodeType == 1)
			{
				if (!(reader.Name != "Field"))
				{
					if (!reader.IsEmptyElement)
					{
						string text = reader["column"];
						Type varType = dbfRecord.GetVarType(text);
						if (varType != null)
						{
							if (varType == typeof(DbfLocValue))
							{
								dbfRecord.SetVar(text, DbfXml.LoadLocalizedString(reader["loc_ID"], reader.ReadSubtree(), hideDbfLocDebugInfo));
							}
							else if (varType == typeof(bool))
							{
								string strVal = reader.ReadElementContentAsString();
								dbfRecord.SetVar(text, GeneralUtils.ForceBool(strVal));
							}
							else if (varType == typeof(ulong))
							{
								dbfRecord.SetVar(text, ulong.Parse(reader.ReadElementContentAsString()));
							}
							else
							{
								dbfRecord.SetVar(text, reader.ReadElementContentAs(varType, null));
							}
						}
						else
						{
							Debug.LogErrorFormat("Type is not defined for column {0}, dbf={1}. Try \"Build->Generate DBFs and Code\"", new object[]
							{
								text,
								dbfRecord.GetType().Name
							});
						}
					}
				}
			}
		}
		dbf.AddRecord(dbfRecord);
	}

	// Token: 0x06000F89 RID: 3977 RVA: 0x00043C98 File Offset: 0x00041E98
	public static DbfLocValue LoadLocalizedString(string locIdStr, XmlReader reader, bool hideDebugInfo = false)
	{
		reader.Read();
		DbfLocValue dbfLocValue = new DbfLocValue(hideDebugInfo);
		if (!string.IsNullOrEmpty(locIdStr))
		{
			int locId = 0;
			if (int.TryParse(locIdStr, ref locId))
			{
				dbfLocValue.SetLocId(locId);
			}
		}
		while (reader.Read())
		{
			if (reader.NodeType == 1)
			{
				string name = reader.Name;
				string text = reader.ReadElementContentAsString();
				Locale @enum;
				try
				{
					@enum = EnumUtils.GetEnum<Locale>(name);
				}
				catch (ArgumentException)
				{
					continue;
				}
				dbfLocValue.SetString(@enum, TextUtils.DecodeWhitespaces(text));
			}
		}
		return dbfLocValue;
	}
}
