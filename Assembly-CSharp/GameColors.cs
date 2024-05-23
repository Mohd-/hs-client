using System;
using UnityEngine;

// Token: 0x02000551 RID: 1361
public class GameColors
{
	// Token: 0x04002801 RID: 10241
	public const string BLIZZARD_BLUE_STR = "5ecaf0ff";

	// Token: 0x04002802 RID: 10242
	public const string PLAYER_NAME_STR = "5ecaf0ff";

	// Token: 0x04002803 RID: 10243
	public const string PLAYER_NAME_ONLINE_STR = "5ecaf0ff";

	// Token: 0x04002804 RID: 10244
	public const string PLAYER_NAME_OFFLINE_STR = "999999ff";

	// Token: 0x04002805 RID: 10245
	public const string PLAYER_NUMBER_STR = "a1a1a1ff";

	// Token: 0x04002806 RID: 10246
	public const string GOLD_STR = "ffd200";

	// Token: 0x04002807 RID: 10247
	public const string ORANGE_STR = "ff9c00";

	// Token: 0x04002808 RID: 10248
	public const string SERVER_SHUTDOWN_STR = "f61f1fff";

	// Token: 0x04002809 RID: 10249
	public static readonly Color BLIZZARD_BLUE = new Color(0.36862746f, 0.7921569f, 0.9411765f, 1f);

	// Token: 0x0400280A RID: 10250
	public static readonly Color PLAYER_NAME = GameColors.BLIZZARD_BLUE;

	// Token: 0x0400280B RID: 10251
	public static readonly Color PLAYER_NAME_ONLINE = GameColors.PLAYER_NAME;

	// Token: 0x0400280C RID: 10252
	public static readonly Color PLAYER_NAME_OFFLINE = new Color(0.6f, 0.6f, 0.6f, 1f);

	// Token: 0x0400280D RID: 10253
	public static readonly Color PLAYER_NUMBER = new Color(0.6313726f, 0.6313726f, 0.6313726f, 1f);

	// Token: 0x0400280E RID: 10254
	public static readonly Color SERVER_SHUTDOWN = new Color(0.9647059f, 0.12156863f, 0.12156863f, 1f);
}
