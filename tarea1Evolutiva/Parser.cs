using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace tarea1Evolutiva
{
    class Parser
    {
        // Methods for "postulantes.txt"
        public List<Student> getStudents(string filePath) {
            try 
	        {
                List<Student> students = new List<Student>();
                int student_counter = 1;
		        using (StreamReader sr = new StreamReader(filePath))
	            {
		            string line;
                    while((line = sr.ReadLine()) != null)
                    {
                        students.Add(getStudent(line, student_counter));
                        student_counter++;
                    } 
	            }
                return students;
	        }
	        catch (Exception e)
	        {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
	        }
        }

        private Student getStudent(string line, int number)
        {
            string[] arr = line.Split(';');
            Student s = new Student(number);
            s.Name = arr[0];
            s.Income = Convert.ToInt32(arr[1]);
            s.Gpa = Convert.ToSingle(arr[2]);
            s.Cost = Convert.ToInt32(arr[3]);

            return s;
        }


        // Methods for "opt.parametros.txt"

        public Parameters getParameters(string filePath)
        {
            Queue<string> lines = new Queue<string>();

            using (StreamReader sr = new StreamReader(filePath))
            {
                string line;
                int counter = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    lines.Enqueue(line);
                    counter++;
                }
            }

            int gens = Convert.ToInt32(lines.Dequeue().Split('=')[1].Trim());
            short popu = Convert.ToInt16(lines.Dequeue().Split('=')[1].Trim());
            float xover = Convert.ToSingle(lines.Dequeue().Split('=')[1].Trim());
            float mut = Convert.ToSingle(lines.Dequeue().Split('=')[1].Trim());
            int money = Convert.ToInt32(lines.Dequeue().Split('=')[1].Trim());

            Parameters p = new Parameters();
            p.evoParams.gen = gens;
            p.evoParams.pop = popu;
            p.evoParams.xover = xover;
            p.evoParams.mut = mut;
            p.uParams.money = money;

            return p;
        }
    }
}
