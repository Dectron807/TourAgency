using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Diagnostics;
using System.Windows.Navigation;
using Paragraph = iTextSharp.text.Paragraph;

namespace TourAgency
{
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=DESKTOP-0JE8UH5\\SQLEXPRESS;Initial Catalog=db_1;Integrated Security=True";
        int ticketCount = 1;
        public MainWindow()
        {
            InitializeComponent();
            LoadTours();
            LoadMyTours();
            LoadHistory();
        }

        private void LoadTours()
        {
            string query = @"
                SELECT t.id, h.Название as Название_Отеля, s.Название as Название_Страны, 
                       t.Дата_Начала, t.Дата_Окончания, t.Стоимость_Услуг_Компании, t.Количество_Свободных_Мест
                FROM Тур t
                JOIN Отель h ON t.Отель_id = h.id
                JOIN Страна s ON t.Страна_id = s.id";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var tours = new List<object>();
                while (reader.Read())
                {
                    tours.Add(new
                    {
                        id = reader["id"],
                        Название_Отеля = reader["Название_Отеля"],
                        Название_Страны = reader["Название_Страны"],
                        Дата_Начала = reader["Дата_Начала"],
                        Дата_Окончания = reader["Дата_Окончания"],
                        Стоимость_Услуг_Компании = reader["Стоимость_Услуг_Компании"],
                        Количество_Свободных_Мест = reader["Количество_Свободных_Мест"],
                        Описание = $"{reader["Название_Отеля"]}, {reader["Название_Страны"]} ({((DateTime)reader["Дата_Начала"]).ToString("d")} - {((DateTime)reader["Дата_Окончания"]).ToString("d")})"
                    });
                }

                ToursDataGrid.ItemsSource = tours;
                TourComboBox.ItemsSource = tours;
            }
        }

        private void LoadMyTours()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT МоиТуры.id, МоиТуры.Клиент_id, МоиТуры.Тур_id, МоиТуры.Дата_Заключения, 
                       Отель.Название AS Название_Отеля, Страна.Название AS Название_Страны
                FROM МоиТуры
                JOIN Тур ON МоиТуры.Тур_id = Тур.id
                JOIN Отель ON Тур.Отель_id = Отель.id
                JOIN Страна ON Тур.Страна_id = Страна.id
                WHERE МоиТуры.Логин = @Логин";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Логин", LoginWindow.CurrentUserName);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable myToursTable = new DataTable();
                    adapter.Fill(myToursTable);

                    MyToursDataGrid.ItemsSource = myToursTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке туров: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void CancelTourButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyToursDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите тур для отмены.");
                return;
            }

            var selectedTour = (dynamic)MyToursDataGrid.SelectedItem;
            int tourId = (int)selectedTour["Тур_id"];
            int clientId = (int)selectedTour["Клиент_id"];

            MessageBoxResult result = MessageBox.Show($"Вы уверены, что хотите отменить тур с ID {tourId}?", "Подтверждение отмены", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                CancelTour(clientId, tourId, ticketCount);
            }
        }

        private void CancelTour(int clientId, int tourId, int ticketCount)
        {
            string deleteMyTourQuery = "DELETE FROM МоиТуры WHERE Клиент_id = @ClientId AND Тур_id = @TourId";
            string deleteTouristQuery = "DELETE FROM Турист WHERE Клиент_id = @ClientId AND Договор_id IN (SELECT id FROM Договор WHERE Тур_id = @TourId)";
            string deleteContractQuery = "DELETE FROM Договор WHERE Тур_id = @TourId AND id IN (SELECT Договор_id FROM Турист WHERE Клиент_id = @ClientId)";
            string updateTourSeatsQuery = "UPDATE Тур SET Количество_Свободных_Мест = Количество_Свободных_Мест + @TicketCount WHERE id = @TourId";
            string insertHistoryQuery = @"
        INSERT INTO История (Логин, Клиент_id, Тур_id, Дата_Операции, Операция, Количество_Билетов) 
        VALUES (@Login, @ClientId, @TourId, @OperationDate, @Operation, @TicketCount)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand deleteMyTourCommand = new SqlCommand(deleteMyTourQuery, connection);
                deleteMyTourCommand.Parameters.AddWithValue("@ClientId", clientId);
                deleteMyTourCommand.Parameters.AddWithValue("@TourId", tourId);

                SqlCommand deleteTouristCommand = new SqlCommand(deleteTouristQuery, connection);
                deleteTouristCommand.Parameters.AddWithValue("@ClientId", clientId);
                deleteTouristCommand.Parameters.AddWithValue("@TourId", tourId);

                SqlCommand deleteContractCommand = new SqlCommand(deleteContractQuery, connection);
                deleteContractCommand.Parameters.AddWithValue("@ClientId", clientId);
                deleteContractCommand.Parameters.AddWithValue("@TourId", tourId);

                SqlCommand updateTourSeatsCommand = new SqlCommand(updateTourSeatsQuery, connection);
                updateTourSeatsCommand.Parameters.AddWithValue("@TicketCount", ticketCount);
                updateTourSeatsCommand.Parameters.AddWithValue("@TourId", tourId);

                SqlCommand insertHistoryCommand = new SqlCommand(insertHistoryQuery, connection);
                insertHistoryCommand.Parameters.AddWithValue("@Login", LoginWindow.CurrentUserName);
                insertHistoryCommand.Parameters.AddWithValue("@ClientId", clientId);
                insertHistoryCommand.Parameters.AddWithValue("@TourId", tourId);
                insertHistoryCommand.Parameters.AddWithValue("@OperationDate", DateTime.Now);
                insertHistoryCommand.Parameters.AddWithValue("@Operation", "Отмена");
                insertHistoryCommand.Parameters.AddWithValue("@TicketCount", ticketCount);

                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                deleteMyTourCommand.Transaction = transaction;
                deleteTouristCommand.Transaction = transaction;
                deleteContractCommand.Transaction = transaction;
                updateTourSeatsCommand.Transaction = transaction;
                insertHistoryCommand.Transaction = transaction;

                try
                {
                    insertHistoryCommand.ExecuteNonQuery();
                    deleteMyTourCommand.ExecuteNonQuery();
                    deleteTouristCommand.ExecuteNonQuery();
                    deleteContractCommand.ExecuteNonQuery();
                    updateTourSeatsCommand.ExecuteNonQuery();

                    transaction.Commit();
                    MessageBox.Show("Тур успешно отменен, денежные средства вернутся вам в течение 2-х рабочих дней.");
                    LoadMyTours();
                    LoadTours();
                    LoadHistory();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Отмена тура не удалась: {ex.Message}");
                }
            }
        }

        private void TourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTour = (dynamic)TourComboBox.SelectedItem;
            if (selectedTour == null) return;

            int availableSeats = selectedTour.Количество_Свободных_Мест;
        }

        private void BookTourButton_Click(object sender, RoutedEventArgs e)
        {
            if (TourComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите тур.");
                return;
            }

            string surname = SurnameTextBox.Text;
            string name = NameTextBox.Text;
            string patronymic = PatronymicTextBox.Text;
            string gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            string passportNumber = PassportNumberTextBox.Text;
            DateTime? birthDate = BirthDatePicker.SelectedDate;

            if (string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(passportNumber) || birthDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            var selectedTour = (dynamic)TourComboBox.SelectedItem;
            int tourId = selectedTour.id;
            int availableSeats = selectedTour.Количество_Свободных_Мест;

            if (availableSeats == 0)
            {
                MessageBox.Show("Недостаточно свободных мест для оформления выбранного количества билетов.");
                return;
            }


            string findClientQuery = @"
SELECT id 
FROM Клиент 
WHERE Номер_Паспорта = @PassportNumber";

            string insertClientQuery = @"
INSERT INTO Клиент (Фамилия, Имя, Отчество, Пол, Номер_Паспорта, Дата_Рождения) 
OUTPUT INSERTED.id 
VALUES (@Surname, @Name, @Patronymic, @Gender, @PassportNumber, @BirthDate)";

            string insertContractQuery = @"
INSERT INTO Договор (Тур_id, Дата_Заключения) 
OUTPUT INSERTED.id 
VALUES (@TourId, @ContractDate)";

            string insertTouristQuery = @"
INSERT INTO Турист (Клиент_id, Договор_id) 
VALUES (@ClientId, @ContractId)";

            string insertMyTourQuery = @"
INSERT INTO МоиТуры (Логин, Клиент_id, Тур_id, Дата_Заключения, Количество_Билетов) 
VALUES (@Login, @ClientId, @TourId, @ContractDate, @TicketCount)";

            string updateTourSeatsQuery = @"
UPDATE Тур 
SET Количество_Свободных_Мест = Количество_Свободных_Мест - @TicketCount 
WHERE id = @TourId";

            string insertHistoryQuery = @"
INSERT INTO История (Логин, Клиент_id, Тур_id, Дата_Операции, Операция, Количество_Билетов) 
VALUES (@Login, @ClientId, @TourId, @OperationDate, @Operation, @TicketCount)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    SqlCommand findClientCommand = new SqlCommand(findClientQuery, connection, transaction);
                    findClientCommand.Parameters.AddWithValue("@PassportNumber", passportNumber);

                    object clientResult = findClientCommand.ExecuteScalar();
                    int clientId;

                    if (clientResult != null)
                    {
                        clientId = (int)clientResult;
                    }
                    else
                    {
                        SqlCommand insertClientCommand = new SqlCommand(insertClientQuery, connection, transaction);
                        insertClientCommand.Parameters.AddWithValue("@Surname", surname);
                        insertClientCommand.Parameters.AddWithValue("@Name", name);
                        insertClientCommand.Parameters.AddWithValue("@Patronymic", patronymic ?? (object)DBNull.Value);
                        insertClientCommand.Parameters.AddWithValue("@Gender", gender);
                        insertClientCommand.Parameters.AddWithValue("@PassportNumber", passportNumber);
                        insertClientCommand.Parameters.AddWithValue("@BirthDate", birthDate.Value);

                        clientId = (int)insertClientCommand.ExecuteScalar();
                    }

                    SqlCommand insertContractCommand = new SqlCommand(insertContractQuery, connection, transaction);
                    insertContractCommand.Parameters.AddWithValue("@TourId", tourId);
                    insertContractCommand.Parameters.AddWithValue("@ContractDate", DateTime.Now);
                    int contractId = (int)insertContractCommand.ExecuteScalar();

                    SqlCommand insertTouristCommand = new SqlCommand(insertTouristQuery, connection, transaction);
                    insertTouristCommand.Parameters.AddWithValue("@ClientId", clientId);
                    insertTouristCommand.Parameters.AddWithValue("@ContractId", contractId);
                    insertTouristCommand.ExecuteNonQuery();

                    SqlCommand insertMyTourCommand = new SqlCommand(insertMyTourQuery, connection, transaction);
                    insertMyTourCommand.Parameters.AddWithValue("@Login", LoginWindow.CurrentUserName);
                    insertMyTourCommand.Parameters.AddWithValue("@ClientId", clientId);
                    insertMyTourCommand.Parameters.AddWithValue("@TourId", tourId);
                    insertMyTourCommand.Parameters.AddWithValue("@ContractDate", DateTime.Now);
                    insertMyTourCommand.Parameters.AddWithValue("@TicketCount", ticketCount);
                    insertMyTourCommand.ExecuteNonQuery();

                    SqlCommand updateTourSeatsCommand = new SqlCommand(updateTourSeatsQuery, connection, transaction);
                    updateTourSeatsCommand.Parameters.AddWithValue("@TicketCount", ticketCount);
                    updateTourSeatsCommand.Parameters.AddWithValue("@TourId", tourId);
                    updateTourSeatsCommand.ExecuteNonQuery();

                    SqlCommand insertHistoryCommand = new SqlCommand(insertHistoryQuery, connection, transaction);
                    insertHistoryCommand.Parameters.AddWithValue("@Login", LoginWindow.CurrentUserName);
                    insertHistoryCommand.Parameters.AddWithValue("@ClientId", clientId);
                    insertHistoryCommand.Parameters.AddWithValue("@TourId", tourId);
                    insertHistoryCommand.Parameters.AddWithValue("@OperationDate", DateTime.Now);
                    insertHistoryCommand.Parameters.AddWithValue("@Operation", "Оформление");
                    insertHistoryCommand.Parameters.AddWithValue("@TicketCount", ticketCount);
                    insertHistoryCommand.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show("Тур успешно оформлен!");
                    LoadTours();
                    LoadMyTours();
                    LoadHistory();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка при оформлении тура: {ex.Message}");
                }
            }
        }

        private void LoadHistory()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT Тур_id, Дата_Операции, Операция
                FROM История
                WHERE Логин = @Логин";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Логин", LoginWindow.CurrentUserName);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable historyTable = new DataTable();
                    adapter.Fill(historyTable);

                    HistoryDataGrid.ItemsSource = historyTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории операций: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DownloadPdfButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyToursDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите тур для скачивания.");
                return;
            }

            var selectedTour = (DataRowView)MyToursDataGrid.SelectedItem;
            int tourId = Convert.ToInt32(selectedTour["Тур_id"]);
            int clientId = Convert.ToInt32(selectedTour["Клиент_id"]);

            var clientInfo = GetClientInfo(clientId);
            if (clientInfo == null)
            {
                MessageBox.Show("Не удалось получить информацию о клиенте.");
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                DefaultExt = "pdf",
                FileName = $"Tour_{tourId}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;

                CreatePdf(selectedTour, filePath, clientInfo.FullName, clientInfo.PassportNumber);

                MessageBox.Show("PDF файл успешно создан и сохранен.");

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
        }

        private void CreatePdf(DataRowView selectedTour, string filePath, string fullName, string passportNumber)
        {
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                var document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                try
                {
                    var backgroundImage = iTextSharp.text.Image.GetInstance("C:\\Users\\Book_dom\\Desktop\\ацу\\TourAgency\\TourAgency\\images\\363561f0db7fb3b35ed241d3a6e9fabf.jpeg");
                    backgroundImage.ScaleAbsolute(document.PageSize.Width, document.PageSize.Height / 3);
                    backgroundImage.SetAbsolutePosition(0, document.PageSize.Height - backgroundImage.ScaledHeight);
                    document.Add(backgroundImage);

                    PdfContentByte cb = writer.DirectContent;
                    cb.BeginText();

                    var baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arialbd.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    float fontSize = 16; 
                    cb.SetFontAndSize(baseFont, fontSize);
                    cb.SetColorFill(BaseColor.BLACK);

                    float topMargin = document.PageSize.Height - 40;
                    float lineHeight = fontSize + 6; 
                    cb.ShowTextAligned(Element.ALIGN_LEFT, $"ФИО: {fullName}", 20, topMargin, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, $"Номер паспорта: {passportNumber}", 20, topMargin - lineHeight, 0);

                    float bottomMargin = document.PageSize.Height - backgroundImage.ScaledHeight + 70; 
                    cb.ShowTextAligned(Element.ALIGN_LEFT, $"Тур ID: {selectedTour["Тур_id"]}", 20, bottomMargin, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, $"Отель: {selectedTour["Название_Отеля"]}", 20, bottomMargin - lineHeight, 0);
                    cb.ShowTextAligned(Element.ALIGN_LEFT, $"Дата оформления: {selectedTour["Дата_Заключения"]}", 20, bottomMargin - 2 * lineHeight, 0);

                    cb.EndText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании PDF: {ex.Message}");
                }
                finally
                {
                    document.Close();
                    writer.Close();
                }
            }
        }

        private ClientInfo GetClientInfo(int clientId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Фамилия, Имя, Отчество, Номер_Паспорта FROM Клиент WHERE id = @ClientId";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClientId", clientId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        return new ClientInfo
                        {
                            FullName = $"{reader["Фамилия"]} {reader["Имя"]} {reader["Отчество"]}",
                            PassportNumber = reader["Номер_Паспорта"].ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при получении информации о клиенте: " + ex.Message);
                }
            }

            return null;
        }

        private class ClientInfo
        {
            public string FullName { get; set; }
            public string PassportNumber { get; set; }
        }

        private void MyToursDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyToursDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите путевку для распечатки.");
                return;
            }

            var selectedTicket = (dynamic)MyToursDataGrid.SelectedItem;
            int tourId = (int)selectedTicket["Тур_id"];
            int clientId = (int)selectedTicket["Клиент_id"];

        }

        private void SaveToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Сохранить PDF файл",
                FileName = "Доступные_туры.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                CreatePdfFromToursDataGrid(filePath);
            }
        }

        private void CreatePdfFromToursDataGrid(string filePath)
        {
            DataTable toursData = GetToursDataForPdf();

            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                var document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                try
                {
                    string fontPath = "C:\\Windows\\Fonts\\arialbd.ttf";
                    var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var font = new iTextSharp.text.Font(baseFont, 12);
                    var titleFont = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);

                    var titleParagraph = new Paragraph("Доступные туры", titleFont);
                    titleParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(titleParagraph);
                    document.Add(new Paragraph("\n"));

                    PdfPTable table = new PdfPTable(toursData.Columns.Count);
                    table.WidthPercentage = 100;

                    foreach (DataColumn column in toursData.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5
                        };
                        table.AddCell(cell);
                    }

                    foreach (DataRow row in toursData.Rows)
                    {
                        foreach (var cell in row.ItemArray)
                        {
                            PdfPCell pdfCell = new PdfPCell(new Phrase(cell?.ToString() ?? string.Empty, font))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 5
                            };
                            table.AddCell(pdfCell);
                        }
                    }

                    document.Add(table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании PDF: {ex.Message}");
                }
                finally
                {
                    document.Close();
                    writer.Close();
                }

                System.Diagnostics.Process.Start(filePath);
            }
        }

        private DataTable GetToursDataForPdf()
        {
            DataTable toursTable = new DataTable();

            foreach (DataGridColumn column in ToursDataGrid.Columns)
            {
                toursTable.Columns.Add(column.Header.ToString());
            }

            foreach (var item in ToursDataGrid.Items)
            {
                DataRow newRow = toursTable.NewRow();
                foreach (DataGridColumn column in ToursDataGrid.Columns)
                {
                    var cellContent = column.GetCellContent(item);
                    if (cellContent is TextBlock textBlock)
                    {
                        newRow[column.DisplayIndex] = textBlock.Text;
                    }
                }
                toursTable.Rows.Add(newRow);
            }

            return toursTable;
        }

        private void SaveToPdfButton_Click1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                Title = "Сохранить PDF файл",
                FileName = "История_операций.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                CreatePdfFromHistoryDataGrid(filePath);
            }
        }

        private void CreatePdfFromHistoryDataGrid(string filePath)
        {
            DataTable historyData = GetHistoryDataForPdf();

            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                var document = new iTextSharp.text.Document();
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                try
                {
                    string fontPath = "C:\\Windows\\Fonts\\arialbd.ttf"; 
                    var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    var font = new iTextSharp.text.Font(baseFont, 12);
                    var titleFont = new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD);

                    var titleParagraph = new Paragraph("История операций", titleFont);
                    titleParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(titleParagraph);
                    document.Add(new Paragraph("\n"));

                    PdfPTable table = new PdfPTable(historyData.Columns.Count);
                    table.WidthPercentage = 100;

                    foreach (DataColumn column in historyData.Columns)
                    {
                        PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, font))
                        {
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            VerticalAlignment = Element.ALIGN_MIDDLE,
                            Padding = 5
                        };
                        table.AddCell(cell);
                    }

                    foreach (DataRow row in historyData.Rows)
                    {
                        foreach (var cell in row.ItemArray)
                        {
                            PdfPCell pdfCell = new PdfPCell(new Phrase(cell?.ToString() ?? string.Empty, font))
                            {
                                HorizontalAlignment = Element.ALIGN_CENTER,
                                VerticalAlignment = Element.ALIGN_MIDDLE,
                                Padding = 5
                            };
                            table.AddCell(pdfCell);
                        }
                    }

                    document.Add(table);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании PDF: {ex.Message}");
                }
                finally
                {
                    document.Close();
                    writer.Close();
                }

                System.Diagnostics.Process.Start(filePath);
            }
        }

        private DataTable GetHistoryDataForPdf()
        {
            DataTable historyTable = new DataTable();

            foreach (DataGridColumn column in HistoryDataGrid.Columns)
            {
                historyTable.Columns.Add(column.Header.ToString());
            }

            foreach (var item in HistoryDataGrid.Items)
            {
                DataRowView rowView = item as DataRowView;
                if (rowView != null)
                {
                    DataRow newRow = historyTable.NewRow();
                    for (int i = 0; i < HistoryDataGrid.Columns.Count; i++)
                    {
                        newRow[i] = rowView[i];
                    }
                    historyTable.Rows.Add(newRow);
                }
            }

            return historyTable;
        }

        private void ShowTotalCostButton_Click(object sender, RoutedEventArgs e)
        {
            int totalTours = 0;
            decimal totalCost = 0;

            Dictionary<int, decimal> tourPrices = new Dictionary<int, decimal>();

            foreach (var item in ToursDataGrid.Items)
            {
                if (item is DataRowView rowView)
                {
                    int tourId = Convert.ToInt32(rowView["id"]);
                    decimal tourPrice = Convert.ToDecimal(rowView["Стоимость_Услуг_Компании"]);
                    tourPrices[tourId] = tourPrice;
                }
                else
                {
                    var properties = item.GetType().GetProperties();
                    var idProperty = properties.FirstOrDefault(p => p.Name == "id");
                    var priceProperty = properties.FirstOrDefault(p => p.Name == "Стоимость_Услуг_Компании");

                    if (idProperty != null && priceProperty != null)
                    {
                        int tourId = Convert.ToInt32(idProperty.GetValue(item, null));
                        decimal tourPrice = Convert.ToDecimal(priceProperty.GetValue(item, null));
                        tourPrices[tourId] = tourPrice;
                    }
                }
            }

            foreach (var item in MyToursDataGrid.Items)
            {
                if (item is DataRowView rowView)
                {
                    int tourId = Convert.ToInt32(rowView["Тур_id"]);
                    if (tourPrices.ContainsKey(tourId))
                    {
                        totalTours++;
                        totalCost += tourPrices[tourId];
                    }
                }
                else
                {
                    var properties = item.GetType().GetProperties();
                    var tourIdProperty = properties.FirstOrDefault(p => p.Name == "Тур_id");

                    if (tourIdProperty != null)
                    {
                        int tourId = Convert.ToInt32(tourIdProperty.GetValue(item, null));
                        if (tourPrices.ContainsKey(tourId))
                        {
                            totalTours++;
                            totalCost += tourPrices[tourId];
                        }
                    }
                }
            }

            MessageBox.Show($"Всего у вас оформлено {totalTours} туров на сумму {totalCost:C}.", "Стоимость моих туров");
        }

        public class Airline
        {
            public string Name { get; set; }
        }

        private void OnAboutButtonClick(object sender, RoutedEventArgs e)
        {
            List<Airline> airlines = GetAirlinesFromDatabase();

            StringBuilder message = new StringBuilder();
            message.AppendLine("Мы - DTF-tours, одно из ведущих турагентств в мире.");
            message.AppendLine("Наш годовой человекопоток достигает 100 млн. человек.");
            message.AppendLine("Мы сотрудничаем с ведущими мировыми авиакомпаниями, такими как:");

            foreach (var airline in airlines)
            {
                message.AppendLine($"- {airline.Name}");
            }

            ShowAboutDialog(message.ToString());
        }

        private List<Airline> GetAirlinesFromDatabase()
        {
            List<Airline> airlines = new List<Airline>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand("SELECT Название FROM Авиакомпании", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            airlines.Add(new Airline { Name = reader.GetString(0) });
                        }
                    }
                }
            }

            return airlines;
        }

        private void ShowAboutDialog(string message)
        {
            Window aboutWindow = new Window
            {
                Title = "О нас",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Content = new ScrollViewer
                {
                    Content = new StackPanel
                    {
                        Margin = new Thickness(10),
                        Children =
                {
                    new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 10) },
                    new TextBlock
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Inlines =
                        {
                            new Run("Если у вас возникнут проблемы с нашим приложением - напишите нам в телеграмм "),
                            CreateHyperlink("Dectron", "https://t.me/Dectron"),
                            new Run(".")
                        }
                    }
                }
                    }
                }
            };

            aboutWindow.ShowDialog();
        }

        private Hyperlink CreateHyperlink(string linkText, string navigateUri)
        {
            Hyperlink hyperlink = new Hyperlink(new Run(linkText))
            {
                NavigateUri = new Uri(navigateUri)
            };
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
            return hyperlink;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

    }
}