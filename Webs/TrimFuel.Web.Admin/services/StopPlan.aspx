<%@ Page Language="C#" %>

<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Web.Services" %>
<%@ Import Namespace="MySql.Data.MySqlClient" %>
<%@ Import Namespace="TrimFuel.Business" %>

<script runat="server">

    [WebMethod]
    public static string StopPlan(int id)
    {
        int adminID = GetCurrentAdminID();

        string query = @"
            update BillingSubscriptionPlan set IsActive=0 where BillingSubscriptionPlanID=@BillingSubscriptionPlanID;
            insert into Notes(BillingID, AdminID, Content, CreateDT) values ((
                select bs.BillingID from BillingSubscriptionPlan bsp
                left join BillingSubscription bs on bsp.BillingSubscriptionID = bs.BillingSubscriptionID
                where bsp.BillingSubscriptionPlanID = @BillingSubscriptionPlanID), @AdminID, @Content, NOW());";

                

        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = query;
            command.Parameters.AddWithValue("@BillingSubscriptionPlanID", id);
            command.Parameters.AddWithValue("@Content", "Loyalty Plan #" + id + " stopped");
            command.Parameters.AddWithValue("@AdminID", adminID);

            command.ExecuteNonQuery();
        }
        catch
        {
            return "Error";
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }

        return "Plan stopped";
    }

    private static int GetCurrentAdminID()
    {
        HttpCookie cookie = HttpContext.Current.Request.Cookies["admName4Net"];
        
        MySqlConnection connection = new MySqlConnection();
        MySqlCommand command = new MySqlCommand();

        try
        {
            connection.ConnectionString = (Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            connection.Open();

            command = new MySqlCommand();
            command.Connection = connection;
            command.CommandText = "select AdminID from Admin where DisplayName = @DisplayName and Active = 1";
            command.Parameters.AddWithValue("@DisplayName", cookie.Value);

            return Convert.ToInt32(command.ExecuteScalar().ToString());
        }
        catch
        {
            return 0;
        }
        finally
        {
            if (connection != null)
                connection.Close();

            if (command != null)
                command.Dispose();
        }
    }

</script>