-GPA es el promedio de alumno

-El algoritmo guarda siempre el mejor dna que tiene

-No lo muta, traté de mutarlo y hacer una copia de él en otra parte pero no lo logré, de alguna forma lo terminaba mutando igual

-Se usa reparación en caso de que un DNA no cumpla restricciones.
Pensé que usar penalización sería más rápido pero no había mucha diferencia, de todos modos los dos están escritos en el código.

-El esquema de selección es elitista, se guardan los mejores 50% y se llevan al pool de reproducción

-La función de fitness trata de favorecer promedios altos, ingresos bajos y costos altos de carrera, a la vez, la cantidad de becas dadas. Para esto hay coeficientes para promedio, ingreso y costo del estudiante. 
Se favorece en un 50% el buen promedio, 30% el ingreso bajo y 20% el costo alto de una carrera.
Para más info ver la función studentFitness() de EvolutionaryAlgorithm 

-Todo individuo de esa mitad puede caer en crossover o ser copiado:

*Si hay crossover se hacen los dos posibles hijos de una pareja,
y luego se lanza una moneda, en una opcion se guardan las copias de padres y en la otra se generan dos nuevos individuos randoms

*Se copian los dos padres y se generan dos hijos random

Se tienen finalmente 4 individuos luego de ver una pareja del pool de reproducción ya que el pool es la mitad y las parejas otra mitad, 25%. Así se restablece el número de población.