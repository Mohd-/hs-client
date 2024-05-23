using System;
using PegasusUtil;

// Token: 0x02000410 RID: 1040
public class NoGTAPPTransactionData
{
	// Token: 0x0600350B RID: 13579 RVA: 0x00107310 File Offset: 0x00105510
	public NoGTAPPTransactionData()
	{
		this.Product = 0;
		this.ProductData = 0;
		this.Quantity = 0;
	}

	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x0600350C RID: 13580 RVA: 0x00107338 File Offset: 0x00105538
	// (set) Token: 0x0600350D RID: 13581 RVA: 0x00107340 File Offset: 0x00105540
	public ProductType Product { get; set; }

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x0600350E RID: 13582 RVA: 0x00107349 File Offset: 0x00105549
	// (set) Token: 0x0600350F RID: 13583 RVA: 0x00107351 File Offset: 0x00105551
	public int ProductData { get; set; }

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06003510 RID: 13584 RVA: 0x0010735A File Offset: 0x0010555A
	// (set) Token: 0x06003511 RID: 13585 RVA: 0x00107362 File Offset: 0x00105562
	public int Quantity { get; set; }
}
