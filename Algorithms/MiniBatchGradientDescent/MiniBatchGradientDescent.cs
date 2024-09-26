using System;

class MiniBatchGradientDescent
{
    static void Main(string[] args)
    {
        // Anfangswerte der Parameter (z.B. x und y)
        double x = 5.0;
        double y = 5.0;

        // Lernrate
        double learningRate = 0.1;

        // Anzahl der Iterationen
        int numIterations = 1000;

        // Mini-Batch-Größe
        int batchSize = 10;

        // Simulierte Trainingsdaten (z.B. Zufallsdaten)
        Random random = new Random();
        double[,] data = new double[100, 2];
        for (int i = 0; i < 100; i++)
        {
            data[i, 0] = random.NextDouble();
            data[i, 1] = random.NextDouble();
        }

        // Mini-Batch Gradient Descent
        for (int iter = 0; iter < numIterations; iter++)
        {
            double gradX = 0.0;
            double gradY = 0.0;

            // Auswahl eines zufälligen Mini-Batches
            for (int i = 0; i < batchSize; i++)
            {
                int index = random.Next(0, 100);

                // Berechnung des Gradienten für den aktuellen Punkt
                gradX += 2 * (x - data[index, 0]);
                gradY += 2 * (y - data[index, 1]);
            }

            // Mittelung der Gradienten
            gradX /= batchSize;
            gradY /= batchSize;

            // Aktualisierung der Parameter
            x -= learningRate * gradX;
            y -= learningRate * gradY;

            // Ausgabe des Fortschritts
            if (iter % 100 == 0)
            {
                Console.WriteLine($"Iteration {iter}: x = {x}, y = {y}");
            }
        }

        Console.WriteLine($"Endergebnis: x = {x}, y = {y}");
    }
}
