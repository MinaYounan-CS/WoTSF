using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace WoT_LSE_WS_v7
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public List<string> WoTLocSearch(string x, string y)
        {
            float probability=int.Parse(x)/100;
            List<string> URLs = new List<string>();
            SqlConnection SqlCon = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=Indexing_WoT_Local;Integrated Security=True");
            SqlCon.Close();
            SqlCon.Open();
            SqlCommand cmd = new SqlCommand("Select distinct URL,probability from " + y + " where date_day=@Devdate and probability >= " + probability + "  ORDER BY probability");
            cmd.Connection = SqlCon;
            cmd.Parameters.Add("Devdate", SqlDbType.Date).Value = System.DateTime.Now.Date;
            try
            {
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    { URLs.Add(rdr.GetString(0)); }
                }
                SqlCon.Close();
            }
            catch { }
            //ls.Add((Convert.ToInt32(x) + Convert.ToInt32(y)).ToString());
            return URLs;
        }
    }
}
