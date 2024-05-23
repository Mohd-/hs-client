using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000248 RID: 584
public class BoxDoor : MonoBehaviour
{
	// Token: 0x06002194 RID: 8596 RVA: 0x000A40DC File Offset: 0x000A22DC
	private void Awake()
	{
		this.m_startingPosition = base.gameObject.transform.localPosition;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000A40F4 File Offset: 0x000A22F4
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000A40FC File Offset: 0x000A22FC
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000A4105 File Offset: 0x000A2305
	public BoxDoorStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000A410D File Offset: 0x000A230D
	public void SetInfo(BoxDoorStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000A4116 File Offset: 0x000A2316
	public void EnableMaster(bool enable)
	{
		this.m_master = enable;
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000A411F File Offset: 0x000A231F
	public bool IsMaster()
	{
		return this.m_master;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000A4128 File Offset: 0x000A2328
	public bool ChangeState(BoxDoor.State state)
	{
		if (this.m_state == state)
		{
			return false;
		}
		this.m_state = state;
		if (state == BoxDoor.State.CLOSED)
		{
			this.m_parent.OnAnimStarted();
			Vector3 vector = this.m_info.m_ClosedRotation - this.m_info.m_OpenedRotation;
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				vector,
				"delay",
				this.m_info.m_ClosedDelaySec,
				"time",
				this.m_info.m_ClosedRotateSec,
				"easeType",
				this.m_info.m_ClosedRotateEaseType,
				"space",
				1,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.RotateAdd(base.gameObject, args);
			if (UniversalInputManager.UsePhoneUI)
			{
				args = iTween.Hash(new object[]
				{
					"position",
					this.m_startingPosition,
					"isLocal",
					true,
					"delay",
					this.m_info.m_ClosedDelaySec,
					"time",
					this.m_info.m_ClosedRotateSec,
					"easeType",
					this.m_info.m_ClosedRotateEaseType
				});
				iTween.MoveTo(base.gameObject, args);
			}
			if (this.IsMaster())
			{
				Spell eventSpell = this.m_parent.GetEventSpell(BoxEventType.DOORS_CLOSE);
				eventSpell.Activate();
				Spell eventSpell2 = this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_IN);
				eventSpell2.ActivateState(SpellStateType.BIRTH);
			}
		}
		else if (state == BoxDoor.State.OPENED)
		{
			this.m_parent.OnAnimStarted();
			Vector3 vector2 = this.m_info.m_OpenedRotation - this.m_info.m_ClosedRotation;
			Hashtable args2 = iTween.Hash(new object[]
			{
				"amount",
				vector2,
				"delay",
				this.m_info.m_OpenedDelaySec,
				"time",
				this.m_info.m_OpenedRotateSec,
				"easeType",
				this.m_info.m_OpenedRotateEaseType,
				"space",
				1,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.RotateAdd(base.gameObject, args2);
			if (UniversalInputManager.UsePhoneUI)
			{
				Vector3 startingPosition = this.m_startingPosition;
				startingPosition.x *= 1.038f;
				args2 = iTween.Hash(new object[]
				{
					"position",
					startingPosition,
					"isLocal",
					true,
					"delay",
					this.m_info.m_ClosedDelaySec,
					"time",
					this.m_info.m_ClosedRotateSec,
					"easeType",
					this.m_info.m_ClosedRotateEaseType
				});
				iTween.MoveTo(base.gameObject, args2);
			}
			if (this.IsMaster())
			{
				Spell eventSpell3 = this.m_parent.GetEventSpell(BoxEventType.DOORS_OPEN);
				eventSpell3.Activate();
				Spell eventSpell4 = this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_OUT);
				eventSpell4.ActivateState(SpellStateType.BIRTH);
			}
		}
		return true;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000A44E4 File Offset: 0x000A26E4
	public void UpdateState(BoxDoor.State state)
	{
		this.m_state = state;
		if (state == BoxDoor.State.CLOSED)
		{
			base.transform.localRotation = Quaternion.Euler(this.m_info.m_ClosedRotation);
			Spell eventSpell = this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_IN);
			eventSpell.ActivateState(SpellStateType.ACTION);
		}
		else if (state == BoxDoor.State.OPENED)
		{
			base.transform.localRotation = Quaternion.Euler(this.m_info.m_OpenedRotation);
			Spell eventSpell2 = this.m_parent.GetEventSpell(BoxEventType.SHADOW_FADE_OUT);
			eventSpell2.ActivateState(SpellStateType.ACTION);
		}
	}

	// Token: 0x040012F6 RID: 4854
	private const float BOX_SLIDE_PERCENTAGE_PHONE = 1.038f;

	// Token: 0x040012F7 RID: 4855
	private Box m_parent;

	// Token: 0x040012F8 RID: 4856
	private BoxDoorStateInfo m_info;

	// Token: 0x040012F9 RID: 4857
	private BoxDoor.State m_state;

	// Token: 0x040012FA RID: 4858
	private bool m_master;

	// Token: 0x040012FB RID: 4859
	private Vector3 m_startingPosition;

	// Token: 0x02000249 RID: 585
	public enum State
	{
		// Token: 0x040012FD RID: 4861
		CLOSED,
		// Token: 0x040012FE RID: 4862
		OPENED
	}
}
