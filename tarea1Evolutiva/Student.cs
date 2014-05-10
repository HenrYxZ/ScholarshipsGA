using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarea1Evolutiva
{
    class Student
    {


        public Student(int num)
        {
            //identification number
            this.number = num;
        }

        /*Attributes*/
        private string name;

       
        private float gpa;
        private int number, income, cost;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float Gpa
        {
            get { return gpa; }
            set { gpa = value; }
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public int Income
        {
            get { return income; }
            set { income = value; }
        }

        public int Cost
        {
            get { return cost; }
            set { cost = value; }
        }

    }
}
