using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000867 RID: 2151
public class RankedRewardChest : MonoBehaviour
{
	// Token: 0x060052A8 RID: 21160 RVA: 0x0018A60C File Offset: 0x0018880C
	public static int GetChestIndexFromRank(int rank)
	{
		if (rank <= 5)
		{
			return 3;
		}
		if (rank <= 10)
		{
			return 2;
		}
		if (rank <= 15)
		{
			return 1;
		}
		if (rank <= 20)
		{
			return 0;
		}
		return -1;
	}

	// Token: 0x060052A9 RID: 21161 RVA: 0x0018A644 File Offset: 0x00188844
	public static string GetChestNameFromRank(int rank)
	{
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(rank);
		return GameStrings.Format(string.Format(RankedRewardChest.s_rewardChestNameText, RankedRewardChest.NUM_REWARD_TIERS - chestIndexFromRank), new object[0]);
	}

	// Token: 0x060052AA RID: 21162 RVA: 0x0018A67C File Offset: 0x0018887C
	public static string GetChestEarnedFromRank(int rank)
	{
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(rank);
		return GameStrings.Format(string.Format(RankedRewardChest.s_rewardChestEarnedText, RankedRewardChest.NUM_REWARD_TIERS - chestIndexFromRank), new object[0]);
	}

	// Token: 0x060052AB RID: 21163 RVA: 0x0018A6B4 File Offset: 0x001888B4
	public ChestVisual GetChestVisualFromRank(int rank)
	{
		int chestIndexFromRank = RankedRewardChest.GetChestIndexFromRank(rank);
		if (chestIndexFromRank >= 0)
		{
			return this.m_chests[chestIndexFromRank];
		}
		return null;
	}

	// Token: 0x060052AC RID: 21164 RVA: 0x0018A6DD File Offset: 0x001888DD
	public bool DoesChestVisualChange(int rank1, int rank2)
	{
		return this.GetChestVisualFromRank(rank1) != this.GetChestVisualFromRank(rank2);
	}

	// Token: 0x060052AD RID: 21165 RVA: 0x0018A6F4 File Offset: 0x001888F4
	public void SetRank(int rank)
	{
		Log.EndOfGame.Print("setting chest to rank " + rank, new object[0]);
		ChestVisual chestVisualFromRank = this.GetChestVisualFromRank(rank);
		this.m_baseMeshFilter.mesh = chestVisualFromRank.m_chestMesh;
		this.m_baseMeshRenderer.material = chestVisualFromRank.m_chestMaterial;
		if (this.m_glowMeshRenderer != null)
		{
			this.m_glowMeshRenderer.material = chestVisualFromRank.m_glowMaterial;
		}
		if (rank == 0)
		{
			this.m_legendaryGem.SetActive(true);
			this.m_rankNumber.gameObject.SetActive(false);
		}
		else
		{
			this.m_legendaryGem.SetActive(false);
			this.m_rankNumber.gameObject.SetActive(true);
		}
		this.m_rankNumber.gameObject.SetActive(true);
		this.m_rankNumber.Text = rank.ToString();
		this.m_rankBanner.Text = GameStrings.Get(chestVisualFromRank.chestName);
	}

	// Token: 0x040038DD RID: 14557
	public GameObject m_starDestinationBone;

	// Token: 0x040038DE RID: 14558
	public MeshFilter m_baseMeshFilter;

	// Token: 0x040038DF RID: 14559
	public MeshRenderer m_baseMeshRenderer;

	// Token: 0x040038E0 RID: 14560
	public MeshRenderer m_glowMeshRenderer;

	// Token: 0x040038E1 RID: 14561
	public List<ChestVisual> m_chests;

	// Token: 0x040038E2 RID: 14562
	public UberText m_rankNumber;

	// Token: 0x040038E3 RID: 14563
	public UberText m_rankBanner;

	// Token: 0x040038E4 RID: 14564
	public GameObject m_legendaryGem;

	// Token: 0x040038E5 RID: 14565
	public static int NUM_REWARD_TIERS = 4;

	// Token: 0x040038E6 RID: 14566
	private static string s_rewardChestEarnedText = "GLOBAL_REWARD_CHEST_TIER{0}_EARNED";

	// Token: 0x040038E7 RID: 14567
	private static string s_rewardChestNameText = "GLOBAL_REWARD_CHEST_TIER{0}";
}
