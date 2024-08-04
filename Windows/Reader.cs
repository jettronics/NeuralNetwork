using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.FileIO;


public class Reader
{
    protected String inputFile;
    protected List<List<double>> inputData;
    protected List<String[]> inputString;
    protected int numOutputClassifiers;
    protected int numInputNodes;
    List<String> classifiers;

    public int getNumClassifiers() { return numOutputClassifiers; }
    public int getNumInputNodes() { return numInputNodes; }
    public String[] getColumnNames() { return inputString.ElementAt(0); }
    public String[] getClassifiers() { return classifiers.ToArray(); }
    
    public Reader()
	{
        inputFile = null;
        inputData = new List<List<double>>();   
        inputString = new List<String[]>();
        numOutputClassifiers = 0;
        numInputNodes = 0;
        classifiers = new List<String>();
    }

    public Reader(String dataFile)
    {
        inputFile = dataFile;
        inputData = new List<List<double>>();
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
        classifiers.Clear();

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

        String classifier = inputString.ElementAt(1).Last();
        double number = 0;
        bool canConvert = double.TryParse(classifier, out number);
        if (canConvert == true)
        {
            numOutputClassifiers = 1;
        }
        else
        {
            for (int i = 1; i < inputString.Count; i++)
            {
                classifier = inputString.ElementAt(i).Last();
                bool avail = false;
                for (int j = 0; j < classifiers.Count; j++)
                {
                    if (classifier == classifiers.ElementAt(j))
                    {
                        avail = true;
                        break;
                    }
                }
                if (avail == false)
                {
                    classifiers.Add(classifier);
                }
            }
            numOutputClassifiers = classifiers.Count;
        }
    }
}
