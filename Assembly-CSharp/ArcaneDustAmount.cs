using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class ArcaneDustAmount : MonoBehaviour
{
	// Token: 0x06004922 RID: 18722 RVA: 0x0015D757 File Offset: 0x0015B957
	private void Awake()
	{
		ArcaneDustAmount.s_instance = this;
	}

	// Token: 0x06004923 RID: 18723 RVA: 0x0015D75F File Offset: 0x0015B95F
	private void Start()
	{
		this.UpdateCurrentDustAmount();
	}

	// Token: 0x06004924 RID: 18724 RVA: 0x0015D767 File Offset: 0x0015B967
	public static ArcaneDustAmount Get()
	{
		return ArcaneDustAmount.s_instance;
	}

	// Token: 0x06004925 RID: 18725 RVA: 0x0015D770 File Offset: 0x0015B970
	public void UpdateCurrentDustAmount()
	{
		long localArcaneDustBalance = CraftingManager.Get().GetLocalArcaneDustBalance();
		this.m_dustCount.Text = localArcaneDustBalance.ToString();
	}

	// Token: 0x04003036 RID: 12342
	public UberText m_dustCount;

	// Token: 0x04003037 RID: 12343
	public GameObject m_dustJar;

	// Token: 0x04003038 RID: 12344
	public GameObject m_dustFX;

	// Token: 0x04003039 RID: 12345
	public GameObject m_explodeFX_Common;

	// Token: 0x0400303A RID: 12346
	public GameObject m_explodeFX_Rare;

	// Token: 0x0400303B RID: 12347
	public GameObject m_explodeFX_Epic;

	// Token: 0x0400303C RID: 12348
	public GameObject m_explodeFX_Legendary;

	// Token: 0x0400303D RID: 12349
	private static ArcaneDustAmount s_instance;
}
