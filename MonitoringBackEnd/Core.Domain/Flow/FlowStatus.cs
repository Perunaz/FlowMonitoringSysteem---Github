using System;
using System.Collections.Generic;

namespace Core.Domain.Flow
{
    public class FlowStatus
    {
        public Guid FlowId { get; set; }
        public string DisplayName { get; set; }
		public string Status { get; set; }
		public int SuccessPercentage { get; set; }
		public int RunCount { get; set; }

        // Parameterized constructor with Guid parameter
        public FlowStatus(Guid flowId, string displayName, string status, int successPercentage, int runCount)
        {
            FlowId = flowId;
            DisplayName = displayName;
			Status = status;
            SuccessPercentage = successPercentage;
            RunCount = runCount;
        }

        // Parameterized constructor with string parameter for FlowId
        public FlowStatus(string flowId, string displayName, string status, int successPercentage, int runCount)
        {
            FlowId = Guid.Parse(flowId);
			DisplayName = displayName;
			Status = status;
			SuccessPercentage = successPercentage;
			RunCount = runCount;
        }

        public override string ToString()
        {
            return $"FlowId: {FlowId}, DisplayName: {DisplayName}, Status: {Status}, SuccessPercentage: {SuccessPercentage}, RunCount: {RunCount}";
        }
    }
}
