using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B39 RID: 2873
	[ActionCategory(9)]
	[Tooltip("Applies a force to a Game Object that simulates explosion effects. The explosion force will fall off linearly with distance. Hint: Use the Explosion Action instead to apply an explosion force to all objects in a blast radius.")]
	public class AddExplosionForce : ComponentAction<Rigidbody>
	{
		// Token: 0x06006221 RID: 25121 RVA: 0x001D3064 File Offset: 0x001D1264
		public override void Reset()
		{
			this.gameObject = null;
			this.center = new FsmVector3
			{
				UseVariable = true
			};
			this.upwardsModifier = 0f;
			this.forceMode = 0;
			this.everyFrame = false;
		}

		// Token: 0x06006222 RID: 25122 RVA: 0x001D30AA File Offset: 0x001D12AA
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006223 RID: 25123 RVA: 0x001D30B8 File Offset: 0x001D12B8
		public override void OnEnter()
		{
			this.DoAddExplosionForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006224 RID: 25124 RVA: 0x001D30D1 File Offset: 0x001D12D1
		public override void OnFixedUpdate()
		{
			this.DoAddExplosionForce();
		}

		// Token: 0x06006225 RID: 25125 RVA: 0x001D30DC File Offset: 0x001D12DC
		private void DoAddExplosionForce()
		{
			GameObject go = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (this.center == null || !base.UpdateCache(go))
			{
				return;
			}
			base.rigidbody.AddExplosionForce(this.force.Value, this.center.Value, this.radius.Value, this.upwardsModifier.Value, this.forceMode);
		}

		// Token: 0x04004934 RID: 18740
		[Tooltip("The GameObject to add the explosion force to.")]
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004935 RID: 18741
		[RequiredField]
		[Tooltip("The center of the explosion. Hint: this is often the position returned from a GetCollisionInfo action.")]
		public FsmVector3 center;

		// Token: 0x04004936 RID: 18742
		[RequiredField]
		[Tooltip("The strength of the explosion.")]
		public FsmFloat force;

		// Token: 0x04004937 RID: 18743
		[RequiredField]
		[Tooltip("The radius of the explosion. Force falls off linearly with distance.")]
		public FsmFloat radius;

		// Token: 0x04004938 RID: 18744
		[Tooltip("Applies the force as if it was applied from beneath the object. This is useful since explosions that throw things up instead of pushing things to the side look cooler. A value of 2 will apply a force as if it is applied from 2 meters below while not changing the actual explosion position.")]
		public FsmFloat upwardsModifier;

		// Token: 0x04004939 RID: 18745
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x0400493A RID: 18746
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
