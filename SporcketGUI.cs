using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;

namespace Sprocket 
{
    public partial class SporcketGUI : Form
    {
        /// <summary>
        /// Создаем объект в поле видимости всего класса,
        /// чтобы можно было получить его в любом обработчике и методе.
        /// </summary>
        private readonly SporcketParams _sporcket;

        /// <summary>
        /// Список для хранения NumericUpDown.
        /// </summary>
        private readonly List<NumericUpDown> _numericUpDown = new List<NumericUpDown>();

        /// <summary>
        /// Конструктор формы
        /// </summary>
        public SporcketGUI()
        {
            //Здесь создаем объект инкапсулирующий параметры звезды для того, чтобы
            //можно было сразу вставить дефолтные значения в текстбоксы   
            _sporcket = new SporcketParams();

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
                    foreach (Parameter parameter in _sporcket.Parameters)
                    {
                        if (control.Name == (@"numericUpDown" + parameter.Name))
                        {
                            parameter.Value = Double.Parse(control.Text);
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
                concreteBuilder.Build(_sporcket);
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
            _sporcket.DefaultValues();

            // заполнение формы стандартными значениями
            foreach (Control control in groupBoxParam.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    foreach (Parameter parameter in _sporcket.Parameters)
                    {
                        if (control.Name == (@"numericUpDown" + parameter.Name))
                        {
                            control.Text = parameter.Value.ToString();
                        }
                    }
                }
            }
        }

        
    }
}
