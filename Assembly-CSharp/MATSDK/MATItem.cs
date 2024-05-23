using System;

namespace MATSDK
{
	// Token: 0x02000B30 RID: 2864
	public struct MATItem
	{
		// Token: 0x060061FD RID: 25085 RVA: 0x001D2400 File Offset: 0x001D0600
		public MATItem(string name)
		{
			this.name = name;
			this.unitPrice = default(double?);
			this.quantity = default(int?);
			this.revenue = default(double?);
			this.attribute1 = null;
			this.attribute2 = null;
			this.attribute3 = null;
			this.attribute4 = null;
			this.attribute5 = null;
		}

		// Token: 0x040048FE RID: 18686
		public string name;

		// Token: 0x040048FF RID: 18687
		public double? unitPrice;

		// Token: 0x04004900 RID: 18688
		public int? quantity;

		// Token: 0x04004901 RID: 18689
		public double? revenue;

		// Token: 0x04004902 RID: 18690
		public string attribute1;

		// Token: 0x04004903 RID: 18691
		public string attribute2;

		// Token: 0x04004904 RID: 18692
		public string attribute3;

		// Token: 0x04004905 RID: 18693
		public string attribute4;

		// Token: 0x04004906 RID: 18694
		public string attribute5;
	}
}
