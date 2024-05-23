using System;
using System.Collections;

// Token: 0x02000A0F RID: 2575
public class NAX_MissionEntity : MissionEntity
{
	// Token: 0x06005B5A RID: 23386 RVA: 0x001B4330 File Offset: 0x001B2530
	protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
	{
		switch (missionEvent)
		{
		case 1001:
			NotificationManager.Get().CreateKTQuote("VO_KT_ANTI_CHEESE1_65", "VO_KT_ANTI_CHEESE1_65", true);
			break;
		case 1002:
			NotificationManager.Get().CreateKTQuote("VO_KT_ANTI_CHEESE2_66", "VO_KT_ANTI_CHEESE2_66", true);
			break;
		case 1003:
			NotificationManager.Get().CreateKTQuote("VO_KT_ANTI_CHEESE3_67", "VO_KT_ANTI_CHEESE3_67", true);
			break;
		}
		yield break;
	}
}
