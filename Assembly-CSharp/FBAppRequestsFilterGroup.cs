using System;
using System.Collections.Generic;

// Token: 0x02000B27 RID: 2855
public sealed class FBAppRequestsFilterGroup : Dictionary<string, object>
{
	// Token: 0x0600618B RID: 24971 RVA: 0x001D1801 File Offset: 0x001CFA01
	public FBAppRequestsFilterGroup(string name, List<string> user_ids)
	{
		this["name"] = name;
		this["user_ids"] = user_ids;
	}
}
