using System.IO.Compression;
using System.Drawing;
using System.Globalization;

namespace FO_PROJECT_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
            textBox1.Visible = false;
            monthCalendar1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
        }
        int currentX = 10;
        FileStream file;
        StreamReader reader;
        StreamWriter writer;
        string filename;


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.Cancel) { return; }
            filename = ofd.FileName;
            if (filename == null) { MessageBox.Show("invalid file"); }
            else
            {
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (ZipArchive archive = new ZipArchive(file, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (!entry.FullName.EndsWith("/")) // Directory entries end with "/"
                            {
                                if (IsImageFile(entry.FullName))
                                {
                                    using (Stream zipStream = entry.Open())
                                    {
                                        Image img = Image.FromStream(zipStream);

                                        PictureBox pictureBox = new PictureBox();
                                        pictureBox.Image = img;
                                        pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                                        pictureBox.Location = new Point(currentX, 30); // Adjust position as needed
                                        Controls.Add(pictureBox);

                                        currentX += pictureBox.Width + 10;
                                    }
                                }
                            }
                        }
                    }
                    button1.Visible = false;
                    checkedListBox1.Visible = true;
                    button2.Visible = true;

                }
            }
        }

        //

        private bool IsImageFile(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png";
        }

        //

        //
        private void button2_Click(object sender, EventArgs e)
        {
            button3.Visible = true;
            foreach (object itemChecked in checkedListBox1.CheckedItems)
            {
                string check = itemChecked.ToString();
                if (check == "date")
                {
                    monthCalendar1.Visible = true;

                }
                else if (check == "location")
                {
                    textBox1.Visible = true;
                }
            }
        }



        //
        private void button3_Click(object sender, EventArgs e)
        {  // Clear existing pictures
            foreach (Control control in Controls)
            {
                if (control is PictureBox)
                {
                    Controls.Remove(control);
                    control.Dispose();
                }
            }

            // Check if a date is selected in the MonthCalendar
            if (monthCalendar1.SelectionStart != null)
            {
                DateTime selectedDate = monthCalendar1.SelectionStart.Date;

                // Iterate through the images in the zip file
                using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    using (ZipArchive archive = new ZipArchive(file, ZipArchiveMode.Read))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            if (!entry.FullName.EndsWith("/")) // Directory entries end with "/"
                            {
                                if (IsImageFile(entry.FullName))
                                {
                                    using (Stream zipStream = entry.Open())
                                    {
                                        Image img = Image.FromStream(zipStream);

                                        // Get the creation date of the image
                                        DateTime creationDate = GetCreationDate(entry.FullName);

                                        // Compare the creation date with the selected date in the MonthCalendar
                                        if (selectedDate == creationDate.Date)
                                        {
                                            PictureBox pictureBox = new PictureBox();
                                            pictureBox.Image = img;
                                            pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                                            pictureBox.Location = new Point(currentX, 30); // Adjust position as needed
                                            Controls.Add(pictureBox);

                                            currentX += pictureBox.Width + 10;
                                        }
                                        if (selectedDate != creationDate.Date)
                                        {
                                            foreach (Control control in Controls)
                                            {
                                                if (control is PictureBox)
                                                {
                                                    Controls.Remove(control);
                                                    control.Dispose();
                                                }
                                            }
                                            MessageBox.Show("No pictures for this day");
                                            return;

                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        private DateTime GetCreationDate(string filename)
        {

            try
            {
                // Extract the date portion after the first hyphen
                string dateString = Path.GetFileNameWithoutExtension(filename)?.Split('-')[1]?.Trim();

                if (!string.IsNullOrEmpty(dateString))
                {
                    // Parse the date using the specified format
                    if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.CurrentCulture,
                        DateTimeStyles.None, out DateTime dateTaken))
                    {
                        return dateTaken;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions (e.g., invalid format, null filename, etc.)
                Console.WriteLine($"Error extracting creation date: {ex.Message}");
            }

            // Return a default value (e.g., DateTime.MinValue) if extraction fails
            return DateTime.MinValue;
        }
    }
}
