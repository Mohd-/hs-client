using System;
using System.IO;
using System.Text;

namespace WTCG.BI
{
	// Token: 0x02000906 RID: 2310
	public class DataOnlyPatching : IProtoBuf
	{
		// Token: 0x0600563A RID: 22074 RVA: 0x0019E640 File Offset: 0x0019C840
		public void Deserialize(Stream stream)
		{
			DataOnlyPatching.Deserialize(stream, this);
		}

		// Token: 0x0600563B RID: 22075 RVA: 0x0019E64A File Offset: 0x0019C84A
		public static DataOnlyPatching Deserialize(Stream stream, DataOnlyPatching instance)
		{
			return DataOnlyPatching.Deserialize(stream, instance, -1L);
		}

		// Token: 0x0600563C RID: 22076 RVA: 0x0019E658 File Offset: 0x0019C858
		public static DataOnlyPatching DeserializeLengthDelimited(Stream stream)
		{
			DataOnlyPatching dataOnlyPatching = new DataOnlyPatching();
			DataOnlyPatching.DeserializeLengthDelimited(stream, dataOnlyPatching);
			return dataOnlyPatching;
		}

		// Token: 0x0600563D RID: 22077 RVA: 0x0019E674 File Offset: 0x0019C874
		public static DataOnlyPatching DeserializeLengthDelimited(Stream stream, DataOnlyPatching instance)
		{
			long num = (long)((ulong)ProtocolParser.ReadUInt32(stream));
			num += stream.Position;
			return DataOnlyPatching.Deserialize(stream, instance, num);
		}

		// Token: 0x0600563E RID: 22078 RVA: 0x0019E69C File Offset: 0x0019C89C
		public static DataOnlyPatching Deserialize(Stream stream, DataOnlyPatching instance, long limit)
		{
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
								if (num2 != 32)
								{
									if (num2 != 40)
									{
										if (num2 != 48)
										{
											if (num2 != 56)
											{
												if (num2 != 66)
												{
													if (num2 != 74)
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
												instance.NewBuild_ = (int)ProtocolParser.ReadUInt64(stream);
											}
										}
										else
										{
											instance.CurrentBuild_ = (int)ProtocolParser.ReadUInt64(stream);
										}
									}
									else
									{
										instance.GameAccountId_ = ProtocolParser.ReadUInt64(stream);
									}
								}
								else
								{
									instance.BnetRegion_ = (DataOnlyPatching.BnetRegion)ProtocolParser.ReadUInt64(stream);
								}
							}
							else
							{
								instance.Platform_ = (DataOnlyPatching.Platform)ProtocolParser.ReadUInt64(stream);
							}
						}
						else
						{
							instance.Locale_ = (DataOnlyPatching.Locale)ProtocolParser.ReadUInt64(stream);
						}
					}
					else
					{
						instance.Status_ = (DataOnlyPatching.Status)ProtocolParser.ReadUInt64(stream);
					}
				}
			}
			if (stream.Position == limit)
			{
				return instance;
			}
			throw new ProtocolBufferException("Read past max limit");
		}

		// Token: 0x0600563F RID: 22079 RVA: 0x0019E820 File Offset: 0x0019CA20
		public void Serialize(Stream stream)
		{
			DataOnlyPatching.Serialize(stream, this);
		}

		// Token: 0x06005640 RID: 22080 RVA: 0x0019E82C File Offset: 0x0019CA2C
		public static void Serialize(Stream stream, DataOnlyPatching instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Status_));
			stream.WriteByte(16);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Locale_));
			stream.WriteByte(24);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.Platform_));
			stream.WriteByte(32);
			ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.BnetRegion_));
			stream.WriteByte(40);
			ProtocolParser.WriteUInt64(stream, instance.GameAccountId_);
			if (instance.HasCurrentBuild_)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.CurrentBuild_));
			}
			if (instance.HasNewBuild_)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt64(stream, (ulong)((long)instance.NewBuild_));
			}
			if (instance.SessionId_ == null)
			{
				throw new ArgumentNullException("SessionId_", "Required by proto specification.");
			}
			stream.WriteByte(66);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.SessionId_));
			if (instance.DeviceUniqueIdentifier_ == null)
			{
				throw new ArgumentNullException("DeviceUniqueIdentifier_", "Required by proto specification.");
			}
			stream.WriteByte(74);
			ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.DeviceUniqueIdentifier_));
		}

		// Token: 0x06005641 RID: 22081 RVA: 0x0019E954 File Offset: 0x0019CB54
		public uint GetSerializedSize()
		{
			uint num = 0U;
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Status_));
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Locale_));
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.Platform_));
			num += ProtocolParser.SizeOfUInt64((ulong)((long)this.BnetRegion_));
			num += ProtocolParser.SizeOfUInt64(this.GameAccountId_);
			if (this.HasCurrentBuild_)
			{
				num += 1U;
				num += ProtocolParser.SizeOfUInt64((ulong)((long)this.CurrentBuild_));
			}
			if (this.HasNewBuild_)
			{
				num += 1U;
				num += ProtocolParser.SizeOfUInt64((ulong)((long)this.NewBuild_));
			}
			uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.SessionId_);
			num += ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			uint byteCount2 = (uint)Encoding.UTF8.GetByteCount(this.DeviceUniqueIdentifier_);
			num += ProtocolParser.SizeOfUInt32(byteCount2) + byteCount2;
			return num + 7U;
		}

		// Token: 0x17000694 RID: 1684
		// (get) Token: 0x06005642 RID: 22082 RVA: 0x0019EA26 File Offset: 0x0019CC26
		// (set) Token: 0x06005643 RID: 22083 RVA: 0x0019EA2E File Offset: 0x0019CC2E
		public DataOnlyPatching.Status Status_ { get; set; }

		// Token: 0x17000695 RID: 1685
		// (get) Token: 0x06005644 RID: 22084 RVA: 0x0019EA37 File Offset: 0x0019CC37
		// (set) Token: 0x06005645 RID: 22085 RVA: 0x0019EA3F File Offset: 0x0019CC3F
		public DataOnlyPatching.Locale Locale_ { get; set; }

		// Token: 0x17000696 RID: 1686
		// (get) Token: 0x06005646 RID: 22086 RVA: 0x0019EA48 File Offset: 0x0019CC48
		// (set) Token: 0x06005647 RID: 22087 RVA: 0x0019EA50 File Offset: 0x0019CC50
		public DataOnlyPatching.Platform Platform_ { get; set; }

		// Token: 0x17000697 RID: 1687
		// (get) Token: 0x06005648 RID: 22088 RVA: 0x0019EA59 File Offset: 0x0019CC59
		// (set) Token: 0x06005649 RID: 22089 RVA: 0x0019EA61 File Offset: 0x0019CC61
		public DataOnlyPatching.BnetRegion BnetRegion_ { get; set; }

		// Token: 0x17000698 RID: 1688
		// (get) Token: 0x0600564A RID: 22090 RVA: 0x0019EA6A File Offset: 0x0019CC6A
		// (set) Token: 0x0600564B RID: 22091 RVA: 0x0019EA72 File Offset: 0x0019CC72
		public ulong GameAccountId_ { get; set; }

		// Token: 0x17000699 RID: 1689
		// (get) Token: 0x0600564C RID: 22092 RVA: 0x0019EA7B File Offset: 0x0019CC7B
		// (set) Token: 0x0600564D RID: 22093 RVA: 0x0019EA83 File Offset: 0x0019CC83
		public int CurrentBuild_
		{
			get
			{
				return this._CurrentBuild_;
			}
			set
			{
				this._CurrentBuild_ = value;
				this.HasCurrentBuild_ = true;
			}
		}

		// Token: 0x1700069A RID: 1690
		// (get) Token: 0x0600564E RID: 22094 RVA: 0x0019EA93 File Offset: 0x0019CC93
		// (set) Token: 0x0600564F RID: 22095 RVA: 0x0019EA9B File Offset: 0x0019CC9B
		public int NewBuild_
		{
			get
			{
				return this._NewBuild_;
			}
			set
			{
				this._NewBuild_ = value;
				this.HasNewBuild_ = true;
			}
		}

		// Token: 0x1700069B RID: 1691
		// (get) Token: 0x06005650 RID: 22096 RVA: 0x0019EAAB File Offset: 0x0019CCAB
		// (set) Token: 0x06005651 RID: 22097 RVA: 0x0019EAB3 File Offset: 0x0019CCB3
		public string SessionId_ { get; set; }

		// Token: 0x1700069C RID: 1692
		// (get) Token: 0x06005652 RID: 22098 RVA: 0x0019EABC File Offset: 0x0019CCBC
		// (set) Token: 0x06005653 RID: 22099 RVA: 0x0019EAC4 File Offset: 0x0019CCC4
		public string DeviceUniqueIdentifier_ { get; set; }

		// Token: 0x06005654 RID: 22100 RVA: 0x0019EAD0 File Offset: 0x0019CCD0
		public override int GetHashCode()
		{
			int num = base.GetType().GetHashCode();
			num ^= this.Status_.GetHashCode();
			num ^= this.Locale_.GetHashCode();
			num ^= this.Platform_.GetHashCode();
			num ^= this.BnetRegion_.GetHashCode();
			num ^= this.GameAccountId_.GetHashCode();
			if (this.HasCurrentBuild_)
			{
				num ^= this.CurrentBuild_.GetHashCode();
			}
			if (this.HasNewBuild_)
			{
				num ^= this.NewBuild_.GetHashCode();
			}
			num ^= this.SessionId_.GetHashCode();
			return num ^ this.DeviceUniqueIdentifier_.GetHashCode();
		}

		// Token: 0x06005655 RID: 22101 RVA: 0x0019EB9C File Offset: 0x0019CD9C
		public override bool Equals(object obj)
		{
			DataOnlyPatching dataOnlyPatching = obj as DataOnlyPatching;
			return dataOnlyPatching != null && this.Status_.Equals(dataOnlyPatching.Status_) && this.Locale_.Equals(dataOnlyPatching.Locale_) && this.Platform_.Equals(dataOnlyPatching.Platform_) && this.BnetRegion_.Equals(dataOnlyPatching.BnetRegion_) && this.GameAccountId_.Equals(dataOnlyPatching.GameAccountId_) && this.HasCurrentBuild_ == dataOnlyPatching.HasCurrentBuild_ && (!this.HasCurrentBuild_ || this.CurrentBuild_.Equals(dataOnlyPatching.CurrentBuild_)) && this.HasNewBuild_ == dataOnlyPatching.HasNewBuild_ && (!this.HasNewBuild_ || this.NewBuild_.Equals(dataOnlyPatching.NewBuild_)) && this.SessionId_.Equals(dataOnlyPatching.SessionId_) && this.DeviceUniqueIdentifier_.Equals(dataOnlyPatching.DeviceUniqueIdentifier_);
		}

		// Token: 0x04003C9D RID: 15517
		public bool HasCurrentBuild_;

		// Token: 0x04003C9E RID: 15518
		private int _CurrentBuild_;

		// Token: 0x04003C9F RID: 15519
		public bool HasNewBuild_;

		// Token: 0x04003CA0 RID: 15520
		private int _NewBuild_;

		// Token: 0x02000907 RID: 2311
		public enum Status
		{
			// Token: 0x04003CA9 RID: 15529
			SUCCEED,
			// Token: 0x04003CAA RID: 15530
			SUCCEED_WITH_CACHE,
			// Token: 0x04003CAB RID: 15531
			SUCCEED_WITH_TIMEOVER,
			// Token: 0x04003CAC RID: 15532
			FAILED_GENERIC,
			// Token: 0x04003CAD RID: 15533
			FAILED_DOWNLOADING,
			// Token: 0x04003CAE RID: 15534
			FAILED_BAD_DATA,
			// Token: 0x04003CAF RID: 15535
			FAILED_MD5_MISMATCH,
			// Token: 0x04003CB0 RID: 15536
			FAILED_BAD_ASSETBUNDLE,
			// Token: 0x04003CB1 RID: 15537
			STARTED
		}

		// Token: 0x02000908 RID: 2312
		public enum Locale
		{
			// Token: 0x04003CB3 RID: 15539
			UnknownLocale,
			// Token: 0x04003CB4 RID: 15540
			enUS,
			// Token: 0x04003CB5 RID: 15541
			enGB,
			// Token: 0x04003CB6 RID: 15542
			frFR,
			// Token: 0x04003CB7 RID: 15543
			deDE,
			// Token: 0x04003CB8 RID: 15544
			koKR,
			// Token: 0x04003CB9 RID: 15545
			esES,
			// Token: 0x04003CBA RID: 15546
			esMX,
			// Token: 0x04003CBB RID: 15547
			ruRU,
			// Token: 0x04003CBC RID: 15548
			zhTW,
			// Token: 0x04003CBD RID: 15549
			zhCN,
			// Token: 0x04003CBE RID: 15550
			itIT,
			// Token: 0x04003CBF RID: 15551
			ptBR,
			// Token: 0x04003CC0 RID: 15552
			plPL,
			// Token: 0x04003CC1 RID: 15553
			Locale15 = 15,
			// Token: 0x04003CC2 RID: 15554
			Locale16
		}

		// Token: 0x02000909 RID: 2313
		public enum Platform
		{
			// Token: 0x04003CC4 RID: 15556
			UnknownPlatform,
			// Token: 0x04003CC5 RID: 15557
			Windows,
			// Token: 0x04003CC6 RID: 15558
			Mac,
			// Token: 0x04003CC7 RID: 15559
			iPad,
			// Token: 0x04003CC8 RID: 15560
			iPhone,
			// Token: 0x04003CC9 RID: 15561
			Android_Tablet,
			// Token: 0x04003CCA RID: 15562
			Android_Phone
		}

		// Token: 0x0200090A RID: 2314
		public enum BnetRegion
		{
			// Token: 0x04003CCC RID: 15564
			REGION_UNINITIALIZED = -1,
			// Token: 0x04003CCD RID: 15565
			REGION_UNKNOWN,
			// Token: 0x04003CCE RID: 15566
			REGION_US,
			// Token: 0x04003CCF RID: 15567
			REGION_EU,
			// Token: 0x04003CD0 RID: 15568
			REGION_KR,
			// Token: 0x04003CD1 RID: 15569
			REGION_TW,
			// Token: 0x04003CD2 RID: 15570
			REGION_CN,
			// Token: 0x04003CD3 RID: 15571
			REGION_LIVE_VERIFICATION = 40,
			// Token: 0x04003CD4 RID: 15572
			REGION_PTR_LOC,
			// Token: 0x04003CD5 RID: 15573
			REGION_DEV = 60
		}
	}
}
