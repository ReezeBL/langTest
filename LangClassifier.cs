using System;
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
        private double accuracy = 0;
        public double Accuracy
        {
            get { return accuracy; }
        }
        LabelList labels = new LabelList();
        int totalStat = 0;
        #region Technical Data
        private class LabelList
        {
            private readonly HashSet<String> labels = new HashSet<string>();
            public readonly HashSet<String> words = new HashSet<string>();
            private Dictionary<String, Label> dictionary = new Dictionary<String, Label>();
            public HashSet<string> Labels { get { return labels; } }
            public Label getDictionary(String label)
            {
                if (!labels.Contains(label))
                {
                    labels.Add(label);
                    dictionary.Add(label, new Label());
                }
                return dictionary[label];
            }
        } 
        private class Label
        {
            private Dictionary<string, int> words = new Dictionary<string, int>();
            public int Count = 0;
            public int Words = 0;
            public int Get(string word)
            {
                if (!words.ContainsKey(word))
                    return 0;
                return words[word];
            }

            public void Add(string word)
            {
                if (!words.ContainsKey(word))
                    words.Add(word, 0);
                words[word]++;
            }
        }
        public struct Data
        {
            public string text;
            public string label;
            public Data(string label, string text) { this.label = label; this.text = text; }
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
                    throw new ArgumentException("Obj is not ComparablePair of nessesary type!");
            }
        }
        private static Regex special = new Regex(@"[-+,.:/\'«»" + '"' + "]");
        private static Regex numbers = new Regex(@"[0-9]");

        String[] splitTextToWords(String text)
        {
            return special.Replace(text, "").ToUpper().Split(' ').Where(e => !numbers.IsMatch(e) && e.Length > 0).ToArray();
        }
        
        #endregion
        public void teach(Data[] data)
        {
            int n = data.Length;
            int m =(int)(n * 0.75);
            for (int i = 0; i < m; i++)
            {
                Label label = labels.getDictionary(data[i].label);
                string[] words = splitTextToWords(data[i].text);
                foreach (string w in words)
                {
                    label.Add(w);
                    label.Words++;
                    labels.words.Add(w);
                }
                label.Count++;
                totalStat++;
            }

            int testSuccess = 0;
            for (int i = m; i < n; i++)
            {
                if (this.classify(data[i].text) == data[i].label)
                    testSuccess++;
            }
            accuracy = (double)testSuccess / (n-m);

            for (int i = m; i < n; i++)
            {
                Label label = labels.getDictionary(data[i].label);
                string[] words = splitTextToWords(data[i].text);
                foreach (string w in words)
                {
                    label.Add(w);
                    label.Words++;
                    labels.words.Add(w);
                }
                label.Count++;
                totalStat++;
            }
        }
        public string classify(String text)
        {
            string[] words = splitTextToWords(text);
            return labels.Labels.Min(c =>
            {
                Label l = labels.getDictionary(c);
                return new ComparablePair<string, double>(c, words.Sum(w => -Math.Log((double)(l.Get(w) + 1) / (labels.words.Count + l.Words))) - Math.Log((double)l.Count / totalStat));
            }).id;
        }
    }
}
