using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.Drawing.Imaging;

namespace Telephone
{
    public partial class Phone : Form
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Petrenko\Documents\Phone.mdf;Integrated Security=True;Connect Timeout=30;");
        
        public Phone()
        {
            InitializeComponent(); 
        }

        private void Phone_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'phoneDataSet.Mobiles' table. You can move, or remove it, as needed.
            this.mobilesTableAdapter.Fill(this.phoneDataSet.Mobiles);
            Display();
        }

        private string tempTextBoxText;

        public string TempTextBoxText
        {
            get
            {
                return tempTextBoxText;
            }
            set
            {
                tempTextBoxText = value;
            }
        }   

        private void button1_Click(object sender, EventArgs e)
        {
            ClearAllFields();
            textBox1.Focus();
        }

        void ClearAllFields()
        {
            textBox1.Text = "";
            textBox2.Clear();
            textBox3.Text = "";
            textBox4.Clear();
            comboBox1.SelectedIndex = -1;            
            pictureBox.Image = null;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "" && textBox3.Text != "" && textBox4.Text != "" && comboBox1.Text != "")
            {
                FileStream fs1 = new FileStream(textBox1Browse.Text, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] image = new byte[fs1.Length];
                fs1.Read(image, 0, Convert.ToInt32(fs1.Length));
                fs1.Close();

                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Petrenko\Documents\Phone.mdf;Integrated Security=True;Connect Timeout=30;");
                con.Open();
                SqlCommand cmd = new SqlCommand(@"INSERT Into Mobiles (First, Last, Mobile, Email, Category, Image) VALUES ('"
                                                   + textBox1.Text + "','" + textBox2.Text + "','"
                                                   + textBox3.Text + "', '" + textBox4.Text + "','" + comboBox1.Text + "', @Pic)", con);

                SqlParameter prm = new SqlParameter("@Pic", SqlDbType.VarBinary, image.Length, ParameterDirection.Input, false, 0, 0, null, DataRowVersion.Current, image);
                cmd.Parameters.Add(prm);
                cmd.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("Successfully saved!", "Info");
                Display();                 
            }
            else
            {
                MessageBox.Show("Fill all the fields", "Info");
            }
        }

        void Display()
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select * from Mobiles", con );
            DataTable dt = new DataTable();

            sda.Fill(dt);
            dataGridView1.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = item["First"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item[1].ToString();
                dataGridView1.Rows[n].Cells[2].Value = item["Mobile"].ToString();
                dataGridView1.Rows[n].Cells[3].Value = item[3].ToString();
                dataGridView1.Rows[n].Cells[4].Value = item[4].ToString();                  
            }
            labelTotalContacts.Text = dataGridView1.Rows.Count.ToString();
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            comboBox1.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            TempTextBoxText = textBox3.Text.ToString();            
            //int tempRowNumber = dataGridView1.CurrentCell.RowIndex; 

            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Petrenko\Documents\Phone.mdf;Integrated Security=True;Connect Timeout=30;");
            SqlDataAdapter sda = new SqlDataAdapter("Select * From Mobiles Where Mobile ='" + textBox3.Text + "'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);

            byte[] MyData = new byte[0];
            MyData = (byte[])dt.Rows[0]["Image"];
            MemoryStream str = new MemoryStream(MyData);
            pictureBox.Image = Image.FromStream(str);  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Mobiles WHERE (Mobile = '" + textBox3.Text + "')",con);
            cmd.ExecuteNonQuery();
            con.Close();
            Display();
            ClearAllFields();
            textBox1.Focus();
            MessageBox.Show("Deleted Successfully!", "Info");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MemoryStream stream = new MemoryStream();
            pictureBox.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);    
           
            SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Petrenko\Documents\Phone.mdf;Integrated Security=True;Connect Timeout=30;");
            con.Open();           
            SqlCommand cmd = new SqlCommand("DELETE FROM Mobiles WHERE (Mobile = '" + tempTextBoxText + "')", con);
            SqlCommand cmd1 = new SqlCommand(@"INSERT Into Mobiles (First, Last, Mobile, Email, Category, Image) VALUES ('"
                                                   + textBox1.Text + "','" + textBox2.Text + "','"
                                                   + textBox3.Text + "', '" + textBox4.Text + "','" + comboBox1.Text + "', @Pic)", con);
            byte[] pic = stream.ToArray();
            cmd1.Parameters.AddWithValue("@Pic", pic);
            cmd.ExecuteNonQuery();              
            cmd1.ExecuteNonQuery();
            con.Close();        
            Display();
            MessageBox.Show("Successfully updated!", "Info");
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            ClearAllFields(); 
            
            SqlDataAdapter sda = new SqlDataAdapter("Select * from Mobiles Where (Mobile like '%" + textBox5.Text + "%') or (First like '%" + textBox5.Text + "%') or (Last like '%" + textBox5.Text + "%') or (Email like '%" + textBox5.Text + "%')", con);
            DataTable dt = new DataTable();

            sda.Fill(dt);
            dataGridView1.Rows.Clear();
            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = item["First"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item[1].ToString();
                dataGridView1.Rows[n].Cells[2].Value = item["Mobile"].ToString();
                dataGridView1.Rows[n].Cells[3].Value = item[3].ToString();
                dataGridView1.Rows[n].Cells[4].Value = item[4].ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog f = new OpenFileDialog();
                f.Filter = "All Files|*.*|JPEGs|*.jpg|Bitmaps|*.bmp|GIFs|*.gif";
                f.FilterIndex = 2;
                if (f.ShowDialog() == DialogResult.OK) 
                {
                    textBox1Browse.Text = f.FileName;
                    pictureBox.Image = Image.FromFile(f.FileName);
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            catch { }
        }       
    }
}
