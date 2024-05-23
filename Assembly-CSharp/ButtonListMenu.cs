using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200046A RID: 1130
public abstract class ButtonListMenu : MonoBehaviour
{
	// Token: 0x06003784 RID: 14212 RVA: 0x001102CC File Offset: 0x0010E4CC
	protected virtual void Awake()
	{
		GameObject gameObject = (GameObject)GameUtils.InstantiateGameObject(this.m_menuDefPrefab, null, false);
		this.m_menu = gameObject.GetComponent<ButtonListMenuDef>();
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.SetTransform();
		Camera camera = CameraUtils.FindFirstByLayer(gameObject.layer);
		GameObject gameObject2 = CameraUtils.CreateInputBlocker(camera, "GameMenuInputBlocker", this, gameObject.transform, 10f);
		this.m_blocker = gameObject2.AddComponent<PegUIElement>();
		FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
		this.m_blocker.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBlockerRelease));
	}

	// Token: 0x06003785 RID: 14213 RVA: 0x00110371 File Offset: 0x0010E571
	protected virtual void OnDestroy()
	{
		FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
	}

	// Token: 0x06003786 RID: 14214 RVA: 0x0011038C File Offset: 0x0010E58C
	public virtual void Show()
	{
		UniversalInputManager.Get().CancelTextInput(base.gameObject, true);
		this.SetTransform();
		SoundManager.Get().LoadAndPlay("Small_Click");
		base.gameObject.SetActive(true);
		UniversalInputManager.Get().SetGameDialogActive(true);
		this.HideAllButtons();
		this.LayoutMenu();
		this.m_isShown = true;
		Bounds textBounds = this.m_menu.m_headerText.GetTextBounds();
		TransformUtil.SetLocalScaleToWorldDimension(this.m_menu.m_headerMiddle, new WorldDimensionIndex[]
		{
			new WorldDimensionIndex(textBounds.size.x, 0)
		});
		this.m_menu.m_header.UpdateSlices();
		AnimationUtil.ShowWithPunch(this.m_menu.gameObject, ButtonListMenu.HIDDEN_SCALE, this.PUNCH_SCALE * this.NORMAL_SCALE, this.NORMAL_SCALE, null, true, null, null, null);
	}

	// Token: 0x06003787 RID: 14215 RVA: 0x00110474 File Offset: 0x0010E674
	public virtual void Hide()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
		UniversalInputManager.Get().SetGameDialogActive(false);
		this.m_isShown = false;
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x001104B0 File Offset: 0x0010E6B0
	public bool IsShown()
	{
		return this.m_isShown;
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x001104B8 File Offset: 0x0010E6B8
	public UIBButton CreateMenuButton(string name, string buttonTextString, UIEvent.Handler releaseHandler)
	{
		UIBButton uibbutton = (UIBButton)GameUtils.Instantiate(this.m_menu.m_templateButton, this.m_menu.m_buttonContainer.gameObject, false);
		uibbutton.SetText(GameStrings.Get(buttonTextString));
		if (name != null)
		{
			uibbutton.gameObject.name = name;
		}
		uibbutton.AddEventListener(UIEventType.RELEASE, releaseHandler);
		uibbutton.transform.localRotation = this.m_menu.m_templateButton.transform.localRotation;
		this.m_allButtons.Add(uibbutton);
		return uibbutton;
	}

	// Token: 0x0600378A RID: 14218
	protected abstract List<UIBButton> GetButtons();

	// Token: 0x0600378B RID: 14219 RVA: 0x00110540 File Offset: 0x0010E740
	protected void SetTransform()
	{
		if (this.m_menuParent == null)
		{
			this.m_menuParent = base.transform;
		}
		TransformUtil.AttachAndPreserveLocalTransform(this.m_menu.transform, this.m_menuParent);
		if (this.m_blocker != null)
		{
			this.m_blocker.transform.localPosition = new Vector3(0f, -5f, 0f);
			this.m_blocker.transform.eulerAngles = new Vector3(90f, 0f, 0f);
		}
		SceneUtils.SetLayer(this, GameLayer.UI);
		this.m_menu.gameObject.transform.localScale = this.NORMAL_SCALE;
	}

	// Token: 0x0600378C RID: 14220 RVA: 0x001105FB File Offset: 0x0010E7FB
	protected virtual void LayoutMenu()
	{
		this.LayoutMenuButtons();
		this.m_menu.m_buttonContainer.UpdateSlices();
		this.LayoutMenuBackground();
	}

	// Token: 0x0600378D RID: 14221 RVA: 0x0011061C File Offset: 0x0010E81C
	protected void LayoutMenuButtons()
	{
		List<UIBButton> buttons = this.GetButtons();
		this.m_menu.m_buttonContainer.ClearSlices();
		int i = 0;
		int num = 0;
		while (i < buttons.Count)
		{
			UIBButton uibbutton = buttons[i];
			Vector3 minLocalPadding = Vector3.zero;
			bool reverse = false;
			GameObject gameObject2;
			if (uibbutton == null)
			{
				GameObject gameObject;
				if (num >= this.m_horizontalDividers.Count)
				{
					gameObject = (GameObject)GameUtils.Instantiate(this.m_menu.m_templateHorizontalDivider, this.m_menu.m_buttonContainer.gameObject, false);
					gameObject.transform.localRotation = this.m_menu.m_templateHorizontalDivider.transform.localRotation;
					this.m_horizontalDividers.Add(gameObject);
				}
				else
				{
					gameObject = this.m_horizontalDividers[num];
				}
				num++;
				gameObject2 = gameObject;
				minLocalPadding = this.m_menu.m_horizontalDividerMinPadding;
				reverse = true;
			}
			else
			{
				gameObject2 = uibbutton.gameObject;
			}
			this.m_menu.m_buttonContainer.AddSlice(gameObject2, minLocalPadding, Vector3.zero, reverse);
			gameObject2.SetActive(true);
			i++;
		}
	}

	// Token: 0x0600378E RID: 14222 RVA: 0x0011073C File Offset: 0x0010E93C
	protected void LayoutMenuBackground()
	{
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_menu.m_buttonContainer.gameObject, true);
		float width = orientedBounds.Extents[0].magnitude * 2f;
		float height = orientedBounds.Extents[2].magnitude * 2f;
		this.m_menu.m_background.SetSize(width, height);
		this.m_menu.m_border.SetSize(width, height);
	}

	// Token: 0x0600378F RID: 14223 RVA: 0x001107B4 File Offset: 0x0010E9B4
	private void OnFatalError(FatalErrorMessage message, object userData)
	{
		this.Hide();
	}

	// Token: 0x06003790 RID: 14224 RVA: 0x001107BC File Offset: 0x0010E9BC
	private void HideAllButtons()
	{
		for (int i = 0; i < this.m_allButtons.Count; i++)
		{
			this.m_allButtons[i].gameObject.SetActive(false);
		}
		for (int j = 0; j < this.m_horizontalDividers.Count; j++)
		{
			this.m_horizontalDividers[j].SetActive(false);
		}
	}

	// Token: 0x06003791 RID: 14225 RVA: 0x0011082A File Offset: 0x0010EA2A
	private void OnBlockerRelease(UIEvent e)
	{
		SoundManager.Get().LoadAndPlay("Small_Click");
		this.Hide();
	}

	// Token: 0x040022C6 RID: 8902
	protected ButtonListMenuDef m_menu;

	// Token: 0x040022C7 RID: 8903
	private bool m_isShown;

	// Token: 0x040022C8 RID: 8904
	private List<UIBButton> m_allButtons = new List<UIBButton>();

	// Token: 0x040022C9 RID: 8905
	private List<GameObject> m_horizontalDividers = new List<GameObject>();

	// Token: 0x040022CA RID: 8906
	protected PegUIElement m_blocker;

	// Token: 0x040022CB RID: 8907
	protected Transform m_menuParent;

	// Token: 0x040022CC RID: 8908
	protected float PUNCH_SCALE = 1.08f;

	// Token: 0x040022CD RID: 8909
	protected Vector3 NORMAL_SCALE = Vector3.one;

	// Token: 0x040022CE RID: 8910
	protected static readonly Vector3 HIDDEN_SCALE = 0.01f * Vector3.one;

	// Token: 0x040022CF RID: 8911
	protected string m_menuDefPrefab = "ButtonListMenuDef";
}
