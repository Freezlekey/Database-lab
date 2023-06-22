using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Xml.Schema;

namespace WindowsFormsApp8
{
    public partial class Form1 : Form // что в Form1 вообще есть
    {
        static string DBFilePath { get; set; } // полный путь к файлу Data_students (где на жёсткм диске файл с данными)
        static int GetIntFromString(string inputStr) //***
        {
            int input = 0;
            try
            {
                input = int.Parse(inputStr);
            }
            catch (FormatException)
            {

            }
            return input;
        }
        CultureInfo culture = CultureInfo.InvariantCulture; //Локальные настройки времени для стран
        string format = "MM-dd-yy";
        public Form1()
        {
            InitializeComponent();

            string fileDBName = "Data_students.txt"; // Имя создаваемого файла
            string fileFolderPath = Path.GetTempPath(); // Путь к папке с файлами
            DBFilePath = fileFolderPath + fileDBName;  // C:\Users\kalin\AppData\Local\Temp\Data_students
            if (File.Exists(DBFilePath) == false) // файл не существует
            {
                var file = File.Create(DBFilePath); // создаём файл
                file.Close(); // закрытие доступа к файлу, чтобы он больше не использовался, сделано во избежании ошибки в методе чтения файла
            }
            var allUsers = ReadAllFromDB();
            foreach (var user in allUsers) listBox1.Items.Add(user);
            dataGridView1.DataSource = allUsers;
            label23.Text = allUsers.Count.ToString();
            float t = 0;
            foreach (var user in allUsers)
            {
                t = t + user.AvgMArk;
            }
            float avg = t / (float)allUsers.Count;
            label25.Text = avg.ToString();
        }
        static List<User> ReadAllFromDB() // чтение всех юзеров, импорт в библиотеку
        {
            string json = File.ReadAllText(DBFilePath); // получаем текст на основе пути
            List<User> currentUsers = JsonConvert.DeserializeObject<List<User>>(json); //делаем десериализацию
            return currentUsers ?? new List<User>(); // это выбор из двух варинтов (существуют ли пользователи в файле?)
        }
        static List<User> SortedDB()
        {
            string json = File.ReadAllText(DBFilePath);
            List<User> currentusers = JsonConvert.DeserializeObject<List<User>>(json);
            currentusers = currentusers.OrderBy(u => u.AvgMArk).ToList();
            return currentusers ?? new List<User>();
        }
        static void SaveToDB(User user) // сохранение в базу данных, (User user)?, скачана библиотека Newtonsoft.Json (для работы с сериализацией и десериализацией данных), тут делаем сериализацию
        {
            List<User> allCurrentUsers = ReadAllFromDB(); // получаем всех пользователей
            int lastID = allCurrentUsers.Count; // Задаём новый ID пользователю ** int lastID = allCurrentUsers.Count == 0 ? 0 : allCurrentUsers.Last().ID;
            if (lastID == 0) // *Возвращаем новый ID* для нового пользователя ID + 1
            {
                lastID = 0;
            }
            else
            {
                lastID = allCurrentUsers.Last().ID;
            }
            user.SetNewID(lastID + 1);
            allCurrentUsers.Add(user); // Добавляем юзера
            string serializedUsers = JsonConvert.SerializeObject(allCurrentUsers); // делаем сериализацию, JsonConvert
            File.WriteAllText(DBFilePath, serializedUsers); //Записываем в путь
        }
        static void SaveToDB(List<User> users) // перегрузка метода, реализация записи нескольких элементов
        {
            string serializedUsers = JsonConvert.SerializeObject(users); // сохранение в базу
            File.WriteAllText(DBFilePath, serializedUsers); //Записываем в путь
        }
        static bool DeleteFromDB(int id) // удаление из базы данных, bool для ****
        {
            List<User> allCurrentUsers = ReadAllFromDB(); // получаем всех пользователей
            User userForDeletion = allCurrentUsers.FirstOrDefault(u => u.ID == id); //Находим id пользователя при помощи лямба выражения (FirstOrDefault, импортируем в библиотеку linq), проверяем, что id равен нашему id.
            bool result = false; // Удалили неуспешно ****
            if (userForDeletion != null) //null - пустая ссылка, которая не ссылается на объект.
            {
                allCurrentUsers.Remove(userForDeletion); //Заново делаем сохранение
                result = true; // Удалили успешно ****
                SaveToDB(allCurrentUsers);
            }
            return result;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            string surname = textBox2.Text;
            int day = int.Parse(textBox3.Text);
            int month = int.Parse(textBox9.Text);
            int year = int.Parse(textBox10.Text);
            DateTime date = new DateTime(year, month, day);
            string institute = textBox4.Text;
            string group = textBox5.Text;
            string course = textBox6.Text;
            float avgmark = float.Parse(textBox7.Text);
            User newUser = new User(0, name, surname, date, institute, group, course, avgmark); // перед тем как записать пользователя надо сделать вывод его ID, надо получать последний ID пользователя.
            SaveToDB(newUser);
            var allUsers = ReadAllFromDB();
            dataGridView1.DataSource = allUsers;
            label23.Text = allUsers.Count.ToString();
            float t = 0;
            foreach (var user in allUsers)
            {
                t = t + user.AvgMArk;
            }
            float avg = t / (float)allUsers.Count;
            label25.Text = avg.ToString();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            string idStr = textBox8.Text;
            int id = GetIntFromString(idStr); //метод проверки вводимой переменной ***
            DeleteFromDB(id);
            listBox1.Items.Clear();
            var allUsers = ReadAllFromDB();
            foreach (var user in allUsers) listBox1.Items.Add(user);
            dataGridView1.DataSource = allUsers;
            label23.Text = allUsers.Count.ToString();
            float t = 0;
            foreach (var user in allUsers)
            {
                t = t + user.AvgMArk;
            }
            float avg = t / (float)allUsers.Count;
            label25.Text = avg.ToString();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var allUsers = SortedDB();
            dataGridView1.ClearSelection();
            if (allUsers.Count() == 0) Console.WriteLine("База данных пустая");
            foreach (var user in allUsers) listBox1.Items.Add(user);
            dataGridView1.DataSource = allUsers;
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            textBox8.Visible = false;
            button4.Visible = false;
            label10.Visible = false;
            textBox9.Visible = false;
            textBox10.Visible = false;
            label1.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
            textBox18.Visible = false;
            textBox19.Visible = false;
            textBox20.Visible = false;
            listBox2.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button15.Visible = false;
            button16.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label2.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            listBox1.Visible = true;
            button17.Visible = false;
            textBox21.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            button19.Visible = false;
            button20.Visible = false;
            label22.Visible = false;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Visible = true;
            textBox2.Visible = true;
            textBox3.Visible = true;
            textBox4.Visible = true;
            textBox5.Visible = true;
            textBox6.Visible = true;
            textBox7.Visible = true;
            button2.Visible = true;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label7.Visible = true;
            label8.Visible = true;
            label9.Visible = true;
            textBox8.Visible = false;
            button4.Visible = false;
            label10.Visible = false;
            textBox9.Visible = true;
            textBox10.Visible = true;
            label1.Visible = true;
            textBox11.Visible = false;
            textBox12.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
            textBox18.Visible = false;
            textBox19.Visible = false;
            textBox20.Visible = false;
            listBox2.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button15.Visible = false;
            button16.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label2.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            listBox1.Visible = true;
            button17.Visible = false;
            textBox21.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            button19.Visible = false;
            button20.Visible = false;
            label22.Visible = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            textBox8.Visible = true;
            button4.Visible = true;
            label10.Visible = true;
            textBox9.Visible = false;
            textBox10.Visible = false;
            label1.Visible = true;
            textBox11.Visible = false;
            textBox12.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
            textBox18.Visible = false;
            textBox19.Visible = false;
            textBox20.Visible = false;
            listBox2.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button15.Visible = false;
            button16.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label2.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            listBox1.Visible = true;
            button17.Visible = false;
            textBox21.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            button19.Visible = false;
            button20.Visible = false;
            label22.Visible = false;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            var allUsers = ReadAllFromDB(); // Получим всех пользователей
            listBox1.Items.Clear();
            dataGridView1 t = new dataGridView1();
            dataGridView1.DataSource = allUsers;
            for (int i = 0; i < allUsers.Count; i++)
            {
                listBox1.Items.Add(allUsers[i].ToString());
            }
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            textBox8.Visible = false;
            button4.Visible = false;
            label10.Visible = false;
            textBox9.Visible = false;
            textBox10.Visible = false;
            label1.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
            textBox18.Visible = false;
            textBox19.Visible = false;
            textBox20.Visible = false;
            listBox2.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button15.Visible = false;
            button16.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label2.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            listBox1.Visible = true;
            button17.Visible = false;
            textBox21.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            button19.Visible = false;
            button20.Visible = false;
            label22.Visible = false;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            textBox8.Visible = false;
            button4.Visible = false;
            label10.Visible = false;
            textBox9.Visible = false;
            textBox10.Visible = false;
            label1.Visible = false;
            label2.Visible = true;
            textBox11.Visible = true;
            textBox12.Visible = true;
            textBox13.Visible = true;
            textBox14.Visible = true;
            textBox15.Visible = true;
            textBox16.Visible = true;
            textBox17.Visible = true;
            textBox18.Visible = true;
            textBox19.Visible = true;
            textBox20.Visible = true;
            listBox2.Visible = true;
            button9.Visible = true;
            button10.Visible = true;
            button11.Visible = true;
            button12.Visible = true;
            button13.Visible = true;
            button14.Visible = true;
            button15.Visible = true;
            button16.Visible = true;
            label11.Visible = true;
            label12.Visible = true;
            label13.Visible = true;
            label14.Visible = true;
            label15.Visible = true;
            label16.Visible = true;
            label17.Visible = true;
            label18.Visible = true;
            listBox1.Visible = false;
            button17.Visible = false;
            textBox21.Visible = false;
            label20.Visible = false;
            label21.Visible = false;
            button19.Visible = true;
            button20.Visible = true;
            label22.Visible = false;
        }
        //Начало поиска
        private void button9_Click(object sender, EventArgs e)
        {
            string idStr = textBox11.Text;
            int id = GetIntFromString(idStr); //метод проверки вводимой переменной ***
            listBox2.Items.Clear();
            var allUsers = ReadAllFromDB();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.ID == id)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button10_Click(object sender, EventArgs e)
        {
            string name = textBox12.Text;
            listBox2.Items.Clear();
            var allUsers = ReadAllFromDB();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.Name.ToLower() == name.ToLower())
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            string surname = textBox13.Text;
            listBox2.Items.Clear();
            var allUsers = ReadAllFromDB();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.Surname.ToLower() == surname.ToLower())
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button12_Click(object sender, EventArgs e)
        {
            int day = int.Parse(textBox14.Text);
            int month = int.Parse(textBox15.Text);
            int year = int.Parse(textBox16.Text);
            var allUsers = ReadAllFromDB();
            DateTime date = new DateTime(year, month, day);
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.BirthdayDate == date)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button13_Click(object sender, EventArgs e)
        {
            string institute = textBox17.Text;
            var allUsers = ReadAllFromDB();
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.Institute.ToLower() == institute.ToLower())
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string group = textBox18.Text;
            var allUsers = ReadAllFromDB();
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.Group.ToLower() == group.ToLower())
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button15_Click(object sender, EventArgs e)
        {
            string course = textBox19.Text;
            var allUsers = ReadAllFromDB();
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.Course == course)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button16_Click(object sender, EventArgs e)
        {
            float avgmark = float.Parse(textBox20.Text);
            var allUsers = ReadAllFromDB();
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.AvgMArk == avgmark)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button20_Click(object sender, EventArgs e)
        {
            var allUsers = ReadAllFromDB();
            float mx = 0;
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.AvgMArk > mx)
                    {
                        mx = user.AvgMArk;
                    }
                }
            }
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.AvgMArk == mx)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void button19_Click(object sender, EventArgs e)
        {
            var allUsers = ReadAllFromDB();
            float mn = 0;
            listBox2.Items.Clear();
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.AvgMArk < mn)
                    {
                        mn = user.AvgMArk;
                    }
                }
            }
            foreach (var user in allUsers)
            {
                for (int i = 0; i < allUsers.Count; i++)
                {
                    if (user.AvgMArk == mn)
                    {
                        listBox2.Items.Add(user);
                        break;
                    }
                }
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
        private void label9_Click(object sender, EventArgs e)
        {

        }
        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        private void label10_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick_2(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void textBox13_TextChanged(object sender, EventArgs e)
        {

        }
        private void listBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void textBox12_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        static string export_file_path { get; set; }
        private void button17_Click(object sender, EventArgs e)
        {
            Int32 selectedRowCount = dataGridView1.Rows.GetRowCount(DataGridViewElementStates.Selected);
            label20.Text = selectedRowCount.ToString();
            if (selectedRowCount > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < selectedRowCount; i++)
                {
                    sb.Append(dataGridView1.SelectedRows[i].DataBoundItem.ToString());
                    sb.Append(Environment.NewLine);
                }
                string file_export = textBox21.Text; // Имя создаваемого файла
                string file_export_folder = Path.GetTempPath(); // Путь к папке с файлами
                export_file_path = file_export_folder + file_export;  // C:\Users\kalin\AppData\Local\Temp\Data_students
                label20.Text = export_file_path;
                File.WriteAllText(export_file_path, sb.ToString());
            }
        }
        private void textBox21_TextChanged(object sender, EventArgs e)
        {

        }
        private void button18_Click(object sender, EventArgs e)
        {
            textBox1.Visible = false;
            textBox2.Visible = false;
            textBox3.Visible = false;
            textBox4.Visible = false;
            textBox5.Visible = false;
            textBox6.Visible = false;
            textBox7.Visible = false;
            button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            label8.Visible = false;
            label9.Visible = false;
            textBox8.Visible = false;
            button4.Visible = false;
            label10.Visible = false;
            textBox9.Visible = false;
            textBox10.Visible = false;
            label1.Visible = false;
            textBox11.Visible = false;
            textBox12.Visible = false;
            textBox13.Visible = false;
            textBox14.Visible = false;
            textBox15.Visible = false;
            textBox16.Visible = false;
            textBox17.Visible = false;
            textBox18.Visible = false;
            textBox19.Visible = false;
            textBox20.Visible = false;
            listBox2.Visible = false;
            button9.Visible = false;
            button10.Visible = false;
            button11.Visible = false;
            button12.Visible = false;
            button13.Visible = false;
            button14.Visible = false;
            button15.Visible = false;
            button16.Visible = false;
            label11.Visible = false;
            label12.Visible = false;
            label13.Visible = false;
            label2.Visible = false;
            label14.Visible = false;
            label15.Visible = false;
            label16.Visible = false;
            label17.Visible = false;
            label18.Visible = false;
            listBox1.Visible = true;
            button17.Visible = true;
            textBox21.Visible = true;
            label20.Visible = true;
            label21.Visible = true;
            button19.Visible = false;
            button20.Visible = false;
            label22.Visible = true;
        }
        private void label23_Click(object sender, EventArgs e)
        {
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
    class User
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        [JsonProperty("BirthdayDate")]
        public DateTime BirthdayDate { get; private set; }
        public string Institute { get; private set; }
        public string Group { get; private set; }
        public string Course { get; private set; }
        public float AvgMArk { get; private set; }
        public User(int id, string name, string surname, DateTime date, string institute, string group, string course, float avgmark) //Делаем перегрузку конструктура
        {
            ID = id;
            Name = name;
            Surname = surname;
            BirthdayDate = date;
            Institute = institute;
            Group = group;
            Course = course;
            AvgMArk = avgmark;
        }
        public void SetNewID(int id) // *Возвращаем новый ID*
        {
            ID = id;
        }
        public override string ToString() //*
        {
            return $"ID: {ID}\t |Name: {Name}|\t |Surname: {Surname}|\t |Data_of_birth: {BirthdayDate}|\t |Institute: {Institute}|\t |Group: {Group}|\t |Course: {Course}|\t |Avarage_Mark: {AvgMArk}|";
        }
    }
}