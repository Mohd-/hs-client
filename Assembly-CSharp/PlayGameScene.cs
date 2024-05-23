using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200038A RID: 906
public abstract class PlayGameScene : Scene
{
	// Token: 0x06002F6A RID: 12138 RVA: 0x000EEB94 File Offset: 0x000ECD94
	protected void Start()
	{
		AssetLoader.Get().LoadUIScreen(this.GetScreenName(), new AssetLoader.GameObjectCallback(this.OnUIScreenLoaded), null, false);
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x000EEBC0 File Offset: 0x000ECDC0
	protected void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x06002F6C RID: 12140 RVA: 0x000EEBCC File Offset: 0x000ECDCC
	public void OnDeckPickerLoaded()
	{
		this.m_deckPickerIsLoaded = true;
	}

	// Token: 0x06002F6D RID: 12141
	public abstract string GetScreenName();

	// Token: 0x06002F6E RID: 12142 RVA: 0x000EEBD5 File Offset: 0x000ECDD5
	public override void PreUnload()
	{
		if (DeckPickerTrayDisplay.Get() != null)
		{
			DeckPickerTrayDisplay.Get().PreUnload();
		}
	}

	// Token: 0x06002F6F RID: 12143 RVA: 0x000EEBF1 File Offset: 0x000ECDF1
	public override void Unload()
	{
		DeckPickerTray.Get().Unload();
		this.m_deckPickerIsLoaded = false;
	}

	// Token: 0x06002F70 RID: 12144 RVA: 0x000EEC04 File Offset: 0x000ECE04
	private void OnUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("PlayGameScene.OnUIScreenLoaded() - failed to load screen {0}", name));
			return;
		}
		base.StartCoroutine(this.WaitForAllToBeLoaded());
	}

	// Token: 0x06002F71 RID: 12145 RVA: 0x000EEC30 File Offset: 0x000ECE30
	private IEnumerator WaitForAllToBeLoaded()
	{
		while (!this.IsLoaded())
		{
			yield return null;
		}
		SceneMgr.Get().NotifySceneLoaded();
		yield break;
	}

	// Token: 0x06002F72 RID: 12146 RVA: 0x000EEC4B File Offset: 0x000ECE4B
	protected virtual bool IsLoaded()
	{
		return this.m_deckPickerIsLoaded;
	}

	// Token: 0x04001D71 RID: 7537
	private bool m_deckPickerIsLoaded;
}
