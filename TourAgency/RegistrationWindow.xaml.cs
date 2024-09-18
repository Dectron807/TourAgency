using System;
using System.Data.SqlClient;
using System.Windows;

namespace TourAgency
{
    public partial class RegistrationWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=db_1;Integrated Security=True";
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void AlreadyRegisteredButton_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
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

                    string query = "INSERT INTO Пользователи (Логин, Пароль) VALUES (@Логин, @Пароль)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Логин", username);
                    command.Parameters.AddWithValue("@Пароль", HashPassword(password));

                    command.ExecuteNonQuery();
                    MessageBox.Show("Регистрация прошла успешно!");

                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}\n{ex.StackTrace}");
            }
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
