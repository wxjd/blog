using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;



using System.Data;

using System.Data.SqlClient;



namespace NiitBlog
{

    public class DBHelper
    {

        public static string ConString = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["BlogEntities1"].ToString();



        //执行增删改的方法

        public static int RunNoQuery(string cmdText, CommandType cmdType, params SqlParameter[] pars)
        {

            SqlConnection con = new SqlConnection(ConString);

            con.Open();

            SqlCommand cmd = new SqlCommand(cmdText, con);

            cmd.CommandType = cmdType;

            if (pars != null && pars.Length > 0)
            {

                foreach (SqlParameter p in pars)
                {

                    cmd.Parameters.Add(p);

                }

            }

            int rows = cmd.ExecuteNonQuery();

            con.Close();

            return rows;

        }



        //执行查询（DataSet）的方法

        public static DataSet RunSelect(string cmdText, CommandType cmdType, params SqlParameter[] pars)
        {

            SqlConnection con = new SqlConnection(ConString);



            SqlDataAdapter da = new SqlDataAdapter(cmdText, con);

            da.SelectCommand.CommandType = cmdType;

            if (pars != null && pars.Length > 0)
            {

                foreach (SqlParameter p in pars)
                {

                    da.SelectCommand.Parameters.Add(p);

                }

            }

            DataSet ds = new DataSet();

            da.Fill(ds);



            return ds;

        }



        //执行查询得到一个值

        public static object RunOneValue(string cmdText, CommandType cmdType, params SqlParameter[] pars)
        {

            SqlConnection con = new SqlConnection(ConString);

            con.Open();

            SqlCommand cmd = new SqlCommand(cmdText, con);

            cmd.CommandType = cmdType;

            if (pars != null && pars.Length > 0)
            {

                foreach (SqlParameter p in pars)
                {

                    cmd.Parameters.Add(p);

                }

            }

            object obj = cmd.ExecuteScalar();

            con.Close();

            return obj;

        }

    }

}