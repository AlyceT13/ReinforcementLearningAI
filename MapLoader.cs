using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AprendizajeReforzado
{
    internal static class MapLoader
    {
        //Funciones de carga y dibujado del mapa

        //Cargar mapa desde un archivo de texto:
        public static char[,] Load(string ruta)
        {
            string[] lineas = File.ReadAllLines(ruta);
            int filas = lineas.Length;
            int columnas = lineas[0].Length;
            char[,] mapa = new char[filas, columnas];

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    mapa[i, j] = lineas[i][j];
                }
            }

            return mapa;
        }
        // Busca una letra específica ('S' o 'E') dentro del mapa (para el inicio y meta del mapa)
        public static(int, int) SearchLetter(char[,] mapa, char objetivo)
        {
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    if (mapa[i, j] == objetivo)
                    {
                        return (i, j);
                    }
                }
            }

            throw new Exception("No se encontró el carácter en el mapa.");
        }
        // Imprime el mapa en consola
        public static void Print(char[,] mapa)
        {
            for (int i = 0; i < mapa.GetLength(0); i++)
            {
                for (int j = 0; j < mapa.GetLength(1); j++)
                {
                    Console.Write(mapa[i, j]);
                }
                Console.WriteLine();
            }
        }
    }
}
