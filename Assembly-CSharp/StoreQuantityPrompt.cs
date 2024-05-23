using System;
using UnityEngine;

// Token: 0x02000AE8 RID: 2792
public class StoreQuantityPrompt : UIBPopup
{
	// Token: 0x06006027 RID: 24615 RVA: 0x001CCB80 File Offset: 0x001CAD80
	private void Awake()
	{
		this.m_quantityText.RichText = false;
		this.m_okayButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSubmitPressed));
		this.m_cancelButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCancelPressed));
		Debug.Log(string.Concat(new object[]
		{
			"show postition = ",
			this.m_showPosition,
			" ",
			this.m_showScale
		}));
	}

	// Token: 0x06006028 RID: 24616 RVA: 0x001CCC08 File Offset: 0x001CAE08
	public bool Show(int maxQuantity, StoreQuantityPrompt.OkayListener delOkay = null, StoreQuantityPrompt.CancelListener delCancel = null)
	{
		if (this.m_shown)
		{
			return false;
		}
		this.m_currentMaxQuantity = maxQuantity;
		this.m_messageText.Text = GameStrings.Format("GLUE_STORE_QUANTITY_MESSAGE", new object[]
		{
			maxQuantity
		});
		this.m_shown = true;
		this.m_currentOkayListener = delOkay;
		this.m_currentCancelListener = delCancel;
		this.m_quantityText.Text = string.Empty;
		base.gameObject.SetActive(true);
		Debug.Log(string.Concat(new object[]
		{
			"show postition2 = ",
			this.m_showPosition,
			" ",
			this.m_showScale
		}));
		base.DoShowAnimation(new UIBPopup.OnAnimationComplete(this.ShowInput));
		return true;
	}

	// Token: 0x06006029 RID: 24617 RVA: 0x001CCCCD File Offset: 0x001CAECD
	protected override void Hide(bool animate)
	{
		if (!this.m_shown)
		{
			return;
		}
		this.m_shown = false;
		this.HideInput();
		base.DoHideAnimation(!animate, delegate()
		{
			base.gameObject.SetActive(false);
		});
	}

	// Token: 0x0600602A RID: 24618 RVA: 0x001CCD00 File Offset: 0x001CAF00
	private bool GetQuantity(out int quantity)
	{
		quantity = -1;
		if (!int.TryParse(this.m_quantityText.Text, ref quantity))
		{
			Debug.LogWarning(string.Format("GeneralStore.OnStoreQuantityOkayPressed: invalid quantity='{0}'", this.m_quantityText.Text));
			return false;
		}
		if (quantity <= 0)
		{
			Log.Rachelle.Print(string.Format("GeneralStore.OnStoreQuantityOkayPressed: quantity {0} must be positive", quantity), new object[0]);
			return false;
		}
		if (quantity > this.m_currentMaxQuantity)
		{
			Log.Rachelle.Print(string.Format("GeneralStore.OnStoreQuantityOkayPressed: quantity {0} is larger than max allowed quantity ({1})", quantity, this.m_currentMaxQuantity), new object[0]);
			return false;
		}
		return true;
	}

	// Token: 0x0600602B RID: 24619 RVA: 0x001CCDAC File Offset: 0x001CAFAC
	private void Submit()
	{
		this.Hide(true);
		int quantity = -1;
		if (this.GetQuantity(out quantity))
		{
			this.FireOkayEvent(quantity);
		}
		else
		{
			this.FireCancelEvent();
		}
	}

	// Token: 0x0600602C RID: 24620 RVA: 0x001CCDE1 File Offset: 0x001CAFE1
	private void Cancel()
	{
		this.Hide(true);
		this.FireCancelEvent();
	}

	// Token: 0x0600602D RID: 24621 RVA: 0x001CCDF0 File Offset: 0x001CAFF0
	private void OnSubmitPressed(UIEvent e)
	{
		this.Submit();
	}

	// Token: 0x0600602E RID: 24622 RVA: 0x001CCDF8 File Offset: 0x001CAFF8
	private void OnCancelPressed(UIEvent e)
	{
		this.Cancel();
	}

	// Token: 0x0600602F RID: 24623 RVA: 0x001CCE00 File Offset: 0x001CB000
	private void FireOkayEvent(int quantity)
	{
		if (this.m_currentOkayListener != null)
		{
			this.m_currentOkayListener(quantity);
		}
		this.m_currentOkayListener = null;
	}

	// Token: 0x06006030 RID: 24624 RVA: 0x001CCE20 File Offset: 0x001CB020
	private void FireCancelEvent()
	{
		if (this.m_currentCancelListener != null)
		{
			this.m_currentCancelListener();
		}
		this.m_currentCancelListener = null;
	}

	// Token: 0x06006031 RID: 24625 RVA: 0x001CCE40 File Offset: 0x001CB040
	private void ShowInput()
	{
		this.m_quantityText.gameObject.SetActive(false);
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		Bounds bounds = this.m_quantityText.GetBounds();
		Rect rect = CameraUtils.CreateGUIViewportRect(camera, bounds.min, bounds.max);
		UniversalInputManager.TextInputParams parms = new UniversalInputManager.TextInputParams
		{
			m_owner = base.gameObject,
			m_number = true,
			m_rect = rect,
			m_updatedCallback = new UniversalInputManager.TextInputUpdatedCallback(this.OnInputUpdated),
			m_completedCallback = new UniversalInputManager.TextInputCompletedCallback(this.OnInputComplete),
			m_canceledCallback = new UniversalInputManager.TextInputCanceledCallback(this.OnInputCanceled),
			m_font = this.m_quantityText.GetLocalizedFont(),
			m_alignment = new TextAnchor?(4),
			m_maxCharacters = 2,
			m_touchScreenKeyboardHideInput = true
		};
		UniversalInputManager.Get().UseTextInput(parms, false);
	}

	// Token: 0x06006032 RID: 24626 RVA: 0x001CCF2C File Offset: 0x001CB12C
	private void HideInput()
	{
		UniversalInputManager.Get().CancelTextInput(base.gameObject, false);
		this.m_quantityText.gameObject.SetActive(true);
	}

	// Token: 0x06006033 RID: 24627 RVA: 0x001CCF5B File Offset: 0x001CB15B
	private void ClearInput()
	{
		UniversalInputManager.Get().SetInputText(string.Empty);
	}

	// Token: 0x06006034 RID: 24628 RVA: 0x001CCF6C File Offset: 0x001CB16C
	private void OnInputUpdated(string input)
	{
		this.m_quantityText.Text = input;
	}

	// Token: 0x06006035 RID: 24629 RVA: 0x001CCF7A File Offset: 0x001CB17A
	private void OnInputComplete(string input)
	{
		this.m_quantityText.Text = input;
		this.Submit();
	}

	// Token: 0x06006036 RID: 24630 RVA: 0x001CCF8E File Offset: 0x001CB18E
	private void OnInputCanceled(bool userRequested, GameObject requester)
	{
		this.m_quantityText.Text = string.Empty;
		this.Cancel();
	}

	// Token: 0x040047AB RID: 18347
	public UIBButton m_okayButton;

	// Token: 0x040047AC RID: 18348
	public UIBButton m_cancelButton;

	// Token: 0x040047AD RID: 18349
	public UberText m_messageText;

	// Token: 0x040047AE RID: 18350
	public UberText m_quantityText;

	// Token: 0x040047AF RID: 18351
	private int m_currentMaxQuantity;

	// Token: 0x040047B0 RID: 18352
	private StoreQuantityPrompt.OkayListener m_currentOkayListener;

	// Token: 0x040047B1 RID: 18353
	private StoreQuantityPrompt.CancelListener m_currentCancelListener;

	// Token: 0x02000AE9 RID: 2793
	// (Invoke) Token: 0x06006039 RID: 24633
	public delegate void OkayListener(int quantity);

	// Token: 0x02000AEA RID: 2794
	// (Invoke) Token: 0x0600603D RID: 24637
	public delegate void CancelListener();
}
