using System;
using UnityEngine;

// Token: 0x02000A9F RID: 2719
public class PackOpeningScene : Scene
{
	// Token: 0x06005E6C RID: 24172 RVA: 0x001C3FD8 File Offset: 0x001C21D8
	protected override void Awake()
	{
		base.Awake();
		string screenName = (!UniversalInputManager.UsePhoneUI) ? "PackOpening" : "PackOpening_phone";
		AssetLoader.Get().LoadUIScreen(screenName, new AssetLoader.GameObjectCallback(this.OnUIScreenLoaded), null, false);
	}

	// Token: 0x06005E6D RID: 24173 RVA: 0x001C4024 File Offset: 0x001C2224
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x06005E6E RID: 24174 RVA: 0x001C4030 File Offset: 0x001C2230
	private void OnUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("PackOpeningScene.OnPackOpeningLoaded() - failed to load {0}", name));
			return;
		}
		this.m_packOpening = screen.GetComponent<PackOpening>();
		if (this.m_packOpening == null)
		{
			Debug.LogError(string.Format("PackOpeningScene.OnPackOpeningLoaded() - {0} did not have a {1} component", name, typeof(PackOpening)));
			return;
		}
	}

	// Token: 0x040045E5 RID: 17893
	private PackOpening m_packOpening;
}
