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

        private enum WorkPlanesEnum
        {
            WorkPlane_ZY = 1,
            WorkPlane_ZX = 2,
            WorkPlane_XY = 3,
        }

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
        private readonly Application _invApp;

        /// <summary>
        /// Конструктор, создающий документ, извлекающий описание документа и
        /// объект транс. геометрии.
        /// </summary>
        private Application InvApp { get; set; }

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
            _invApp = inputApp;

            // В открытом приложении создаем документ
            var partDoc = (PartDocument)_invApp.Documents.Add
                (DocumentTypeEnum.kPartDocumentObject,
                    _invApp.FileManager.GetTemplateFile
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
        public void Build(SporcketParams sporcketParams)
        {
            if (sporcketParams == null) throw new ArgumentNullException();

            //Вызываем методы для построения 
            PlanarSketch sporcketSketch1 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            Point2d point = _transientGeometry.CreatePoint2d(0, 0);
            DrawCircle(sporcketParams.RadiusA, sporcketSketch1, point);
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch1.Profiles.AddForSolid(), PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.LengthA, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
             _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

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
            // EdgeCollection edge = (
           
           // FilletDefinition = _partDef.Features.FilletFeatures.CreateFilletDefinition();


            PlanarSketch sporcketSketch2 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            DrawCircle(sporcketParams.RadiusB, sporcketSketch2, point);
            
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch2.Profiles.AddForSolid(),PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.LengthB+sporcketParams.LengthA, PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
              _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            TurnCamera();

            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch2.Profiles.AddForSolid(),PartFeatureOperationEnum.kJoinOperation);
            extrudeDef.SetDistanceExtent(sporcketParams.LengthC, PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);


            PlanarSketch sporcketSketch3 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.LengthA+sporcketParams.LengthB);
            DrawCircle(sporcketParams.RadiusC, sporcketSketch3, point);
             extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch3.Profiles.AddForSolid(),PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
              _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            //рисуем круг

            PlanarSketch sporcketSketch4 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d(0, (sporcketParams.RadiusA-sporcketParams.RadiusB)/2+sporcketParams.RadiusB);
            DrawCircle(sporcketParams.RadiusD, sporcketSketch4, point);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch4.Profiles.AddForSolid(),PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kPositiveExtentDirection);
            ExtrudeFeature scratcExtrudeFeature = _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
           
           
            //Коллекция фич для кругового массива
            ObjectCollection featureCollection = _invApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            //Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, sporcketParams.NumberD, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);

            PlanarSketch sporcketSketch5 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.LengthB + sporcketParams.LengthA);
            point = _transientGeometry.CreatePoint2d(0, 10);
            Point2d point2 = _transientGeometry.CreatePoint2d(sporcketParams.RadiusC+sporcketParams.LengthE, -10);
            sporcketSketch5.SketchLines.AddAsTwoPointRectangle(point, point2);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch5.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
             _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            PlanarSketch sporcketSketch6 = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZY, _centerPoint.X);
            point = _transientGeometry.CreatePoint2d((sporcketParams.LengthB/2)+sporcketParams.LengthA, 0);
            DrawCircle(sporcketParams.RadiusF, sporcketSketch6, point);
            extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch6.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            _partDef.Features.ExtrudeFeatures.Add(extrudeDef);

            ExtrudeArrayOfTeeth(sporcketParams);
           

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
        /// <param name="radius">Диаметр круга</param>
        /// <param name="sketch">Эскиз для рисования</param>
        /// <param name="centerPoint">Центр рисуемого круга</param>
        private void DrawCircle(double radius, PlanarSketch sketch, Object centerPoint)
        {
            sketch.SketchCircles.AddByCenterRadius(centerPoint, radius);
        }







        /// <summary>
        /// Метод для создания массива зубьев звездочки.
        /// </summary>
        /// <param name="sporcketParams">параметры детали</param>
       
        private void ExtrudeArrayOfTeeth(SporcketParams sporcketParams)
        {
            PlanarSketch sporcketSketch = MakeNewSketch(WorkPlanesEnum.WorkPlane_ZX, sporcketParams.LengthA);
            Point2d point = _transientGeometry.CreatePoint2d(sporcketParams.RadiusA, 0);

            DrawCircle(sporcketParams.DepthOfTooth, sporcketSketch, point);
            ExtrudeDefinition extrudeDef = _partDef.Features.ExtrudeFeatures.CreateExtrudeDefinition(sporcketSketch.Profiles.AddForSolid(), PartFeatureOperationEnum.kCutOperation);
            extrudeDef.SetThroughAllExtent(PartFeatureExtentDirectionEnum.kNegativeExtentDirection);
            ExtrudeFeature  scratcExtrudeFeature = _partDef.Features.ExtrudeFeatures.Add(extrudeDef);
            //Коллекция фич для кругового массива
            ObjectCollection featureCollection = _invApp.TransientObjects.CreateObjectCollection();
            featureCollection.Add(scratcExtrudeFeature);

            //Создание массива объектов
            _partDef.Features.CircularPatternFeatures.Add(featureCollection, _partDef.WorkAxes[2], true, ((sporcketParams.RadiusA*3.14) / sporcketParams.DepthOfTooth)*0.9, "360 deg", true, PatternComputeTypeEnum.kIdenticalCompute);

        }

   

        /// <summary>
        /// Метод для перемещения камеры
        /// </summary>
        private void TurnCamera()
        {
            //Получаем указатель на объект камеры
            Camera camera = _invApp.ActiveView.Camera;
            //Выбираем нужный ракурс
            camera.ViewOrientationType = ViewOrientationTypeEnum.kIsoTopRightViewOrientation;
            //Меняем ракурс
            camera.Apply();
        }

     
        #endregion
    }
}
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