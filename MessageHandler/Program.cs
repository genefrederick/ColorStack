using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Data;

using System.Timers;
using Microsoft.ServiceBus.Messaging;

namespace MessageHandler
{
    class Program
    {

        //string declarations
        public static string databaseConnection = "Data Source=.;Initial Catalog=Colors;User ID=roygbiv;Password=roygbiv";
        public static string AzureConnectionString = "Endpoint=sb://colorqueue.servicebus.windows.net/;SharedAccessKeyName=submitandprocess;SharedAccessKey=YSMA0hy8TwOV6eER8n8rMxvDEeMYZN7fwMd57bUQtG0=";
        public static string execSendQuery = "dbo.sendToAzure";
        public static string execInsertQuery = "dbo.insertFromAzure";

        public static string databaseName = "colors";

        public static void Main()
        {

            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 1000;
            aTimer.Enabled = true;

            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;

        }

        protected static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            MessagingFactory factory = MessagingFactory.CreateFromConnectionString(AzureConnectionString);

            AzureMessageSender(factory);
            AzureMessageConsumer(factory);

        }

        protected static void AzureMessageSender(MessagingFactory f)
        {
            //use the already created messaging factory to create a msg sender
            MessageSender testQueueSender = f.CreateMessageSender(databaseName);

            SqlConnection conn = new SqlConnection(databaseConnection);

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(execSendQuery, conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                //parameters
                cmd.Parameters.Add("@r", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                //read output
                string result = Convert.ToString(cmd.Parameters["@r"].Value);

                //craft and send message
                BrokeredMessage message = new BrokeredMessage(result);
                testQueueSender.Send(message);
                Console.WriteLine("Message is sent: " + result);

                cmd.Dispose();

            }
            catch (Exception o)
            {
                Console.WriteLine(o);
            }
            finally
            {
                if (conn != null)
                { conn.Close(); }
            }
        }

        protected static void AzureMessageConsumer(MessagingFactory f)
        {
            //use the already created messaging factory to create a msg receiver
            MessageReceiver testQueueReceiver = f.CreateMessageReceiver("colors");

            while (true)
            {
                using (BrokeredMessage retrievedMessage = testQueueReceiver.Receive())
                {

                    try
                    {
                        string msgResult = retrievedMessage.GetBody<string>();

                        //call SP to insert the data into the proper table
                        InsertSQL(msgResult);

                        Console.WriteLine("Message received: " + msgResult);
                        retrievedMessage.Complete();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        retrievedMessage.Abandon();
                    }
                }
            }
        }

        protected static void InsertSQL(string color)
        {
            SqlConnection conn = new SqlConnection(databaseConnection);

            try
            {
                conn.Open();
                
                SqlCommand cmd = new SqlCommand(execInsertQuery, conn);

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@inputColor", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Input;

                cmd.Parameters["@inputColor"].Value = color;

                Console.WriteLine("The sql value is: " + cmd.Parameters["@inputColor"].Value);

                cmd.ExecuteNonQuery();

                cmd.Dispose();
            }
            catch (Exception o)
            {
                Console.WriteLine(o);
            }
            finally
            {                
                if (conn != null)
                { conn.Close(); }
            }
        }

    }    


        
    
}
