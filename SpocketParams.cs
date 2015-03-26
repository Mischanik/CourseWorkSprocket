using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sprocket
{
    /// <summary>
    /// Класс инкапсулирующий параметры детали
    /// </summary>
    public class SporcketParams
    {
        #region Приватные поля параметров
        /// <summary>
        /// Радиус внешней окружности (а)
        /// </summary>
        private readonly Parameter _radiusA = new Parameter { Value = 125, Name = @"RadiusA" };

        /// <summary>
        /// Радиус цилиндра (b)
        /// </summary>
        private readonly Parameter _radiusB = new Parameter { Value = 45, Name = @"RadiusB" };

        /// <summary>
        ///Радиус отверстия в цилиндре (c)
        /// </summary>
        private readonly Parameter _radiusC = new Parameter { Value = 25, Name = @"RadiusC" };
        
        /// <summary>
        ///Высота внешней части звездочки (a)
        /// </summary>
        private readonly Parameter _lengthA = new Parameter { Value = 15, Name = @"LengthA"};
       
        /// <summary>
        ///Высота цилиндра (b)
        /// </summary>
        private readonly Parameter _lengthB = new Parameter { Value = 35, Name = @"LengthB"};
       
        /// <summary>
        ///Высота цилиндра в обратню сторону 
        /// </summary>
        private readonly Parameter _lengthC = new Parameter { Value = 15, Name = @"LengthC"};

        /// <summary>
        ///дальность выдавливание выемки (е)
        /// </summary>
        private readonly Parameter _lengthE = new Parameter { Value = 10, Name = @"LengthE" };

        /// <summary>
        /// количество окружностей d
        /// </summary>
        private readonly Parameter _numberD = new Parameter { Value = 6, Name = @"NumberD"};

        /// <summary>
        /// радиус окружности d
        /// </summary>
        private readonly Parameter _radiusD = new Parameter { Value = 20, Name = @"RadiusD" };
        /// <summary>
        /// радиус окружности в выемке (f)
        /// </summary>
        private readonly Parameter _radiusF = new Parameter { Value = 5, Name = @"RadiusF" };


        /// <summary>
        /// высота зуба звезды
        /// </summary>
        private readonly Parameter _depthOfTooth = new Parameter { Value = 10, Name = @"DepthOfTooth" };


        /// <summary>
        /// Список параметров
        /// </summary>
        private List<Parameter> _parameters;
        #endregion

        #region Публичные поля параметров
       
      
        /// <summary>
        /// Список параметров текущего объекта.
        /// </summary>
        public List<Parameter> Parameters
        {
            get
            {
                _parameters = new List<Parameter>
                {
                    _radiusA,
                    _radiusB,
                    _radiusC,
                    _radiusD,
                    _numberD,
                    _lengthA,
                    _lengthB,
                    _lengthC,
                    _lengthE,
                    _radiusF,
                    _depthOfTooth
                };
                return _parameters;
            }
        }
        #endregion

        
        #region свойство
    
        /// <summary>
        ///get/set радиуса а
        /// </summary>
        public double RadiusA
        {
            get
            {
                return _radiusA.Value;
            }
            set
            {
                _radiusA.Value = value;
                
            }
        }

        /// <summary>
        /// get/set радиуса В
        /// </summary>
        public double RadiusB
        {
            get
            {
                return _radiusB.Value;
            }
            set
            {
                _radiusB.Value = value;
              
            }
        }
        /// <summary>
        /// get/set радиуса С
        /// </summary>
        public double RadiusC
        {
            get
            {
                return _radiusC.Value;
            }
            set
            {
                _radiusC.Value = value;

            }
        }
        /// <summary>
        /// get/set дальности выдавливания а
        /// </summary>
        public double LengthA
        {
            get { return _lengthA.Value; }
            set { _lengthA.Value = value; }
        }
        /// <summary>
        /// get/set дальности выдавливания B
        /// </summary>
        public double LengthB
        {
            get { return _lengthB.Value; }
            set { _lengthB.Value = value; }
        }
        /// <summary>
        /// get/set дальности выдавливания C
        /// </summary>
        public double LengthC
        {
            get { return _lengthC.Value; }
            set { _lengthC.Value = value; }
        }
        /// <summary>
        /// get/set дальности выдавливания E
        /// </summary>
        public double LengthE
        {
            get { return _lengthE.Value; }
            set { _lengthE.Value = value; }
        }

        /// <summary>
        /// get/set диаметра D
        /// </summary>
        public double RadiusD
        {
            get { return _radiusD.Value; }
            set { _radiusD.Value = value; }
        }

        /// <summary>
        /// get/set радиуса F
        /// </summary>
        public double RadiusF
        {
            get { return _radiusF.Value; }
            set { _radiusF.Value = value; }
        }

        /// <summary>
        /// get/set количества отверстий
        /// </summary>
        public double NumberD
        {
            get { return _numberD.Value; }
            set { _numberD.Value = value; }
        }

        /// <summary>
        ///get/set радиуса а
        /// </summary>
        public double DepthOfTooth
        {
            get
            {
                return _depthOfTooth.Value;
            }
            set
            {
                _depthOfTooth.Value = value;

            }
        }
        #endregion

        /// <summary>
        ///устанавливает стандартные значения
        /// </summary>
        public void DefaultValues()
        {
            
            RadiusA = 125;
            RadiusB = 45;
            RadiusC = 25;
            RadiusD = 20;
            RadiusF = 5;
            LengthA = 15;
            LengthB = 35;
            LengthC = 15;
            NumberD = 6;
            LengthE = 10;
            DepthOfTooth = 10;
        }
    }
}