-GPA es el promedio de alumno

-El algoritmo guarda siempre el mejor dna que tiene

-No lo muta, trat� de mutarlo y hacer una copia de �l en otra parte pero no lo logr�, de alguna forma lo terminaba mutando igual

-Se usa reparaci�n en caso de que un DNA no cumpla restricciones.
Pens� que usar penalizaci�n ser�a m�s r�pido pero no hab�a mucha diferencia, de todos modos los dos est�n escritos en el c�digo.

-El esquema de selecci�n es elitista, se guardan los mejores 50% y se llevan al pool de reproducci�n

-La funci�n de fitness trata de favorecer promedios altos, ingresos bajos y costos altos de carrera, a la vez, la cantidad de becas dadas. Para esto hay coeficientes para promedio, ingreso y costo del estudiante. 
Se favorece en un 50% el buen promedio, 30% el ingreso bajo y 20% el costo alto de una carrera.
Para m�s info ver la funci�n studentFitness() de EvolutionaryAlgorithm 

-Todo individuo de esa mitad puede caer en crossover o ser copiado:

*Si hay crossover se hacen los dos posibles hijos de una pareja,
y luego se lanza una moneda, en una opcion se guardan las copias de padres y en la otra se generan dos nuevos individuos randoms

*Se copian los dos padres y se generan dos hijos random

Se tienen finalmente 4 individuos luego de ver una pareja del pool de reproducci�n ya que el pool es la mitad y las parejas otra mitad, 25%. As� se restablece el n�mero de poblaci�n.