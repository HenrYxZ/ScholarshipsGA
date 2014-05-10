using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tarea1Evolutiva
{
    class EvolutionaryAlgorithm
    {
        //***********************************   Attributes      ***********************************

        private int generations, money, avgCost, avgIncome, actualGen;
        private short populationSize;
        private float coefXover, coefMutation, gpaFactor, incomeFactor, costFactor, avgGpa;
        private List<Student> students;
        private List<Dna> population;
        private List<Dna> reproductionPool;
        private Dna bestDna;
        private uParameters universityParameters;
        private float totalFit;
        private float minGpa;
        private int minIncome;
        private int minCost;

        // 1 for no realTime output, 2 for raw realTime output and 3 for realtime output with sleep(30)
        private int option = 3;

        public int Money
        {
            get { return money; }
            set { money = value; }
        }

        public float GpaFactor
        {
            get { return gpaFactor; }
            set { gpaFactor = value; }
        }

        public float IncomeFactor
        {
            get { return incomeFactor; }
            set { incomeFactor = value; }
        }

        public float CostFactor
        {
            get { return costFactor; }
            set { costFactor = value; }
        }

        public List<Student> Students
        {
            get { return students; }
            set { students = value; }
        }

        public List<Dna> Population
        {
            get { return population; }
            set { population = value; }
        }

        public uParameters UniversityParameters
        {
            get { return universityParameters; }
            set { universityParameters = value; }
        }



        //***********************************       Methods     *********************************** 
        public EvolutionaryAlgorithm(int num_gens, short pop_size, float c_xover, float c_mutation)
        {
            this.generations = num_gens;
            this.populationSize = pop_size;
            this.coefXover = c_xover;
            this.coefMutation = c_mutation;

            // Factors for the fitness function
            this.gpaFactor = 0.5f;
            this.incomeFactor = 0.3f;
            this.costFactor = 0.2f;

            this.population = new List<Dna>(pop_size);
            this.reproductionPool = new List<Dna>(pop_size / 2);
        }

        public void init()
        {
            // Initialize population
            for (int i = 0; i < populationSize; i++)
            {
                this.population.Add(new Dna(students.Count * 2));
            }
        }

        public void run()
        {
            init();
            setUniversityAverages();
            BitArray bi = new BitArray(2 * students.Count, true);
            this.bestDna = new Dna(2 * students.Count);
            this.bestDna.String = bi;

            for (this.actualGen = 1; this.actualGen <= generations; this.actualGen++)
            {
                select();
                reproduce();
                mutate();
                repair();

                if(option != 1)
                {
                    realTimeInfo();
                    if (option == 3)
                        System.Threading.Thread.Sleep(30);
                }
                

            }
        }

        public void realTimeInfo()
        {
            Console.WriteLine("Total fitness " + totalFit);
            // Writes the best dna cost
            Console.WriteLine("Best DNA cost " + assignationCost(bestDna).ToString("C"));
            Console.WriteLine("Best DNA fitness " + bestDna.Fitness);
            Stats st = getStatsFromDna(this.bestDna);
            print(st);
            // Time to look at the output
            //System.Threading.Thread.Sleep(60);
        }
        
        public void setOption(int opt)
        {
            this.option = opt;
        }
        
        //      Selection Methods

        public void select()
        {
            totalFit = totalFitness(); 
            // Selects the best 50% 
            population.OrderByDescending(x => x.Fitness);
            reproductionPool = population.Take(populationSize / 2).ToList<Dna>();
        }


        //      Fitness Methods
        
        public float studentFitness(Student s, Dna d, int index)
        {
            // If the scholarship assigned doesn't fulfill the restriction, this function will reassign it giving the real fitness
            // Under gpa 5.0 or Income more than $1.600.000
            if (s.Gpa < 5.0f || s.Income > 1600000)
            {
                d.String[index] = false;
                d.String[index + 1] = false;
                return 0;
            }

            BitArray bi = new BitArray(new bool[2]{d.String[index], d.String[index+1]});
            Scholarship sch = mapArrayToScholarship(bi);
            if (sch == Scholarship.None)
                return 0;
            float gpa = gpaFactor * s.Gpa / minGpa;
            float income = incomeFactor * s.Income / minIncome;
            float cost = costFactor * s.Cost / minCost;

            if (sch == Scholarship.Half)
                return gpa - income + cost/ 2 + 1;
                //return 0.5f;
            else
            {
                if (s.Income >= 1000000)
                {
                    reassign(d, s, Scholarship.Half);
                    return gpa - income + cost / 2 + 1;
                    //return 0.5f;
                }
                else
                    return gpa - income + cost + 1;
                    //return 1;
            }
        }

        public float fitness(Dna d)
        {
            float f = 0;
            for (int i = 0; i < students.Count; i++)
            {
                f += studentFitness(students[i], d, i*2);
            }
            d.Fitness = f;
            return f;
        }

        public float totalFitness()
        {
            float total = 0.0f;
            float bestFitness = 0.0f;
            Dna bestDnaInCurrentGeneration = population[0];
            for (int i = 0; i < population.Count; i++)
            {
                total += fitness(population[i]);
                if( population[i].Fitness  > bestFitness)
                {
                    bestFitness = population[i].Fitness;
                    bestDnaInCurrentGeneration = population[i];
                }

            }
            if(bestDna.Fitness < bestDnaInCurrentGeneration.Fitness)
                bestDna = bestDnaInCurrentGeneration;

            return total;
        }

        //      Reproduction Methods
        private void reproduce()
        {
            List<Dna> newPopulation = new List<Dna>(populationSize);
            Random xover = new Random();
            // generates pairs of parents and their child
            // the last with the first

            //pairs
            Random index = new Random();
            List<DnaPair> Parents = new List<DnaPair>();
            int pos;
            while(Parents.Count < reproductionPool.Count/2)
            {
                pos = index.Next(reproductionPool.Count);
                Dna x = reproductionPool[pos];
                reproductionPool.RemoveAt(pos);

                pos = index.Next(reproductionPool.Count);
                Dna y = reproductionPool[pos];
                reproductionPool.RemoveAt(pos);

                DnaPair p = new DnaPair(x, y);
                Parents.Add(p);
            }

            foreach (DnaPair p in Parents)
            {
                if(xover.NextDouble() <= coefXover)
                {
                    newPopulation.Add(new Dna(p.x, p.y));
                    newPopulation.Add(new Dna(p.y, p.x));
                    if(xover.Next(2) == 1)
                    {
                        newPopulation.Add(p.x);
                        newPopulation.Add(p.y);
                    }
                    else
                    {
                        newPopulation.Add(new Dna(students.Count * 2));
                        newPopulation.Add(new Dna(students.Count * 2));
                    }
                }
                else
                {
                    newPopulation.Add(p.x);
                    newPopulation.Add(p.y);
                    newPopulation.Add(new Dna(students.Count * 2));
                    newPopulation.Add(new Dna(students.Count * 2));
                }
            }

            /*
            for (int i = 0; i < reproductionPool.Count; i++)
            {
                if(xover.NextDouble() <= coefXover)
                {
                    newPopulation.Add(new Dna(reproductionPool[i], reproductionPool[populationSize / 2 - i - 1]));
                    newPopulation.Add(new Dna(reproductionPool[populationSize / 2 - i - 1], reproductionPool[i]));
                }
                    
                else
                {
                    // If we don't do simple crossing over add a new one or the original
                    int rand = xover.Next(2);
                    if (rand == 1)
                        newPopulation.Add(new Dna(students.Count * 2));
                    else
                        newPopulation.Add(reproductionPool[i]);
                }    
            }
             * */

            // Make sure we don't remove the bestDna
            newPopulation.Add(bestDna);
            if (newPopulation.Count > populationSize)
                newPopulation.RemoveAt(0);
            population = newPopulation;
            
        }


        // Mutation Methods

        private void mutate()
        {
            // We don't want to mutate the best candidate
            Random r = new Random();
            for(int i = 0; i < population.Count; i++)
            {
                if (r.NextDouble() <= coefMutation && population[i].Fitness != bestDna.Fitness)
                    mutate(population[i]);
            }
        }

        private void mutate(Dna dna)
        {
            // changes 4 bits
            int numberOfChanges = 4;
            Random randStringIndex = new Random();

            /* (didn't work)
            // If we mutate the best candidate, save a copy of him
            if (dna.Fitness == bestDna.Fitness)
            {
                population[0] = bestDna;
                bestDna = population[0];
            }
            */
            for (int i = 0; i < numberOfChanges; i++)
            {
                int index = randStringIndex.Next(dna.String.Count - 1);
                if (dna.String[index])
                    dna.String[index] = false;
                else
                    dna.String[index] = true;
            }

        }

        //      Restrictions Methods

        private void repair()
        {
            for(int i=0; i<population.Count; i++)
            {
                int dnaCost = assignationCost(population[i]);
                if (dnaCost > money)
                    repair(population[i], dnaCost);
                    //penalize(population[i], dnaCost);
            }
        }

        private void penalize(Dna d, int totalCost)
        {
            d.Fitness -= (totalCost - money)/totalCost;
            Console.WriteLine("Penalized, new fitness " + d.Fitness);
        }

        private void repair(Dna d, int totalCost)
        {
            // Remove random scholarships until the sum of them is less than the money available
            Random r = new Random();
            int difference = totalCost - money;
            while (difference > 0)
            {
                int studentIndex = r.Next(students.Count - 1);
                Student student = students[studentIndex];
                BitArray studentBits = getBitsFromDna(student, d);
                Scholarship assigned = getScholarshipFromDna(student, d);
                if (assigned == Scholarship.Full)
                { 
                    reassign(d, student, Scholarship.Half);
                    difference -= student.Cost / 2;
                }
                else if (assigned == Scholarship.Half)
                {
                    reassign(d, student, Scholarship.None);
                    difference -= student.Cost / 2;
                }
            }
            
            
        }

        private void reassign(Dna d, Student s, Scholarship newSch)
        {
            if(newSch == Scholarship.Full)
            {
                d.String[(s.Number - 1) * 2] = true;
                d.String[(s.Number - 1) * 2 + 1] = true;
            }

            else if(newSch == Scholarship.Half)
            {
                d.String[(s.Number - 1) * 2] = true;
                d.String[(s.Number - 1) * 2 + 1] = false;
            }
            else
            {
                d.String[(s.Number - 1) * 2] = false;
                d.String[(s.Number - 1) * 2 + 1] = false;
            }
        }


        //***********************************       Helper Methods     *********************************** 

        private void setUniversityAverages()
        {
            int totalIncome = 0, totalCost = 0;
            float totalGpa = 0;

            minCost = students[0].Cost;
            minGpa = students[0].Gpa;
            minIncome = students[0].Income;

            for (int i = 0; i < students.Count; i++)
            {
                if (students[i].Cost < minCost)
                    minCost = students[i].Cost;

                if (students[i].Gpa < minGpa)
                    minGpa = students[i].Gpa;

                if (students[i].Income < minIncome)
                    minIncome = students[i].Income;


                totalIncome += students[i].Income;
                totalGpa += students[i].Gpa;
                totalCost += students[i].Cost;
            }

            avgIncome = totalIncome / students.Count;
            avgGpa = totalGpa / students.Count;
            avgCost = totalCost / students.Count;

        }

        private BitArray getBitsFromDna(Student s, Dna d)
        {
            bool first = d.String[(s.Number - 1) * 2];
            bool second = d.String[(s.Number - 1) * 2 + 1];
            return new BitArray(new bool[2] { first,  second});
        }

        private Scholarship mapArrayToScholarship(BitArray b) 
        {
            Scholarship sch;
            if (b[0] && b[1])
                sch = Scholarship.Full;
            else if (b[0] || b[1])
                sch = Scholarship.Half;
            else
                sch = Scholarship.None;
            return sch;
        }

        private Scholarship getScholarshipFromDna(Student s, Dna d)
        {
            return mapArrayToScholarship(getBitsFromDna(s, d));
        }

        // Maybe this function won't be needed
        private static int compareDna(Dna x, Dna y)
        {
            return (int)(x.Fitness - y.Fitness);
        }

        private Stats getStatsFromDna(Dna d)
        {
            Stats stats = new Stats();
            int totalCost = 0, totalIncome = 0, numFulls = 0, numHalfs = 0, numNones = 0;
            float totalGpa = 0;

            foreach (Student s in students)
            {
                Scholarship assigned = getScholarshipFromDna(s, d);
                
                if (assigned == Scholarship.Full)
                {
                    totalCost += s.Cost;
                    totalIncome += s.Income;
                    numFulls++;
                    totalGpa += s.Gpa;
                }

                else if (assigned == Scholarship.Half)
                {
                    totalCost += s.Cost / 2;
                    totalIncome += s.Income;
                    numHalfs++;
                    totalGpa += s.Gpa;
                }
                else
                    numNones++;
            }
            if (numFulls + numHalfs != 0)
            {
                stats.avgCost = totalCost / (numFulls + numHalfs);
                stats.avgIncome = totalIncome / (numFulls + numHalfs);
                stats.avgGpa = totalGpa / (numFulls + numHalfs);
                stats.numFulls = numFulls;
                stats.numHalfs = numHalfs;
                stats.numNones = numNones;
            }
            else
            {
                stats.avgCost = 0;
                stats.avgIncome = 0;
                stats.avgGpa = 0.0f;
                stats.numFulls =0;
                stats.numHalfs = 0;
                stats.numNones = students.Count;
            }
            
            return stats;
        }

        private void print(Stats st)
        {
            String s = "In generation {0} stats are:\n"+
                       "Average cost: {1}, average income: {2}, number of fulls: {3}, number of halfs: {4}, number of nones: {5},\n"+
                       "average gpa: {6} \n";
            Console.WriteLine(s, this.actualGen, st.avgCost.ToString("C"), st.avgIncome.ToString("C"), st.numFulls, st.numHalfs, st.numNones, st.avgGpa);
        }

        public void showResults()
        {
            Stats st = getStatsFromDna(bestDna);
            print(st);
            Console.WriteLine("End of the simulation, the best assignation has been saved in the file assignation.txt \n");
        }

        private string print(Scholarship sch)
        {
            string s = "";
            if (sch == Scholarship.Full)
                s = "Total";
            else if (sch == Scholarship.Half)
                s = "Media";
            else
                s = "Nada";
            return s;
        }

        public void writeAssignation()
        {
            using (StreamWriter sw = new StreamWriter("assignation.txt"))
            {
                sw.WriteLine("ASIGNACION OPTIMA");
                int totalCost = 0;
                foreach (Student s in students)
                {
                    Scholarship sch = getScholarshipFromDna(s, bestDna);
                    sw.WriteLine("Postulante " + s.Number + ", " + print(sch));
                    totalCost += assignationCost(sch, s);
                }
                sw.WriteLine("Valor total asignado: " + totalCost.ToString("C"));
            }
            
        }

        private int assignationCost(Scholarship sch, Student s)
        {
            if (sch == Scholarship.Full)
                return s.Cost;
            else if (sch == Scholarship.Half)
                return s.Cost / 2;
            else
                return 0;
        }

        private int assignationCost(Dna d)
        {
            int totalCost = 0;
            Scholarship assigned;
            foreach (Student s in students)
            {
                assigned = getScholarshipFromDna(s, d);
                totalCost += assignationCost(assigned, s);
            }
            return totalCost;
        }

        public int maxCost()
        {
            // The cost of assigning every possible scholarship
            int maxCost = 0;
            foreach (Student s in students)
            {
                if (s.Income < 1600000 && s.Gpa > 5.0f)
                {
                    if (s.Income < 1000000)
                        maxCost += s.Cost;
                    else
                        maxCost += s.Cost / 2;
                }
            }
            return maxCost;
        }

        //***********************************       Enumerations     *********************************** 

        enum Scholarship
        {
            None,
            Half,
            Full
        };

        struct Stats
        {
            public int avgCost, avgIncome, numFulls, numHalfs, numNones;
            public float avgGpa;
        }

        struct DnaPair
        {
            public Dna x;
            public Dna y;

            public DnaPair(Dna x1, Dna y1)
            {
                // TODO: Complete member initialization
                this.x = x1;
                this.y = y1;
            }
        }

        
    }
}
