/*
 * 由SharpDevelop创建。
 * 用户： Hasee
 * 日期: 2015/11/1
 * 时间: 9:42
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using YGOCore.Game;
using System.Text;

namespace YGOCore.Net
{
	/// <summary>
	/// Description of Password.
	/// </summary>
	public static class Password
	{
		public static string OnlyName(string namepassword){
			if(string.IsNullOrEmpty(namepassword)){
				return namepassword;
			}
			return namepassword.Split('$')[0];
		}
		public static string GetPwd(string namepassword){
			if(string.IsNullOrEmpty(namepassword)){
				return namepassword;
			}
			int i = namepassword.IndexOf('$');
			if(i>0 && i+1 < namepassword.Length){
				return namepassword.Substring(i+1);
			}
			return "";
		}
		public static bool CheckAuth(this GameSession client, string namepassword){
			if(namepassword==null || client.Server == null){
				return true;
			}
			if(!client.Server.CheckPlayerBan(namepassword)){
				client.ServerMessage(Messages.MSG_PLAYER_BAN);
				return false;
			}
			if(client.Server.Config.isNeedAuth || namepassword.StartsWith("[AI]")){
				string[] _names=namepassword.Split(new char[]{'$'}, 2);
				if(_names.Length==1){
					client.ServerMessage(Messages.ERR_NO_PASS);
					return false;
				}else{
					if(!client.Server.OnLogin(_names[0],_names[1])){
						//LobbyError("Auth Fail");
						if(Encoding.Default.GetBytes(namepassword).Length>=20){
							client.ServerMessage(Messages.ERR_NAME_PASSWORD_LONG);
						}else{
							client.ServerMessage(Messages.ERR_NAME_PASSWORD);
						}
						return false;
					}
				}
			}
			return true;
		}
	}
}
