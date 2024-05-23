using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB3 RID: 3763
	public class AllocationStack : IDisposable, MemoryStreamStack
	{
		// Token: 0x06007168 RID: 29032 RVA: 0x002164DE File Offset: 0x002146DE
		public MemoryStream Pop()
		{
			return new MemoryStream();
		}

		// Token: 0x06007169 RID: 29033 RVA: 0x002164E5 File Offset: 0x002146E5
		public void Push(MemoryStream stream)
		{
		}

		// Token: 0x0600716A RID: 29034 RVA: 0x002164E7 File Offset: 0x002146E7
		public void Dispose()
		{
		}
	}
}
