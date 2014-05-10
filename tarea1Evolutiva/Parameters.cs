using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarea1Evolutiva
{
    //Helper class for getting the parameters
    struct uParameters
    {
        public int money;
    }

    struct evoParameters
    {
        public int gen;
        public short pop;
        public float xover, mut;
    }

    struct Parameters
    {
        public uParameters uParams;
        public evoParameters evoParams;
    }

}
