
using System.Collections.Generic;


namespace Sprocket
{
    /// <summary>
    /// Класс инкапсулирующий параметры детали
    /// </summary>
    public class SprocketParams
    {
        public string ParamString { get; set; }



        /// <summary>
        /// Cписок параметров текущего объекта.
        /// </summary>
        public Dictionary<string, Parameter> Parameters {  get; set;  }
        


        /// <summary>
        /// Конструктор заполняет параметры стандартными значениями
        /// </summary>
        public SprocketParams()
        {
             DefaultValues();
        }

        /// <summary>
        /// Устанавливает стандартные значения
        /// </summary>
        public void DefaultValues()
        {
            Parameters = new Dictionary<string, Parameter>
                {
                    {"RadiusA", new Parameter { Min = 150,  Max = 350, Value = 200 }},
                    {"RadiusB", new Parameter { Min = 80,   Max = 120, Value = 80 }},
                    {"RadiusC", new Parameter { Min = 20,   Max = 50, Value = 50 }},
                    {"RadiusD", new Parameter { Min = 10,   Max = 30, Value = 15}},
                    {"RadiusF", new Parameter { Min = 2,    Max = 10, Value = 5 }},
                    {"LengthA", new Parameter { Min = 10,   Max = 30, Value = 15 }},
                    {"LengthB", new Parameter { Min = 30,   Max = 70, Value = 35 }},
                    {"LengthC", new Parameter { Min = 10,   Max = 50, Value = 15 }},
                    {"LengthE", new Parameter { Min = 5,    Max = 9, Value = 6 }},
                    {"NumberD", new Parameter { Min = 2,    Max = 8,Value = 6 }},
                    {"DepthOfTooth", new Parameter { Min = 3, Max = 10, Value = 10 }},
                    {"Emboss", new Parameter {ParamTextValue = "Звездочка цепной передачи"  }}
                };
        }
        
    }
}