using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000213 RID: 531
[CustomEditClass]
public class AdventureSubScene : MonoBehaviour
{
	// Token: 0x06002095 RID: 8341 RVA: 0x0009F543 File Offset: 0x0009D743
	public void SetIsLoaded(bool loaded)
	{
		this.m_IsLoaded = loaded;
	}

	// Token: 0x06002096 RID: 8342 RVA: 0x0009F54C File Offset: 0x0009D74C
	public bool IsLoaded()
	{
		return this.m_IsLoaded;
	}

	// Token: 0x06002097 RID: 8343 RVA: 0x0009F554 File Offset: 0x0009D754
	public void AddSubSceneTransitionFinishedListener(AdventureSubScene.SubSceneTransitionFinished dlg)
	{
		this.m_SubSceneTransitionListeners.Add(dlg);
	}

	// Token: 0x06002098 RID: 8344 RVA: 0x0009F562 File Offset: 0x0009D762
	public void RemoveSubSceneTransitionFinishedListener(AdventureSubScene.SubSceneTransitionFinished dlg)
	{
		this.m_SubSceneTransitionListeners.Remove(dlg);
	}

	// Token: 0x06002099 RID: 8345 RVA: 0x0009F571 File Offset: 0x0009D771
	public void NotifyTransitionComplete()
	{
		this.FireSubSceneTransitionFinishedEvent();
	}

	// Token: 0x0600209A RID: 8346 RVA: 0x0009F57C File Offset: 0x0009D77C
	private void FireSubSceneTransitionFinishedEvent()
	{
		AdventureSubScene.SubSceneTransitionFinished[] array = this.m_SubSceneTransitionListeners.ToArray();
		foreach (AdventureSubScene.SubSceneTransitionFinished subSceneTransitionFinished in array)
		{
			subSceneTransitionFinished();
		}
	}

	// Token: 0x040011D2 RID: 4562
	[CustomEditField(Sections = "Animation Settings")]
	public float m_TransitionAnimationTime = 1f;

	// Token: 0x040011D3 RID: 4563
	[CustomEditField(Sections = "Bounds Settings")]
	public Vector3_MobileOverride m_SubSceneBounds;

	// Token: 0x040011D4 RID: 4564
	private bool m_IsLoaded;

	// Token: 0x040011D5 RID: 4565
	private List<AdventureSubScene.SubSceneTransitionFinished> m_SubSceneTransitionListeners = new List<AdventureSubScene.SubSceneTransitionFinished>();

	// Token: 0x02000228 RID: 552
	// (Invoke) Token: 0x0600212A RID: 8490
	public delegate void SubSceneTransitionFinished();
}
