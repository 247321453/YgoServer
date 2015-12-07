using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using OcgWrapper.Enums;
using OcgWrapper.Helpers;

namespace OcgWrapper
{
	public class Duel
	{
		private IntPtr m_pDuel;
		private Func<GameMessage, BinaryReader, byte[], int> m_analyzer;
		private Action<string> m_errorHandler;
		private IntPtr m_buffer;

		public void SetAnalyzer(Func<GameMessage, BinaryReader, byte[], int> analyzer)
		{
			m_analyzer = analyzer;
		}

		public void SetErrorHandler(Action<string> errorHandler)
		{
			m_errorHandler = errorHandler;
		}
		public byte[] QueryFieldInfo()
		{
			Api.query_field_info(m_pDuel,  m_buffer);
			byte[] result = new byte[256];
			Marshal.Copy(m_buffer, result, 0, 256);
			return result;
		}
		public void InitPlayers(int startLp, int startHand, int drawCount)
		{
			Api.set_player_info(m_pDuel, 0, startLp, startHand, drawCount);
			Api.set_player_info(m_pDuel, 1, startLp, startHand, drawCount);
		}

		public void AddCard(int cardId, int owner, CardLocation location)
		{
			Api.new_card(m_pDuel, (uint)cardId, (byte)owner, (byte)owner, (byte)location, 0, 0);
		}

		public void AddTagCard(int cardId, int owner, CardLocation location)
		{
			Api.new_tag_card(m_pDuel, (uint)cardId, (byte)owner, (byte)location);
		}

		public void Start(int options)
		{
			Api.start_duel(m_pDuel, options);
		}

		public int Process()
		{
			if(m_pDuel == IntPtr.Zero) return -1;
			int fail = 0;
			while (true)
			{
                int result = -1;
                try {
                    result = Api.process(m_pDuel);
                } catch (Exception e){
                    Console.WriteLine("api.Process:" + e.ToString());
                }
				int len = result & 0xFFFF;

				if (len > 0)
				{
					fail = 0;
					byte[] arr = new byte[4096];
					Api.get_message(m_pDuel, m_buffer);
					Marshal.Copy(m_buffer, arr, 0, 4096);
					using(MemoryStream stream = new MemoryStream(arr)){
						using(BinaryReader reader= new BinaryReader(stream)){
							result = HandleMessage(reader, arr, len);
						}
					}
					if (result != 0)
						return result;
				}
				else if (++fail == 10)
					return -1;
			}
		}

		public void SetResponse(int resp)
		{
			Api.set_responsei(m_pDuel, (uint)resp);
		}

		public bool SetResponse(byte[] resp)
		{
			if (resp.Length > 64) return false;
			IntPtr buf = Marshal.AllocHGlobal(64);
			Marshal.Copy(resp, 0, buf, resp.Length);
			Api.set_responseb(m_pDuel, buf);
			try{
				Marshal.FreeHGlobal(buf);
                return true;
			}catch{
            }
            return false;
		}

		public int QueryFieldCount(int player, CardLocation location)
		{
			return Api.query_field_count(m_pDuel, (byte)player, (byte)location);
		}

		public byte[] QueryFieldCard(int player, CardLocation location, int flag, bool useCache)
		{
			int len = Api.query_field_card(m_pDuel, (byte)player, (byte)location, flag, m_buffer, useCache ? 1 : 0);
			byte[] result = new byte[len];
			Marshal.Copy(m_buffer, result, 0, len);
			return result;
		}

		public byte[] QueryCard(int player, int location, int sequence, int flag)
		{
			int len = Api.query_card(m_pDuel, (byte)player, (byte)location, (byte)sequence, flag, m_buffer, 0);
			byte[] result = new byte[len];
			Marshal.Copy(m_buffer, result, 0, len);
			return result;
		}

		public void End()
		{
			Api.end_duel(m_pDuel);
			Dispose();
		}

		public IntPtr GetNativePtr()
		{
			return m_pDuel;
		}

		internal Duel(IntPtr pDuel)
		{
			m_buffer = Marshal.AllocHGlobal(4096);
			m_pDuel = pDuel;
			lock(Duels){
				if(!Duels.ContainsKey(m_pDuel))
					Duels.Add(m_pDuel, this);
			}
		}

		internal void Dispose()
		{
			try{
				Marshal.FreeHGlobal(m_buffer);
			}catch(Exception){
				//Console.WriteLine(e.ToString());
			}
			lock(Duels)
				Duels.Remove(m_pDuel);
			m_pDuel = IntPtr.Zero;
		}

		internal void OnMessage(UInt32 messageType)
		{
			byte[] arr = new byte[256];
			Api.get_log_message(m_pDuel, m_buffer);
			Marshal.Copy(m_buffer, arr, 0, 256);
			string message = System.Text.Encoding.UTF8.GetString(arr);
			if (message.Contains("\0"))
				message = message.Substring(0, message.IndexOf('\0'));
			if (m_errorHandler != null)
				m_errorHandler.Invoke(message);
		}

		private int HandleMessage(BinaryReader reader, byte[] raw, int len)
		{
			while (reader.BaseStream.Position < len)
			{
				GameMessage msg = (GameMessage)reader.ReadByte();
				int result = -1;
				if (m_analyzer != null)
					result = m_analyzer.Invoke(msg, reader, raw);
				if (result != 0)
					return result;
			}
			return 0;
		}

		internal readonly static IDictionary<IntPtr, Duel> Duels = new ConcurrentDictionary<IntPtr, Duel>();

		public static Duel Create(uint seed)
		{
			MtRandom random = new MtRandom(seed);
			random.Reset(seed);
			IntPtr pDuel = Api.create_duel(random.Rand());
			return Create(pDuel);
		}

		internal static Duel Create(IntPtr pDuel)
		{
			if (pDuel == IntPtr.Zero)
				return null;
			lock(Duels){
				if(Duels.ContainsKey(pDuel)){
					//返回存在的？
					return Duels[pDuel];
				}
			}
			return new Duel(pDuel);
		}
	}
}
