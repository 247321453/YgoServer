namespace OcgWrapper.Enums
{
    public enum StocMessage
    {
        GameMsg = 0x1,
        ErrorMsg = 0x2,
        SelectHand = 0x3,
        SelectTp = 0x4,
        HandResult = 0x5,
        TpResult = 0x6,
        ChangeSide = 0x7,
        WaitingSide = 0x8,
        CreateGame = 0x11,
        JoinGame = 0x12,
        TypeChange = 0x13,
        LeaveGame = 0x14,
        DuelStart = 0x15,
        DuelEnd = 0x16,
        Replay = 0x17,
        TimeLimit = 0x18,
        Chat = 0x19,
        HsPlayerEnter = 0x20,
        HsPlayerChange = 0x21,
        HsWatchChange = 0x22,
        
        /// <summary>
        /// 添加一个房间
        /// </summary>
        RoomCreate = 0x101,
        /// <summary>
        /// 关闭一个房间
        /// </summary>
        RoomClose  = 0x102,
        /// <summary>
        /// 添加一个玩家
        /// </summary>
        RoomAdd    = 0x103,
        /// <summary>
        /// 移除一个玩家
        /// </summary>
        RoomRemove = 0x104,
        /// <summary>
        /// 所有房间信息
        /// </summary>
        RoomList   = 0x105,
    }
}