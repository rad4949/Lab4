using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Default;

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            int number;

            do
            {
                Console.WriteLine("\n1 - INSERT");
                Console.WriteLine("2 - UPDATE");
                Console.WriteLine("3 - DELETE");
                Console.WriteLine("4 - SELECT");
                Console.WriteLine("5 - ReturnNumberCode");
                Console.WriteLine("6 - ReturnInfoCode");
                Console.WriteLine("7 - TransactionInsert");

                Console.WriteLine("0 - Вихід");
                Console.Write("Введіть номер: ");
                number = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("-------------------\n");

                switch (number)
                {
                    case 1:
                        InsertCommand();
                        break;

                    case 2:
                        UpdateCommand();
                        break;

                    case 3:
                        DeleteCommand();
                        break;

                    case 4:
                        SelectAllCommand();
                        break;

                    case 5:
                        ReturnNumberCommand();
                        break;

                    case 6:
                        Console.Write("Введіть Id: ");
                        int id = Convert.ToInt32(Console.ReadLine());
                        GetInfoColorCommand(id);
                        break;

                    case 7:
                        InsertTransactionCommand();
                        break;

                    case 0:
                        Console.WriteLine("Ви вийшли!");
                        break;
                }

            }
            while (number != 0);

        }

        static void InsertCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "INSERT INTO Colors (TypeOfCoating, ColorCode, ColorName) VALUES ('Глянцеве', '487', 'Лагуна')";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int amount = command.ExecuteNonQuery();
                Console.WriteLine("Додано об'єктів: {0}", amount);
            }
        }

        static void UpdateCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "UPDATE Colors SET ColorName='Лагуна UPDATE' WHERE ColorCode='487' AND TypeOfCoating='Глянцеве'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int amount = command.ExecuteNonQuery();
                Console.WriteLine("Оновлено об'єктів: {0}", amount);
            }

        }

        static void DeleteCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string sqlExpression = "DELETE FROM Colors WHERE ColorCode='487' AND TypeOfCoating='Глянцеве'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                int amount = command.ExecuteNonQuery();
                Console.WriteLine("Видалено об'єктів: {0}", amount);
            }
        }

        static void SelectAllCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "SELECT * FROM Colors";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    Console.WriteLine("{0,2} {1,15} {2,15} {3,15}", reader.GetName(0), reader.GetName(1), reader.GetName(2), reader.GetName(3));

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string TypeOfCoating = reader.GetString(1);
                        string ColorCode = reader.GetString(2);
                        string ColorName = reader.GetString(3);

                        string str = String.Format("{0,2} {1,15} {2,15} {3,15}"
                            , id, TypeOfCoating, ColorCode, ColorName);

                        Console.WriteLine(str);
                    }
                }
                reader.Close();
            }
        }

        static void ReturnNumberCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string sqlExpression = "SELECT COUNT(*) FROM Colors";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                object count = command.ExecuteScalar();

                command.CommandText = "SELECT MIN(ColorCode) FROM Colors";
                object minColorCode = command.ExecuteScalar();

                Console.WriteLine("В таблиці {0} об'єктів", count);
                Console.WriteLine("Мінімальний шифр коду: {0}", minColorCode);
            }
        }

        static void GetInfoColorCommand(int id)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string sqlExpression = "getInfoColor";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                command.CommandType = CommandType.StoredProcedure;

                SqlParameter Id = new SqlParameter
                {
                    ParameterName = "@id",
                    Value = id
                };
                command.Parameters.Add(Id);

                SqlParameter ColorCodeParam = new SqlParameter("@ColorCode", SqlDbType.NVarChar, 30);               
                ColorCodeParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(ColorCodeParam);

                SqlParameter ColorNameParam = new SqlParameter("@ColorName", SqlDbType.NVarChar, 30);               
                ColorNameParam.Direction = ParameterDirection.Output;
                command.Parameters.Add(ColorNameParam);

                command.ExecuteScalar();

                Console.WriteLine("Код кольору: {0}", command.Parameters["@ColorCode"].Value);
                Console.WriteLine("Назва кольору: {0}", command.Parameters["@ColorName"].Value);
            }
        }

        static void InsertTransactionCommand()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                SqlCommand command = connection.CreateCommand();
                command.Transaction = transaction;

                try
                {
                    command.CommandText = "INSERT INTO Colors (TypeOfCoating, ColorCode, ColorName) VALUES ('Глянцеве', '487', 'Лагуна 1')";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Colors (TypeOfCoating, ColorCode, ColorName) VALUES ('Глянцеве', '487', 'Лагуна 2')"; 
                    command.ExecuteNonQuery();

                    transaction.Commit();
                    Console.WriteLine("Дані додані до бази!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                }
            }
        }
    }
}
