using Reser.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Reser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            openFileDialog1.Filter = "CSV файлы (*.csv)|*.csv";
            saveFileDialog1.Filter = "CSV файлы (*.csv)|*.csv";
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.AddExtension = true;

            this.FormClosing += (sender, e) => { TryDeleteFile(tempFilePath); };
        }

        // Генерация уникального имени файла
        string tempFilePath = Path.Combine(Path.GetTempPath(),
                Guid.NewGuid().ToString() + ".pdf");


        double[,] Table = new double[32768, 8];
        int count = 0;
        double t2 = 0;

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            bool err = false;

            count = 0;

            label22.Visible = false;
            label23.Visible = false;

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Minimum = 0;

            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.Fr.Text == "") { this.Fr.BackColor = Color.Red; err = true; }
            if (this.kn.Text == "") { this.kn.BackColor = Color.Red; err = true; }
            if (this.kr.Text == "") { this.kr.BackColor = Color.Red; err = true; }
            if (this.T1.Text == "") { this.T1.BackColor = Color.Red; err = true; }
            if (this.TT2.Text == "") { this.TT2.BackColor = Color.Red; err = true; }
            if (this.T3.Text == "") { this.T3.BackColor = Color.Red; err = true; }
            if (this.T4.Text == "") { this.T4.BackColor = Color.Red; err = true; }
            if (this.T5.Text == "") { this.T5.BackColor = Color.Red; err = true; }
            if (this.T6.Text == "") { this.T6.BackColor = Color.Red; err = true; }
            if (this.Q1.Text == "") { this.Q1.BackColor = Color.Red; err = true; }
            if (this.Q2.Text == "") { this.Q2.BackColor = Color.Red; err = true; }
            if (this.Q3.Text == "") { this.Q3.BackColor = Color.Red; err = true; }
            if (this.Q4.Text == "") { this.Q4.BackColor = Color.Red; err = true; }
            if (this.Q5.Text == "") { this.Q5.BackColor = Color.Red; err = true; }
            if (this.Q6.Text == "") { this.Q6.BackColor = Color.Red; err = true; }
            if (this.dt.Text == "") { this.dt.BackColor = Color.Red; err = true; }
            if (this.Tras.Text == "") { this.Tras.BackColor = Color.Red; err = true; }
            if (err)
            {
                MessageBox.Show("Необходимо ввести исходные данные", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            //if (GetDouble(this.T6.Text, 0d) < GetDouble(this.Tras.Text, 0d))
            //{ this.T6.Text = this.Tras.Text; }

            double Ld = GetDouble(this.Ld.Text, 0d);
            double Fd = GetDouble(this.Fd.Text, 0d);
            double Fr = GetDouble(this.Fr.Text, 0d);
            double kn = GetDouble(this.kn.Text, 0d);
            double kr = GetDouble(this.kr.Text, 0d);
            double t1 = GetDouble(this.T1.Text, 0d);
                   t2 = GetDouble(this.TT2.Text, 0d);
            double t3 = GetDouble(this.T3.Text, 0d);
            double t4 = GetDouble(this.T4.Text, 0d);
            double t5 = GetDouble(this.T5.Text, 0d);
            double t6 = GetDouble(this.T6.Text, 0d);
            double Q1 = GetDouble(this.Q1.Text, 0d);
            double Q2 = GetDouble(this.Q2.Text, 0d);
            double Q3 = GetDouble(this.Q3.Text, 0d);
            double Q4 = GetDouble(this.Q4.Text, 0d);
            double Q5 = GetDouble(this.Q5.Text, 0d);
            double Q6 = GetDouble(this.Q6.Text, 0d);
            double dt = GetDouble(this.dt.Text, 0d);
            double Tras = GetDouble(this.Tras.Text, 0d);

            double kwd;

            if (kn < 0) kn = 0;

            if (dt > 10)
            {
                this.dt.BackColor = SystemColors.HotTrack;
                MessageBox.Show("Шаг расчета не должен превышать 10 с", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                return;
            }

            chart1.ChartAreas[0].AxisX.Maximum = Tras;
            chart2.ChartAreas[0].AxisX.Maximum = Tras;

            double Dd = Math.Pow(4 * Fd / 3.1415, 0.5);
            double R = Dd / 4;
            if (kn > 0)
            {
                double Shesi = 1 / kn * Math.Pow(R, 1 / 6);
                kwd = Ld / (Math.Pow(Shesi, 2) * R * Math.Pow(Fd, 2));
            }
            else { kwd = 0; }

            double kwr = kr / (19.62 * Math.Pow(Fd, 2));

            double Qst;

            int IK = 6;
            double[] UT = new double[6] { t1, t2, t3, t4, t5, t6 };
            double[] UQST = new double[6] { Q1, Q2, Q3, Q4, Q5, Q6 };

            double Qd;
            double Qr;
            double Hwd;
            double Hwr;
            double Z;
            double Hd;

            double T = 0;
            try
            {
                Qst = Int11(T, IK, UT, UQST);
                Qd = Qst;
                Qr = 0;
                Hwd = kwd * Qd * Math.Abs(Qd);
                Hwr = Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                Z = -Hwd - Hwr;
                Hd = Z + Hwr;

                Table[0, 0] = T;
                Table[0, 1] = Qst;
                Table[0, 2] = Qd;
                Table[0, 3] = Qr;
                Table[0, 4] = Hwd;
                Table[0, 5] = Hwr;
                Table[0, 6] = Z;
                Table[0, 7] = Hd;

                while (T <= Tras)
                {
                    T += dt;
                    count++;
                    if (count >= 32768) 
                    {
                        MessageBox.Show("Слишком много значений. Увеличьте шаг расчета", "Внимание!",
                        MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                        break;
                    }

                    Qst = Int11(T, IK, UT, UQST);
                    double DZDT = (Qd - Qst) / Fr;
                    Z += DZDT * dt;
                    double DQdDt = -(Z + Hwd + Hwr) * Fd * 9.81 / Ld;
                    Qd += DQdDt * dt;
                    Hwd = kwd * Qd * Math.Abs(Qd);
                    Qr = Qd - Qst;
                    Hwr = kwr * Qr * Math.Abs(Qr) + Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                    Hd = Z + Hwr;
                    Table[count, 0] = T;
                    Table[count, 1] = Qst;
                    Table[count, 2] = Qd;
                    Table[count, 3] = Qr;
                    Table[count, 4] = Hwd;
                    Table[count, 5] = Hwr;
                    Table[count, 6] = Z;
                    Table[count, 7] = Hd;
                }

                for (int i = 0; i < count; i++)
                {
                    chart1.Series[0].Points.AddXY(Table[i, 0], Table[i, 6]);
                    chart1.Series[1].Points.AddXY(Table[i, 0], Table[i, 7]);
                    chart2.Series[0].Points.AddXY(Table[i, 0], Table[i, 2]);
                    chart2.Series[1].Points.AddXY(Table[i, 0], Table[i, 1]);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка расчета. Проверьте корректность введенных данных",
                    "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }

            double First = Table[0, 6];
            double Second = Table[0, 6];
            // Определяем минимальный и максимальный уровень
            for (int i = 0; i < count; i++)
            {
                if (Table[i, 6] < First) First = Table[i, 6];
                if (Table[i, 6] > Second) Second = Table[i, 6];
            }
            label30.Text = "Максимальный уровень: " + Math.Round(Second, 2) + " м";
            label29.Text = "Минимальный уровень: " + Math.Round(First, 2) + " м";

            // Определяем максимумы
            if (Q1 > Q2)
            {
                double[] DYDX = new double[count];
                First = 0;
                Second = 0;
                int SecondPosition = 0;

                double Min = Table[0, 7];
                int Imin = 0;
                int t2Pos = (int)(t2 / dt);

                // Ищем первый полупериод
                for (int i = 1; i < count; i++)
                {
                    //Debug.WriteLine("{0}, {1}, {2}, {3}", i, Imin, Min, Table[i, 7]);
                    if (Table[i, 7] < Min)
                    {
                        Min = Table[i, 7];
                        Imin = i;
                        //Debug.WriteLine("{0}, {1}, {2}", i, Imin, Min);
                    }
                }
                if (Imin == 0) Imin = count;
                Second = Table[Imin, 7];
                //Debug.WriteLine("{0}, {1}", Imin, count);

                // Ищем второй максимум
                for (int i = Imin; i > t2Pos; i--)
                {
                    Debug.WriteLine("{0}, {1}, {2}, {3}, {4}",
                        i, Second, SecondPosition, Table[i, 7], Table[i - 1, 7]);
                    if (Table[i, 7] > Second)
                    {
                        Second = Table[i, 7];
                        SecondPosition = i;
                    }
                    if (Table[i - 1, 7] < Second) break;
                }

                // Ищем первый максимум
                //Debug.WriteLine("{0}", t2Pos);
                for (int i = 1; i < t2Pos + 1; i++)
                {
                    DYDX[i] = (Table[i, 7] - Table[i - 1, 7]) / dt;
                }

                for (int i = 1; i < t2Pos + 2; i++)
                {
                    //Debug.WriteLine("{0}, {1}, {2}", i, DYDX[i - 1], DYDX[i]);
                    if (DYDX[i - 1] > 1.5 * DYDX[i])
                    {
                        First = Table[i - 1, 7];
                    }
                    if (DYDX[i - 1] > 0 && DYDX[i] < 0)
                    {
                        First = Table[i - 1, 7];
                        //Debug.WriteLine("break");
                        break;
                    }
                }
                label22.Text = "Первый максимум: " + Math.Round(First, 2) + " м";
                label23.Text = "Второй максимум: " + Math.Round(Second, 2) + " м";

                label27.Visible = true;
                label22.BackColor = SystemColors.Info;
                label22.Visible = true;
                label23.BackColor = SystemColors.Info;
                label23.Visible = true;
            }
            label28.Visible = true;
            label30.BackColor = SystemColors.Info;
            label30.Visible = true;
            label29.BackColor = SystemColors.Info;
            label29.Visible = true;
        }

        private double Int11(double D, int N, double[] X, double[] Y)
        {
            double V = -1;
            int i;
            for (i = 1; i < N; i++)
            {
                if (D - X[i] <= 0)
                {
                    int i1 = i - 1;
                    V = (Y[i] * (D - X[i1]) - Y[i1] * (D - X[i])) / (X[i] - X[i1]);
                    break;
                }
            }
            if (V == -1)
            {
                V = (Y[2] * (D - X[1]) - Y[1] * (D - X[2])) / (X[2] - X[1]);
            }
            return V;
        }

        private void Ld_Enter(object sender, EventArgs e)
        {
            Ld.BackColor = SystemColors.Window;
        }
        private void Fd_Enter(object sender, EventArgs e)
        {
            Fd.BackColor = SystemColors.Window;
        }
        private void Fr_Enter(object sender, EventArgs e)
        {
            Fr.BackColor = SystemColors.Window;
        }
        private void kn_Enter(object sender, EventArgs e)
        {
            kn.BackColor = SystemColors.Window;
        }
        private void kr_Enter(object sender, EventArgs e)
        {
            kr.BackColor = SystemColors.Window;
        }
        private void T1_Enter(object sender, EventArgs e)
        {
            T1.BackColor = SystemColors.Window;
        }
        private void TT2_Enter(object sender, EventArgs e)
        {
            TT2.BackColor = SystemColors.Window;
        }
        private void T3_Enter(object sender, EventArgs e)
        {
            T3.BackColor = SystemColors.Window;
        }
        private void T4_Enter(object sender, EventArgs e)
        {
            T4.BackColor = SystemColors.Window;
        }
        private void T5_Enter(object sender, EventArgs e)
        {
            T5.BackColor = SystemColors.Window;
        }
        private void T6_Enter(object sender, EventArgs e)
        {
            T6.BackColor = SystemColors.Window;
        }
        private void Q1_Enter(object sender, EventArgs e)
        {
            Q1.BackColor = SystemColors.Window;
        }
        private void Q2_Enter(object sender, EventArgs e)
        {
            Q2.BackColor = SystemColors.Window;
        }
        private void Q3_Enter(object sender, EventArgs e)
        {
            Q3.BackColor = SystemColors.Window;
        }
        private void Q4_Enter(object sender, EventArgs e)
        {
            Q4.BackColor = SystemColors.Window;
        }
        private void Q5_Enter(object sender, EventArgs e)
        {
            Q5.BackColor = SystemColors.Window;
        }
        private void Q6_Enter(object sender, EventArgs e)
        {
            Q6.BackColor = SystemColors.Window;
        }
        private void dt_Enter(object sender, EventArgs e)
        {
            dt.BackColor = SystemColors.Window;
        }
        private void Tras_Enter(object sender, EventArgs e)
        {
            Tras.BackColor = SystemColors.Window;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool err = false;
            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.Fr.Text == "") { this.Fr.BackColor = Color.Red; err = true; }
            if (this.kn.Text == "") { this.kn.BackColor = Color.Red; err = true; }
            if (this.kr.Text == "") { this.kr.BackColor = Color.Red; err = true; }
            if (this.T1.Text == "") { this.T1.BackColor = Color.Red; err = true; }
            if (this.TT2.Text == "") { this.TT2.BackColor = Color.Red; err = true; }
            if (this.T3.Text == "") { this.T3.BackColor = Color.Red; err = true; }
            if (this.T4.Text == "") { this.T4.BackColor = Color.Red; err = true; }
            if (this.T5.Text == "") { this.T5.BackColor = Color.Red; err = true; }
            if (this.T6.Text == "") { this.T6.BackColor = Color.Red; err = true; }
            if (this.Q1.Text == "") { this.Q1.BackColor = Color.Red; err = true; }
            if (this.Q2.Text == "") { this.Q2.BackColor = Color.Red; err = true; }
            if (this.Q3.Text == "") { this.Q3.BackColor = Color.Red; err = true; }
            if (this.Q4.Text == "") { this.Q4.BackColor = Color.Red; err = true; }
            if (this.Q5.Text == "") { this.Q5.BackColor = Color.Red; err = true; }
            if (this.Q6.Text == "") { this.Q6.BackColor = Color.Red; err = true; }
            if (this.dt.Text == "") { this.dt.BackColor = Color.Red; err = true; }
            if (this.Tras.Text == "") { this.Tras.BackColor = Color.Red; err = true; }
            if (err)
            {
                MessageBox.Show("Необходимо ввести исходные данные", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // получаем выбранный файл
                string filename = saveFileDialog1.FileName;

                //если существует - удаляем
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                List<string> block1 = new List<string>();
                List<string> block2 = new List<string>();
                List<string> block3 = new List<string>();
                List<string> block4 = new List<string>();

                block1.Add(Ld.Text);
                block1.Add(Fd.Text);
                block2.Add(Fr.Text);
                block2.Add(kn.Text);
                block2.Add(kr.Text);
                block3.Add(T1.Text);
                block3.Add(TT2.Text);
                block3.Add(T3.Text);
                block3.Add(T4.Text);
                block3.Add(T5.Text);
                block3.Add(T6.Text);
                block3.Add(Q1.Text);
                block3.Add(Q2.Text);
                block3.Add(Q3.Text);
                block3.Add(Q4.Text);
                block3.Add(Q5.Text);
                block3.Add(Q6.Text);
                block4.Add(dt.Text);
                block4.Add(Tras.Text);

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(string.Join(";", block1));
                    writer.WriteLine(string.Join(";", block2));
                    writer.WriteLine(string.Join(";", block3));
                    writer.WriteLine(string.Join(";", block4));
                }

            }
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // получаем выбранный файл
                string filename = openFileDialog1.FileName;

                List<List<string>> blocks = new List<List<string>>();

                using (StreamReader reader = new StreamReader(filename))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        List<string> row = line.Split(';').ToList();
                        blocks.Add(row);
                    }
                }

                List<string> block1 = blocks.ElementAtOrDefault(0);
                List<string> block2 = blocks.ElementAtOrDefault(1);
                List<string> block3 = blocks.ElementAtOrDefault(2);
                List<string> block4 = blocks.ElementAtOrDefault(3);

                try
                {
                    Ld.Text = block1?.ElementAtOrDefault(0) ?? string.Empty;
                    Fd.Text = block1?.ElementAtOrDefault(1) ?? string.Empty;
                    Fr.Text = block2?.ElementAtOrDefault(0) ?? string.Empty;
                    kn.Text = block2?.ElementAtOrDefault(1) ?? string.Empty;
                    kr.Text = block2?.ElementAtOrDefault(2) ?? string.Empty;
                    T1.Text = block3?.ElementAtOrDefault(0) ?? string.Empty;
                    TT2.Text = block3?.ElementAtOrDefault(1) ?? string.Empty;
                    T3.Text = block3?.ElementAtOrDefault(2) ?? string.Empty;
                    T4.Text = block3?.ElementAtOrDefault(3) ?? string.Empty;
                    T5.Text = block3?.ElementAtOrDefault(4) ?? string.Empty;
                    T6.Text = block3?.ElementAtOrDefault(5) ?? string.Empty;
                    Q1.Text = block3?.ElementAtOrDefault(6) ?? string.Empty;
                    Q2.Text = block3?.ElementAtOrDefault(7) ?? string.Empty;
                    Q3.Text = block3?.ElementAtOrDefault(8) ?? string.Empty;
                    Q4.Text = block3?.ElementAtOrDefault(9) ?? string.Empty;
                    Q5.Text = block3?.ElementAtOrDefault(10) ?? string.Empty;
                    Q6.Text = block3?.ElementAtOrDefault(11) ?? string.Empty;
                    dt.Text = block4?.ElementAtOrDefault(0) ?? string.Empty;
                    Tras.Text = block4?.ElementAtOrDefault(1) ?? string.Empty;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Неверный формат файла исходных данных " +
                        "/ файл исходных данных повреждён \n\n" + ex, "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
            }
        }

        private void SaveResultButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // получаем выбранный файл
                string filename = saveFileDialog1.FileName;

                //если существует - удаляем
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                using (StreamWriter writer = new StreamWriter(filename, true,
                    System.Text.Encoding.GetEncoding(1251)))
                {
                    List<string> columnsNames = new List<string>()
                   { "Время, с", "Расход турбинного водовода, м3/с", "Расход деривации, м3/с",
                        "Расход резервуара, м3/с", "Потери в деривации, м",
                        "Потери в резервуаре, м", "Уровень в резервуаре, м",
                        "Давление в деривации, м"};
                    writer.WriteLine(string.Join(";", columnsNames));

                    for (int j = 0; j < count; j++)
                    {
                        List<string> list = new List<string>();
                        for (int i = 0; i < 8; i++)
                        {
                            double tmp;
                            tmp = Table[j, i];
                            //Debug.WriteLine("{0}, {1}, {2}", j, i, tmp);
                            list.Add(tmp.ToString());
                        }
                        writer.WriteLine(string.Join(";", list));
                    }
                }
            }
        }

        private double GetDouble(string str, double defaultValue)
        {
            double result;
            //Try parsing in the current culture
            if (!double.TryParse(str, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
                //Then try in US english
                !double.TryParse(str, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out result) &&
                //Then in neutral language
                !double.TryParse(str, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result))
            {
                result = defaultValue;
            }

            return result;
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
           byte[] fileData = Properties.Resources.Reser_help;
                       
            try
            {
                // Сохранение ресурса во временный файл
                File.WriteAllBytes(tempFilePath, fileData);

                // Запуск приложения по умолчанию
                ProcessStartInfo startInfo = new ProcessStartInfo()
                {
                    FileName = tempFilePath,
                    UseShellExecute = true // Ключевой параметр для использования ассоциаций ОС
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                // Удаление файла в случае ошибки
                TryDeleteFile(tempFilePath);
            }
        }
        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { /* Игнорируем ошибки удаления */ }
        }
    }
}
