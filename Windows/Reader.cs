using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualBasic.FileIO;


public class Reader
{
    protected String inputFile;
    protected List<List<double>> inputData;
    protected List<String[]> inputString;
    
    public Reader()
	{
        inputFile = null;
	}

    public Reader(String dataFile)
    {
        inputFile = dataFile;   
    }

    public void Convert()
    {
        TextFieldParser csvParser = new TextFieldParser(inputFile);
        
        csvParser.CommentTokens = new string[] { "#" };
        csvParser.SetDelimiters(new string[] { "," });
        csvParser.HasFieldsEnclosedInQuotes = true;

        // Skip the row with the column names
        csvParser.ReadLine();

        while (!csvParser.EndOfData)
        {
            // Read current line fields, pointer moves to the next line.
            inputString.Add(csvParser.ReadFields());
        }
    }
}
