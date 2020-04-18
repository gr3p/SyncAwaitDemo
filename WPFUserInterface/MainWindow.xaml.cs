using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace WPFUserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public HttpClient HttpClient { get; set; } = new HttpClient();
        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindowBlack.Text += $"\n Total execution time: { elapsedMs }";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindowBlack.Text += $"\n Total execution time: { elapsedMs }";
        }


        private List<string> PrepData()
        {
            List<string> output = new List<string>();

            resultsWindowBlack.Text = "";

            output.Add("https://www.dn.se");
            output.Add("https://www.idg.se");
            output.Add("https://www.ikea.se");
            output.Add("https://www.cnn.com");
            output.Add("https://www.bbc.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }

        private void ClearControls()
        {
            resultsWindowBlack.Text = string.Empty;
            resultsWindowGreen.Text = string.Empty;
        }
        /// <summary>
        /// Async but not with async threads.
        /// </summary>
        /// <returns></returns>
        private async Task RunDownloadAsync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = await Task.Run(() => DownloadWebsite(site));
                ReportWebsiteInfo(results);
            }
        }
        /// <summary>
        /// True Async
        /// </summary>
        /// <returns></returns>
        private async Task RunDownloadParallelAsync()
        {
            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                tasks.Add(DownloadWebsiteAsync(site));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var item in results)
            {
                ReportWebsiteInfo(item);
            }
        }

        private void RunDownloadSync()
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
                ReportWebsiteInfo(results);
            }
        }

        private WebsiteDataModel DownloadWebsite(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            var result = client.DownloadString(websiteUrl);
            output.WebsiteData = result;
            output.getResults = !string.IsNullOrEmpty(result);
            return output;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();

            output.WebsiteUrl = websiteUrl;
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var result = await HttpClient.GetStringAsync(websiteUrl);

            output.getResults = !string.IsNullOrEmpty(result);

            output.WebsiteData = result;
            output.ThreadElapsedTime = $"{stopwatch.ElapsedMilliseconds}";
            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {

            resultsWindowBlack.Text += $"{ data.WebsiteUrl } (DL: { data.WebsiteData.Length } bytes). Time: {data.ThreadElapsedTime} ms. { Environment.NewLine }";
            resultsWindowGreen.Text += $"\t {(data.getResults ? "OK!" : "FAILED!")} { Environment.NewLine }";

        }
    }
}
