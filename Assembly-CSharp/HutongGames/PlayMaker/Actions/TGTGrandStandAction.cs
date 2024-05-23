using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFC RID: 3580
	[Tooltip("Used to control TGT Grand Stands")]
	[ActionCategory("Pegasus")]
	public class TGTGrandStandAction : FsmStateAction
	{
		// Token: 0x06006E0B RID: 28171 RVA: 0x00204D48 File Offset: 0x00202F48
		public void PlayEmote(TGTGrandStandAction.EMOTE emote)
		{
			TGTGrandStand tgtgrandStand = TGTGrandStand.Get();
			if (tgtgrandStand == null)
			{
				return;
			}
			if (emote != TGTGrandStandAction.EMOTE.Cheer)
			{
				if (emote == TGTGrandStandAction.EMOTE.OhNo)
				{
					tgtgrandStand.PlayOhNoAnimation();
				}
			}
			else
			{
				tgtgrandStand.PlayCheerAnimation();
			}
		}

		// Token: 0x06006E0C RID: 28172 RVA: 0x00204D92 File Offset: 0x00202F92
		public override void Reset()
		{
			this.m_emote = TGTGrandStandAction.EMOTE.Cheer;
		}

		// Token: 0x06006E0D RID: 28173 RVA: 0x00204D9B File Offset: 0x00202F9B
		public override void OnEnter()
		{
			this.PlayEmote(this.m_emote);
		}

		// Token: 0x040056AC RID: 22188
		[RequiredField]
		public TGTGrandStandAction.EMOTE m_emote;

		// Token: 0x040056AD RID: 22189
		protected Actor m_actor;

		// Token: 0x02000DFD RID: 3581
		public enum EMOTE
		{
			// Token: 0x040056AF RID: 22191
			Cheer,
			// Token: 0x040056B0 RID: 22192
			OhNo
		}
	}
}
