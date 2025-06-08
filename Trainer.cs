using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeReforzado
{
    internal class Trainer
    {
        // Función principal de entrenamiento del agente usando Q-Learning o SARSA
        public static void TrainAgent(char[,] map, (int x, int y) start, (int x, int y) goal, Parameters parameters, bool useQLearning)
        {
            // Inicialización de la tabla Q
            int rows = map.GetLength(0);
            int cols = map.GetLength(1);
            double[,,] Q = new double[rows, cols, 4]; // 4 acciones posibles (arriba, derecha, abajo, izquierda)
            int executions = 500;
            double epsilon = 0.1;
            Random random = new Random();

            // Métricas por ejecución
            List<int> nodesExpandedPerExecution = new List<int>();
            List<double> durationPerExecution = new List<double>();
            List<bool> successPerExecution = new List<bool>();
            List<(int, int)> lastMoves = new List<(int, int)>();

            for (int episode = 0; episode < executions; episode++)
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                int x = start.x, y = start.y;
                int action = random.Next(4);
                int nodesExpanded = 0;
                lastMoves.Clear();

                while ((x, y) != goal)
                {
                    // Movimiento tentativo
                    nodesExpanded++;
                    int nextX = x, nextY = y;
                    switch (action)
                    {
                        case 0: nextX--; break;
                        case 1: nextY++; break;
                        case 2: nextX++; break;
                        case 3: nextY--; break;
                    }

                    // Rechaza movimientos inválidos
                    if (nextX < 0 || nextY < 0 || nextX >= rows || nextY >= cols || map[nextX, nextY] == '#'){ nextX = x; nextY = y; }

                    // Recompensa
                    double reward = (nextX == goal.x && nextY == goal.y) ? parameters.FinishPrize : parameters.MovementPrize;

                    // Selección de acción
                    int nextAction = random.Next(4);
                    if (random.NextDouble() > epsilon)
                    {
                        double maxQ = double.MinValue;
                        for (int a = 0; a < 4; a++)
                            if (Q[nextX, nextY, a] > maxQ)
                            {
                                maxQ = Q[nextX, nextY, a];
                                nextAction = a;
                            }
                    }
                    // Q-Learning
                    if (useQLearning)
                    {
                        double maxNextQ = double.MinValue;
                        for (int a = 0; a < 4; a++)
                            maxNextQ = Math.Max(maxNextQ, Q[nextX, nextY, a]);
                        Q[x, y, action] += parameters.LearningRate * (reward + parameters.DiscountRate * maxNextQ - Q[x, y, action]);
                    }
                    else // SARSA
                    {
                        Q[x, y, action] += parameters.LearningRate * (reward + parameters.DiscountRate * Q[nextX, nextY, nextAction] - Q[x, y, action]);
                    }

                    // Guardar trayectoria del último episodio
                    if (episode == executions - 1)
                        lastMoves.Add((nextX, nextY));

                    x = nextX; y = nextY; action = nextAction;
                }

                sw.Stop();
                nodesExpandedPerExecution.Add(nodesExpanded);
                durationPerExecution.Add(sw.Elapsed.TotalMilliseconds);
                successPerExecution.Add(true);
            }

            // Muestra resultados del entrenamiento
            Console.WriteLine("\n RESUMEN DEL ENTRENAMIENTO");
            int successes = successPerExecution.Count(e => e);
            Console.WriteLine($" Éxitos: {successes}/{executions} ({(double)successes / executions * 100:F1}%)");
            Console.WriteLine($" Tiempo promedio: {durationPerExecution.Average():F2} ms");

            // Exporta resultados a CSV
            string fileName = useQLearning ? @"..\..\..\results\qlearning_results.csv" : @"..\..\..\results\sarsa_resuls.csv";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("Ejecución,NodosExpandidos,TiempoMs,Exito");
                for (int i = 0; i < executions; i++)
                {
                    sw.WriteLine($"\"{i + 1}\",\"{nodesExpandedPerExecution[i]}\",\"{durationPerExecution[i]:F2}\",\"{successPerExecution[i]}\"");
                }
            }
            Console.WriteLine("\n Resultados exportados a: " + fileName);

            // Simulación final de la ultima ejecución
            Console.WriteLine("\nSimulación de la última ejecución (post-entrenamiento):");
            RunSimulation(map, lastMoves);
        }
        private static void RunSimulation(char[,] map, List<(int, int)> path)
        {
            char[,] simMap = (char[,])map.Clone();
            foreach (var (x, y) in path)
            {
                simMap[x, y] = 'A';
                MapLoader.Print(simMap);
                Console.WriteLine();
                simMap[x, y] = '.';
            }
        }
    }
}
