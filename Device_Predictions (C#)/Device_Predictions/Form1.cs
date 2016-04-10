using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using AutocompleteMenuNS;
using System.Net;
using System.IO;

namespace Device_Predictions
{
    public partial class Form1 : Form
    {
        private static SqlConnection con2db = new SqlConnection();
        private static SqlConnection con2WoTIndex = new SqlConnection();
        private string type = "DevNaming";
        private string[] TableURL = { "WoT_01", "WoT_02", "WoT_03", "WoT_04", "WoT_05", "WoT_06", "WoT_07", "WoT_08", "WoT_09", "WoT_10" };
        public Form1()
        {
            InitializeComponent();
            con2db.ConnectionString = Properties.Settings.Default.ConStr_SmartHome;
            con2WoTIndex.ConnectionString = Properties.Settings.Default.ConStr_WoTIndex;
            string []Display_List={"title","id","root_id","pin_id","gateway_id"};
          //  G_Operations.LoadCompWithCondition(com_Devices,Display_List, "id", " Select distinct title,id,root_id,pin_id,gateway_id from device order by id", con2db);
         //   com_Devices.SelectedIndex = 0;
            G_Operations.LoadCompWithCondition(com_type, "type", "id", " Select distinct type,id from Dev_Types order by id", con2WoTIndex);
            com_type.SelectedIndex = 0;
            try
            {
                G_Operations.LoadCompWithCondition(com_oldtype, "type", "type", " Select distinct type from device order by type", con2db);
                com_type.SelectedIndex = 0;
                get_devices();
            }
            catch { }
            text_maxbuilding.Text = get_max("building_id").ToString();
            text_maxdev.Text = get_max("device_id").ToString();
            //autosuggest();
        }
        private int get_max(string column)
        {
            int x = 0;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2WoTIndex;
           
            cmd.CommandText = "select max(" + column + ") as exp from DevPredictionModel";
            try
            {
                x = int.Parse(cmd.ExecuteScalar()+"");
            }
            catch { }
            con2WoTIndex.Close();
            return x;
        }
        private int get_count()
        {
            int x = 0;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2WoTIndex;

            cmd.CommandText = "select count(building_id) as exp from DevPredictionModel";
            try
            {
                x = int.Parse(cmd.ExecuteScalar() + "");
            }
            catch { }
            con2WoTIndex.Close();
            return x;
        }
        private int get_count_building_Index()
        {
            int x = 0;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2WoTIndex;

            cmd.CommandText = "select count(building_id) as exp from Dev_Building_Index";
            try
            {
                x = int.Parse(cmd.ExecuteScalar() + "");
            }
            catch { }
            con2WoTIndex.Close();
            return x;
        }
        private void get_devices()
        {
            con2db.Close();
            con2db.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = " select distinct title as Title, id as ID, root_id as Root, pin_id as Pin_ID, gateway_id as Gw_ID from device where type='" + com_oldtype.Text+ "'";
            cmd.Connection = con2db;
            SqlDataAdapter DA = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            DGV_Dev.DataSource = dt;
            con2db.Close();
            Lb_Devices.Text = "Devices=" + (DGV_Dev.RowCount - 1);
        }
        private void get_predictions()
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();//" select TOP (1000) * from DevPredictionModel ";
            cmd.CommandText = " SELECT        TOP (1000) DevPredictionModel.building_id, DevPredictionModel.device_id, DevPredictionModel.period,"
                                +" DevPredictionModel.sub_period, DevPredictionModel.state,  DevPredictionModel.probability, Dev_Types.type"
                                +" FROM            DevPredictionModel INNER JOIN       Dev_Types ON DevPredictionModel.type = Dev_Types.id"
                                + " order by DevPredictionModel.building_id, DevPredictionModel.device_id, DevPredictionModel.sub_period";
            cmd.Connection = con2WoTIndex;
            SqlDataAdapter DA = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            DGV_Predictions.DataSource = dt;
            con2WoTIndex.Close();
            Lb_Devices.Text = "Devices=" + (DGV_Predictions.RowCount - 1);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)  // real
        {
            group_rand.Enabled = false;
            group_real.Enabled = true;
            try { get_devices(); }
            catch { }
            listBox1.Items.Clear();
            Lb_Task_count.Text = "0";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)  // random
        {
            group_rand.Enabled = true;
            group_real.Enabled = false;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radio_Random.Checked)
            {
                listBox1.Items.Clear();
                type = "DevNaming";
                timer1.Start();
                
                //for (int j = 0; j < int.Parse(text_buildings.Text); j++)
                //    for (int i = 0; i < int.Parse(text_dev.Text); i++)
                //        listBox1.Items.Add(com_type.Text +"_"+j+ "_" + i);
                //if (com_period.Text == "7-Days")
                //    Lb_Task_count.Text = (int.Parse(text_buildings.Text) * int.Parse(text_dev.Text)).ToString() + " * " + com_period.Text + " = " + int.Parse(text_buildings.Text) * int.Parse(text_dev.Text) * 7;
                //else
                //    Lb_Task_count.Text = (int.Parse(text_buildings.Text) * int.Parse(text_dev.Text)).ToString() + " * " + com_period.Text + " = " + int.Parse(text_buildings.Text) * int.Parse(text_dev.Text) * 30;
                //pictureBox1.Visible = false;
            }
            else
            {
                //  for (int i = 0; i < com_Devices.Items.Count; i++)
                //     listBox1.Items.Add(com_Devices.Items[i].ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int R_ind;
            string dev;
            if (checkBox1.Checked)
            {
                Lb_Task_count.Text = (DGV_Dev.RowCount - 1).ToString();
                listBox1.Items.Clear();
                int c = DGV_Dev.RowCount - 1;
                for (int i = 0; i < c; i++)//for (int i = 0; i < c; c--)
                {
                    R_ind = i;
                    dev = DGV_Dev[0, R_ind].Value.ToString() + "(" + DGV_Dev[1, R_ind].Value.ToString() + "/" + DGV_Dev[2, R_ind].Value.ToString() + ":" + DGV_Dev[3, R_ind].Value.ToString() + "/" + DGV_Dev[4, R_ind].Value.ToString() + ")";
                    listBox1.Items.Add(dev);
                    // dataGridView1.Rows.RemoveAt(R_ind);
                    DGV_Dev.Rows[R_ind].DefaultCellStyle.BackColor = Color.Red;
                }
                //checkBox1.Checked = false;
            }
            else
            {
                R_ind = DGV_Dev.CurrentRow.Index;
                dev = DGV_Dev[0, R_ind].Value.ToString() + "(" + DGV_Dev[1, R_ind].Value.ToString() + "/" + DGV_Dev[2, R_ind].Value.ToString() + ":" + DGV_Dev[3, R_ind].Value.ToString() + "/" + DGV_Dev[4, R_ind].Value.ToString() + ")";
                if (!listBox1.Items.Contains(dev))
                {
                    listBox1.Items.Add(dev);
                    //  dataGridView1.Rows.RemoveAt(R_ind);
                    Lb_Task_count.Text = (int.Parse(Lb_Task_count.Text) + 1).ToString();
                    DGV_Dev.CurrentRow.DefaultCellStyle.BackColor = Color.Red;
                }
            }
            listBox1.SelectedIndex = 0;
        }

        private void com_oldtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            get_devices();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear(); 
            Lb_Task_count.Text = "0";
            checkBox1.Checked = false;
            for (int i = 0; i < DGV_Dev.RowCount - 1; i++)
                DGV_Dev.Rows[i].DefaultCellStyle.BackColor = Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            type = "DevGeneration";
           // progressBar2.Value = 0;
            timer1.Start();
            //Random x = new Random(1);
            //// building_id, device_id, type, period, sub_period, state, probability
            //string sqlins= "insert into DevPredictionModel values ";
            //for (int bl = 0; bl < int.Parse(text_buildings.Text); bl++)
            //    for (int dev = 0; dev < int.Parse(text_dev.Text); dev++)
            //        for (int day = 1; day < 8; day++)
            //            sqlins += "("+bl+","+dev+","+com_type.SelectedValue+",'"+com_period+"',"+day+",'Empty',"+x.Next()+")";

        }
 //------------------------------------------------------------
        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Value++;
            if (type == "DevNaming")
            {
                if (progressBar1.Value == 2)
                    pictureBox1.Visible = true;
                else if (progressBar1.Value == 4)
                {
                    for (int j = 0; j < int.Parse(text_buildings.Text); j++)
                        for (int i = 0; i < int.Parse(text_dev.Text); i++)
                            listBox1.Items.Add(com_type.Text + "_" + j + "_" + i);
                    if (com_period.Text == "7-Days")
                        Lb_Task_count.Text = (int.Parse(text_buildings.Text) * int.Parse(text_dev.Text)).ToString() + " * " + com_period.Text + " = " + int.Parse(text_buildings.Text) * int.Parse(text_dev.Text) * 7;
                    else
                        Lb_Task_count.Text = (int.Parse(text_buildings.Text) * int.Parse(text_dev.Text)).ToString() + " * " + com_period.Text + " = " + int.Parse(text_buildings.Text) * int.Parse(text_dev.Text) * 30;
                }
                else if (progressBar1.Value == 5)
                {
                    progressBar1.Value = 0;
                    pictureBox1.Visible = false;
                    timer1.Stop();
                }
            }
            //------------------------------------------------------------
            else
            {
               
             
                int days=7;
                if (com_period.Text != "7-Days")
                    days = 30;
                
                 int Maximum= (int.Parse(text_buildings.Text) * int.Parse(text_dev.Text) * days);
                 if (progressBar1.Value == 2)
                    pictureBox1.Visible = true;
                 else if (progressBar1.Value == 4)
                 {
                    // Random x = new Random(1);
                     // building_id, device_id, type, period, sub_period, state, probability

                     con2WoTIndex.Close();
                     con2WoTIndex.Open();
                     SqlCommand cmd = new SqlCommand();
                     cmd.Connection = con2WoTIndex;
                     string sqlins;
                     Random random = new Random();
                     int x = 0;

                     for (int bl = int.Parse(text_maxbuilding.Text); bl < int.Parse(text_buildings.Text) + int.Parse(text_maxbuilding.Text); bl++)
                     {
                         try
                         {
                             cmd = new SqlCommand("insert into Building_URL values (" + bl + ",'" + TableURL[bl - 1] + "')", con2WoTIndex); // + "\\" + dev + "
                             cmd.ExecuteNonQuery();
                         }
                         catch { }
                         for (int dev = int.Parse(text_maxdev.Text); dev < int.Parse(text_dev.Text) + int.Parse(text_maxdev.Text); dev++)
                         {
                             Random no_days = new Random();
                             days = no_days.Next(1, 8);
                             Random r_day = new Random();
                             int day_digit, count = -1;
                             int[] list = new int[7];

                             for (int day = 1; day <= days; day++)
                             {
                                 x++;

                                 do
                                 {
                                     day_digit = r_day.Next(1, 8);
                                 } while (list.Contains(day_digit));
                                 list[++count] = day_digit;
                                 sbrScan.Text = x.ToString();
                                 // toolStripProgressBar1.Value = x / ((bl + 1 - (int.Parse(text_maxbuilding.Text))) * (dev + 1 - (int.Parse(text_maxdev.Text))) * day * 100);//((bl+1 - (int.Parse(text_maxbuilding.Text))) * (dev+1 - (int.Parse(text_maxdev.Text))) * day * 100) / Maximum;
                                 // toolStripStatusLabel1.Text = x + "";
                                 double number;
                                 number = Math.Round(random.NextDouble(), 2);
                                 sqlins = "insert into DevPredictionModel values (" + bl + "," + dev + "," + com_type.SelectedValue + ",'" + com_period.Text + "'," + day_digit + ",'Empty'," + number + ")";
                                 cmd.CommandText = sqlins;
                                 cmd.ExecuteNonQuery();
                             }
                         }
                       
                     }// end for  
                     con2WoTIndex.Close();
                 }// end else if
                 else if (progressBar1.Value == 5)
                 {
                     progressBar1.Value = 0;
                     //progressBar2.Value = 0;
                     pictureBox1.Visible = false;
                  //   progressBar2.Visible = false;
                     timer1.Stop();
                 }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Random random = new Random();
            for (int i = 0; i < 50; i++)
            {
                double number;
                number = Math.Round(random.NextDouble(), 2);
               // do number =Math.Round(random.NextDouble(),2);
               // while (listBox2.Items.Contains(number));

               // listBox2.Items.Add(number);
            }
        }
        public static List<double> GetRandomNumbers(int count)
        {
            List<double> randomNumbers = new List<double>();
            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                double number;

                do number = Math.Round(random.NextDouble(), 2);
                while (randomNumbers.Contains(number));

                randomNumbers.Add(number);
            }
            return randomNumbers;
        }

        private void button6_Click(object sender, EventArgs e)
        {

            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand(" delete from DevPredictionModel ");//where building_id>=5000 ");
            cmd.Connection = con2WoTIndex;
            cmd.ExecuteNonQuery();
            con2WoTIndex.Close();
            text_maxbuilding.Text = get_max("building_id").ToString();
            text_maxdev.Text = get_max("device_id").ToString();
            button7_Click( sender,  e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            get_predictions();
            textBox2.Text =""+ get_count();
            textBox1.Text = "" + get_max("device_id");

            textBox5.Text = text_maxbuilding.Text;
            textBox20.Text=textBox6.Text = textBox2.Text;
            textBox7.Text = textBox1.Text; 
        }
        private DateTime get_date(int d)
        {
            int test = d - 1;
            string[] list = {"Sunday","Monday","Tuesday","Wednesday","Thursday","Friday","Saturday"};
            DateTime x=DateTime.Now.Date;
           // int c=(DateTime.Now.Date.Day) /7;c=c*7+d;
            int s;
            for (s = 0; s < 7; s++)
                if (list[s] == x.DayOfWeek.ToString())
                    break;
            if (test == s)
                return x;
            else if (test > s)
             x = x.AddDays(test - s); 
            else
             x = x.AddDays(7 - s + test);
            
            //if (d > (s + 1))
            //    x=x.AddDays(d - s - 1);
            //else if (d < (s + 1))
            //    x=x.AddDays(7- s);
             


            return x;
        }
        private List<int> getdeviceurl(int b, int d, double p)
        {
            List<int> list = new List<int>();
            SqlConnection sqlcon = new SqlConnection(Properties.Settings.Default.ConStr_WoTIndex);
            sqlcon.Open();
            SqlCommand cmd = new SqlCommand("SELECT device_id FROM DevPredictionModel where (building_id=" + b + ") and (sub_period=" + d + ") and (probability=" + p+")", sqlcon);
            using (SqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    list.Add(rdr.GetInt32(0));
                }
            }
            sqlcon.Close();
            return list;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2WoTIndex;

            SqlConnection con2dbb = new SqlConnection();
            con2dbb.ConnectionString = Properties.Settings.Default.ConStr_WoTIndex;
            con2dbb.Close();
            con2dbb.Open();
            SqlCommand cmd_ins = new SqlCommand();
            cmd_ins.Connection = con2dbb;
            string sqlinsert;
            //int x = 0;
            if (radio_SFW.Checked)
            {
                cmd.CommandText = " SELECT     DISTINCT    building_id, sub_period,  MAX(probability) AS Prediction"
                                 + " FROM          DevPredictionModel"
                                 + " GROUP BY      sub_period, building_id"
                                 + " ORDER BY      building_id, sub_period, Prediction";
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int value1 = rdr.GetInt32(0);
                        int value2 = rdr.GetInt32(1);
                        double value3 = rdr.GetDouble(2);
                        List<int>Devids= getdeviceurl(value1,value2,value3);
                        DateTime dt = get_date(value2);
                       // string durl = "";
                        for (int i = 0; i < Devids.Count; i++)
                        {
                           // durl = TableURL + "\\" + Devids[i];
                            sqlinsert = "insert into Dev_Building_Index values (" + value1 + ",'" + dt.Date.ToShortDateString() + "'," + value3 + ",'Empty','" + (TableURL[value1-1] + "\\" + Devids[i]) + "')";
                            cmd_ins.CommandText = sqlinsert;
                            // cmd_ins.Parameters.Add("dtt", SqlDbType.DateTime).Value = dt;
                            cmd_ins.ExecuteNonQuery();
                            statusBar2.Text = (int.Parse(statusBar2.Text) + 1).ToString();
                        }
                    }
                }
            }
            else
            {
                cmd.CommandText = " SELECT         building_id,device_id, sub_period, probability AS Prediction"
                                 + " FROM          DevPredictionModel "//where building_id>5000"
                                 + " ORDER BY      sub_period, Prediction";
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        int value1 = rdr.GetInt32(0);
                        int value2 = rdr.GetInt32(1);
                        int value3 = rdr.GetInt32(2);
                        double value4 = rdr.GetDouble(3);
                        DateTime dt = get_date(value3);
                        sqlinsert = "insert into Dev_Dev_Index values (" + value1 + "," + value2 + ",'" + dt.Date.ToShortDateString() + "'," + value4 + ",'Empty','" + (TableURL[value1 - 1] + "\\" + value2) + "')";
                        cmd_ins.CommandText = sqlinsert;
                        // cmd_ins.Parameters.Add("dtt", SqlDbType.DateTime).Value = dt;
                        cmd_ins.ExecuteNonQuery();
                        statusBar1.Text = (int.Parse(statusBar1.Text) + 1).ToString();
                    }
                }
            }
            con2WoTIndex.Close();
            con2dbb.Close();
            if (radio_SFW.Checked)
                button10_Click(sender, e);
            else
                button13_Click(sender, e);
            
        }

        private void button10_Click(object sender, EventArgs e)
        {
            get_building_predictions();
            textBox3.Text = textBox15.Text = get_count_building_Index().ToString();

        }
        private void get_building_predictions()
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = " select * from Dev_Building_index order by date_day , building_id, probability";
            cmd.Connection = con2WoTIndex;
            SqlDataAdapter DA = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            dataGridView1.DataSource = dt;
            con2WoTIndex.Close();
           // Lb_Devices.Text = "Devices=" + (DGV_Predictions.RowCount - 1);
        }
        private void get_device_predictions()
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = " select top(1000) * from Dev_Dev_index order by date_day , probability";
            cmd.Connection = con2WoTIndex;
            SqlDataAdapter DA = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            dataGridView2.DataSource = dt;
            con2WoTIndex.Close();
            // Lb_Devices.Text = "Devices=" + (DGV_Predictions.RowCount - 1);
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
          
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            textBox2.Text = "" + get_count();
            textBox20.Text=textBox6.Text = textBox2.Text;

            textBox1.Text = "" + get_max("device_id");
            textBox7.Text = textBox1.Text;
 
            textBox5.Text = text_maxbuilding.Text;
           
           
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox2.Text = "" + get_count();
            textBox20.Text = textBox6.Text = textBox2.Text;
            textBox3.Text = textBox15.Text = get_count_building_Index().ToString();
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox6.Enabled = false;
            groupBox12.Enabled = false;
            groupBox7.Enabled = true;
        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            groupBox6.Enabled = true;
            groupBox12.Enabled = true;
            groupBox7.Enabled = false;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            about frm = new about();
            frm.ShowDialog();
        }

        private void autosuggest()
        {
            SqlCommand cmd = new SqlCommand("SELECT word FROM Dictionary", con2WoTIndex);
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            AutoCompleteStringCollection MyCollection = new AutoCompleteStringCollection();
            while (reader.Read())
            {
                MyCollection.Add(reader.GetString(0));
            }
            
           // autocompleteMenu1.Items = MyCollection;
            txtFirstName.AutoCompleteCustomSource = MyCollection;
            con2WoTIndex.Close();
            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DateTime Dt_Start;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();
            string k_result = ""; 
            float probability=0;
            try 
            {
                if (radio_K_result.Checked)
                {
                    int max = int.Parse(textBox4.Text);
                    k_result = " top(" + max + ") ";
                }
                else
                    probability = int.Parse(text_probability.Text)/100;
                
            }
            catch { }
            string sql = "select " + k_result;
            DataTable dt = new DataTable();
            if (radio_Search_DSE.Checked)
            {
                Dt_Start = System.DateTime.Now;
                sql += " device_id, date_day, probability, Device_URL from Dev_Dev_Index where (probability>=" + probability + " and date_day= @Devdate) order by probability desc";
                
                cmd.CommandText = sql;
                cmd.Parameters.Add("Devdate", SqlDbType.Date).Value = System.DateTime.Now.Date;
                cmd.Connection = con2WoTIndex;

                SqlDataAdapter DA = new SqlDataAdapter(cmd);
                dt.Clear();
                DA.Fill(dt);
                SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
                dataGridView4.DataSource = dt;
                if (!k_result.Contains("top"))
                    textBox14.Text = count("Dev_Dev_Index").ToString();
                else
                    textBox14.Text = textBox4.Text;
                //textBox13.Text = System.DateTime.Now.Subtract(Dt_Start).Ticks.ToString();// textBox13.Text = DT_End.Subtract(Dt_Start).Ticks.ToString();  
                //

                textBox13.Text=TimeSpan.FromTicks(System.DateTime.Now.Subtract(Dt_Start).Ticks).TotalMilliseconds.ToString();


            }
            else
            {
                Dt_Start = System.DateTime.Now;
                if (radio_Fast.Checked)
                {
                    //1 sql += " building_id from Dev_Building_Index where (probability>=" + probability + " and date_day= @Devdate) order by probability desc";
                    //2 sql += " Building_URL.URL, Dev_Building_Index.probability "
                    //     +  " FROM         Dev_Building_Index INNER JOIN Building_URL ON Dev_Building_Index.building_id = Building_URL.building_id"
                    //     +  " WHERE        (Dev_Building_Index.probability >=" + probability + " and Dev_Building_Index.date_day= @Devdate) order by Dev_Building_Index.probability desc";
                    sql += " building_id, date_day, probability, Device_URL FROM  Dev_Building_Index "
                        + " WHERE        (probability >= " + probability + ") and (date_day= @Devdate) order by probability desc";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("Devdate", SqlDbType.Date).Value = System.DateTime.Now.Date;
                    cmd.Connection = con2WoTIndex;
                    SqlDataAdapter DA = new SqlDataAdapter(cmd);
                    dt.Clear();
                    DA.Fill(dt);
                    SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
                    dataGridView3.DataSource = dt;
                }
                else
                {
                    sql = "SELECT DISTINCT Building_URL.URL, Dev_Building_Index.probability"
                        +" FROM         Building_URL INNER JOIN   Dev_Building_Index ON Building_URL.building_id = Dev_Building_Index.building_id"
                        + " WHERE        (Dev_Building_Index.probability >=" + probability + ") and (date_day= @Devdate) order by Dev_Building_Index.probability desc";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("Devdate", SqlDbType.Date).Value = System.DateTime.Now.Date;
                    cmd.Connection = con2WoTIndex;
                    SqlDataReader rdr;
                    using (rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string buildingur = rdr.GetString(0);
                            WebRequest request = WebRequest.Create("http://localhost:7000/Service1.svc/WoTLocSearch/"+text_probability.Text+"/"+buildingur+"");
                            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                            Stream dataStream = response.GetResponseStream();
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();
                            responseFromServer = responseFromServer.Replace("\"", null);
                            responseFromServer = responseFromServer.Replace("[", null);
                            responseFromServer = responseFromServer.Replace("]", null);
                            responseFromServer = responseFromServer.Replace("\\\\", "/");
                            string[] list = responseFromServer.Split(',');
                            for (int i = 0; i < list.Length; i++)
                                listBox_Result.Items.Add("http://.../" + list[i]);
                        }
                    }
                }
             /*   SqlDataReader rdr;
                using (rdr = cmd.ExecuteReader())
                {
                    string table;
                    float prob;
                    SqlCommand ncmd;
                    SqlConnection con2dbb = new SqlConnection();
                    con2dbb.ConnectionString = Properties.Settings.Default.ConStr_WoT_Local_Index;
                    con2dbb.Close();
                    con2dbb.Open();
                    
                   // List<string> ls=new List<string>();
                    while (rdr.Read())
                    {
                        table = rdr.GetString(0);
                        prob =float.Parse(rdr.GetSqlValue(1).ToString());
                        ncmd = new SqlCommand("Select URL from " + table + " where probability = " + prob);
                        ncmd.Connection = con2dbb;
                        if (radio_Accurate.Checked)
                        {

                        }
                        listBox2.Items.Add(ncmd.ExecuteScalar().ToString());

                    }
                }*/
               // dataGridView3.DataSource = dt;
                if (!k_result.Contains("top"))
                    textBox12.Text = count("Dev_Building_Index").ToString();
                else
                    textBox12.Text = textBox4.Text;
                if (radio_Accurate.Checked)
                    textBox12.Text = listBox_Result.Items.Count.ToString();
               // textBox11.Text = System.DateTime.Now.Subtract(Dt_Start).Ticks.ToString();//textBox11.Text = DT_End.Subtract(Dt_Start).Ticks.ToString();
                textBox11.Text = TimeSpan.FromTicks(System.DateTime.Now.Subtract(Dt_Start).Ticks).TotalMilliseconds.ToString();
            }
            button12_Click(sender, e);
            con2WoTIndex.Close();
        }
        private int count(string table_name)
        {
            //DateTime Dt_Start = System.DateTime.Now;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "select count(building_id) from " + table_name + " where (probability>0 and date_day= @Devdate)";
            cmd.Parameters.Add("Devdate", SqlDbType.Date).Value = System.DateTime.Now.Date;
            cmd.Connection = con2WoTIndex;
            int x= int.Parse(cmd.ExecuteScalar().ToString());
            con2WoTIndex.Close();
            return x;
        }
        private void button13_Click(object sender, EventArgs e)
        {
            get_device_predictions();
            textBox2.Text = "" + get_count();
            textBox20.Text = textBox6.Text = textBox2.Text;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand(" delete from Dev_Dev_Index ");//where building_id>=5000");
            cmd.Connection = con2WoTIndex;
            cmd.ExecuteNonQuery();
            con2WoTIndex.Close();
           
            button13_Click(sender, e);
            textBox6.Text = "0";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd = new SqlCommand(" delete from Dev_Building_Index ");//where building_id<5000");
            cmd.Connection = con2WoTIndex;
            cmd.ExecuteNonQuery();
            con2WoTIndex.Close();
           
            button10_Click(sender, e);
            textBox3.Text = "0";
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = false;
            groupBox5.Enabled = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            groupBox3.Enabled = true;
            groupBox5.Enabled = false;
        }

        private void txtFirstName_Enter(object sender, EventArgs e)
        {
            autocompleteMenu1.AddItem(txtFirstName.Text);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            statusBar3.Visible = true;
            con2WoTIndex.Close();
            con2WoTIndex.Open();
            SqlCommand cmd ;//= new SqlCommand();
            //cmd.Connection = con2WoTIndex;

            SqlConnection con2dbb = new SqlConnection();
            con2dbb.ConnectionString = Properties.Settings.Default.ConStr_WoT_Local_Index;
            con2dbb.Close();
            con2dbb.Open();
            SqlCommand cmd_ins = new SqlCommand();
            cmd_ins.Connection = con2dbb;
            string sqlinsert;
            SqlDataReader rdr;
            if (checkBox2.Checked)
            {
                for (int i = 0; i < 10; i++)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = con2WoTIndex;
                    cmd.CommandText = " SELECT         device_id, date_day, probability, state "
                                     + " FROM          Dev_Dev_Index where building_id=" + (i+1);
                              
                    using ( rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            statusBar3.Text = (int.Parse(statusBar3.Text) + 1).ToString();
                            int value1 = rdr.GetInt32(0);
                            DateTime value2 = rdr.GetDateTime(1);
                            
                            double value3 = rdr.GetDouble(2);
                            string value4 = rdr.GetString(3);

                            sqlinsert = "insert into " + comboBox1.Items[i].ToString() + " values (" + value1 + ",'" + value2.Date + "'," + value3 + ",'" + value4 + "','" + comboBox1.Items[i].ToString() + "\\" + value1 + "')";
                            cmd_ins.CommandText = sqlinsert;
                            // cmd_ins.Parameters.Add("dtt", SqlDbType.DateTime).Value = dt;
                            cmd_ins.ExecuteNonQuery();
                            statusBar1.Text = (int.Parse(statusBar1.Text) + 1).ToString();
                        }
                    }
                }
            }
            else
            {
            }
            statusBar3.Visible = false;
        }
//--------------------------------------------------------------------
        private void button17_Click(object sender, EventArgs e)
        {
            SqlConnection con2dbb = new SqlConnection();
            con2dbb.ConnectionString = Properties.Settings.Default.ConStr_WoT_Local_Index;
            con2dbb.Close();
            con2dbb.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2dbb;
            if (checkBox2.Checked)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    cmd.CommandText = "delete from " + comboBox1.Items[i].ToString();
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                cmd.CommandText = "delete from " + comboBox1.Text;
                cmd.ExecuteNonQuery();
            }
            con2dbb.Close();
            button16_Click(sender, e);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SqlConnection con2dbb = new SqlConnection();
            con2dbb.ConnectionString = Properties.Settings.Default.ConStr_WoT_Local_Index;
            con2dbb.Close();
            con2dbb.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con2dbb;
            if (checkBox3.Checked)
                cmd.CommandText = "SELECT     top(1000)   device_id, date_day, probability, state, URL "+
                                  " FROM         ( " +
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_01 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_02 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_03 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_04 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_05 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_06 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_07 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_08 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_09 UNION"+
                                  " SELECT        device_id, date_day, probability, state, URL FROM            WoT_10 " +
                                  " ) AS derivedtbl_1 ORDER BY date_day,device_id, URL ";
            else
                cmd.CommandText = "select *  from " + comboBox1.Text + " ORDER BY date_day,device_id";
            SqlDataAdapter DA = new SqlDataAdapter(cmd);
            
            DataTable dt = new DataTable();
            dt.Clear();
            DA.Fill(dt);
            SqlCommandBuilder ComB = new SqlCommandBuilder(DA);
            dataGridView5.DataSource = dt;
            con2dbb.Close();
        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {
            txtFirstName.Text = "";
        }

        private void radio_Fast_CheckedChanged(object sender, EventArgs e)
        {
            listBox_Result.Visible = false;
        }

        private void radio_Accurate_CheckedChanged(object sender, EventArgs e)
        {
            listBox_Result.Visible = true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                comboBox1.Text="All";
            else
                comboBox1.Text="WoT_01";
        }


 
//------------------------------------------------------------       
    }

    internal class EmailSnippet : AutocompleteItem
    {
        public EmailSnippet(string email)
            : base(email)
        {
            ImageIndex = 0;
            ToolTipTitle = "Insert logic path:";
            ToolTipText = email;
        }

        public override CompareResult Compare(string fragmentText)
        {
            if (fragmentText == Text)
                return CompareResult.VisibleAndSelected;
            if (fragmentText.Contains("@"))
                return CompareResult.Visible;
            return CompareResult.Hidden;
        }
    }
}
