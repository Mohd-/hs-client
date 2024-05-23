using System;

// Token: 0x020001BE RID: 446
[AttributeUsage(384)]
public class CustomEditField : Attribute
{
	// Token: 0x06001D0B RID: 7435 RVA: 0x000883F4 File Offset: 0x000865F4
	public override string ToString()
	{
		if (this.Sections == null)
		{
			return this.T.ToString();
		}
		return string.Format("Sections={0} T={1}", this.Sections, this.T);
	}

	// Token: 0x04000F2F RID: 3887
	public bool Hide;

	// Token: 0x04000F30 RID: 3888
	public string Sections;

	// Token: 0x04000F31 RID: 3889
	public string Parent;

	// Token: 0x04000F32 RID: 3890
	public string Label;

	// Token: 0x04000F33 RID: 3891
	public string Range;

	// Token: 0x04000F34 RID: 3892
	public bool ListTable;

	// Token: 0x04000F35 RID: 3893
	public int ListSortPriority = -1;

	// Token: 0x04000F36 RID: 3894
	public bool ListSortable;

	// Token: 0x04000F37 RID: 3895
	public bool AllowSceneObject = true;

	// Token: 0x04000F38 RID: 3896
	public EditType T;
}
