using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006EA RID: 1770
public class CollectionNewDeckButton : PegUIElement
{
	// Token: 0x06004912 RID: 18706 RVA: 0x0015D3B0 File Offset: 0x0015B5B0
	protected override void Awake()
	{
		base.Awake();
		this.SetEnabled(false);
		this.m_buttonText.Text = ((SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL) ? GameStrings.Get("GLUE_COLLECTION_NEW_DECK") : string.Empty);
		UIBScrollableItem component = base.GetComponent<UIBScrollableItem>();
		if (component != null)
		{
			component.SetCustomActiveState(new UIBScrollableItem.ActiveStateCallback(this.IsUsable));
		}
	}

	// Token: 0x06004913 RID: 18707 RVA: 0x0015D41F File Offset: 0x0015B61F
	public void SetIsUsable(bool isUsable)
	{
		this.m_isUsable = isUsable;
	}

	// Token: 0x06004914 RID: 18708 RVA: 0x0015D428 File Offset: 0x0015B628
	public bool IsUsable()
	{
		return this.m_isUsable;
	}

	// Token: 0x06004915 RID: 18709 RVA: 0x0015D430 File Offset: 0x0015B630
	public void PlayPopUpAnimation()
	{
		this.PlayPopUpAnimation(null);
	}

	// Token: 0x06004916 RID: 18710 RVA: 0x0015D43C File Offset: 0x0015B63C
	public void PlayPopUpAnimation(CollectionNewDeckButton.DelOnAnimationFinished callback)
	{
		this.PlayPopUpAnimation(callback, null, default(float?));
	}

	// Token: 0x06004917 RID: 18711 RVA: 0x0015D45C File Offset: 0x0015B65C
	public void PlayPopUpAnimation(CollectionNewDeckButton.DelOnAnimationFinished callback, object callbackData, float? speed = null)
	{
		base.gameObject.SetActive(true);
		if (this.m_isPoppedUp)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isPoppedUp = true;
		base.GetComponent<Animation>()[this.DECKBOX_POPUP_ANIM_NAME].time = 0f;
		base.GetComponent<Animation>()[this.DECKBOX_POPUP_ANIM_NAME].speed = ((speed == null) ? 2.5f : speed.Value);
		this.PlayAnimation(this.DECKBOX_POPUP_ANIM_NAME, callback, callbackData);
	}

	// Token: 0x06004918 RID: 18712 RVA: 0x0015D4F1 File Offset: 0x0015B6F1
	public void PlayPopDownAnimation()
	{
		this.PlayPopDownAnimation(null);
	}

	// Token: 0x06004919 RID: 18713 RVA: 0x0015D4FC File Offset: 0x0015B6FC
	public void PlayPopDownAnimation(CollectionNewDeckButton.DelOnAnimationFinished callback)
	{
		this.PlayPopDownAnimation(callback, null, default(float?));
	}

	// Token: 0x0600491A RID: 18714 RVA: 0x0015D51C File Offset: 0x0015B71C
	public void PlayPopDownAnimation(CollectionNewDeckButton.DelOnAnimationFinished callback, object callbackData, float? speed = null)
	{
		base.gameObject.SetActive(true);
		if (!this.m_isPoppedUp)
		{
			if (callback != null)
			{
				callback(callbackData);
			}
			return;
		}
		this.m_isPoppedUp = false;
		base.GetComponent<Animation>()[this.DECKBOX_POPDOWN_ANIM_NAME].time = 0f;
		base.GetComponent<Animation>()[this.DECKBOX_POPDOWN_ANIM_NAME].speed = ((speed == null) ? 2.5f : speed.Value);
		this.PlayAnimation(this.DECKBOX_POPDOWN_ANIM_NAME, callback, callbackData);
	}

	// Token: 0x0600491B RID: 18715 RVA: 0x0015D5B4 File Offset: 0x0015B7B4
	public void FlipHalfOverAndHide(float animTime, CollectionNewDeckButton.DelOnAnimationFinished finished = null)
	{
		if (!this.m_isPoppedUp)
		{
			Debug.LogWarning("Can't flip over and hide button. It is currently not popped up.");
			return;
		}
		iTween.StopByName(base.gameObject, "rotation");
		iTween.RotateTo(base.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			new Vector3(270f, 0f, 0f),
			"isLocal",
			true,
			"time",
			animTime,
			"easeType",
			iTween.EaseType.easeInCubic,
			"oncomplete",
			delegate(object _1)
			{
				if (finished != null)
				{
					finished(this);
				}
				this.gameObject.SetActive(false);
				this.transform.localEulerAngles = Vector3.zero;
			},
			"name",
			"rotation"
		}));
		this.m_isPoppedUp = false;
	}

	// Token: 0x0600491C RID: 18716 RVA: 0x0015D69A File Offset: 0x0015B89A
	private void PlayAnimation(string animationName)
	{
		this.PlayAnimation(animationName, null, null);
	}

	// Token: 0x0600491D RID: 18717 RVA: 0x0015D6A8 File Offset: 0x0015B8A8
	private void PlayAnimation(string animationName, CollectionNewDeckButton.DelOnAnimationFinished callback, object callbackData)
	{
		base.GetComponent<Animation>().Play(animationName);
		CollectionNewDeckButton.OnPopAnimationFinishedCallbackData onPopAnimationFinishedCallbackData = new CollectionNewDeckButton.OnPopAnimationFinishedCallbackData
		{
			m_callback = callback,
			m_callbackData = callbackData,
			m_animationName = animationName
		};
		base.StopCoroutine("WaitThenCallAnimationCallback");
		base.StartCoroutine("WaitThenCallAnimationCallback", onPopAnimationFinishedCallbackData);
	}

	// Token: 0x0600491E RID: 18718 RVA: 0x0015D6F8 File Offset: 0x0015B8F8
	private IEnumerator WaitThenCallAnimationCallback(CollectionNewDeckButton.OnPopAnimationFinishedCallbackData callbackData)
	{
		yield return new WaitForSeconds(base.GetComponent<Animation>()[callbackData.m_animationName].length / base.GetComponent<Animation>()[callbackData.m_animationName].speed);
		bool enableInput = callbackData.m_animationName.Equals(this.DECKBOX_POPUP_ANIM_NAME);
		this.SetEnabled(enableInput);
		if (callbackData.m_callback == null)
		{
			yield break;
		}
		callbackData.m_callback(callbackData.m_callbackData);
		yield break;
	}

	// Token: 0x0600491F RID: 18719 RVA: 0x0015D721 File Offset: 0x0015B921
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("Hub_Mouseover");
		this.m_highlightState.ChangeState(ActorStateType.HIGHLIGHT_MOUSE_OVER);
	}

	// Token: 0x06004920 RID: 18720 RVA: 0x0015D740 File Offset: 0x0015B940
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_highlightState.ChangeState(ActorStateType.NONE);
	}

	// Token: 0x0400302F RID: 12335
	private const float BUTTON_POP_SPEED = 2.5f;

	// Token: 0x04003030 RID: 12336
	public HighlightState m_highlightState;

	// Token: 0x04003031 RID: 12337
	public UberText m_buttonText;

	// Token: 0x04003032 RID: 12338
	private readonly string DECKBOX_POPUP_ANIM_NAME = "NewDeck_PopUp";

	// Token: 0x04003033 RID: 12339
	private readonly string DECKBOX_POPDOWN_ANIM_NAME = "NewDeck_PopDown";

	// Token: 0x04003034 RID: 12340
	private bool m_isPoppedUp;

	// Token: 0x04003035 RID: 12341
	private bool m_isUsable;

	// Token: 0x02000754 RID: 1876
	// (Invoke) Token: 0x06004B97 RID: 19351
	public delegate void DelOnAnimationFinished(object callbackData);

	// Token: 0x020007A1 RID: 1953
	private class OnPopAnimationFinishedCallbackData
	{
		// Token: 0x0400340A RID: 13322
		public string m_animationName;

		// Token: 0x0400340B RID: 13323
		public CollectionNewDeckButton.DelOnAnimationFinished m_callback;

		// Token: 0x0400340C RID: 13324
		public object m_callbackData;
	}
}
