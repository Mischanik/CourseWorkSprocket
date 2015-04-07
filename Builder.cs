using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Inventor;

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
        /// <param name="sporcketParams">Параметры звезды</param>
        public void Build(SprocketParams sporcketParams)
        {
            if (sporcketParams == null) throw new ArgumentNullException();

            //выдавливаем внешнюю часть звезды
            PlanarSketch sporcketSketch1 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            Point2d point = _transientGeometry.CreatePoint2d(0, 0);
            DrawCircle(sporcketParams.Parameters["RadiusA"].Value, sporcketSketch1, point);
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch1.Profiles.AddForSolid(), PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.Parameters["LengthA"].Value, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            
            // выдавливаем цилиндр

            PlanarSketch sporcketSketch2 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            DrawCircle(sporcketParams.Parameters["RadiusB"].Value, sporcketSketch2, point);

            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch2.Profiles.AddForSolid(), PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.Parameters["LengthB"].Value + sporcketParams.Parameters["LengthA"].Value, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            TurnCamera();
            // выдавливаем цилиндр в обратную сторону
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch2.Profiles.AddForSolid(), PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.Parameters["LengthC"].Value, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            // вырезаем отверстие в цилиндре
            PlanarSketch sporcketSketch3 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.Parameters["LengthA"].Value + sporcketParams.Parameters["LengthB"].Value);
            DrawCircle(sporcketParams.Parameters["RadiusC"].Value, sporcketSketch3, point);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch3.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            //рисуем круг

            PlanarSketch sporcketSketch4 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d(0, (sporcketParams.Parameters["RadiusA"].Value - sporcketParams.Parameters["RadiusB"].Value - sporcketParams.Parameters["DepthOfTooth"].Value) / 2 + sporcketParams.Parameters["RadiusB"].Value);
            DrawCircle(sporcketParams.Parameters["RadiusD"].Value, sporcketSketch4, point);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch4.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            ExtrudeFeature scratcExtrudeFeature = _partDef.Features.ExtrudeFeatures.Add(extrudeDef);


            //Коллекция фич для кругового массива
            ObjectCollection featureCollection = InvApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            //Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, sporcketParams.Parameters["NumberD"].Value, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);
            // выдвливание выемки
            PlanarSketch sporcketSketch5 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.Parameters["LengthB"].Value + sporcketParams.Parameters["LengthA"].Value);
            point = _transientGeometry.CreatePoint2d(0, 10);
            Point2d point2 = _transientGeometry.CreatePoint2d(sporcketParams.Parameters["RadiusC"].Value + sporcketParams.Parameters["LengthE"].Value, -10);
            sporcketSketch5.SketchLines.AddAsTwoPointRectangle(point, point2);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch5.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
            // выдавливание отверстия в выемке
            PlanarSketch sporcketSketch6 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZY, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d((sporcketParams.Parameters["LengthB"].Value / 2) + sporcketParams.Parameters["LengthA"].Value, 0);
            DrawCircle(sporcketParams.Parameters["RadiusF"].Value, sporcketSketch6, point);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch6.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
            // создание зубьев звездочки    
            ExtrudeArrayOfTeeth(sporcketParams);

            ChangeMaterial();
            TurnCamera();
        }

        #endregion

        #region Методы для моделирования
        /// <summary>
        /// Метод для создания нового эскиза
        /// [1 - ZY; 2 - ZX; 3 - XY]
        /// </summary>
        /// <param name="workPlane">Елемент определяющий плоскость</param>
        /// <param name="offset">Отступ от начала координат</param>
        /// <returns>Полученный эскиз</returns>
        private PlanarSketch MakeNewSketch(WorkPlanesEnum workPlane, double offset)
        {
            //Получаем ссылку на рабочую плоскость.
            WorkPlane mainPlane = _partDef.WorkPlanes[workPlane];
            //Делаем сдвинутую плоскость.
            WorkPlane offsetPlane = _partDef.WorkPlanes.AddByPlaneAndOffset(mainPlane, offset, false);
            //Создаем на плоскости скетч.
            PlanarSketch sketch = _partDef.Sketches.Add(offsetPlane, false);
            //прячем скетч от пользователя.
            offsetPlane.Visible = false;
            //возвращаем скетч вызвавшему методу
            return sketch;
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
            //Ссылка на документ
            PartDocument partDoc = (PartDocument)InvApp.ActiveDocument;

            //Получаем библиотеку
            Materials materialsLibrary = partDoc.Materials;

            //Берем необходимый материал
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
            PlanarSketch sporcketSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.Parameters["LengthA"].Value);
            Point2d point = _transientGeometry.CreatePoint2d(sporcketParams.Parameters["RadiusA"].Value, 0);
            // выдавливание круга скраю звездочки
            DrawCircle(sporcketParams.Parameters["DepthOfTooth"].Value, sporcketSketch, point);
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            ExtrudeFeature scratcExtrudeFeature = _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
            // Коллекция фич для кругового массива
            ObjectCollection featureCollection = InvApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            // Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, ((sporcketParams.Parameters["RadiusA"].Value * 3.14) / sporcketParams.Parameters["DepthOfTooth"].Value) * 0.9, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);

        }



        /// <summary>
        /// Метод для перемещения камеры
        /// </summary>
        private void TurnCamera()
        {
            // Получаем указатель на объект камеры
            Camera camera = InvApp.ActiveView.Camera;
            // Выбираем нужный ракурс
            camera.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
            // Меняем ракурс
            camera.Apply();
        }


        #endregion
    }
}
#region не работает 
// EdgeCollection edge = (

            // FilletDefinition = _partDef.Features.FilletFeatures.CreateFilletDefinition();
/*  PlanarSketch bevelSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_XY, 0);
  {
      Point2d pointA0 = _transientGeometry.CreatePoint2d(0, 0);
      Point2d pointA1 = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA-sporcketParams.DepthOfTooth, 0);
      SketchLine lineA = bevelSketch.SketchLines.AddByTwoPoints(pointA0, pointA1);
      Point2d pointB0 = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA, sporcketParams.LengthA/2);
      Point2d pointB1 = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA, sporcketParams.LengthA/2 - sporcketParams.LengthA*0.6/2);
      SketchLine lineB = bevelSketch.SketchLines.AddByTwoPoints(pointB0, pointB1);

      //var t = bevelSketch.SketchEntities[0].;

      bevelSketch.GeometricConstraints.AddPerpendicular((SketchEntity)lineA, (SketchEntity)lineB);
      Point2d pointC = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA, 0);
      Point2d pointC1 = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA - sporcketParams.DepthOfTooth / 2, 0);
      Point2d pointD = _transientGeometry.CreatePoint2d(pointA1.X, pointB1.Y);
      SketchArc arc = bevelSketch.SketchArcs.AddByCenterStartEndPoint(pointD, pointB1, pointA1, false);
      ObjectCollection points = _invApp.TransientObjects.CreateObjectCollection();
      points.Add(pointA1);
      points.Add(pointC1);
      points.Add(pointB1);
/*
      SketchSpline spline = bevelSketch.SketchSplines.Add(points, SplineFitMethodEnum.kSweetSplineFit);
      bevelSketch.GeometricConstraints.AddMidpoint(lineB.EndSketchPoint, lineB);
      bevelSketch.GeometricConstraints.AddMidpoint(lineA.EndSketchPoint, lineA);
      bevelSketch.GeometricConstraints.AddTangent((SketchEntity)lineA, (SketchEntity)spline);*/
/**/
/*  Point2d oPt1 = default(Point2d);
  oPt1 = _transientGeometry.CreatePoint2d(0, 0);

  Point2d oPt2 = default(Point2d);
  oPt2 = _transientGeometry.CreatePoint2d(0, -10);

  Point2d oPt3 = default(Point2d);
  oPt3 = _transientGeometry.CreatePoint2d(4, -5);

  SketchLine oLine = default(SketchLine);
  SketchLine oLine1 = default(SketchLine);

  oLine = bevelSketch.SketchLines.AddByTwoPoints(oPt1, oPt2);
  oLine1 = bevelSketch.SketchLines.AddByTwoPoints(bevelSketch.SketchLines[1].EndSketchPoint, oPt3);

  bevelSketch.GeometricConstraints.AddPerpendicular((SketchEntity)oLine, oLine1 as SketchEntity, false, false);
}*/
/*
          //Создаем прямоугольник для выдавливания
          PlanarSketch scratchSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, offset);
          scratchSketch.SketchLines.AddAsTwoPointRectangle(firstRectPoint, secondRectPoint);

          //Описание выдавливания.
          ExtrudeDefinition extrudeDef;
          if (direction)
          {
              extrudeDef =
                  _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(scratchSketch.Profiles.AddForSolid(),
                          PartFeatureOperationEnum.kJoinOperation);
              extrudeDef.SetDistanceExtent(extrudeValue, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
          }
          else
          {
              extrudeDef =
                  _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(scratchSketch.Profiles.AddForSolid(),
                          PartFeatureOperationEnum.kCutOperation);
              extrudeDef.SetDistanceExtent(extrudeValue, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
          }

          //Создаем само выдавливание по описанию.
          ExtrudeFeature scratcExtrudeFeature = _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

          //Коллекция фич для кругового массива
          ObjectCollection featureCollection = _invApp.TransientObjects.CreateObjectCollection();
          featureCollection.Add(scratcExtrudeFeature);

          //Создание массива объектов
          _partDef.Features.CircularPatternFeatures.
              Add(featureCollection, _partDef.WorkAxes[1], true, count, 360, true,
                  PatternComputeTypeEnum.kIdenticalCompute);
      */
#endregion