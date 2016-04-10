using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Device_Predictions
{
    class G_Operations
    {
        public static void LoadCompWithCondition(ComboBox CBox, string []display_column, string value_column, string sqlstring, SqlConnection con)
        {
            con.Close();
            con.Open();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sqlstring;
            cmd.Connection = con;
            SqlDataAdapter adaptorr = new SqlDataAdapter(cmd);
            DataTable DTt = new DataTable();
            adaptorr.Fill(DTt);
            // CBox.Items.Clear();
            CBox.DataSource = DTt;
            CBox.DisplayMember = "" + display_column[0]+"(" +display_column[1]+"/"+display_column[2]+":"+display_column[2]+"/"+display_column[3]+")"+"";
            CBox.ValueMember = "" + value_column + "";
            con.Close();
        }
        public static void LoadCompWithCondition(ComboBox CBox, string display_column, string value_column, string sqlstring, SqlConnection con)
        {
            con.Close();
            con.Open();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = sqlstring;
            cmd.Connection = con;
            SqlDataAdapter adaptorr = new SqlDataAdapter(cmd);
            DataTable DTt = new DataTable();
            adaptorr.Fill(DTt);
            // CBox.Items.Clear();
            CBox.DataSource = DTt;
            CBox.DisplayMember = "" + display_column;
            CBox.ValueMember = "" + value_column + "";
            con.Close();
        }
    }
}
