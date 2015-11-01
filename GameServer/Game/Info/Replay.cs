using System.IO;
using SevenZip.Compression.LZMA;
using System.Threading;

namespace YGOCore.Game
{
	public class Replay
	{
		public const uint FlagCompressed = 0x1;
		public const uint FlagTag = 0x2;

		public const int MaxReplaySize = 0x20000;

		public bool Disabled { get; private set; }
		public ReplayHeader Header;
		public BinaryWriter Writer { get; private set; }

		private MemoryStream m_stream;
		private byte[] m_data;
		/// <summary>
		/// 保存路径
		/// </summary>
		private string fileName;
		/// <summary>
		/// 自动保存
		/// </summary>
		public bool AutoReplay;

		public Replay(string filename,bool autosave, int clientVersion, int mode,uint seed, bool tag)
		{
			Header.Id = 0x31707279;
			Header.Version = (uint)clientVersion;
			Header.Flag = tag ? FlagTag : 0;
			Header.Seed = seed;

			fileName = filename;
			AutoReplay = autosave;
			m_stream =new MemoryStream();
			Writer = new BinaryWriter(m_stream);
		}

		public void Check()
		{
			if (m_stream.Position >= MaxReplaySize)
			{
				Writer.Close();
				m_stream.Dispose();
				Disabled = true;
			}
		}

		public void End()
		{
			if (Disabled)
				return;

			byte[] raw = m_stream.ToArray();

			Header.DataSize = (uint)raw.Length;
			Header.Flag |= FlagCompressed;
			Header.Props = new byte[8];

			Encoder lzma = new Encoder();
			using (MemoryStream props = new MemoryStream(Header.Props))
				lzma.WriteCoderProperties(props);

			MemoryStream compressed = new MemoryStream();
			lzma.Code(new MemoryStream(raw), compressed, raw.LongLength, -1, null);

			raw = compressed.ToArray();

			MemoryStream ms = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(ms);

			writer.Write(Header.Id);
			writer.Write(Header.Version);
			writer.Write(Header.Flag);
			writer.Write(Header.Seed);
			writer.Write(Header.DataSize);
			writer.Write(Header.Hash);
			writer.Write(Header.Props);

			writer.Write(raw);

			m_data = ms.ToArray();
			
			if(AutoReplay){
				ThreadPool.QueueUserWorkItem(new WaitCallback(saveYrp));
			}
		}
		
		private void saveYrp(object obj){
			if(!File.Exists(fileName)){
				Tool.CreateDirectory(Tool.GetDir(fileName));
				try{
					File.WriteAllBytes(fileName, m_data);
				}catch(IOException){
					
				}
			}
		}

		public byte[] GetFile()
		{
			return m_data;
		}
	}
}