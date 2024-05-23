using System;

// Token: 0x0200054C RID: 1356
public class RichPresence
{
	// Token: 0x040027E8 RID: 10216
	public const uint FIELD_INDEX_START = 458752U;

	// Token: 0x040027E9 RID: 10217
	public static readonly FourCC STATUS_STREAMID = new FourCC("stat");

	// Token: 0x040027EA RID: 10218
	public static readonly FourCC TUTORIAL_STREAMID = new FourCC("tut");

	// Token: 0x040027EB RID: 10219
	public static readonly FourCC SCENARIOS_STREAMID = new FourCC("scen");

	// Token: 0x040027EC RID: 10220
	public static readonly Map<Type, FourCC> s_streamIds = new Map<Type, FourCC>
	{
		{
			typeof(PresenceStatus),
			RichPresence.STATUS_STREAMID
		},
		{
			typeof(PresenceTutorial),
			RichPresence.TUTORIAL_STREAMID
		},
		{
			typeof(ScenarioDbId),
			RichPresence.SCENARIOS_STREAMID
		}
	};
}
