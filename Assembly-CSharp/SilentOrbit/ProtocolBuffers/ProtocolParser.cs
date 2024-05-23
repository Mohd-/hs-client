using System;
using System.IO;
using System.Text;

namespace SilentOrbit.ProtocolBuffers
{
	// Token: 0x02000EB1 RID: 3761
	public static class ProtocolParser
	{
		// Token: 0x0600713F RID: 28991 RVA: 0x00215E3D File Offset: 0x0021403D
		public static string ReadString(Stream stream)
		{
			return Encoding.UTF8.GetString(ProtocolParser.ReadBytes(stream));
		}

		// Token: 0x06007140 RID: 28992 RVA: 0x00215E50 File Offset: 0x00214050
		public static byte[] ReadBytes(Stream stream)
		{
			int num = (int)ProtocolParser.ReadUInt32(stream);
			byte[] array = new byte[num];
			int num2;
			for (int i = 0; i < num; i += num2)
			{
				num2 = stream.Read(array, i, num - i);
				if (num2 == 0)
				{
					throw new ProtocolBufferException(string.Concat(new object[]
					{
						"Expected ",
						num - i,
						" got ",
						i
					}));
				}
			}
			return array;
		}

		// Token: 0x06007141 RID: 28993 RVA: 0x00215EC8 File Offset: 0x002140C8
		public static void SkipBytes(Stream stream)
		{
			int num = (int)ProtocolParser.ReadUInt32(stream);
			if (stream.CanSeek)
			{
				stream.Seek((long)num, 1);
			}
			else
			{
				ProtocolParser.ReadBytes(stream);
			}
		}

		// Token: 0x06007142 RID: 28994 RVA: 0x00215EFD File Offset: 0x002140FD
		public static void WriteString(Stream stream, string val)
		{
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(val));
		}

		// Token: 0x06007143 RID: 28995 RVA: 0x00215F10 File Offset: 0x00214110
		public static void WriteBytes(Stream stream, byte[] val)
		{
			ProtocolParser.WriteUInt32(stream, (uint)val.Length);
			stream.Write(val, 0, val.Length);
		}

		// Token: 0x06007144 RID: 28996 RVA: 0x00215F26 File Offset: 0x00214126
		[Obsolete("Only for reference")]
		public static ulong ReadFixed64(BinaryReader reader)
		{
			return reader.ReadUInt64();
		}

		// Token: 0x06007145 RID: 28997 RVA: 0x00215F2E File Offset: 0x0021412E
		[Obsolete("Only for reference")]
		public static long ReadSFixed64(BinaryReader reader)
		{
			return reader.ReadInt64();
		}

		// Token: 0x06007146 RID: 28998 RVA: 0x00215F36 File Offset: 0x00214136
		[Obsolete("Only for reference")]
		public static uint ReadFixed32(BinaryReader reader)
		{
			return reader.ReadUInt32();
		}

		// Token: 0x06007147 RID: 28999 RVA: 0x00215F3E File Offset: 0x0021413E
		[Obsolete("Only for reference")]
		public static int ReadSFixed32(BinaryReader reader)
		{
			return reader.ReadInt32();
		}

		// Token: 0x06007148 RID: 29000 RVA: 0x00215F46 File Offset: 0x00214146
		[Obsolete("Only for reference")]
		public static void WriteFixed64(BinaryWriter writer, ulong val)
		{
			writer.Write(val);
		}

		// Token: 0x06007149 RID: 29001 RVA: 0x00215F4F File Offset: 0x0021414F
		[Obsolete("Only for reference")]
		public static void WriteSFixed64(BinaryWriter writer, long val)
		{
			writer.Write(val);
		}

		// Token: 0x0600714A RID: 29002 RVA: 0x00215F58 File Offset: 0x00214158
		[Obsolete("Only for reference")]
		public static void WriteFixed32(BinaryWriter writer, uint val)
		{
			writer.Write(val);
		}

		// Token: 0x0600714B RID: 29003 RVA: 0x00215F61 File Offset: 0x00214161
		[Obsolete("Only for reference")]
		public static void WriteSFixed32(BinaryWriter writer, int val)
		{
			writer.Write(val);
		}

		// Token: 0x0600714C RID: 29004 RVA: 0x00215F6A File Offset: 0x0021416A
		[Obsolete("Only for reference")]
		public static float ReadFloat(BinaryReader reader)
		{
			return reader.ReadSingle();
		}

		// Token: 0x0600714D RID: 29005 RVA: 0x00215F72 File Offset: 0x00214172
		[Obsolete("Only for reference")]
		public static double ReadDouble(BinaryReader reader)
		{
			return reader.ReadDouble();
		}

		// Token: 0x0600714E RID: 29006 RVA: 0x00215F7A File Offset: 0x0021417A
		[Obsolete("Only for reference")]
		public static void WriteFloat(BinaryWriter writer, float val)
		{
			writer.Write(val);
		}

		// Token: 0x0600714F RID: 29007 RVA: 0x00215F83 File Offset: 0x00214183
		[Obsolete("Only for reference")]
		public static void WriteDouble(BinaryWriter writer, double val)
		{
			writer.Write(val);
		}

		// Token: 0x06007150 RID: 29008 RVA: 0x00215F8C File Offset: 0x0021418C
		public static Key ReadKey(Stream stream)
		{
			uint num = ProtocolParser.ReadUInt32(stream);
			return new Key(num >> 3, (Wire)(num & 7U));
		}

		// Token: 0x06007151 RID: 29009 RVA: 0x00215FAC File Offset: 0x002141AC
		public static Key ReadKey(byte firstByte, Stream stream)
		{
			if (firstByte < 128)
			{
				return new Key((uint)(firstByte >> 3), (Wire)(firstByte & 7));
			}
			uint field = ProtocolParser.ReadUInt32(stream) << 4 | (uint)(firstByte >> 3 & 15);
			return new Key(field, (Wire)(firstByte & 7));
		}

		// Token: 0x06007152 RID: 29010 RVA: 0x00215FEC File Offset: 0x002141EC
		public static void WriteKey(Stream stream, Key key)
		{
			uint val = key.Field << 3 | (uint)key.WireType;
			ProtocolParser.WriteUInt32(stream, val);
		}

		// Token: 0x06007153 RID: 29011 RVA: 0x00216010 File Offset: 0x00214210
		public static void SkipKey(Stream stream, Key key)
		{
			switch (key.WireType)
			{
			case Wire.Varint:
				ProtocolParser.ReadSkipVarInt(stream);
				return;
			case Wire.Fixed64:
				stream.Seek(8L, 1);
				return;
			case Wire.LengthDelimited:
				stream.Seek((long)((ulong)ProtocolParser.ReadUInt32(stream)), 1);
				return;
			case Wire.Fixed32:
				stream.Seek(4L, 1);
				return;
			}
			throw new NotImplementedException("Unknown wire type: " + key.WireType);
		}

		// Token: 0x06007154 RID: 29012 RVA: 0x00216090 File Offset: 0x00214290
		public static byte[] ReadValueBytes(Stream stream, Key key)
		{
			int i = 0;
			switch (key.WireType)
			{
			case Wire.Varint:
				return ProtocolParser.ReadVarIntBytes(stream);
			case Wire.Fixed64:
			{
				byte[] array = new byte[8];
				while (i < 8)
				{
					i += stream.Read(array, i, 8 - i);
				}
				return array;
			}
			case Wire.LengthDelimited:
			{
				uint num = ProtocolParser.ReadUInt32(stream);
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					ProtocolParser.WriteUInt32(memoryStream, num);
					array = new byte[(ulong)num + (ulong)memoryStream.Length];
					memoryStream.ToArray().CopyTo(array, 0);
					i = (int)memoryStream.Length;
				}
				while (i < array.Length)
				{
					i += stream.Read(array, i, array.Length - i);
				}
				return array;
			}
			case Wire.Fixed32:
			{
				byte[] array = new byte[4];
				while (i < 4)
				{
					i += stream.Read(array, i, 4 - i);
				}
				return array;
			}
			}
			throw new NotImplementedException("Unknown wire type: " + key.WireType);
		}

		// Token: 0x06007155 RID: 29013 RVA: 0x002161B0 File Offset: 0x002143B0
		public static void ReadSkipVarInt(Stream stream)
		{
			for (;;)
			{
				int num = stream.ReadByte();
				if (num < 0)
				{
					break;
				}
				if ((num & 128) == 0)
				{
					return;
				}
			}
			throw new IOException("Stream ended too early");
		}

		// Token: 0x06007156 RID: 29014 RVA: 0x002161E8 File Offset: 0x002143E8
		public static byte[] ReadVarIntBytes(Stream stream)
		{
			byte[] array = new byte[10];
			int num = 0;
			for (;;)
			{
				int num2 = stream.ReadByte();
				if (num2 < 0)
				{
					break;
				}
				array[num] = (byte)num2;
				num++;
				if ((num2 & 128) == 0)
				{
					goto Block_2;
				}
				if (num >= array.Length)
				{
					goto Block_3;
				}
			}
			throw new IOException("Stream ended too early");
			Block_2:
			byte[] array2 = new byte[num];
			Array.Copy(array, array2, array2.Length);
			return array2;
			Block_3:
			throw new ProtocolBufferException("VarInt too long, more than 10 bytes");
		}

		// Token: 0x06007157 RID: 29015 RVA: 0x0021625D File Offset: 0x0021445D
		[Obsolete("Use (int)ReadUInt64(stream); //yes 64")]
		public static int ReadInt32(Stream stream)
		{
			return (int)ProtocolParser.ReadUInt64(stream);
		}

		// Token: 0x06007158 RID: 29016 RVA: 0x00216266 File Offset: 0x00214466
		[Obsolete("Use WriteUInt64(stream, (ulong)val); //yes 64, negative numbers are encoded that way")]
		public static void WriteInt32(Stream stream, int val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)((long)val));
		}

		// Token: 0x06007159 RID: 29017 RVA: 0x00216270 File Offset: 0x00214470
		public static int ReadZInt32(Stream stream)
		{
			uint num = ProtocolParser.ReadUInt32(stream);
			return (int)(num >> 1 ^ (uint)((int)((int)num << 31) >> 31));
		}

		// Token: 0x0600715A RID: 29018 RVA: 0x0021628F File Offset: 0x0021448F
		public static void WriteZInt32(Stream stream, int val)
		{
			ProtocolParser.WriteUInt32(stream, (uint)(val << 1 ^ val >> 31));
		}

		// Token: 0x0600715B RID: 29019 RVA: 0x002162A0 File Offset: 0x002144A0
		public static uint ReadUInt32(Stream stream)
		{
			uint num = 0U;
			for (int i = 0; i < 5; i++)
			{
				int num2 = stream.ReadByte();
				if (num2 < 0)
				{
					throw new IOException("Stream ended too early");
				}
				if (i == 4 && (num2 & 240) != 0)
				{
					throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
				}
				if ((num2 & 128) == 0)
				{
					return num | (uint)((uint)num2 << 7 * i);
				}
				num |= (uint)((uint)(num2 & 127) << 7 * i);
			}
			throw new ProtocolBufferException("Got larger VarInt than 32bit unsigned");
		}

		// Token: 0x0600715C RID: 29020 RVA: 0x0021632C File Offset: 0x0021452C
		public static void WriteUInt32(Stream stream, uint val)
		{
			byte b;
			for (;;)
			{
				b = (byte)(val & 127U);
				val >>= 7;
				if (val == 0U)
				{
					break;
				}
				b |= 128;
				stream.WriteByte(b);
			}
			stream.WriteByte(b);
		}

		// Token: 0x0600715D RID: 29021 RVA: 0x0021636B File Offset: 0x0021456B
		[Obsolete("Use (long)ReadUInt64(stream); instead")]
		public static int ReadInt64(Stream stream)
		{
			return (int)ProtocolParser.ReadUInt64(stream);
		}

		// Token: 0x0600715E RID: 29022 RVA: 0x00216374 File Offset: 0x00214574
		[Obsolete("Use WriteUInt64 (stream, (ulong)val); instead")]
		public static void WriteInt64(Stream stream, int val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)((long)val));
		}

		// Token: 0x0600715F RID: 29023 RVA: 0x00216380 File Offset: 0x00214580
		public static long ReadZInt64(Stream stream)
		{
			ulong num = ProtocolParser.ReadUInt64(stream);
			return (long)(num >> 1 ^ num << 63 >> 63);
		}

		// Token: 0x06007160 RID: 29024 RVA: 0x0021639F File Offset: 0x0021459F
		public static void WriteZInt64(Stream stream, long val)
		{
			ProtocolParser.WriteUInt64(stream, (ulong)(val << 1 ^ val >> 63));
		}

		// Token: 0x06007161 RID: 29025 RVA: 0x002163B0 File Offset: 0x002145B0
		public static ulong ReadUInt64(Stream stream)
		{
			ulong num = 0UL;
			for (int i = 0; i < 10; i++)
			{
				int num2 = stream.ReadByte();
				if (num2 < 0)
				{
					throw new IOException("Stream ended too early");
				}
				if (i == 9 && (num2 & 254) != 0)
				{
					throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
				}
				if ((num2 & 128) == 0)
				{
					return num | (ulong)((ulong)((long)num2) << 7 * i);
				}
				num |= (ulong)((ulong)((long)(num2 & 127)) << 7 * i);
			}
			throw new ProtocolBufferException("Got larger VarInt than 64 bit unsigned");
		}

		// Token: 0x06007162 RID: 29026 RVA: 0x00216440 File Offset: 0x00214640
		public static void WriteUInt64(Stream stream, ulong val)
		{
			byte b;
			for (;;)
			{
				b = (byte)(val & 127UL);
				val >>= 7;
				if (val == 0UL)
				{
					break;
				}
				b |= 128;
				stream.WriteByte(b);
			}
			stream.WriteByte(b);
		}

		// Token: 0x06007163 RID: 29027 RVA: 0x00216480 File Offset: 0x00214680
		public static bool ReadBool(Stream stream)
		{
			int num = stream.ReadByte();
			if (num < 0)
			{
				throw new IOException("Stream ended too early");
			}
			if (num == 1)
			{
				return true;
			}
			if (num == 0)
			{
				return false;
			}
			throw new ProtocolBufferException("Invalid boolean value");
		}

		// Token: 0x06007164 RID: 29028 RVA: 0x002164C1 File Offset: 0x002146C1
		public static void WriteBool(Stream stream, bool val)
		{
			stream.WriteByte((!val) ? 0 : 1);
		}

		// Token: 0x04005AD8 RID: 23256
		public static MemoryStreamStack Stack = new AllocationStack();
	}
}
