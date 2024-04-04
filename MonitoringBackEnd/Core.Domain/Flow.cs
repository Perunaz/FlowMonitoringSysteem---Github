using System.Collections.Generic;

namespace Core.Domain
{
    public class Flow
    {
        public string Id { get; set; }
        public int Category { get; set; }
        public string Name { get; set; }
        public int StateCode { get; set; }
    }
}
