using System;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB9 RID: 3769
	public class KeyValue
	{
		// Token: 0x06007184 RID: 29060 RVA: 0x00216627 File Offset: 0x00214827
		public KeyValue(Key key, byte[] value)
		{
			this.Key = key;
			this.Value = value;
		}

		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06007185 RID: 29061 RVA: 0x0021663D File Offset: 0x0021483D
		// (set) Token: 0x06007186 RID: 29062 RVA: 0x00216645 File Offset: 0x00214845
		public Key Key { get; set; }

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06007187 RID: 29063 RVA: 0x0021664E File Offset: 0x0021484E
		// (set) Token: 0x06007188 RID: 29064 RVA: 0x00216656 File Offset: 0x00214856
		public byte[] Value { get; set; }

		// Token: 0x06007189 RID: 29065 RVA: 0x00216660 File Offset: 0x00214860
		public override string ToString()
		{
			return string.Format("[KeyValue: {0}, {1}, {2} bytes]", this.Key.Field, this.Key.WireType, this.Value.Length);
		}
	}
}
