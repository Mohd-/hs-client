using System;
using System.Linq;

// Token: 0x0200061D RID: 1565
public class Tags
{
	// Token: 0x06004462 RID: 17506 RVA: 0x00148A40 File Offset: 0x00146C40
	public static string DebugTag(int tag, int val)
	{
		string text = tag.ToString();
		try
		{
			text = ((GAME_TAG)tag).ToString();
		}
		catch (Exception)
		{
		}
		string text2 = val.ToString();
		GAME_TAG game_TAG = (GAME_TAG)tag;
		switch (game_TAG)
		{
		case GAME_TAG.NEXT_STEP:
			break;
		case GAME_TAG.CLASS:
			try
			{
				text2 = ((TAG_CLASS)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		case GAME_TAG.CARDRACE:
			try
			{
				text2 = ((TAG_RACE)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		case GAME_TAG.FACTION:
			try
			{
				text2 = ((TAG_FACTION)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		case GAME_TAG.CARDTYPE:
			try
			{
				text2 = ((TAG_CARDTYPE)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		case GAME_TAG.RARITY:
			try
			{
				text2 = ((TAG_RARITY)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		case GAME_TAG.STATE:
			try
			{
				text2 = ((TAG_STATE)val).ToString();
			}
			catch (Exception)
			{
			}
			goto IL_1EF;
		default:
			switch (game_TAG)
			{
			case GAME_TAG.PLAYSTATE:
				try
				{
					text2 = ((TAG_PLAYSTATE)val).ToString();
				}
				catch (Exception)
				{
				}
				goto IL_1EF;
			default:
				if (game_TAG == GAME_TAG.ENCHANTMENT_BIRTH_VISUAL || game_TAG == GAME_TAG.ENCHANTMENT_IDLE_VISUAL)
				{
					try
					{
						text2 = ((TAG_ENCHANTMENT_VISUAL)val).ToString();
					}
					catch (Exception)
					{
					}
					goto IL_1EF;
				}
				if (game_TAG == GAME_TAG.ZONE)
				{
					try
					{
						text2 = ((TAG_ZONE)val).ToString();
					}
					catch (Exception)
					{
					}
					goto IL_1EF;
				}
				if (game_TAG == GAME_TAG.CARD_SET)
				{
					try
					{
						text2 = ((TAG_CARD_SET)val).ToString();
					}
					catch (Exception)
					{
					}
					goto IL_1EF;
				}
				if (game_TAG != GAME_TAG.MULLIGAN_STATE)
				{
					goto IL_1EF;
				}
				try
				{
					text2 = ((TAG_MULLIGAN)val).ToString();
				}
				catch (Exception)
				{
				}
				goto IL_1EF;
			case GAME_TAG.STEP:
				break;
			}
			break;
		}
		try
		{
			text2 = ((TAG_STEP)val).ToString();
		}
		catch (Exception)
		{
		}
		IL_1EF:
		return string.Format("tag={0} value={1}", text, text2);
	}

	// Token: 0x06004463 RID: 17507 RVA: 0x00148CE8 File Offset: 0x00146EE8
	public static void DebugDump(EntityBase entity, params GAME_TAG[] specificTagsToDump)
	{
		Log.Henry.Print(LogLevel.Debug, string.Format("Tags.DebugDump: entity={0}", entity), new object[0]);
		Map<int, int> map = entity.GetTags().GetMap();
		int[] array;
		if (specificTagsToDump != null && specificTagsToDump.Length > 0)
		{
			array = Enumerable.ToArray<int>(Enumerable.Select<GAME_TAG, int>(specificTagsToDump, (GAME_TAG t) => (int)t));
		}
		else
		{
			array = Enumerable.ToArray<int>(map.Keys);
		}
		int[] array2 = array;
		foreach (int num in array2)
		{
			string text = string.Empty;
			if (map.ContainsKey(num))
			{
				int val = map[num];
				text = Tags.DebugTag(num, val);
			}
			else
			{
				text = string.Format("tag={0} value=(NULL)", ((GAME_TAG)num).ToString());
			}
			Log.Henry.Print(LogLevel.Debug, string.Format("Tags.DebugDump:           {0}", text), new object[0]);
		}
	}

	// Token: 0x04002B5C RID: 11100
	public const int MAX = 512;
}
