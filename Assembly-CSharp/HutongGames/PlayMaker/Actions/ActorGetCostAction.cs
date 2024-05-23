using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D93 RID: 3475
	[Tooltip("Put an Actor's cost data into variables.")]
	[ActionCategory("Pegasus")]
	public class ActorGetCostAction : ActorAction
	{
		// Token: 0x06006C61 RID: 27745 RVA: 0x001FE32C File Offset: 0x001FC52C
		protected override GameObject GetActorOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_ActorObject);
		}

		// Token: 0x06006C62 RID: 27746 RVA: 0x001FE340 File Offset: 0x001FC540
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_ManaGem = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_UberText = new FsmGameObject
			{
				UseVariable = true
			};
			this.m_Cost = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x06006C63 RID: 27747 RVA: 0x001FE390 File Offset: 0x001FC590
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				base.Finish();
				return;
			}
			if (!this.m_ManaGem.IsNone)
			{
				this.m_ManaGem.Value = this.m_actor.m_manaObject;
			}
			if (!this.m_UberText.IsNone)
			{
				this.m_UberText.Value = this.m_actor.GetCostTextObject();
			}
			if (!this.m_Cost.IsNone)
			{
				Entity entity = this.m_actor.GetEntity();
				if (entity != null)
				{
					this.m_Cost.Value = entity.GetCost();
				}
				else
				{
					EntityDef entityDef = this.m_actor.GetEntityDef();
					if (entityDef != null)
					{
						this.m_Cost.Value = entityDef.GetCost();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x040054FD RID: 21757
		public FsmOwnerDefault m_ActorObject;

		// Token: 0x040054FE RID: 21758
		public FsmGameObject m_ManaGem;

		// Token: 0x040054FF RID: 21759
		public FsmGameObject m_UberText;

		// Token: 0x04005500 RID: 21760
		public FsmInt m_Cost;
	}
}
