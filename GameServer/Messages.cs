/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/9/7
 * 时间: 22:16
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace YGOCore
{
	/// <summary>
	/// Description of Messages.
	/// </summary>
	public class Messages
	{
		public const string MSG_PLAYER_INGAME="玩家已经在游戏";
		public const string MSG_CLOSE ="服务器将3分钟后关闭";
		public const string MSG_HIGH_VERSION="你的游戏版本比服务器的高";
		public const string MSG_FULL="房间满了";
		public const string MSG_GAMEOVER="游戏结束";
		public const string MSG_SEND_FAIL="发送失败";
		public const string MSG_NO_AI="不能添加AI";
		public const string MSG_NO_FREE_AI="没有空闲AI";
		public const string MSG_ADD_AI="添加AI成功";
		public const string MSG_BAN_PCHAT="私聊已经禁止";
		public const string MSG_NOCHECKDECK="该房间不检查卡组，可以使用禁卡";
		public const string MSG_NOSHUFFLEDECK ="该房间卡组不洗牌，小心作弊";
		public const string MSG_ENABLE_PROIORITY="该房间使用旧规则（先手抽卡，单场地），非正常设置";
		public const string ERR_NAME_PASSWORD="用户名或者密码错误";
		public const string ERR_NAME_PASSWORD_LONG="名字和密码的长度过长，请修改密码。";
		public const string MSG_SIDE="请在120秒内换好side";
		public const string MSG_DISCONECT="玩家 {0} 掉线，请等待{1}秒，超时则算结束决斗";
		public const string MSG_TIP_TIME="你还有 {0} 秒 超时";
		public const string MSG_READY="{0} 准备好";
		public const string MSG_WATCH_SIDE="正在更换side";
		public const string MSG_PLAYER_BAN="你的帐号无法登陆本服务器";
		public const string ERR_NO_NAME="名字不能为空";
		public const string ERR_NO_PASS="密码不能为空";
		public const string ERR_AUTH_FAIL="登录失败";
		public const string ERR_IS_LOGIN="已经登录";
		public const string ERR_LOW_VERSION="游戏版本太低";
		public const string ERR_PASSWORD="房间密码错误";
		public const string ERR_NO_CLIENT ="用户:{0}未在线";
	}
}
