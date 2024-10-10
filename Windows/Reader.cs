
#define TAKE_RANDOM_SAMPLES

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.FileIO;

// Important Note: Reader supports one column for output only

public class Reader
{
    protected String inputFile;
    protected List<List<double>> inputData, inputDataLowered, inputTestData;
    protected List<double> outputData, outputDataLowered, outputTestData;
    protected List<String[]> inputString;
    protected int numOutputClassifiers;
    protected int numInputNodes;
    protected List<String> classifiers;


    public int getNumClassifiers() { return numOutputClassifiers; }
    public int getNumInputNodes() { return numInputNodes; }
    public String[] getColumnNames() { return inputString.ElementAt(0); }
    public String[] getClassifiers() { return classifiers.ToArray(); }
    public int getNumTotalData() { return inputString.Count()-1; }
    public ref List<List<double>> getInputData() { return ref inputData; }
    public ref List<double> getOutputData() { return ref outputData; }
    public double getOutputData(int index) { return outputData[index]; }
    public ref List<List<double>> getInputTrainData() { return ref inputDataLowered; }
    public double getOutputTrainData(int index) { return outputDataLowered[index]; }
    public ref List<List<double>> getInputTestData() { return ref inputTestData; }
    public double getOutputTestData(int index) { return outputTestData[index]; }

    public Reader()
	{
        inputFile = null;
        inputData = new List<List<double>>();
        inputDataLowered = new List<List<double>>();
        inputTestData = new List<List<double>>();
        outputData = new List<double>();
        outputDataLowered = new List<double>();
        outputTestData = new List<double>();
        inputString = new List<String[]>();
        numOutputClassifiers = 0;
        numInputNodes = 0;
        classifiers = new List<String>();
    }

    public Reader(String dataFile)
    {
        inputFile = dataFile;
        inputData = new List<List<double>>();
        inputDataLowered = new List<List<double>>();
        inputTestData = new List<List<double>>();
        outputData = new List<double>();
        outputDataLowered = new List<double>();
        outputTestData = new List<double>();
        inputString = new List<String[]>();
        numOutputClassifiers = 0;   
        numInputNodes = 0;
        classifiers = new List<String>();
    }

    public void Convert()
    {
        TextFieldParser csvParser = new TextFieldParser(inputFile);
        
        csvParser.CommentTokens = new string[] { "#" };
        csvParser.SetDelimiters(new string[] { "," });
        csvParser.HasFieldsEnclosedInQuotes = true;

        inputString.Clear();
        inputData.Clear();
        outputData.Clear();

        // Skip the row with the column names
        //csvParser.ReadLine();
        // Read column names
        inputString.Add(csvParser.ReadFields());

        while (!csvParser.EndOfData)
        {
            // Read current line fields, pointer moves to the next line.
            inputString.Add(csvParser.ReadFields());
        }
       
        numInputNodes = inputString.ElementAt(1).Count() - 1;
        int lastColumn = numInputNodes;

        // Check columns for String or Number
        for (int k = 0; k < inputString.ElementAt(1).Count(); k++)
        {
            List<double> column = new List<double>();
            classifiers.Clear();
            // Check line for String or Number 
            for (int i = 1; i < inputString.Count(); i++)
            {
                //String element = inputString.ElementAt(1).Last();
                String element = inputString.ElementAt(i).ElementAt(k);
                double number = 0.0;
                bool canConvert = Double.TryParse(element, NumberStyles.Any, CultureInfo.InvariantCulture, out number);
                if (canConvert == true)
                {
                    column.Add(number);
                }
                else
                {
                    //element = inputString.ElementAt(i).Last();
                    //element = inputString.ElementAt(i).ElementAt(k);
                    bool avail = false;
                    for (int j = 0; j < classifiers.Count(); j++)
                    {
                        if (element == classifiers.ElementAt(j))
                        {
                            avail = true;
                            column.Add((double)j);
                            break;
                        }
                    }
                    if (avail == false)
                    {
                        classifiers.Add(element);
                        column.Add((double)(classifiers.Count()-1));
                    }
                }
            }
            if (k < lastColumn)
            {
                inputData.Add(column);
            }
            else
            {
                outputData = column;
                if (classifiers.Count() == 0)
                {
                    numOutputClassifiers = 1;
                }
                else
                {
                    numOutputClassifiers = classifiers.Count();
                }
            }
        }

        inputDataLowered.Clear();
        inputTestData.Clear();
        for ( int i = 0; i < inputData.Count; i++ )
        {
            List<double> columnLowered = new List<double>();
            List<double> columnTest = new List<double>();
            for (int j = 0; j < inputData[i].Count; j++)
            {
                columnLowered.Add(inputData[i][j]);
                columnTest.Add(inputData[i][j]);
            }
            inputDataLowered.Add(columnLowered);
            inputTestData.Add(columnTest);
        }
        outputDataLowered.Clear();
        outputTestData.Clear();
        outputDataLowered.AddRange(outputData);
        outputTestData.AddRange(outputData);
    }



    public void LimitData( double percentageLimit )
    {
#if TAKE_RANDOM_SAMPLES
        inputDataLowered.Clear();
        for (int i = 0; i < inputData.Count; i++)
        {
            List<double> columnLowered = new List<double>();
            for (int j = 0; j < inputData[i].Count; j++)
            {
                columnLowered.Add(inputData[i][j]);
            }
            inputDataLowered.Add(columnLowered);
        }
        outputDataLowered.Clear();
        outputDataLowered.AddRange(outputData);

        double toRemovePer = (100.0 - percentageLimit) * 0.01;
        int toRemoveNum = (int)(toRemovePer * (double)getNumTotalData());

        var random = new Random(Guid.NewGuid().GetHashCode());

        for (int j = 0; j < toRemoveNum; j++)
        {
            // Random Index
            int randIdx = random.Next(outputDataLowered.Count);
            // For all columns
            for (int i = 0; i < inputDataLowered.Count(); i++)
            {
                if(inputDataLowered[i].Count < randIdx)
                {
                    randIdx = inputDataLowered[i].Count-1;
                }
                inputDataLowered[i].RemoveAt(randIdx);
            }
            outputDataLowered.RemoveAt(randIdx);
        }
#else
        inputDataLowered.Clear();
        outputDataLowered.Clear();

        //double toRemovePer = (100.0 - (double)percentageLimit) * 0.01;
        //int toRemoveNum = (int)(toRemovePer * (double)getNumTotalData());

        //var random = new Random(Guid.NewGuid().GetHashCode());
        //int randIdx = random.Next(outputTestData.Count);
        int numOfBatch = 4;
        for (int i = 0; i < inputData.Count; i++)
        {
            List<double> columnLowered = new List<double>();
            for (int j = 0; j < numOfBatch; j++)
            {
                columnLowered.Add(inputData[i][j]);
            }
            inputDataLowered.Add(columnLowered);
        }
        for (int j = 0; j < numOfBatch; j++)
        {
            outputDataLowered.Add(outputData[j]);
        }
#endif
    }

    public void TestData(double percentageLimit)
    {
        inputTestData.Clear();
        for (int i = 0; i < inputData.Count; i++)
        {
            List<double> columnTest = new List<double>();
            for (int j = 0; j < inputData[i].Count; j++)
            {
                columnTest.Add(inputData[i][j]);
            }
            inputTestData.Add(columnTest);
        }
        outputTestData.Clear();
        outputTestData.AddRange(outputData);

        double toRemovePer = (100.0 - percentageLimit) * 0.01;
        int toRemoveNum = (int)(toRemovePer * (double)getNumTotalData());

        var random = new Random(Guid.NewGuid().GetHashCode());

        for (int j = 0; j < toRemoveNum; j++)
        {
            // Random Index
            int randIdx = random.Next(outputTestData.Count);
            // For all columns
            for (int i = 0; i < inputTestData.Count(); i++)
            {
                if (inputTestData[i].Count < randIdx)
                {
                    randIdx = inputTestData[i].Count - 1;
                }
                inputTestData[i].RemoveAt(randIdx);
            }
            outputTestData.RemoveAt(randIdx);
        }
    }
}
