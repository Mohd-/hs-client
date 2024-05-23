using System;
using UnityEngine;

// Token: 0x02000727 RID: 1831
public class DeckTrayContent : MonoBehaviour
{
	// Token: 0x06004AD3 RID: 19155 RVA: 0x001668FE File Offset: 0x00164AFE
	public virtual void Show(bool showAll = false)
	{
	}

	// Token: 0x06004AD4 RID: 19156 RVA: 0x00166900 File Offset: 0x00164B00
	public virtual void Hide(bool hideAll = false)
	{
	}

	// Token: 0x06004AD5 RID: 19157 RVA: 0x00166902 File Offset: 0x00164B02
	public virtual void OnContentLoaded()
	{
	}

	// Token: 0x06004AD6 RID: 19158 RVA: 0x00166904 File Offset: 0x00164B04
	public virtual bool IsContentLoaded()
	{
		return true;
	}

	// Token: 0x06004AD7 RID: 19159 RVA: 0x00166907 File Offset: 0x00164B07
	public virtual bool PreAnimateContentEntrance()
	{
		return true;
	}

	// Token: 0x06004AD8 RID: 19160 RVA: 0x0016690A File Offset: 0x00164B0A
	public virtual bool PostAnimateContentEntrance()
	{
		return true;
	}

	// Token: 0x06004AD9 RID: 19161 RVA: 0x0016690D File Offset: 0x00164B0D
	public virtual bool AnimateContentEntranceStart()
	{
		return true;
	}

	// Token: 0x06004ADA RID: 19162 RVA: 0x00166910 File Offset: 0x00164B10
	public virtual bool AnimateContentEntranceEnd()
	{
		return true;
	}

	// Token: 0x06004ADB RID: 19163 RVA: 0x00166913 File Offset: 0x00164B13
	public virtual bool AnimateContentExitStart()
	{
		return true;
	}

	// Token: 0x06004ADC RID: 19164 RVA: 0x00166916 File Offset: 0x00164B16
	public virtual bool AnimateContentExitEnd()
	{
		return true;
	}

	// Token: 0x06004ADD RID: 19165 RVA: 0x00166919 File Offset: 0x00164B19
	public virtual bool PreAnimateContentExit()
	{
		return true;
	}

	// Token: 0x06004ADE RID: 19166 RVA: 0x0016691C File Offset: 0x00164B1C
	public virtual bool PostAnimateContentExit()
	{
		return true;
	}

	// Token: 0x06004ADF RID: 19167 RVA: 0x0016691F File Offset: 0x00164B1F
	public virtual void OnTaggedDeckChanged(CollectionManager.DeckTag tag, CollectionDeck newDeck, CollectionDeck oldDeck, bool isNewDeck)
	{
	}

	// Token: 0x06004AE0 RID: 19168 RVA: 0x00166921 File Offset: 0x00164B21
	public bool IsModeActive()
	{
		return this.m_isModeActive;
	}

	// Token: 0x06004AE1 RID: 19169 RVA: 0x00166929 File Offset: 0x00164B29
	public bool IsModeTryingOrActive()
	{
		return this.m_isModeTrying || this.m_isModeActive;
	}

	// Token: 0x06004AE2 RID: 19170 RVA: 0x0016693F File Offset: 0x00164B3F
	public void SetModeActive(bool active)
	{
		this.m_isModeActive = active;
	}

	// Token: 0x06004AE3 RID: 19171 RVA: 0x00166948 File Offset: 0x00164B48
	public void SetModeTrying(bool trying)
	{
		this.m_isModeTrying = trying;
	}

	// Token: 0x040031E4 RID: 12772
	private bool m_isModeActive;

	// Token: 0x040031E5 RID: 12773
	private bool m_isModeTrying;
}
