using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB2 RID: 3762
	public interface MemoryStreamStack : IDisposable
	{
		// Token: 0x06007165 RID: 29029
		MemoryStream Pop();

		// Token: 0x06007166 RID: 29030
		void Push(MemoryStream stream);
	}
}
