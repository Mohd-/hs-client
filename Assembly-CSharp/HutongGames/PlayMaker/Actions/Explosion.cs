using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA0 RID: 2976
	[ActionCategory(9)]
	[Tooltip("Applies an explosion Force to all Game Objects with a Rigid Body inside a Radius.")]
	public class Explosion : FsmStateAction
	{
		// Token: 0x060063EB RID: 25579 RVA: 0x001DC57D File Offset: 0x001DA77D
		public override void Reset()
		{
			this.center = null;
			this.upwardsModifier = 0f;
			this.forceMode = 0;
			this.everyFrame = false;
		}

		// Token: 0x060063EC RID: 25580 RVA: 0x001DC5A4 File Offset: 0x001DA7A4
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x001DC5B2 File Offset: 0x001DA7B2
		public override void OnEnter()
		{
			this.DoExplosion();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063EE RID: 25582 RVA: 0x001DC5CB File Offset: 0x001DA7CB
		public override void OnFixedUpdate()
		{
			this.DoExplosion();
		}

		// Token: 0x060063EF RID: 25583 RVA: 0x001DC5D4 File Offset: 0x001DA7D4
		private void DoExplosion()
		{
			Collider[] array = Physics.OverlapSphere(this.center.Value, this.radius.Value);
			foreach (Collider collider in array)
			{
				Rigidbody component = collider.gameObject.GetComponent<Rigidbody>();
				if (component != null && this.ShouldApplyForce(collider.gameObject))
				{
					component.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
				}
			}
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x001DC67C File Offset: 0x001DA87C
		private bool ShouldApplyForce(GameObject go)
		{
			int num = ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value);
			return (1 << go.layer & num) > 0;
		}

		// Token: 0x04004B5C RID: 19292
		[Tooltip("The world position of the center of the explosion.")]
		[RequiredField]
		public FsmVector3 center;

		// Token: 0x04004B5D RID: 19293
		[Tooltip("The strength of the explosion.")]
		[RequiredField]
		public FsmFloat force;

		// Token: 0x04004B5E RID: 19294
		[RequiredField]
		[Tooltip("The radius of the explosion. Force falls of linearly with distance.")]
		public FsmFloat radius;

		// Token: 0x04004B5F RID: 19295
		[Tooltip("Applies the force as if it was applied from beneath the object. This is useful since explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
		public FsmFloat upwardsModifier;

		// Token: 0x04004B60 RID: 19296
		[Tooltip("The type of force to apply.")]
		public ForceMode forceMode;

		// Token: 0x04004B61 RID: 19297
		[UIHint(8)]
		public FsmInt layer;

		// Token: 0x04004B62 RID: 19298
		[UIHint(8)]
		[Tooltip("Layers to effect.")]
		public FsmInt[] layerMask;

		// Token: 0x04004B63 RID: 19299
		[Tooltip("Invert the mask, so you effect all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004B64 RID: 19300
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
