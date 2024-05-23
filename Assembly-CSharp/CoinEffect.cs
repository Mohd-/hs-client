using System;
using UnityEngine;

// Token: 0x020008EE RID: 2286
public class CoinEffect : MonoBehaviour
{
	// Token: 0x060055C0 RID: 21952 RVA: 0x0019BC17 File Offset: 0x00199E17
	private void Start()
	{
	}

	// Token: 0x060055C1 RID: 21953 RVA: 0x0019BC1C File Offset: 0x00199E1C
	public void DoAnim(bool localWin)
	{
		if (localWin)
		{
			this.animToUse = this.coinDropAnim2;
			this.GlowanimToUse = this.coinGlowDropAnim2;
		}
		else
		{
			this.animToUse = this.coinDropAnim;
			this.GlowanimToUse = this.coinGlowDropAnim;
		}
		this.coinSpawnObject.SetActive(true);
		this.coin.SetActive(true);
		this.coinGlow.SetActive(true);
		this.coinSpawnObject.GetComponent<Animation>().Stop(this.coinSpawnAnim);
		this.coin.GetComponent<Animation>().Stop(this.animToUse);
		this.coinGlow.GetComponent<Animation>().Stop(this.GlowanimToUse);
		this.coinSpawnObject.GetComponent<Animation>().Play(this.coinSpawnAnim);
		this.coin.GetComponent<Animation>().Play(this.animToUse);
		this.coinGlow.GetComponent<Animation>().Play(this.GlowanimToUse);
	}

	// Token: 0x04003C13 RID: 15379
	public GameObject coinSpawnObject;

	// Token: 0x04003C14 RID: 15380
	private string coinSpawnAnim = "CoinSpawn1_edit";

	// Token: 0x04003C15 RID: 15381
	public GameObject coin;

	// Token: 0x04003C16 RID: 15382
	private string coinDropAnim = "MulliganCoinDropGo2Card";

	// Token: 0x04003C17 RID: 15383
	public GameObject coinGlow;

	// Token: 0x04003C18 RID: 15384
	private string coinDropAnim2 = "MulliganCoinDrop2_Edit";

	// Token: 0x04003C19 RID: 15385
	private string animToUse;

	// Token: 0x04003C1A RID: 15386
	private string coinGlowDropAnim = "MulliganCoinDrop1Glow_Edit";

	// Token: 0x04003C1B RID: 15387
	private string coinGlowDropAnim2 = "MulliganCoinDrop2Glow_Edit";

	// Token: 0x04003C1C RID: 15388
	private string GlowanimToUse;
}
