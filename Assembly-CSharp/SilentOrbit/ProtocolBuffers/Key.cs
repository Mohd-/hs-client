using System;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB5 RID: 3765
	public class Key
	{
		// Token: 0x0600716C RID: 29036 RVA: 0x002164F2 File Offset: 0x002146F2
		public Key(uint field, Wire wireType)
		{
			this.Field = field;
			this.WireType = wireType;
		}

		// Token: 0x170009CB RID: 2507
		// (get) Token: 0x0600716D RID: 29037 RVA: 0x00216508 File Offset: 0x00214708
		// (set) Token: 0x0600716E RID: 29038 RVA: 0x00216510 File Offset: 0x00214710
		public uint Field { get; set; }

		// Token: 0x170009CC RID: 2508
		// (get) Token: 0x0600716F RID: 29039 RVA: 0x00216519 File Offset: 0x00214719
		// (set) Token: 0x06007170 RID: 29040 RVA: 0x00216521 File Offset: 0x00214721
		public Wire WireType { get; set; }

		// Token: 0x06007171 RID: 29041 RVA: 0x0021652A File Offset: 0x0021472A
		public override string ToString()
		{
			return string.Format("[Key: {0}, {1}]", this.Field, this.WireType);
		}
	}
}
