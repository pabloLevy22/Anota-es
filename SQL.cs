using System.Data.SqlClient;

public static class Sql
{
    private static string conexão = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Anotações;Integrated Security=True;";

    public static bool VerificaQuantosRegistroTem(string instrução)
    {
        using (SqlConnection connection = new SqlConnection(conexão))
        {
            connection.Open();

            SqlCommand command = new SqlCommand(instrução, connection);

            int numeroDoRegistro = Convert.ToInt32(command.ExecuteScalar());

            return numeroDoRegistro == 0 ? false : true;
        }
    }

    public static void Atualização(string instrução)
    {
        using (SqlConnection? servido = new SqlConnection(conexão))
        {
            servido.Open();

            SqlCommand command = new SqlCommand(instrução, servido);
            command.ExecuteNonQuery();
        }
    }

    public static IEnumerable<object[]> Consultar(string instrução)
    {
        using (SqlConnection? servido = new SqlConnection(conexão))
        {
            servido.Open();

            SqlCommand command = new SqlCommand(instrução, servido);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                object[] retorno = new object[reader.FieldCount];

                reader.GetValues(retorno);

                yield return retorno;
            }

        }
    }

    public static bool VerificaExistenciaBanco()
    {
        using (var connection = new SqlConnection(conexão))
        {
            connection.Open();
            string query = "SELECT COUNT(*) FROM sys.databases WHERE name = 'Anotações'";
            SqlCommand command = new SqlCommand(query, connection);
            int count = (int)command.ExecuteScalar();
            return count > 0;
        }
    }

    public static void CreateDatabase()
    {
        using (var connection = new SqlConnection(conexão))
        {
            connection.Open();
            string query = "CREATE DATABASE Anotações";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }

    public static void CreateTable()
    {
        using (var connection = new SqlConnection(conexão))
        {
            connection.Open();
            string query = "CREATE TABLE Anotações (ID INT PRIMARY KEY, Nome NVARCHAR(50), Texto VARCHAR(MAX), Data DATE)";
            SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }
    }
}

public static class TabelaAnotações
{
    public const string consultar = "SELECT ID,Nome,Texto,Data FROM Anotação";
    public const string VerificaQuantosRegistroTem = "SELECT COUNT(*) FROM Anotação";

    public enum Coluna { ID, Nome, Texto, Data }
}