using System;
using UnityEngine;

// Token: 0x02000386 RID: 902
public class HeroPickerDisplay : MonoBehaviour
{
	// Token: 0x06002E7A RID: 11898 RVA: 0x000E93A0 File Offset: 0x000E75A0
	private void Awake()
	{
		base.transform.localPosition = HeroPickerDisplay.HERO_PICKER_START_POSITION;
		AssetLoader.Get().LoadActor((!UniversalInputManager.UsePhoneUI) ? "DeckPickerTray" : "DeckPickerTray_phone", new AssetLoader.GameObjectCallback(this.DeckPickerTrayLoaded), null, false);
		if (HeroPickerDisplay.s_instance != null)
		{
			Debug.LogWarning("HeroPickerDisplay is supposed to be a singleton, but a second instance of it is being created!");
		}
		HeroPickerDisplay.s_instance = this;
		SoundManager.Get().Load("hero_panel_slide_on");
		SoundManager.Get().Load("hero_panel_slide_off");
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x000E9439 File Offset: 0x000E7639
	private void OnDestroy()
	{
		HeroPickerDisplay.s_instance = null;
	}

	// Token: 0x06002E7C RID: 11900 RVA: 0x000E9441 File Offset: 0x000E7641
	public static HeroPickerDisplay Get()
	{
		return HeroPickerDisplay.s_instance;
	}

	// Token: 0x06002E7D RID: 11901 RVA: 0x000E9448 File Offset: 0x000E7648
	public bool IsShown()
	{
		return base.transform.localPosition == HeroPickerDisplay.HERO_PICKER_END_POSITION;
	}

	// Token: 0x06002E7E RID: 11902 RVA: 0x000E945F File Offset: 0x000E765F
	public bool IsHidden()
	{
		return base.transform.localPosition == HeroPickerDisplay.HERO_PICKER_START_POSITION;
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x000E947C File Offset: 0x000E767C
	public void ShowTray()
	{
		SoundManager.Get().LoadAndPlay("hero_panel_slide_on");
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			HeroPickerDisplay.HERO_PICKER_END_POSITION,
			"time",
			0.5f,
			"isLocal",
			true,
			"easeType",
			iTween.EaseType.easeOutBounce
		}));
	}

	// Token: 0x06002E80 RID: 11904 RVA: 0x000E94FC File Offset: 0x000E76FC
	private void DeckPickerTrayLoaded(string name, GameObject go, object callbackData)
	{
		this.m_deckPickerTray = go.GetComponent<DeckPickerTrayDisplay>();
		this.m_deckPickerTray.UpdateCreateDeckText();
		this.m_deckPickerTray.transform.parent = base.transform;
		this.m_deckPickerTray.transform.localScale = this.m_deckPickerBone.transform.localScale;
		this.m_deckPickerTray.transform.localPosition = this.m_deckPickerBone.transform.localPosition;
		this.m_deckPickerTray.Init();
		this.ShowTray();
	}

	// Token: 0x06002E81 RID: 11905 RVA: 0x000E9588 File Offset: 0x000E7788
	public void HideTray(float delay = 0f)
	{
		SoundManager.Get().LoadAndPlay("hero_panel_slide_off");
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			HeroPickerDisplay.HERO_PICKER_START_POSITION,
			"time",
			0.5f,
			"isLocal",
			true,
			"oncomplete",
			"KillHeroPicker",
			"oncompletetarget",
			base.gameObject,
			"easeType",
			iTween.EaseType.easeInCubic,
			"delay",
			delay
		}));
	}

	// Token: 0x06002E82 RID: 11906 RVA: 0x000E9643 File Offset: 0x000E7843
	private void KillHeroPicker()
	{
		this.m_deckPickerTray.Unload();
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x04001CE7 RID: 7399
	public GameObject m_deckPickerBone;

	// Token: 0x04001CE8 RID: 7400
	private static readonly PlatformDependentValue<Vector3> HERO_PICKER_START_POSITION = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(-57.36467f, 2.4869f, -28.6f),
		Phone = new Vector3(-66.4f, 2.4869f, -28.6f)
	};

	// Token: 0x04001CE9 RID: 7401
	private static readonly Vector3 HERO_PICKER_END_POSITION = new Vector3(40.6f, 2.4869f, -28.6f);

	// Token: 0x04001CEA RID: 7402
	private static HeroPickerDisplay s_instance;

	// Token: 0x04001CEB RID: 7403
	private DeckPickerTrayDisplay m_deckPickerTray;
}
