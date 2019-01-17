using System.Collections.Generic;

namespace Funda.API.Models
{
    internal class MakelaarResults
    {
        public List<MakelaarResult> Objects { get; set; } = new List<MakelaarResult>();
        public Paging Paging { get; set; }
    }

    internal class Paging
    {
        public string VolgendeUrl { get; set; }
    }
}