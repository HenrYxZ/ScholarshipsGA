using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarea1Evolutiva
{
    class Dna
    {
        private BitArray _string;
        private float fitness;

        public float Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        public BitArray String
        {
            get { return _string; }
            set { _string = value; }
        }

        public Dna(int size)
        {
            // This DNA has two bits per student
            // representing 00 - no scholarship, 01 or 10 half-scholarship and 11 full scholarship.
            // (in this case, two different DNA's can represent the same thing because of the half-scholarship representation
            
            this.fitness = 0.0f;
            this._string = new BitArray(size);
            Random r = new Random();
            /*
            int randNum = r.Next(2 ^ size);
            BitArray randArray = new BitArray(new int[]{randNum});
            // this copies the randArray since _string has all of his elements as false
           for(int i = randArray.Count-1; i >= 0; i--)
           {
               if (randArray[i] == true)
                   this._string[i] = true;
           }
             * */
            // Random bit by bit (slower)
            for (int i = 0; i < _string.Count; i++)
            {
                if (r.Next(2) == 1)
                    _string[i] = true;

            }

        }

        public Dna(Dna x, Dna y)
        {
            this._string = new BitArray(x.String.Count);
            //This constructor makes the child of parents x and y using crossing over
            for (int i = 0; i < _string.Count / 2; i++)
            {
                _string[i] = x.String[i];
            }

            for (int j = _string.Count + 1; j < _string.Count; j++)
            {
                _string[j] = y.String[j];
            }
            this.fitness = 0.0f;
        }

    }
}
