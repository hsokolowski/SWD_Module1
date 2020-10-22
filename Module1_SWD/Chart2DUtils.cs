using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Windows.Forms.DataVisualization.Charting;


namespace Module1_SWD
{
    public class Chart2DUtils
    {
        private static Form1 form1 = new Form1();
        private static Chart chart1 = ((Chart) form1.Controls.Find("chart1", true)[0]);
        public static void SetSeries<T>(int series, T data)
        {
            chart1.Series[series].Points.Clear();
            var counter = 0;
            foreach (var D in (IEnumerable) data)
            {
                chart1.Series[series].Points.AddXY((decimal)D,counter++ );
            }
            
        }
    }
}