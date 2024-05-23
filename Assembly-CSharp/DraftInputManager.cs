using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000209 RID: 521
public class DraftInputManager : MonoBehaviour
{
	// Token: 0x06001FA1 RID: 8097 RVA: 0x0009AA4A File Offset: 0x00098C4A
	private void Awake()
	{
		DraftInputManager.s_instance = this;
	}

	// Token: 0x06001FA2 RID: 8098 RVA: 0x0009AA52 File Offset: 0x00098C52
	private void OnDestroy()
	{
		DraftInputManager.s_instance = null;
	}

	// Token: 0x06001FA3 RID: 8099 RVA: 0x0009AA5A File Offset: 0x00098C5A
	public static DraftInputManager Get()
	{
		return DraftInputManager.s_instance;
	}

	// Token: 0x06001FA4 RID: 8100 RVA: 0x0009AA61 File Offset: 0x00098C61
	public void Unload()
	{
	}

	// Token: 0x06001FA5 RID: 8101 RVA: 0x0009AA64 File Offset: 0x00098C64
	public bool HandleKeyboardInput()
	{
		DraftDisplay draftDisplay = DraftDisplay.Get();
		if (draftDisplay == null)
		{
			return false;
		}
		if (Input.GetKeyUp(27) && DraftDisplay.Get().IsInHeroSelectMode())
		{
			DraftDisplay.Get().DoHeroCancelAnimation();
			return true;
		}
		if (!ApplicationMgr.IsInternal())
		{
			return false;
		}
		List<DraftCardVisual> cardVisuals = DraftDisplay.Get().GetCardVisuals();
		if (cardVisuals == null)
		{
			return false;
		}
		if (cardVisuals.Count == 0)
		{
			return false;
		}
		int num = -1;
		if (Input.GetKeyUp(49))
		{
			num = 0;
		}
		else if (Input.GetKeyUp(50))
		{
			num = 1;
		}
		else if (Input.GetKeyUp(51))
		{
			num = 2;
		}
		if (num == -1)
		{
			return false;
		}
		if (cardVisuals.Count < num + 1)
		{
			return false;
		}
		DraftCardVisual draftCardVisual = cardVisuals[num];
		if (draftCardVisual == null)
		{
			return false;
		}
		draftCardVisual.ChooseThisCard();
		return true;
	}

	// Token: 0x0400116E RID: 4462
	private static DraftInputManager s_instance;
}
