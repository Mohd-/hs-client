using System;
using UnityEngine;

// Token: 0x0200081B RID: 2075
public class DraftScene : Scene
{
	// Token: 0x06005003 RID: 20483 RVA: 0x0017BC68 File Offset: 0x00179E68
	protected override void Awake()
	{
		base.Awake();
		if (UniversalInputManager.UsePhoneUI)
		{
			AssetLoader.Get().LoadUIScreen("Draft_phone", new AssetLoader.GameObjectCallback(this.OnPhoneUIScreenLoaded), null, false);
		}
		else
		{
			AssetLoader.Get().LoadUIScreen("Draft", new AssetLoader.GameObjectCallback(this.OnUIScreenLoaded), null, false);
		}
	}

	// Token: 0x06005004 RID: 20484 RVA: 0x0017BCCB File Offset: 0x00179ECB
	public override bool IsUnloading()
	{
		return this.m_unloading;
	}

	// Token: 0x06005005 RID: 20485 RVA: 0x0017BCD3 File Offset: 0x00179ED3
	public override void Unload()
	{
		this.m_unloading = true;
		DraftDisplay.Get().Unload();
		this.m_unloading = false;
	}

	// Token: 0x06005006 RID: 20486 RVA: 0x0017BCF0 File Offset: 0x00179EF0
	private void OnUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("DraftScene.OnUIScreenLoaded() - failed to load screen {0}", name));
			return;
		}
		screen.transform.position = new Vector3(-0.5f, 1.27f, 0f);
	}

	// Token: 0x06005007 RID: 20487 RVA: 0x0017BD3C File Offset: 0x00179F3C
	private void OnPhoneUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("DraftScene.OnUIScreenLoaded() - failed to load screen {0}", name));
			return;
		}
		screen.transform.position = new Vector3(26.1f, 0f, -9.88f);
		screen.transform.localScale = new Vector3(1.38f, 1.38f, 1.38f);
	}

	// Token: 0x040036CB RID: 14027
	private bool m_unloading;
}
