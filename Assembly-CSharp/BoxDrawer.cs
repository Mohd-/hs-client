using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200024C RID: 588
public class BoxDrawer : MonoBehaviour
{
	// Token: 0x060021A5 RID: 8613 RVA: 0x000A4850 File Offset: 0x000A2A50
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000A4858 File Offset: 0x000A2A58
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000A4861 File Offset: 0x000A2A61
	public BoxDrawerStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000A4869 File Offset: 0x000A2A69
	public void SetInfo(BoxDrawerStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x060021A9 RID: 8617 RVA: 0x000A4874 File Offset: 0x000A2A74
	public bool ChangeState(BoxDrawer.State state)
	{
		if (DemoMgr.Get().GetMode() == DemoMode.PAX_EAST_2013)
		{
			return true;
		}
		if (DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2013)
		{
			return true;
		}
		if (this.m_state == state)
		{
			return false;
		}
		BoxDrawer.State state2 = this.m_state;
		this.m_state = state;
		if (this.IsInactiveState(state2) && this.IsInactiveState(this.m_state))
		{
			return true;
		}
		base.gameObject.SetActive(true);
		if (state == BoxDrawer.State.CLOSED)
		{
			this.m_parent.OnAnimStarted();
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_info.m_ClosedBone.transform.position,
				"delay",
				this.m_info.m_ClosedDelaySec,
				"time",
				this.m_info.m_ClosedMoveSec,
				"easeType",
				this.m_info.m_ClosedMoveEaseType,
				"oncomplete",
				"OnClosedAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(base.gameObject, args);
			Spell eventSpell = this.m_parent.GetEventSpell(BoxEventType.DRAWER_CLOSE);
			eventSpell.Activate();
		}
		else if (state == BoxDrawer.State.CLOSED_BOX_OPENED)
		{
			this.m_parent.OnAnimStarted();
			Hashtable args2 = iTween.Hash(new object[]
			{
				"position",
				this.m_info.m_ClosedBoxOpenedBone.transform.position,
				"delay",
				this.m_info.m_ClosedBoxOpenedDelaySec,
				"time",
				this.m_info.m_ClosedBoxOpenedMoveSec,
				"easeType",
				this.m_info.m_ClosedBoxOpenedMoveEaseType,
				"oncomplete",
				"OnClosedBoxOpenedAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(base.gameObject, args2);
			Spell eventSpell2 = this.m_parent.GetEventSpell(BoxEventType.DRAWER_CLOSE);
			eventSpell2.Activate();
		}
		else if (state == BoxDrawer.State.OPENED)
		{
			this.m_parent.OnAnimStarted();
			Hashtable args3 = iTween.Hash(new object[]
			{
				"position",
				this.m_info.m_OpenedBone.transform.position,
				"delay",
				this.m_info.m_OpenedDelaySec,
				"time",
				this.m_info.m_OpenedMoveSec,
				"easeType",
				this.m_info.m_OpenedMoveEaseType,
				"oncomplete",
				"OnOpenedAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
			iTween.MoveTo(base.gameObject, args3);
			Spell eventSpell3 = this.m_parent.GetEventSpell(BoxEventType.DRAWER_OPEN);
			eventSpell3.Activate();
		}
		return true;
	}

	// Token: 0x060021AA RID: 8618 RVA: 0x000A4B88 File Offset: 0x000A2D88
	public void UpdateState(BoxDrawer.State state)
	{
		this.m_state = state;
		if (state == BoxDrawer.State.CLOSED)
		{
			base.transform.position = this.m_info.m_ClosedBone.transform.position;
			base.gameObject.SetActive(false);
		}
		else if (state == BoxDrawer.State.CLOSED_BOX_OPENED)
		{
			base.transform.position = this.m_info.m_ClosedBoxOpenedBone.transform.position;
			base.gameObject.SetActive(false);
		}
		else if (state == BoxDrawer.State.OPENED)
		{
			base.transform.position = this.m_info.m_OpenedBone.transform.position;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x060021AB RID: 8619 RVA: 0x000A4C3E File Offset: 0x000A2E3E
	private bool IsInactiveState(BoxDrawer.State state)
	{
		return state == BoxDrawer.State.CLOSED || state == BoxDrawer.State.CLOSED_BOX_OPENED;
	}

	// Token: 0x060021AC RID: 8620 RVA: 0x000A4C4D File Offset: 0x000A2E4D
	private void OnClosedAnimFinished()
	{
		base.gameObject.SetActive(false);
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x060021AD RID: 8621 RVA: 0x000A4C66 File Offset: 0x000A2E66
	private void OnClosedBoxOpenedAnimFinished()
	{
		base.gameObject.SetActive(false);
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x060021AE RID: 8622 RVA: 0x000A4C7F File Offset: 0x000A2E7F
	private void OnOpenedAnimFinished()
	{
		base.gameObject.SetActive(true);
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x04001305 RID: 4869
	private Box m_parent;

	// Token: 0x04001306 RID: 4870
	private BoxDrawerStateInfo m_info;

	// Token: 0x04001307 RID: 4871
	private BoxDrawer.State m_state;

	// Token: 0x0200024D RID: 589
	public enum State
	{
		// Token: 0x04001309 RID: 4873
		CLOSED,
		// Token: 0x0400130A RID: 4874
		CLOSED_BOX_OPENED,
		// Token: 0x0400130B RID: 4875
		OPENED
	}
}
