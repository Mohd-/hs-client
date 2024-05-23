using System;

// Token: 0x02000035 RID: 53
internal static class HashPrimeNumbers
{
	// Token: 0x06000449 RID: 1097 RVA: 0x00013090 File Offset: 0x00011290
	public static bool TestPrime(int x)
	{
		if ((x & 1) != 0)
		{
			int num = (int)Math.Sqrt((double)x);
			for (int i = 3; i < num; i += 2)
			{
				if (x % i == 0)
				{
					return false;
				}
			}
			return true;
		}
		return x == 2;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x000130D0 File Offset: 0x000112D0
	public static int CalcPrime(int x)
	{
		for (int i = (x & -2) - 1; i < 2147483647; i += 2)
		{
			if (HashPrimeNumbers.TestPrime(i))
			{
				return i;
			}
		}
		return x;
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00013108 File Offset: 0x00011308
	public static int ToPrime(int x)
	{
		for (int i = 0; i < HashPrimeNumbers.primeTbl.Length; i++)
		{
			if (x <= HashPrimeNumbers.primeTbl[i])
			{
				return HashPrimeNumbers.primeTbl[i];
			}
		}
		return HashPrimeNumbers.CalcPrime(x);
	}

	// Token: 0x040001DC RID: 476
	private static readonly int[] primeTbl = new int[]
	{
		11,
		19,
		37,
		73,
		109,
		163,
		251,
		367,
		557,
		823,
		1237,
		1861,
		2777,
		4177,
		6247,
		9371,
		14057,
		21089,
		31627,
		47431,
		71143,
		106721,
		160073,
		240101,
		360163,
		540217,
		810343,
		1215497,
		1823231,
		2734867,
		4102283,
		6153409,
		9230113,
		13845163
	};
}
