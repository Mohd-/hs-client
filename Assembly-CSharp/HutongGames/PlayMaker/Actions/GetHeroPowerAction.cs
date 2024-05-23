using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB8 RID: 3512
	[Tooltip("Use the spell to find the hero power")]
	[ActionCategory("Pegasus")]
	public class GetHeroPowerAction : FsmStateAction
	{
		// Token: 0x06006CFB RID: 27899 RVA: 0x00200EDE File Offset: 0x001FF0DE
		public override void Reset()
		{
			this.m_HeroPowerGameObject = null;
		}

		// Token: 0x06006CFC RID: 27900 RVA: 0x00200EE8 File Offset: 0x001FF0E8
		public override void OnEnter()
		{
			Spell spell = base.Owner.gameObject.GetComponentInChildren<Spell>();
			if (spell == null)
			{
				spell = SceneUtils.FindComponentInThisOrParents<Spell>(base.Owner);
				if (spell == null)
				{
					base.Finish();
					return;
				}
			}
			if (spell == null)
			{
				Debug.LogWarning("GetHeroPowerAction: spell is null!");
				return;
			}
			Card card = spell.GetSourceCard();
			if (card == null)
			{
				Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.Owner);
				card = actor.GetCard();
				if (card == null)
				{
					Debug.LogWarning("GetHeroPowerAction: card is null!");
					return;
				}
			}
			Card heroPowerCard = card.GetHeroPowerCard();
			if (heroPowerCard == null)
			{
				Debug.LogWarning("GetHeroPowerAction: heroPowerCard is null!");
				return;
			}
			Actor actor2 = heroPowerCard.GetActor();
			if (actor2 == null)
			{
				Debug.LogWarning("GetHeroPowerAction: heroPowerCardActor is null!");
				return;
			}
			GameObject gameObject = actor2.gameObject;
			if (!this.m_HeroPowerGameObject.IsNone)
			{
				this.m_HeroPowerGameObject.Value = gameObject;
			}
			base.Finish();
		}

		// Token: 0x040055AF RID: 21935
		public FsmGameObject m_HeroPowerGameObject;
	}
}
