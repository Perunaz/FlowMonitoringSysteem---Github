using System.Collections.Generic;

namespace Core.Domain
{
    public class FlowStatus
    {
        public Guid FlowId { get; set; }
        public string FlowName { get; set; }
        public int FlowCategory { get; set; }
        public int StateCode { get; set; }

        public FlowStatus(Guid FlowId, string FlowName, int FlowCategory, int StateCode)
        {
            this.FlowId = FlowId;
            this.FlowName = FlowName;
            this.FlowCategory = FlowCategory;
            this.StateCode = StateCode;
        }

        public FlowStatus(string FlowId, string FlowName, int FlowCategory, int StateCode)
        {
            this.FlowId = Guid.Parse(FlowId);
            this.FlowName = FlowName;
            this.FlowCategory = FlowCategory;
            this.StateCode = StateCode;
        }
    }
}
