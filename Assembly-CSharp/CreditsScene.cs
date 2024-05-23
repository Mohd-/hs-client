using System;
using UnityEngine;

// Token: 0x020007D6 RID: 2006
public class CreditsScene : Scene
{
	// Token: 0x06004E57 RID: 20055 RVA: 0x00175048 File Offset: 0x00173248
	protected override void Awake()
	{
		base.Awake();
		AssetLoader.Get().LoadUIScreen("Credits", new AssetLoader.GameObjectCallback(this.OnUIScreenLoaded), null, false);
		if (InactivePlayerKicker.Get() != null)
		{
			InactivePlayerKicker.Get().SetShouldCheckForInactivity(false);
		}
	}

	// Token: 0x06004E58 RID: 20056 RVA: 0x00175094 File Offset: 0x00173294
	public override bool IsUnloading()
	{
		return this.m_unloading;
	}

	// Token: 0x06004E59 RID: 20057 RVA: 0x0017509C File Offset: 0x0017329C
	public override void Unload()
	{
		this.m_unloading = true;
		if (InactivePlayerKicker.Get() != null)
		{
			InactivePlayerKicker.Get().SetShouldCheckForInactivity(true);
		}
		this.m_unloading = false;
	}

	// Token: 0x06004E5A RID: 20058 RVA: 0x001750C7 File Offset: 0x001732C7
	private void OnUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("CreditsScene.OnUIScreenLoaded() - failed to load screen {0}", name));
			return;
		}
	}

	// Token: 0x04003553 RID: 13651
	private bool m_unloading;
}
