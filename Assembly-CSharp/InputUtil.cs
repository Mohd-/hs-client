using System;
using UnityEngine;

// Token: 0x02000204 RID: 516
public class InputUtil
{
	// Token: 0x06001E7F RID: 7807 RVA: 0x0008DCC4 File Offset: 0x0008BEC4
	public static InputScheme GetInputScheme()
	{
		RuntimePlatform platform = Application.platform;
		if (platform == 11 || platform == 8)
		{
			return InputScheme.TOUCH;
		}
		if (platform == 9 || platform == 10)
		{
			return InputScheme.GAMEPAD;
		}
		return InputScheme.KEYBOARD_MOUSE;
	}

	// Token: 0x06001E80 RID: 7808 RVA: 0x0008DCFC File Offset: 0x0008BEFC
	public static bool IsMouseOnScreen()
	{
		return UniversalInputManager.Get().GetMousePosition().x >= 0f && UniversalInputManager.Get().GetMousePosition().x <= (float)Screen.width && UniversalInputManager.Get().GetMousePosition().y >= 0f && UniversalInputManager.Get().GetMousePosition().y <= (float)Screen.height;
	}

	// Token: 0x06001E81 RID: 7809 RVA: 0x0008DD80 File Offset: 0x0008BF80
	public static bool IsPlayMakerMouseInputAllowed(GameObject go)
	{
		if (UniversalInputManager.Get() == null)
		{
			return false;
		}
		if (InputUtil.ShouldCheckGameplayForPlayMakerMouseInput(go))
		{
			GameState gameState = GameState.Get();
			if (gameState != null && gameState.IsMulliganManagerActive())
			{
				return false;
			}
			TargetReticleManager targetReticleManager = TargetReticleManager.Get();
			if (targetReticleManager != null && targetReticleManager.IsLocalArrowActive())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06001E82 RID: 7810 RVA: 0x0008DDE4 File Offset: 0x0008BFE4
	private static bool ShouldCheckGameplayForPlayMakerMouseInput(GameObject go)
	{
		if (SceneMgr.Get() == null)
		{
			return false;
		}
		if (!SceneMgr.Get().IsInGame())
		{
			return false;
		}
		if (LoadingScreen.Get() != null && LoadingScreen.Get().IsPreviousSceneActive())
		{
			LoadingScreen loadingScreen = SceneUtils.FindComponentInThisOrParents<LoadingScreen>(go);
			if (loadingScreen != null)
			{
				return false;
			}
		}
		BaseUI baseUI = SceneUtils.FindComponentInThisOrParents<BaseUI>(go);
		return !(baseUI != null);
	}
}
