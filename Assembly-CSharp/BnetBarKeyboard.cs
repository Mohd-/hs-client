using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054E RID: 1358
public class BnetBarKeyboard : PegUIElement
{
	// Token: 0x06003E74 RID: 15988 RVA: 0x0012E170 File Offset: 0x0012C370
	public void ShowHighlight(bool show)
	{
		if (show)
		{
			base.gameObject.GetComponent<Renderer>().material.SetColor("_Color", this.m_highlight);
		}
		else
		{
			base.gameObject.GetComponent<Renderer>().material.SetColor("_Color", this.m_origColor);
		}
	}

	// Token: 0x06003E75 RID: 15989 RVA: 0x0012E1C8 File Offset: 0x0012C3C8
	protected override void OnPress()
	{
		W8Touch.Get().ShowKeyboard();
		OnKeyboardPressed[] array = this.m_keyboardPressedListeners.ToArray();
		foreach (OnKeyboardPressed onKeyboardPressed in array)
		{
			onKeyboardPressed();
		}
	}

	// Token: 0x06003E76 RID: 15990 RVA: 0x0012E20B File Offset: 0x0012C40B
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		this.ShowHighlight(true);
	}

	// Token: 0x06003E77 RID: 15991 RVA: 0x0012E214 File Offset: 0x0012C414
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.ShowHighlight(false);
	}

	// Token: 0x06003E78 RID: 15992 RVA: 0x0012E21D File Offset: 0x0012C41D
	public void RegisterKeyboardPressedListener(OnKeyboardPressed listener)
	{
		if (this.m_keyboardPressedListeners.Contains(listener))
		{
			return;
		}
		this.m_keyboardPressedListeners.Add(listener);
	}

	// Token: 0x06003E79 RID: 15993 RVA: 0x0012E23D File Offset: 0x0012C43D
	public void UnregisterKeyboardPressedListener(OnKeyboardPressed listener)
	{
		this.m_keyboardPressedListeners.Remove(listener);
	}

	// Token: 0x040027EE RID: 10222
	public Color m_highlight;

	// Token: 0x040027EF RID: 10223
	public Color m_origColor;

	// Token: 0x040027F0 RID: 10224
	private List<OnKeyboardPressed> m_keyboardPressedListeners = new List<OnKeyboardPressed>();
}
