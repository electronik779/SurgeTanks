using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ReserK
{
    public partial class Form2 : Form
    {
        int count = 0;
        double[,] Table = new double[32768, 9];

        public Form2()
        {
            InitializeComponent();

            openFileDialog1.Filter = "CSV файлы (*.csv)|*.csv";
            saveFileDialog1.Filter = "CSV файлы (*.csv)|*.csv";
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.AddExtension = true;
        }

        private void ExecuteButton1_Click(object sender, EventArgs e)
        {
            bool err = false;

            label34.Visible = false;
            label35.Visible = false;

            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart2.Series[0].Points.Clear();
            chart2.Series[1].Points.Clear();

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Minimum = 0;

            if (this.Fr.Text == "") { this.Fr.BackColor = Color.Red; err = true; }
            if (this.Kn.Text == "") { this.Kn.BackColor = Color.Red; err = true; }
            if (this.Kr.Text == "") { this.Kr.BackColor = Color.Red; err = true; }
            if (this.Zwod.Text == "") { this.Zwod.BackColor = Color.Red; err = true; }
            if (this.Bwod.Text == "") { this.Bwod.BackColor = Color.Red; err = true; }
            if (this.Mwod.Text == "") { this.Mwod.BackColor = Color.Red; err = true; }
            if (this.Zwnk.Text == "") { this.Zwnk.BackColor = Color.Red; err = true; }
            if (this.Znnk.Text == "") { this.Znnk.BackColor = Color.Red; err = true; }
            if (this.Fnk.Text == "") { this.Fnk.BackColor = Color.Red; err = true; }
            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.t1.Text == "") { this.t1.BackColor = Color.Red; err = true; }
            if (this.t2.Text == "") { this.t2.BackColor = Color.Red; err = true; }
            if (this.Q1.Text == "") { this.Q1.BackColor = Color.Red; err = true; }
            if (this.Q2.Text == "") { this.Q2.BackColor = Color.Red; err = true; }
            if (this.dt.Text == "") { this.dt.BackColor = Color.Red; err = true; }
            if (this.Tras.Text == "") { this.Tras.BackColor = Color.Red; err = true; }

            if (err)
            {
                MessageBox.Show("Необходимо ввести исходные данные", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
                return;
            }

            this.t3.Text = this.Tras.Text;
            this.Q3.Text = this.Q2.Text;

            double Fr = GetDouble(this.Fr.Text, 0d);
            double kn = GetDouble(this.Kn.Text, 0d);
            double kr = GetDouble(this.Kr.Text, 0d);
            double Zwod = GetDouble(this.Zwod.Text, 0d);
            double Bwod = GetDouble(this.Bwod.Text, 0d);
            double Mwod = GetDouble(this.Mwod.Text, 0d);
            double Zwnk = GetDouble(this.Zwnk.Text, 0d);
            double Znnk = GetDouble(this.Znnk.Text, 0d);
            double Fnk = GetDouble(this.Fnk.Text, 0d);
            double Ld = GetDouble(this.Ld.Text, 0d);
            double Fd = GetDouble(this.Fd.Text, 0d);
            double t1 = GetDouble(this.t1.Text, 0d);
            double t2 = GetDouble(this.t2.Text, 0d);
            double t3 = GetDouble(this.t3.Text, 0d);
            double Q1 = GetDouble(this.Q1.Text, 0d);
            double Q2 = GetDouble(this.Q2.Text, 0d);
            double Q3 = GetDouble(this.Q3.Text, 0d);
            double dt = GetDouble(this.dt.Text, 0d);
            double Tras = GetDouble(this.Tras.Text, 0d);

            double Wwk = 0;
            double LevelMax = 0;
            double LevelMin = 0;

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

            try
            {
                double kwd;
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

                int IK = 3;
                count = 0;

                double[] UT = new double[3] { t1, t2, t3 };
                double[] UQST = new double[3] { Q1, Q2, Q3 };

                double Qd;
                double Qr;
                double Hwd;
                double Hwr;
                double Z;
                double Hd;

                double T = 0;

                Qst = Int11(T, IK, UT, UQST);
                Qd = Qst;
                Qr = 0;
                Hwd = kwd * Qd * Math.Abs(Qd) + Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                Hwr = 0;
                Z = -Hwd;
                Hd = Z;

                Table[0, 0] = T;
                Table[0, 1] = Qst;
                Table[0, 2] = Qd;
                Table[0, 3] = Qr;
                Table[0, 4] = Hwd;
                Table[0, 5] = Hwr;
                Table[0, 6] = Z;
                Table[0, 7] = Hd;
                Table[0, 8] = Wwk;

                double[] UZ = new double[4] { Z, Zwnk, Znnk, -100 };
                double[] UW = new double[4] { 0, Fr * (Z - Zwnk),
                    Fr * (Z - Zwnk) + Fnk * (Zwnk - Znnk),
                    Fr * (Z - Zwnk) + Fnk * (Zwnk - Znnk) + Fr * (Znnk + 100) };

                double W = 0;

                while (T <= Tras)
                {
                    if (Z > LevelMax) LevelMax = Z;
                    if (Z < LevelMin) LevelMin = Z;

                    T += dt;
                    count++;
                    if (count >= 32767)
                    {
                        MessageBox.Show("Слишком много значений. Увеличьте шаг расчета", "Внимание!",
                        MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                        break;
                    }
                    Qst = Int11(T, IK, UT, UQST);
                    Qr = Qd - Qst;
                    Hwd = kwd * Qd * Math.Abs(Qd) + Math.Pow(Qd, 2) / (19.62 * Math.Pow(Fd, 2));
                    Hwr = kwr * Qr * Math.Abs(Qr);
                    //Debug.WriteLine("Common: {0} {1} {2} {3} {4}", T, Z, Qd, Qst, Qr);

                    if (Qr <= 0)
                    {
                        W += Math.Abs(Qr) * dt;
                        Z = Int11(W, 4, UW, UZ);
                        Qd += (-(Z + Hwd + Hwr) * 9.81 * Fd / Ld) * dt;
                        Qr = Qd - Qst;
                        Hd = Z + Hwr;
                        //Debug.WriteLine("Qr < 0: {0} {1} {2} {3} {4}",T, Z, Qd, Qst, Qr);
                        if (Qr >= 0) { break; }
                    }
                    if (Z <= Zwod && Qr > 0)
                    {
                        Z += Qr / Fr * dt;
                        Qd += (-(Z + Hwd + Hwr) * 9.81 * Fd / Ld) * dt;
                        Qr = Qd - Qst;
                        Hd = Z + Hwr;
                        //Debug.WriteLine("Z <= Zwod: {0} {1} {2} {3} {4}", T, Z, Qd, Qst, Qr);
                        if (Qr <= 0) { break; }
                    }
                    else if (Z > Zwod && Qr > 0)
                    {
                        Z = Zwod + Math.Pow((Qr / (Mwod * Bwod * 4.43)), 2 / 3);
                        Qd += (-(Z + Hwd + Hwr) * 9.81 * Fd / Ld) * dt;
                        Qr = Qd - Qst;
                        Hd = Z + Hwr;
                        Wwk += Qr * dt;
                        //Debug.WriteLine("Z > Zwod: {0} {1} {2} {3} {4}", T, Z, Qd, Qst, Qr);
                        if (Qr <= 0) { break; }
                    }
                    Table[count, 0] = T;
                    Table[count, 1] = Qst;
                    Table[count, 2] = Qd;
                    Table[count, 3] = Qr;
                    Table[count, 4] = Hwd;
                    Table[count, 5] = Hwr;
                    Table[count, 6] = Z;
                    Table[count, 7] = Hd;
                    Table[count, 8] = Wwk;
                }

                double Ttmp = Math.Ceiling(count * dt);
                //Debug.WriteLine("Ttmp= {0}", Ttmp);
                if (Ttmp < Tras)
                {
                    Ttmp = Math.Ceiling(Ttmp / 10) * 10;
                    //Debug.WriteLine("Ttmp= {0}", Ttmp);
                    chart1.ChartAreas[0].AxisX.Maximum = Ttmp;
                    chart2.ChartAreas[0].AxisX.Maximum = Ttmp;
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

            label35.Text = "Минимальный уровень в УР: " + Math.Round(LevelMin, 2) + " м";

            if (Q1 > Q2)
            {
                label34.Text = "Объем верхней камеры: " + Math.Round(Wwk, 0) + " м3";
                label34.BackColor = SystemColors.Info;
                label34.Visible = true;

                label35.Text = "Максимальный уровень в УР: " + Math.Round(LevelMax, 2) + " м";
            }

            label35.BackColor = SystemColors.Info;
            label35.Visible = true;
        }

        private double Int11(double D, int N, double[] X, double[] Y)
        {
            double V = -1;
            int i;
            for (i = 1; i < N; i++)
            {
                //Debug.WriteLine("i = {0}, D = {1}, X[i] = {2} ", i, D, X[i]);
                if (D - X[i] <= 0)
                {
                    int i1 = i - 1;
                    V = (Y[i] * (D - X[i1]) - Y[i1] * (D - X[i])) / (X[i] - X[i1]);
                    //Debug.WriteLine("{0} {1} {2} {3}", X[i], X[i1], Y[i], Y[i1]);
                    break;
                }
            }
            if (V == -1)
            {
                V = (Y[2] * (D - X[1]) - Y[1] * (D - X[2])) / (X[2] - X[1]);
            }
            //Debug.WriteLine("V = {0} ", V);
            return V;
        }


        private void Fr_Enter(object sender, EventArgs e)
        {
            Fr.BackColor = SystemColors.Window;
        }
        private void kn_Enter(object sender, EventArgs e)
        {
            Kn.BackColor = SystemColors.Window;
        }
        private void kr_Enter(object sender, EventArgs e)
        {
            Kr.BackColor = SystemColors.Window;
        }
        private void Zwod_Enter(object sender, EventArgs e)
        {
            Zwod.BackColor = SystemColors.Window;
        }
        private void Bwod_Enter(object sender, EventArgs e)
        {
            Bwod.BackColor = SystemColors.Window;
        }
        private void Mwod_Enter(object sender, EventArgs e)
        {
            Mwod.BackColor = SystemColors.Window;
        }
        private void Zwnk_Enter(object sender, EventArgs e)
        {
            Zwnk.BackColor = SystemColors.Window;
        }
        private void Znnk_Enter(object sender, EventArgs e)
        {
            Znnk.BackColor = SystemColors.Window;
        }
        private void Fnk_Enter(object sender, EventArgs e)
        {
            Fnk.BackColor = SystemColors.Window;
        }
        private void Ld_Enter(object sender, EventArgs e)
        {
            Ld.BackColor = SystemColors.Window;
        }
        private void Fd_Enter(object sender, EventArgs e)
        {
            Fd.BackColor = SystemColors.Window;
        }
        private void t1_Enter(object sender, EventArgs e)
        {
            t1.BackColor = SystemColors.Window;
        }
        private void t2_Enter(object sender, EventArgs e)
        {
            t2.BackColor = SystemColors.Window;
        }
        private void Q1_Enter(object sender, EventArgs e)
        {
            Q1.BackColor = SystemColors.Window;
        }
        private void Q2_Enter(object sender, EventArgs e)
        {
            Q2.BackColor = SystemColors.Window;
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
            if (this.Fr.Text == "") { this.Fr.BackColor = Color.Red; err = true; }
            if (this.Kn.Text == "") { this.Kn.BackColor = Color.Red; err = true; }
            if (this.Kr.Text == "") { this.Kr.BackColor = Color.Red; err = true; }
            if (this.Zwod.Text == "") { this.Zwod.BackColor = Color.Red; err = true; }
            if (this.Bwod.Text == "") { this.Bwod.BackColor = Color.Red; err = true; }
            if (this.Mwod.Text == "") { this.Mwod.BackColor = Color.Red; err = true; }
            if (this.Zwnk.Text == "") { this.Zwnk.BackColor = Color.Red; err = true; }
            if (this.Znnk.Text == "") { this.Znnk.BackColor = Color.Red; err = true; }
            if (this.Fnk.Text == "") { this.Fnk.BackColor = Color.Red; err = true; }
            if (this.Ld.Text == "") { this.Ld.BackColor = Color.Red; err = true; }
            if (this.Fd.Text == "") { this.Fd.BackColor = Color.Red; err = true; }
            if (this.t1.Text == "") { this.t1.BackColor = Color.Red; err = true; }
            if (this.t2.Text == "") { this.t2.BackColor = Color.Red; err = true; }
            if (this.Q1.Text == "") { this.Q1.BackColor = Color.Red; err = true; }
            if (this.Q2.Text == "") { this.Q2.BackColor = Color.Red; err = true; }
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
                List<string> block5 = new List<string>();
                List<string> block6 = new List<string>();

                block1.Add(Fr.Text);
                block1.Add(Kn.Text);
                block1.Add(Kr.Text);
                block2.Add(Zwod.Text);
                block2.Add(Bwod.Text);
                block2.Add(Mwod.Text);
                block3.Add(Zwnk.Text);
                block3.Add(Znnk.Text);
                block3.Add(Fnk.Text);
                block4.Add(Ld.Text);
                block4.Add(Fd.Text);
                block5.Add(t1.Text);
                block5.Add(t2.Text);
                block5.Add(t3.Text);
                block5.Add(Q1.Text);
                block5.Add(Q2.Text);
                block5.Add(Q3.Text);
                block6.Add(dt.Text);
                block6.Add(Tras.Text);

                using (StreamWriter writer = new StreamWriter(filename))
                {
                    writer.WriteLine(string.Join(";", block1));
                    writer.WriteLine(string.Join(";", block2));
                    writer.WriteLine(string.Join(";", block3));
                    writer.WriteLine(string.Join(";", block4));
                    writer.WriteLine(string.Join(";", block5));
                    writer.WriteLine(string.Join(";", block6));
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
                List<string> block5 = blocks.ElementAtOrDefault(4);
                List<string> block6 = blocks.ElementAtOrDefault(5);

                try
                {
                    Fr.Text = block1?.ElementAtOrDefault(0) ?? string.Empty;
                    Kn.Text = block1?.ElementAtOrDefault(1) ?? string.Empty;
                    Kr.Text = block1?.ElementAtOrDefault(2) ?? string.Empty;
                    Zwod.Text = block2?.ElementAtOrDefault(0) ?? string.Empty;
                    Bwod.Text = block2?.ElementAtOrDefault(1) ?? string.Empty;
                    Mwod.Text = block2?.ElementAtOrDefault(2) ?? string.Empty;
                    Zwnk.Text = block3?.ElementAtOrDefault(0) ?? string.Empty;
                    Znnk.Text = block3?.ElementAtOrDefault(1) ?? string.Empty;
                    Fnk.Text = block3?.ElementAtOrDefault(2) ?? string.Empty;
                    Ld.Text = block4?.ElementAtOrDefault(0) ?? string.Empty;
                    Fd.Text = block4?.ElementAtOrDefault(1) ?? string.Empty;
                    t1.Text = block5?.ElementAtOrDefault(0) ?? string.Empty;
                    t2.Text = block5?.ElementAtOrDefault(1) ?? string.Empty;
                    t3.Text = block5?.ElementAtOrDefault(2) ?? string.Empty;
                    Q1.Text = block5?.ElementAtOrDefault(3) ?? string.Empty;
                    Q2.Text = block5?.ElementAtOrDefault(4) ?? string.Empty;
                    Q3.Text = block5?.ElementAtOrDefault(5) ?? string.Empty;
                    dt.Text = block6?.ElementAtOrDefault(0) ?? string.Empty;
                    Tras.Text = block6?.ElementAtOrDefault(1) ?? string.Empty;
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
                        "Давление в деривации, м", "Объем воды в верхней камере, м3"};
                    writer.WriteLine(string.Join(";", columnsNames));

                    for (int j = 0; j < count; j++)
                    {
                        List<string> list = new List<string>();
                        for (int i = 0; i < 9; i++)
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
            string fileName = @"ReserK_help.pdf";
            if (File.Exists(fileName))
            {
                Help.ShowHelp(this, fileName, HelpNavigator.TableOfContents);
            }
            else
            {
                MessageBox.Show("Отсутствует файл справки", "Внимание!",
                    MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
            }
        }

    }
}