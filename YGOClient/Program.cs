using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace YGOClient
{
	class Program
	{
		public static string PRO="ccygo";
		static void Main(string[] args)
		{
			if(args.Length >= 1){
				switch(args[0]){
					case "install":
						Protocol.Reg(PRO);
						MessageBox.Show("install ok");
						break;
					case "uninstall":
						Protocol.UnReg(PRO);
						MessageBox.Show("uninstall ok");
						break;
					default:
						if(args[0].StartsWith(PRO)){
							RoomTool.Command(args[0]);
							//MessageBox.Show(args[0]);
						}
						break;
				}
			}
			Environment.Exit(0);
		}

	}
}
