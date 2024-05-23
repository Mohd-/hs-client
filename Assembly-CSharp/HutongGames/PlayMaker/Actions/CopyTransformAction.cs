using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB2 RID: 3506
	[Tooltip("Copies a game object's transform to another game object.")]
	[ActionCategory("Pegasus")]
	public class CopyTransformAction : FsmStateAction
	{
		// Token: 0x06006CE0 RID: 27872 RVA: 0x00200634 File Offset: 0x001FE834
		public override void Reset()
		{
			this.m_SourceObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_TargetObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_Position = true;
			this.m_Rotation = true;
			this.m_Scale = true;
			this.m_WorldSpace = true;
		}

		// Token: 0x06006CE1 RID: 27873 RVA: 0x0020069C File Offset: 0x001FE89C
		public override void OnEnter()
		{
			if (this.m_SourceObject == null || this.m_SourceObject.IsNone || !this.m_SourceObject.Value || this.m_TargetObject == null || this.m_TargetObject.IsNone || !this.m_TargetObject.Value)
			{
				base.Finish();
				return;
			}
			Transform transform = this.m_SourceObject.Value.transform;
			Transform transform2 = this.m_TargetObject.Value.transform;
			if (this.m_WorldSpace.Value)
			{
				if (this.m_Position.Value)
				{
					transform2.position = transform.position;
				}
				if (this.m_Rotation.Value)
				{
					transform2.rotation = transform.rotation;
				}
				if (this.m_Scale.Value)
				{
					TransformUtil.CopyWorldScale(transform2, transform);
				}
			}
			else
			{
				if (this.m_Position.Value)
				{
					transform2.localPosition = transform.localPosition;
				}
				if (this.m_Rotation.Value)
				{
					transform2.localRotation = transform.localRotation;
				}
				if (this.m_Scale.Value)
				{
					transform2.localScale = transform.localScale;
				}
			}
			base.Finish();
		}

		// Token: 0x0400558C RID: 21900
		[RequiredField]
		public FsmGameObject m_SourceObject;

		// Token: 0x0400558D RID: 21901
		[RequiredField]
		public FsmGameObject m_TargetObject;

		// Token: 0x0400558E RID: 21902
		public FsmBool m_Position;

		// Token: 0x0400558F RID: 21903
		public FsmBool m_Rotation;

		// Token: 0x04005590 RID: 21904
		public FsmBool m_Scale;

		// Token: 0x04005591 RID: 21905
		[Tooltip("Copies the transform in world space if checked, otherwise copies in local space.")]
		public FsmBool m_WorldSpace;
	}
}
