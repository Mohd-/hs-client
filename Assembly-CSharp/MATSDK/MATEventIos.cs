using System;

namespace MATSDK
{
	// Token: 0x02000B34 RID: 2868
	internal struct MATEventIos
	{
		// Token: 0x06006209 RID: 25097 RVA: 0x001D27C8 File Offset: 0x001D09C8
		private MATEventIos(int dummy1, int dummy2)
		{
			this.eventId = null;
			this.name = null;
			this.revenue = null;
			this.currencyCode = null;
			this.advertiserRefId = null;
			this.transactionState = null;
			this.contentType = null;
			this.contentId = null;
			this.level = null;
			this.quantity = null;
			this.searchString = null;
			this.rating = null;
			this.date1 = null;
			this.date2 = null;
			this.attribute1 = null;
			this.attribute2 = null;
			this.attribute3 = null;
			this.attribute4 = null;
			this.attribute5 = null;
		}

		// Token: 0x0600620A RID: 25098 RVA: 0x001D285A File Offset: 0x001D0A5A
		public MATEventIos(string name)
		{
			this = new MATEventIos(0, 0);
			this.name = name;
		}

		// Token: 0x0600620B RID: 25099 RVA: 0x001D286B File Offset: 0x001D0A6B
		public MATEventIos(int id)
		{
			this = new MATEventIos(0, 0);
			this.eventId = id.ToString();
		}

		// Token: 0x0600620C RID: 25100 RVA: 0x001D2884 File Offset: 0x001D0A84
		public MATEventIos(MATEvent matEvent)
		{
			this.name = matEvent.name;
			this.eventId = ((matEvent.name != null) ? null : matEvent.id.ToString());
			this.advertiserRefId = matEvent.advertiserRefId;
			this.attribute1 = matEvent.attribute1;
			this.attribute2 = matEvent.attribute2;
			this.attribute3 = matEvent.attribute3;
			this.attribute4 = matEvent.attribute4;
			this.attribute5 = matEvent.attribute5;
			this.contentId = ((matEvent.contentId != null) ? matEvent.contentId.ToString() : null);
			this.contentType = matEvent.contentType;
			this.currencyCode = matEvent.currencyCode;
			int? num = matEvent.level;
			this.level = ((num != null) ? matEvent.level.ToString() : null);
			int? num2 = matEvent.quantity;
			this.quantity = ((num2 != null) ? matEvent.quantity.ToString() : null);
			double? num3 = matEvent.rating;
			this.rating = ((num3 != null) ? matEvent.rating.ToString() : null);
			double? num4 = matEvent.revenue;
			this.revenue = ((num4 != null) ? matEvent.revenue.ToString() : null);
			this.searchString = matEvent.searchString;
			int? num5 = matEvent.transactionState;
			this.transactionState = ((num5 != null) ? matEvent.transactionState.ToString() : null);
			this.date1 = null;
			this.date2 = null;
			DateTime dateTime;
			dateTime..ctor(1970, 1, 1);
			if (matEvent.date1 != null)
			{
				TimeSpan timeSpan;
				timeSpan..ctor(matEvent.date1.Value.Ticks);
				double totalMilliseconds = timeSpan.TotalMilliseconds;
				double num6 = totalMilliseconds;
				TimeSpan timeSpan2;
				timeSpan2..ctor(dateTime.Ticks);
				this.date1 = (num6 - timeSpan2.TotalMilliseconds).ToString();
			}
			if (matEvent.date2 != null)
			{
				TimeSpan timeSpan3;
				timeSpan3..ctor(matEvent.date2.Value.Ticks);
				double totalMilliseconds2 = timeSpan3.TotalMilliseconds;
				double num7 = totalMilliseconds2;
				TimeSpan timeSpan4;
				timeSpan4..ctor(dateTime.Ticks);
				this.date2 = (num7 - timeSpan4.TotalMilliseconds).ToString();
			}
		}

		// Token: 0x04004907 RID: 18695
		public string name;

		// Token: 0x04004908 RID: 18696
		public string eventId;

		// Token: 0x04004909 RID: 18697
		public string revenue;

		// Token: 0x0400490A RID: 18698
		public string currencyCode;

		// Token: 0x0400490B RID: 18699
		public string advertiserRefId;

		// Token: 0x0400490C RID: 18700
		public string transactionState;

		// Token: 0x0400490D RID: 18701
		public string contentType;

		// Token: 0x0400490E RID: 18702
		public string contentId;

		// Token: 0x0400490F RID: 18703
		public string level;

		// Token: 0x04004910 RID: 18704
		public string quantity;

		// Token: 0x04004911 RID: 18705
		public string searchString;

		// Token: 0x04004912 RID: 18706
		public string rating;

		// Token: 0x04004913 RID: 18707
		public string date1;

		// Token: 0x04004914 RID: 18708
		public string date2;

		// Token: 0x04004915 RID: 18709
		public string attribute1;

		// Token: 0x04004916 RID: 18710
		public string attribute2;

		// Token: 0x04004917 RID: 18711
		public string attribute3;

		// Token: 0x04004918 RID: 18712
		public string attribute4;

		// Token: 0x04004919 RID: 18713
		public string attribute5;
	}
}
