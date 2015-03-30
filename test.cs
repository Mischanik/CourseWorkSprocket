using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Inventor;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using File = System.IO.File;
namespace Sprocket
{
    [TestFixture, Description("Тесты строителя")]
    internal class Test
    {

        /// <summary>
        /// Менеджер запуска приложения
        /// </summary>
        private readonly InventorManager _manager = new InventorManager();

        /// <summary>
        /// параметры для тестирования.
        /// </summary>
        private Dictionary<string, double> _sporcketParam;

        /// <summary>
        /// Сам билдер для тестирования.
        /// </summary>
        
        private Builder _builder;
      /*  [Test, Description("Нагрузочный тест")]
        public void LoadTest()
        {
            _sporcketParam = new Dictionary<string, double>
            {
                {"RadiusA", 125.0},
                {"RadiusB", 45.0},
                {"RadiusC", 30.0},
                {"RadiusD", 20.0},
                {"RadiusF", 5.0},
                {"LengthA", 15.0},
                {"LengthB", 40.0},
                {"LengthC", 15.0},
                {"LengthE", 10.0},
                {"NumberD", 6.0},
                {"DepthOfTooth", 10.0}
            };
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
            //excelApp.Visible = true;

            //Get a new workbook.
            nwb = excelApp.Workbooks.Add();
            nws = (Worksheet) nwb.ActiveSheet;

            for (int i = 1; i < 50; i++)
            {
                stopwatch.Start();

                _builder = new Builder(_manager.InvApp);

                //Проверяем, что постройка не выкинула ексепшн.
                Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));
                docCount = _manager.InvApp.Documents.Count - 1;
                stopwatchCount = stopwatch.ElapsedMilliseconds;
                stopwatch.Reset();

                nws.Cells[i, 1] = docCount;
                nws.Cells[i, 2] = stopwatchCount;

                Console.WriteLine(i);
            }
        

        }
        */
        [Test, Description("Позитивное стандартное построение")]
        public void BuildPositive()
        {
            _sporcketParam = new Dictionary<string, double>
            {
                {"RadiusA", 125.0},
                {"RadiusB", 45.0},
                {"RadiusC", 30.0},
                {"RadiusD", 20.0},
                {"RadiusF", 5.0},
                {"LengthA", 15.0},
                {"LengthB", 40.0},
                {"LengthC", 15.0},
                {"LengthE", 9.0},
                {"NumberD", 6.0},
                {"DepthOfTooth", 10.0}
            };

            
                _builder = new Builder(_manager.InvApp);

                //Проверяем, что постройка не выкинула ексепшн.
                Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));
            
            
            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }
        [Test, Description("Позитивное построение с максимальными параметрами")]
        public void BuildMax()
        {
            _sporcketParam = new Dictionary<string, double>
            {
                {"RadiusA", 350.0},
                {"RadiusB", 150.0},
                {"RadiusC", 50.0},
                {"RadiusD", 30.0},
                {"RadiusF", 10.0},
                {"LengthA", 30.0},
                {"LengthB", 70.0},
                {"LengthC", 50.0},
                {"LengthE", 9.0},
                {"NumberD", 10.0},
                {"DepthOfTooth", 10.0}
            };


            _builder = new Builder(_manager.InvApp);

            //Проверяем, что постройка не выкинула ексепшн.
            Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));


            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }


        [Test, Description("Позитивное построение с мимимальными параметрами")]
        public void BuildMin()
        {
            _sporcketParam = new Dictionary<string, double>
            {
                 {"RadiusA", 100.0},
                {"RadiusB", 60.0},
                {"RadiusC", 20.0},
                {"RadiusD", 10.0},
                {"RadiusF", 2.0},
                {"LengthA", 10.0},
                {"LengthB", 30.0},
                {"LengthC", 10.0},
                {"LengthE", 5.0},
                {"NumberD",2.0},
                {"DepthOfTooth", 3.0}
            };


            _builder = new Builder(_manager.InvApp);

            //Проверяем, что постройка не выкинула ексепшн.
            Assert.DoesNotThrow(() => _builder.Build(_sporcketParam));


            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }




    /*    [Test, Description("Негативное построение")]
        public void BuildNegative()
        {
            _sporcketParam = new Dictionary<string, double>
            {
                {"RadiusA", 100.0},
                {"RadiusB", 60.0},
                {"RadiusC", 20.0},
                {"RadiusD", 10.0},
                {"RadiusF", 2.0},
                {"LengthA", 10.0},
                {"LengthB", 30.0},
                {"LengthC", 10.0},
                {"LengthE", 5.0},
                {"NumberD",2.0},
                {"DepthOfTooth", 3.0}
            };
            
            _builder = new Builder(_manager.InvApp);

            //Проверяем что постройка выкинула ексепшн.

            Assert.Throws<ArgumentNullException>(() => _builder.Build(_sporcketParam));

            
            foreach (Document document in _manager.InvApp.Documents)
            {
                document.Close(true);
            }
        }
        *********************/
    }
        

}
