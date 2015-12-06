﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace langTest
{
    public class LangClassifier
    {

        private static Dictionary<string, int> labelMap;
        private static Dictionary<int, string> revMap;           
        private double accuracy;
        public Double Accuracy
        {
            get { return accuracy; }
        }
        Dictionary<String, int>[] classes;
        Dictionary<String, int> wordsDic;
        int[] classStat;
        int totalStat;
        public LangClassifier()
        {
            accuracy = 0;
            classes = new Dictionary<string, int>[revMap.Count];
            wordsDic = new Dictionary<string, int>();
            for (int i = 0; i < revMap.Count; i++)
            {
                classes[i] = new Dictionary<string, int>();
            }
            classStat = new int[revMap.Count];
        }
        #region Technical Data
        public struct Data
        {
            public String text;
            public int label;
            public Data(int label, String word) { this.label = label; this.text = word; }
        }
        private class ComparablePair<TKey, TVal> : IComparable
        {
            public TKey id;
            public TVal val;
            public ComparablePair(TKey a, TVal b) { id = a; val = b; }
            public int CompareTo(object obj)
            {
                if (obj == null)
                    return 1;
                ComparablePair<TKey, TVal> p = obj as ComparablePair<TKey, TVal>;
                if (p != null)
                    return (val as IComparable).CompareTo(p.val);
                else
                    throw new ArgumentException("Obj is not ComparablePair of nessesary types!");
            }
        }
        private static Regex special = new Regex(@"[-+,.:/\'«»" + '"' + "]");
        private static Regex numbers = new Regex(@"[0-9]");
        static public int getLabel(string l)
        {
            return labelMap[l];
        }

        static public String getLabel(int i)
        {
            return revMap[i];
        }
        String[] splitTextToWords(String text)
        {
            return special.Replace(text, "").ToUpper().Split(' ').Where(e => !numbers.IsMatch(e)).ToArray();
        }
        public static void InitDictionary()
        {
            labelMap = new Dictionary<string, int>();
            revMap = new Dictionary<int, string>();
            labelMap.Add("SV", 0);
            labelMap.Add("ES", 1);
            labelMap.Add("EN", 2);
            labelMap.Add("HU", 3);
            labelMap.Add("DA", 4);
            labelMap.Add("DE", 5);
            labelMap.Add("IT", 6);
            labelMap.Add("NL", 7);
            labelMap.Add("PL", 8);
            labelMap.Add("LV", 9);
            labelMap.Add("EE", 10);
            labelMap.Add("FR", 11);
            labelMap.Add("CS", 12);
            labelMap.Add("FI", 13);
            labelMap.Add("LT", 14);

            revMap.Add(0, "SV");
            revMap.Add(1, "ES");
            revMap.Add(2, "EN");
            revMap.Add(3, "HU");
            revMap.Add(4, "DA");
            revMap.Add(5, "DE");
            revMap.Add(6, "IT");
            revMap.Add(7, "NL");
            revMap.Add(8, "PL");
            revMap.Add(9, "LV");
            revMap.Add(10, "EE");
            revMap.Add(11, "FR");
            revMap.Add(12, "CS");
            revMap.Add(13, "FI");
            revMap.Add(14, "LT");

        }
        #endregion
        public void teach(Data[] data)
        {
            int n = data.Length;
            for (int i = 0; i < n; i++)
            {
                string[] words = splitTextToWords(data[i].text);
                foreach (string s in words)
                {
                    if (!classes[data[i].label].ContainsKey(s))
                        classes[data[i].label].Add(s, 0);
                    if (!wordsDic.ContainsKey(s))
                        wordsDic.Add(s, 0);
                    classes[data[i].label][s]++;
                    wordsDic[s]++;
                    classStat[data[i].label]++;
                    totalStat++;
                }
            }
            int testSuccess = 0;
            for (int i = 0; i < n; i++)
            {
                if (this.classify(data[i].text) == data[i].label)
                    testSuccess++;
            }
            accuracy = (double)testSuccess / (n);
        }
        public int classify(String text)
        {
            String[] words = splitTextToWords(text);
            
            return revMap.Keys.Min(c => new ComparablePair<int,double>(c, words.Sum(w => -Math.Log(classes[c].ContainsKey(w) ? (double)classes[c][w] / wordsDic[w] : 1e-7)) - Math.Log((double)classStat[c] / totalStat))).id;

        }
    }
}
