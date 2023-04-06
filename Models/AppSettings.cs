using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScenariosConfiguration.Models {
    public class AppSettings {

        public int UpdateFrequency { get; set; }

        public WatchDog WatchDog { get; set; } = new WatchDog();
        
    }

    public class WatchDog
    {
        public int Id { get; set;}

        public int UpdateTime { get; set; }

    }
}
