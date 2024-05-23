using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000DE1 RID: 3553
[Tooltip("Shake Minions")]
[ActionCategory("Pegasus")]
public class ShakeMinionsAction : FsmStateAction
{
	// Token: 0x06006DA5 RID: 28069 RVA: 0x00203990 File Offset: 0x00201B90
	public override void Reset()
	{
		this.gameObject = null;
		this.MinionsToShake = ShakeMinionsAction.MinionsToShakeEnum.All;
		this.shakeType = ShakeMinionType.RandomDirection;
		this.shakeSize = ShakeMinionIntensity.SmallShake;
		this.customShakeIntensity = 0.1f;
		this.radius = 0f;
	}

	// Token: 0x06006DA6 RID: 28070 RVA: 0x002039D9 File Offset: 0x00201BD9
	public override void OnEnter()
	{
		this.DoShakeMinions();
		base.Finish();
	}

	// Token: 0x06006DA7 RID: 28071 RVA: 0x002039E8 File Offset: 0x00201BE8
	private void DoShakeMinions()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
		if (ownerDefaultTarget == null)
		{
			base.Finish();
			return;
		}
		if (this.MinionsToShake == ShakeMinionsAction.MinionsToShakeEnum.All)
		{
			MinionShake.ShakeAllMinions(ownerDefaultTarget, this.shakeType, ownerDefaultTarget.transform.position, this.shakeSize, this.customShakeIntensity.Value, this.radius.Value, 0f);
		}
		else if (this.MinionsToShake == ShakeMinionsAction.MinionsToShakeEnum.Target)
		{
			MinionShake.ShakeTargetMinion(ownerDefaultTarget, this.shakeType, ownerDefaultTarget.transform.position, this.shakeSize, this.customShakeIntensity.Value, 0f, 0f);
		}
		else if (this.MinionsToShake == ShakeMinionsAction.MinionsToShakeEnum.SelectedGameObject)
		{
			MinionShake.ShakeObject(ownerDefaultTarget, this.shakeType, ownerDefaultTarget.transform.position, this.shakeSize, this.customShakeIntensity.Value, 0f, 0f);
		}
	}

	// Token: 0x0400564E RID: 22094
	[RequiredField]
	[Tooltip("Impact Object Location")]
	public FsmOwnerDefault gameObject;

	// Token: 0x0400564F RID: 22095
	[RequiredField]
	[Tooltip("Shake Type")]
	public ShakeMinionType shakeType = ShakeMinionType.RandomDirection;

	// Token: 0x04005650 RID: 22096
	[Tooltip("Minions To Shake")]
	[RequiredField]
	public ShakeMinionsAction.MinionsToShakeEnum MinionsToShake;

	// Token: 0x04005651 RID: 22097
	[RequiredField]
	[Tooltip("Shake Intensity")]
	public ShakeMinionIntensity shakeSize = ShakeMinionIntensity.SmallShake;

	// Token: 0x04005652 RID: 22098
	[Tooltip("Custom Shake Intensity 0-1. Used when Shake Size is Custom")]
	[RequiredField]
	public FsmFloat customShakeIntensity;

	// Token: 0x04005653 RID: 22099
	[RequiredField]
	[Tooltip("Radius - 0 = for all objects")]
	public FsmFloat radius;

	// Token: 0x02000DE2 RID: 3554
	public enum MinionsToShakeEnum
	{
		// Token: 0x04005655 RID: 22101
		All,
		// Token: 0x04005656 RID: 22102
		Target,
		// Token: 0x04005657 RID: 22103
		SelectedGameObject
	}
}
