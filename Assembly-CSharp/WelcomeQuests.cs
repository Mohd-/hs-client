using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class WelcomeQuests : MonoBehaviour
{
	// Token: 0x06002241 RID: 8769 RVA: 0x000A8634 File Offset: 0x000A6834
	public static void Show(UserAttentionBlocker blocker, bool fromLogin, WelcomeQuests.DelOnWelcomeQuestsClosed onCloseCallback = null, bool keepRichPresence = false)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "WelcomeQuests.Show:" + fromLogin))
		{
			if (onCloseCallback != null)
			{
				onCloseCallback();
			}
			return;
		}
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.WELCOMEQUESTS
		});
		WelcomeQuests.ShowRequestData showRequestData = new WelcomeQuests.ShowRequestData
		{
			m_fromLogin = fromLogin,
			m_onCloseCallback = onCloseCallback,
			m_keepRichPresence = keepRichPresence
		};
		if (WelcomeQuests.s_instance != null)
		{
			Debug.LogWarning("WelcomeQuests.Show(): requested to show welcome quests while it was already active!");
			WelcomeQuests.s_instance.InitAndShow(showRequestData);
			return;
		}
		AssetLoader.Get().LoadGameObject("WelcomeQuests", new AssetLoader.GameObjectCallback(WelcomeQuests.OnWelcomeQuestsLoaded), showRequestData, false);
	}

	// Token: 0x06002242 RID: 8770 RVA: 0x000A86E4 File Offset: 0x000A68E4
	public static void ShowSpecialQuest(UserAttentionBlocker blocker, Achievement achievement, WelcomeQuests.DelOnWelcomeQuestsClosed onCloseCallback = null, bool keepRichPresence = false)
	{
		if (!UserAttentionManager.CanShowAttentionGrabber(blocker, "WelcomeQuests.ShowSpecialQuest:" + ((achievement != null) ? achievement.ID.ToString() : "null")))
		{
			if (onCloseCallback != null)
			{
				onCloseCallback();
			}
			return;
		}
		PresenceMgr.Get().SetStatus(new Enum[]
		{
			PresenceStatus.WELCOMEQUESTS
		});
		WelcomeQuests.ShowRequestData showRequestData = new WelcomeQuests.ShowRequestData
		{
			m_onCloseCallback = onCloseCallback,
			m_keepRichPresence = keepRichPresence,
			m_achievement = achievement
		};
		if (WelcomeQuests.s_instance != null)
		{
			Debug.LogWarning("WelcomeQuests.Show(): requested to show welcome quests while it was already active!");
			WelcomeQuests.s_instance.InitAndShow(showRequestData);
			return;
		}
		AssetLoader.Get().LoadGameObject("WelcomeQuests", new AssetLoader.GameObjectCallback(WelcomeQuests.OnWelcomeQuestsLoaded), showRequestData, false);
	}

	// Token: 0x06002243 RID: 8771 RVA: 0x000A87AC File Offset: 0x000A69AC
	public static void Hide()
	{
		if (WelcomeQuests.s_instance == null)
		{
			return;
		}
		WelcomeQuests.s_instance.Close();
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x000A87C9 File Offset: 0x000A69C9
	public static WelcomeQuests Get()
	{
		return WelcomeQuests.s_instance;
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x000A87D0 File Offset: 0x000A69D0
	public QuestTile GetFirstQuestTile()
	{
		return this.m_currentQuests[0];
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x000A87E0 File Offset: 0x000A69E0
	private void Awake()
	{
		this.m_originalScale = base.transform.localScale;
		this.m_headlineBanner.gameObject.SetActive(false);
		this.m_clickCatcher.gameObject.SetActive(false);
		this.m_allCompletedCaption.gameObject.SetActive(false);
		SoundManager.Get().Load("new_quest_pop_up");
		SoundManager.Get().Load("existing_quest_pop_up");
		SoundManager.Get().Load("new_quest_click_and_shrink");
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x000A8864 File Offset: 0x000A6A64
	private void OnDestroy()
	{
		if (WelcomeQuests.s_instance != null)
		{
			Navigation.PopUnique(new Navigation.NavigateBackHandler(WelcomeQuests.OnNavigateBack));
			WelcomeQuests.s_instance = null;
			this.FadeEffectsOut();
			InnKeepersSpecial.Get().Show(false);
		}
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x000A88AC File Offset: 0x000A6AAC
	private static void OnWelcomeQuestsLoaded(string name, GameObject go, object callbackData)
	{
		if (SceneMgr.Get() != null && SceneMgr.Get().IsInGame())
		{
			if (WelcomeQuests.s_instance != null)
			{
				WelcomeQuests.s_instance.Close();
			}
			return;
		}
		if (go == null)
		{
			Debug.LogError(string.Format("WelcomeQuests.OnWelcomeQuestsLoaded() - FAILED to load \"{0}\"", name));
			return;
		}
		WelcomeQuests.s_instance = go.GetComponent<WelcomeQuests>();
		if (WelcomeQuests.s_instance == null)
		{
			Debug.LogError(string.Format("WelcomeQuests.OnWelcomeQuestsLoaded() - ERROR object \"{0}\" has no WelcomeQuests component", name));
			return;
		}
		WelcomeQuests.ShowRequestData showRequestData = callbackData as WelcomeQuests.ShowRequestData;
		WelcomeQuests.s_instance.InitAndShow(showRequestData);
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x000A8950 File Offset: 0x000A6B50
	private List<Achievement> GetQuestsToShow(WelcomeQuests.ShowRequestData showRequestData)
	{
		List<Achievement> list;
		if (showRequestData.m_achievement == null)
		{
			list = AchieveManager.Get().GetActiveQuests(false);
		}
		else
		{
			list = new List<Achievement>();
			list.Add(showRequestData.m_achievement);
		}
		return list;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000A898C File Offset: 0x000A6B8C
	private void InitAndShow(WelcomeQuests.ShowRequestData showRequestData)
	{
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.m_showRequestData = showRequestData;
		List<Achievement> questsToShow = this.GetQuestsToShow(showRequestData);
		if (questsToShow.Count < 1 && (!InnKeepersSpecial.Get().LoadedSuccessfully() || InnKeepersSpecial.Get().HasAlreadySeenResponse()))
		{
			Log.InnKeepersSpecial.Print("Skipping IKS! hasAlreadySeen={0}, loadedSucsesfully={1}", new object[]
			{
				InnKeepersSpecial.Get().HasAlreadySeenResponse(),
				InnKeepersSpecial.Get().LoadedSuccessfully()
			});
			this.Close();
			return;
		}
		this.m_clickCatcher.gameObject.SetActive(true);
		if (showRequestData.IsSpecialQuestRequest())
		{
			base.Invoke("RegisterClickCatcher", 2.5f);
		}
		else
		{
			this.RegisterClickCatcher();
		}
		this.ShowQuests(showRequestData);
		this.FadeEffectsIn();
		base.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
		iTween.ScaleTo(base.gameObject, this.m_originalScale, 0.5f);
		int num = Options.Get().GetInt(Option.IKS_VIEWS, 0);
		num++;
		Options.Get().SetInt(Option.IKS_VIEWS, num);
		bool @bool = Options.Get().GetBool(Option.FORCE_SHOW_IKS);
		if (showRequestData.m_fromLogin && InnKeepersSpecial.Get().LoadedSuccessfully())
		{
			if (num > 3 || @bool)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					Vector3 localPosition = base.transform.localPosition;
					localPosition.y += 2f;
					base.transform.localPosition = localPosition;
				}
				Log.InnKeepersSpecial.Print("Showing IKS!", new object[0]);
				InnKeepersSpecial.Get().Show(true);
			}
			else
			{
				Log.InnKeepersSpecial.Print("Skipping IKS! views={0}", new object[]
				{
					num
				});
			}
		}
		else
		{
			Log.InnKeepersSpecial.Print("Skipping IKS! login={0}, loadedSucsesfully={1}!", new object[]
			{
				showRequestData.m_fromLogin,
				InnKeepersSpecial.Get().LoadedSuccessfully()
			});
		}
		Navigation.PushUnique(new Navigation.NavigateBackHandler(WelcomeQuests.OnNavigateBack));
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000A8BB9 File Offset: 0x000A6DB9
	private void RegisterClickCatcher()
	{
		if (WelcomeQuests.s_instance != null)
		{
			this.m_clickCatcher.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseWelcomeQuests));
		}
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000A8BE4 File Offset: 0x000A6DE4
	private void ShowQuests(WelcomeQuests.ShowRequestData showRequestData)
	{
		List<Achievement> questsToShow = this.GetQuestsToShow(showRequestData);
		if (questsToShow.Count < 1)
		{
			this.m_allCompletedCaption.gameObject.SetActive(true);
			return;
		}
		this.m_headlineBanner.gameObject.SetActive(true);
		if (this.m_showRequestData.IsSpecialQuestRequest())
		{
			this.m_headlineBanner.SetText(GameStrings.Get("GLUE_SPECIAL_QUEST_NOTIFICATION_HEADER"));
		}
		else if (this.m_showRequestData.m_fromLogin)
		{
			this.m_headlineBanner.SetText(GameStrings.Get("GLUE_QUEST_NOTIFICATION_HEADER"));
		}
		else
		{
			this.m_headlineBanner.SetText(GameStrings.Get("GLUE_QUEST_NOTIFICATION_HEADER_NEW_ONLY"));
		}
		if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.DAILY_QUESTS) && !showRequestData.IsSpecialQuestRequest())
		{
			this.m_questCaption.Text = GameStrings.Get("GLUE_QUEST_NOTIFICATION_CAPTION");
		}
		else
		{
			this.m_questCaption.Text = string.Empty;
		}
		bool flag = true;
		foreach (Achievement achievement in questsToShow)
		{
			if (!achievement.IsLegendary)
			{
				flag = false;
				break;
			}
		}
		foreach (GameObject gameObject in this.m_normalFXs)
		{
			gameObject.SetActive(!flag);
		}
		foreach (GameObject gameObject2 in this.m_legendaryFXs)
		{
			gameObject2.SetActive(flag);
		}
		this.m_currentQuests = new List<QuestTile>();
		float num = 0.4808684f;
		float num2 = this.m_placementCollider.transform.position.x - this.m_placementCollider.GetComponent<Collider>().bounds.extents.x;
		float num3 = this.m_placementCollider.bounds.size.x / (float)questsToShow.Count;
		float num4 = num3 / 2f;
		bool flag2 = false;
		for (int k = 0; k < questsToShow.Count; k++)
		{
			Achievement achievement2 = questsToShow[k];
			bool flag3 = achievement2.IsNewlyActive();
			float num5 = 180f;
			if (flag3)
			{
				num5 = 0f;
				this.DoInnkeeperLine(achievement2);
			}
			GameObject gameObject3 = Object.Instantiate<GameObject>(this.m_questTilePrefab.gameObject);
			SceneUtils.SetLayer(gameObject3, GameLayer.UI);
			gameObject3.transform.position = new Vector3(num2 + num4, this.m_placementCollider.transform.position.y, this.m_placementCollider.transform.position.z);
			gameObject3.transform.parent = base.transform;
			gameObject3.transform.localEulerAngles = new Vector3(90f, num5, 0f);
			gameObject3.transform.localScale = new Vector3(num, num, num);
			QuestTile component = gameObject3.GetComponent<QuestTile>();
			component.SetupTile(achievement2);
			this.m_currentQuests.Add(component);
			num4 += num3;
			if (flag3)
			{
				flag2 = true;
				this.FlipQuest(component);
			}
		}
		if (flag2)
		{
			SoundManager.Get().LoadAndPlay("new_quest_pop_up");
		}
		else
		{
			SoundManager.Get().LoadAndPlay("existing_quest_pop_up");
		}
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000A8F68 File Offset: 0x000A7168
	private void DoInnkeeperLine(Achievement quest)
	{
		if (quest.ID == 11)
		{
		}
	}

	// Token: 0x0600224E RID: 8782 RVA: 0x000A8F78 File Offset: 0x000A7178
	private void FlipQuest(QuestTile quest)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0f, 0f, 540f),
			"delay",
			1,
			"time",
			2f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1
		});
		iTween.RotateAdd(quest.gameObject, args);
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x000A900C File Offset: 0x000A720C
	private void FadeEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			return;
		}
		fullScreenFXMgr.SetBlurBrightness(1f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.4f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.4f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x000A9068 File Offset: 0x000A7268
	private void FadeEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			return;
		}
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x000A90A4 File Offset: 0x000A72A4
	private void Close()
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(WelcomeQuests.OnNavigateBack));
		WelcomeQuests.s_instance = null;
		this.m_clickCatcher.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CloseWelcomeQuests));
		this.FadeEffectsOut();
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			0.5f,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"DestroyWelcomeQuests"
		}));
		SoundManager.Get().LoadAndPlay("new_quest_click_and_shrink");
		this.m_bannerFX.Play("BannerClose");
		GameToastMgr.Get().UpdateQuestProgressToasts();
		GameToastMgr.Get().AddSeasonTimeRemainingToast();
		if (this.m_showRequestData != null)
		{
			if (!this.m_showRequestData.m_keepRichPresence)
			{
				PresenceMgr.Get().SetPrevStatus();
			}
			if (this.m_showRequestData.m_onCloseCallback != null)
			{
				this.m_showRequestData.m_onCloseCallback();
			}
		}
		InnKeepersSpecial.Get().Show(false);
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x000A91CD File Offset: 0x000A73CD
	public static bool OnNavigateBack()
	{
		if (WelcomeQuests.s_instance != null)
		{
			WelcomeQuests.s_instance.Close();
		}
		return true;
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x000A91EA File Offset: 0x000A73EA
	private void CloseWelcomeQuests(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x000A91F2 File Offset: 0x000A73F2
	private void DestroyWelcomeQuests()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x040013B9 RID: 5049
	private const float SPECIAL_QUEST_DISMISS_DELAY = 2.5f;

	// Token: 0x040013BA RID: 5050
	public QuestTile m_questTilePrefab;

	// Token: 0x040013BB RID: 5051
	public Collider m_placementCollider;

	// Token: 0x040013BC RID: 5052
	public Banner m_headlineBanner;

	// Token: 0x040013BD RID: 5053
	public PegUIElement m_clickCatcher;

	// Token: 0x040013BE RID: 5054
	public UberText m_questCaption;

	// Token: 0x040013BF RID: 5055
	public UberText m_allCompletedCaption;

	// Token: 0x040013C0 RID: 5056
	public Animation m_bannerFX;

	// Token: 0x040013C1 RID: 5057
	public GameObject m_Root;

	// Token: 0x040013C2 RID: 5058
	public GameObject[] m_normalFXs;

	// Token: 0x040013C3 RID: 5059
	public GameObject[] m_legendaryFXs;

	// Token: 0x040013C4 RID: 5060
	private static WelcomeQuests s_instance;

	// Token: 0x040013C5 RID: 5061
	private WelcomeQuests.ShowRequestData m_showRequestData;

	// Token: 0x040013C6 RID: 5062
	private List<QuestTile> m_currentQuests;

	// Token: 0x040013C7 RID: 5063
	private Vector3 m_originalScale;

	// Token: 0x02000262 RID: 610
	// (Invoke) Token: 0x06002256 RID: 8790
	public delegate void DelOnWelcomeQuestsClosed();

	// Token: 0x0200068B RID: 1675
	private class ShowRequestData
	{
		// Token: 0x060046EE RID: 18158 RVA: 0x0015493F File Offset: 0x00152B3F
		public bool IsSpecialQuestRequest()
		{
			return this.m_achievement != null;
		}

		// Token: 0x04002E03 RID: 11779
		public bool m_fromLogin;

		// Token: 0x04002E04 RID: 11780
		public WelcomeQuests.DelOnWelcomeQuestsClosed m_onCloseCallback;

		// Token: 0x04002E05 RID: 11781
		public bool m_keepRichPresence;

		// Token: 0x04002E06 RID: 11782
		public Achievement m_achievement;
	}
}
