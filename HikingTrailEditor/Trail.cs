using System;
using System.Collections.Generic;
using System.Text;

namespace HikingTrailEditor
{
    public class Trail
    {
        public int Trail_Id { get; set; }
        public string TrailName { get; set; }
        public string TrailLocation { get; set; }
        public int TrailLength { get; set; }
        public string TrailSummary { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
