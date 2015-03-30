using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace Sprocket 
{
    public partial class SporcketGUI : Form
    {
        /// <summary>
        /// Создаем каталог параметров
        /// </summary>
        private readonly Dictionary<string, double> _sporcketParam;

        /// <summary>
        /// Список для хранения NumericUpDown.
        /// </summary>
        private readonly List<NumericUpDown> _numericUpDown = new List<NumericUpDown>();

        /// <summary>
        /// Конструктор формы
        /// </summary>
        public SporcketGUI()
        {
            //Здесь создаем объект и вводим параметры звезды    
            _sporcketParam = new Dictionary<string, double>();
            InitializeParametes();

            //Инициализируется форма
            InitializeComponent();

            //Заполняется словарь текстбоксов с флагами по умолчанию.
            foreach (Control control in groupBoxParam.Controls.Cast<Control>().
                Where(control => control.GetType() == typeof(NumericUpDown)))
            {
                _numericUpDown.Add((NumericUpDown)control);
            }

           
        }

        #region Обработчики
        /// <summary>
        /// Обработка нажатия кнопки "Построить деталь"
        /// </summary>
        private void ButtonStart_Click(object sender, EventArgs e)
        {
            foreach (Control control in groupBoxParam.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    for (int count = 0; count < _sporcketParam.Count; count++)
                    {
                        string key = _sporcketParam.ElementAt(count).Key;
                        if (control.Name == (@"numericUpDown" + key))
                        {
                            _sporcketParam[key] = Double.Parse(control.Text);
                            break;
                        }
                    }
                }
            }

            //Запуск инвентора
            InventorManager inventorManager = new InventorManager();

            // Проверка указателя на приложение.
            if (inventorManager.InvApp != null)
            {
                //Создаем билдер и передаем указатель на приложение
                Builder concreteBuilder = new Builder(inventorManager.InvApp);

                //Строим конкретный объект
                concreteBuilder.Build(_sporcketParam);
            }
            else
            {
                MessageBox.Show(@"Не удалось запустить Inventor");
            }
        }

      
        #endregion

        /// <summary>
        /// Обработчик нажания кнопки "по умолчанию"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonDefault_Click(object sender, EventArgs e)
        {
            // сброс параметров на стандартные
            InitializeParametes();

            // заполнение формы стандартными значениями
            foreach (Control control in groupBoxParam.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    foreach (string parameter in _sporcketParam.Keys)
                    {
                        if (control.Name == (@"numericUpDown" + parameter))
                        {
                            control.Text = _sporcketParam[parameter].ToString();
                        }
                    }
                }
            }
        }


        //инициализация параметров
        public void InitializeParametes()
        {
            _sporcketParam.Clear();
            _sporcketParam.Add("RadiusA", 125.0);
            _sporcketParam.Add("RadiusB", 45.0);
            _sporcketParam.Add("RadiusC", 30.0);
            _sporcketParam.Add("RadiusD", 20.0);
            _sporcketParam.Add("RadiusF", 5.0);
            _sporcketParam.Add("LengthA", 15.0);
            _sporcketParam.Add("LengthB", 40.0);
            _sporcketParam.Add("LengthC", 15.0);
            _sporcketParam.Add("LengthE", 10.0);
            _sporcketParam.Add("NumberD", 6.0);
            
            _sporcketParam.Add("DepthOfTooth", 10.0);
        }
    }
}
