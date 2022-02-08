using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net;
using System.Text.Json;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace WindowsFormsApp2
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private readonly string api_key = "DIK4IAKE5SWJT721";
        public string[] time = new string[1000];
        public double[] open = new double[1000];
        public string[] company_list = new string[20];
        public dynamic json_data;
        public int no_com = 0;

        public void search_symbol(string sym_name)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?function=SYMBOL_SEARCH&keywords={sym_name}&apikey={this.api_key}");
            HttpWebResponse rep = (HttpWebResponse)req.GetResponse();
            json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(rep.GetResponseStream());

            for (int i = 0; i < json_data["bestMatches"].GetArrayLength(); i++)
            {
                company_list[i] = Convert.ToString((i+1) + ". " +  JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json_data["bestMatches"][i])["1. symbol"] + " (" + JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json_data["bestMatches"][i])["2. name"] + ")");
            }

            update_label(company_list);
        }

        public void update_label(string[] label_output)
        {
            for (int i = 0; i < label_output.Length; ++i)
            {
                label2.Text += label_output[i] + Environment.NewLine;
            }
        }

        public void getData_company(int c_name)
        {
            string sel = Convert.ToString(JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json_data["bestMatches"][c_name - 1])["1. symbol"]);
            use_data(sel) ;
        }

        public void use_data(string selected_company)
        {
            HttpWebRequest reqSel = (HttpWebRequest)WebRequest.Create("https://" + $@"www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={selected_company}&interval=5min&apikey={this.api_key}");
            HttpWebResponse repSel = (HttpWebResponse)reqSel.GetResponse();
            dynamic json_selected = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(repSel.GetResponseStream());
            dynamic t = json_selected["Time Series (5min)"];
            var x = t.EnumerateObject();

            
            while (x.MoveNext())
            {
                this.time[no_com] = x.Current.ToString().Substring(1, 19);
                this.open[no_com] = Convert.ToDouble(Convert.ToString((t.GetProperty(x.Current.ToString().Substring(1, 19)).GetProperty("1. open"))));
                ++no_com;
            }

            draw_graph(time, open,selected_company);
        }


        public void draw_graph(string[] time, double[] open, string company)
        {
            chart1.Titles.Add("Stock prices of " + company);
            var chart = chart1.ChartAreas[0];
            chart.AxisY.Minimum = open[0] - 2;
            chart1.Series[0].ChartType = SeriesChartType.Spline;
            for(int i = 0; i < no_com; ++i)
            {
                chart1.Series["Open Prices"].Points.AddXY(time[i].Substring(11), open[i]);
            }
            chart1.Series[0].ToolTip = "#VALY";

        }


        private void button1_Click(object sender, EventArgs e)
        {
            search_symbol(textBox1.Text);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            getData_company(Convert.ToInt32(textBox2.Text));
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
