using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200040C RID: 1036
[CustomEditClass]
public class UIBPopup : MonoBehaviour
{
	// Token: 0x060034C7 RID: 13511 RVA: 0x00106D31 File Offset: 0x00104F31
	public virtual bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x00106D3C File Offset: 0x00104F3C
	public virtual void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		OverlayUI overlayUI = OverlayUI.Get();
		CanvasScaleMode scaleMode = this.m_scaleMode;
		overlayUI.AddGameObject(base.gameObject, CanvasAnchor.CENTER, this.m_destroyOnSceneLoad, scaleMode);
		this.m_shown = true;
		this.DoShowAnimation(null);
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x00106D82 File Offset: 0x00104F82
	public virtual void Hide()
	{
		this.Hide(false);
	}

	// Token: 0x060034CA RID: 13514 RVA: 0x00106D8C File Offset: 0x00104F8C
	protected virtual void Hide(bool animate)
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.DoHideAnimation(!animate, new UIBPopup.OnAnimationComplete(this.OnHidden));
	}

	// Token: 0x060034CB RID: 13515 RVA: 0x00106DC3 File Offset: 0x00104FC3
	protected virtual void OnHidden()
	{
	}

	// Token: 0x060034CC RID: 13516 RVA: 0x00106DC5 File Offset: 0x00104FC5
	protected void DoShowAnimation(UIBPopup.OnAnimationComplete animationDoneCallback = null)
	{
		this.DoShowAnimation(false, animationDoneCallback);
	}

	// Token: 0x060034CD RID: 13517 RVA: 0x00106DD0 File Offset: 0x00104FD0
	protected void DoShowAnimation(bool disableAnimation, UIBPopup.OnAnimationComplete animationDoneCallback = null)
	{
		base.transform.localPosition = this.m_showPosition;
		OverlayUI overlayUI = OverlayUI.Get();
		CanvasScaleMode scaleMode = this.m_scaleMode;
		overlayUI.AddGameObject(base.gameObject, CanvasAnchor.CENTER, this.m_destroyOnSceneLoad, scaleMode);
		this.EnableAnimationClickBlocker(true);
		if (!disableAnimation && this.m_showAnimTime > 0f)
		{
			base.transform.localScale = this.m_showScale * 0.01f;
			if (!string.IsNullOrEmpty(this.m_showAnimationSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_showAnimationSound));
			}
			Hashtable hashtable = iTween.Hash(new object[]
			{
				"scale",
				this.m_showScale,
				"isLocal",
				false,
				"time",
				this.m_showAnimTime,
				"easetype",
				iTween.EaseType.easeOutBounce,
				"name",
				"SHOW_ANIMATION"
			});
			if (animationDoneCallback != null)
			{
				hashtable.Add("oncomplete", delegate(object o)
				{
					this.EnableAnimationClickBlocker(false);
					animationDoneCallback();
				});
			}
			iTween.StopByName(base.gameObject, "SHOW_ANIMATION");
			iTween.ScaleTo(base.gameObject, hashtable);
		}
		else
		{
			if (this.m_playShowSoundWithNoAnimation && !string.IsNullOrEmpty(this.m_showAnimationSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_showAnimationSound));
			}
			base.transform.localScale = this.m_showScale;
			if (animationDoneCallback != null)
			{
				this.EnableAnimationClickBlocker(false);
				animationDoneCallback();
			}
		}
	}

	// Token: 0x060034CE RID: 13518 RVA: 0x00106F8C File Offset: 0x0010518C
	protected void DoHideAnimation(UIBPopup.OnAnimationComplete animationDoneCallback = null)
	{
		this.DoHideAnimation(false, animationDoneCallback);
	}

	// Token: 0x060034CF RID: 13519 RVA: 0x00106F98 File Offset: 0x00105198
	protected void DoHideAnimation(bool disableAnimation, UIBPopup.OnAnimationComplete animationDoneCallback = null)
	{
		Action setHidePosition = delegate()
		{
			this.transform.localPosition = this.m_hidePosition;
			this.transform.localScale = this.m_showScale;
		};
		if (!disableAnimation && this.m_hideAnimTime > 0f)
		{
			if (!string.IsNullOrEmpty(this.m_showAnimationSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_hideAnimationSound));
			}
			Hashtable hashtable = iTween.Hash(new object[]
			{
				"scale",
				this.m_showScale * 0.01f,
				"isLocal",
				true,
				"time",
				this.m_hideAnimTime,
				"easetype",
				iTween.EaseType.linear,
				"name",
				"SHOW_ANIMATION"
			});
			if (animationDoneCallback != null)
			{
				hashtable.Add("oncomplete", delegate(object o)
				{
					setHidePosition.Invoke();
					animationDoneCallback();
				});
			}
			else
			{
				hashtable.Add("oncomplete", delegate(object o)
				{
					setHidePosition.Invoke();
				});
			}
			iTween.StopByName(base.gameObject, "SHOW_ANIMATION");
			iTween.ScaleTo(base.gameObject, hashtable);
		}
		else
		{
			if (this.m_playHideSoundWithNoAnimation && !string.IsNullOrEmpty(this.m_hideAnimationSound))
			{
				SoundManager.Get().LoadAndPlay(FileUtils.GameAssetPathToName(this.m_hideAnimationSound));
			}
			setHidePosition.Invoke();
			if (animationDoneCallback != null)
			{
				animationDoneCallback();
			}
		}
	}

	// Token: 0x060034D0 RID: 13520 RVA: 0x00107130 File Offset: 0x00105330
	private void EnableAnimationClickBlocker(bool enable)
	{
		if (this.m_animationClickBlocker != null)
		{
			this.m_animationClickBlocker.gameObject.SetActive(enable);
		}
	}

	// Token: 0x040020E4 RID: 8420
	private const string s_ShowiTweenAnimationName = "SHOW_ANIMATION";

	// Token: 0x040020E5 RID: 8421
	[CustomEditField(Sections = "Animation & Positioning")]
	public Vector3 m_showPosition = Vector3.zero;

	// Token: 0x040020E6 RID: 8422
	[CustomEditField(Sections = "Animation & Positioning")]
	public Vector3 m_showScale = Vector3.one;

	// Token: 0x040020E7 RID: 8423
	[CustomEditField(Sections = "Animation & Positioning")]
	public float m_showAnimTime = 0.5f;

	// Token: 0x040020E8 RID: 8424
	[CustomEditField(Sections = "Animation & Positioning")]
	public Vector3 m_hidePosition = new Vector3(-1000f, 0f, 0f);

	// Token: 0x040020E9 RID: 8425
	[CustomEditField(Sections = "Animation & Positioning")]
	public float m_hideAnimTime = 0.1f;

	// Token: 0x040020EA RID: 8426
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_showAnimationSound = "Assets/Game/Sounds/Interface/Expand_Up";

	// Token: 0x040020EB RID: 8427
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public bool m_playShowSoundWithNoAnimation;

	// Token: 0x040020EC RID: 8428
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public string m_hideAnimationSound = "Assets/Game/Sounds/Interface/Shrink_Down";

	// Token: 0x040020ED RID: 8429
	[CustomEditField(Sections = "Sounds", T = EditType.SOUND_PREFAB)]
	public bool m_playHideSoundWithNoAnimation;

	// Token: 0x040020EE RID: 8430
	[CustomEditField(Sections = "Click Blockers")]
	public BoxCollider m_animationClickBlocker;

	// Token: 0x040020EF RID: 8431
	protected bool m_shown;

	// Token: 0x040020F0 RID: 8432
	protected CanvasScaleMode m_scaleMode = CanvasScaleMode.HEIGHT;

	// Token: 0x040020F1 RID: 8433
	protected bool m_destroyOnSceneLoad = true;

	// Token: 0x02000532 RID: 1330
	// (Invoke) Token: 0x06003DCA RID: 15818
	public delegate void OnAnimationComplete();
}
