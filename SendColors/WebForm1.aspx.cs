using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

using Microsoft.ServiceBus.Messaging;

namespace SendColors
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        //master database connection string
        string databaseConnection = "Data Source=.;Initial Catalog=Colors;User ID=roygbiv;Password=roygbiv";
        

        protected void Page_Load(object sender, EventArgs e)
        {

            Display_Colors(databaseConnection, "select * from SentColors", sentcolorsholder, "Sent Colors");
            Display_Colors(databaseConnection, "select * from ReceivedColors", receivedcolorsholder, "Received Colors");

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string insertcommand = "insert into SentColors values ('" + RadioButtonList1.SelectedValue + "')";

            InsertData(databaseConnection, insertcommand);

            //reload page
            Page_Load(sender, e);
        }

        protected void Display_Colors(string dbConnStr, string sqlQuery1, HtmlGenericControl colorsholder, string queueName)
        {
            string colorHTML = "<div class=\"title\">" + queueName + "</div>";

            SqlConnection conn = new SqlConnection(dbConnStr);
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sqlQuery1, conn);

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    colorHTML += "<div class=\"" + rdr[1] + "\">" + rdr[1] + "</div>";
                }
            }

            finally
            {
                if (rdr != null)
                { rdr.Close(); }

                if (conn != null)
                { conn.Close(); }

                colorsholder.InnerHtml = colorHTML;
            }
        }

        protected void InsertData(string dbConnStr, string sqlQuery1)
        {

            SqlConnection conn = new SqlConnection(dbConnStr);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery1, conn);
                cmd.ExecuteNonQuery();
            }

            finally
            {
                if (conn != null)
                { conn.Close(); }
            }
        }

        protected void Timer1_Tick1(object sender, EventArgs e)
        {
            Display_Colors(databaseConnection, "select * from SentColors", sentcolorsholder, "Sent Colors");
            Display_Colors(databaseConnection, "select * from ReceivedColors", receivedcolorsholder, "Received Colors");
        }
    }
}