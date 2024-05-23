using System;
using System.Collections;

// Token: 0x02000AA4 RID: 2724
public class Reset : Scene
{
	// Token: 0x06005E82 RID: 24194 RVA: 0x001C49E3 File Offset: 0x001C2BE3
	private void Start()
	{
		SceneMgr.Get().NotifySceneLoaded();
		base.StartCoroutine("WaitThenReset");
	}

	// Token: 0x06005E83 RID: 24195 RVA: 0x001C49FC File Offset: 0x001C2BFC
	private IEnumerator WaitThenReset()
	{
		while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
		{
			yield return null;
		}
		ApplicationMgr.Get().Reset();
		yield break;
	}
}
