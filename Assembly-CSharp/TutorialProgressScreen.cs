using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004EC RID: 1260
public class TutorialProgressScreen : MonoBehaviour
{
	// Token: 0x06003B32 RID: 15154 RVA: 0x0011ECA8 File Offset: 0x0011CEA8
	private void Awake()
	{
		TutorialProgressScreen.s_instance = this;
		FullScreenFXMgr.Get().Vignette(1f, 0.5f, iTween.EaseType.easeInOutQuad, null);
		this.m_lessonTitle.Text = GameStrings.Get("TUTORIAL_PROGRESS_LESSON_TITLE");
		this.m_missionProgressTitle.Text = GameStrings.Get("TUTORIAL_PROGRESS_TITLE");
		this.m_exitButton.gameObject.SetActive(false);
		this.InitMissionRecords();
	}

	// Token: 0x06003B33 RID: 15155 RVA: 0x0011ED12 File Offset: 0x0011CF12
	private void OnDestroy()
	{
		NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.UpdateProgress));
		TutorialProgressScreen.s_instance = null;
	}

	// Token: 0x06003B34 RID: 15156 RVA: 0x0011ED30 File Offset: 0x0011CF30
	public static TutorialProgressScreen Get()
	{
		return TutorialProgressScreen.s_instance;
	}

	// Token: 0x06003B35 RID: 15157 RVA: 0x0011ED38 File Offset: 0x0011CF38
	public void StartTutorialProgress()
	{
		if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
		{
			Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
			if (friendlySidePlayer.GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) == TAG_PLAYSTATE.WON)
			{
				Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
				Card heroCard = opposingSidePlayer.GetHeroCard();
				Spell actorSpell = heroCard.GetActorSpell(SpellType.ENDGAME_WIN, true);
				actorSpell.ActivateState(SpellStateType.DEATH);
				this.m_showProgressSavedMessage = true;
			}
			Gameplay.Get().RemoveGamePlayNameBannerPhone();
		}
		this.LoadAllTutorialHeroEntities();
	}

	// Token: 0x06003B36 RID: 15158 RVA: 0x0011EDA8 File Offset: 0x0011CFA8
	public void SetCoinPressCallback(HeroCoin.CoinPressCallback callback)
	{
		if (callback == null)
		{
			return;
		}
		this.m_coinPressCallback = delegate()
		{
			this.Hide();
			callback();
		};
	}

	// Token: 0x06003B37 RID: 15159 RVA: 0x0011EDE8 File Offset: 0x0011CFE8
	private void InitMissionRecords()
	{
		foreach (ScenarioDbfRecord scenarioDbfRecord in GameDbf.Scenario.GetRecords())
		{
			if (scenarioDbfRecord.AdventureId == 1)
			{
				int id = scenarioDbfRecord.ID;
				if (Enum.IsDefined(typeof(ScenarioDbId), id))
				{
					this.m_sortedMissionRecords.Add(scenarioDbfRecord);
				}
			}
		}
		this.m_sortedMissionRecords.Sort(new Comparison<ScenarioDbfRecord>(GameUtils.MissionSortComparison));
	}

	// Token: 0x06003B38 RID: 15160 RVA: 0x0011EE9C File Offset: 0x0011D09C
	private void LoadAllTutorialHeroEntities()
	{
		for (int i = 0; i < this.m_sortedMissionRecords.Count; i++)
		{
			ScenarioDbfRecord scenarioDbfRecord = this.m_sortedMissionRecords[i];
			int id = scenarioDbfRecord.ID;
			string missionHeroCardId = GameUtils.GetMissionHeroCardId(id);
			if (DefLoader.Get().GetEntityDef(missionHeroCardId) == null)
			{
				Debug.LogError(string.Format("TutorialProgress.OnTutorialHeroEntityDefLoaded() - failed to load {0}", missionHeroCardId));
			}
		}
		this.SetupCoins();
		this.Show();
	}

	// Token: 0x06003B39 RID: 15161 RVA: 0x0011EF10 File Offset: 0x0011D110
	private void SetupCoins()
	{
		this.HERO_COIN_START = new Vector3(0.5f, 0.1f, 0.32f);
		Vector3 vector = Vector3.zero;
		for (int i = 0; i < this.m_sortedMissionRecords.Count; i++)
		{
			ScenarioDbfRecord scenarioDbfRecord = this.m_sortedMissionRecords[i];
			int id = scenarioDbfRecord.ID;
			HeroCoin heroCoin = Object.Instantiate<HeroCoin>(this.m_coinPrefab);
			heroCoin.transform.parent = base.transform;
			heroCoin.gameObject.SetActive(false);
			heroCoin.SetCoinPressCallback(this.m_coinPressCallback);
			int num = Random.Range(0, 3);
			Vector2 crackTexture;
			if (num == 1)
			{
				crackTexture..ctor(0.25f, -1f);
			}
			else if (num == 2)
			{
				crackTexture..ctor(0.5f, -1f);
			}
			else
			{
				crackTexture..ctor(0f, -1f);
			}
			if (i == 0)
			{
				heroCoin.transform.localPosition = this.HERO_COIN_START;
			}
			else
			{
				heroCoin.transform.localPosition = new Vector3(vector.x + -0.2f, vector.y, vector.z);
			}
			string text = null;
			TutorialProgressScreen.LessonAsset lessonAsset;
			this.m_missionIdToLessonAssetMap.TryGetValue((ScenarioDbId)id, out lessonAsset);
			if (lessonAsset != null)
			{
				if (UniversalInputManager.UsePhoneUI && !string.IsNullOrEmpty(lessonAsset.m_phoneAsset))
				{
					text = lessonAsset.m_phoneAsset;
				}
				else
				{
					text = lessonAsset.m_asset;
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				heroCoin.SetLessonAssetName(text);
			}
			this.m_heroCoins.Add(heroCoin);
			Vector2 zero = Vector2.zero;
			Vector2 zero2 = Vector2.zero;
			int num2 = id;
			if (num2 != 3)
			{
				if (num2 != 4)
				{
					if (num2 != 248)
					{
						if (num2 != 249)
						{
							if (num2 != 181)
							{
								if (num2 == 201)
								{
									zero..ctor(0f, 0f);
									zero2..ctor(0.25f, 0f);
								}
							}
							else
							{
								zero..ctor(0.5f, -0.25f);
								zero2..ctor(0.75f, -0.25f);
							}
						}
						else
						{
							zero..ctor(0.5f, -0.5f);
							zero2..ctor(0.75f, -0.5f);
						}
					}
					else
					{
						zero..ctor(0f, -0.5f);
						zero2..ctor(0.25f, -0.5f);
					}
				}
				else
				{
					zero..ctor(0.5f, 0f);
					zero2..ctor(0.75f, 0f);
				}
			}
			else
			{
				zero..ctor(0f, -0.25f);
				zero2..ctor(0.25f, -0.25f);
			}
			heroCoin.SetCoinInfo(zero, zero2, crackTexture, id);
			vector = heroCoin.transform.localPosition;
		}
		SceneUtils.SetLayer(base.gameObject, GameLayer.IgnoreFullScreenEffects);
	}

	// Token: 0x06003B3A RID: 15162 RVA: 0x0011F224 File Offset: 0x0011D424
	private void Show()
	{
		iTween.FadeTo(base.gameObject, 1f, 0.25f);
		bool flag = SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY;
		base.transform.position = ((!flag) ? this.FINAL_POS_OVER_BOX : this.FINAL_POS);
		base.transform.localScale = this.START_SCALE;
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			(!flag) ? this.FINAL_SCALE_OVER_BOX : this.FINAL_SCALE,
			"time",
			0.5f,
			"oncomplete",
			"OnScaleAnimComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.ScaleTo(base.gameObject, args);
	}

	// Token: 0x06003B3B RID: 15163 RVA: 0x0011F304 File Offset: 0x0011D504
	private void Hide()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			this.START_SCALE,
			"time",
			0.5f,
			"oncomplete",
			"OnHideAnimComplete",
			"oncompletetarget",
			base.gameObject
		});
		iTween.ScaleTo(base.gameObject, args);
		args = iTween.Hash(new object[]
		{
			"alpha",
			0f,
			"time",
			0.25f,
			"delay",
			0.25f
		});
		iTween.FadeTo(base.gameObject, args);
	}

	// Token: 0x06003B3C RID: 15164 RVA: 0x0011F3CC File Offset: 0x0011D5CC
	private void OnScaleAnimComplete()
	{
		if (this.IS_TESTING)
		{
			this.UpdateProgress();
		}
		else
		{
			NetCache.Get().RegisterTutorialEndGameScreen(new NetCache.NetCacheCallback(this.UpdateProgress), new NetCache.ErrorCallback(NetCache.DefaultErrorHandler));
		}
		foreach (HeroCoin heroCoin in this.m_heroCoins)
		{
			heroCoin.FinishIntroScaling();
		}
	}

	// Token: 0x06003B3D RID: 15165 RVA: 0x0011F460 File Offset: 0x0011D660
	private void OnHideAnimComplete()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06003B3E RID: 15166 RVA: 0x0011F470 File Offset: 0x0011D670
	private void UpdateProgress()
	{
		ScenarioDbId nextMissionId;
		if (this.IS_TESTING)
		{
			nextMissionId = this.m_progressToNextMissionIdMap[TutorialProgress.HOGGER_COMPLETE];
		}
		else
		{
			NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
			nextMissionId = this.m_progressToNextMissionIdMap[netObject.CampaignProgress];
		}
		int num = this.m_heroCoins.FindIndex((HeroCoin coin) => coin.GetMissionId() == (int)nextMissionId);
		for (int i = 0; i < this.m_heroCoins.Count; i++)
		{
			HeroCoin heroCoin = this.m_heroCoins[i];
			if (i == num - 1)
			{
				base.StartCoroutine(this.SetActiveToDefeated(heroCoin));
			}
			else if (i < num)
			{
				heroCoin.SetProgress(HeroCoin.CoinStatus.DEFEATED);
			}
			else if (i == num)
			{
				base.StartCoroutine(this.SetUnrevealedToActive(heroCoin));
				string lessonAssetName = heroCoin.GetLessonAssetName();
				if (!string.IsNullOrEmpty(lessonAssetName))
				{
					AssetLoader.Get().LoadGameObject(lessonAssetName, new AssetLoader.GameObjectCallback(this.OnTutorialImageLoaded), null, false);
				}
			}
			else
			{
				heroCoin.SetProgress(HeroCoin.CoinStatus.UNREVEALED);
			}
		}
		if (this.m_showProgressSavedMessage)
		{
			UIStatus.Get().AddInfo(GameStrings.Get("TUTORIAL_PROGRESS_SAVED"));
			this.m_showProgressSavedMessage = false;
		}
	}

	// Token: 0x06003B3F RID: 15167 RVA: 0x0011F5B2 File Offset: 0x0011D7B2
	private void OnTutorialImageLoaded(string name, GameObject go, object callbackData)
	{
		this.SetupTutorialImage(go);
	}

	// Token: 0x06003B40 RID: 15168 RVA: 0x0011F5BC File Offset: 0x0011D7BC
	private void SetupTutorialImage(GameObject go)
	{
		SceneUtils.SetLayer(go, GameLayer.IgnoreFullScreenEffects);
		go.transform.parent = this.m_currentLessonBone.transform;
		go.transform.localScale = Vector3.one;
		go.transform.localEulerAngles = Vector3.zero;
		go.transform.localPosition = Vector3.zero;
	}

	// Token: 0x06003B41 RID: 15169 RVA: 0x0011F618 File Offset: 0x0011D818
	private IEnumerator SetActiveToDefeated(HeroCoin coin)
	{
		coin.SetProgress(HeroCoin.CoinStatus.ACTIVE);
		coin.m_inputEnabled = false;
		yield return new WaitForSeconds(1f);
		coin.SetProgress(HeroCoin.CoinStatus.ACTIVE_TO_DEFEATED);
		yield break;
	}

	// Token: 0x06003B42 RID: 15170 RVA: 0x0011F63C File Offset: 0x0011D83C
	private IEnumerator SetUnrevealedToActive(HeroCoin coin)
	{
		coin.SetProgress(HeroCoin.CoinStatus.UNREVEALED);
		coin.m_inputEnabled = false;
		yield return new WaitForSeconds(2f);
		coin.SetProgress(HeroCoin.CoinStatus.UNREVEALED_TO_ACTIVE);
		yield break;
	}

	// Token: 0x06003B43 RID: 15171 RVA: 0x0011F660 File Offset: 0x0011D860
	private void ExitButtonPress(UIEvent e)
	{
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		FullScreenFXMgr.Get().Vignette(0f, 0.5f, iTween.EaseType.easeInOutQuad, null);
	}

	// Token: 0x040025C8 RID: 9672
	private const float START_SCALE_VAL = 0.5f;

	// Token: 0x040025C9 RID: 9673
	private const float HERO_SPACING = -0.2f;

	// Token: 0x040025CA RID: 9674
	public HeroCoin m_coinPrefab;

	// Token: 0x040025CB RID: 9675
	public UberText m_lessonTitle;

	// Token: 0x040025CC RID: 9676
	public UberText m_missionProgressTitle;

	// Token: 0x040025CD RID: 9677
	public GameObject m_currentLessonBone;

	// Token: 0x040025CE RID: 9678
	public PegUIElement m_exitButton;

	// Token: 0x040025CF RID: 9679
	public UberText m_exitButtonLabel;

	// Token: 0x040025D0 RID: 9680
	private static TutorialProgressScreen s_instance;

	// Token: 0x040025D1 RID: 9681
	private List<HeroCoin> m_heroCoins = new List<HeroCoin>();

	// Token: 0x040025D2 RID: 9682
	private HeroCoin.CoinPressCallback m_coinPressCallback;

	// Token: 0x040025D3 RID: 9683
	private bool m_showProgressSavedMessage;

	// Token: 0x040025D4 RID: 9684
	private readonly Map<TutorialProgress, ScenarioDbId> m_progressToNextMissionIdMap = new Map<TutorialProgress, ScenarioDbId>
	{
		{
			TutorialProgress.NOTHING_COMPLETE,
			ScenarioDbId.TUTORIAL_HOGGER
		},
		{
			TutorialProgress.HOGGER_COMPLETE,
			ScenarioDbId.TUTORIAL_MILLHOUSE
		},
		{
			TutorialProgress.MILLHOUSE_COMPLETE,
			ScenarioDbId.TUTORIAL_CHO
		},
		{
			TutorialProgress.CHO_COMPLETE,
			ScenarioDbId.TUTORIAL_MUKLA
		},
		{
			TutorialProgress.MUKLA_COMPLETE,
			ScenarioDbId.TUTORIAL_NESINGWARY
		},
		{
			TutorialProgress.NESINGWARY_COMPLETE,
			ScenarioDbId.TUTORIAL_ILLIDAN
		}
	};

	// Token: 0x040025D5 RID: 9685
	private readonly Map<ScenarioDbId, TutorialProgressScreen.LessonAsset> m_missionIdToLessonAssetMap = new Map<ScenarioDbId, TutorialProgressScreen.LessonAsset>
	{
		{
			ScenarioDbId.TUTORIAL_HOGGER,
			null
		},
		{
			ScenarioDbId.TUTORIAL_MILLHOUSE,
			new TutorialProgressScreen.LessonAsset
			{
				m_asset = "Tutorial_Lesson1"
			}
		},
		{
			ScenarioDbId.TUTORIAL_CHO,
			new TutorialProgressScreen.LessonAsset
			{
				m_asset = "Tutorial_Lesson2",
				m_phoneAsset = "Tutorial_Lesson2_phone"
			}
		},
		{
			ScenarioDbId.TUTORIAL_MUKLA,
			new TutorialProgressScreen.LessonAsset
			{
				m_asset = "Tutorial_Lesson3"
			}
		},
		{
			ScenarioDbId.TUTORIAL_NESINGWARY,
			new TutorialProgressScreen.LessonAsset
			{
				m_asset = "Tutorial_Lesson4"
			}
		},
		{
			ScenarioDbId.TUTORIAL_ILLIDAN,
			new TutorialProgressScreen.LessonAsset
			{
				m_asset = "Tutorial_Lesson5"
			}
		}
	};

	// Token: 0x040025D6 RID: 9686
	private List<ScenarioDbfRecord> m_sortedMissionRecords = new List<ScenarioDbfRecord>();

	// Token: 0x040025D7 RID: 9687
	private Vector3 START_SCALE = new Vector3(0.5f, 0.5f, 0.5f);

	// Token: 0x040025D8 RID: 9688
	private Vector3 FINAL_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(7f, 1f, 7f),
		Phone = new Vector3(6.1f, 1f, 6.1f)
	};

	// Token: 0x040025D9 RID: 9689
	private Vector3 FINAL_SCALE_OVER_BOX = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(92.5f, 14f, 92.5f),
		Phone = new Vector3(106f, 16f, 106f)
	};

	// Token: 0x040025DA RID: 9690
	private PlatformDependentValue<Vector3> FINAL_POS = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(-8f, 5f, -5f),
		Phone = new Vector3(-8f, 5f, -4.58f)
	};

	// Token: 0x040025DB RID: 9691
	private PlatformDependentValue<Vector3> FINAL_POS_OVER_BOX = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
	{
		PC = new Vector3(0f, 5f, -0.2f),
		Phone = new Vector3(0f, 5f, -2.06f)
	};

	// Token: 0x040025DC RID: 9692
	private Vector3 HERO_COIN_START;

	// Token: 0x040025DD RID: 9693
	private bool IS_TESTING;

	// Token: 0x0200085C RID: 2140
	private class LessonAsset
	{
		// Token: 0x040038A8 RID: 14504
		public string m_asset;

		// Token: 0x040038A9 RID: 14505
		public string m_phoneAsset;
	}
}
