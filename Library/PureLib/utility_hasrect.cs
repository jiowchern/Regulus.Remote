using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
namespace Regulus.Utility
{
	
	/// <summary>
	/// An interface that defines and object with a rectangle
	/// </summary>
	public interface IHasRect
	{
		RectangleF Rectangle { get; }
	}
}
