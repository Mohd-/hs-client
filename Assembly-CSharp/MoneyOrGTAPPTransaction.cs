using System;
using PegasusShared;

// Token: 0x02000076 RID: 118
public class MoneyOrGTAPPTransaction
{
	// Token: 0x060005B6 RID: 1462 RVA: 0x000148E0 File Offset: 0x00012AE0
	public MoneyOrGTAPPTransaction(long id, string productID, BattlePayProvider? provider, bool isGTAPP)
	{
		this.ID = id;
		this.ProductID = productID;
		this.IsGTAPP = isGTAPP;
		this.Provider = provider;
		this.ClosedStore = false;
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060005B8 RID: 1464 RVA: 0x00014919 File Offset: 0x00012B19
	// (set) Token: 0x060005B9 RID: 1465 RVA: 0x00014921 File Offset: 0x00012B21
	public long ID { get; private set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060005BA RID: 1466 RVA: 0x0001492A File Offset: 0x00012B2A
	// (set) Token: 0x060005BB RID: 1467 RVA: 0x00014932 File Offset: 0x00012B32
	public string ProductID { get; private set; }

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060005BC RID: 1468 RVA: 0x0001493B File Offset: 0x00012B3B
	// (set) Token: 0x060005BD RID: 1469 RVA: 0x00014943 File Offset: 0x00012B43
	public bool IsGTAPP { get; private set; }

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060005BE RID: 1470 RVA: 0x0001494C File Offset: 0x00012B4C
	// (set) Token: 0x060005BF RID: 1471 RVA: 0x00014954 File Offset: 0x00012B54
	public BattlePayProvider? Provider { get; private set; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060005C0 RID: 1472 RVA: 0x0001495D File Offset: 0x00012B5D
	// (set) Token: 0x060005C1 RID: 1473 RVA: 0x00014965 File Offset: 0x00012B65
	public bool ClosedStore { get; set; }

	// Token: 0x060005C2 RID: 1474 RVA: 0x00014970 File Offset: 0x00012B70
	public override int GetHashCode()
	{
		return this.ID.GetHashCode() * this.ProductID.GetHashCode();
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x00014998 File Offset: 0x00012B98
	public override bool Equals(object obj)
	{
		MoneyOrGTAPPTransaction moneyOrGTAPPTransaction = obj as MoneyOrGTAPPTransaction;
		if (moneyOrGTAPPTransaction == null)
		{
			return false;
		}
		bool flag = this.Provider == null || moneyOrGTAPPTransaction.Provider == null || this.Provider.Value == moneyOrGTAPPTransaction.Provider.Value;
		return moneyOrGTAPPTransaction.ID == this.ID && moneyOrGTAPPTransaction.ProductID == this.ProductID && flag;
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00014A30 File Offset: 0x00012C30
	public override string ToString()
	{
		return string.Format("[MoneyOrGTAPPTransaction: ID={0},ProductID='{1}',IsGTAPP={2},Provider={3}]", new object[]
		{
			this.ID,
			this.ProductID,
			this.IsGTAPP,
			(this.Provider == null) ? "UNKNOWN" : this.Provider.Value.ToString()
		});
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x00014AAC File Offset: 0x00012CAC
	public bool ShouldShowMiniSummary()
	{
		return (StoreManager.HAS_THIRD_PARTY_APP_STORE && ApplicationMgr.GetAndroidStore() != AndroidStore.BLIZZARD) || this.ClosedStore;
	}

	// Token: 0x040002F6 RID: 758
	public static readonly BattlePayProvider? UNKNOWN_PROVIDER;
}
