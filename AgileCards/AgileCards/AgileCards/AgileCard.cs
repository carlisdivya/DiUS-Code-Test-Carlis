using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgileCards
{
    /// <summary>
    /// Entity class to store Agile Card information
    /// </summary>
    class AgileCard
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Estimate { get; set; }
        public int ID { get; set; }
    }
}
