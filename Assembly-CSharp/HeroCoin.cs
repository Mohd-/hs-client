using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200069A RID: 1690
public class HeroCoin : PegUIElement
{
	// Token: 0x0600473F RID: 18239 RVA: 0x00155C22 File Offset: 0x00153E22
	protected override void Awake()
	{
		base.Awake();
		this.m_tooltip.SetActive(false);
	}

	// Token: 0x06004740 RID: 18240 RVA: 0x00155C36 File Offset: 0x00153E36
	private void Start()
	{
		this.m_coinLabel.Text = GameStrings.Get("GLOBAL_PLAY");
	}

	// Token: 0x06004741 RID: 18241 RVA: 0x00155C4D File Offset: 0x00153E4D
	private void OnDestroy()
	{
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
		}
	}

	// Token: 0x06004742 RID: 18242 RVA: 0x00155C76 File Offset: 0x00153E76
	public int GetMissionId()
	{
		return this.m_missionID;
	}

	// Token: 0x06004743 RID: 18243 RVA: 0x00155C80 File Offset: 0x00153E80
	public void SetCoinInfo(Vector2 goldTexture, Vector2 grayTexture, Vector2 crackTexture, int missionID)
	{
		this.m_material = base.GetComponent<Renderer>().materials[0];
		this.m_grayMaterial = base.GetComponent<Renderer>().materials[1];
		this.m_goldTexture = goldTexture;
		this.m_material.mainTextureOffset = this.m_goldTexture;
		this.m_grayTexture = grayTexture;
		this.m_grayMaterial.mainTextureOffset = this.m_grayTexture;
		this.m_crackTexture = crackTexture;
		this.m_cracks.GetComponent<Renderer>().material.mainTextureOffset = this.m_crackTexture;
		this.m_missionID = missionID;
		this.m_tooltipText.Text = GameUtils.GetMissionHeroName(missionID);
		this.m_originalPosition = base.transform.localPosition;
		this.m_originalXPosition = this.m_coinX.transform.localPosition;
	}

	// Token: 0x06004744 RID: 18244 RVA: 0x00155D46 File Offset: 0x00153F46
	public void SetCoinPressCallback(HeroCoin.CoinPressCallback callback)
	{
		this.m_coinPressCallback = callback;
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x00155D4F File Offset: 0x00153F4F
	public void SetLessonAssetName(string lessonAssetName)
	{
		this.m_lessonAssetName = lessonAssetName;
	}

	// Token: 0x06004746 RID: 18246 RVA: 0x00155D58 File Offset: 0x00153F58
	public string GetLessonAssetName()
	{
		return this.m_lessonAssetName;
	}

	// Token: 0x06004747 RID: 18247 RVA: 0x00155D60 File Offset: 0x00153F60
	public void SetProgress(HeroCoin.CoinStatus status)
	{
		base.gameObject.SetActive(true);
		this.m_currentStatus = status;
		if (status == HeroCoin.CoinStatus.DEFEATED)
		{
			this.m_material.mainTextureOffset = this.m_grayTexture;
			this.m_cracks.SetActive(true);
			this.m_coinX.SetActive(true);
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			this.m_inputEnabled = false;
			this.m_coinLabel.gameObject.SetActive(false);
		}
		else if (status == HeroCoin.CoinStatus.ACTIVE)
		{
			this.m_cracks.SetActive(false);
			this.m_coinX.SetActive(false);
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			this.m_inputEnabled = true;
		}
		else if (status == HeroCoin.CoinStatus.UNREVEALED_TO_ACTIVE)
		{
			this.OnUpdateAlphaVal(0f);
			Hashtable args = iTween.Hash(new object[]
			{
				"y",
				3f,
				"z",
				this.m_originalPosition.z - 0.2f,
				"time",
				0.5f,
				"isLocal",
				true,
				"easetype",
				iTween.EaseType.easeOutCubic
			});
			iTween.MoveTo(base.gameObject, args);
			Hashtable args2 = iTween.Hash(new object[]
			{
				"rotation",
				new Vector3(0f, 0f, 0f),
				"time",
				1f,
				"isLocal",
				true,
				"delay",
				0.5f,
				"easetype",
				iTween.EaseType.easeOutElastic
			});
			iTween.RotateTo(base.gameObject, args2);
			Hashtable args3 = iTween.Hash(new object[]
			{
				"y",
				this.m_originalPosition.y,
				"z",
				this.m_originalPosition.z,
				"time",
				0.5f,
				"isLocal",
				true,
				"delay",
				1.75f,
				"easetype",
				iTween.EaseType.easeOutCubic
			});
			iTween.MoveTo(base.gameObject, args3);
			Hashtable args4 = iTween.Hash(new object[]
			{
				"from",
				0,
				"to",
				1,
				"time",
				0.25f,
				"delay",
				1.5f,
				"easetype",
				iTween.EaseType.easeOutCirc,
				"onupdate",
				"OnUpdateAlphaVal",
				"oncomplete",
				"EnableInput",
				"oncompletetarget",
				base.gameObject
			});
			iTween.ValueTo(base.gameObject, args4);
			SoundManager.Get().LoadAndPlay("tutorial_mission_hero_coin_rises", base.gameObject);
			base.StartCoroutine(this.ShowCoinText());
			this.m_inputEnabled = false;
		}
		else if (status == HeroCoin.CoinStatus.ACTIVE_TO_DEFEATED)
		{
			this.m_coinX.transform.localPosition = new Vector3(0f, 10f, (!UniversalInputManager.UsePhoneUI) ? -0.23f : 0f);
			this.m_cracks.SetActive(true);
			this.m_coinX.SetActive(true);
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			this.m_inputEnabled = false;
			RenderUtils.SetAlpha(this.m_coinX, 0f);
			RenderUtils.SetAlpha(this.m_cracks, 0f);
			Hashtable args5 = iTween.Hash(new object[]
			{
				"y",
				this.m_originalXPosition.y,
				"z",
				this.m_originalXPosition.z,
				"time",
				0.25f,
				"isLocal",
				true,
				"easetype",
				iTween.EaseType.easeInCirc
			});
			iTween.MoveTo(this.m_coinX, args5);
			Hashtable args6 = iTween.Hash(new object[]
			{
				"amount",
				1,
				"delay",
				0,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeInCirc
			});
			iTween.FadeTo(this.m_coinX, args6);
			Hashtable args7 = iTween.Hash(new object[]
			{
				"amount",
				1,
				"delay",
				0.15f,
				"time",
				0.25f,
				"easeType",
				iTween.EaseType.easeInCirc
			});
			iTween.FadeTo(this.m_cracks, args7);
			SoundManager.Get().LoadAndPlay("tutorial_mission_x_descend", base.gameObject);
			base.StartCoroutine(this.SwitchCoinToGray());
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
			{
				GameState.Get().GetGameEntity().NotifyOfDefeatCoinAnimation();
			}
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			this.m_coinLabel.gameObject.SetActive(false);
			this.m_cracks.SetActive(false);
			this.m_coinX.SetActive(false);
			this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
			this.m_inputEnabled = false;
		}
	}

	// Token: 0x06004748 RID: 18248 RVA: 0x0015633C File Offset: 0x0015453C
	public IEnumerator ShowCoinText()
	{
		yield return new WaitForSeconds(1.5f);
		this.m_coinLabel.gameObject.SetActive(true);
		yield break;
	}

	// Token: 0x06004749 RID: 18249 RVA: 0x00156358 File Offset: 0x00154558
	public IEnumerator SwitchCoinToGray()
	{
		yield return new WaitForSeconds(0.25f);
		this.m_material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
		this.m_coinLabel.gameObject.SetActive(false);
		iTween.ShakePosition(Camera.main.gameObject, iTween.Hash(new object[]
		{
			"amount",
			new Vector3(0.2f, 0.2f, 0.2f),
			"name",
			"HeroCoin",
			"time",
			0.5f,
			"delay",
			0,
			"axis",
			"none"
		}));
		yield break;
	}

	// Token: 0x0600474A RID: 18250 RVA: 0x00156373 File Offset: 0x00154573
	public void HideTooltip()
	{
		this.m_tooltip.SetActive(false);
	}

	// Token: 0x0600474B RID: 18251 RVA: 0x00156381 File Offset: 0x00154581
	public void OnUpdateAlphaVal(float val)
	{
		this.m_material.SetColor("_Color", new Color(1f, 1f, 1f, val));
	}

	// Token: 0x0600474C RID: 18252 RVA: 0x001563A8 File Offset: 0x001545A8
	public void EnableInput()
	{
		this.m_inputEnabled = true;
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
	}

	// Token: 0x0600474D RID: 18253 RVA: 0x001563C0 File Offset: 0x001545C0
	public void FinishIntroScaling()
	{
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
		this.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
		this.m_originalMiddleWidth = this.m_middle.GetComponent<Renderer>().bounds.size.x;
	}

	// Token: 0x0600474E RID: 18254 RVA: 0x00156430 File Offset: 0x00154630
	private void ShowTooltip()
	{
		if (this.m_currentStatus == HeroCoin.CoinStatus.UNREVEALED)
		{
			return;
		}
		this.m_tooltip.SetActive(true);
		float num = 0f;
		float num2 = this.m_tooltipText.GetTextWorldSpaceBounds().size.x + num;
		float x = num2 / this.m_originalMiddleWidth;
		TransformUtil.SetLocalScaleX(this.m_middle, x);
		float num3 = this.m_originalMiddleWidth * 0.223f;
		float num4 = this.m_originalMiddleWidth * 0.01f;
		TransformUtil.SetPoint(this.m_leftCap, Anchor.RIGHT, this.m_middle, Anchor.LEFT, new Vector3(num3, 0f, num4));
		TransformUtil.SetPoint(this.m_rightCap, Anchor.LEFT, this.m_middle, Anchor.RIGHT, new Vector3(-num3, 0f, num4));
	}

	// Token: 0x0600474F RID: 18255 RVA: 0x001564F0 File Offset: 0x001546F0
	private void OnOver(UIEvent e)
	{
		if (this.m_nextTutorialStarted)
		{
			return;
		}
		this.ShowTooltip();
		if (!this.m_inputEnabled)
		{
			return;
		}
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_MOUSE_OVER);
		SoundManager.Get().LoadAndPlay("tutorial_mission_hero_coin_mouse_over", base.gameObject);
	}

	// Token: 0x06004750 RID: 18256 RVA: 0x00156540 File Offset: 0x00154740
	private void OnOut(UIEvent e)
	{
		this.HideTooltip();
		if (this.m_nextTutorialStarted)
		{
			return;
		}
		if (!this.m_inputEnabled)
		{
			return;
		}
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
	}

	// Token: 0x06004751 RID: 18257 RVA: 0x0015657C File Offset: 0x0015477C
	private void OnPress(UIEvent e)
	{
		if (!this.m_inputEnabled)
		{
			return;
		}
		if (this.m_nextTutorialStarted)
		{
			return;
		}
		this.m_inputEnabled = false;
		SoundManager.Get().LoadAndPlay("tutorial_mission_hero_coin_play_select", base.gameObject);
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		if (this.m_coinPressCallback != null)
		{
			this.m_coinPressCallback();
			return;
		}
		LoadingScreen.Get().AddTransitionBlocker();
		LoadingScreen.Get().AddTransitionObject(base.gameObject);
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
		this.StartNextTutorial();
		PlayMakerFSM componentInChildren = base.GetComponentInChildren<PlayMakerFSM>();
		componentInChildren.SendEvent("Action");
	}

	// Token: 0x06004752 RID: 18258 RVA: 0x0015662C File Offset: 0x0015482C
	private void OnGameLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		PlayMakerFSM componentInChildren = base.GetComponentInChildren<PlayMakerFSM>();
		componentInChildren.SendEvent("Death");
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameLoaded));
		base.StartCoroutine(this.WaitThenTransition());
	}

	// Token: 0x06004753 RID: 18259 RVA: 0x00156670 File Offset: 0x00154870
	private IEnumerator WaitThenTransition()
	{
		yield return new WaitForSeconds(1.25f);
		LoadingScreen.Get().NotifyTransitionBlockerComplete();
		yield break;
	}

	// Token: 0x06004754 RID: 18260 RVA: 0x00156684 File Offset: 0x00154884
	private void StartNextTutorial()
	{
		this.m_nextTutorialStarted = true;
		GameMgr.Get().FindGame(4, this.m_missionID, 0L, 0L);
	}

	// Token: 0x04002E3E RID: 11838
	public GameObject m_coin;

	// Token: 0x04002E3F RID: 11839
	public GameObject m_coinX;

	// Token: 0x04002E40 RID: 11840
	public GameObject m_cracks;

	// Token: 0x04002E41 RID: 11841
	public HighlightState m_highlight;

	// Token: 0x04002E42 RID: 11842
	public UberText m_coinLabel;

	// Token: 0x04002E43 RID: 11843
	public GameObject m_tooltip;

	// Token: 0x04002E44 RID: 11844
	public UberText m_tooltipText;

	// Token: 0x04002E45 RID: 11845
	public GameObject m_leftCap;

	// Token: 0x04002E46 RID: 11846
	public GameObject m_rightCap;

	// Token: 0x04002E47 RID: 11847
	public GameObject m_middle;

	// Token: 0x04002E48 RID: 11848
	public GameObject m_explosionPrefab;

	// Token: 0x04002E49 RID: 11849
	public bool m_inputEnabled;

	// Token: 0x04002E4A RID: 11850
	private Vector2 m_goldTexture;

	// Token: 0x04002E4B RID: 11851
	private Vector2 m_grayTexture;

	// Token: 0x04002E4C RID: 11852
	private Vector2 m_crackTexture;

	// Token: 0x04002E4D RID: 11853
	private string m_lessonAssetName;

	// Token: 0x04002E4E RID: 11854
	private Vector2 m_lessonCoords;

	// Token: 0x04002E4F RID: 11855
	private int m_missionID;

	// Token: 0x04002E50 RID: 11856
	private HeroCoin.CoinStatus m_currentStatus;

	// Token: 0x04002E51 RID: 11857
	private float m_originalMiddleWidth;

	// Token: 0x04002E52 RID: 11858
	private Vector3 m_originalPosition;

	// Token: 0x04002E53 RID: 11859
	private Material m_material;

	// Token: 0x04002E54 RID: 11860
	private Material m_grayMaterial;

	// Token: 0x04002E55 RID: 11861
	private Vector3 m_originalXPosition;

	// Token: 0x04002E56 RID: 11862
	private bool m_nextTutorialStarted;

	// Token: 0x04002E57 RID: 11863
	private HeroCoin.CoinPressCallback m_coinPressCallback;

	// Token: 0x0200069B RID: 1691
	// (Invoke) Token: 0x06004756 RID: 18262
	public delegate void CoinPressCallback();

	// Token: 0x0200085E RID: 2142
	public enum CoinStatus
	{
		// Token: 0x040038AF RID: 14511
		ACTIVE,
		// Token: 0x040038B0 RID: 14512
		DEFEATED,
		// Token: 0x040038B1 RID: 14513
		UNREVEALED,
		// Token: 0x040038B2 RID: 14514
		UNREVEALED_TO_ACTIVE,
		// Token: 0x040038B3 RID: 14515
		ACTIVE_TO_DEFEATED
	}
}
