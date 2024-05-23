using System;
using UnityEngine;

// Token: 0x02000537 RID: 1335
[CustomEditClass]
public class GeneralStorePane : MonoBehaviour
{
	// Token: 0x06003DE5 RID: 15845 RVA: 0x0012BC6C File Offset: 0x00129E6C
	public void Refresh()
	{
		this.OnRefresh();
	}

	// Token: 0x06003DE6 RID: 15846 RVA: 0x0012BC74 File Offset: 0x00129E74
	public virtual bool AnimateEntranceStart()
	{
		return true;
	}

	// Token: 0x06003DE7 RID: 15847 RVA: 0x0012BC77 File Offset: 0x00129E77
	public virtual bool AnimateEntranceEnd()
	{
		return true;
	}

	// Token: 0x06003DE8 RID: 15848 RVA: 0x0012BC7A File Offset: 0x00129E7A
	public virtual bool AnimateExitStart()
	{
		return true;
	}

	// Token: 0x06003DE9 RID: 15849 RVA: 0x0012BC7D File Offset: 0x00129E7D
	public virtual bool AnimateExitEnd()
	{
		return true;
	}

	// Token: 0x06003DEA RID: 15850 RVA: 0x0012BC80 File Offset: 0x00129E80
	public virtual void PrePaneSwappedIn()
	{
	}

	// Token: 0x06003DEB RID: 15851 RVA: 0x0012BC82 File Offset: 0x00129E82
	public virtual void PostPaneSwappedIn()
	{
	}

	// Token: 0x06003DEC RID: 15852 RVA: 0x0012BC84 File Offset: 0x00129E84
	public virtual void PrePaneSwappedOut()
	{
	}

	// Token: 0x06003DED RID: 15853 RVA: 0x0012BC86 File Offset: 0x00129E86
	public virtual void PostPaneSwappedOut()
	{
	}

	// Token: 0x06003DEE RID: 15854 RVA: 0x0012BC88 File Offset: 0x00129E88
	public virtual void OnPurchaseFinished()
	{
	}

	// Token: 0x06003DEF RID: 15855 RVA: 0x0012BC8A File Offset: 0x00129E8A
	public virtual void StoreShown(bool isCurrent)
	{
	}

	// Token: 0x06003DF0 RID: 15856 RVA: 0x0012BC8C File Offset: 0x00129E8C
	public virtual void StoreHidden(bool isCurrent)
	{
	}

	// Token: 0x06003DF1 RID: 15857 RVA: 0x0012BC8E File Offset: 0x00129E8E
	protected virtual void OnRefresh()
	{
	}

	// Token: 0x0400277A RID: 10106
	public GeneralStoreContent m_parentContent;

	// Token: 0x0400277B RID: 10107
	public GameObject m_paneContainer;
}
