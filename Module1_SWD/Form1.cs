using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Module1_SWD
{
    public partial class Form1 : Form
    {
        private static Dictionary<string, List<object>> orginalAtributesToRecord = new Dictionary<string, List<object>>();
        public Form1()
        {
            InitializeComponent();
            //chart1.Series["s1"].Points.AddXY(0, 8, 21, 12,19,13,14); //avg/ mediana
        }

        private void openFileBtn_Click(object sender, EventArgs e)
        {
            Stream myStream;
            OpenFileDialog theDialog = new OpenFileDialog
            {
                Title = @"Open Text File",
                Filter = @"txt files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = @"C:\",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = theDialog.OpenFile()) != null)
                    {
                        pathTb.Text = theDialog.FileName;
                        using (myStream)
                        {
                            SaveDataToDictionary(myStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

            PrintMatrix(orginalAtributesToRecord);
        }

        private void PrintMatrix(Dictionary<string, List<object>> dictionary)
        {
            dataGridView1.Dock = DockStyle.Fill;

            var keyCount = dictionary.Keys.Count;
            dataGridView1.ColumnCount = keyCount;
            dataGridView1.ColumnHeadersVisible = true;

            List<List<object>> tmpList = new List<List<object>>();
            var counter = 0;
            var max = 0;
            foreach (var VARIABLE in dictionary)
            {
                dataGridView1.Columns[counter++].Name = VARIABLE.Key;
                tmpList.Add(VARIABLE.Value);
                var tmpMax = VARIABLE.Value.Count;
                if (tmpMax > max)
                {
                    max = tmpMax;
                }
            }

            for (int i = 0; i < max; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < tmpList.Count; j++)
                {
                    row.Add(tmpList.ElementAt(j)[i]);
                }
                dataGridView1.Rows.Add(row.ToArray());
            }
        }

        public static DictionaryBindingList<TKey, TValue> ToBindingList<TKey, TValue>(IDictionary<TKey, TValue> data)
        {
            return new DictionaryBindingList<TKey, TValue>(data);
        }

        public class DictionaryBindingList<TKey, TValue>
            : BindingList<Pair<TKey, TValue>>
        {
            private readonly IDictionary<TKey, TValue> data;
            public DictionaryBindingList(IDictionary<TKey, TValue> data)
            {
                this.data = data;
                Reset();
            }
            public void Reset()
            {
                bool oldRaise = RaiseListChangedEvents;
                RaiseListChangedEvents = false;
                try
                {
                    Clear();
                    foreach (TKey key in data.Keys)
                    {
                        Add(new Pair<TKey, TValue>(key, data));
                    }
                }
                finally
                {
                    RaiseListChangedEvents = oldRaise;
                    ResetBindings();
                }
            }
        }
        public sealed class Pair<TKey, TValue>
        {
            private readonly TKey key;
            private readonly IDictionary<TKey, TValue> data;
            public Pair(TKey key, IDictionary<TKey, TValue> data)
            {
                this.key = key;
                this.data = data;
            }
            public TKey Key { get { return key; } }
            public TValue Value
            {
                get
                {
                    TValue value;
                    data.TryGetValue(key, out value);
                    return value;
                }
                set { data[key] = value; }
            }
        }

        private static void SaveDataToDictionary(Stream myStream)
        {
            string headers = null;
            string[] splitHeaders = null;

            StreamReader reader = new StreamReader(myStream);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith("#") && line != "")
                {
                    if (headers == null)
                    {
                        headers = InitialHeaders(line, orginalAtributesToRecord, out splitHeaders);
                        continue;
                    }
                    SplitRecordsLine(line, orginalAtributesToRecord, splitHeaders);
                }
            }
        }

        private static void SplitRecordsLine(string line, Dictionary<string, List<object>> orginalAtributesToRecord, string[] splitHeaders)
        {
            string[] split = line.Split(null);
            for (int i = 0; i < split.Length; i++)
            {
                List<object> tmpRecords;
                orginalAtributesToRecord.TryGetValue(splitHeaders[i], out tmpRecords);
                tmpRecords.Add(split[i]);
            }
        }

        private static string InitialHeaders(string line, Dictionary<string, List<object>> orginalAtributesToRecord, out string[] splitHeaders)
        {
            string headers = line;
            splitHeaders = headers.Split(null);
            for (int i = 0; i < splitHeaders.Length; i++)
            {
                orginalAtributesToRecord.Add(splitHeaders[i], new List<object>());
            }
            return headers;
        }
    }
}
