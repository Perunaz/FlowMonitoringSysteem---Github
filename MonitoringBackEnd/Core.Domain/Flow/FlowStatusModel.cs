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
		public string Time {  get; set; }

		public FlowStatusModel() { }

		// Parameterized constructor with string parameter for FlowId
		public FlowStatusModel(string flowId, string displayName, string status, string time)
		{
			FlowId = Guid.Parse(flowId);
			DisplayName = displayName;
			Status = status;
			Time = time;
		}

		// Parameterized constructor with Guid parameter
		public FlowStatusModel(Guid flowId, string displayName, string status, string time)
		{
			FlowId = flowId;
			DisplayName = displayName;
			Status = status;
			Time = time;
		}
	}
}
