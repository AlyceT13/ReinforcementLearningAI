using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using AprendizajeReforzado;

class Program
{
    // Muestra un menú al usuario para elegir el algoritmo de aprendizaje
    static int Menu()
    {
        Console.WriteLine("Seleccione el algoritmo:");
        Console.WriteLine("1 - Q-Learning");
        Console.WriteLine("2 - SARSA");
        bool entradaValida = int.TryParse(Console.ReadLine(), out int selectedAlgorithm);

        if (!entradaValida)
        {
            Console.WriteLine("Entrada fuera de los valores. Se usará Q-Learning por defecto.");
            selectedAlgorithm = 1;
        }

        return selectedAlgorithm;
    }

    // Solicita al usuario parámetros como tasas de aprendizaje y recompensas
    static Parameters SetParameters()
    {
        var parameters = new Parameters();

        // Learning Rate
        Console.Write("\nIngrese el Learning Rate (ej: 0.1): ");
        bool entradaValida = double.TryParse(Console.ReadLine(), out double learningRate);
        parameters.LearningRate = (entradaValida && learningRate >= 0 && learningRate <= 1) ? learningRate : 0.1;

        // Discount Rate
        Console.Write("Ingrese el Discount Rate (ej: 0.9): ");
        entradaValida = double.TryParse(Console.ReadLine(), out double discountRate);
        parameters.DiscountRate = (entradaValida && discountRate >= 0 && discountRate <= 1) ? discountRate : 0.9;

        // Recompensa Meta
        Console.Write("Ingrese la recompensa al llegar a la meta (ej: 100): ");
        entradaValida = double.TryParse(Console.ReadLine(), out double recompensaMeta);
        parameters.FinishPrize = entradaValida ? recompensaMeta : 100;

        // Recompensa Movimiento
        Console.Write("Ingrese la recompensa por movimiento (ej: -1): ");
        entradaValida = double.TryParse(Console.ReadLine(), out double recompensaMovimiento);
        parameters.MovementPrize = entradaValida ? recompensaMovimiento : -1;

        return parameters;
    }

    static void Main()
    {
        Console.WriteLine("=== Aprendizaje por Refuerzo: Navegación en Laberinto ===");
        bool useQLearning = false;
        // Selección del algoritmo
        switch (Menu())
        {
            case 1:
                Console.WriteLine("Has seleccionado Q-Learning.");
                useQLearning = true;
                break;
            case 2:
                Console.WriteLine("Has seleccionado SARSA.");
                useQLearning = false;
                break;
        }

        Parameters parameters = SetParameters();

        // Configuración de parámetros y carga del mapa
        Console.WriteLine("Cargando mapa...");
        char[,] map = MapLoader.Load(@"..\..\..\maps\map1.txt");
        // Búsqueda de las coordenadas de inicio ('S') y fin ('E')
        (int startX, int startY) = MapLoader.SearchLetter(map, 'S');
        (int endX, int endY) = MapLoader.SearchLetter(map, 'E');

        Console.WriteLine("Mapa cargado correctamente:");
        MapLoader.Print(map);

        Console.WriteLine($"\nInicio en: ({startX},{startY})");
        Console.WriteLine($"Meta en: ({endX},{endY})");

        // Llama a la función principal de entrenamiento
        Trainer.TrainAgent(map, (startX, startY), (endX, endY), parameters, useQLearning);
    }
}
