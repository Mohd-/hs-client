using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DED RID: 3565
	[Tooltip("Send an event based on a Spell's Card's Type.")]
	[ActionCategory("Pegasus")]
	public class SpellCardTypeAction : SpellAction
	{
		// Token: 0x06006DD0 RID: 28112 RVA: 0x002042C0 File Offset: 0x002024C0
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DD1 RID: 28113 RVA: 0x002042D4 File Offset: 0x002024D4
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_MinionEvent = null;
			this.m_HeroEvent = null;
			this.m_HeroPowerEvent = null;
			this.m_WeaponEvent = null;
		}

		// Token: 0x06006DD2 RID: 28114 RVA: 0x0020430C File Offset: 0x0020250C
		public override void OnEnter()
		{
			base.OnEnter();
			Card card = base.GetCard(this.m_WhichCard);
			if (card == null)
			{
				Error.AddDevFatal("SpellCardTypeAction.OnEnter() - Card not found!", new object[0]);
				base.Finish();
				return;
			}
			TAG_CARDTYPE cardType = card.GetEntity().GetCardType();
			switch (cardType)
			{
			case TAG_CARDTYPE.HERO:
				base.Fsm.Event(this.m_HeroEvent);
				goto IL_EB;
			case TAG_CARDTYPE.MINION:
				base.Fsm.Event(this.m_MinionEvent);
				goto IL_EB;
			case TAG_CARDTYPE.WEAPON:
				base.Fsm.Event(this.m_WeaponEvent);
				goto IL_EB;
			case TAG_CARDTYPE.HERO_POWER:
				base.Fsm.Event(this.m_HeroPowerEvent);
				goto IL_EB;
			}
			Error.AddDevFatal("SpellCardTypeAction.OnEnter() - unknown type {0} on {1}", new object[]
			{
				cardType,
				card
			});
			IL_EB:
			base.Finish();
		}

		// Token: 0x0400567F RID: 22143
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005680 RID: 22144
		public SpellAction.Which m_WhichCard;

		// Token: 0x04005681 RID: 22145
		public FsmEvent m_MinionEvent;

		// Token: 0x04005682 RID: 22146
		public FsmEvent m_HeroEvent;

		// Token: 0x04005683 RID: 22147
		public FsmEvent m_HeroPowerEvent;

		// Token: 0x04005684 RID: 22148
		public FsmEvent m_WeaponEvent;
	}
}
