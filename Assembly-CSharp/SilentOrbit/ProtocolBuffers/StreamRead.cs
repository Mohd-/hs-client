using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB7 RID: 3767
	[Obsolete("Renamed to PositionStream")]
	public class StreamRead : PositionStream
	{
		// Token: 0x06007172 RID: 29042 RVA: 0x0021654C File Offset: 0x0021474C
		public StreamRead(Stream baseStream) : base(baseStream)
		{
		}
	}
}
