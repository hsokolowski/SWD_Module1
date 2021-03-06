﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Module1_SWD
{
    public partial class Form1 : Form
    {
        private static Dictionary<string, List<object>> orginalAtributesToRecord =
            new Dictionary<string, List<object>>();

        public Form1()
        {
            InitializeComponent();
            numericUpDown1.Maximum = 100;
            numericUpDown1.Minimum = 1;
            
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

            SetSerializeDataGrid(orginalAtributesToRecord);
            PrintMatrix(orginalAtributesToRecord);
        }

        private void PrintMatrix(Dictionary<string, List<object>> dictionary)
        {
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Refresh();

            var keyCount = dictionary.Keys.Count;
            dataGridView1.ColumnCount = keyCount;
            dataGridView1.ColumnHeadersVisible = true;

            List<List<object>> tmpList = new List<List<object>>();
            var counter = 0;
            var max = 0;
            foreach (var VARIABLE in dictionary)
            {
                dataGridView1.Columns[counter].SortMode = DataGridViewColumnSortMode.NotSortable;
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

        private void SetSerializeDataGrid(Dictionary<string, List<object>> dictionary)
        {
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            dataGridView2.Dock = DockStyle.Fill;
            foreach (var VARIABLE in dictionary)
            {
                dataGridView2.Rows.Add(VARIABLE.Key);
                domainUpDown1.Items.Add(VARIABLE.Key);
                domainUpDown2.Items.Add(VARIABLE.Key);
                domainUpDown3.Items.Add(VARIABLE.Key);
                domainUpDown4.Items.Add(VARIABLE.Key);
                domainUpDown5.Items.Add(VARIABLE.Key);
            }

            domainUpDown1.SelectedIndex = 0;
        }

        private void SaveDataToDictionary(Stream myStream)
        {
            orginalAtributesToRecord = new Dictionary<string, List<object>>();
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
                        if (!checkBox1.Checked)
                        {
                            splitHeaders = CreateCustomHeaders(line, ref headers);
                            DataFileUtils.SplitRecordsLine(line, orginalAtributesToRecord, splitHeaders);
                        }
                        else
                        {
                            headers = InitialHeaders(line, orginalAtributesToRecord, out splitHeaders);
                        }
                    }
                    else
                    {
                        DataFileUtils.SplitRecordsLine(line, orginalAtributesToRecord, splitHeaders);
                    }

                }
            }
        }

        private static string[] CreateCustomHeaders(string line, ref string headers)
        {
            string[] splitHeaders;
            string tmp = line.Replace(';', ' ');
            string[] splitFirstRow = tmp.Split(null);
            splitHeaders = new string[splitFirstRow.Length];
            for (var i = 0; i < splitFirstRow.Length; i++)
            {
                string customAttributeName = "Attribute_" + i;
                headers += customAttributeName + " ";
                orginalAtributesToRecord.Add(customAttributeName, new List<object>());
                splitHeaders[i] = customAttributeName;
            }

            return splitHeaders;
        }


        private static string InitialHeaders(string line, Dictionary<string, List<object>> orginalAtributesToRecord,
            out string[] splitHeaders)
        {
            string headers = line;
            splitHeaders = headers.Split(null);
            for (int i = 0; i < splitHeaders.Length; i++)
            {
                orginalAtributesToRecord.Add(splitHeaders[i], new List<object>());
            }

            return headers;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

            for (var i = 0; i < dataGridView2.Rows.Count; i++)
            {
                var dataGridViewRow = dataGridView2.Rows[i];
                var columns = dataGridViewRow.Cells;


                // Kolumna 0 to nazwa atrybutu
                var attributeName = (string) dataGridViewRow.Cells[0].Value;
                // Kolumna 1 i 2 to box czy chcemy wykonac deskretyzacje oraz wartosc range 
                if (columns[1].Value != null && columns[2].Value != null)
                {
                    bool isBoxChecked = (bool) dataGridViewRow.Cells[1].Value;
                    if (isBoxChecked)
                    {
                        List<decimal> discretization = MathUtils.Discretization(
                            orginalAtributesToRecord[attributeName].Cast<decimal>().ToList(),
                            Int32.Parse((string) dataGridViewRow.Cells[2].Value));
                        orginalAtributesToRecord.Remove(attributeName);
                        orginalAtributesToRecord.Add(attributeName + "_D", discretization.Cast<Object>().ToList());
                    }
                }
                // Kolumna 3 to box czy chcemy zamienic wartosci tekstowe na numeryczne 
                else if (columns[3].Value != null)
                {
                    bool isBoxChecked = (bool) dataGridViewRow.Cells[3].Value;
                    bool alphabeticalOrder = radioButton1.Checked;
                    if (isBoxChecked)
                    {
                        KeyValuePair<string, List<object>> changeValuesToNumeric =
                            RecordUtils.ChangeValuesToNumeric(attributeName, orginalAtributesToRecord[attributeName],
                                alphabeticalOrder);
                        orginalAtributesToRecord.Remove(attributeName);
                        orginalAtributesToRecord.Add(attributeName, changeValuesToNumeric.Value);
                    }
                }
                else if (columns[4].Value != null)
                {
                    bool isBoxChecked = (bool) dataGridViewRow.Cells[4].Value;
                    if (isBoxChecked)
                    {
                        var normalization =
                            MathUtils.Normalization(orginalAtributesToRecord[attributeName].Cast<decimal>().ToList());
                        orginalAtributesToRecord.Remove(attributeName);
                        orginalAtributesToRecord.Add(attributeName + "_N", normalization.Cast<object>().ToList());
                    }
                }
                else if (columns[6].Value != null && columns[5].Value != null)
                {
                    decimal newValueMin = Decimal.Parse(columns[5].Value.ToString(),
                            NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                        newValueMax = Decimal.Parse(columns[6].Value.ToString(),
                            NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

                    var tmp = new List<object>();
                    orginalAtributesToRecord.TryGetValue(attributeName, out tmp);
                    var tmp2 = tmp.Cast<Decimal>().ToList();

                    for (int j = 0; j < tmp2.Count; j++)
                    {
                        if (tmp2[j] < newValueMin)
                        {
                            tmp2[j] = newValueMin;
                        }

                        if (tmp2[j] > newValueMax)
                        {
                            tmp2[j] = newValueMax;
                        }
                    }

                    orginalAtributesToRecord[attributeName] = tmp2.Cast<object>().ToList();
                }

            }

            PrintMatrix(orginalAtributesToRecord);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var attribute = domainUpDown1.SelectedItem.ToString();
            var percentage = numericUpDown1.Value;
            var countOfSelected = orginalAtributesToRecord[attribute].Count * percentage / 100;

            dataGridView3.DataSource = null;
            dataGridView3.Rows.Clear();
            dataGridView3.Columns.Clear();
            dataGridView3.Refresh();

            var keyCount = orginalAtributesToRecord.Keys.Count;
            dataGridView3.ColumnCount = keyCount;
            dataGridView3.ColumnHeadersVisible = true;

            List<List<object>> tmpList = new List<List<object>>();
            var counter = 0;
            var max = 0;
            foreach (var VARIABLE in orginalAtributesToRecord)
            {
                //dataGridView3.Columns[counter].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView3.Columns[counter++].Name = VARIABLE.Key;
                tmpList.Add(VARIABLE.Value);
            }

            for (int i = 0; i < orginalAtributesToRecord[attribute].Count; i++)
            {
                List<object> row = new List<object>();
                for (int j = 0; j < tmpList.Count; j++)
                {
                    row.Add(tmpList.ElementAt(j)[i]);
                }

                dataGridView3.Rows.Add(row.ToArray());
            }

            dataGridView3.Sort(dataGridView3.Columns[domainUpDown1.SelectedIndex],
                radioButton3.Checked ? ListSortDirection.Ascending : ListSortDirection.Descending);

            for (int i = 0; i < countOfSelected; i++)
            {
                dataGridView3.Rows[i].DefaultCellStyle.BackColor = Color.Red;
            }

            //dataGridView3.DataSource = (BindingList)orginalAtributesToRecord.Take((int)countOfSelected);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = "C:\\Users\\"+Environment.UserName+"\\PycharmProjects\\SWD_module1_python\\dist\\Main\\Main.exe";

            ParserToCSV parser = new ParserToCSV(orginalAtributesToRecord);
            string path = parser.Parse(domainUpDown6.SelectedItem.ToString(), domainUpDown7.SelectedItem.ToString(), domainUpDown8.SelectedItem.ToString());

            start.Arguments = string.Format("{0} {1} {2} {3} {4} {5}", "path="+ path, "x-attr=" + domainUpDown6.SelectedItem.ToString(), "y-attr="+domainUpDown7.SelectedItem.ToString(), "z-attr=" + domainUpDown8.SelectedItem.ToString(), "color=" + domainUpDown9.SelectedItem.ToString(), "C:\\Users\\"+Environment.UserName+"\\Desktop");
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            using (Process process = Process.Start(start))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    // this prints 11
                    Console.Write(result);
                    webBrowser1.Url = new Uri(String.Format("file:///C:/Users/"+Environment.UserName+"/Desktop/3dChart.html"));


                }
            }
            Console.Read();
            //webBrowser1.ScriptErrorsSuppressed = true;
            // string fileName = @"C:\Users\Hubert\PycharmProjects\ED_1\WersjaFinalna.py";
            //
            // Process p = new Process();
            // p.StartInfo = new ProcessStartInfo(@"C:\Python27\python.exe", fileName)
            // {
            //     RedirectStandardOutput = true,
            //     UseShellExecute = false,
            //     CreateNoWindow = true
            // };
            // p.Start();
            //
            // string output = p.StandardOutput.ReadToEnd();
            // p.WaitForExit();
        }


        private void button4_Click(object sender, EventArgs e)
        {
            var value1 = domainUpDown2.SelectedItem;
            var value2 = domainUpDown3.SelectedItem;
            var classDec = domainUpDown4.SelectedItem;

            var data1 = orginalAtributesToRecord[value1.ToString()];
            var data2 = orginalAtributesToRecord[value2.ToString()];

            var decisionClasses = orginalAtributesToRecord[classDec.ToString()].Distinct();
            var allDecisionClasses = orginalAtributesToRecord[classDec.ToString()];

            chart1.ChartAreas[0].AxisX.Title = value1.ToString();
            chart1.ChartAreas[0].AxisY.Title = value2.ToString();
            chart1.Series.Clear();
            for (int i = 0; i < decisionClasses.Count(); i++)
            {
                chart1.Series.Add(decisionClasses.ElementAt(i).ToString());
                chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
                chart1.Series[i].LegendText = decisionClasses.ElementAt(i).ToString();
                chart1.Series[i].Points.Clear();
            }
            
            for (int i = 0; i < data1.Count; i++)
            {
                chart1.Series[allDecisionClasses.ElementAt(i).ToString()].Points.AddXY(data1[i], data2[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {

            var attribute = domainUpDown5.SelectedItem;
            var data = orginalAtributesToRecord[attribute.ToString()]; 
            chart2.Series[0].Points.Clear();

            var distinctDataString = data.Distinct();
            Dictionary<string,int> dic = new Dictionary<string, int>();
            for (int i = 0; i < distinctDataString.Count(); i++)
            {
                dic.Add(distinctDataString.ElementAt(i).ToString(),0);
            }

            for (int i = 0; i < data.Count; i++)
            {
                dic[data[i].ToString()]++;
            }

            chart2.Series[0].LegendText = attribute.ToString();
            foreach (var key in dic.Keys)
            {
                chart2.Series[0].Points.AddXY(key, dic[key]);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // tutaj trzeba dodać czyśzczenie domainupów i wstawianie nowych warotści np attribute_D
            foreach (var VARIABLE in orginalAtributesToRecord)
            {
                domainUpDown5.Items.Add(VARIABLE.Key);
                domainUpDown6.Items.Add(VARIABLE.Key);
                domainUpDown7.Items.Add(VARIABLE.Key);
                domainUpDown8.Items.Add(VARIABLE.Key);
                domainUpDown9.Items.Add(VARIABLE.Key);
            }
        }
    }
}
