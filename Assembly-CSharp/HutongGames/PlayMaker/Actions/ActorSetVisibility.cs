using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D97 RID: 3479
	[ActionCategory("Pegasus")]
	[Tooltip("Show or Hide an Actor without messing up the game.")]
	public class ActorSetVisibility : ActorAction
	{
		// Token: 0x06006C71 RID: 27761 RVA: 0x001FE6C6 File Offset: 0x001FC8C6
		protected override GameObject GetActorOwner()
		{
			return this.m_ActorObject.Value;
		}

		// Token: 0x06006C72 RID: 27762 RVA: 0x001FE6D4 File Offset: 0x001FC8D4
		public override void Reset()
		{
			this.m_ActorObject = null;
			this.m_Visible = false;
			this.m_IgnoreSpells = false;
			this.m_ResetOnExit = false;
		}

		// Token: 0x06006C73 RID: 27763 RVA: 0x001FE708 File Offset: 0x001FC908
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.m_actor == null)
			{
				base.Finish();
				return;
			}
			this.m_initialVisibility = this.m_actor.IsShown();
			if (this.m_Visible.Value)
			{
				this.ShowActor();
			}
			else
			{
				this.HideActor();
			}
			base.Finish();
		}

		// Token: 0x06006C74 RID: 27764 RVA: 0x001FE76C File Offset: 0x001FC96C
		public override void OnExit()
		{
			if (!this.m_ResetOnExit)
			{
				return;
			}
			if (this.m_initialVisibility)
			{
				this.ShowActor();
			}
			else
			{
				this.HideActor();
			}
		}

		// Token: 0x06006C75 RID: 27765 RVA: 0x001FE7A1 File Offset: 0x001FC9A1
		public void ShowActor()
		{
			this.m_actor.Show(this.m_IgnoreSpells.Value);
		}

		// Token: 0x06006C76 RID: 27766 RVA: 0x001FE7B9 File Offset: 0x001FC9B9
		public void HideActor()
		{
			this.m_actor.Hide(this.m_IgnoreSpells.Value);
		}

		// Token: 0x0400550C RID: 21772
		public FsmGameObject m_ActorObject;

		// Token: 0x0400550D RID: 21773
		[Tooltip("Should the Actor be set to visible or invisible?")]
		public FsmBool m_Visible;

		// Token: 0x0400550E RID: 21774
		[Tooltip("Don't touch the Actor's SpellTable when setting visibility")]
		public FsmBool m_IgnoreSpells;

		// Token: 0x0400550F RID: 21775
		[Tooltip("Resets to the initial visibility once\nit leaves the state")]
		public bool m_ResetOnExit;

		// Token: 0x04005510 RID: 21776
		protected bool m_initialVisibility;
	}
}
