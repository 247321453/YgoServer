namespace OcgWrapper.Enums
{
	public enum CtosMessage : byte
	{
		Unknown = 0,
		Response = 0x1,
		UpdateDeck = 0x2,
		HandResult = 0x3,
		TpResult = 0x4,
		PlayerInfo = 0x10,
		CreateGame = 0x11,
		JoinGame = 0x12,
		LeaveGame = 0x13,
		Surrender = 0x14,
		TimeConfirm = 0x15,
		Chat = 0x16,
		HsToDuelist = 0x20,
		HsToObserver = 0x21,
		HsReady = 0x22,
		HsNotReady = 0x23,
		HsKick = 0x24,
		HsStart = 0x25,
		/// <summary>
		/// 登录
		/// </summary>
		Login = 0x40,
		/// <summary>
		/// 加入房间
		/// </summary>
		PlayerJoin= 0x42,
		/// <summary>
		/// 离开房间
		/// </summary>
		PlayerLeave = 0x43,
	}
}