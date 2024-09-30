using System;

class StochasticGradientDescent
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

        // Simulierte Trainingsdaten (z.B. Zufallsdaten)
        Random random = new Random();
        double[,] data = new double[100, 2];
        for (int i = 0; i < 100; i++)
        {
            data[i, 0] = random.NextDouble();
            data[i, 1] = random.NextDouble();
        }

        // Stochastic Gradient Descent
        for (int iter = 0; iter < numIterations; iter++)
        {
            // Auswahl eines zufälligen Datenpunkts
            int index = random.Next(0, 100);

            // Berechnung des Gradienten für den ausgewählten Punkt
            double gradX = 2 * (x - data[index, 0]);
            double gradY = 2 * (y - data[index, 1]);

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
