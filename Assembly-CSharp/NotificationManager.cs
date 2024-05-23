using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class NotificationManager : MonoBehaviour
{
	// Token: 0x1700031D RID: 797
	// (get) Token: 0x0600162B RID: 5675 RVA: 0x00069326 File Offset: 0x00067526
	public static Vector3 NOTIFICATITON_WORLD_SCALE
	{
		get
		{
			return (!UniversalInputManager.UsePhoneUI) ? (18f * Vector3.one) : (25f * Vector3.one);
		}
	}

	// Token: 0x0600162C RID: 5676 RVA: 0x0006935A File Offset: 0x0006755A
	private void Awake()
	{
		NotificationManager.s_instance = this;
		this.m_quotesThisSession = new List<string>();
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x0006936D File Offset: 0x0006756D
	private void OnDestroy()
	{
		NotificationManager.s_instance = null;
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x00069375 File Offset: 0x00067575
	private void Start()
	{
		this.notificationsToDestroyUponNewNotifier = new List<Notification>();
		this.arrows = new List<Notification>();
		this.popUpTexts = new List<Notification>();
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x00069398 File Offset: 0x00067598
	public static NotificationManager Get()
	{
		return NotificationManager.s_instance;
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000693A0 File Offset: 0x000675A0
	public Notification CreatePopupDialog(string headlineText, string bodyText, string yesOrOKButtonText, string noButtonText)
	{
		return this.CreatePopupDialog(headlineText, bodyText, yesOrOKButtonText, noButtonText, new Vector3(0f, 0f, 0f));
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000693CC File Offset: 0x000675CC
	public Notification CreatePopupDialog(string headlineText, string bodyText, string yesOrOKButtonText, string noButtonText, Vector3 offset)
	{
		if (this.popUpDialog != null)
		{
			Object.Destroy(this.popUpDialog.gameObject);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.dialogBoxPrefab);
		Vector3 position = Camera.main.transform.position;
		gameObject.transform.position = position + new Vector3(-0.07040818f, -16.10709f, 1.79612f) + offset;
		this.popUpDialog = gameObject.GetComponent<Notification>();
		this.popUpDialog.ChangeDialogText(headlineText, bodyText, yesOrOKButtonText, noButtonText);
		this.popUpDialog.PlayBirth();
		UniversalInputManager.Get().SetGameDialogActive(true);
		return this.popUpDialog;
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x0006947A File Offset: 0x0006767A
	public Notification CreateSpeechBubble(string speechText, Actor actor)
	{
		return this.CreateSpeechBubble(speechText, Notification.SpeechBubbleDirection.BottomLeft, actor, false, true);
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00069487 File Offset: 0x00067687
	public Notification CreateSpeechBubble(string speechText, Actor actor, bool bDestroyWhenNewCreated)
	{
		return this.CreateSpeechBubble(speechText, Notification.SpeechBubbleDirection.BottomLeft, actor, bDestroyWhenNewCreated, true);
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x00069494 File Offset: 0x00067694
	public Notification CreateSpeechBubble(string speechText, Notification.SpeechBubbleDirection direction, Actor actor)
	{
		return this.CreateSpeechBubble(speechText, direction, actor, false, true);
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x000694A4 File Offset: 0x000676A4
	public Notification CreateSpeechBubble(string speechText, Notification.SpeechBubbleDirection direction, Actor actor, bool bDestroyWhenNewCreated, bool parentToActor = true)
	{
		this.DestroyOtherNotifications(direction);
		Notification component;
		if (speechText == string.Empty)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.speechIndicatorPrefab);
			component = gameObject.GetComponent<Notification>();
			component.PlaySmallBirthForFakeBubble();
			component.SetPositionForSmallBubble(actor);
		}
		else
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.speechBubblePrefab);
			component = gameObject.GetComponent<Notification>();
			component.ChangeText(speechText);
			component.FaceDirection(direction);
			component.PlayBirth();
			component.SetPosition(actor, direction);
		}
		if (bDestroyWhenNewCreated)
		{
			this.notificationsToDestroyUponNewNotifier.Add(component);
		}
		if (parentToActor)
		{
			component.transform.parent = actor.transform;
		}
		return component;
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00069548 File Offset: 0x00067748
	public Notification CreateBouncingArrow(UserAttentionBlocker blocker, bool addToList)
	{
		if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManger.CreateBouncingArrow"))
		{
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.bounceArrowPrefab);
		Notification component = gameObject.GetComponent<Notification>();
		component.PlayBirth();
		if (addToList)
		{
			this.arrows.Add(component);
		}
		return component;
	}

	// Token: 0x06001637 RID: 5687 RVA: 0x000695A2 File Offset: 0x000677A2
	public Notification CreateBouncingArrow(UserAttentionBlocker blocker, Vector3 position, Vector3 rotation)
	{
		return this.CreateBouncingArrow(blocker, position, rotation, true);
	}

	// Token: 0x06001638 RID: 5688 RVA: 0x000695B0 File Offset: 0x000677B0
	public Notification CreateBouncingArrow(UserAttentionBlocker blocker, Vector3 position, Vector3 rotation, bool addToList)
	{
		if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManger.CreateBouncingArrow"))
		{
			return null;
		}
		Notification notification = this.CreateBouncingArrow(blocker, addToList);
		notification.transform.position = position;
		notification.transform.localEulerAngles = rotation;
		return notification;
	}

	// Token: 0x06001639 RID: 5689 RVA: 0x00069604 File Offset: 0x00067804
	public Notification CreateFadeArrow(bool addToList)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.fadeArrowPrefab);
		Notification component = gameObject.GetComponent<Notification>();
		component.PlayBirth();
		if (addToList)
		{
			this.arrows.Add(component);
		}
		return component;
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x0006963D File Offset: 0x0006783D
	public Notification CreateFadeArrow(Vector3 position, Vector3 rotation)
	{
		return this.CreateFadeArrow(position, rotation, true);
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x00069648 File Offset: 0x00067848
	public Notification CreateFadeArrow(Vector3 position, Vector3 rotation, bool addToList)
	{
		Notification notification = this.CreateFadeArrow(addToList);
		notification.transform.position = position;
		notification.transform.localEulerAngles = rotation;
		return notification;
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x00069678 File Offset: 0x00067878
	public Notification CreatePopupText(UserAttentionBlocker blocker, Transform bone, string text, bool convertLegacyPosition = true)
	{
		if (convertLegacyPosition)
		{
			return this.CreatePopupText(blocker, bone.position, bone.localScale, text, convertLegacyPosition);
		}
		return this.CreatePopupText(blocker, bone.localPosition, bone.localScale, text, convertLegacyPosition);
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x000696BC File Offset: 0x000678BC
	public Notification CreatePopupText(UserAttentionBlocker blocker, Vector3 position, Vector3 scale, string text, bool convertLegacyPosition = true)
	{
		if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManager.CreatePopupText"))
		{
			return null;
		}
		Vector3 localPosition = position;
		if (convertLegacyPosition)
		{
			Camera camera;
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
			{
				camera = BoardCameras.Get().GetComponentInChildren<Camera>();
			}
			else
			{
				camera = Box.Get().GetBoxCamera().GetComponent<Camera>();
			}
			localPosition = OverlayUI.Get().GetRelativePosition(position, camera, OverlayUI.Get().m_heightScale.m_Center, 0f);
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.popupTextPrefab);
		SceneUtils.SetLayer(gameObject, GameLayer.UI);
		gameObject.transform.localPosition = localPosition;
		gameObject.transform.localScale = scale;
		OverlayUI.Get().AddGameObject(gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		Notification component = gameObject.GetComponent<Notification>();
		component.ChangeText(text);
		component.PlayBirth();
		this.popUpTexts.Add(component);
		return component;
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x0600163E RID: 5694 RVA: 0x0006979F File Offset: 0x0006799F
	public bool IsQuotePlaying
	{
		get
		{
			return this.m_quote != null;
		}
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000697AD File Offset: 0x000679AD
	public Notification CreateInnkeeperQuote(UserAttentionBlocker blocker, string text, string soundName, float durationSeconds = 0f, Action finishCallback = null)
	{
		return this.CreateInnkeeperQuote(blocker, NotificationManager.DEFAULT_CHARACTER_POS, text, soundName, durationSeconds, finishCallback);
	}

	// Token: 0x06001640 RID: 5696 RVA: 0x000697C1 File Offset: 0x000679C1
	public Notification CreateInnkeeperQuote(UserAttentionBlocker blocker, string text, string soundName, Action finishCallback)
	{
		return this.CreateInnkeeperQuote(blocker, NotificationManager.DEFAULT_CHARACTER_POS, text, soundName, 0f, finishCallback);
	}

	// Token: 0x06001641 RID: 5697 RVA: 0x000697D8 File Offset: 0x000679D8
	public Notification CreateInnkeeperQuote(UserAttentionBlocker blocker, Vector3 position, string text, string soundName, float durationSeconds = 0f, Action finishCallback = null)
	{
		if (!SceneMgr.Get().IsInGame() && !UserAttentionManager.CanShowAttentionGrabber(blocker, "NotificationManager.CreateInnkeeperQuote"))
		{
			if (finishCallback != null)
			{
				finishCallback.Invoke();
			}
			return null;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.innkeeperQuotePrefab);
		Notification component = gameObject.GetComponent<Notification>();
		if (finishCallback != null)
		{
			Notification notification = component;
			notification.OnFinishDeathState = (Action)Delegate.Combine(notification.OnFinishDeathState, finishCallback);
		}
		this.PlayCharacterQuote(component, position, text, soundName, durationSeconds, CanvasAnchor.BOTTOM_LEFT);
		return component;
	}

	// Token: 0x06001642 RID: 5698 RVA: 0x00069855 File Offset: 0x00067A55
	public Notification CreateTirionQuote(string stringTag, string soundName, bool allowRepeatDuringSession = true)
	{
		return this.CreateTirionQuote(NotificationManager.DEFAULT_CHARACTER_POS, stringTag, soundName, allowRepeatDuringSession);
	}

	// Token: 0x06001643 RID: 5699 RVA: 0x00069868 File Offset: 0x00067A68
	public Notification CreateTirionQuote(Vector3 position, string stringTag, string soundName, bool allowRepeatDuringSession = true)
	{
		return this.CreateCharacterQuote("Tirion_Quote", position, GameStrings.Get(stringTag), soundName, allowRepeatDuringSession, 0f, null, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x06001644 RID: 5700 RVA: 0x00069891 File Offset: 0x00067A91
	public Notification CreateKTQuote(string stringTag, string soundName, bool allowRepeatDuringSession = true)
	{
		return this.CreateKTQuote(NotificationManager.DEFAULT_CHARACTER_POS, stringTag, soundName, allowRepeatDuringSession);
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000698A4 File Offset: 0x00067AA4
	public Notification CreateKTQuote(Vector3 position, string stringTag, string soundName, bool allowRepeatDuringSession = true)
	{
		return this.CreateCharacterQuote("KT_Quote", position, GameStrings.Get(stringTag), soundName, allowRepeatDuringSession, 0f, null, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x06001646 RID: 5702 RVA: 0x000698D0 File Offset: 0x00067AD0
	public Notification CreateZombieNefarianQuote(Vector3 position, string stringTag, string soundName, bool allowRepeatDuringSession)
	{
		return this.CreateCharacterQuote("NefarianDragon_Quote", position, GameStrings.Get(stringTag), soundName, allowRepeatDuringSession, 0f, null, CanvasAnchor.BOTTOM_LEFT);
	}

	// Token: 0x06001647 RID: 5703 RVA: 0x000698FC File Offset: 0x00067AFC
	public Notification CreateCharacterQuote(string prefabName, string text, string soundName, bool allowRepeatDuringSession = true, float durationSeconds = 0f, CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT)
	{
		return this.CreateCharacterQuote(prefabName, NotificationManager.DEFAULT_CHARACTER_POS, text, soundName, allowRepeatDuringSession, durationSeconds, null, anchorPoint);
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x00069920 File Offset: 0x00067B20
	public Notification CreateCharacterQuote(string prefabName, Vector3 position, string text, string soundName, bool allowRepeatDuringSession = true, float durationSeconds = 0f, Action finishCallback = null, CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT)
	{
		if (!allowRepeatDuringSession && this.m_quotesThisSession.Contains(soundName))
		{
			return null;
		}
		this.m_quotesThisSession.Add(soundName);
		Notification notification = GameUtils.LoadGameObjectWithComponent<Notification>(prefabName);
		if (notification == null)
		{
			return null;
		}
		if (finishCallback != null)
		{
			Notification notification2 = notification;
			notification2.OnFinishDeathState = (Action)Delegate.Combine(notification2.OnFinishDeathState, finishCallback);
		}
		this.PlayCharacterQuote(notification, position, text, soundName, durationSeconds, anchorPoint);
		return notification;
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x00069999 File Offset: 0x00067B99
	public Notification CreateBigCharacterQuote(string prefabName, string soundName, bool allowRepeatDuringSession = true, float durationSeconds = 0f, Action finishCallback = null)
	{
		return this.CreateBigCharacterQuote(prefabName, soundName, soundName, allowRepeatDuringSession, durationSeconds, finishCallback);
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x000699AC File Offset: 0x00067BAC
	public Notification CreateBigCharacterQuote(string prefabName, string soundName, string textID, bool allowRepeatDuringSession = true, float durationSeconds = 0f, Action finishCallback = null)
	{
		if (!allowRepeatDuringSession && this.m_quotesThisSession.Contains(textID))
		{
			return null;
		}
		this.m_quotesThisSession.Add(textID);
		Notification notification = GameUtils.LoadGameObjectWithComponent<Notification>(prefabName);
		if (notification == null)
		{
			return null;
		}
		if (finishCallback != null)
		{
			Notification notification2 = notification;
			notification2.OnFinishDeathState = (Action)Delegate.Combine(notification2.OnFinishDeathState, finishCallback);
		}
		this.PlayBigCharacterQuote(notification, GameStrings.Get(textID), soundName, durationSeconds);
		return notification;
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x00069A24 File Offset: 0x00067C24
	public void ForceAddSoundToPlayedList(string soundName)
	{
		this.m_quotesThisSession.Add(soundName);
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x00069A32 File Offset: 0x00067C32
	public void ForceRemoveSoundFromPlayedList(string soundName)
	{
		this.m_quotesThisSession.Remove(soundName);
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x00069A41 File Offset: 0x00067C41
	public bool HasSoundPlayedThisSession(string soundName)
	{
		return this.m_quotesThisSession.Contains(soundName);
	}

	// Token: 0x0600164E RID: 5710 RVA: 0x00069A50 File Offset: 0x00067C50
	private void PlayBigCharacterQuote(Notification quote, string text, string soundName, float durationSeconds)
	{
		if (this.m_quote)
		{
			Object.Destroy(this.m_quote.gameObject);
		}
		this.m_quote = quote;
		this.m_quote.ChangeText(text);
		TransformUtil.AttachAndPreserveLocalTransform(this.m_quote.transform, Board.Get().FindBone("OffScreenSpeaker1"));
		this.m_quote.transform.localPosition = Vector3.zero;
		this.m_quote.transform.localScale = Vector3.one * 0.01f;
		this.m_quote.transform.localEulerAngles = Vector3.zero;
		if (!string.IsNullOrEmpty(soundName))
		{
			NotificationManager.QuoteSoundCallbackData quoteSoundCallbackData = new NotificationManager.QuoteSoundCallbackData();
			quoteSoundCallbackData.m_quote = this.m_quote;
			quoteSoundCallbackData.m_durationSeconds = durationSeconds;
			AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnBigQuoteSoundLoaded), quoteSoundCallbackData, false, SoundManager.Get().GetPlaceholderSound());
		}
		else
		{
			this.m_quote.PlayBirthWithForcedScale(Vector3.one);
			if (durationSeconds > 0f)
			{
				this.DestroyNotification(this.m_quote, durationSeconds);
			}
		}
	}

	// Token: 0x0600164F RID: 5711 RVA: 0x00069B70 File Offset: 0x00067D70
	private void PlayCharacterQuote(Notification quote, Vector3 position, string text, string soundName, float durationSeconds, CanvasAnchor anchorPoint = CanvasAnchor.BOTTOM_LEFT)
	{
		if (this.m_quote)
		{
			Object.Destroy(this.m_quote.gameObject);
		}
		this.m_quote = quote;
		this.m_quote.ChangeText(text);
		this.m_quote.transform.position = position;
		this.m_quote.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
		OverlayUI.Get().AddGameObject(this.m_quote.gameObject, anchorPoint, false, CanvasScaleMode.HEIGHT);
		if (!string.IsNullOrEmpty(soundName))
		{
			NotificationManager.QuoteSoundCallbackData quoteSoundCallbackData = new NotificationManager.QuoteSoundCallbackData();
			quoteSoundCallbackData.m_quote = this.m_quote;
			quoteSoundCallbackData.m_durationSeconds = durationSeconds;
			AssetLoader.Get().LoadSound(soundName, new AssetLoader.GameObjectCallback(this.OnQuoteSoundLoaded), quoteSoundCallbackData, false, SoundManager.Get().GetPlaceholderSound());
		}
		else
		{
			this.PlayQuoteWithoutSound(durationSeconds);
		}
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x00069C58 File Offset: 0x00067E58
	private void PlayQuoteWithoutSound(float durationSeconds)
	{
		this.m_quote.PlayBirthWithForcedScale((!UniversalInputManager.UsePhoneUI) ? this.NOTIFICATION_SCALE : this.NOTIFICATION_SCALE_PHONE);
		if (durationSeconds > 0f)
		{
			this.DestroyNotification(this.m_quote, durationSeconds);
		}
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x00069CA8 File Offset: 0x00067EA8
	private void OnQuoteSoundLoaded(string name, GameObject go, object userData)
	{
		NotificationManager.QuoteSoundCallbackData quoteSoundCallbackData = (NotificationManager.QuoteSoundCallbackData)userData;
		if (!quoteSoundCallbackData.m_quote)
		{
			Object.Destroy(go);
			return;
		}
		if (!go)
		{
			Debug.LogWarning("Quote Sound failed to load!");
			this.PlayQuoteWithoutSound((quoteSoundCallbackData.m_durationSeconds <= 0f) ? 8f : quoteSoundCallbackData.m_durationSeconds);
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		this.m_quote.AssignAudio(component);
		SoundManager.Get().PlayPreloaded(component);
		this.m_quote.PlayBirthWithForcedScale((!UniversalInputManager.UsePhoneUI) ? this.NOTIFICATION_SCALE : this.NOTIFICATION_SCALE_PHONE);
		float delaySeconds = Mathf.Max(quoteSoundCallbackData.m_durationSeconds, component.clip.length);
		this.DestroyNotification(this.m_quote, delaySeconds);
		if (this.m_quote.clickOff != null)
		{
			this.m_quote.clickOff.SetData(this.m_quote);
			this.m_quote.clickOff.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
		}
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x00069DC8 File Offset: 0x00067FC8
	private void OnBigQuoteSoundLoaded(string name, GameObject go, object userData)
	{
		NotificationManager.QuoteSoundCallbackData quoteSoundCallbackData = (NotificationManager.QuoteSoundCallbackData)userData;
		if (!quoteSoundCallbackData.m_quote)
		{
			Object.Destroy(go);
			return;
		}
		if (!go)
		{
			Debug.LogWarning("Quote Sound failed to load!");
			this.PlayQuoteWithoutSound((quoteSoundCallbackData.m_durationSeconds <= 0f) ? 8f : quoteSoundCallbackData.m_durationSeconds);
			return;
		}
		AudioSource component = go.GetComponent<AudioSource>();
		this.m_quote.AssignAudio(component);
		SoundManager.Get().PlayPreloaded(component);
		this.m_quote.PlayBirthWithForcedScale(Vector3.one);
		float delaySeconds = Mathf.Max(quoteSoundCallbackData.m_durationSeconds, component.clip.length);
		this.DestroyNotification(this.m_quote, delaySeconds);
		if (this.m_quote.clickOff != null)
		{
			this.m_quote.clickOff.SetData(this.m_quote);
			this.m_quote.clickOff.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
		}
	}

	// Token: 0x06001653 RID: 5715 RVA: 0x00069ECC File Offset: 0x000680CC
	public void DestroyAllArrows()
	{
		if (this.arrows.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.arrows.Count; i++)
		{
			if (this.arrows[i] != null)
			{
				this.NukeNotificationWithoutPlayingAnim(this.arrows[i]);
			}
		}
	}

	// Token: 0x06001654 RID: 5716 RVA: 0x00069F30 File Offset: 0x00068130
	public void DestroyAllPopUps()
	{
		if (this.popUpTexts.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.popUpTexts.Count; i++)
		{
			if (!(this.popUpTexts[i] == null))
			{
				this.NukeNotification(this.popUpTexts[i]);
			}
		}
		this.popUpTexts = new List<Notification>();
	}

	// Token: 0x06001655 RID: 5717 RVA: 0x00069FA4 File Offset: 0x000681A4
	private void DestroyOtherNotifications(Notification.SpeechBubbleDirection direction)
	{
		if (this.notificationsToDestroyUponNewNotifier.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.notificationsToDestroyUponNewNotifier.Count; i++)
		{
			if (!(this.notificationsToDestroyUponNewNotifier[i] == null))
			{
				if (this.notificationsToDestroyUponNewNotifier[i].GetSpeechBubbleDirection() == direction)
				{
					this.NukeNotificationWithoutPlayingAnim(this.notificationsToDestroyUponNewNotifier[i]);
				}
			}
		}
	}

	// Token: 0x06001656 RID: 5718 RVA: 0x0006A028 File Offset: 0x00068228
	public void DestroyNotification(Notification notification, float delaySeconds)
	{
		if (notification == null)
		{
			return;
		}
		if (delaySeconds == 0f)
		{
			this.NukeNotification(notification);
			return;
		}
		base.StartCoroutine(this.WaitAndThenDestroyNotification(notification, delaySeconds));
	}

	// Token: 0x06001657 RID: 5719 RVA: 0x0006A05C File Offset: 0x0006825C
	public void DestroyNotificationWithText(string text, float delaySeconds = 0f)
	{
		Notification notification = null;
		for (int i = 0; i < this.popUpTexts.Count; i++)
		{
			if (!(this.popUpTexts[i] == null))
			{
				if (this.popUpTexts[i].speechUberText.Text == text)
				{
					notification = this.popUpTexts[i];
				}
			}
		}
		this.DestroyNotification(notification, delaySeconds);
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x0006A0DC File Offset: 0x000682DC
	private void ClickNotification(UIEvent e)
	{
		Notification notification = (Notification)e.GetElement().GetData();
		this.NukeNotification(notification);
		notification.clickOff.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.ClickNotification));
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x0006A11A File Offset: 0x0006831A
	public void DestroyActiveNotification(float delaySeconds)
	{
		if (this.popUpDialog == null)
		{
			return;
		}
		if (delaySeconds == 0f)
		{
			this.NukeNotification(this.popUpDialog);
			return;
		}
		base.StartCoroutine(this.WaitAndThenDestroyNotification(this.popUpDialog, delaySeconds));
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x0006A15A File Offset: 0x0006835A
	public void DestroyActiveQuote(float delaySeconds)
	{
		if (this.m_quote == null)
		{
			return;
		}
		if (delaySeconds == 0f)
		{
			this.NukeNotification(this.m_quote);
			return;
		}
		base.StartCoroutine(this.WaitAndThenDestroyNotification(this.m_quote, delaySeconds));
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x0006A19A File Offset: 0x0006839A
	public void DestroyNotificationNowWithNoAnim(Notification notification)
	{
		if (notification == null)
		{
			return;
		}
		this.NukeNotificationWithoutPlayingAnim(notification);
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x0006A1B0 File Offset: 0x000683B0
	private IEnumerator WaitAndThenDestroyNotification(Notification notification, float amountSeconds)
	{
		yield return new WaitForSeconds(amountSeconds);
		if (notification != null)
		{
			this.NukeNotification(notification);
		}
		yield break;
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x0006A1E8 File Offset: 0x000683E8
	private void NukeNotification(Notification notification)
	{
		if (this.notificationsToDestroyUponNewNotifier.Contains(notification))
		{
			this.notificationsToDestroyUponNewNotifier.Remove(notification);
		}
		if (notification.IsDying())
		{
			return;
		}
		notification.PlayDeath();
		UniversalInputManager.Get().SetGameDialogActive(false);
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x0006A230 File Offset: 0x00068430
	private void NukeNotificationWithoutPlayingAnim(Notification notification)
	{
		if (this.notificationsToDestroyUponNewNotifier.Contains(notification))
		{
			this.notificationsToDestroyUponNewNotifier.Remove(notification);
		}
		Object.Destroy(notification.gameObject);
		UniversalInputManager.Get().SetGameDialogActive(false);
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x0006A274 File Offset: 0x00068474
	public TutorialNotification CreateTutorialDialog(string headlineGameString, string bodyTextGameString, string buttonGameString, UIEvent.Handler buttonHandler, Vector2 materialOffset, bool swapMaterial = false)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor("TutorialIntroDialog", true, false);
		if (gameObject == null)
		{
			Debug.LogError("Unable to load tutorial dialog TutorialIntroDialog prefab.");
			return null;
		}
		TutorialNotification notification = gameObject.GetComponent<TutorialNotification>();
		if (notification == null)
		{
			Debug.LogError("TutorialNotification component does not exist on TutorialIntroDialog prefab.");
			return null;
		}
		TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, OverlayUI.Get().m_heightScale.m_Center);
		if (UniversalInputManager.UsePhoneUI)
		{
			gameObject.transform.localScale = 1.5f * gameObject.transform.localScale;
		}
		this.popUpDialog = notification;
		notification.headlineUberText.Text = GameStrings.Get(headlineGameString);
		notification.speechUberText.Text = GameStrings.Get(bodyTextGameString);
		notification.m_ButtonStart.SetText(GameStrings.Get(buttonGameString));
		if (swapMaterial)
		{
			notification.artOverlay.material = notification.swapMaterial;
		}
		notification.artOverlay.material.mainTextureOffset = materialOffset;
		notification.m_ButtonStart.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			if (buttonHandler != null)
			{
				buttonHandler(e);
			}
			notification.m_ButtonStart.ClearEventListeners();
			this.DestroyNotification(notification, 0f);
		});
		this.popUpDialog.PlayBirth();
		UniversalInputManager.Get().SetGameDialogActive(true);
		return notification;
	}

	// Token: 0x04000B1A RID: 2842
	public const string TIRION_PREFAB_NAME = "Tirion_Quote";

	// Token: 0x04000B1B RID: 2843
	public const string KT_PREFAB_NAME = "KT_Quote";

	// Token: 0x04000B1C RID: 2844
	public const string NORMAL_NEFARIAN_PREFAB_NAME = "NormalNefarian_Quote";

	// Token: 0x04000B1D RID: 2845
	public const string ZOMBIE_NEFARIAN_PREFAB_NAME = "NefarianDragon_Quote";

	// Token: 0x04000B1E RID: 2846
	public const string RAGNAROS_PREFAB_NAME = "Ragnaros_Quote";

	// Token: 0x04000B1F RID: 2847
	private const float DEFAULT_QUOTE_DURATION = 8f;

	// Token: 0x04000B20 RID: 2848
	public static readonly float DEPTH = -5f;

	// Token: 0x04000B21 RID: 2849
	public static readonly Vector3 DEFAULT_CHARACTER_POS = new Vector3(100f, NotificationManager.DEPTH, 24.7f);

	// Token: 0x04000B22 RID: 2850
	public static readonly Vector3 ALT_ADVENTURE_SCREEN_POS = new Vector3(104.8f, NotificationManager.DEPTH, 131.1f);

	// Token: 0x04000B23 RID: 2851
	public static readonly Vector3 PHONE_CHARACTER_POS = new Vector3(124.1f, NotificationManager.DEPTH, 24.7f);

	// Token: 0x04000B24 RID: 2852
	public GameObject speechBubblePrefab;

	// Token: 0x04000B25 RID: 2853
	public GameObject speechIndicatorPrefab;

	// Token: 0x04000B26 RID: 2854
	public GameObject bounceArrowPrefab;

	// Token: 0x04000B27 RID: 2855
	public GameObject fadeArrowPrefab;

	// Token: 0x04000B28 RID: 2856
	public GameObject popupTextPrefab;

	// Token: 0x04000B29 RID: 2857
	public GameObject dialogBoxPrefab;

	// Token: 0x04000B2A RID: 2858
	public GameObject innkeeperQuotePrefab;

	// Token: 0x04000B2B RID: 2859
	private static NotificationManager s_instance;

	// Token: 0x04000B2C RID: 2860
	private List<Notification> notificationsToDestroyUponNewNotifier;

	// Token: 0x04000B2D RID: 2861
	private List<Notification> arrows;

	// Token: 0x04000B2E RID: 2862
	private List<Notification> popUpTexts;

	// Token: 0x04000B2F RID: 2863
	private Notification popUpDialog;

	// Token: 0x04000B30 RID: 2864
	private Notification m_quote;

	// Token: 0x04000B31 RID: 2865
	private List<string> m_quotesThisSession;

	// Token: 0x04000B32 RID: 2866
	private Vector3 NOTIFICATION_SCALE = 0.163f * Vector3.one;

	// Token: 0x04000B33 RID: 2867
	private Vector3 NOTIFICATION_SCALE_PHONE = 0.326f * Vector3.one;

	// Token: 0x020003E9 RID: 1001
	private class QuoteSoundCallbackData
	{
		// Token: 0x0400201F RID: 8223
		public Notification m_quote;

		// Token: 0x04002020 RID: 8224
		public float m_durationSeconds;
	}
}
