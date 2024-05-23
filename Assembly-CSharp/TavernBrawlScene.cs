using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000B04 RID: 2820
[CustomEditClass]
public class TavernBrawlScene : Scene
{
	// Token: 0x0600609D RID: 24733 RVA: 0x001CECF3 File Offset: 0x001CCEF3
	protected override void Awake()
	{
		base.Awake();
	}

	// Token: 0x0600609E RID: 24734 RVA: 0x001CECFB File Offset: 0x001CCEFB
	private void Start()
	{
		TavernBrawlManager.Get().EnsureScenarioDataReady(new TavernBrawlManager.CallbackEnsureServerDataReady(this.OnServerDataReady));
	}

	// Token: 0x0600609F RID: 24735 RVA: 0x001CED13 File Offset: 0x001CCF13
	private void Update()
	{
		Network.Get().ProcessNetwork();
	}

	// Token: 0x060060A0 RID: 24736 RVA: 0x001CED1F File Offset: 0x001CCF1F
	public override bool IsUnloading()
	{
		return this.m_unloading;
	}

	// Token: 0x060060A1 RID: 24737 RVA: 0x001CED28 File Offset: 0x001CCF28
	public override void Unload()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			BnetBar.Get().ToggleActive(true);
		}
		this.m_unloading = true;
		if (CollectionManagerDisplay.Get() != null)
		{
			CollectionManagerDisplay.Get().Unload();
		}
		if (TavernBrawlDisplay.Get() != null)
		{
			TavernBrawlDisplay.Get().Unload();
		}
		Network.SendAckCardsSeen();
		this.m_unloading = false;
	}

	// Token: 0x060060A2 RID: 24738 RVA: 0x001CED98 File Offset: 0x001CCF98
	private void OnServerDataReady()
	{
		this.m_collectionManagerNeeded = (TavernBrawlManager.Get().CurrentMission() != null && TavernBrawlManager.Get().CurrentMission().canEditDeck);
		if (this.m_collectionManagerNeeded)
		{
			AssetLoader.Get().LoadUIScreen(FileUtils.GameAssetPathToName(this.m_TavernBrawlPrefab), new AssetLoader.GameObjectCallback(this.OnTavernBrawlLoaded), null, false);
			AssetLoader.Get().LoadUIScreen(FileUtils.GameAssetPathToName(this.m_CollectionManagerPrefab), new AssetLoader.GameObjectCallback(this.OnCollectionManagerLoaded), null, false);
		}
		else
		{
			AssetLoader.Get().LoadUIScreen(FileUtils.GameAssetPathToName(this.m_TavernBrawlNoDeckPrefab), new AssetLoader.GameObjectCallback(this.OnTavernBrawlLoaded), null, false);
		}
		base.StartCoroutine(this.NotifySceneLoadedWhenReady());
	}

	// Token: 0x060060A3 RID: 24739 RVA: 0x001CEE64 File Offset: 0x001CD064
	private IEnumerator NotifySceneLoadedWhenReady()
	{
		while (!this.m_tavernBrawlPrefabLoaded)
		{
			yield return 0;
		}
		while (this.m_collectionManagerNeeded && !this.m_collectionManagerPrefabLoaded && !CollectionManagerDisplay.Get().IsReady())
		{
			yield return 0;
		}
		TavernBrawlMission currentMission = TavernBrawlManager.Get().CurrentMission();
		CollectionDeck currentDeck = TavernBrawlManager.Get().CurrentDeck();
		if (currentMission != null && currentMission.canCreateDeck && currentDeck != null)
		{
			CollectionManagerDisplay.Get().ShowTavernBrawlDeck(currentDeck.ID);
		}
		SceneMgr.Get().NotifySceneLoaded();
		yield break;
	}

	// Token: 0x060060A4 RID: 24740 RVA: 0x001CEE80 File Offset: 0x001CD080
	private void OnCollectionManagerLoaded(string name, GameObject screen, object callbackData)
	{
		this.m_collectionManagerPrefabLoaded = true;
		if (screen == null)
		{
			Debug.LogError(string.Format("TavernBrawlScene.OnCollectionManagerLoaded() - failed to load screen {0}", name));
			return;
		}
	}

	// Token: 0x060060A5 RID: 24741 RVA: 0x001CEEB4 File Offset: 0x001CD0B4
	private void OnTavernBrawlLoaded(string name, GameObject screen, object callbackData)
	{
		this.m_tavernBrawlPrefabLoaded = true;
		if (screen == null)
		{
			Debug.LogError(string.Format("TavernBrawlScene.OnTavernBrawlLoaded() - failed to load screen {0}", name));
			return;
		}
	}

	// Token: 0x0400483A RID: 18490
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_CollectionManagerPrefab;

	// Token: 0x0400483B RID: 18491
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_TavernBrawlPrefab;

	// Token: 0x0400483C RID: 18492
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public String_MobileOverride m_TavernBrawlNoDeckPrefab;

	// Token: 0x0400483D RID: 18493
	private bool m_unloading;

	// Token: 0x0400483E RID: 18494
	private bool m_tavernBrawlPrefabLoaded;

	// Token: 0x0400483F RID: 18495
	private bool m_collectionManagerNeeded;

	// Token: 0x04004840 RID: 18496
	private bool m_collectionManagerPrefabLoaded;
}
