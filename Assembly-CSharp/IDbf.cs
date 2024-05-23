using System;

// Token: 0x0200012C RID: 300
public interface IDbf
{
	// Token: 0x06000F75 RID: 3957
	DbfRecord CreateNewRecord();

	// Token: 0x06000F76 RID: 3958
	void AddRecord(DbfRecord record);

	// Token: 0x06000F77 RID: 3959
	Type GetRecordType();
}
