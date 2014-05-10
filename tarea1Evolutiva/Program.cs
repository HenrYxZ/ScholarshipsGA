using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarea1Evolutiva
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the scholarship optimizer, please,\n" +
                              "write the name of the text containing the parameters");
            Parser parser = new Parser();
            Parameters p = parser.getParameters(Console.ReadLine());
            //Parameters p = parser.getParameters("opt.parametros.txt");
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm(p.evoParams.gen, p.evoParams.pop, p.evoParams.xover, p.evoParams.mut);
            ea.Money = p.uParams.money;

            Console.WriteLine("Now write the name of the text containing the students");
            ea.Students = parser.getStudents(Console.ReadLine());
            //ea.Students = parser.getStudents("20141tarea1.csv");
            Console.WriteLine("The cost of assigning all the possible scholarships is " + ea.maxCost().ToString("C"));
            Console.WriteLine("Press 1 if you don't want realtime output,\n" +
                              "2 for really fast realtime output\n" +
                              "3 for readable realtime output");
            ea.setOption(Convert.ToInt32(Console.ReadLine()));
            Console.WriteLine("Press any key to start running the optimization");
            Console.ReadKey();
            ea.run();
            ea.showResults();
            ea.writeAssignation();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
