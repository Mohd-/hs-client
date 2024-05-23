using System;
using System.Collections;
using UnityEngine;

// Token: 0x020007A4 RID: 1956
[CustomEditClass]
public class CollectionManagerScene : Scene
{
	// Token: 0x06004D21 RID: 19745 RVA: 0x0016ED48 File Offset: 0x0016CF48
	protected override void Awake()
	{
		base.Awake();
		AssetLoader.Get().LoadUIScreen(FileUtils.GameAssetPathToName(this.m_CollectionManagerPrefab), new AssetLoader.GameObjectCallback(this.OnUIScreenLoaded), null, false);
	}

	// Token: 0x06004D22 RID: 19746 RVA: 0x0016ED84 File Offset: 0x0016CF84
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x06004D23 RID: 19747 RVA: 0x0016ED90 File Offset: 0x0016CF90
	public override bool IsUnloading()
	{
		return this.m_unloading;
	}

	// Token: 0x06004D24 RID: 19748 RVA: 0x0016ED98 File Offset: 0x0016CF98
	public override void Unload()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().ToggleActive(true);
		}
		this.m_unloading = true;
		CollectionManagerDisplay.Get().Unload();
		Network.SendAckCardsSeen();
		this.m_unloading = false;
	}

	// Token: 0x06004D25 RID: 19749 RVA: 0x0016EDDC File Offset: 0x0016CFDC
	private void OnUIScreenLoaded(string name, GameObject screen, object callbackData)
	{
		if (screen == null)
		{
			Debug.LogError(string.Format("CollectionManagerScene.OnUIScreenLoaded() - failed to load screen {0}", name));
			return;
		}
		base.StartCoroutine(this.NotifySceneLoadedWhenReady());
	}

	// Token: 0x06004D26 RID: 19750 RVA: 0x0016EE08 File Offset: 0x0016D008
	private IEnumerator NotifySceneLoadedWhenReady()
	{
		while (!CollectionManagerDisplay.Get().IsReady())
		{
			yield return null;
		}
		SceneMgr.Get().NotifySceneLoaded();
		yield break;
	}

	// Token: 0x04003415 RID: 13333
	private bool m_unloading;

	// Token: 0x04003416 RID: 13334
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_CollectionManagerPrefab;
}
