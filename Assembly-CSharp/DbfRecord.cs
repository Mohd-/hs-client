using System;
using System.Reflection;

// Token: 0x02000017 RID: 23
public abstract class DbfRecord
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000269 RID: 617 RVA: 0x0000BFD9 File Offset: 0x0000A1D9
	[DbfField("ID", "")]
	public int ID
	{
		get
		{
			return this.m_ID;
		}
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0000BFE1 File Offset: 0x0000A1E1
	public void SetID(int id)
	{
		this.m_ID = id;
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000BFEC File Offset: 0x0000A1EC
	public DbfFieldAttribute GetDbfFieldAttribute(string propertyName)
	{
		PropertyInfo property = base.GetType().GetProperty(propertyName);
		if (property != null)
		{
			object[] customAttributes = property.GetCustomAttributes(typeof(DbfFieldAttribute), true);
			if (customAttributes.Length > 0)
			{
				return (DbfFieldAttribute)customAttributes[0];
			}
		}
		return null;
	}

	// Token: 0x0600026C RID: 620
	public abstract object GetVar(string varName);

	// Token: 0x0600026D RID: 621
	public abstract void SetVar(string varName, object value);

	// Token: 0x0600026E RID: 622
	public abstract Type GetVarType(string varName);

	// Token: 0x040000BC RID: 188
	private int m_ID;
}
