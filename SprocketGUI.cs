using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;


namespace Sprocket
{
    public partial class SprocketGUI : Form
    {
        /// <summary>
        //Здесь создаем объект и вводим параметры звезды    
        /// </summary>
        private readonly SprocketParams _sporcket;

        /// <summary>
        /// Список для хранения NumericUpDown.
        /// </summary>
        private readonly List<NumericUpDown> _numericUpDownControlList = new List<NumericUpDown>();
      
        /// <summary>
        /// Конструктор формы
        /// </summary>
        public SprocketGUI()
        {
            //Здесь создаем объект инкапсулирующий параметры звезды для того, чтобы
            //можно было сразу вставить дефолтные значения в текстбоксы   
            _sporcket = new SprocketParams();
            
            //Инициализируется форма
            InitializeComponent();

            //Заполняется список нумериков.
            foreach (Control control in groupBoxParam.Controls.Cast<Control>().
                Where(control => control.GetType() == typeof(NumericUpDown)))
            {
                _numericUpDownControlList.Add((NumericUpDown)control);
            }
            //загрузка параметров на форму
            LoadParamToForm();

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
                    foreach (KeyValuePair<string, Parameter> parameter in _sporcket.Parameters)
                    {
                        if (control.Name == (@"numericUpDown" + parameter.Key))
                        {
                            parameter.Value.Value = Double.Parse(control.Text);
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
            LoadParamToForm();

        }

        private void LoadParamToForm()
        {
            // заполнение формы стандартными значениями
            foreach (Control control in groupBoxParam.Controls)
            {
                if (control.GetType() == typeof(NumericUpDown))
                {
                    foreach (KeyValuePair<string, Parameter> parameter in _sporcket.Parameters)
                    {
                        if (control.Name == (@"numericUpDown" + parameter.Key))
                        { 
                            ((NumericUpDown) control).Minimum = (decimal)parameter.Value.Min;
                            ((NumericUpDown) control).Maximum = (decimal)parameter.Value.Max;
                            ((NumericUpDown) control).Value = (decimal)parameter.Value.Value;
                           
                        }
                    }
                }
            }
        }

     
    }
}
