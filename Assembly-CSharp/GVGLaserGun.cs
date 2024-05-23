using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000658 RID: 1624
[CustomEditClass]
public class GVGLaserGun : MonoBehaviour
{
	// Token: 0x06004580 RID: 17792 RVA: 0x0014DA88 File Offset: 0x0014BC88
	private void Awake()
	{
		if (this.m_AngleDefs.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_AngleDefs.Count; i++)
		{
			this.m_sortedAngleDefIndexes.Add(i);
		}
		this.m_sortedAngleDefIndexes.Sort(delegate(int index1, int index2)
		{
			GVGLaserGun.AngleDef def = this.m_AngleDefs[index1];
			GVGLaserGun.AngleDef def2 = this.m_AngleDefs[index2];
			return GVGLaserGun.AngleDefSortComparison(def, def2);
		});
		this.m_angleIndex = 0;
		this.m_minAngleIndex = 0;
		this.m_maxAngleIndex = 0;
		float angle = this.m_AngleDefs[0].m_Angle;
		float num = angle;
		for (int j = 0; j < this.m_sortedAngleDefIndexes.Count; j++)
		{
			GVGLaserGun.AngleDef angleDef = this.m_AngleDefs[this.m_sortedAngleDefIndexes[j]];
			if (angleDef.m_Angle < angle)
			{
				angle = angleDef.m_Angle;
				this.m_minAngleIndex = j;
			}
			if (angleDef.m_Angle > num)
			{
				num = angleDef.m_Angle;
				this.m_maxAngleIndex = j;
			}
			if (angleDef.m_Default)
			{
				this.m_angleIndex = j;
				this.SetAngle(angleDef.m_Angle);
			}
		}
	}

	// Token: 0x06004581 RID: 17793 RVA: 0x0014DB9B File Offset: 0x0014BD9B
	private void Update()
	{
		this.HandleRotation();
		this.HandleLever();
	}

	// Token: 0x06004582 RID: 17794 RVA: 0x0014DBAC File Offset: 0x0014BDAC
	private GVGLaserGun.AngleDef GetAngleDef()
	{
		if (this.m_angleIndex < 0)
		{
			return null;
		}
		if (this.m_angleIndex >= this.m_sortedAngleDefIndexes.Count)
		{
			return null;
		}
		int num = this.m_sortedAngleDefIndexes[this.m_angleIndex];
		return this.m_AngleDefs[num];
	}

	// Token: 0x06004583 RID: 17795 RVA: 0x0014DBFD File Offset: 0x0014BDFD
	private static int AngleDefSortComparison(GVGLaserGun.AngleDef def1, GVGLaserGun.AngleDef def2)
	{
		if (def1.m_Angle < def2.m_Angle)
		{
			return -1;
		}
		if (def1.m_Angle > def2.m_Angle)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06004584 RID: 17796 RVA: 0x0014DC28 File Offset: 0x0014BE28
	private void HandleRotation()
	{
		if (this.m_leverEffectsActive)
		{
			return;
		}
		this.m_requestedRotationDir = 0;
		if (UniversalInputManager.Get().GetMouseButton(0))
		{
			if (this.IsOver(this.m_RotateLeftButton))
			{
				this.m_requestedRotationDir = -1;
			}
			else if (this.IsOver(this.m_RotateRightButton))
			{
				this.m_requestedRotationDir = 1;
			}
		}
		if (this.ShouldStartRotating())
		{
			this.StartRotating(this.m_requestedRotationDir);
		}
		if (this.m_rotationDir < 0)
		{
			this.RotateLeft();
		}
		else if (this.m_rotationDir > 0)
		{
			this.RotateRight();
		}
	}

	// Token: 0x06004585 RID: 17797 RVA: 0x0014DCD0 File Offset: 0x0014BED0
	private bool ShouldStartRotating()
	{
		return this.m_requestedRotationDir != 0 && this.m_requestedRotationDir != this.m_rotationDir && (this.m_requestedRotationDir >= 0 || this.m_angleIndex != this.m_minAngleIndex) && (this.m_requestedRotationDir <= 0 || this.m_angleIndex != this.m_maxAngleIndex);
	}

	// Token: 0x06004586 RID: 17798 RVA: 0x0014DD3C File Offset: 0x0014BF3C
	private void RotateLeft()
	{
		GVGLaserGun.AngleDef angleDef = this.GetAngleDef();
		float num = Mathf.MoveTowards(this.m_angle, angleDef.m_Angle, this.m_RotationSpeed * Time.deltaTime);
		if (num <= angleDef.m_Angle)
		{
			if (this.m_requestedRotationDir == 0 || this.m_angleIndex == this.m_minAngleIndex)
			{
				this.SetAngle(num);
				this.StopRotating();
			}
			else
			{
				this.m_angleIndex--;
			}
		}
		else
		{
			this.SetAngle(num);
		}
	}

	// Token: 0x06004587 RID: 17799 RVA: 0x0014DDC4 File Offset: 0x0014BFC4
	private void RotateRight()
	{
		GVGLaserGun.AngleDef angleDef = this.GetAngleDef();
		float num = Mathf.MoveTowards(this.m_angle, angleDef.m_Angle, this.m_RotationSpeed * Time.deltaTime);
		if (num >= angleDef.m_Angle)
		{
			if (this.m_requestedRotationDir == 0 || this.m_angleIndex == this.m_maxAngleIndex)
			{
				this.SetAngle(num);
				this.StopRotating();
			}
			else
			{
				this.m_angleIndex++;
			}
		}
		else
		{
			this.SetAngle(num);
		}
	}

	// Token: 0x06004588 RID: 17800 RVA: 0x0014DE4C File Offset: 0x0014C04C
	private void StartRotating(int dir)
	{
		this.m_rotationDir = dir;
		if (dir < 0)
		{
			this.m_angleIndex--;
		}
		else
		{
			this.m_angleIndex++;
		}
		if (this.m_StartRotationSpell)
		{
			this.m_StartRotationSpell.Activate();
		}
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x0014DEA4 File Offset: 0x0014C0A4
	private void StopRotating()
	{
		this.m_rotationDir = 0;
		if (this.m_StopRotationSpell)
		{
			this.m_StopRotationSpell.Activate();
		}
	}

	// Token: 0x0600458A RID: 17802 RVA: 0x0014DED3 File Offset: 0x0014C0D3
	private void SetAngle(float angle)
	{
		this.m_angle = angle;
		TransformUtil.SetLocalEulerAngleY(this.m_GunRotator, this.m_angle);
	}

	// Token: 0x0600458B RID: 17803 RVA: 0x0014DEF0 File Offset: 0x0014C0F0
	private void HandleLever()
	{
		if (this.m_rotationDir != 0)
		{
			return;
		}
		if (this.m_leverEffectsActive)
		{
			return;
		}
		if (UniversalInputManager.Get().GetMouseButtonUp(0) && this.IsOver(this.m_GunLever))
		{
			this.PullLever();
		}
	}

	// Token: 0x0600458C RID: 17804 RVA: 0x0014DF3C File Offset: 0x0014C13C
	private void PullLever()
	{
		if (!this.m_PullLeverSpell)
		{
			return;
		}
		this.m_leverEffectsActive = true;
		this.m_PullLeverSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnPullLeverSpellFinished));
		this.m_PullLeverSpell.Activate();
	}

	// Token: 0x0600458D RID: 17805 RVA: 0x0014DF84 File Offset: 0x0014C184
	private void OnPullLeverSpellFinished(Spell spell, object userData)
	{
		GVGLaserGun.AngleDef angleDef = this.GetAngleDef();
		Spell impactSpell = angleDef.m_ImpactSpell;
		if (!impactSpell)
		{
			this.m_leverEffectsActive = false;
			return;
		}
		impactSpell.AddFinishedCallback(new Spell.FinishedCallback(this.OnImpactSpellFinished));
		impactSpell.Activate();
	}

	// Token: 0x0600458E RID: 17806 RVA: 0x0014DFCA File Offset: 0x0014C1CA
	private void OnImpactSpellFinished(Spell spell, object userData)
	{
		this.m_leverEffectsActive = false;
	}

	// Token: 0x0600458F RID: 17807 RVA: 0x0014DFD4 File Offset: 0x0014C1D4
	private bool IsOver(GameObject go)
	{
		return go && InputUtil.IsPlayMakerMouseInputAllowed(go) && UniversalInputManager.Get().InputIsOver(go);
	}

	// Token: 0x04002C78 RID: 11384
	[CustomEditField(Sections = "Lever")]
	public GameObject m_GunLever;

	// Token: 0x04002C79 RID: 11385
	[CustomEditField(Sections = "Lever")]
	public Spell m_PullLeverSpell;

	// Token: 0x04002C7A RID: 11386
	[CustomEditField(Sections = "Rotation")]
	[Tooltip("The thing that will be rotated.")]
	public GameObject m_GunRotator;

	// Token: 0x04002C7B RID: 11387
	[CustomEditField(Sections = "Rotation")]
	public GameObject m_RotateLeftButton;

	// Token: 0x04002C7C RID: 11388
	[CustomEditField(Sections = "Rotation")]
	public GameObject m_RotateRightButton;

	// Token: 0x04002C7D RID: 11389
	[CustomEditField(Sections = "Rotation")]
	public Spell m_StartRotationSpell;

	// Token: 0x04002C7E RID: 11390
	[CustomEditField(Sections = "Rotation")]
	public Spell m_StopRotationSpell;

	// Token: 0x04002C7F RID: 11391
	[CustomEditField(Sections = "Rotation")]
	[Tooltip("How fast the gun rotates in degrees per second.")]
	public float m_RotationSpeed;

	// Token: 0x04002C80 RID: 11392
	[CustomEditField(Sections = "Rotation", ListTable = true)]
	public List<GVGLaserGun.AngleDef> m_AngleDefs = new List<GVGLaserGun.AngleDef>();

	// Token: 0x04002C81 RID: 11393
	[CustomEditField(Sections = "Debug")]
	public bool m_DebugShowGunAngle;

	// Token: 0x04002C82 RID: 11394
	private List<int> m_sortedAngleDefIndexes = new List<int>();

	// Token: 0x04002C83 RID: 11395
	private int m_rotationDir;

	// Token: 0x04002C84 RID: 11396
	private int m_requestedRotationDir;

	// Token: 0x04002C85 RID: 11397
	private float m_angle;

	// Token: 0x04002C86 RID: 11398
	private int m_angleIndex = -1;

	// Token: 0x04002C87 RID: 11399
	private int m_minAngleIndex = -1;

	// Token: 0x04002C88 RID: 11400
	private int m_maxAngleIndex = -1;

	// Token: 0x04002C89 RID: 11401
	private bool m_leverEffectsActive;

	// Token: 0x02000659 RID: 1625
	[Serializable]
	public class AngleDef
	{
		// Token: 0x06004592 RID: 17810 RVA: 0x0014E046 File Offset: 0x0014C246
		public int CustomBehaviorListCompare(GVGLaserGun.AngleDef def1, GVGLaserGun.AngleDef def2)
		{
			return GVGLaserGun.AngleDefSortComparison(def1, def2);
		}

		// Token: 0x04002C8A RID: 11402
		public bool m_Default;

		// Token: 0x04002C8B RID: 11403
		[CustomEditField(ListSortable = true)]
		public float m_Angle;

		// Token: 0x04002C8C RID: 11404
		public Spell m_ImpactSpell;
	}
}
