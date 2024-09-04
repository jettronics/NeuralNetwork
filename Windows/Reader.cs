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
    protected List<List<double>> inputData;
    protected List<double> outputData;
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
    public double getOutputData(int index) { return outputData[index]; }

    public Reader()
	{
        inputFile = null;
        inputData = new List<List<double>>(); 
        outputData = new List<double>();
        inputString = new List<String[]>();
        numOutputClassifiers = 0;
        numInputNodes = 0;
        classifiers = new List<String>();
    }

    public Reader(String dataFile)
    {
        inputFile = dataFile;
        inputData = new List<List<double>>();
        outputData = new List<double>();
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
    }

    public void LimitData( int percentageLimit )
    {
        double toRemovePer = (100.0 - (double)percentageLimit) * 0.01;
        int toRemoveNum = (int)(toRemovePer * (double)getNumTotalData());

        var random = new Random(Guid.NewGuid().GetHashCode());

        for (int j = 0; j < toRemoveNum; j++)
        {
            // Random Index
            int randIdx = random.Next(outputData.Count);
            // For all columns
            for (int i = 0; i < inputData.Count(); i++)
            {
                inputData[i].RemoveAt(randIdx);
            }
            outputData.RemoveAt(randIdx);
        }
    }
}
