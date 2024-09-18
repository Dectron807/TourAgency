using System;
using System.Data.SqlClient;
using System.Windows;

namespace TourAgency
{
    public partial class LoginWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=db_1;Integrated Security=True";
        public static int CurrentUserId { get; private set; }
        public static string CurrentUserName { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин и пароль не могут быть пустыми.");
                return;
            }


            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT id, Логин, Пароль FROM Пользователи WHERE Логин = @Логин";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Логин", username);

                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        string storedPassword = reader["Пароль"].ToString();
                        if (storedPassword == HashPassword(password))
                        {
                            CurrentUserId = (int)reader["id"];
                            CurrentUserName = reader["Логин"].ToString();
                            MessageBox.Show("Вход выполнен успешно!");
                            MainWindow mainWindow = new MainWindow();
                            mainWindow.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Неправильный пароль.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пользователь не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
