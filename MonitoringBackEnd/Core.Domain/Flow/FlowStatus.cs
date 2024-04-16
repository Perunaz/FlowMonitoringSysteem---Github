using System;
using System.Collections.Generic;

namespace Core.Domain.Flow
{
    public class FlowStatus
    {
        public Guid FlowId { get; set; }
        public int StateCode { get; set; }

        // Parameterless constructor
        public FlowStatus()
        {
        }

        // Parameterized constructor with Guid parameter
        public FlowStatus(Guid flowId, int stateCode)
        {
            FlowId = flowId;
            StateCode = stateCode;
        }

        // Parameterized constructor with string parameter for FlowId
        public FlowStatus(string flowId, int stateCode)
        {
            FlowId = Guid.Parse(flowId);
            StateCode = stateCode;
        }
    }
}
