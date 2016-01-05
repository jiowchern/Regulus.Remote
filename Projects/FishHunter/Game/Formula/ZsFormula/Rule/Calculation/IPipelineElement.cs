namespace VGame.Project.FishHunter.Formula.ZsFormula.Rule.Calculation
{
	public interface IPipelineElement
	{
		bool IsComplete { get; }
		
		void Connect(IPipelineElement next);

		void Process();
	}
}