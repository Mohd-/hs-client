using System;

namespace MATSDK
{
	// Token: 0x02000B35 RID: 2869
	internal struct MATItemIos
	{
		// Token: 0x0600620D RID: 25101 RVA: 0x001D2B1C File Offset: 0x001D0D1C
		public MATItemIos(string name)
		{
			this.name = name;
			this.unitPrice = 0.0;
			this.quantity = 0;
			this.revenue = 0.0;
			this.attribute1 = null;
			this.attribute2 = null;
			this.attribute3 = null;
			this.attribute4 = null;
			this.attribute5 = null;
		}

		// Token: 0x0600620E RID: 25102 RVA: 0x001D2B78 File Offset: 0x001D0D78
		public MATItemIos(MATItem matItem)
		{
			this.name = matItem.name;
			double? num = matItem.unitPrice;
			this.unitPrice = ((num == null) ? 0.0 : num.Value);
			int? num2 = matItem.quantity;
			this.quantity = ((num2 == null) ? 0 : num2.Value);
			double? num3 = matItem.revenue;
			this.revenue = ((num3 == null) ? 0.0 : num3.Value);
			this.attribute1 = matItem.attribute1;
			this.attribute2 = matItem.attribute2;
			this.attribute3 = matItem.attribute3;
			this.attribute4 = matItem.attribute4;
			this.attribute5 = matItem.attribute5;
		}

		// Token: 0x0400491A RID: 18714
		public string name;

		// Token: 0x0400491B RID: 18715
		public double unitPrice;

		// Token: 0x0400491C RID: 18716
		public int quantity;

		// Token: 0x0400491D RID: 18717
		public double revenue;

		// Token: 0x0400491E RID: 18718
		public string attribute1;

		// Token: 0x0400491F RID: 18719
		public string attribute2;

		// Token: 0x04004920 RID: 18720
		public string attribute3;

		// Token: 0x04004921 RID: 18721
		public string attribute4;

		// Token: 0x04004922 RID: 18722
		public string attribute5;
	}
}
