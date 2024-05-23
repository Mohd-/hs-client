using System;
using System.Collections.Generic;

// Token: 0x0200026F RID: 623
public class RankMgr
{
	// Token: 0x060022E2 RID: 8930 RVA: 0x000AB976 File Offset: 0x000A9B76
	public static RankMgr Get()
	{
		if (RankMgr.s_instance == null)
		{
			RankMgr.s_instance = new RankMgr();
		}
		return RankMgr.s_instance;
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000AB994 File Offset: 0x000A9B94
	public bool SetRankPresenceField(NetCache.NetCacheMedalInfo medalInfo)
	{
		List<byte> list = new List<byte>();
		TranslatedMedalInfo currentMedal = new MedalInfoTranslator(medalInfo).GetCurrentMedal(false);
		byte b = Convert.ToByte(currentMedal.rank);
		int legendIndex = currentMedal.legendIndex;
		list.Add(b);
		byte[] bytes = BitConverter.GetBytes(legendIndex);
		list.Add(bytes[0]);
		list.Add(bytes[1]);
		currentMedal = new MedalInfoTranslator(medalInfo).GetCurrentMedal(true);
		byte b2 = Convert.ToByte(currentMedal.rank);
		int legendIndex2 = currentMedal.legendIndex;
		list.Add(b2);
		byte[] bytes2 = BitConverter.GetBytes(legendIndex2);
		list.Add(bytes2[0]);
		list.Add(bytes2[1]);
		byte[] val = list.ToArray();
		return BnetPresenceMgr.Get().SetGameField(18U, val);
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000ABA47 File Offset: 0x000A9C47
	public MedalInfoTranslator GetRankPresenceField(BnetPlayer player)
	{
		return this.GetRankPresenceField(player.GetHearthstoneGameAccount());
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000ABA58 File Offset: 0x000A9C58
	public MedalInfoTranslator GetRankPresenceField(BnetGameAccount gameAccount)
	{
		byte[] array;
		if (!(gameAccount != null) || !gameAccount.TryGetGameFieldBytes(18U, out array))
		{
			return null;
		}
		if (array == null)
		{
			return null;
		}
		if (array.Length < 6)
		{
			return null;
		}
		int rank = Convert.ToInt32(array[0]);
		int legendIndex = (int)BitConverter.ToUInt16(array, 1);
		int wildRank = Convert.ToInt32(array[3]);
		int wildLegendIndex = (int)BitConverter.ToUInt16(array, 4);
		return new MedalInfoTranslator(rank, legendIndex, wildRank, wildLegendIndex);
	}

	// Token: 0x04001437 RID: 5175
	public const int INVALID_RANK = -1;

	// Token: 0x04001438 RID: 5176
	private static RankMgr s_instance;

	// Token: 0x04001439 RID: 5177
	private TranslatedMedalInfo m_medalInfo;
}
