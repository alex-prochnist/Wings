using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using WINGS_LIBRARY;

namespace WING
{
    public partial class WingForm : Form
    {
        Analysis _an;
        InitData InitD;//для тестового запуска
        EditGorForm _edGorForm;
        EditVerForm _edVerForm;
        EditPlochForm _edPlochForm;
        EditDltForm _edDltForm;
        //*************************************************
        public WingForm()
        {
            InitializeComponent();
            _an = new Analysis();
            InitD = new InitData();
            numLonComboBox.Items.AddRange(_an.InitD.MaterialName.ToArray());
            numStrComboBox.Items.AddRange(_an.InitD.MaterialName.ToArray());
        }

        //ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
        //********************************************************
        void SetModYprFFZ(ref int LonORStr, int indx, int selectIndx)//УСТАНАВЛИВАЕМ ИМЯ МАТ И ЗНАЧ МОД УПР ВЫБРАНЫХ МАТЕР 
        {//В ПОСЛЕДНЕМ ВЫПАДАЮЩЕМ СПИСКЕ
            switch (selectIndx)
            {
                case 0:
                    LonORStr = 1;
                    _an.InitD.ModYprFFZName[indx] = _an.InitD.MaterialName[0];
                    _an.InitD.ModYprFFZValue[indx] = _an.InitD.ModYprMatValue[0];
                    _an.InitD.SigmaMaterFFZ[indx] = _an.InitD.SigmaMater[0];
                    break;
                case 1:
                    LonORStr = 2;
                    _an.InitD.ModYprFFZName[indx] = _an.InitD.MaterialName[1];
                    _an.InitD.ModYprFFZValue[indx] = _an.InitD.ModYprMatValue[1];
                    _an.InitD.SigmaMaterFFZ[indx] = _an.InitD.SigmaMater[1];
                    break;
                case 2:
                    LonORStr = 3;
                    _an.InitD.ModYprFFZName[indx] = _an.InitD.MaterialName[2];
                    _an.InitD.ModYprFFZValue[indx] = _an.InitD.ModYprMatValue[2];
                    _an.InitD.SigmaMaterFFZ[indx] = _an.InitD.SigmaMater[2];
                    break;
                case 3:
                    LonORStr = 4;
                    _an.InitD.ModYprFFZName[indx] = _an.InitD.MaterialName[3];
                    _an.InitD.ModYprFFZValue[indx] = _an.InitD.ModYprMatValue[3];
                    _an.InitD.SigmaMaterFFZ[indx] = _an.InitD.SigmaMater[3];
                    break;
                case 4:
                    LonORStr = 5;
                    _an.InitD.ModYprFFZName[indx] = _an.InitD.MaterialName[4];
                    _an.InitD.ModYprFFZValue[indx] = _an.InitD.ModYprMatValue[4];
                    _an.InitD.SigmaMaterFFZ[indx] = _an.InitD.SigmaMater[4];
                    break;
            }
        }

        void addButton(ref List<double> XYF, ListBox koorOrPlochListBox, ref int countEl, ref int indexEl, TextBox textBox, int numEl)
        {
            if (_an.InitD.NumEl > indexEl && textBox.Text != "")
            {
                string tmp = textBox.Text;
                if (tmp.Contains("."))
                    tmp = tmp.Replace(".", ",");//учет и запятых и точек
                if (tmp.Contains("*"))//если внесли 
                {
                    string[] arrString = tmp.Split('*');
                    int countStEl = Convert.ToInt32(arrString[0]);//количество стандартных элементов
                    for (int i = 0; i < countStEl; i++)
                    {
                        countEl++;
                        indexEl++;
                        string disp = indexEl.ToString() + ". " + arrString[1];
                        koorOrPlochListBox.Items.Add(disp);
                        XYF.Add((float)Convert.ToDouble(arrString[1]));
                        textBox.Clear();
                    }
                }
                else//если внесли единичный элемент
                {
                    countEl++;
                    indexEl++;
                    string disp = indexEl.ToString() + ". " + tmp;
                    koorOrPlochListBox.Items.Add(disp);
                    XYF.Add((float)Convert.ToDouble(tmp));
                    textBox.Clear();
                }
            }
            else
                if (_an.InitD.NumEl == indexEl && textBox.Text != "")
                {
                    MessageBox.Show("Номер добавляемого элемента больше указанного количества элементов.");
                    textBox.Clear();
                }
        }

        void delButton(ref List<double> XYF, ListBox koorOrPlochListBox, ref int countEnum, ref int indexEnum)
        {
            if (koorOrPlochListBox.SelectedIndex != -1)
            {
                MessageBoxButtons mes = new MessageBoxButtons();
                mes = MessageBoxButtons.YesNo;
                DialogResult res = MessageBox.Show("Вы дествительно хотите удалить выбранный элемент?", "Удаление", mes);
                if (res == DialogResult.Yes)
                {
                    XYF.RemoveAt(koorOrPlochListBox.SelectedIndex);
                    koorOrPlochListBox.Items.RemoveAt(koorOrPlochListBox.SelectedIndex);
                    countEnum--;
                    indexEnum--;
                    if (countEnum >= 0)
                    {
                        List<float> listTmp = new List<float>(countEnum);
                        koorOrPlochListBox.Items.Clear();
                        string disp = null;
                        int j = 0;
                        for (int i = 0; i <= countEnum; i++)
                        {
                            j = i + 1;
                            disp = j.ToString() + ". " + Convert.ToString(XYF[i]);
                            koorOrPlochListBox.Items.Add(disp);
                        }
                    }
                }
            }
        }

        void clearButton(ref List<double> XYF, ListBox koorOrPlochListBox, ref int countEl, ref int indexEl)
        {
            if (XYF.Count > 0)
            {
                MessageBoxButtons mes = new MessageBoxButtons();
                mes = MessageBoxButtons.YesNo;
                DialogResult res = MessageBox.Show("Вы дествительно хотите очистить список?", "Очистка списка", mes);
                if (res == DialogResult.Yes)
                {
                    XYF.Clear();
                    koorOrPlochListBox.Items.Clear();
                    countEl = -1;
                    indexEl = 0;
                }
            }
        }

        void addDltFuncButton(ref List<double> dltList, ListBox dltListBox, ref int countEl, ref int indexEl, TextBox dltTextBox, int numEl)
        {
            if ((_an.InitD.NumEl + 1) > indexEl && dltTextBox.Text != "")
            {
                countEl++;
                indexEl++;
                string disp = indexEl.ToString() + ". " + dltTextBox.Text;
                dltListBox.Items.Add(disp);
                dltList.Add((float)Convert.ToDouble(dltTextBox.Text));//работает только с запятыми
                dltTextBox.Clear();
            }
            else
                if ((_an.InitD.NumEl + 1) == indexEl && dltTextBox.Text != "")
                {
                    MessageBox.Show("Номер добавляемого элемента больше указанного количества элементов.");
                    dltTextBox.Clear();
                }
        }
        
        private void BlockNotDigit(KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true) return;
            if (e.KeyChar == Convert.ToChar(Keys.Back)) return;
            if (e.KeyChar == Convert.ToChar(Keys.Insert)) return;
            e.Handled = true;
        }//ввод только цифр

        private void BlockNotDigitOrPoint(KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) == true) return;
            if (e.KeyChar == Convert.ToChar(Keys.Back)) return;
            if (e.KeyChar == ',' || e.KeyChar == '.') return;
            e.Handled = true;
        }//ввод только цифр и разделителя

        private void checkInitData()
        {
            if (_an.InitD.LName.Length == 0)
                throw new Exception("Ошибка!! Не указанa фамилия. Пожалуйста, введите фамилию пользователя.");
             if (_an.InitD.FName.Length == 0)
                 throw new Exception("Ошибка!! Не указано имя. Пожалуйста, введите имя пользователя.");
             if (_an.InitD.SName.Length == 0)
                 throw new Exception("Ошибка!! Не указано отчество. Пожалуйста, введите отчество пользователя.");
             if(_an.InitD.NumEl==0)
                 throw new Exception("Ошибка!! Не указано количество продольных элементов."+
                     " Пожалуйста, введите число продольных силовых элементов.");
             if (_an.InitD.NumLon == 0)
                 throw new Exception("Ошибка!! Не указано количество полок лонжеронов." +
                     " Пожалуйста, введите число полок лонжеронов.");
             if (_an.InitD.NumMatLon == 0)
                 throw new Exception("Ошибка!! Не указан материал полок лонжеронов." +
                     " Пожалуйста, выберете материал полок лонжеронов.");
             if (_an.InitD.NumMatStringer == 0)
                 throw new Exception("Ошибка!! Не указан материал стрингера+обшивки." +
                     " Пожалуйста, выберете материал стрингера+обшивки.");
             if (_an.InitD.NumLonPVPL == 0)
                 throw new Exception("Ошибка!! Не указан номер передней верхней полки лонжерона." +
                     " Пожалуйста, введите номер передней верхней полки лонжерона.");
            if (_an.InitD.NumLonZVPL == 0)
                throw new Exception("Ошибка!! Не указан номер задней верхней полки лонжерона." +
                     " Пожалуйста, введите номер задней верхней полки лонжерона.");
            if (_an.InitD.NumLonPNPL == 0)
                throw new Exception("Ошибка!! Не указан номер передней нижней полки лонжерона." +
                     " Пожалуйста, введите номер передней нижней полки лонжерона.");
            if (_an.InitD.NumLonZNPL == 0)
                throw new Exception("Ошибка!! Не указан номер задней нижней полки лонжерона." +
                     " Пожалуйста, введите номер задней верхней нижней лонжерона.");
            if (_an.InitD.ChoiceMat == -1)
                throw new Exception("Ошибка!! Не указан материал для фиктивного физического закона." +
                     " Пожалуйста, выберете материал для фиктивного физического закона.");
            if (_an.InitD.PNaprSgLon == 0)
                throw new Exception("Ошибка!! Не указано значение предельного допускаемого напряжения для полок лонжерона." +
                     " Пожалуйста, введите значение предельного допускаемого напряжения для полок лонжерона.");
            if (_an.InitD.PNaprSgStr == 0)
                throw new Exception("Ошибка!! Не указано значение предельного допускаемого напряжения для стрингера+обшивкa." +
                     " Пожалуйста, введите значение предельного допускаемого напряжения для стрингера+обшивкa.");
            if (_an.InitD.Mt == -1)
                throw new Exception("Ошибка!! Не указано значение изгибающего момента, вектор момент которого направлен вдоль хорды крыла." +
                     " Пожалуйста, введите значение изгибающего момента.");
            if (_an.InitD.Mn == -1)
                throw new Exception("Ошибка!! Не указано значение изгибающего момента, вектор момент которого направлен вдоль размаха крыла." +
                     " Пожалуйста, введите значение изгибающего момента.");
            if (_an.InitD.N == -1)
                throw new Exception("Ошибка!! Не указано значение осевой силы." +
                     " Пожалуйста, введите значение силы.");
            if (_an.InitD.Qn == -1)
                throw new Exception("Ошибка!! Не указано значение силы, которая направлена вверх." +
                     " Пожалуйста, введите значение силы.");
            if (_an.InitD.Qt == -1)
                throw new Exception("Ошибка!! Не указано значение силы, которая направлена к хвосту самолета." +
                     " Пожалуйста, введите значение силы.");
            if (_an.InitD.E == -1)
                throw new Exception("Ошибка!! Не указано расстояние от стенки переднего лонжерона до линии действия Qn." +
                     " Пожалуйста, введите значение расстояния.");
            if (_an.InitD.x.Count < _an.InitD.NumEl)
                throw new Exception("Ошибка!! Количество координат ц.т. продольных силовых элементов по оси Х меньше указанного общего числа продольных элементов." +
                     " Пожалуйста, уточните количество продольных элементов или координат ц.т. продольных элементов по оси Х.");
            if (_an.InitD.y.Count < _an.InitD.NumEl)
                throw new Exception("Ошибка!! Количество координат ц.т. продольных силовых элементов по оси Y меньше указанного общего числа продольных элементов." +
                     " Пожалуйста, уточните количество продольных элементов или координат ц.т. продольных элементов по оси Y.");
            if (_an.InitD.f.Count < _an.InitD.NumEl)
                throw new Exception("Ошибка!! Количество значений площадей продольных силовых элементов меньше указанного общего числа продольных элементов." +
                     " Пожалуйста, уточните количество продольных элементов или значений площадей продольных элементов.");
            if (_an.InitD.dlt.Count < _an.InitD.NumEl+1)
                throw new Exception("Ошибка!! Количество значений редуцированых толщин и стенок лонжеронов меньше на 2 от указанного общего числа продольных элементов." +
                     " Пожалуйста, уточните количество продольных элементов или значений значений редуцированых толщин и стенок лонжеронов.");
        }

        private void createReport()
        {
            var tstDoc = new Document();
            var tstDocCreate = PdfWriter.GetInstance(tstDoc, new FileStream("Результаты расчета.pdf", FileMode.Create));
            tstDoc.OpenDocument();
            var table1 = new PdfPTable(9); var table2 = new PdfPTable(8); var table3 = new PdfPTable(4); var table4 = new PdfPTable(4);
            var BaseFontDoc = BaseFont.CreateFont(@"C:\Windows\Fonts\times.ttf", "cp1251", BaseFont.EMBEDDED);
            var FontDoc = new iTextSharp.text.Font(BaseFontDoc, 14, iTextSharp.text.Font.NORMAL);
            var CellDoc = new PdfPCell(new Phrase("Cell", FontDoc));

            var headFont = new iTextSharp.text.Font(BaseFontDoc, 16, iTextSharp.text.Font.BOLD);
            Paragraph head = new Paragraph("Результаты расчета сечения крыла большого удлинения на прочность", headFont);
            head.SetAlignment("center"); tstDoc.Add(head); tstDoc.Add(new Paragraph("\n\n"));

            StringBuilder person = new StringBuilder("Выполнил студент(ка): ");
            person.Append(_an.InitD.LName + " " + _an.InitD.FName + " " + _an.InitD.SName);
            tstDoc.Add(new Paragraph(person.ToString(), FontDoc)); tstDoc.Add(new Paragraph("\n"));

            Paragraph head2 = new Paragraph("Исходные данные:",
                new iTextSharp.text.Font(BaseFontDoc, 14, iTextSharp.text.Font.BOLD)); head2.SetAlignment("center");
            tstDoc.Add(head2);
            tstDoc.Add(new Paragraph("\n"));

            createNameTable("              Таблица 1", tstDoc, BaseFontDoc);
            Single[] widhtCell = { 40.0F, 50.0F, 60.0F, 30.0F, 30.0F, 30.0F, 30.0F, 30.0F, 30.0F };
            table1.SetTotalWidth(widhtCell);
            table1.HorizontalAlignment = Element.ALIGN_CENTER;
            CellDoc.HorizontalAlignment = Element.ALIGN_CENTER;
            CreateTable1(tstDoc, table1, FontDoc, CellDoc);

            createNameTable("              Таблица 2", tstDoc, BaseFontDoc);
            Single[] widhtCell2 = { 40.0F, 40.0F, 55.0F, 55.0F, 40.0F, 40.0F, 40.0F, 40.0F};
            table2.SetTotalWidth(widhtCell2);
            table2.HorizontalAlignment = Element.ALIGN_CENTER;
            CreateTable2(tstDoc, table2, FontDoc, CellDoc);

            createNameTable("              Таблица 3", tstDoc, BaseFontDoc);
            table3.HorizontalAlignment = Element.ALIGN_CENTER;
            CreateTable3(tstDoc, table3, FontDoc, CellDoc, BaseFontDoc);

            table4.HorizontalAlignment = Element.ALIGN_CENTER;
            CreateTable4(tstDoc, table4, FontDoc, CellDoc, BaseFontDoc);

            tstDoc.Close();

            System.Diagnostics.Process.Start("IExplore.exe", Directory.GetCurrentDirectory() + @"\Результаты расчета.pdf");
        }

        private void createNameTable(string name, Document doc, BaseFont baseFont)
        {
            Paragraph nameTable = new Paragraph(name,
               new iTextSharp.text.Font(baseFont, 14, iTextSharp.text.Font.NORMAL));
            doc.Add(nameTable);
            doc.Add(new Paragraph("\n", new iTextSharp.text.Font(baseFont, 7, iTextSharp.text.Font.BOLD)));
        }

        private void CreateTable1(Document doc, PdfPTable table, iTextSharp.text.Font font, PdfPCell cellDoc)
        {
            cellDoc.Phrase = new Phrase("Eффз, мПа", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Доп. напр. п. л., мПа", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Доп. напр. стр+об, мПа", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Mt, кН*м", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Mn, кН*м", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("N, кН", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Qn, кН", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Qt, кН", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("e, м", font);
            table.AddCell(cellDoc);

            cellDoc.Phrase = new Phrase(_an.InitD.ModUprFD.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.PNaprSgLon / 1000000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.PNaprSgStr / 1000000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.Mt / 1000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.Mn / 1000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.N / 1000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.Qn / 1000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.Qt / 1000).ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase((_an.InitD.E).ToString(), font);
            table.AddCell(cellDoc);

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }

        private void CreateTable2(Document doc, PdfPTable table, iTextSharp.text.Font font, PdfPCell cellDoc)
        {
            cellDoc.Phrase = new Phrase("Число прод. элем.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Число пол. лон.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Материал полок лонж.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Материал стр+обш", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Номер верх. перед. п. л.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Номер нижн. перед. п. л.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Номер верх. задн. п. л.", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Номер нижн. задн. п. л.", font);
            table.AddCell(cellDoc);

            cellDoc.Phrase = new Phrase(_an.InitD.NumEl.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.NumLon.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.ModYprFFZName[0].ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.ModYprFFZName[1].ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.NumLonPVPL.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.NumLonPNPL.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.NumLonZVPL.ToString(), font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.NumLonZNPL.ToString(), font);
            table.AddCell(cellDoc);

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }

        private void CreateTable3(Document doc, PdfPTable table, iTextSharp.text.Font font, PdfPCell cellDoc, BaseFont baseFont)
        {
            cellDoc.Phrase = new Phrase("Координаты ц.т. по оси Х", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Координаты ц.т. по оси У", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Площадь элементов, м^2", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Ред. толщ. обш. и стен., м", font);
            table.AddCell(cellDoc);

            for (int i = 0; i < _an.InitD.x.Count; i++)
            {
                cellDoc.Phrase = new Phrase(_an.InitD.x[i].ToString(), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.InitD.y[i].ToString(), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.InitD.f[i].ToString(), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.InitD.dlt[i].ToString(), font);
                table.AddCell(cellDoc); 
            }
            cellDoc.Phrase = new Phrase("", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase(_an.InitD.dlt[_an.InitD.x.Count].ToString(), font);
            table.AddCell(cellDoc);

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }

        private void CreateTable4(Document doc, PdfPTable table, iTextSharp.text.Font font, PdfPCell cellDoc, BaseFont baseFont)
        {
            Paragraph head3 = new Paragraph("Результаты вычисления",
                new iTextSharp.text.Font(baseFont, 16, iTextSharp.text.Font.BOLD)); head3.SetAlignment("center");
            doc.Add(head3);
            doc.Add(new Paragraph("\n"));

            cellDoc.Phrase = new Phrase("Деств. напряжения, мПа", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Координаты ц.т. в ГЦО по оси Х", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Координаты ц.т. в ГЦО по оси У", font);
            table.AddCell(cellDoc);
            cellDoc.Phrase = new Phrase("Редукц. коэфф. след. итер.", font);
            table.AddCell(cellDoc);

            for (int i = 0; i < _an.InitD.x.Count; i++)
            {
                cellDoc.Phrase = new Phrase(_an.N_NaD[0,i].ToString().Remove(8), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.U[i].ToString().Remove(6), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.V[i].ToString().Remove(6), font);
                table.AddCell(cellDoc);
                cellDoc.Phrase = new Phrase(_an.CfRedNext[i].ToString().Remove(6), font);
                table.AddCell(cellDoc);
            }

            doc.Add(table);
            doc.Add(new Paragraph("\n"));
        }
        /// -------------------------------------------------
        void TestSystem()//по файлу кряжева
        {
            InitD.FName = "Виктор"; fNameTextBox.Text = InitD.FName;
            InitD.SName = "Петрович"; sNameTextBox.Text = InitD.SName;
            InitD.LName = "Кряжев"; lNameTextBox.Text = InitD.LName;
            InitD.NumEl = 30; numElNumericUpDown.Value = InitD.NumEl;
            InitD.NumLon = 4; _4RadioButton.Checked = true;
            InitD.NumMatLon = 2; numLonComboBox.SelectedIndex = 1;
            InitD.ModYprFFZName[0] = _an.InitD.MaterialName[1];   
            InitD.ModYprFFZValue[0] = _an.InitD.ModYprMatValue[1];
            InitD.SigmaMaterFFZ[0] = _an.InitD.SigmaMater[1];;
            InitD.NumMatStringer = 4; numStrComboBox.SelectedIndex = 3;
            InitD.ModYprFFZName[1] = _an.InitD.MaterialName[3];
            InitD.ModYprFFZValue[1] = _an.InitD.ModYprMatValue[3];
            InitD.SigmaMaterFFZ[1] = _an.InitD.SigmaMater[3];
            InitD.NumLonPVPL = 2; numPVPLTextBox.Text = "2";
            InitD.NumLonZVPL = 16; numZVPLTextBox.Text = "16";
            InitD.NumLonZNPL = 17; numZNPLTextBox.Text = "17";
            InitD.NumLonPNPL = 30; numPNPLTextBox.Text = "30";
            InitD.MasElNumLon[0] = InitD.NumLonPVPL;
            InitD.MasElNumLon[1] = InitD.NumLonZVPL;
            InitD.MasElNumLon[2] = InitD.NumLonZNPL;
            InitD.MasElNumLon[3] = InitD.NumLonPNPL;
            InitD.ModUprFD = InitD.ModYprFFZValue[0]; modYprFFZComboBox.SelectedIndex = 0; InitD.ChoiceMat = InitD.NumMatLon;
            InitD.PNaprSgLon = -1600000000; sigmaDopPLTextBox.Text = "-1600000000";
            InitD.PNaprSgStr = -370000000; sigmaDopStrTextBox.Text = "-370000000";
            InitD.Mt = 3490000; mtTextBox.Text = "3490000";
            InitD.Mn = 0; mnTextBox.Text = "0";
            InitD.N = 0; nTextBox.Text = "0";
            InitD.Qn = 0; qnTextBox.Text = "0";
            InitD.Qt = 589000; qtTextBox.Text = "589000";
            InitD.E = 0.854; eTextBox.Text = "0.854";
            SetKoorX();
            SetKoorY();
            SetF();
            SetDlt();
        }

        private void SetKoorX()
        {
            InitD.x.Add(0); InitD.x.Add(0.768); InitD.x.Add(0.938);
            InitD.x.Add(1.128); InitD.x.Add(1.317); InitD.x.Add(1.507);
            InitD.x.Add(1.697); InitD.x.Add(1.887); InitD.x.Add(2.077);
            InitD.x.Add(2.267); InitD.x.Add(2.457); InitD.x.Add(2.647);
            InitD.x.Add(2.837); InitD.x.Add(3.038); InitD.x.Add(3.253);
            InitD.x.Add(3.47); InitD.x.Add(3.47); InitD.x.Add(3.272);
            InitD.x.Add(3.062); InitD.x.Add(2.852); InitD.x.Add(2.642);
            InitD.x.Add(2.432); InitD.x.Add(2.222); InitD.x.Add(2.012);
            InitD.x.Add(1.802); InitD.x.Add(1.592); InitD.x.Add(1.382);
            InitD.x.Add(1.172); InitD.x.Add(0.963); InitD.x.Add(0.764);

            foreach (var item in InitD.x)
            {
                InitData.gorCount++;
                InitData.gorIndex++;
                string disp = InitData.gorIndex.ToString() + ". " + item;
                gorKoorListBox.Items.Add(disp);
            }
        }

        private void SetKoorY()
        {
            InitD.y.Add(0); InitD.y.Add(0.3417); InitD.y.Add(0.372);
            InitD.y.Add(0.383); InitD.y.Add(0.387); InitD.y.Add(0.388);
            InitD.y.Add(0.385); InitD.y.Add(0.378); InitD.y.Add(0.369);
            InitD.y.Add(0.358); InitD.y.Add(0.343); InitD.y.Add(0.327);
            InitD.y.Add(0.309); InitD.y.Add(0.289); InitD.y.Add(0.264);
            InitD.y.Add(0.222); InitD.y.Add(-0.101); InitD.y.Add(-0.126);
            InitD.y.Add(-0.139); InitD.y.Add(-0.15); InitD.y.Add(-0.162);
            InitD.y.Add(-0.17); InitD.y.Add(-0.18); InitD.y.Add(-0.186);
            InitD.y.Add(-0.190); InitD.y.Add(-0.192); InitD.y.Add(-0.191);
            InitD.y.Add(-0.185); InitD.y.Add(-0.176); InitD.y.Add(-0.148);

            foreach (var item in InitD.y)
            {
                InitData.verCount++;
                InitData.verIndex++;
                string disp = InitData.verIndex.ToString() + ". " + item;
                verKoorListBox.Items.Add(disp);
            }
        }

        private void SetF()
        {
            InitD.f.Add(0.0004); InitD.f.Add(0.0016); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.0008); InitD.f.Add(0.0006); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00145);
            InitD.f.Add(0.00145); InitD.f.Add(0.00145); InitD.f.Add(0.00125);

            foreach (var item in InitD.f)
            {
                InitData.plochCount++;
                InitData.plochIndex++;
                string disp = InitData.plochIndex.ToString() + ". " + item;
                plochListBox.Items.Add(disp);
            }
        }

        private void SetDlt()
        {
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0081); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135);
            InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0135); InitD.dlt.Add(0.0168);
            InitD.dlt.Add(0.0135);

            foreach (var item in InitD.dlt)
            {
                InitData.dltCount++;
                InitData.dltIndex++;
                string disp = InitData.dltIndex.ToString() + ". " + item;
                dltListBox.Items.Add(disp);
            }
        }
        //---------------------------------------------------

        //*******************************************************************
        private void lNameTextBox_TextChanged(object sender, EventArgs e)
        {
            _an.InitD.LName = lNameTextBox.Text;
        }

        private void fNameTextBox_TextChanged(object sender, EventArgs e)
        {
            _an.InitD.FName = fNameTextBox.Text;
        }

        private void sNameTextBox_TextChanged(object sender, EventArgs e)
        {
            _an.InitD.SName = sNameTextBox.Text;
        }

        private void numElNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _an.InitD.NumEl = (int)numElNumericUpDown.Value;
        }

        private void _2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _an.InitD.NumLon = 2;
        }

        private void _4RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            _an.InitD.NumLon = 4;
        }

        private void numLonComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetModYprFFZ(ref _an.InitD.NumMatLon, 0, numLonComboBox.SelectedIndex);
            if (_an.InitD.ModYprFFZName[1] != null)
            { modYprFFZComboBox.Items.Clear(); modYprFFZComboBox.Items.AddRange(_an.InitD.ModYprFFZName); }
            else
            { modYprFFZComboBox.Items.Clear(); modYprFFZComboBox.Items.Add(_an.InitD.ModYprFFZName[0]); }
        }

        private void numStrComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetModYprFFZ(ref _an.InitD.NumMatStringer, 1, numStrComboBox.SelectedIndex);
            if (_an.InitD.ModYprFFZName[0] != null)
            { modYprFFZComboBox.Items.Clear(); modYprFFZComboBox.Items.AddRange(_an.InitD.ModYprFFZName); }
            else
            { modYprFFZComboBox.Items.Clear(); modYprFFZComboBox.Items.Add(_an.InitD.ModYprFFZName[1]); }
        }

        private void numPVPLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (numPVPLTextBox.Text!="")
                _an.InitD.NumLonPVPL = Convert.ToInt32(numPVPLTextBox.Text);
        }
        
        private void numPVPLTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void numZVPLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (numZVPLTextBox.Text != "")
                _an.InitD.NumLonZVPL = Convert.ToInt32(numZVPLTextBox.Text);
        }
        
        private void numZVPLTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void numPNPLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (numPNPLTextBox.Text != "")
                _an.InitD.NumLonPNPL = Convert.ToInt32(numPNPLTextBox.Text);
        }

        private void numPNPLTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void numZNPLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (numZNPLTextBox.Text != "")
                _an.InitD.NumLonZNPL = Convert.ToInt32(numZNPLTextBox.Text);
        }
        
        private void numZNPLTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void modYprFFZComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (modYprFFZComboBox.SelectedIndex)
            {
                case 0:
                    _an.InitD.ModUprFD = _an.InitD.ModYprFFZValue[0];
                    _an.InitD.ChoiceMat = _an.InitD.NumMatLon;
                    break;
                case 1:
                    _an.InitD.ModUprFD = _an.InitD.ModYprFFZValue[1];
                    _an.InitD.ChoiceMat = _an.InitD.NumMatStringer;
                    break;
            } 
        }

        private void sigmaDopPLTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sigmaDopPLTextBox.Text != "")
                _an.InitD.PNaprSgLon = (float)Convert.ToDecimal(sigmaDopPLTextBox.Text);
        }
        
        private void sigmaDopPLTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void sigmaDopStrTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sigmaDopStrTextBox.Text != "")
                _an.InitD.PNaprSgStr = (float)Convert.ToDecimal(sigmaDopStrTextBox.Text);
        }

        private void sigmaDopStrTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void mtTextBox_TextChanged(object sender, EventArgs e)
        {
            if (mtTextBox.Text != "")
                _an.InitD.Mt = (float)Convert.ToDecimal(mtTextBox.Text);
        }

        private void mtTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void mnTextBox_TextChanged(object sender, EventArgs e)
        {
            if (mnTextBox.Text != "")
                _an.InitD.Mn = (float)Convert.ToDecimal(mnTextBox.Text);
        }

        private void mnTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void qnTextBox_TextChanged(object sender, EventArgs e)
        {
            if (qnTextBox.Text != "")
                _an.InitD.Qn = (float)Convert.ToDecimal(qnTextBox.Text);
        }

        private void qnTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void qtTextBox_TextChanged(object sender, EventArgs e)
        {
            if (qtTextBox.Text != "")
                _an.InitD.Qn = (float)Convert.ToDecimal(qtTextBox.Text);
        }

        private void qtTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void nTextBox_TextChanged(object sender, EventArgs e)
        {
            if (nTextBox.Text != "")
                _an.InitD.N = (float)Convert.ToDecimal(nTextBox.Text);
        }

        private void nTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigit(e);
        }

        private void eTextBox_TextChanged(object sender, EventArgs e)
        {
            if (eTextBox.Text != "")
            {
                string tmp = eTextBox.Text;
                if (tmp.Contains("."))
                    tmp = tmp.Replace(".", ",");//учет и запятых и точек
                _an.InitD.E = (double)Convert.ToDouble(tmp);
            }
        }

        private void eTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigitOrPoint(e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage2;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void addGorKoorButton_Click(object sender, EventArgs e)
        {
            addButton(ref _an.InitD.x, gorKoorListBox, ref InitData.gorCount, ref InitData.gorIndex, gorKoorTextBox, _an.InitD.NumEl);
        }

        private void gorKoorTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigitOrPoint(e);
        }

        private void delGorKoorButton_Click(object sender, EventArgs e)
        {
            delButton(ref _an.InitD.x, gorKoorListBox, ref InitData.gorCount, ref InitData.gorIndex);
        }

        private void editGorKoorButton_Click(object sender, EventArgs e)
        {
            _edGorForm = new EditGorForm(ref gorKoorListBox, ref _an.InitD.x);
            _edGorForm.ShowDialog();
        }

        private void clearGorKoorButton_Click(object sender, EventArgs e)
        {
            clearButton(ref _an.InitD.x, gorKoorListBox, ref InitData.gorCount, ref InitData.gorIndex);
        }

        private void addVerKoorButton_Click(object sender, EventArgs e)
        {
            addButton(ref _an.InitD.y, verKoorListBox, ref InitData.verCount, ref InitData.verIndex, verKoorTextBox, _an.InitD.NumEl);
        }

        private void verKoorTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigitOrPoint(e);
        }

        private void delVerKoorButton_Click(object sender, EventArgs e)
        {
            delButton(ref _an.InitD.y, verKoorListBox, ref InitData.verCount, ref InitData.verIndex);
        }

        private void editVerKoorButton_Click(object sender, EventArgs e)
        {
            _edVerForm = new EditVerForm(ref verKoorListBox, ref _an.InitD.y);
            _edVerForm.ShowDialog();
        }

        private void clearVerKoorButton_Click(object sender, EventArgs e)
        {
            clearButton(ref _an.InitD.y, verKoorListBox, ref InitData.verCount, ref InitData.verIndex);
        }

        private void addPlochButton_Click_1(object sender, EventArgs e)
        {
            addButton(ref _an.InitD.f, plochListBox, ref InitData.plochCount, ref InitData.plochIndex, plochTextBox, _an.InitD.NumEl);
        }

        private void plochTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigitOrPoint(e);
        }

        private void delPlochButton_Click(object sender, EventArgs e)
        {
            delButton(ref _an.InitD.f, plochListBox, ref InitData.plochCount, ref InitData.plochIndex);
        }

        private void editPlochButton_Click(object sender, EventArgs e)
        {
            _edPlochForm = new EditPlochForm(ref plochListBox, ref _an.InitD.f);
            _edPlochForm.ShowDialog();
        } 

        private void clearPlochButton_Click_1(object sender, EventArgs e)
        {
            clearButton(ref _an.InitD.f, plochListBox, ref InitData.plochCount, ref InitData.plochIndex);
        }

        private void addDltButton_Click(object sender, EventArgs e)
        {
            addDltFuncButton(ref _an.InitD.dlt, dltListBox, ref InitData.dltCount, ref InitData.dltIndex, dltTextBox, _an.InitD.NumEl);
        }

        private void dltTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            BlockNotDigitOrPoint(e);
        }

        private void delDltButton_Click(object sender, EventArgs e)
        {
            delButton(ref _an.InitD.dlt, dltListBox, ref InitData.dltCount, ref InitData.dltIndex);
        }

        private void editDltButton_Click(object sender, EventArgs e)
        {
            _edDltForm = new EditDltForm(ref dltListBox, ref _an.InitD.dlt);
            _edDltForm.ShowDialog();
        }

        private void clearDltButton_Click(object sender, EventArgs e)
        {
            clearButton(ref _an.InitD.dlt, dltListBox, ref InitData.dltCount, ref InitData.dltIndex);
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            this.tabControl1.SelectedTab = this.tabPage1;
        }

        private void cancelFormButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void startAnalisButton_Click(object sender, EventArgs e)
        {
            try
            {
                checkInitData();
                _an.cfRedNull(_an.InitD.PNaprSgLon, _an.InitD.PNaprSgStr, _an.InitD.PNaprSgLon, _an.InitD.SigmaMaterFFZ[1]);
                _an.redF();
                _an.detCtx();
                _an.detCty();
                _an.detCx();
                _an.detCy();
                _an.detMomInX();
                _an.detMomInY();
                _an.detMomInXY();
                _an.detBeta();
                _an.detU();
                _an.detV();
                _an.detMU();
                _an.detMV();
                _an.detMomInU();
                _an.detMomInV();
                _an.detNNapR();
                _an.InitD.ChoiceMat = 2;
                _an.detMasElFFZ(_an.InitD.ChoiceMat);
                _an.detNNapD(_an.InitD.NumMatLon, _an.InitD.NumMatStringer, _an.InitD.MasElNumLon);
                _an.detCfRedNext();
                createReport();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void TestButton_Click(object sender, EventArgs e)
        {
            TestSystem();
            _an = new Analysis(InitD);
        }  
    }
}
