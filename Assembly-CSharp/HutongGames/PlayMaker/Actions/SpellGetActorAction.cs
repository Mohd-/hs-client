using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF3 RID: 3571
	[ActionCategory("Pegasus")]
	[Tooltip("Put a Spell's Source or Target Actor into a GameObject variable.")]
	public class SpellGetActorAction : SpellAction
	{
		// Token: 0x06006DED RID: 28141 RVA: 0x00204810 File Offset: 0x00202A10
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DEE RID: 28142 RVA: 0x00204823 File Offset: 0x00202A23
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichActor = SpellAction.Which.SOURCE;
			this.m_GameObject = null;
		}

		// Token: 0x06006DEF RID: 28143 RVA: 0x0020483C File Offset: 0x00202A3C
		public override void OnEnter()
		{
			base.OnEnter();
			Actor actor = base.GetActor(this.m_WhichActor);
			if (actor == null)
			{
				Error.AddDevFatal("SpellGetActorAction.OnEnter() - Actor not found!", new object[0]);
				base.Finish();
				return;
			}
			if (!this.m_GameObject.IsNone)
			{
				this.m_GameObject.Value = actor.gameObject;
			}
			base.Finish();
		}

		// Token: 0x04005693 RID: 22163
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005694 RID: 22164
		public SpellAction.Which m_WhichActor;

		// Token: 0x04005695 RID: 22165
		public FsmGameObject m_GameObject;
	}
}
