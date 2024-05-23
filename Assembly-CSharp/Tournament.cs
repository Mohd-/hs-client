using System;
using UnityEngine;

// Token: 0x02000698 RID: 1688
public class Tournament
{
	// Token: 0x06004736 RID: 18230 RVA: 0x001559F0 File Offset: 0x00153BF0
	public static void Init()
	{
		if (Tournament.s_instance != null)
		{
			return;
		}
		Tournament.s_instance = new Tournament();
	}

	// Token: 0x06004737 RID: 18231 RVA: 0x00155A07 File Offset: 0x00153C07
	public static Tournament Get()
	{
		if (Tournament.s_instance == null)
		{
			Debug.LogError("Trying to retrieve the Tournament without calling Tournament.Init()!");
		}
		return Tournament.s_instance;
	}

	// Token: 0x06004738 RID: 18232 RVA: 0x00155A22 File Offset: 0x00153C22
	public void NotifyOfBoxTransitionStart()
	{
		Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
	}

	// Token: 0x06004739 RID: 18233 RVA: 0x00155A3C File Offset: 0x00153C3C
	public void OnBoxTransitionFinished(object userData)
	{
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
		if (!Options.Get().GetBool(Option.HAS_SEEN_TOURNAMENT, false))
		{
			Options.Get().SetBool(Option.HAS_SEEN_TOURNAMENT, true);
		}
	}

	// Token: 0x04002E2D RID: 11821
	private static Tournament s_instance;
}
