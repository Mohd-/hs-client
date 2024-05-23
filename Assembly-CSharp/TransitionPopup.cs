using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002DF RID: 735
public abstract class TransitionPopup : MonoBehaviour
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x0600267C RID: 9852 RVA: 0x000BBA23 File Offset: 0x000B9C23
	// (remove) Token: 0x0600267D RID: 9853 RVA: 0x000BBA3C File Offset: 0x000B9C3C
	public event Action<TransitionPopup> OnHidden;

	// Token: 0x0600267E RID: 9854 RVA: 0x000BBA55 File Offset: 0x000B9C55
	public void SetAdventureId(AdventureDbId adventureId)
	{
		this.m_adventureId = adventureId;
	}

	// Token: 0x0600267F RID: 9855 RVA: 0x000BBA60 File Offset: 0x000B9C60
	protected virtual void Awake()
	{
		this.m_fullScreenEffectsCamera = Camera.main;
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelButtonReleased));
		this.m_cancelButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnCancelButtonOver));
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		base.gameObject.transform.localPosition = this.m_startPosition;
	}

	// Token: 0x06002680 RID: 9856 RVA: 0x000BBAE0 File Offset: 0x000B9CE0
	protected virtual void Start()
	{
		if (this.m_fullScreenEffectsCamera == null)
		{
			this.m_fullScreenEffectsCamera = Camera.main;
		}
		if (!this.m_shown)
		{
			iTween.FadeTo(base.gameObject, 0f, 0f);
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06002681 RID: 9857 RVA: 0x000BBB38 File Offset: 0x000B9D38
	protected virtual void OnDestroy()
	{
		if (FullScreenFXMgr.Get())
		{
			this.DisableFullScreenBlur();
		}
		this.StopBlockingTransition();
		GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		if (SceneMgr.Get() != null)
		{
			SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		}
		if (this.m_shown && this.OnHidden != null)
		{
			this.OnHidden.Invoke(this);
		}
	}

	// Token: 0x06002682 RID: 9858 RVA: 0x000BBBC1 File Offset: 0x000B9DC1
	public bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000BBBC9 File Offset: 0x000B9DC9
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		this.AnimateShow();
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000BBBDD File Offset: 0x000B9DDD
	public void Hide()
	{
		if (!this.m_shown)
		{
			return;
		}
		this.AnimateHide();
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000BBBF4 File Offset: 0x000B9DF4
	public void Cancel()
	{
		if (!this.m_shown)
		{
			return;
		}
		if (this.m_fullScreenEffectsCamera == null)
		{
			return;
		}
		this.DisableFullScreenBlur();
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000BBC25 File Offset: 0x000B9E25
	public void RegisterMatchCanceledEvent(TransitionPopup.MatchCanceledEvent callback)
	{
		this.m_matchCanceledListeners.Add(callback);
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000BBC33 File Offset: 0x000B9E33
	public bool UnregisterMatchCanceledEvent(TransitionPopup.MatchCanceledEvent callback)
	{
		return this.m_matchCanceledListeners.Remove(callback);
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x000BBC44 File Offset: 0x000B9E44
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		if (!this.m_shown)
		{
			return false;
		}
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_ERROR:
			this.OnGameError(eventData);
			break;
		case FindGameState.BNET_QUEUE_ENTERED:
			this.OnGameEntered(eventData);
			break;
		case FindGameState.BNET_QUEUE_DELAYED:
			this.OnGameDelayed(eventData);
			break;
		case FindGameState.BNET_QUEUE_UPDATED:
			this.OnGameUpdated(eventData);
			break;
		case FindGameState.BNET_QUEUE_CANCELED:
			this.OnGameCanceled(eventData);
			break;
		case FindGameState.SERVER_GAME_CONNECTING:
			this.OnGameConnecting(eventData);
			break;
		case FindGameState.SERVER_GAME_STARTED:
			this.OnGameStarted(eventData);
			break;
		}
		return false;
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000BBCE7 File Offset: 0x000B9EE7
	protected virtual void OnGameEntered(FindGameEventData eventData)
	{
		this.m_queueTab.UpdateDisplay(eventData.m_queueMinSeconds, eventData.m_queueMaxSeconds);
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000BBD00 File Offset: 0x000B9F00
	protected virtual void OnGameDelayed(FindGameEventData eventData)
	{
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000BBD02 File Offset: 0x000B9F02
	protected virtual void OnGameUpdated(FindGameEventData eventData)
	{
		this.m_queueTab.UpdateDisplay(eventData.m_queueMinSeconds, eventData.m_queueMaxSeconds);
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000BBD1B File Offset: 0x000B9F1B
	protected virtual void OnGameConnecting(FindGameEventData eventData)
	{
		this.DisableCancelButton();
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000BBD23 File Offset: 0x000B9F23
	protected virtual void OnGameStarted(FindGameEventData eventData)
	{
		this.StartBlockingTransition();
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
	}

	// Token: 0x0600268E RID: 9870 RVA: 0x000BBD42 File Offset: 0x000B9F42
	protected virtual void OnGameCanceled(FindGameEventData eventData)
	{
	}

	// Token: 0x0600268F RID: 9871 RVA: 0x000BBD44 File Offset: 0x000B9F44
	protected virtual void OnGameError(FindGameEventData eventData)
	{
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x000BBD48 File Offset: 0x000B9F48
	protected virtual bool EnableCancelButtonIfPossible()
	{
		if (!this.m_showAnimationFinished)
		{
			return false;
		}
		if (GameMgr.Get().IsAboutToStopFindingGame())
		{
			return false;
		}
		if (this.m_cancelButton.IsEnabled())
		{
			return false;
		}
		this.EnableCancelButton();
		return true;
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x000BBD8C File Offset: 0x000B9F8C
	protected virtual void EnableCancelButton()
	{
		this.m_cancelButton.Flip(true);
		this.m_cancelButton.SetEnabled(true);
	}

	// Token: 0x06002692 RID: 9874 RVA: 0x000BBDA6 File Offset: 0x000B9FA6
	protected virtual void DisableCancelButton()
	{
		this.m_cancelButton.Flip(false);
		this.m_cancelButton.SetEnabled(false);
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000BBDC0 File Offset: 0x000B9FC0
	protected virtual void OnCancelButtonReleased(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Back_Click");
		this.m_cancelButton.SetEnabled(false);
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000BBDDD File Offset: 0x000B9FDD
	protected virtual void OnCancelButtonOver(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Mouseover");
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000BBDF0 File Offset: 0x000B9FF0
	protected void FireMatchCanceledEvent()
	{
		TransitionPopup.MatchCanceledEvent[] array = this.m_matchCanceledListeners.ToArray();
		if (array.Length == 0)
		{
			Debug.LogError("TransitionPopup.FireMatchCanceledEvent() - Cancel triggered, but nobody was listening!!");
		}
		foreach (TransitionPopup.MatchCanceledEvent matchCanceledEvent in array)
		{
			matchCanceledEvent();
		}
	}

	// Token: 0x06002696 RID: 9878 RVA: 0x000BBE3C File Offset: 0x000BA03C
	protected virtual void AnimateShow()
	{
		iTween.Stop(base.gameObject);
		this.m_shown = true;
		this.m_showAnimationFinished = false;
		base.gameObject.SetActive(true);
		SceneUtils.EnableRenderers(base.gameObject, false);
		this.DisableCancelButton();
		this.ShowPopup();
		this.AnimateBlurBlendOn();
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000BBE8C File Offset: 0x000BA08C
	protected virtual void ShowPopup()
	{
		SceneUtils.EnableRenderers(base.gameObject, true);
		iTween.FadeTo(base.gameObject, 1f, this.POPUP_TIME);
		base.gameObject.transform.localScale = new Vector3(this.START_SCALE_VAL, this.START_SCALE_VAL, this.START_SCALE_VAL);
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(this.m_endScale, this.m_endScale, this.m_endScale),
			"time",
			this.POPUP_TIME,
			"oncomplete",
			"PunchPopup",
			"oncompletetarget",
			base.gameObject
		});
		iTween.ScaleTo(base.gameObject, args);
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"position",
			base.gameObject.transform.localPosition + new Vector3(0.02f, 0.02f, 0.02f),
			"time",
			1.5f,
			"islocal",
			true
		}));
		this.m_queueTab.ResetTimer();
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000BBFE8 File Offset: 0x000BA1E8
	private void PunchPopup()
	{
		iTween.ScaleTo(base.gameObject, new Vector3(this.m_scaleAfterPunch, this.m_scaleAfterPunch, this.m_scaleAfterPunch), 0.15f);
		this.OnAnimateShowFinished();
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000BC031 File Offset: 0x000BA231
	protected virtual void OnAnimateShowFinished()
	{
		this.m_showAnimationFinished = true;
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000BC03C File Offset: 0x000BA23C
	protected virtual void AnimateHide()
	{
		this.m_shown = false;
		this.DisableCancelButton();
		iTween.FadeTo(base.gameObject, 0f, this.POPUP_TIME);
		Hashtable hashtable = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(this.START_SCALE_VAL, this.START_SCALE_VAL, this.START_SCALE_VAL),
			"time",
			this.POPUP_TIME
		});
		if (this.OnHidden != null)
		{
			hashtable["oncomplete"] = delegate(object data)
			{
				this.OnHidden.Invoke(this);
			};
		}
		iTween.ScaleTo(base.gameObject, hashtable);
		this.AnimateBlurBlendOff();
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000BC0E9 File Offset: 0x000BA2E9
	private void AnimateBlurBlendOn()
	{
		this.EnableFullScreenBlur();
	}

	// Token: 0x0600269C RID: 9884 RVA: 0x000BC0F1 File Offset: 0x000BA2F1
	protected void AnimateBlurBlendOff()
	{
		this.DisableFullScreenBlur();
		base.StartCoroutine(this.DelayDeactivatePopup(0.5f));
	}

	// Token: 0x0600269D RID: 9885 RVA: 0x000BC10C File Offset: 0x000BA30C
	private IEnumerator DelayDeactivatePopup(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		if (this.m_shown)
		{
			yield break;
		}
		this.DeactivatePopup();
		yield break;
	}

	// Token: 0x0600269E RID: 9886 RVA: 0x000BC135 File Offset: 0x000BA335
	protected void DeactivatePopup()
	{
		base.gameObject.SetActive(false);
		this.StopBlockingTransition();
	}

	// Token: 0x0600269F RID: 9887 RVA: 0x000BC14C File Offset: 0x000BA34C
	protected void StartBlockingTransition()
	{
		this.m_blockingLoadingScreen = true;
		LoadingScreen.Get().AddTransitionBlocker();
		LoadingScreen.Get().AddTransitionObject(base.gameObject);
	}

	// Token: 0x060026A0 RID: 9888 RVA: 0x000BC17C File Offset: 0x000BA37C
	protected void StopBlockingTransition()
	{
		if (!this.m_blockingLoadingScreen)
		{
			return;
		}
		this.m_blockingLoadingScreen = false;
		if (LoadingScreen.Get())
		{
			LoadingScreen.Get().NotifyTransitionBlockerComplete();
		}
	}

	// Token: 0x060026A1 RID: 9889 RVA: 0x000BC1B8 File Offset: 0x000BA3B8
	protected virtual void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (!this.m_shown)
		{
			return;
		}
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		this.OnGameplaySceneLoaded();
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000BC1F0 File Offset: 0x000BA3F0
	private void EnableFullScreenBlur()
	{
		if (this.m_blurEnabled)
		{
			return;
		}
		this.m_blurEnabled = true;
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.SetBlurAmount(0.3f);
		fullScreenFXMgr.SetBlurBrightness(0.4f);
		fullScreenFXMgr.SetBlurDesaturation(0.5f);
		fullScreenFXMgr.Blur(1f, 0.5f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000BC24A File Offset: 0x000BA44A
	private void DisableFullScreenBlur()
	{
		if (!this.m_blurEnabled)
		{
			return;
		}
		this.m_blurEnabled = false;
		FullScreenFXMgr.Get().StopBlur(0.5f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x060026A4 RID: 9892
	protected abstract void OnGameplaySceneLoaded();

	// Token: 0x040016FA RID: 5882
	public UberText m_title;

	// Token: 0x040016FB RID: 5883
	public MatchingQueueTab m_queueTab;

	// Token: 0x040016FC RID: 5884
	public UIBButton m_cancelButton;

	// Token: 0x040016FD RID: 5885
	public Vector3_MobileOverride m_startPosition = new Vector3_MobileOverride(new Vector3(-0.05f, 8.2f, -1.8f));

	// Token: 0x040016FE RID: 5886
	public Float_MobileOverride m_endScale;

	// Token: 0x040016FF RID: 5887
	public Float_MobileOverride m_scaleAfterPunch;

	// Token: 0x04001700 RID: 5888
	protected bool m_shown;

	// Token: 0x04001701 RID: 5889
	protected bool m_blockingLoadingScreen;

	// Token: 0x04001702 RID: 5890
	protected Camera m_fullScreenEffectsCamera;

	// Token: 0x04001703 RID: 5891
	protected List<TransitionPopup.MatchCanceledEvent> m_matchCanceledListeners = new List<TransitionPopup.MatchCanceledEvent>();

	// Token: 0x04001704 RID: 5892
	protected AdventureDbId m_adventureId;

	// Token: 0x04001705 RID: 5893
	protected bool m_showAnimationFinished;

	// Token: 0x04001706 RID: 5894
	private float POPUP_TIME = 0.3f;

	// Token: 0x04001707 RID: 5895
	private float START_SCALE_VAL = 0.1f;

	// Token: 0x04001708 RID: 5896
	private Vector3 END_POSITION;

	// Token: 0x04001709 RID: 5897
	private bool m_blurEnabled;

	// Token: 0x02000325 RID: 805
	// (Invoke) Token: 0x060029F8 RID: 10744
	public delegate void MatchCanceledEvent();
}
