using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class DialogBase : MonoBehaviour
{
	// Token: 0x06002351 RID: 9041 RVA: 0x000AE52C File Offset: 0x000AC72C
	protected virtual CanvasScaleMode ScaleMode()
	{
		return CanvasScaleMode.HEIGHT;
	}

	// Token: 0x06002352 RID: 9042 RVA: 0x000AE530 File Offset: 0x000AC730
	protected virtual void Awake()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.PUNCH_SCALE = 1.08f * Vector3.one;
		}
		OverlayUI overlayUI = OverlayUI.Get();
		CanvasScaleMode scaleMode = this.ScaleMode();
		overlayUI.AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, scaleMode);
		this.m_originalPosition = base.transform.position;
		this.m_originalScale = base.transform.localScale;
		this.SetHiddenPosition(null);
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000AE5A4 File Offset: 0x000AC7A4
	public virtual bool HandleKeyboardInput()
	{
		return false;
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000AE5A7 File Offset: 0x000AC7A7
	public virtual void GoBack()
	{
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000AE5A9 File Offset: 0x000AC7A9
	public virtual void Show()
	{
		this.m_shown = true;
		this.SetShownPosition();
	}

	// Token: 0x06002356 RID: 9046 RVA: 0x000AE5B8 File Offset: 0x000AC7B8
	public virtual void Hide()
	{
		this.m_shown = false;
		base.StartCoroutine(this.HideWhenAble());
	}

	// Token: 0x06002357 RID: 9047 RVA: 0x000AE5CE File Offset: 0x000AC7CE
	public virtual bool IsShown()
	{
		return this.m_shown;
	}

	// Token: 0x06002358 RID: 9048 RVA: 0x000AE5D6 File Offset: 0x000AC7D6
	public bool AddHideListener(DialogBase.HideCallback callback)
	{
		return this.AddHideListener(callback, null);
	}

	// Token: 0x06002359 RID: 9049 RVA: 0x000AE5E0 File Offset: 0x000AC7E0
	public bool AddHideListener(DialogBase.HideCallback callback, object userData)
	{
		DialogBase.HideListener hideListener = new DialogBase.HideListener();
		hideListener.SetCallback(callback);
		hideListener.SetUserData(userData);
		if (this.m_hideListeners.Contains(hideListener))
		{
			return false;
		}
		this.m_hideListeners.Add(hideListener);
		return true;
	}

	// Token: 0x0600235A RID: 9050 RVA: 0x000AE621 File Offset: 0x000AC821
	public bool RemoveHideListener(DialogBase.HideCallback callback)
	{
		return this.RemoveHideListener(callback, null);
	}

	// Token: 0x0600235B RID: 9051 RVA: 0x000AE62C File Offset: 0x000AC82C
	public bool RemoveHideListener(DialogBase.HideCallback callback, object userData)
	{
		DialogBase.HideListener hideListener = new DialogBase.HideListener();
		hideListener.SetCallback(callback);
		hideListener.SetUserData(userData);
		return this.m_hideListeners.Remove(hideListener);
	}

	// Token: 0x0600235C RID: 9052 RVA: 0x000AE659 File Offset: 0x000AC859
	protected void SetShownPosition()
	{
		base.transform.position = this.m_originalPosition;
	}

	// Token: 0x0600235D RID: 9053 RVA: 0x000AE66C File Offset: 0x000AC86C
	protected void SetHiddenPosition(Camera referenceCamera = null)
	{
		if (referenceCamera == null)
		{
			referenceCamera = PegUI.Get().orthographicUICam;
		}
		base.transform.position = referenceCamera.transform.TransformPoint(0f, 0f, -1000f);
	}

	// Token: 0x0600235E RID: 9054 RVA: 0x000AE6B8 File Offset: 0x000AC8B8
	protected void DoShowAnimation()
	{
		this.m_showAnimState = DialogBase.ShowAnimState.IN_PROGRESS;
		AnimationUtil.ShowWithPunch(base.gameObject, this.START_SCALE, Vector3.Scale(this.PUNCH_SCALE, this.m_originalScale), this.m_originalScale, "OnShowAnimFinished", false, null, null, null);
	}

	// Token: 0x0600235F RID: 9055 RVA: 0x000AE6FD File Offset: 0x000AC8FD
	protected void DoHideAnimation()
	{
		AnimationUtil.ScaleFade(base.gameObject, this.START_SCALE, "OnHideAnimFinished");
	}

	// Token: 0x06002360 RID: 9056 RVA: 0x000AE715 File Offset: 0x000AC915
	protected virtual void OnHideAnimFinished()
	{
		this.SetHiddenPosition(null);
		UniversalInputManager.Get().SetSystemDialogActive(false);
		this.FireHideListeners();
	}

	// Token: 0x06002361 RID: 9057 RVA: 0x000AE730 File Offset: 0x000AC930
	protected void FireHideListeners()
	{
		DialogBase.HideListener[] array = this.m_hideListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire(this);
		}
	}

	// Token: 0x06002362 RID: 9058 RVA: 0x000AE766 File Offset: 0x000AC966
	protected virtual void OnShowAnimFinished()
	{
		this.m_showAnimState = DialogBase.ShowAnimState.FINISHED;
	}

	// Token: 0x06002363 RID: 9059 RVA: 0x000AE770 File Offset: 0x000AC970
	private IEnumerator HideWhenAble()
	{
		while (this.m_showAnimState == DialogBase.ShowAnimState.IN_PROGRESS)
		{
			yield return null;
		}
		this.DoHideAnimation();
		yield break;
	}

	// Token: 0x04001468 RID: 5224
	protected readonly Vector3 START_SCALE = 0.01f * Vector3.one;

	// Token: 0x04001469 RID: 5225
	protected Vector3 PUNCH_SCALE = 1.2f * Vector3.one;

	// Token: 0x0400146A RID: 5226
	protected DialogBase.ShowAnimState m_showAnimState;

	// Token: 0x0400146B RID: 5227
	protected bool m_shown;

	// Token: 0x0400146C RID: 5228
	protected Vector3 m_originalPosition;

	// Token: 0x0400146D RID: 5229
	protected Vector3 m_originalScale;

	// Token: 0x0400146E RID: 5230
	protected List<DialogBase.HideListener> m_hideListeners = new List<DialogBase.HideListener>();

	// Token: 0x02000276 RID: 630
	// (Invoke) Token: 0x06002365 RID: 9061
	public delegate void HideCallback(DialogBase dialog, object userData);

	// Token: 0x02000472 RID: 1138
	protected enum ShowAnimState
	{
		// Token: 0x0400238B RID: 9099
		NOT_CALLED,
		// Token: 0x0400238C RID: 9100
		IN_PROGRESS,
		// Token: 0x0400238D RID: 9101
		FINISHED
	}

	// Token: 0x020005CD RID: 1485
	protected class HideListener : EventListener<DialogBase.HideCallback>
	{
		// Token: 0x06004255 RID: 16981 RVA: 0x00140243 File Offset: 0x0013E443
		public void Fire(DialogBase dialog)
		{
			this.m_callback(dialog, this.m_userData);
		}
	}
}
