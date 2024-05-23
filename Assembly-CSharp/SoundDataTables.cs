using System;

// Token: 0x02000366 RID: 870
public class SoundDataTables
{
	// Token: 0x04001B70 RID: 7024
	public static readonly Map<SoundCategory, Option> s_categoryEnabledOptionMap = new Map<SoundCategory, Option>
	{
		{
			SoundCategory.MUSIC,
			Option.MUSIC
		},
		{
			SoundCategory.SPECIAL_MUSIC,
			Option.MUSIC
		},
		{
			SoundCategory.HERO_MUSIC,
			Option.MUSIC
		}
	};

	// Token: 0x04001B71 RID: 7025
	public static readonly Map<SoundCategory, Option> s_categoryVolumeOptionMap = new Map<SoundCategory, Option>
	{
		{
			SoundCategory.MUSIC,
			Option.MUSIC_VOLUME
		},
		{
			SoundCategory.SPECIAL_MUSIC,
			Option.MUSIC_VOLUME
		},
		{
			SoundCategory.HERO_MUSIC,
			Option.MUSIC_VOLUME
		}
	};
}
