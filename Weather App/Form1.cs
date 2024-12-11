using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Guna.UI2.WinForms;

namespace Weather_App
{
    public partial class Form1 : Form
    {
        private const string ApiKey = "073bd01b8b193f3dfc7db66c5a7a8c94";
        private List<string> recentSearches = new List<string>(); 
        private const string filePath = "recent_searches.txt";
        public Form1()
        {
            InitializeComponent();
            LoadRecentSearches();
            //txtSearch.Click += txtSearch_KeyPress;
            PictureBox pbWeather = new PictureBox
            {
               
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(10, 10)
                
            };
            Label lblTemp = new Label
            {
              
                ForeColor = Color.White,
                BackColor = Color.Transparent, // Ensure the label background is transparent
                AutoSize = true,
                Location = new Point(50, 50) // Position relative to the PictureBox
            };

            Label lblWeather = new Label
            {
               
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(150, 100) 
            };
            lblTemp.Parent = pbWeather;
            lblWeather.Parent = pbWeather;
            pbWeather.Controls.Add(lblTemp);
            this.Controls.Add(pbWeather);
        }
        private void LoadRecentSearches()
        {
            if (File.Exists(filePath))
            {
                recentSearches = new List<string>(File.ReadAllLines(filePath));
            }
        }
        private void SaveRecentSearches()
        {
          

               
                File.WriteAllLines(filePath, recentSearches);
                
            
        }
      
        private void UpdateWeatherImage(string weatherCondition)
    {
        // Set the appropriate image based on the weather condition
        switch (weatherCondition.ToLower())
        {
            case "clear":
                pbWeather.Image = Properties.Resources.Summer; // Example: sunny image
                break;
            case "rain":
                    pbWeather.Image = Properties.Resources.Rainy; // Example: rainy image
                break;
            case "clouds":
                    pbWeather.Image = Properties.Resources.Cloud; // Example: cloudy image
                break;
            case "snow":
                    pbWeather.Image = Properties.Resources.snow; // Example: snowy image
                break;
            default:
                    pbWeather.Image = Properties.Resources.temp; // Default image
                break;
        }
    }
   
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }



      

 
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            txtSearch.Text = "Enter city...";
            txtSearch.ForeColor = Color.Gray;

            txtSearch.GotFocus += RemovePlaceholder;
            txtSearch.LostFocus += SetPlaceholder;
            DisplayRecentSearchButtons();
        }
        private void DisplayRecentSearchButtons()
        {

            panel2.Controls.Clear();

            // Loop through the recentSearches and display them
            for (int i = recentSearches.Count - 1; i >= 0; i--)
            {
             
            string search = recentSearches[i];

                // Create a new Guna2Chip for each search term
                Guna2Chip searchChip = new Guna2Chip
                {
                    Text = search,
                    Width = 150,
                    Height = 25,
                    Margin = new Padding(5),
                    Tag = search // Store the search term in the Tag property
                };

                // Add click event to perform a search when the chip is clicked
                searchChip.Click += (s, e) =>
                {
                    txtSearch.Text = (string)((Guna2Chip)s).Tag; // Set the chip's text in the search box
                    SearchWeather((string)((Guna2Chip)s).Tag);  // Perform the search
                };

                // Add the chip to the panel
                panel2.Controls.Add(searchChip);
            }


        }
        private void RemovePlaceholder(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2TextBox textBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            if (textBox.Text == "Enter city...")
            {
                textBox.Text = "";
                textBox.ForeColor = Color.Black; // Default color
            }
        }

        private void SetPlaceholder(object sender, EventArgs e)
        {
            Guna.UI2.WinForms.Guna2TextBox textBox = (Guna.UI2.WinForms.Guna2TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Enter city...";
                textBox.ForeColor = Color.Gray;
            }
        }

        private async void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                string city = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(city))
                {
                    MessageBox.Show("Please Enter a city name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                SearchWeather(city);
            }
        }

        private async void SearchWeather(string city)
        {
            string ApiUrl = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}&units=metric";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        JObject weatherData = JObject.Parse(responseBody);

                        double temperature = weatherData["main"]["temp"].ToObject<double>();
                        string description = weatherData["weather"][0]["description"].ToString();
                        string weatherCondition = (string)weatherData["weather"][0]["main"];
                        lblTemp.Text = $"{temperature} °C";
                        lblWeather.Text = description;
                        UpdateWeatherImage(weatherCondition);
                        if (recentSearches.Contains(city))
                        {
                            recentSearches.Remove(city);
                        }
                        recentSearches.Insert(0, city);

                        if (recentSearches.Count > 3)
                        {
                            recentSearches.RemoveAt(recentSearches.Count - 1);
                        }
                        DisplayRecentSearchButtons();
                    }
                    else
                    {
                        lblWeather.Text = "Invalid city";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lblTemp_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveRecentSearches();
        }

        private void pbWeather_Click(object sender, EventArgs e)
        {

        }

        private void pbWeather_Click_1(object sender, EventArgs e)
        {

        }

        private void guna2Chip1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
