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
		public int StateCode { get; set; }
		public string Time {  get; set; }

		public FlowStatusModel() { }

		// Parameterized constructor with string parameter for FlowId
		public FlowStatusModel(string flowId, int stateCode, string time)
		{
			FlowId = Guid.Parse(flowId);
			StateCode = stateCode;
			Time = time;
		}

		// Parameterized constructor with Guid parameter
		public FlowStatusModel(Guid flowId, int stateCode, string time)
		{
			FlowId = flowId;
			StateCode = stateCode;
			Time = time;
		}
	}
}
