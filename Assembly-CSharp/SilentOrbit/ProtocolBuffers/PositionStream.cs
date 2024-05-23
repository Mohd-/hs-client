using System;
using System.IO;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB8 RID: 3768
	public class PositionStream : Stream
	{
		// Token: 0x06007173 RID: 29043 RVA: 0x00216555 File Offset: 0x00214755
		public PositionStream(Stream baseStream)
		{
			this.stream = baseStream;
		}

		// Token: 0x170009CD RID: 2509
		// (get) Token: 0x06007174 RID: 29044 RVA: 0x00216564 File Offset: 0x00214764
		// (set) Token: 0x06007175 RID: 29045 RVA: 0x0021656C File Offset: 0x0021476C
		public int BytesRead { get; private set; }

		// Token: 0x06007176 RID: 29046 RVA: 0x00216575 File Offset: 0x00214775
		public override void Flush()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06007177 RID: 29047 RVA: 0x0021657C File Offset: 0x0021477C
		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = this.stream.Read(buffer, offset, count);
			this.BytesRead += num;
			return num;
		}

		// Token: 0x06007178 RID: 29048 RVA: 0x002165A8 File Offset: 0x002147A8
		public override int ReadByte()
		{
			int result = this.stream.ReadByte();
			this.BytesRead++;
			return result;
		}

		// Token: 0x06007179 RID: 29049 RVA: 0x002165D0 File Offset: 0x002147D0
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600717A RID: 29050 RVA: 0x002165D7 File Offset: 0x002147D7
		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600717B RID: 29051 RVA: 0x002165DE File Offset: 0x002147DE
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		// Token: 0x170009CE RID: 2510
		// (get) Token: 0x0600717C RID: 29052 RVA: 0x002165E5 File Offset: 0x002147E5
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009CF RID: 2511
		// (get) Token: 0x0600717D RID: 29053 RVA: 0x002165E8 File Offset: 0x002147E8
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009D0 RID: 2512
		// (get) Token: 0x0600717E RID: 29054 RVA: 0x002165EB File Offset: 0x002147EB
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		// Token: 0x170009D1 RID: 2513
		// (get) Token: 0x0600717F RID: 29055 RVA: 0x002165EE File Offset: 0x002147EE
		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06007180 RID: 29056 RVA: 0x002165FB File Offset: 0x002147FB
		// (set) Token: 0x06007181 RID: 29057 RVA: 0x00216604 File Offset: 0x00214804
		public override long Position
		{
			get
			{
				return (long)this.BytesRead;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		// Token: 0x06007182 RID: 29058 RVA: 0x0021660B File Offset: 0x0021480B
		public override void Close()
		{
			base.Close();
		}

		// Token: 0x06007183 RID: 29059 RVA: 0x00216613 File Offset: 0x00214813
		protected override void Dispose(bool disposing)
		{
			this.stream.Dispose();
			base.Dispose(disposing);
		}

		// Token: 0x04005AE0 RID: 23264
		private Stream stream;
	}
}
