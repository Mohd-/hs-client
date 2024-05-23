using System;

// Token: 0x0200012B RID: 299
[AttributeUsage(128)]
public class DbfFieldAttribute : Attribute
{
	// Token: 0x06000F74 RID: 3956 RVA: 0x00043A43 File Offset: 0x00041C43
	public DbfFieldAttribute(string varName, string comment)
	{
		this.m_varName = varName;
		this.m_comment = comment;
	}

	// Token: 0x0400083B RID: 2107
	public string m_varName;

	// Token: 0x0400083C RID: 2108
	public string m_comment;
}
