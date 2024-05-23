using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B66 RID: 2918
	[Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	[ActionCategory(12)]
	[Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object.\nNOTE: This action won't work on the very first frame of the game...")]
	public class BroadcastEvent : FsmStateAction
	{
		// Token: 0x060062FE RID: 25342 RVA: 0x001D9450 File Offset: 0x001D7650
		public override void Reset()
		{
			this.broadcastEvent = null;
			this.gameObject = null;
			this.sendToChildren = false;
			this.excludeSelf = false;
		}

		// Token: 0x060062FF RID: 25343 RVA: 0x001D9484 File Offset: 0x001D7684
		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.broadcastEvent.Value))
			{
				if (this.gameObject.Value != null)
				{
					base.Fsm.BroadcastEventToGameObject(this.gameObject.Value, this.broadcastEvent.Value, this.sendToChildren.Value, this.excludeSelf.Value);
				}
				else
				{
					base.Fsm.BroadcastEvent(this.broadcastEvent.Value, this.excludeSelf.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04004A76 RID: 19062
		[RequiredField]
		public FsmString broadcastEvent;

		// Token: 0x04004A77 RID: 19063
		[Tooltip("Optionally specify a game object to broadcast the event to all FSMs on that game object.")]
		public FsmGameObject gameObject;

		// Token: 0x04004A78 RID: 19064
		[Tooltip("Broadcast to all FSMs on the game object's children.")]
		public FsmBool sendToChildren;

		// Token: 0x04004A79 RID: 19065
		public FsmBool excludeSelf;
	}
}
