using System;
using System.IO;
using System.Text;

namespace WTCG.BI
{
	// Token: 0x020000B4 RID: 180
	public class Telemetry : IProtoBuf
	{
		// Token: 0x0600087E RID: 2174 RVA: 0x00021378 File Offset: 0x0001F578
		public void Deserialize(Stream stream)
		{
			Telemetry.Deserialize(stream, this);
		}

		// Token: 0x0600087F RID: 2175 RVA: 0x00021382 File Offset: 0x0001F582
		public static Telemetry Deserialize(Stream stream, Telemetry instance)
		{
			return Telemetry.Deserialize(stream, instance, -1L);
		}

		// Token: 0x06000880 RID: 2176 RVA: 0x00021390 File Offset: 0x0001F590
		public static Telemetry DeserializeLengthDelimited(Stream stream)
		{
			Telemetry telemetry = new Telemetry();
			Telemetry.DeserializeLengthDelimited(stream, telemetry);
			return telemetry;
		}

		// Token: 0x06000881 RID: 2177 RVA: 0x000213AC File Offset: 0x0001F5AC
		public static Telemetry DeserializeLengthDelimited(Stream stream, Telemetry instance)
		{
			long num = (long)((ulong)ProtocolParser.ReadUInt32(stream));
			num += stream.Position;
			return Telemetry.Deserialize(stream, instance, num);
		}

		// Token: 0x06000882 RID: 2178 RVA: 0x000213D4 File Offset: 0x0001F5D4
		public static Telemetry Deserialize(Stream stream, Telemetry instance, long limit)
		{
			instance.BnetRegion_ = Telemetry.BnetRegion.REGION_UNINITIALIZED;
			while (limit < 0L || stream.Position < limit)
			{
				int num = stream.ReadByte();
				if (num == -1)
				{
					if (limit >= 0L)
					{
						throw new EndOfStreamException();
					}
					return instance;
				}
				else
				{
					int num2 = num;
					if (num2 != 8)
					{
						if (num2 != 16)
						{
							if (num2 != 24)
							{
								if (num2 != 34)
								{
									if (num2 != 40)
									{
										if (num2 != 50)
										{
											if (num2 != 56)
											{
												if (num2 != 64)
												{
													if (num2 != 74)
													{
														if (num2 != 82)
														{
															if (num2 != 88)
															{
																if (num2 != 96)
																{
																	if (num2 != 104)
																	{
																		if (num2 != 112)
																		{
																			if (num2 != 122)
																			{
																				Key key = ProtocolParser.ReadKey((byte)num, stream);
																				uint field = key.Field;
																				if (field == 0U)
																				{
																					throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
																				}
																				ProtocolParser.SkipKey(stream, key);
																			}
																			else
																			{
																				instance.Message_ = ProtocolParser.ReadString(stream);
																			}
																		}
																		else
																		{
																			instance.ErrorCode_ = (long)ProtocolParser.ReadUInt64(stream);
																		}
																	}
																	else
																	{
																		instance.GameAccountId_ = ProtocolParser.ReadUInt64(stream);
																	}
																}
																else
																{
																	instance.BnetRegion_ = (Telemetry.BnetRegion)ProtocolParser.ReadUInt64(stream);
																}
															}
															else
															{
																instance.Event_ = ProtocolParser.ReadUInt64(stream);
															}
														}
														else
														{
															instance.DeviceUniqueIdentifier_ = ProtocolParser.ReadString(stream);
														}
													}
													else
													{
														instance.SessionId_ = ProtocolParser.ReadString(stream);
													}
												}
												else
												{
													instance.Store_ = (Telemetry.Store)ProtocolParser.ReadUInt64(stream);
												}
											}
											else
											{
												instance.ScreenUI_ = (Telemetry.ScreenUI)ProtocolParser.ReadUInt64(stream);
											}
										}
										else
										{
											instance.Os_ = ProtocolParser.ReadString(stream);
										}
									}
									else
									{
										instance.Platform_ = (Telemetry.Platform)ProtocolParser.ReadUInt64(stream);
									}
								}
								else
								{
									instance.Version_ = ProtocolParser.ReadString(stream);
								}
							}
							else
							{
								instance.Locale_ = (Telemetry.Locale)ProtocolParser.ReadUInt64(stream);
							}
						}
						else
						{
							instance.Level_ = (Telemetry.Level)ProtocolParser.ReadUInt64(stream);
						}
					}
					else
					{
						instance.Time_ = (long)ProtocolParser.ReadUInt64(stream);
					}
				}
			}
			if (stream.Position == limit)
			{
				return instance;
			}
			throw new ProtocolBufferException("Read past max limit");
		}

		// Token: 0x06000883 RID: 2179 RVA: 0x000215F5 File Offset: 0x0001F7F5
		public void Serialize(Stream stream)
		{
			Telemetry.Serialize(stream, this);
		}

		// Token: 0x06000884 RID: 2180 RVA: 0x00021600 File Offset: 0x0001F800
		public static void Serialize(Stream stream, Telemetry instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)instance.Time_);
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Level_));
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Locale_));
			if (instance.Version_ == null)
			{
				throw new ArgumentNullException("Version_", "Required by proto specification.");
			}
			stream.WriteByte(34);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Version_));
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Platform_));
			if (instance.Os_ == null)
			{
				throw new ArgumentNullException("Os_", "Required by proto specification.");
			}
			stream.WriteByte(50);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Os_));
			stream.WriteByte(56);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.ScreenUI_));
			stream.WriteByte(64);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Store_));
			if (instance.SessionId_ == null)
			{
				throw new ArgumentNullException("SessionId_", "Required by proto specification.");
			}
			stream.WriteByte(74);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SessionId_));
			if (instance.DeviceUniqueIdentifier_ == null)
			{
				throw new ArgumentNullException("DeviceUniqueIdentifier_", "Required by proto specification.");
			}
			stream.WriteByte(82);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DeviceUniqueIdentifier_));
			stream.WriteByte(88);
			ProtocolParser.WriteUInt64(stream, instance.Event_);
			if (instance.HasBnetRegion_)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.BnetRegion_));
			}
			if (instance.HasGameAccountId_)
			{
				stream.WriteByte(104);
				ProtocolParser.WriteUInt64(stream, instance.GameAccountId_);
			}
			if (instance.HasErrorCode_)
			{
				stream.WriteByte(112);
				ProtocolParser.WriteUInt64(stream, (ulong)instance.ErrorCode_);
			}
			if (instance.HasMessage_)
			{
				stream.WriteByte(122);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Message_));
			}
		}

		// Token: 0x06000885 RID: 2181 RVA: 0x00021808 File Offset: 0x0001FA08
		public uint GetSerializedSize()
		{
			uint num = 0U;
			num += ProtocolParser.SizeOfUInt64((ulong)this.Time_);
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Level_));
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Locale_));
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Version_);
			num += ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Platform_));
			uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.Os_);
			num += ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.ScreenUI_));
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Store_));
			uint byteCount3 = (uint)Encoding.UTF8.GetByteCount(this.SessionId_);
			num += ProtocolParser.SizeOfUInt32(byteCount3) + byteCount3;
			uint byteCount4 = (uint)Encoding.UTF8.GetByteCount(this.DeviceUniqueIdentifier_);
			num += ProtocolParser.SizeOfUInt32(byteCount4) + byteCount4;
			num += ProtocolParser.SizeOfUInt64(this.Event_);
			if (this.HasBnetRegion_)
			{
				num += 1U;
				num += ProtocolParser.SizeOfUInt64((ulong)((long)this.BnetRegion_));
			}
			if (this.HasGameAccountId_)
			{
				num += 1U;
				num += ProtocolParser.SizeOfUInt64(this.GameAccountId_);
			}
			if (this.HasErrorCode_)
			{
				num += 1U;
				num += ProtocolParser.SizeOfUInt64((ulong)this.ErrorCode_);
			}
			if (this.HasMessage_)
			{
				num += 1U;
				uint byteCount5 = (uint)Encoding.UTF8.GetByteCount(this.Message_);
				num += ProtocolParser.SizeOfUInt32(byteCount5) + byteCount5;
			}
			return num + 11U;
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x0002197D File Offset: 0x0001FB7D
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x00021985 File Offset: 0x0001FB85
		public long Time_ { get; set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x0002198E File Offset: 0x0001FB8E
		// (set) Token: 0x06000889 RID: 2185 RVA: 0x00021996 File Offset: 0x0001FB96
		public Telemetry.Level Level_ { get; set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x0600088A RID: 2186 RVA: 0x0002199F File Offset: 0x0001FB9F
		// (set) Token: 0x0600088B RID: 2187 RVA: 0x000219A7 File Offset: 0x0001FBA7
		public Telemetry.Locale Locale_ { get; set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x0600088C RID: 2188 RVA: 0x000219B0 File Offset: 0x0001FBB0
		// (set) Token: 0x0600088D RID: 2189 RVA: 0x000219B8 File Offset: 0x0001FBB8
		public string Version_ { get; set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x000219C1 File Offset: 0x0001FBC1
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x000219C9 File Offset: 0x0001FBC9
		public Telemetry.Platform Platform_ { get; set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06000890 RID: 2192 RVA: 0x000219D2 File Offset: 0x0001FBD2
		// (set) Token: 0x06000891 RID: 2193 RVA: 0x000219DA File Offset: 0x0001FBDA
		public string Os_ { get; set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000892 RID: 2194 RVA: 0x000219E3 File Offset: 0x0001FBE3
		// (set) Token: 0x06000893 RID: 2195 RVA: 0x000219EB File Offset: 0x0001FBEB
		public Telemetry.ScreenUI ScreenUI_ { get; set; }

		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000894 RID: 2196 RVA: 0x000219F4 File Offset: 0x0001FBF4
		// (set) Token: 0x06000895 RID: 2197 RVA: 0x000219FC File Offset: 0x0001FBFC
		public Telemetry.Store Store_ { get; set; }

		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000896 RID: 2198 RVA: 0x00021A05 File Offset: 0x0001FC05
		// (set) Token: 0x06000897 RID: 2199 RVA: 0x00021A0D File Offset: 0x0001FC0D
		public string SessionId_ { get; set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000898 RID: 2200 RVA: 0x00021A16 File Offset: 0x0001FC16
		// (set) Token: 0x06000899 RID: 2201 RVA: 0x00021A1E File Offset: 0x0001FC1E
		public string DeviceUniqueIdentifier_ { get; set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600089A RID: 2202 RVA: 0x00021A27 File Offset: 0x0001FC27
		// (set) Token: 0x0600089B RID: 2203 RVA: 0x00021A2F File Offset: 0x0001FC2F
		public ulong Event_ { get; set; }

		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600089C RID: 2204 RVA: 0x00021A38 File Offset: 0x0001FC38
		// (set) Token: 0x0600089D RID: 2205 RVA: 0x00021A40 File Offset: 0x0001FC40
		public Telemetry.BnetRegion BnetRegion_
		{
			get
			{
				return this._BnetRegion_;
			}
			set
			{
				this._BnetRegion_ = value;
				this.HasBnetRegion_ = true;
			}
		}

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x0600089E RID: 2206 RVA: 0x00021A50 File Offset: 0x0001FC50
		// (set) Token: 0x0600089F RID: 2207 RVA: 0x00021A58 File Offset: 0x0001FC58
		public ulong GameAccountId_
		{
			get
			{
				return this._GameAccountId_;
			}
			set
			{
				this._GameAccountId_ = value;
				this.HasGameAccountId_ = true;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060008A0 RID: 2208 RVA: 0x00021A68 File Offset: 0x0001FC68
		// (set) Token: 0x060008A1 RID: 2209 RVA: 0x00021A70 File Offset: 0x0001FC70
		public long ErrorCode_
		{
			get
			{
				return this._ErrorCode_;
			}
			set
			{
				this._ErrorCode_ = value;
				this.HasErrorCode_ = true;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060008A2 RID: 2210 RVA: 0x00021A80 File Offset: 0x0001FC80
		// (set) Token: 0x060008A3 RID: 2211 RVA: 0x00021A88 File Offset: 0x0001FC88
		public string Message_
		{
			get
			{
				return this._Message_;
			}
			set
			{
				this._Message_ = value;
				this.HasMessage_ = (value != null);
			}
		}

		// Token: 0x060008A4 RID: 2212 RVA: 0x00021AA0 File Offset: 0x0001FCA0
		public override int GetHashCode()
		{
			int num = base.GetType().GetHashCode();
			num ^= this.Time_.GetHashCode();
			num ^= this.Level_.GetHashCode();
			num ^= this.Locale_.GetHashCode();
			num ^= this.Version_.GetHashCode();
			num ^= this.Platform_.GetHashCode();
			num ^= this.Os_.GetHashCode();
			num ^= this.ScreenUI_.GetHashCode();
			num ^= this.Store_.GetHashCode();
			num ^= this.SessionId_.GetHashCode();
			num ^= this.DeviceUniqueIdentifier_.GetHashCode();
			num ^= this.Event_.GetHashCode();
			if (this.HasBnetRegion_)
			{
				num ^= this.BnetRegion_.GetHashCode();
			}
			if (this.HasGameAccountId_)
			{
				num ^= this.GameAccountId_.GetHashCode();
			}
			if (this.HasErrorCode_)
			{
				num ^= this.ErrorCode_.GetHashCode();
			}
			if (this.HasMessage_)
			{
				num ^= this.Message_.GetHashCode();
			}
			return num;
		}

		// Token: 0x060008A5 RID: 2213 RVA: 0x00021BE4 File Offset: 0x0001FDE4
		public override bool Equals(object obj)
		{
			Telemetry telemetry = obj as Telemetry;
			return telemetry != null && this.Time_.Equals(telemetry.Time_) && this.Level_.Equals(telemetry.Level_) && this.Locale_.Equals(telemetry.Locale_) && this.Version_.Equals(telemetry.Version_) && this.Platform_.Equals(telemetry.Platform_) && this.Os_.Equals(telemetry.Os_) && this.ScreenUI_.Equals(telemetry.ScreenUI_) && this.Store_.Equals(telemetry.Store_) && this.SessionId_.Equals(telemetry.SessionId_) && this.DeviceUniqueIdentifier_.Equals(telemetry.DeviceUniqueIdentifier_) && this.Event_.Equals(telemetry.Event_) && this.HasBnetRegion_ == telemetry.HasBnetRegion_ && (!this.HasBnetRegion_ || this.BnetRegion_.Equals(telemetry.BnetRegion_)) && this.HasGameAccountId_ == telemetry.HasGameAccountId_ && (!this.HasGameAccountId_ || this.GameAccountId_.Equals(telemetry.GameAccountId_)) && this.HasErrorCode_ == telemetry.HasErrorCode_ && (!this.HasErrorCode_ || this.ErrorCode_.Equals(telemetry.ErrorCode_)) && this.HasMessage_ == telemetry.HasMessage_ && (!this.HasMessage_ || this.Message_.Equals(telemetry.Message_));
		}

		// Token: 0x04000453 RID: 1107
		public bool HasBnetRegion_;

		// Token: 0x04000454 RID: 1108
		private Telemetry.BnetRegion _BnetRegion_;

		// Token: 0x04000455 RID: 1109
		public bool HasGameAccountId_;

		// Token: 0x04000456 RID: 1110
		private ulong _GameAccountId_;

		// Token: 0x04000457 RID: 1111
		public bool HasErrorCode_;

		// Token: 0x04000458 RID: 1112
		private long _ErrorCode_;

		// Token: 0x04000459 RID: 1113
		public bool HasMessage_;

		// Token: 0x0400045A RID: 1114
		private string _Message_;

		// Token: 0x020000B5 RID: 181
		public enum Level
		{
			// Token: 0x04000467 RID: 1127
			LEVEL_NONE,
			// Token: 0x04000468 RID: 1128
			LEVEL_INFO,
			// Token: 0x04000469 RID: 1129
			LEVEL_WARN,
			// Token: 0x0400046A RID: 1130
			LEVEL_ERROR
		}

		// Token: 0x0200090B RID: 2315
		public enum Locale
		{
			// Token: 0x04003CD7 RID: 15575
			LOCALE_UNKNOWN,
			// Token: 0x04003CD8 RID: 15576
			LOCALE_ENUS,
			// Token: 0x04003CD9 RID: 15577
			LOCALE_ENGB,
			// Token: 0x04003CDA RID: 15578
			LOCALE_FRFR,
			// Token: 0x04003CDB RID: 15579
			LOCALE_DEDE,
			// Token: 0x04003CDC RID: 15580
			LOCALE_KOKR,
			// Token: 0x04003CDD RID: 15581
			LOCALE_ESES,
			// Token: 0x04003CDE RID: 15582
			LOCALE_ESMX,
			// Token: 0x04003CDF RID: 15583
			LOCALE_RURU,
			// Token: 0x04003CE0 RID: 15584
			LOCALE_ZHTW,
			// Token: 0x04003CE1 RID: 15585
			LOCALE_ZHCN,
			// Token: 0x04003CE2 RID: 15586
			LOCALE_ITIT,
			// Token: 0x04003CE3 RID: 15587
			LOCALE_PTBR,
			// Token: 0x04003CE4 RID: 15588
			LOCALE_PLPL,
			// Token: 0x04003CE5 RID: 15589
			LOCALE_15 = 15,
			// Token: 0x04003CE6 RID: 15590
			LOCALE_16
		}

		// Token: 0x0200090C RID: 2316
		public enum Platform
		{
			// Token: 0x04003CE8 RID: 15592
			PLATFORM_UNKNOWN,
			// Token: 0x04003CE9 RID: 15593
			PLATFORM_PC,
			// Token: 0x04003CEA RID: 15594
			PLATFORM_MAC,
			// Token: 0x04003CEB RID: 15595
			PLATFORM_IOS,
			// Token: 0x04003CEC RID: 15596
			PLATFORM_ANDROID
		}

		// Token: 0x0200090D RID: 2317
		public enum ScreenUI
		{
			// Token: 0x04003CEE RID: 15598
			SCREENUI_UNKNOWN,
			// Token: 0x04003CEF RID: 15599
			SCREENUI_DESKTOP,
			// Token: 0x04003CF0 RID: 15600
			SCREENUI_TABLET,
			// Token: 0x04003CF1 RID: 15601
			SCREENUI_PHONE
		}

		// Token: 0x0200090E RID: 2318
		public enum Store
		{
			// Token: 0x04003CF3 RID: 15603
			STORE_BLIZZARD,
			// Token: 0x04003CF4 RID: 15604
			STORE_IOS,
			// Token: 0x04003CF5 RID: 15605
			STORE_GOOGLEPLAY,
			// Token: 0x04003CF6 RID: 15606
			STORE_AMAZON
		}

		// Token: 0x0200090F RID: 2319
		public enum BnetRegion
		{
			// Token: 0x04003CF8 RID: 15608
			REGION_UNINITIALIZED = -1,
			// Token: 0x04003CF9 RID: 15609
			REGION_UNKNOWN,
			// Token: 0x04003CFA RID: 15610
			REGION_US,
			// Token: 0x04003CFB RID: 15611
			REGION_EU,
			// Token: 0x04003CFC RID: 15612
			REGION_KR,
			// Token: 0x04003CFD RID: 15613
			REGION_TW,
			// Token: 0x04003CFE RID: 15614
			REGION_CN,
			// Token: 0x04003CFF RID: 15615
			REGION_LIVE_VERIFICATION = 40,
			// Token: 0x04003D00 RID: 15616
			REGION_PTR_LOC,
			// Token: 0x04003D01 RID: 15617
			REGION_DEV = 60,
			// Token: 0x04003D02 RID: 15618
			REGION_PTR = 98
		}
	}
}
