using System;
using System.IO;
using bgs;

// Token: 0x020000BF RID: 191
public class PegasusPacket : PacketFormat
{
	// Token: 0x06000A7A RID: 2682 RVA: 0x0002E7C6 File Offset: 0x0002C9C6
	public PegasusPacket()
	{
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x0002E7D0 File Offset: 0x0002C9D0
	public PegasusPacket(int type, int context, object body)
	{
		this.Type = type;
		this.Context = context;
		this.Size = -1;
		this.Body = body;
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x0002E7FF File Offset: 0x0002C9FF
	public PegasusPacket(int type, int context, int size, object body)
	{
		this.Type = type;
		this.Context = context;
		this.Size = size;
		this.Body = body;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x0002E824 File Offset: 0x0002CA24
	public object GetBody()
	{
		return this.Body;
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x0002E82C File Offset: 0x0002CA2C
	public override bool IsLoaded()
	{
		return this.Body != null;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x0002E83C File Offset: 0x0002CA3C
	public override int Decode(byte[] bytes, int offset, int available)
	{
		string text = string.Empty;
		int num = 0;
		while (num < 8 && num < available)
		{
			text = text + bytes[offset + num] + " ";
			num++;
		}
		int num2 = 0;
		if (!this.typeRead)
		{
			if (available < 4)
			{
				return num2;
			}
			this.Type = BitConverter.ToInt32(bytes, offset);
			this.typeRead = true;
			available -= 4;
			num2 += 4;
			offset += 4;
		}
		if (!this.sizeRead)
		{
			if (available < 4)
			{
				return num2;
			}
			this.Size = BitConverter.ToInt32(bytes, offset);
			this.sizeRead = true;
			available -= 4;
			num2 += 4;
			offset += 4;
		}
		if (this.Body == null)
		{
			if (available < this.Size)
			{
				return num2;
			}
			byte[] array = new byte[this.Size];
			Array.Copy(bytes, offset, array, 0, this.Size);
			this.Body = array;
			num2 += this.Size;
		}
		return num2;
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0002E934 File Offset: 0x0002CB34
	public override byte[] Encode()
	{
		if (this.Body is IProtoBuf)
		{
			IProtoBuf protoBuf = (IProtoBuf)this.Body;
			this.Size = (int)protoBuf.GetSerializedSize();
			byte[] array = new byte[this.Size + 4 + 4];
			Array.Copy(BitConverter.GetBytes(this.Type), 0, array, 0, 4);
			Array.Copy(BitConverter.GetBytes(this.Size), 0, array, 4, 4);
			protoBuf.Serialize(new MemoryStream(array, 8, this.Size));
			return array;
		}
		return null;
	}

	// Token: 0x040004FA RID: 1274
	private const int TYPE_BYTES = 4;

	// Token: 0x040004FB RID: 1275
	private const int SIZE_BYTES = 4;

	// Token: 0x040004FC RID: 1276
	public int Size;

	// Token: 0x040004FD RID: 1277
	public int Type;

	// Token: 0x040004FE RID: 1278
	public int Context;

	// Token: 0x040004FF RID: 1279
	public object Body;

	// Token: 0x04000500 RID: 1280
	private bool sizeRead;

	// Token: 0x04000501 RID: 1281
	private bool typeRead;
}
