using System;
using System.Linq;

using Regulus.Utility;

using VGame.Project.FishHunter.Common.Data;
using VGame.Project.FishHunter.Formula.ZsFormula.Data;

namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	/// <summary>
	///     算法伺服器-累積buffer規則
	/// </summary>
	public class AccumulationBuffer : IPipelineElement
	{
		private readonly HitRequest _Request;

		private readonly DataVisitor _Visitor;

		public AccumulationBuffer(DataVisitor visitor, HitRequest request)
		{
			_Visitor = visitor;
			_Request = request;
		}

		bool IPipelineElement.IsComplete
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IPipelineElement.Process()
		{
			var enumData = EnumHelper.GetEnums<FarmDataRoot.BufferNode.BUFFER_NAME>();

			foreach(var data
					in enumData.Select(buffer_type => _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, buffer_type)))
			{
				_AddBufferRate(data);
			}

			_MoveSpecBufferToNormalBuffer();
		}

		void IPipelineElement.Connect(IPipelineElement next)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///     當block裡的spec type裡有錢大於0就要搬動)，就要把spec的錢移到normal，spec清空
		/// </summary>
		private void _MoveSpecBufferToNormalBuffer()
		{
			var spec = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.SPEC);

			var normal = _Visitor.Farm.FindDataRoot(_Visitor.FocusBlockName, FarmDataRoot.BufferNode.BUFFER_NAME.NORMAL);

			normal.Buffer.WinScore += spec.Buffer.WinScore;

			spec.Buffer.WinScore = 0;
		}

		private void _AddBufferRate(FarmDataRoot root)
		{
			root.Buffer.Count += _Request.WeaponData.GetTotalBet() * root.Buffer.Rate;

			if(root.Buffer.Count < 1000)
			{
				return;
			}

			root.Buffer.WinScore += root.Buffer.Count / 1000;
			root.Buffer.Count = root.Buffer.Count % 1000;
		}
	}
}
