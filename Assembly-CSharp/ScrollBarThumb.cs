using System;

// Token: 0x020002B8 RID: 696
public class ScrollBarThumb : PegUIElement
{
	// Token: 0x06002598 RID: 9624 RVA: 0x000B83F9 File Offset: 0x000B65F9
	private void Update()
	{
		if (this.IsDragging() && UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			this.StopDragging();
		}
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000B841C File Offset: 0x000B661C
	public bool IsDragging()
	{
		return this.m_isDragging;
	}

	// Token: 0x0600259A RID: 9626 RVA: 0x000B8424 File Offset: 0x000B6624
	public void StartDragging()
	{
		this.m_isDragging = true;
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000B842D File Offset: 0x000B662D
	public void StopDragging()
	{
		this.m_isDragging = false;
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000B8436 File Offset: 0x000B6636
	protected override void OnHold()
	{
		this.StartDragging();
	}

	// Token: 0x0400163F RID: 5695
	private bool m_isDragging;
}
