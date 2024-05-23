using System;
using UnityEngine;

// Token: 0x020008A6 RID: 2214
public class FriendlyDisplay : MonoBehaviour
{
	// Token: 0x0600541D RID: 21533 RVA: 0x001928F0 File Offset: 0x00190AF0
	private void Awake()
	{
		FriendlyDisplay.s_instance = this;
		AssetLoader.Get().LoadActor((!UniversalInputManager.UsePhoneUI) ? "DeckPickerTray" : "DeckPickerTray_phone", delegate(string name, GameObject go, object data)
		{
			if (go == null)
			{
				Debug.LogError("Unable to load DeckPickerTray.");
				return;
			}
			this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
			if (this.m_deckPickerTray == null)
			{
				Debug.LogError("DeckPickerTrayDisplay component not found in DeckPickerTray object.");
				return;
			}
			GameUtils.SetParent(this.m_deckPickerTray, this.m_deckPickerTrayContainer, false);
			this.m_deckPickerTray.SetHeaderText(GameStrings.Get((!FriendChallengeMgr.Get().IsChallengeTavernBrawl()) ? "GLOBAL_FRIEND_CHALLENGE_TITLE" : "GLOBAL_TAVERN_BRAWL"));
			this.m_deckPickerTray.Init();
			this.DisableOtherModeStuff();
			NetCache.Get().RegisterScreenFriendly(new NetCache.NetCacheCallback(this.OnNetCacheReady));
			MusicManager.Get().StartPlaylist((!FriendChallengeMgr.Get().IsChallengeTavernBrawl()) ? MusicPlaylistType.UI_Friendly : MusicPlaylistType.UI_TavernBrawl);
		}, null, false);
	}

	// Token: 0x0600541E RID: 21534 RVA: 0x0019293A File Offset: 0x00190B3A
	private void OnDestroy()
	{
		FriendlyDisplay.s_instance = null;
	}

	// Token: 0x0600541F RID: 21535 RVA: 0x00192942 File Offset: 0x00190B42
	public static FriendlyDisplay Get()
	{
		return FriendlyDisplay.s_instance;
	}

	// Token: 0x06005420 RID: 21536 RVA: 0x00192949 File Offset: 0x00190B49
	public void Unload()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
	}

	// Token: 0x06005421 RID: 21537 RVA: 0x00192964 File Offset: 0x00190B64
	private void DisableOtherModeStuff()
	{
		if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		Camera camera = CameraUtils.FindFullScreenEffectsCamera(true);
		if (camera != null)
		{
			FullScreenEffects component = camera.GetComponent<FullScreenEffects>();
			component.Disable();
		}
	}

	// Token: 0x06005422 RID: 21538 RVA: 0x001929A4 File Offset: 0x00190BA4
	private void OnNetCacheReady()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
		if (AchieveManager.Get().HasActiveQuests(true))
		{
			WelcomeQuests.Show(UserAttentionBlocker.NONE, false, null, false);
		}
		else
		{
			GameToastMgr.Get().UpdateQuestProgressToasts();
		}
	}

	// Token: 0x04003A42 RID: 14914
	public GameObject m_deckPickerTrayContainer;

	// Token: 0x04003A43 RID: 14915
	private static FriendlyDisplay s_instance;

	// Token: 0x04003A44 RID: 14916
	private DeckPickerTrayDisplay m_deckPickerTray;
}
