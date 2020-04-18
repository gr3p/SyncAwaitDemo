using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            RunDownloadSync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindowBlack.Text += $"Total execution time: { elapsedMs }";
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();

            await RunDownloadParallelAsync();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            resultsWindowBlack.Text += $"Total execution time: { elapsedMs }";
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

        private WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(websiteURL);

            return output;
        }

        private async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteUrl)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteUrl;
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteUrl);

            return output;
        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
           
            resultsWindowBlack.Text += $"{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } length.{ Environment.NewLine }";
            resultsWindowGreen.Text += $"\t OK! { Environment.NewLine }";
           
        }
    }
}
