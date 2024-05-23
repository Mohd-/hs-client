using System;

// Token: 0x02000655 RID: 1621
internal struct ArenaRecord
{
	// Token: 0x0600457A RID: 17786 RVA: 0x0014D894 File Offset: 0x0014BA94
	public static bool TryParse(string s, out ArenaRecord result)
	{
		result = ArenaRecord.Invalid;
		if (string.IsNullOrEmpty(s))
		{
			return false;
		}
		char[] array = new char[]
		{
			','
		};
		string[] array2 = s.Split(array);
		if (array2.Length != 3)
		{
			return false;
		}
		int num = 0;
		int num2 = 0;
		if (!int.TryParse(array2[0], ref num) || !int.TryParse(array2[1], ref num2))
		{
			return false;
		}
		int num3 = 0;
		if (!int.TryParse(array2[2], ref num3))
		{
			return false;
		}
		result.wins = num;
		result.losses = num2;
		result.isFinished = (num3 == 1);
		return true;
	}

	// Token: 0x04002C6F RID: 11375
	public int wins;

	// Token: 0x04002C70 RID: 11376
	public int losses;

	// Token: 0x04002C71 RID: 11377
	public bool isFinished;

	// Token: 0x04002C72 RID: 11378
	public static ArenaRecord Invalid;
}
