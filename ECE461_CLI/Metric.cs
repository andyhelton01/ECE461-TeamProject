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
				if (m is NetScore) continue; // dont try to count ourselves

				runningSum += m.weight * m.score;
				divisor += m.weight;
			}

			if (divisor == 0) divisor = 1; // avoid a divide by zero (most likely because this lib has no metrics other than netscore)

			this.score = runningSum / divisor;
		}
	}


}