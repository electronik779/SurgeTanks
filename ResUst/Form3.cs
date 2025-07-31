using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ResUst
{
    public partial class Form3 : Form
    {
        double[,,] Table = new double[3, 10000, 8];
        int[] count = new int[3] { 0, 0, 0 };
        public Form3()
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

        private void button1_Click(object sender, EventArgs e)
        {
            bool err = false;

            //button1.ForeColor = SystemColors.ActiveCaptionText;
            //button1.BackColor = SystemColors.Info;
            for (int i = 0; i < count.Length; i++)
            {
                count[i] = 0;
            }

            chart1.Series[0].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart3.Series[0].Points.Clear();

            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.kn.Text == "") { this.kn.BackColor = Color.Red; err = true; }
            if (this.kr.Text == "") { this.kr.BackColor = Color.Red; err = true; }
            if (this.Qd.Text == "") { this.Qd.BackColor = Color.Red; err = true; }
            if (this.Hst.Text == "") { this.Hst.BackColor = Color.Red; err = true; }
            if (this.dt.Text == "") { this.dt.BackColor = Color.Red; err = true; }
            if (this.Tras.Text == "") { this.Tras.BackColor = Color.Red; err = true; }
            if (this.K1.Text == "") { this.K1.BackColor = Color.Red; err = true; }
            if (this.K2.Text == "") { this.K2.BackColor = Color.Red; err = true; }
            if (err)
            {
                MessageBox.Show("Необходимо ввести исходные данные", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                //button1.BackColor = SystemColors.Highlight;
                //button1.ForeColor = SystemColors.HighlightText;
                return;
            }

            double Ld = GetDouble(this.Ld.Text, 0d);
            double Fd = GetDouble(this.Fd.Text, 0d);
            double kn = GetDouble(this.kn.Text, 0d);
            double kr = GetDouble(this.kr.Text, 0d);
            double Qst = GetDouble(this.Qd.Text, 0d);
            double Hst = GetDouble(this.Hst.Text, 0d);
            double dt = GetDouble(this.dt.Text, 0d);
            double Tras = GetDouble(this.Tras.Text, 0d);
            double K1 = GetDouble(this.K1.Text, 1.4d);
            double K2 = GetDouble(this.K2.Text, 0.6d);

            double kwd;

            double Dd = Math.Pow(4 * Fd / 3.1415, 0.5);
            double R = Dd / 4;
            if (kn > 0)
            {
                double Shesi = 1 / kn * Math.Pow(R, 1 / 6);
                kwd = Ld / (Math.Pow(Shesi, 2) * R * Math.Pow(Fd, 2));
            }
            else { kwd = 0; }
            double Hwd = kwd * Qst * Math.Abs(Qst);

            double Hsk = Math.Pow(Qst, 2) / 19.62 / Math.Pow(Fd, 2);

            double Hr = Hst - Hwd;

            double Fro = (Ld * Math.Pow(Qst, 2)) / ((Hwd + Hsk) * 19.62 * Fd * Hr);
            this.Fkr.Text = "= " + Math.Round(Fro, 2) + " м2";
            double Fr = Fro;

            if (kn < 0) kn = 0;

            if (dt > 10)
            {
                this.dt.BackColor = SystemColors.HotTrack;
                MessageBox.Show("Шаг расчета не должен превышать 10 с", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                //button1.BackColor = SystemColors.Highlight;
                //button1.ForeColor = SystemColors.HighlightText;
                return;
            }

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart3.ChartAreas[0].AxisX.Minimum = 0;

            chart1.ChartAreas[0].AxisX.Maximum = Tras;
            chart2.ChartAreas[0].AxisX.Maximum = Tras;
            chart3.ChartAreas[0].AxisX.Maximum = Tras;

            //double dx = 10;
            //if (Tras > 100) { dx = 100; }
            //if (Tras > 100) { dx = 500; }
            //chart1.ChartAreas[0].AxisX.MajorGrid.Interval = dx;
            //chart2.ChartAreas[0].AxisX.MajorGrid.Interval = dx;
            //chart3.ChartAreas[0].AxisX.MajorGrid.Interval = dx;
            //double dy = 10;
            //chart1.ChartAreas[0].AxisY.MajorGrid.Interval = dy;
            //chart2.ChartAreas[0].AxisY.MajorGrid.Interval = dy;
            //chart3.ChartAreas[0].AxisY.MajorGrid.Interval = dy;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        Fr = K1 * Fro;
                        break;
                    case 1:
                        Fr = Fro;
                        break;
                    case 2:
                        Fr = K2 * Fro;
                        break;
                    default:
                        Fr = Fro;
                        break;
                }
                //Debug.WriteLine("S: i= {0}, Fr= {1}", i, Fr);
                Dd = Math.Pow(4 * Fd / 3.1415, 0.5);
                R = Dd / 4;
                if (kn > 0)
                {
                    double Shesi = 1 / kn * Math.Pow(R, 1 / 6);
                    kwd = Ld / (Math.Pow(Shesi, 2) * R * Math.Pow(Fd, 2));
                }
                else { kwd = 0; }

                double kwr = kr / (19.62 * Math.Pow(Fd, 2));

                double Qd;
                double Qr;
                //double Hwd;
                double Hwr;
                double Z;
                double Z0;
                double Hd;
                double Qst0 = Qst;

                double T = 0;
                try
                {
                    Qd = Qst0;
                    Qr = 0;
                    Hwd = kwd * Qd * Math.Abs(Qd);
                    Hwr = 0;
                    Z = -Hwd - Hwr - Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                    Z0 = Z;
                    Z = Z0 + 1;
                    Hd = Z;

                    Table[i, 0, 0] = T;
                    Table[i, 0, 1] = Qst0;
                    Table[i, 0, 2] = Qd;
                    Table[i, 0, 3] = Qr;
                    Table[i, 0, 4] = Hwd;
                    Table[i, 0, 5] = Hwr;
                    Table[i, 0, 6] = Z;
                    Table[i, 0, 7] = Hd;

                    //Debug.WriteLine("i={0},  {1} {2} {3} {4} {5} {6} {7} {8}",
                    //    i, Table[i, 0, 0], Table[i, 0, 1], Table[i, 0, 2], Table[i, 0, 3],
                    //    Table[i, 0, 4], Table[i, 0, 5], Table[i, 0, 6], Table[i, 0, 7]);

                    while (T <= Tras)
                    {
                        T += dt;
                        count[i]++;
                        Qst = Qst0 * (Hst + Z0) / (Hst + Z);
                        double DZDT = (Qd - Qst) / Fr;
                        Z += DZDT * dt;
                        double DQdDt = -(Z + Hwd + Hwr) * Fd * 9.81 / Ld;
                        Qd += DQdDt * dt;
                        Hwd = kwd * Qd * Math.Abs(Qd);
                        Qr = Qd - Qst;
                        Hwr = kwr * Qr * Math.Abs(Qr) + Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                        Hd = Z + Hwr;
                        Table[i, count[i], 0] = T;
                        Table[i, count[i], 1] = Qst;
                        Table[i, count[i], 2] = Qd;
                        Table[i, count[i], 3] = Qr;
                        Table[i, count[i], 4] = Hwd;
                        Table[i, count[i], 5] = Hwr;
                        Table[i, count[i], 6] = Z;
                        Table[i, count[i], 7] = Hd;

                        if (Z > 100 || Z < -100)
                        {
                            MessageBox.Show("Ошибка расчета.\nСлишком большой диапазон колебаний." +
                                " Попробуйте задать К1 и / или К2 ближе к 1.",
                            "Внимание!",
                            MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Ошибка расчета. Проверьте корректность введенных данных",
                        "Внимание!",
                        MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int y = 0; y < count[i]; y++)
                {
                    switch (i)
                    {
                        case 0:
                            chart1.Series[0].Points.AddXY(Table[i, y, 0], Table[i, y, 6]);
                            break;
                        case 1:
                            chart2.Series[0].Points.AddXY(Table[i, y, 0], Table[i, y, 6]);
                            break;
                        case 2:
                            chart3.Series[0].Points.AddXY(Table[i, y, 0], Table[i, y, 6]);
                            break;
                        default:
                            break;
                    }
                }
            }

            //button1.BackColor = SystemColors.Highlight;
            //button1.ForeColor = SystemColors.HighlightText;
        }



        private void Ld_Enter(object sender, EventArgs e)
        {
            Ld.BackColor = SystemColors.Window;
        }
        private void Fd_Enter(object sender, EventArgs e)
        {
            Fd.BackColor = SystemColors.Window;
        }

        private void kn_Enter(object sender, EventArgs e)
        {
            kn.BackColor = SystemColors.Window;
        }
        private void kr_Enter(object sender, EventArgs e)
        {
            kr.BackColor = SystemColors.Window;
        }
        private void Qd_Enter(object sender, EventArgs e)
        {
            Qd.BackColor = SystemColors.Window;
        }
        private void Hst_Enter(object sender, EventArgs e)
        {
            Hst.BackColor = SystemColors.Window;
        }
        private void dt_Enter(object sender, EventArgs e)
        {
            dt.BackColor = SystemColors.Window;
        }
        private void Tras_Enter(object sender, EventArgs e)
        {
            Tras.BackColor = SystemColors.Window;
        }
        private void K1_Enter(object sender, EventArgs e)
        {
            K1.BackColor = SystemColors.Window;
        }
        private void K2_Enter(object sender, EventArgs e)
        {
            K2.BackColor = SystemColors.Window;
        }
        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool err = false;
            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.kn.Text == "") { this.kn.BackColor = Color.Red; err = true; }
            if (this.kr.Text == "") { this.kr.BackColor = Color.Red; err = true; }
            if (this.Qd.Text == "") { this.Qd.BackColor = Color.Red; err = true; }
            if (this.Hst.Text == "") { this.Hst.BackColor = Color.Red; err = true; }
            if (this.dt.Text == "") { this.dt.BackColor = Color.Red; err = true; }
            if (this.Tras.Text == "") { this.Tras.BackColor = Color.Red; err = true; }
            if (this.K1.Text == "") { this.K1.BackColor = Color.Red; err = true; }
            if (this.K2.Text == "") { this.K2.BackColor = Color.Red; err = true; }
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

                block1.Add(Ld.Text);
                block1.Add(Fd.Text);
                block2.Add(kn.Text);
                block2.Add(kr.Text);
                block3.Add(Qd.Text);
                block3.Add(Hst.Text);
                block3.Add(dt.Text);
                block3.Add(Tras.Text);
                block3.Add(K1.Text);
                block3.Add(K2.Text);

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(string.Join(";", block1));
                    writer.WriteLine(string.Join(";", block2));
                    writer.WriteLine(string.Join(";", block3));
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

                try
                {
                    Ld.Text = block1?.ElementAtOrDefault(0) ?? string.Empty;
                    Fd.Text = block1?.ElementAtOrDefault(1) ?? string.Empty;
                    kn.Text = block2?.ElementAtOrDefault(0) ?? string.Empty;
                    kr.Text = block2?.ElementAtOrDefault(1) ?? string.Empty;
                    Qd.Text = block3?.ElementAtOrDefault(0) ?? string.Empty;
                    Hst.Text = block3?.ElementAtOrDefault(1) ?? string.Empty;
                    dt.Text = block3?.ElementAtOrDefault(2) ?? string.Empty;
                    Tras.Text = block3?.ElementAtOrDefault(3) ?? string.Empty;
                    K1.Text = block3?.ElementAtOrDefault(4) ?? string.Empty;
                    K2.Text = block3?.ElementAtOrDefault(5) ?? string.Empty;
                }
                catch (Exception ex)
                {


                    MessageBox.Show("Неверный формат файла исходных данных " +
                        "/ файл исходных данных повреждён \n\n" + ex, "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
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
            byte[] fileData = Properties.Resources.ResUst_help;

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

        private void ResultButton1_Click(object sender, EventArgs e)
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
                    List<string> cases = new List<string>()
                        { "Fр = К1 * Fкр", "Fр = Fкр", "Fр = К2 * Fкр" };

                    List<string> columnsNames = new List<string>()
                        { "Время, с", "Расход турбинного водовода, м3/с", "Расход деривации, м3/с",
                        "Расход резервуара, м3/с", "Потери в деривации, м",
                        "Потери в резервуаре, м", "Уровень в резервуаре, м",
                        "Давление в деривации, м"};

                    for (int i = 0; i < 3; i++)
                    {
                        writer.WriteLine(string.Join(";", cases[i]));
                        writer.WriteLine(string.Join(";", columnsNames));

                        for (int j = 0; j < count[i]; j++)
                        {
                            List<string> list = new List<string>();
                            for (int k = 0; k < 8; k++)
                            {
                                double tmp;
                                tmp = Table[i, j, k];
                                //Debug.WriteLine("{0}, {1}, {2}", j, i, tmp);
                                list.Add(tmp.ToString());
                            }
                            writer.WriteLine(string.Join(";", list));
                        }
                    }
                }
            }
        }
    }
}
