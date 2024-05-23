using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC8 RID: 3528
	[ActionCategory("Pegasus")]
	[Tooltip("Instantly moves an object to a destination object's position or to a specified position vector.")]
	public class MoveToAction : FsmStateAction
	{
		// Token: 0x06006D41 RID: 27969 RVA: 0x00201FE0 File Offset: 0x002001E0
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_DestinationObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_VectorPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.m_Space = 0;
		}

		// Token: 0x06006D42 RID: 27970 RVA: 0x00202024 File Offset: 0x00200224
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				this.SetPosition(ownerDefaultTarget.transform);
			}
			base.Finish();
		}

		// Token: 0x06006D43 RID: 27971 RVA: 0x00202064 File Offset: 0x00200264
		private void SetPosition(Transform source)
		{
			Vector3 vector = (!this.m_VectorPosition.IsNone) ? this.m_VectorPosition.Value : Vector3.zero;
			if (!this.m_DestinationObject.IsNone && this.m_DestinationObject.Value != null)
			{
				Transform transform = this.m_DestinationObject.Value.transform;
				source.position = transform.position;
				if (this.m_Space == null)
				{
					source.position += vector;
				}
				else
				{
					source.localPosition += vector;
				}
			}
			else if (this.m_Space == null)
			{
				source.position = vector;
			}
			else
			{
				source.localPosition = vector;
			}
		}

		// Token: 0x040055F1 RID: 22001
		public FsmOwnerDefault m_GameObject;

		// Token: 0x040055F2 RID: 22002
		[Tooltip("Move to a destination object's position.")]
		public FsmGameObject m_DestinationObject;

		// Token: 0x040055F3 RID: 22003
		[Tooltip("Move to a specific position vector. If Destination Object is defined, this is used as an offset.")]
		public FsmVector3 m_VectorPosition;

		// Token: 0x040055F4 RID: 22004
		[Tooltip("Whether Vector Position is in local or world space.")]
		public Space m_Space;
	}
}
