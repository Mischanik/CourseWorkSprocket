using System;
using System.Diagnostics;
using NUnit.Framework;
using Inventor;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
namespace Sprocket
{
    [TestFixture, Description("Тесты Sprocket")]
    internal class Test
    {

        /// <summary>
        /// Менеджер запуска приложения
        /// </summary>
        private readonly InventorManager _manager = new InventorManager();

        /// <summary>
        /// Метод закрытия приложения.
        /// </summary>
        public void ClearDoc()
        {
            // Подчищаем за собой
            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }

        /// <summary>
        /// параметры для тестирования.
        /// </summary>
        private SprocketParams _sporcketParam;

        /// <summary>
        /// билдер для тестирования.
        /// </summary>
        private Builder _builder;

        #region Тесты билдера

      
        [Test, Description("Позитивный тест для конструктора Builder")]
        public void BuilderPositive()
        {
            // Проверяем конструктор валидной ссылкой.
            Assert.DoesNotThrow(() => { new Builder(_manager.InvApp); });

            ClearDoc();
        }

        [Test, Description("Негативный тест для конструктора Builder")]
        public void BuilderNegative()
        {
            // Подаем невалидную ссылку на приложение.
            Assert.Throws<ArgumentNullException>(() => { new Builder(null); });
        }





        [Test, Description("стандартное построение")]
        public void BuildPositive()
        {
            _sporcketParam = new SprocketParams();

            _builder = new Builder(_manager.InvApp);

            //Проверяем, что постройка не выкинула исключение.
            Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));

            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }


        [Test, Description("построение с максимальными параметрами")]
        public void BuildMax()
        {
            _sporcketParam = new SprocketParams();
            
            _sporcketParam.Parameters["RadiusA"].Value = 350.0;
            _sporcketParam.Parameters["RadiusB"].Value = 150.0;
            _sporcketParam.Parameters["RadiusC"].Value = 50.0;
            _sporcketParam.Parameters["RadiusD"].Value = 30.0;
            _sporcketParam.Parameters["RadiusF"].Value = 10;
            _sporcketParam.Parameters["LengthA"].Value = 30.0;
            _sporcketParam.Parameters["LengthB"].Value = 70.0;
            _sporcketParam.Parameters["LengthC"].Value = 50.0;
            _sporcketParam.Parameters["NumberD"].Value = 10;
            _sporcketParam.Parameters["LengthE"].Value = 9.0;
            _sporcketParam.Parameters["DepthOfTooth"].Value = 10;
           
            _builder = new Builder(_manager.InvApp);

            //Проверяем, что постройка не выкинула исключение.
            Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));

            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }


        [Test, Description("построение с мимимальными параметрами")]
        public void BuildMin()
        {
            _sporcketParam = new SprocketParams();
            
                _sporcketParam.Parameters["RadiusA"].Value = 100;
            _sporcketParam.Parameters["RadiusB"].Value = 45;
            _sporcketParam.Parameters["RadiusC"].Value = 20.0;
            _sporcketParam.Parameters["RadiusD"].Value = 10.0;
            _sporcketParam.Parameters["RadiusF"].Value = 2;
            _sporcketParam.Parameters["LengthA"].Value = 10.0;
            _sporcketParam.Parameters["LengthB"].Value = 30.0;
            _sporcketParam.Parameters["LengthC"].Value = 10.0;
            _sporcketParam.Parameters["NumberD"].Value = 2;
            _sporcketParam.Parameters["LengthE"].Value = 5;
            _sporcketParam.Parameters["DepthOfTooth"].Value = 3;
                
            _builder = new Builder(_manager.InvApp);

            //Проверяем, что постройка не выкинула исключение.
            Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));


            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }

        #endregion

        #region Тесты параметров


        /// <summary>
        /// Тестируемый объект.
        /// </summary>
        private readonly Parameter _parameter = new Parameter
        {
            Max = 10,
            Min = 0
        };

        /// <summary>
        /// Невалидное значение.
        /// </summary>
        private const int TestValue = 11;

        [Test, Description("Позитивный тест валидного значения")]
        public void SetCorrectValue()
        {
            Assert.DoesNotThrow(() => { _parameter.Value = 5; });
        }

        [Test, Description("Негативный тест для невалидного значения, находящегося за границами.")]
        [TestCase(ExpectedException = typeof (ArgumentOutOfRangeException))]
        public void SetIncorrectValue()
        {
            _parameter.Value = TestValue;
        }

        [Test, Description("Тест того, что невалидное значение не сохраняется")]
        public void ValueIsNotSaved()
        {
            try
            {
                _parameter.Value = TestValue;
            }
            catch (Exception)
            {
                Assert.AreNotEqual(_parameter.Value, TestValue);
            }
        }


        #endregion

        [Test, Description("Нагрузочный тест")]
        public void LoadTest()
        {
         _sporcketParam = new SprocketParams();
            _builder = new Builder(_manager.InvApp);
            Stopwatch stopwatch = new Stopwatch();
            int docCount;
            long stopwatchCount;

            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }

            Application excelApp = new Application();
            _Workbook nwb = null;
            _Worksheet nws = null;
            excelApp.Visible = true;
            

            //Get a new workbook.
            nwb = excelApp.Workbooks.Add();
            nws = (Worksheet) nwb.ActiveSheet;

            for (int i = 1; i < 50; i++)
            {
                stopwatch.Start();

                _builder = new Builder(_manager.InvApp);

                //Проверяем, что постройка не выкинула исключение.
                Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));
                docCount = _manager.InvApp.Documents.Count - 1;
                stopwatchCount = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();

                nws.Cells[i, 1] = docCount;
                nws.Cells[i, 2] = stopwatchCount;

                Console.WriteLine(i);
            }


        }
    
    }
}
