using System.Collections.Generic;

namespace Core.Domain
{
    public class FlowStatus
    {
        public string Id { get; set; }
        public int FlowCategory { get; set; }
        public string FlowName { get; set; }
        public int StateCode { get; set; }

        public FlowStatus(string Id, int FlowCategory, string FlowName, int StateCode) { 
            this.Id = Id;
            this.FlowCategory = FlowCategory;
            this.FlowName = FlowName;
            this.StateCode = StateCode;
        }
    }
}
