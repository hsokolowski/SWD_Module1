using System;
using System.Collections.Generic;
using System.IO;

namespace Module1_SWD
{
    public class ParserToCSV
    {
        public Dictionary<string, List<object>> dictionary; 
        public ParserToCSV (Dictionary<string, List<object>> dic)
        {
            dictionary = dic;
        }

        public  string Parse(string x, string y, string z)
        {
            string result = "";
            string delimiter = ";";
            string newLine = "\n";
            string header = "\""+x+"\"" + delimiter + "\""+ y+"\"" + delimiter + "\"" +z+ "\"" + newLine;

            result += header;

            int count = dictionary[x].Count;

            for (int i = 0; i < count; i++)
            {
                string line = "";
                if (dictionary[x][i] is string)
                {
                    line += "\"" + dictionary[x][i]+ "\""+ delimiter;


                }
                else
                {
                    line += dictionary[x][i] + delimiter;

                }
                if (dictionary[y][i] is string)
                {
                    line += "\"" + dictionary[y][i] + "\"" + delimiter;


                }
                else
                {
                    line += dictionary[y][i] + delimiter;

                }
                if (dictionary[z][i] is string)
                {
                    line += "\"" + dictionary[z][i] + "\"" + delimiter;


                }
                else
                {
                    line += dictionary[z][i] + delimiter;

                }

                line += newLine;
                result += line;
            }

            File.WriteAllText(@"C:\\Users\\"+Environment.UserName+"\\source\\repos\\Module1_SWD\\Module1_SWD\\Assets\\dataFromFile.csv", result);

            return "C:/Users/" + Environment.UserName + "/source/repos/Module1_SWD/Module1_SWD/Assets/dataFromFile.csv";
        }
    }
}