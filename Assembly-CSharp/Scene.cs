using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class Scene : MonoBehaviour
{
	// Token: 0x06001FDD RID: 8157 RVA: 0x0009C145 File Offset: 0x0009A345
	protected virtual void Awake()
	{
		SceneMgr.Get().SetScene(this);
	}

	// Token: 0x06001FDE RID: 8158 RVA: 0x0009C152 File Offset: 0x0009A352
	public virtual void PreUnload()
	{
	}

	// Token: 0x06001FDF RID: 8159 RVA: 0x0009C154 File Offset: 0x0009A354
	public virtual bool IsUnloading()
	{
		return false;
	}

	// Token: 0x06001FE0 RID: 8160 RVA: 0x0009C157 File Offset: 0x0009A357
	public virtual void Unload()
	{
	}

	// Token: 0x06001FE1 RID: 8161 RVA: 0x0009C15C File Offset: 0x0009A35C
	public virtual bool HandleKeyboardInput()
	{
		if (BackButton.backKey != null && Input.GetKeyUp(BackButton.backKey))
		{
			if (DialogManager.Get().ShowingDialog())
			{
				DialogManager.Get().GoBack();
				return true;
			}
			if (ChatMgr.Get().IsFriendListShowing() || ChatMgr.Get().IsChatLogFrameShown())
			{
				ChatMgr.Get().GoBack();
				return true;
			}
			if (OptionsMenu.Get() != null && OptionsMenu.Get().IsShown())
			{
				OptionsMenu.Get().Hide(true);
				return true;
			}
			if (GameMenu.Get() != null && GameMenu.Get().IsShown())
			{
				GameMenu.Get().Hide();
				return true;
			}
			if (Navigation.GoBack())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x02000466 RID: 1126
	// (Invoke) Token: 0x0600373D RID: 14141
	public delegate void BackButtonPressedDelegate();
}
