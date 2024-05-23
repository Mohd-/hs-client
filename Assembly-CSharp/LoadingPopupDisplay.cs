using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E1 RID: 737
public class LoadingPopupDisplay : TransitionPopup
{
	// Token: 0x060026B9 RID: 9913 RVA: 0x000BC7BC File Offset: 0x000BA9BC
	protected override void Awake()
	{
		base.Awake();
		this.GenerateTaskNameMap();
		this.m_title.Text = GameStrings.Get("GLUE_STARTING_GAME");
		base.gameObject.transform.localPosition = new Vector3(-0.05f, 8.2f, 3.908f);
		SoundManager.Get().Load("StartGame_window_expand_up");
		SoundManager.Get().Load("StartGame_window_shrink_down");
		SoundManager.Get().Load("StartGame_window_loading_bar_move_down_and_forward");
		SoundManager.Get().Load("StartGame_window_loading_bar_flip");
		SoundManager.Get().Load("StartGame_window_bar_filling_loop");
		SoundManager.Get().Load("StartGame_window_loading_bar_drop");
		this.DisableCancelButton();
	}

	// Token: 0x060026BA RID: 9914 RVA: 0x000BC874 File Offset: 0x000BAA74
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (SoundManager.Get() != null)
		{
			this.StopLoopingSound();
		}
	}

	// Token: 0x060026BB RID: 9915 RVA: 0x000BC892 File Offset: 0x000BAA92
	protected override bool EnableCancelButtonIfPossible()
	{
		if (!base.EnableCancelButtonIfPossible())
		{
			return false;
		}
		TransformUtil.SetLocalPosX(this.m_queueTab, -0.3234057f);
		return true;
	}

	// Token: 0x060026BC RID: 9916 RVA: 0x000BC8B2 File Offset: 0x000BAAB2
	protected override void EnableCancelButton()
	{
		this.m_cancelButtonParent.SetActive(true);
		base.EnableCancelButton();
	}

	// Token: 0x060026BD RID: 9917 RVA: 0x000BC8C6 File Offset: 0x000BAAC6
	protected override void DisableCancelButton()
	{
		base.DisableCancelButton();
		this.m_cancelButtonParent.SetActive(false);
	}

	// Token: 0x060026BE RID: 9918 RVA: 0x000BC8DC File Offset: 0x000BAADC
	protected override void AnimateShow()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			"ShowCancelButton",
			"time",
			30f,
			"ignoretimescale",
			true,
			"oncomplete",
			new Action<object>(this.OnCancelButtonShowTimerCompleted),
			"oncompletetarget",
			base.gameObject
		});
		iTween.Timer(base.gameObject, args);
		this.SetTipOfTheDay();
		this.SetLoadingBarTexture();
		SoundManager.Get().LoadAndPlay("StartGame_window_expand_up");
		base.AnimateShow();
		this.m_stopAnimating = false;
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x060026BF RID: 9919 RVA: 0x000BC99A File Offset: 0x000BAB9A
	protected void OnCancelButtonShowTimerCompleted(object userData)
	{
		this.EnableCancelButtonIfPossible();
	}

	// Token: 0x060026C0 RID: 9920 RVA: 0x000BC9A3 File Offset: 0x000BABA3
	protected override void OnGameError(FindGameEventData eventData)
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x060026C1 RID: 9921 RVA: 0x000BC9B6 File Offset: 0x000BABB6
	protected override void OnGameCanceled(FindGameEventData eventData)
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x060026C2 RID: 9922 RVA: 0x000BC9CC File Offset: 0x000BABCC
	protected override void AnimateHide()
	{
		SoundManager.Get().LoadAndPlay("StartGame_window_shrink_down");
		iTween.StopByName(base.gameObject, "ShowCancelButton");
		if (this.m_barAnimating)
		{
			base.StopCoroutine("AnimateBar");
			this.m_barAnimating = false;
			this.StopLoopingSound();
		}
		base.AnimateHide();
	}

	// Token: 0x060026C3 RID: 9923 RVA: 0x000BCA21 File Offset: 0x000BAC21
	protected override void OnAnimateShowFinished()
	{
		base.OnAnimateShowFinished();
		this.AnimateInLoadingTile();
	}

	// Token: 0x060026C4 RID: 9924 RVA: 0x000BCA30 File Offset: 0x000BAC30
	private void AnimateInLoadingTile()
	{
		if (this.m_stopAnimating)
		{
			this.m_animationStopped = true;
			return;
		}
		this.m_loadingTile.transform.localEulerAngles = new Vector3(180f, 0f, 0f);
		this.m_loadingTile.transform.localPosition = LoadingPopupDisplay.START_POS;
		this.m_progressBar.SetProgressBar(0f);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			LoadingPopupDisplay.MID_POS,
			"isLocal",
			true,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.MoveTo(this.m_loadingTile, args);
		SoundManager.Get().LoadAndPlay("StartGame_window_loading_bar_move_down_and_forward");
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			LoadingPopupDisplay.END_POS,
			"isLocal",
			true,
			"time",
			0.5f,
			"delay",
			0.5f,
			"easetype",
			iTween.EaseType.easeOutCubic
		});
		iTween.MoveTo(this.m_loadingTile, args2);
		Hashtable args3 = iTween.Hash(new object[]
		{
			"amount",
			new Vector3(180f, 0f, 0f),
			"time",
			0.5f,
			"delay",
			0.8f,
			"easeType",
			iTween.EaseType.easeOutElastic,
			"space",
			1,
			"name",
			"flip"
		});
		iTween.RotateAdd(this.m_loadingTile, args3);
		this.m_progressBar.SetLabel(this.GetRandomTaskName());
		base.StartCoroutine("AnimateBar");
	}

	// Token: 0x060026C5 RID: 9925 RVA: 0x000BCC44 File Offset: 0x000BAE44
	private void AnimateOutLoadingTile()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			LoadingPopupDisplay.MID_POS,
			"isLocal",
			true,
			"time",
			0.25f,
			"easetype",
			iTween.EaseType.easeOutBounce
		});
		iTween.MoveTo(this.m_loadingTile, args);
		SoundManager.Get().LoadAndPlay("StartGame_window_loading_bar_drop");
		Hashtable args2 = iTween.Hash(new object[]
		{
			"position",
			LoadingPopupDisplay.OFFSCREEN_POS,
			"isLocal",
			true,
			"time",
			0.25f,
			"delay",
			0.25f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"oncomplete",
			"AnimateInLoadingTile",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_loadingTile, args2);
	}

	// Token: 0x060026C6 RID: 9926 RVA: 0x000BCD65 File Offset: 0x000BAF65
	private float GetRandomTaskDuration()
	{
		return 1f + Random.value * 2f;
	}

	// Token: 0x060026C7 RID: 9927 RVA: 0x000BCD78 File Offset: 0x000BAF78
	private string GetRandomTaskName()
	{
		List<string> list;
		if (GameMgr.Get().IsSpectator())
		{
			list = this.m_spectatorTaskNameMap;
		}
		else if (!this.m_taskNameMap.TryGetValue(this.m_adventureId, out list))
		{
			list = this.m_taskNameMap[AdventureDbId.INVALID];
		}
		if (list.Count == 0)
		{
			return "ERROR - OUT OF TASK NAMES!!!";
		}
		int num = Random.Range(0, list.Count);
		return list[num];
	}

	// Token: 0x060026C8 RID: 9928 RVA: 0x000BCDEC File Offset: 0x000BAFEC
	private IEnumerator AnimateBar()
	{
		this.m_barAnimating = true;
		yield return new WaitForSeconds(0.8f);
		SoundManager.Get().LoadAndPlay("StartGame_window_loading_bar_flip");
		yield return new WaitForSeconds(0.19999999f);
		float timeToAnimate = this.GetRandomTaskDuration();
		this.m_progressBar.m_increaseAnimTime = timeToAnimate;
		this.m_progressBar.AnimateProgress(0f, 1f);
		SoundManager.Get().LoadAndPlay("StartGame_window_bar_filling_loop", null, 1f, new SoundManager.LoadedCallback(this.LoopingSoundLoadedCallback));
		yield return new WaitForSeconds(timeToAnimate);
		this.StopLoopingSound();
		this.AnimateOutLoadingTile();
		this.m_barAnimating = false;
		yield break;
	}

	// Token: 0x060026C9 RID: 9929 RVA: 0x000BCE07 File Offset: 0x000BB007
	private void LoopingSoundLoadedCallback(AudioSource source, object userData)
	{
		this.StopLoopingSound();
		if (this.m_barAnimating)
		{
			this.m_loopSound = source;
		}
		else
		{
			SoundManager.Get().Stop(source);
		}
	}

	// Token: 0x060026CA RID: 9930 RVA: 0x000BCE32 File Offset: 0x000BB032
	protected override void OnGameplaySceneLoaded()
	{
		base.StartCoroutine(this.StopLoading());
		Navigation.Clear();
	}

	// Token: 0x060026CB RID: 9931 RVA: 0x000BCE48 File Offset: 0x000BB048
	private IEnumerator StopLoading()
	{
		this.m_stopAnimating = true;
		while (!this.m_animationStopped)
		{
			yield return null;
		}
		if (this.m_adventureId == AdventureDbId.PRACTICE)
		{
			int practiceProgress = Options.Get().GetInt(Option.TIP_PRACTICE_PROGRESS, 0);
			Options.Get().SetInt(Option.TIP_PRACTICE_PROGRESS, practiceProgress + 1);
		}
		this.AnimateHide();
		yield break;
	}

	// Token: 0x060026CC RID: 9932 RVA: 0x000BCE63 File Offset: 0x000BB063
	private void StopLoopingSound()
	{
		SoundManager.Get().Stop(this.m_loopSound);
		this.m_loopSound = null;
	}

	// Token: 0x060026CD RID: 9933 RVA: 0x000BCE80 File Offset: 0x000BB080
	private bool OnNavigateBack()
	{
		if (!this.m_cancelButtonParent.gameObject.activeSelf)
		{
			return false;
		}
		base.StartCoroutine(this.StopLoading());
		base.FireMatchCanceledEvent();
		return true;
	}

	// Token: 0x060026CE RID: 9934 RVA: 0x000BCEB8 File Offset: 0x000BB0B8
	protected override void OnCancelButtonReleased(UIEvent e)
	{
		base.OnCancelButtonReleased(e);
		Navigation.GoBack();
	}

	// Token: 0x060026CF RID: 9935 RVA: 0x000BCEC8 File Offset: 0x000BB0C8
	private void GenerateTaskNameMap()
	{
		this.GenerateTaskNamesForAdventure(AdventureDbId.INVALID, "GLUE_LOADING_BAR_TASK_");
		this.GenerateTaskNamesForAdventure(AdventureDbId.NAXXRAMAS, "GLUE_NAXX_LOADING_BAR_TASK_");
		this.GenerateTaskNamesForAdventure(AdventureDbId.BRM, "GLUE_BRM_LOADING_BAR_TASK_");
		this.GenerateTaskNamesForAdventure(AdventureDbId.LOE, "GLUE_LOE_LOADING_BAR_TASK_");
		this.GenerateTaskNamesForPrefix(this.m_spectatorTaskNameMap, "GLUE_SPECTATOR_LOADING_BAR_TASK_");
	}

	// Token: 0x060026D0 RID: 9936 RVA: 0x000BCF18 File Offset: 0x000BB118
	private void GenerateTaskNamesForAdventure(AdventureDbId adventureId, string prefix)
	{
		List<string> list = new List<string>();
		this.GenerateTaskNamesForPrefix(list, prefix);
		this.m_taskNameMap[adventureId] = list;
	}

	// Token: 0x060026D1 RID: 9937 RVA: 0x000BCF40 File Offset: 0x000BB140
	private void GenerateTaskNamesForPrefix(List<string> taskNames, string prefix)
	{
		taskNames.Clear();
		for (int i = 1; i < 100; i++)
		{
			string text = prefix + i;
			string text2 = GameStrings.Get(text);
			if (text2 == text)
			{
				break;
			}
			taskNames.Add(text2);
		}
	}

	// Token: 0x060026D2 RID: 9938 RVA: 0x000BCF94 File Offset: 0x000BB194
	private void SetTipOfTheDay()
	{
		if (this.m_adventureId == AdventureDbId.PRACTICE)
		{
			this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.PRACTICE, Options.Get().GetInt(Option.TIP_PRACTICE_PROGRESS, 0), TipCategory.DEFAULT);
		}
		else if (GameUtils.IsExpansionAdventure(this.m_adventureId))
		{
			this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.ADVENTURE);
		}
		else
		{
			this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.DEFAULT);
		}
	}

	// Token: 0x060026D3 RID: 9939 RVA: 0x000BD008 File Offset: 0x000BB208
	private void SetLoadingBarTexture()
	{
		Texture texture = this.m_barTextures[0].texture;
		foreach (LoadingPopupDisplay.LoadingbarTexture loadingbarTexture in this.m_barTextures)
		{
			if (loadingbarTexture.adventureID == this.m_adventureId)
			{
				texture = loadingbarTexture.texture;
				this.m_progressBar.m_barIntensity = loadingbarTexture.m_barIntensity;
				this.m_progressBar.m_barIntensityIncreaseMax = loadingbarTexture.m_barIntensityIncreaseMax;
			}
		}
		this.m_progressBar.SetBarTexture(texture);
	}

	// Token: 0x04001711 RID: 5905
	private const int TASK_DURATION_VARIATION = 2;

	// Token: 0x04001712 RID: 5906
	private const float ROTATION_DURATION = 0.5f;

	// Token: 0x04001713 RID: 5907
	private const float ROTATION_DELAY = 0.5f;

	// Token: 0x04001714 RID: 5908
	private const float SLIDE_IN_TIME = 0.5f;

	// Token: 0x04001715 RID: 5909
	private const float SLIDE_OUT_TIME = 0.25f;

	// Token: 0x04001716 RID: 5910
	private const float RAISE_TIME = 0.5f;

	// Token: 0x04001717 RID: 5911
	private const float LOWER_TIME = 0.25f;

	// Token: 0x04001718 RID: 5912
	private const string SHOW_CANCEL_BUTTON_TWEEN_NAME = "ShowCancelButton";

	// Token: 0x04001719 RID: 5913
	private const float SHOW_CANCEL_BUTTON_THRESHOLD = 30f;

	// Token: 0x0400171A RID: 5914
	public UberText m_tipOfTheDay;

	// Token: 0x0400171B RID: 5915
	public ProgressBar m_progressBar;

	// Token: 0x0400171C RID: 5916
	public GameObject m_loadingTile;

	// Token: 0x0400171D RID: 5917
	public GameObject m_cancelButtonParent;

	// Token: 0x0400171E RID: 5918
	public List<LoadingPopupDisplay.LoadingbarTexture> m_barTextures = new List<LoadingPopupDisplay.LoadingbarTexture>();

	// Token: 0x0400171F RID: 5919
	private Map<AdventureDbId, List<string>> m_taskNameMap = new Map<AdventureDbId, List<string>>();

	// Token: 0x04001720 RID: 5920
	private List<string> m_spectatorTaskNameMap = new List<string>();

	// Token: 0x04001721 RID: 5921
	private bool m_stopAnimating;

	// Token: 0x04001722 RID: 5922
	private bool m_animationStopped;

	// Token: 0x04001723 RID: 5923
	private AudioSource m_loopSound;

	// Token: 0x04001724 RID: 5924
	private bool m_barAnimating;

	// Token: 0x04001725 RID: 5925
	public static readonly Vector3 START_POS = new Vector3(-0.0152f, -0.0894f, -0.0837f);

	// Token: 0x04001726 RID: 5926
	public static readonly Vector3 MID_POS = new Vector3(-0.0152f, -0.0894f, 0.0226f);

	// Token: 0x04001727 RID: 5927
	public static readonly Vector3 END_POS = new Vector3(-0.0152f, 0.0368f, 0.0226f);

	// Token: 0x04001728 RID: 5928
	public static readonly Vector3 OFFSCREEN_POS = new Vector3(-0.0152f, -0.0894f, 0.13f);

	// Token: 0x02000978 RID: 2424
	[Serializable]
	public class LoadingbarTexture
	{
		// Token: 0x04003F20 RID: 16160
		public AdventureDbId adventureID;

		// Token: 0x04003F21 RID: 16161
		public Texture texture;

		// Token: 0x04003F22 RID: 16162
		public float m_barIntensity = 1.2f;

		// Token: 0x04003F23 RID: 16163
		public float m_barIntensityIncreaseMax = 3f;
	}
}
