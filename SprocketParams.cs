
using System.Collections.Generic;


namespace Sprocket
{
    /// <summary>
    /// Класс инкапсулирующий параметры детали
    /// </summary>
    public class SprocketParams
    {
        #region Публичные поля параметров

        
        /// <summary>
        /// публичный список параметров текущего объекта.
        /// </summary>
        public Dictionary<string, Parameter> Parameters {  get; set;  }
        #endregion


        /// <summary>
        /// конструктор заполняет параметры 
        /// </summary>
        public SprocketParams()
        {
             DefaultValues();
        }

        /// <summary>
        /// устанавливает стандартные значения
        /// </summary>
        public void DefaultValues()
        {
            Parameters = new Dictionary<string, Parameter>
                {
                    {"RadiusA", new Parameter { Min = 100,  Max = 350, Value = 125 }},
                    {"RadiusB", new Parameter { Min = 45,   Max = 150, Value = 60 }},
                    {"RadiusC", new Parameter { Min = 20,   Max = 50, Value = 25 }},
                    {"RadiusD", new Parameter { Min = 10,   Max = 30, Value = 20 }},
                    {"RadiusF", new Parameter { Min = 2,    Max = 10, Value = 5 }},
                    {"LengthA", new Parameter { Min = 10,   Max = 30, Value = 15 }},
                    {"LengthB", new Parameter { Min = 30,   Max = 70, Value = 35 }},
                    {"LengthC", new Parameter { Min = 10,   Max = 50, Value = 15 }},
                    {"LengthE", new Parameter { Min = 5,    Max = 9, Value = 6 }},
                    {"NumberD", new Parameter { Min = 2,    Max = 10,Value = 6 }},
                    {"DepthOfTooth", new Parameter { Min = 3, Max = 10, Value = 10 }}
                };
        }
        
    }
}