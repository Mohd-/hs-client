using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD2 RID: 3538
	[Tooltip("Randomly picks an object from a list and creates it. THE CREATED OBJECT MUST BE DESTROYED!")]
	[ActionCategory("Pegasus")]
	public class RandomCreateObjectAction : FsmStateAction
	{
		// Token: 0x06006D64 RID: 28004 RVA: 0x00202854 File Offset: 0x00200A54
		public override void Reset()
		{
			this.m_CreatedObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_Objects = new FsmGameObject[3];
			this.m_Weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
		}

		// Token: 0x06006D65 RID: 28005 RVA: 0x002028B4 File Offset: 0x00200AB4
		public override void OnEnter()
		{
			if (this.m_Objects == null || this.m_Objects.Length == 0)
			{
				base.Finish();
				return;
			}
			GameObject gameObject = null;
			FsmGameObject fsmGameObject = this.m_Objects[ActionHelpers.GetRandomWeightedIndex(this.m_Weights)];
			if (fsmGameObject != null && !fsmGameObject.IsNone && fsmGameObject.Value)
			{
				gameObject = Object.Instantiate<GameObject>(fsmGameObject.Value);
				TransformUtil.CopyWorld(gameObject, fsmGameObject.Value);
			}
			if (this.m_CreatedObject != null)
			{
				this.m_CreatedObject.Value = gameObject;
			}
			base.Finish();
		}

		// Token: 0x04005610 RID: 22032
		[Tooltip("The created object gets stored in this variable.")]
		[UIHint(10)]
		public FsmGameObject m_CreatedObject;

		// Token: 0x04005611 RID: 22033
		[CompoundArray("Random Objects", "Object", "Weight")]
		public FsmGameObject[] m_Objects;

		// Token: 0x04005612 RID: 22034
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] m_Weights;
	}
}
