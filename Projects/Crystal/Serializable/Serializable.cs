using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regulus.Project.Crystal
{
	[Serializable]
	public class AccountInfomation
	{
		public string Name { get; set; }
		public string Password { get; set; }
		public Guid Id { get; set; }
	}

	[Serializable]
	public enum LoginResult
	{
        Success,
        Fail,
        Repeat
	}
}