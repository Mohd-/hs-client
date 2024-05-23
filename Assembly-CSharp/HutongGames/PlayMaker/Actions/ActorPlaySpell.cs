using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D95 RID: 3477
	[Tooltip("Play a spell on the given Actor")]
	[ActionCategory("Pegasus")]
	public class ActorPlaySpell : FsmStateAction
	{
		// Token: 0x06006C69 RID: 27753 RVA: 0x001FE50D File Offset: 0x001FC70D
		public override void Reset()
		{
			this.m_Spell = SpellType.NONE;
			this.m_actorObject = null;
		}

		// Token: 0x06006C6A RID: 27754 RVA: 0x001FE520 File Offset: 0x001FC720
		public override void OnEnter()
		{
			if (!this.m_actorObject.IsNone)
			{
				Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(this.m_actorObject.Value);
				if (actor != null && this.m_Spell != SpellType.NONE)
				{
					actor.ActivateSpell(this.m_Spell);
				}
			}
			base.Finish();
		}

		// Token: 0x06006C6B RID: 27755 RVA: 0x001FE578 File Offset: 0x001FC778
		public override void OnUpdate()
		{
		}

		// Token: 0x04005504 RID: 21764
		public SpellType m_Spell;

		// Token: 0x04005505 RID: 21765
		public FsmGameObject m_actorObject;
	}
}
