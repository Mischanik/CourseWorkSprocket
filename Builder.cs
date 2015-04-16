using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Inventor;
using System.Drawing;


using System.Linq;
namespace Sprocket
{
    /// <summary>
    /// Класс инкапсулирующий процедуры простроения детали
    /// На вход в конструктор получает ссылку на прилоежние.
    /// </summary>
    internal class Builder
    {
        #region Перечисления
        /// <summary>
        /// выбор рабочей плоскости 1,2
        /// </summary>
        private enum WorkPlanesEnum{WorkPlane_ZY = 1, WorkPlane_ZX}
     
        #endregion

        #region Приватные поля
        /// <summary>
        /// Описание частей документа
        /// </summary>
        private readonly PartComponentDefinition _partDef;

        /// <summary>
        /// центр координат
        /// </summary>
        private readonly Point2d _centerPoint;

        /// <summary>
        /// Cсылка на приложение Inventor
        /// </summary>
        //private readonly Application _invApp;

        /// <summary>
        /// Конструктор, создающий документ, извлекающий описание документа и
        /// объект транс. геометрии.
        /// </summary>
        public Application InvApp { get; private set; }

        /// <summary>
        /// Объект инкапсулирующий всю 2D-3D геометрию, 
        /// инициализируется в конструкторе из приложения.
        /// </summary>
        private readonly TransientGeometry _transientGeometry;
        #endregion

        #region Конструкторы
        /// <summary>
        /// Конструктор, создающий документ.
        /// </summary>
        /// <param name="inputApp">Ссылка на приложение</param>
        public Builder(Application inputApp)
        {
            if (inputApp == null) throw new ArgumentNullException();

            //Получение ссылки на приложение.
            InvApp = inputApp;

            // В открытом приложении создаем документ
            var partDoc = (PartDocument)InvApp.Documents.Add
                (DocumentTypeEnum.kPartDocumentObject,
                    InvApp.FileManager.GetTemplateFile
                        (DocumentTypeEnum.kPartDocumentObject,
                            SystemOfMeasureEnum.kMetricSystemOfMeasure));

            // Описание документа
            _partDef = partDoc.ComponentDefinition;
            // Инициализация метода геометрии
            _transientGeometry = inputApp.TransientGeometry;

            _centerPoint = _transientGeometry.CreatePoint2d(0, 0);

        }
        #endregion

        #region Моделирование
        /// <summary>
        /// Метод для построения модели
        /// </summary>
        /// <param name="sprocketParams">Параметры звезды</param>
        public void Build(SprocketParams sprocketParams)
        {
            if (sprocketParams == null) throw new ArgumentNullException();

            //выдавливаем внешнюю часть звезды
            PlanarSketch sprocketSketch1 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            Point2d point = _transientGeometry.CreatePoint2d(0, 0);
            DrawCircle(sprocketParams.Parameters["RadiusA"].Value, sprocketSketch1, point);
            //ExtrudeDefinition extr =
            Extrude(sprocketSketch1,sprocketParams.Parameters["LengthA"].Value, PartFeatureOperationEnum.kJoinOperation, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
          
            // выдавливаем цилиндр
            PlanarSketch sprocketSketch2 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            DrawCircle(sprocketParams.Parameters["RadiusB"].Value, sprocketSketch2, point);
            ExtrudeFeature scratcExtrudeFeature =  Extrude(sprocketSketch2, sprocketParams.Parameters["LengthB"].Value + sprocketParams.Parameters["LengthA"].Value, PartFeatureOperationEnum.kJoinOperation, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            TurnCamera();
            if (!(sprocketParams.Parameters["Emboss"].ParamTextValue == ""))
            {
                int facesCount = scratcExtrudeFeature.Faces.Count;
                Face embossedFace = scratcExtrudeFeature.Faces[1];
                DrawingText(sprocketParams.Parameters["RadiusB"].Value, (sprocketParams.Parameters["LengthB"].Value / 1.5) + sprocketParams.Parameters["LengthA"].Value, 0.5, scratcExtrudeFeature, sprocketParams.Parameters["RadiusF"].Value, sprocketParams.Parameters["Emboss"].ParamTextValue);
            }
            // выдавливаем цилиндр в обратную сторону
            Extrude(sprocketSketch2, sprocketParams.Parameters["LengthC"].Value, PartFeatureOperationEnum.kJoinOperation, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
          
            // вырезаем отверстие в цилиндре
            PlanarSketch sprocketSketch3 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sprocketParams.Parameters["LengthA"].Value + sprocketParams.Parameters["LengthB"].Value);
            DrawCircle(sprocketParams.Parameters["RadiusC"].Value, sprocketSketch3, point);
            ExtrudeAll(sprocketSketch3, PartFeatureOperationEnum.kCutOperation, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
           

            //рисуем круг
            PlanarSketch sprocketSketch4 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d(0, (sprocketParams.Parameters["RadiusA"].Value - sprocketParams.Parameters["RadiusB"].Value - sprocketParams.Parameters["DepthOfTooth"].Value) / 2 + sprocketParams.Parameters["RadiusB"].Value);
            DrawCircle(sprocketParams.Parameters["RadiusD"].Value, sprocketSketch4, point);
             scratcExtrudeFeature = ExtrudeAll(sprocketSketch4, PartFeatureOperationEnum.kCutOperation, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
           
            //Коллекция фич для кругового массива
            ObjectCollection featureCollection = InvApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            //Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, sprocketParams.Parameters["NumberD"].Value, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);
            
            
            // выдвливание выемки
            PlanarSketch sprocketSketch5 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sprocketParams.Parameters["LengthB"].Value + sprocketParams.Parameters["LengthA"].Value);
            point = _transientGeometry.CreatePoint2d(0, 10);
            Point2d point2 = _transientGeometry.CreatePoint2d(sprocketParams.Parameters["RadiusC"].Value + sprocketParams.Parameters["LengthE"].Value, -10);
            sprocketSketch5.SketchLines.AddAsTwoPointRectangle(point, point2);
            ExtrudeAll(sprocketSketch5, PartFeatureOperationEnum.kCutOperation, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            


            // выдавливание отверстия в выемке
            PlanarSketch sprocketSketch6 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZY, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d((sprocketParams.Parameters["LengthB"].Value / 2) + sprocketParams.Parameters["LengthA"].Value, 0);
            DrawCircle(sprocketParams.Parameters["RadiusF"].Value, sprocketSketch6, point);
            ExtrudeAll(sprocketSketch6, PartFeatureOperationEnum.kCutOperation, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            
            // создание зубьев звездочки    
            ExtrudeArrayOfTeeth(sprocketParams);
            ChangeMaterial();
            TurnCamera();
        }

        #endregion

        #region Методы для моделирования
        
        /// <param name="workPlane"> Элемент определяющий плоскость</param>
        /// <param name="offset"> Отступ от начала координат</param>
        /// <returns> Полученный эскиз</returns>
        private PlanarSketch MakeNewSketch(WorkPlanesEnum workPlane, double offset)
        {
            WorkPlane mainPlane = _partDef.WorkPlanes[workPlane];
            WorkPlane offsetPlane = _partDef.WorkPlanes.AddByPlaneAndOffset(mainPlane, offset, false);
            PlanarSketch sketch = _partDef.Sketches.Add(offsetPlane, false);
            offsetPlane.Visible = false;
            // Возвращаем скетч вызвавшему методу
            return sketch;
        }
       

        /// <summary>
        /// Метод гравировки текста
        /// </summary>
        /// <param name="offset"> Отступ от центра</param>
        /// <param name="length"> Отступ по высоте </param>
        /// <param name="size"> Дальность выдавливания</param>
        /// <param name="scratcExtrudeFeature"> Грань на которую будет гравироваться текст</param>
        private void DrawingText(double offset, double length, double size , ExtrudeFeature scratcExtrudeFeature, double RadiusF,string text)
        {
            
            PlanarSketch sprocketSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZY, offset);

            // Если строка подается слишком длинная, то урезаем её до 30 символов 
            if (text.Length >= 30)
                text = text.Substring(0,30);
            for (int i = 10; i >= 1; i--)
            {
                Font font = new Font("Arial", i);
                Size size1 = System.Windows.Forms.TextRenderer.MeasureText(text, font);
                if (offset * Math.PI - RadiusF * 2  >= size1.Width)
                {
                    string formattedText = "<StyleOverride Font='ARIAL' FontSize='" + i + "'>" + text + " </StyleOverride>";
                    TextBox textBox = sprocketSketch.TextBoxes.AddFitted(_transientGeometry.CreatePoint2d(length, -RadiusF), formattedText);
                    textBox.Rotation = -Math.PI / 2;

                    int facesCount = scratcExtrudeFeature.SideFaces.Count;
                    Face embossedFace = scratcExtrudeFeature.SideFaces[1];
                    EmbossFeature emb = _partDef.Features.EmbossFeatures.AddEngraveFromFace(sprocketSketch.Profiles.AddForSolid(), size, PartFeatureExtentDirectionEnum.kPositiveExtentDirection, Type.Missing, embossedFace);

                    //Коллекция фич для кругового массива
                    ObjectCollection featureCollection = InvApp.TransientObjects.CreateObjectCollection();
                    featureCollection.Add(emb);

                    //Создание массива объектов
                    _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, 2, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);
                    break;
                }
            }
        }
        ///
        /// 
        /// <param name="sprocketSketch1"> Эскиз</param>
        /// <param name="sprocketParams"> Значение параметра</param>
        /// <paasdram name="operation"> enum Join/cut</param>
        /// <param name="direction"> enum positive/negative direction</param>
        /// <returns> Выдавленная деталь </returns>
        private ExtrudeFeature Extrude(PlanarSketch sprocketSketch,double paramValue, PartFeatureOperationEnum operation, PartFeatureExtentDirectionEnum direction)
        {
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sprocketSketch.Profiles.AddForSolid(), operation);
            extrudeDef.SetDistanceExtent(paramValue, direction);
            return _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
        }


        /// 
        /// <param name="sprocketSketch1"> Эскиз</param>
        /// <param name="sprocketParams"> Значение параметра</param>
        /// <param name="operation"> enum Join/cut</param>
        /// <param name="direction"> enum positive/negative direction</param>
        /// <returns> Выдавленная деталь </returns>
        private ExtrudeFeature ExtrudeAll(PlanarSketch sprocketSketch, PartFeatureOperationEnum operation, PartFeatureExtentDirectionEnum direction)
        {
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sprocketSketch.Profiles.AddForSolid(), operation);
            extrudeDef.SetThroughAllExtent(direction);
            return _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
        }
        /// <summary>
        /// Метод для рисования круга
        /// </summary>
        /// <param name="radius">радиус круга</param>
        /// <param name="sketch">Эскиз для рисования</param>
        /// <param name="centerPoint">Центр рисуемого круга</param>
        private  void DrawCircle(double radius, PlanarSketch sketch, Object centerPoint)
        {
            sketch.SketchCircles.AddByCenterRadius(centerPoint, radius);
        }


        /// <summary>
        ///выбор материала
        /// </summary>
        private void ChangeMaterial()
        {
            PartDocument partDoc = (PartDocument)InvApp.ActiveDocument;
            Materials materialsLibrary = partDoc.Materials;
            Material myMaterial = materialsLibrary["Steel"];

            //Проверка на то, что материал входит в текущую библиотеку
            Material tempMaterial = null;
            tempMaterial =
                myMaterial.StyleLocation == StyleLocationEnum.kLibraryStyleLocation
                    ? myMaterial.ConvertToLocal()
                    : myMaterial;

            //Меняем материал.
            partDoc.ComponentDefinition.Material = tempMaterial;
            //Обновляем документ.
            partDoc.Update();
        }





        /// <summary>
        /// Метод для создания массива зубьев звездочки.
        /// </summary>
        /// <param name="sporcketParams"> Параметры детали</param>
        private void ExtrudeArrayOfTeeth(SprocketParams sporcketParams)
        {
            PlanarSketch sprocketSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.Parameters["LengthA"].Value);
            Point2d point = _transientGeometry.CreatePoint2d(sporcketParams.Parameters["RadiusA"].Value, 0);
            // выдавливание круга скраю звездочки
            DrawCircle(sporcketParams.Parameters["DepthOfTooth"].Value, sprocketSketch, point);
            ExtrudeFeature scratcExtrudeFeature = ExtrudeAll(sprocketSketch, PartFeatureOperationEnum.kCutOperation, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
           
            // Коллекция фич для кругового массива
            ObjectCollection featureCollection = InvApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            // Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, ((sporcketParams.Parameters["RadiusA"].Value * Math.PI) / sporcketParams.Parameters["DepthOfTooth"].Value) * 0.9, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);

        }

        /// <summary>
        /// Метод для перемещения камеры
        /// </summary>
        private void TurnCamera()
        {
            
            Camera camera = InvApp.ActiveView.Camera;
            // Выбираем нужный ракурс
            camera.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
            // Меняем ракурс
            camera.Apply();
        }


        #endregion
    }
}
