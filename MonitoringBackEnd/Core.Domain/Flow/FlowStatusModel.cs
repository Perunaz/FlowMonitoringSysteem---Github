using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Flow
{
	public class FlowStatusModel
	{
		public Guid FlowId { get; set; }
		public string DisplayName { get; set; }
		public string Status { get; set; }
		public int SuccessPercentage { get; set; }
		public int RunCount { get; set; }
		public string Time {  get; set; }

		public FlowStatusModel() { }

		// Parameterized constructor with string parameter for FlowId
		public FlowStatusModel(string flowId, string displayName, string status, int successPercentage, int runCount, string time)
		{
			FlowId = Guid.Parse(flowId);
			DisplayName = displayName;
			Status = status;
			SuccessPercentage = successPercentage;
			RunCount = runCount;
			Time = time;
		}

		// Parameterized constructor with Guid parameter
		public FlowStatusModel(Guid flowId, string displayName, string status, int successPercentage, int runCount, string time)
		{
			FlowId = flowId;
			DisplayName = displayName;
			Status = status;
			SuccessPercentage = successPercentage;
			RunCount = runCount;
			Time = time;
		}

		public override string ToString()
		{
			return $"FlowId: {FlowId}, DisplayName: {DisplayName}, Status: {Status}, SuccessPercentage: {SuccessPercentage}, RunCount: {RunCount}, Time: {Time}";
		}
	}
}
