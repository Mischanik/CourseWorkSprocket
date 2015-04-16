
using System;

namespace Sprocket
{
    /// <summary>
    /// Класс для инкапсуляции параметра.
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// Приватное значение параметра
        /// </summary>
        private double _paramValue;

        
        /// <summary>
        /// Публичное значение параметра
        /// </summary>
        public double Value
        {
            get { return _paramValue; }
            set
            {
                if (value >= Min && value <= Max)
                {
                    _paramValue = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Строковое значение параметра
        /// </summary>
        public string ParamTextValue { get; set; }

        /// <summary>
        /// Минимальная граница
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Максимальная граница
        /// </summary>
        public double Max { get; set; }
    }
}