using System;
using System.Collections.Generic;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EBA RID: 3770
	public class ThreadSafeStack : IDisposable, MemoryStreamStack
	{
		// Token: 0x0600718B RID: 29067 RVA: 0x002166B8 File Offset: 0x002148B8
		public MemoryStream Pop()
		{
			Stack<MemoryStream> stack = this.stack;
			MemoryStream result;
			lock (stack)
			{
				if (this.stack.Count == 0)
				{
					result = new MemoryStream();
				}
				else
				{
					result = this.stack.Pop();
				}
			}
			return result;
		}

		// Token: 0x0600718C RID: 29068 RVA: 0x0021671C File Offset: 0x0021491C
		public void Push(MemoryStream stream)
		{
			Stack<MemoryStream> stack = this.stack;
			lock (stack)
			{
				this.stack.Push(stream);
			}
		}

		// Token: 0x0600718D RID: 29069 RVA: 0x00216760 File Offset: 0x00214960
		public void Dispose()
		{
			Stack<MemoryStream> stack = this.stack;
			lock (stack)
			{
				this.stack.Clear();
			}
		}

		// Token: 0x04005AE4 RID: 23268
		private Stack<MemoryStream> stack = new Stack<MemoryStream>();
	}
}
