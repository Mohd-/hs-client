using System;
using UnityEngine;

// Token: 0x02000023 RID: 35
[Serializable]
public class DbfLocValue
{
	// Token: 0x0600030E RID: 782 RVA: 0x0000EBAA File Offset: 0x0000CDAA
	public DbfLocValue()
	{
	}

	// Token: 0x0600030F RID: 783 RVA: 0x0000EBBD File Offset: 0x0000CDBD
	public DbfLocValue(bool hideDebugInfo)
	{
		this.m_hideDebugInfo = hideDebugInfo;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x0000EBD7 File Offset: 0x0000CDD7
	public string GetString(bool defaultToLoadOrder = true)
	{
		return this.GetString(Localization.GetLocale(), defaultToLoadOrder);
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0000EBE8 File Offset: 0x0000CDE8
	public string GetString(Locale loc, bool defaultToLoadOrder = true)
	{
		string empty = string.Empty;
		if (!this.m_locValues.TryGetValue(loc, out empty) && defaultToLoadOrder)
		{
			Locale[] loadOrder = Localization.GetLoadOrder(false);
			for (int i = 0; i < loadOrder.Length; i++)
			{
				if (this.m_locValues.TryGetValue(loadOrder[i], out empty))
				{
					return empty;
				}
			}
			if (!this.m_hideDebugInfo)
			{
				return string.Format("ID={0} COLUMN={1}", this.m_recordId, this.m_recordColumn);
			}
		}
		return empty;
	}

	// Token: 0x06000312 RID: 786 RVA: 0x0000EC70 File Offset: 0x0000CE70
	public void SetString(Locale loc, string value)
	{
		this.m_locValues[loc] = value;
	}

	// Token: 0x06000313 RID: 787 RVA: 0x0000EC7F File Offset: 0x0000CE7F
	public void SetString(string value)
	{
		this.SetString(Localization.GetLocale(), value);
	}

	// Token: 0x06000314 RID: 788 RVA: 0x0000EC8D File Offset: 0x0000CE8D
	public void SetLocId(int locId)
	{
		this.m_locId = locId;
	}

	// Token: 0x06000315 RID: 789 RVA: 0x0000EC96 File Offset: 0x0000CE96
	public int GetLocId()
	{
		return this.m_locId;
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0000EC9E File Offset: 0x0000CE9E
	public void SetDebugInfo(int recordId, string recordColumn)
	{
		this.m_recordId = recordId;
		this.m_recordColumn = recordColumn;
	}

	// Token: 0x06000317 RID: 791 RVA: 0x0000ECAE File Offset: 0x0000CEAE
	public static implicit operator string(DbfLocValue v)
	{
		return (v == null) ? string.Empty : v.GetString(true);
	}

	// Token: 0x04000143 RID: 323
	[SerializeField]
	private Map<Locale, string> m_locValues = new Map<Locale, string>();

	// Token: 0x04000144 RID: 324
	[SerializeField]
	private int m_locId;

	// Token: 0x04000145 RID: 325
	private int m_recordId;

	// Token: 0x04000146 RID: 326
	private string m_recordColumn;

	// Token: 0x04000147 RID: 327
	private bool m_hideDebugInfo;
}
