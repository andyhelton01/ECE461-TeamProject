namespace ECE461_CLI
{
	public abstract class Metric
	{
		public Library parentLibrary;

		public float score;

		public string name;

		public float weight; // must be overridden by child class

		public Metric(Library parentLibrary)
		{
			this.parentLibrary = parentLibrary;

		}


		/*
		 * <summary> calculates the score for this metric
		 */
		public abstract void Calculate();



	}


	public class NetScore : Metric { 
		
		public NetScore(Library parentLibrary) : base(parentLibrary)
		{
			this.weight = 0;
			this.name = "NetScore";
		}

	
		public override void Calculate()
		{
			// calculate a weighted average of all the scores of the other metricsf
			float runningSum = 0;
			float divisor = 0;
			foreach (Metric m in parentLibrary.metrics) {
				runningSum += m.weight * m.score;
				divisor += m.weight;
			}

			this.score = runningSum / divisor;
		}
	}


}