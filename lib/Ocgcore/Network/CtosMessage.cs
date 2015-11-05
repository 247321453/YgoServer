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
        /// 所有房间
        /// </summary>
        RoomList = 0x41,
        /// <summary>
        /// 进入房间，暂停刷新
        /// </summary>
        RoomJoin = 0x42,
        /// <summary>
        /// 离开房间，继续刷新
        /// </summary>
        Roomleave = 0x43,
        /// <summary>
        /// 房间外聊天
        /// </summary>
        OutRoomChat = 0x44,
    }
}