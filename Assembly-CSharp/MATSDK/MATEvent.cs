using System;

namespace MATSDK
{
	// Token: 0x02000B2E RID: 2862
	public struct MATEvent
	{
		// Token: 0x060061F9 RID: 25081 RVA: 0x001D224C File Offset: 0x001D044C
		private MATEvent(int dummy1, int dummy2)
		{
			this.name = null;
			this.id = default(int?);
			this.revenue = default(double?);
			this.currencyCode = null;
			this.advertiserRefId = null;
			this.eventItems = null;
			this.transactionState = default(int?);
			this.receipt = null;
			this.receiptSignature = null;
			this.contentType = null;
			this.contentId = null;
			this.level = default(int?);
			this.quantity = default(int?);
			this.searchString = null;
			this.rating = default(double?);
			this.date1 = default(DateTime?);
			this.date2 = default(DateTime?);
			this.attribute1 = null;
			this.attribute2 = null;
			this.attribute3 = null;
			this.attribute4 = null;
			this.attribute5 = null;
		}

		// Token: 0x060061FA RID: 25082 RVA: 0x001D2337 File Offset: 0x001D0537
		public MATEvent(string name)
		{
			this = new MATEvent(0, 0);
			this.name = name;
		}

		// Token: 0x060061FB RID: 25083 RVA: 0x001D2348 File Offset: 0x001D0548
		public MATEvent(int id)
		{
			this = new MATEvent(0, 0);
			this.id = new int?(id);
		}

		// Token: 0x040048D3 RID: 18643
		public string name;

		// Token: 0x040048D4 RID: 18644
		public int? id;

		// Token: 0x040048D5 RID: 18645
		public double? revenue;

		// Token: 0x040048D6 RID: 18646
		public string currencyCode;

		// Token: 0x040048D7 RID: 18647
		public string advertiserRefId;

		// Token: 0x040048D8 RID: 18648
		public MATItem[] eventItems;

		// Token: 0x040048D9 RID: 18649
		public int? transactionState;

		// Token: 0x040048DA RID: 18650
		public string receipt;

		// Token: 0x040048DB RID: 18651
		public string receiptSignature;

		// Token: 0x040048DC RID: 18652
		public string contentType;

		// Token: 0x040048DD RID: 18653
		public string contentId;

		// Token: 0x040048DE RID: 18654
		public int? level;

		// Token: 0x040048DF RID: 18655
		public int? quantity;

		// Token: 0x040048E0 RID: 18656
		public string searchString;

		// Token: 0x040048E1 RID: 18657
		public double? rating;

		// Token: 0x040048E2 RID: 18658
		public DateTime? date1;

		// Token: 0x040048E3 RID: 18659
		public DateTime? date2;

		// Token: 0x040048E4 RID: 18660
		public string attribute1;

		// Token: 0x040048E5 RID: 18661
		public string attribute2;

		// Token: 0x040048E6 RID: 18662
		public string attribute3;

		// Token: 0x040048E7 RID: 18663
		public string attribute4;

		// Token: 0x040048E8 RID: 18664
		public string attribute5;
	}
}
