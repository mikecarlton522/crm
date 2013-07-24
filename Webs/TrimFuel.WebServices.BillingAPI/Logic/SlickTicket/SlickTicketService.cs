using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using TrimFuel.Business;

namespace TrimFuel.WebServices.BillingAPI.Logic.SlickTicket
{
    public class SlickTicketService
    {
        private string _connectionString = string.Empty;

        public SlickTicketService()
        {
            _connectionString = TrimFuel.Business.Config.Current.CONNECTION_STRINGS["SlickTicket"];
        }

        public User GetUserByName(string admin)
        {
            SqlConnection connection = null;
            SqlCommand command = null;

            User res = null;

            try
            {
                connection = new SqlConnection(_connectionString);

                connection.Open();

                command = new SqlCommand("select id, userName, sub_unit from users where userName = @admin");
                command.Connection = connection;
                command.Parameters.AddWithValue("@admin", admin);

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    res = new User();

                    while (reader.Read())
                    {
                        res.UserID = Convert.ToInt32(reader["id"]);
                        res.UserName = Convert.ToString(reader["userName"]);
                        res.SubUnit = Convert.ToInt32(reader["sub_unit"]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();

                if (command != null)
                    command.Dispose();
            }

            return res;
        }

        public Ticket CreateTicket(string subject, string content, int assignTo, int priority, int submitter, int originatingGroup, long? billingID)
        {
            SqlConnection connection = null;
            SqlCommand command = null;

            Ticket res = null;

            try
            {
                connection = new SqlConnection(_connectionString);

                connection.Open();

                command = new SqlCommand(@"
                    INSERT INTO tickets (title,details,submitter,submitted,last_action,closed,assigned_to_group,ticket_status,priority,originating_group,active,billingID) 
                    VALUES(@title,@details,@submitter,@submitted,@last_action,@closed,@assigned_to_group,@ticket_status,@priority,@originating_group,@active,@billingID);
                    SELECT @@IDENTITY;");

                command.Connection = connection;
                
                command.Parameters.AddWithValue("@title", subject);
                command.Parameters.AddWithValue("@details", content);
                command.Parameters.AddWithValue("@submitter", submitter);
                command.Parameters.AddWithValue("@submitted", DateTime.Now);
                command.Parameters.AddWithValue("@last_action", DateTime.Now);
                command.Parameters.AddWithValue("@closed", DateTime.Parse("1/1/2001"));
                command.Parameters.AddWithValue("@assigned_to_group", assignTo);
                command.Parameters.AddWithValue("@ticket_status", 1);
                command.Parameters.AddWithValue("@priority", priority);
                command.Parameters.AddWithValue("@originating_group", originatingGroup);
                command.Parameters.AddWithValue("@active", true);
                command.Parameters.AddWithValue("@billingID", billingID);

                Ticket ticket = new Ticket();
                ticket.Body = content;
                ticket.Subject = subject;
                
                command.ExecuteScalar();

                res = ticket;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();

                if (command != null)
                    command.Dispose();
            }

            return res;
        }
    }
}