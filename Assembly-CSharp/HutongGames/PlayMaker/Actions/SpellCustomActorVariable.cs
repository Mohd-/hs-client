using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEE RID: 3566
	[Tooltip("Initialize a spell state, setting a variable that references one of the Actor's game objects by name.")]
	[ActionCategory("Pegasus")]
	public class SpellCustomActorVariable : FsmStateAction
	{
		// Token: 0x06006DD4 RID: 28116 RVA: 0x00204412 File Offset: 0x00202612
		public override void Reset()
		{
			this.m_objectName = string.Empty;
			this.m_actorObject = null;
		}

		// Token: 0x06006DD5 RID: 28117 RVA: 0x0020442C File Offset: 0x0020262C
		public override void OnEnter()
		{
			if (!this.m_actorObject.IsNone)
			{
				Actor actor = SceneUtils.FindComponentInThisOrParents<Actor>(base.Owner);
				if (actor != null)
				{
					GameObject gameObject = actor.gameObject;
					GameObject gameObject2 = SceneUtils.FindChildBySubstring(gameObject, this.m_objectName.Value);
					if (gameObject2 == null)
					{
						Debug.LogWarning("Could not find object of name " + this.m_objectName + " in actor");
					}
					else
					{
						this.m_actorObject.Value = gameObject2;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006DD6 RID: 28118 RVA: 0x002044B7 File Offset: 0x002026B7
		public override void OnUpdate()
		{
		}

		// Token: 0x04005685 RID: 22149
		public FsmString m_objectName;

		// Token: 0x04005686 RID: 22150
		public FsmGameObject m_actorObject;
	}
}
