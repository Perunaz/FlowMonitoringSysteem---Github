using System;
using System.Collections.Generic;

namespace Core.Domain.Flow
{
    public class FlowStatus
    {
        public Guid FlowId { get; set; }
        public string DisplayName { get; set; }
		public string Status { get; set; }

        // Parameterless constructor
        public FlowStatus()
        {
        }

        // Parameterized constructor with Guid parameter
        public FlowStatus(Guid flowId, string displayName, string status)
        {
            FlowId = flowId;
            DisplayName = displayName;
			Status = status;
        }

        // Parameterized constructor with string parameter for FlowId
        public FlowStatus(string flowId, string displayName, string status)
        {
            FlowId = Guid.Parse(flowId);
			DisplayName = displayName;
			Status = status;
        }
    }
}
