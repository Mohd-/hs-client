using System;
using System.Collections.Generic;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EBB RID: 3771
	public class ThreadUnsafeStack : IDisposable, MemoryStreamStack
	{
		// Token: 0x0600718F RID: 29071 RVA: 0x002167B7 File Offset: 0x002149B7
		public MemoryStream Pop()
		{
			if (this.stack.Count == 0)
			{
				return new MemoryStream();
			}
			return this.stack.Pop();
		}

		// Token: 0x06007190 RID: 29072 RVA: 0x002167DA File Offset: 0x002149DA
		public void Push(MemoryStream stream)
		{
			this.stack.Push(stream);
		}

		// Token: 0x06007191 RID: 29073 RVA: 0x002167E8 File Offset: 0x002149E8
		public void Dispose()
		{
			this.stack.Clear();
		}

		// Token: 0x04005AE5 RID: 23269
		private Stack<MemoryStream> stack = new Stack<MemoryStream>();
	}
}
