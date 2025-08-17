using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient {
    public partial class Form1 : Form {
        private readonly HttpClient _httpClient;
        private Stopwatch _stopwatch;

        public Form1() {
            InitializeComponent();

            _httpClient = new HttpClient {
                BaseAddress = new Uri("http://localhost:5213"), // Change to your API URL
                Timeout = TimeSpan.FromMinutes(5)
            };

            timer1.Interval = 1000; // 1 second
            timer1.Tick += Timer1_Tick;
        }

        private void Timer1_Tick(object sender, EventArgs e) {
            if (_stopwatch != null && _stopwatch.IsRunning) {
                lblTimer.Text = $"Elapsed: {_stopwatch.Elapsed.TotalSeconds:F1} sec";
            }
        }

        private async void btnUpload_Click(object sender, EventArgs e) {
            using (var ofd = new OpenFileDialog()) {
                ofd.Filter = "Documents|*.pdf;*.docx;*.txt";
                if (ofd.ShowDialog() == DialogResult.OK) {
                    await UploadFileAsync(ofd.FileName);
                }
            }
        }

        private async Task UploadFileAsync(string filePath) {
            try {
                _stopwatch = Stopwatch.StartNew();
                timer1.Start();
                lblTimer.Text = "Elapsed: 0 sec";
                txtResponse.Text = "Sending file...";

                using var form = new MultipartFormDataContent();
                using var stream = File.OpenRead(filePath);
                form.Add(new StreamContent(stream), "file", Path.GetFileName(filePath));

                var response = await _httpClient.PostAsync("/api/documentapi/analyze", form);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                _stopwatch.Stop();
                timer1.Stop();
                lblTimer.Text = $"Completed in {_stopwatch.Elapsed.TotalSeconds:F1} sec";

                txtResponse.Text = result;
            }
            catch (Exception ex) {
                _stopwatch?.Stop();
                timer1.Stop();
                txtResponse.Text = $"Error: {ex.Message}";
            }
        }
    }
}
