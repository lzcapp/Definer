using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Forms;
using JiebaNet.Analyser;
using Newtonsoft.Json;

namespace Definer {
    public partial class Form1 : Form {
        private const string Host = "https://customsearch.googleapis.com/customsearch/v1?";
        private const int Num = 10;
        private const int Start = 0;
        private static string _query = "";
        private const string Cx = "d8ccc68be37a9d3cd";
        private const string Key = "";

        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private async void Button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                return;
            }
            _query = textBox1.Text;
            var result = "";
            var client = new HttpClient();
            var uri = Host + "cx=" + Cx + "&key=" + Key + "&num=" + Num + "&start=" + Start + "&q=" + System.Net.WebUtility.UrlEncode(_query);

            var response = await client.GetAsync(uri);

            var contentString = await response.Content.ReadAsStringAsync();
            dynamic parsedJson = JsonConvert.DeserializeObject(contentString);

            var items = parsedJson?.items;
            for (var i = Start; i < Num; i++)
            {
                result += items?[i].snippet.ToString();
            }

            var extractor = new TfidfExtractor();
            var pairs = extractor.ExtractTagsWithWeight(result, 30);

            var words = new List<string>();
            var freqs = new List<int>();
            foreach (var pair in pairs)
            {
                words.Add(pair.Word);
                freqs.Add(Convert.ToInt32(pair.Weight * Math.Pow(10, 6)));
            }

            var wc = new WordCloud.WordCloud(1920, 1080);
            var image = wc.Draw(words, freqs);
            pictureBox1.Image = image;
            button2.Enabled = true;
            button2.Visible = true;
        }

        private void Button2_Click(object sender, EventArgs e) {
            pictureBox1.Image.Save(_query + ".png");
        }
    }
}
