using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace langTest
{
    class Program
    {             
        static Dictionary<String, int> d = new Dictionary<string, int>();
        static void Main(string[] args)
        {
            LangClassifier.InitDictionary();     
            test("train.txt","test.txt");
        }    
        static void test(string filename, string testfilename)
        {
            string[] lines = File.ReadAllLines(filename);
            List<LangClassifier.Data> forTrain = new List<LangClassifier.Data>();          
            foreach (string s in lines)
            {
                string[] split = s.Split('\t');
                forTrain.Add(new LangClassifier.Data(LangClassifier.getLabel(split[0]), split[1]));           
            }
            LangClassifier classifier = new LangClassifier();
            classifier.teach(forTrain.ToArray());
            Console.WriteLine(classifier.accuracy);
            lines = File.ReadAllLines(testfilename);
            StringBuilder ans = new StringBuilder();
            foreach(string s in lines)
            {
                ans.AppendLine(String.Format("{0}", LangClassifier.getLabel(classifier.classify(s))));
            }
            File.WriteAllText("output.txt", ans.ToString());
        }

    }
}
