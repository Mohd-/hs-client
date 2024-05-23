using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D19 RID: 3353
	[Tooltip("Rotates a Vector3 direction from Current towards Target.")]
	[ActionCategory(19)]
	public class Vector3RotateTowards : FsmStateAction
	{
		// Token: 0x06006A1C RID: 27164 RVA: 0x001F10F8 File Offset: 0x001EF2F8
		public override void Reset()
		{
			this.currentDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.rotateSpeed = 360f;
			this.maxMagnitude = 1f;
		}

		// Token: 0x06006A1D RID: 27165 RVA: 0x001F1150 File Offset: 0x001EF350
		public override void OnUpdate()
		{
			this.currentDirection.Value = Vector3.RotateTowards(this.currentDirection.Value, this.targetDirection.Value, this.rotateSpeed.Value * 0.017453292f * Time.deltaTime, this.maxMagnitude.Value);
		}

		// Token: 0x040051F5 RID: 20981
		[RequiredField]
		public FsmVector3 currentDirection;

		// Token: 0x040051F6 RID: 20982
		[RequiredField]
		public FsmVector3 targetDirection;

		// Token: 0x040051F7 RID: 20983
		[RequiredField]
		[Tooltip("Rotation speed in degrees per second")]
		public FsmFloat rotateSpeed;

		// Token: 0x040051F8 RID: 20984
		[Tooltip("Max Magnitude per second")]
		[RequiredField]
		public FsmFloat maxMagnitude;
	}
}
