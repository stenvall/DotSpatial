﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Projections;
using NUnit.Framework;
using GeoAPI.Geometries;

namespace DotSpatial.Analysis.Tests
{
    /// <summary>
    ///This is a test class for VectorToRasterTest and is intended
    ///to contain all VectorToRasterTest Unit Tests
    ///</summary>
    [TestFixture]
    public class VectorToRasterTests
    {


        [Test]
        public void SamePolygonDifferentCellsTest()
        {
            #region shared settings
            var cellSize = 25.00D;
            var columnName = "DataValue";
            var coordinateList_polygon_1 = GetCoordinateList_1();
            var coordinateList_polygon_2 = GetCoordinateList_2();
            var value1 = 1;
            #endregion

            #region raster 1
            var raster1FileName = @"c:\temp\raster1.bgd";
            var featureSet1FileName = @"c:\temp\raster1.shp";
            var featureSet1 = new FeatureSet(FeatureType.Polygon);
            featureSet1.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
            featureSet1.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            IFeature feature = new Feature(FeatureType.Polygon, coordinateList_polygon_1);

            featureSet1.Features.Add(feature);

            feature.DataRow[columnName] = value1;

            var raster1 = VectorToRaster.ToRaster(featureSet1, cellSize, columnName, raster1FileName);
            if (!raster1.IsInRam)
                raster1.GetStatistics();

            raster1.Projection = featureSet1.Projection;
            raster1.Save();

            featureSet1.SaveAs(featureSet1FileName, true);


            var raster1Clipper = new RasterClipper(raster1);

            var raster1Cells = raster1Clipper.GetCellsWithValue(value1);

            #endregion

            #region raster 2
            var raster2FileName = @"c:\temp\raster2.bgd";
            var featureSet2FileName = @"c:\temp\raster2.shp";
            var featureSet2 = new FeatureSet(FeatureType.Polygon);
            featureSet2.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
            featureSet2.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            IFeature f1 = new Feature(FeatureType.Polygon, coordinateList_polygon_1);
            IFeature f2 = new Feature(FeatureType.Polygon, coordinateList_polygon_2);

            featureSet2.Features.Add(f1);
            featureSet2.Features.Add(f2);

            f1.DataRow[columnName] = value1;
            f2.DataRow[columnName] = 2;

            var raster2 = VectorToRaster.ToRaster(featureSet2, cellSize, columnName, raster2FileName);
            if (!raster2.IsInRam)
                raster2.GetStatistics();

            raster2.Projection = featureSet2.Projection;
            raster2.Save();

            featureSet2.SaveAs(featureSet2FileName, true);
               

            var raster2Clipper = new RasterClipper(raster2);

            var raster2Cells = raster2Clipper.GetCellsWithValue(value1);
            #endregion

            Assert.AreEqual(raster1Cells.Count, raster2Cells.Count);
                
        }



        [Test]
        public void CreateVectorToRasterTest()
        {
            try
            {
                var cellSize = 25.00D;
                var columnName = "DataValue";
                var resultRasterFileName = @"c:\temp\CreateVectorToRasterTest.bgd";
                var featureSetFileName = @"c:\temp\CreateVectorToRasterTest.shp";

                var coordinateList_polygon_1 = GetCoordinateList_1();
                var coordinateList_polygon_2 = GetCoordinateList_2();
                
                var fs = new FeatureSet(FeatureType.Polygon);
                fs.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
                fs.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
                //fs.Projection = GISHelpers.GetRT90_Projection();
                IFeature f1 = new Feature(FeatureType.Polygon, coordinateList_polygon_1);
                IFeature f2 = new Feature(FeatureType.Polygon, coordinateList_polygon_2);

                fs.Features.Add(f1);
                fs.Features.Add(f2);

                f1.DataRow[columnName] = 1;
                f2.DataRow[columnName] = 2;

                var raster = VectorToRaster.ToRaster(fs, cellSize, columnName, resultRasterFileName);
                if (!raster.IsInRam)
                    raster.GetStatistics();

                raster.Projection = fs.Projection;
                raster.Save();

                fs.SaveAs(featureSetFileName, true);

                Assert.AreEqual(1, 1);

            }
            catch (Exception exception)
            {

                Assert.AreEqual(1, 2);
            }

        }

        [Test]
        public void CellCenterInsidePolygonTest()
        {
            try
            {
                var cellSize = 25.00D;
                var columnName = "DataValue";
                var resultRasterFileName = @"c:\temp\CellCenterInsidePolygonTest.bgd";
                var featureSetFileName = @"c:\temp\CellCenterInsidePolygonTest.shp";

                var coordinateList = GetCoordinateList_1();
                

                var fs = new FeatureSet(FeatureType.Polygon);
                fs.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
                fs.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
                //fs.Projection = GISHelpers.GetRT90_Projection();
                IFeature feature = new Feature(FeatureType.Polygon, coordinateList);

                fs.Features.Add(feature);

                feature.DataRow[columnName] = 1;

                var raster = VectorToRaster.ToRaster(fs, cellSize, columnName, resultRasterFileName);
                if (!raster.IsInRam)
                    raster.GetStatistics();

                raster.Projection = fs.Projection;
                raster.Save();

                fs.SaveAs(featureSetFileName, true);


                var rasterClipper = new RasterClipper(raster);

                var dictionary = rasterClipper.GetCellsIntersectingFeatureButCenterDoesNotIntersect(feature);

                Assert.AreEqual(0,dictionary.Count);

            }
            catch (Exception exception)
            {

                Assert.AreEqual(1, 2);
            }

        }

        [Test]
        public void NoValueCellInsidePolygonTest()
        {
            var cellSize = 25.00D;
            var columnName = "DataValue";
            var resultRasterFileName = @"c:\temp\NoValueCellInsidePolygonTest.bgd";
            var featureSetFileName = @"c:\temp\NoValueCellInsidePolygonTest.shp";

            var coordinateList = GetCoordinateList_1();


            var fs = new FeatureSet(FeatureType.Polygon);
            fs.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
            fs.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            //fs.Projection = GISHelpers.GetRT90_Projection();
            IFeature feature = new Feature(FeatureType.Polygon, coordinateList);

            fs.Features.Add(feature);

            feature.DataRow[columnName] = 1;

            var raster = VectorToRaster.ToRaster(fs, cellSize, columnName, resultRasterFileName);
            if (!raster.IsInRam)
                raster.GetStatistics();

            raster.Projection = fs.Projection;
            raster.Save();

            fs.SaveAs(featureSetFileName, true);


            var rasterClipper = new RasterClipper(raster);

            var dictionary = rasterClipper.GetCellsIntersectingFeatureHaveNoValue(feature);

            Assert.AreEqual(0,dictionary.Count);

        }


        [Test]
        public void ValueCellOutsidePolygonTest()
        {
            
            var cellSize = 25.00D;
            var columnName = "DataValue";
            var resultRasterFileName = @"c:\temp\ValueCellOutsidePolygonTest.bgd";
            var featureSetFileName = @"c:\temp\ValueCellOutsidePolygonTest.shp";

            var coordinateList = GetCoordinateList_2();


            var fs = new FeatureSet(FeatureType.Polygon);
            fs.DataTable.Columns.Add(new DataColumn(columnName, typeof(int)));
            fs.Projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            //fs.Projection = GISHelpers.GetRT90_Projection();

            IFeature feature = new Feature(FeatureType.Polygon, coordinateList);

            fs.Features.Add(feature);

            feature.DataRow[columnName] = 1;

            var raster = VectorToRaster.ToRaster(fs, cellSize, columnName, resultRasterFileName);
            if (!raster.IsInRam)
                raster.GetStatistics();

            raster.Projection = fs.Projection;
            raster.Save();

            fs.SaveAs(featureSetFileName, true);


            var rasterClipper = new RasterClipper(raster);

            var dictionary = rasterClipper.GetCellsWithValueOutsideFeature(feature);

            Assert.AreEqual(0, dictionary.Count);

        }

        


        private static IEnumerable<Coordinate> GetCoordinateList_1()
        {
            IEnumerable<Coordinate> coordinateList = new List<Coordinate>()
            {
                new Coordinate(1683206.3750000000000000, 7134169.0000000000000000),
                new Coordinate(1683198.0000000000000000, 7134157.5000000000000000),
                new Coordinate(1683193.8750000000000000, 7134149.5000000000000000),
                new Coordinate(1683188.8750000000000000, 7134133.5000000000000000),
                new Coordinate(1683189.6250000000000000, 7134125.5000000000000000),
                new Coordinate(1683193.6250000000000000, 7134117.5000000000000000),
                new Coordinate(1683206.5000000000000000, 7134101.0000000000000000),
                new Coordinate(1683200.6250000000000000, 7134077.0000000000000000),
                new Coordinate(1683200.5000000000000000, 7134068.5000000000000000),
                new Coordinate(1683223.1250000000000000, 7134023.5000000000000000),
                new Coordinate(1683229.5000000000000000, 7134015.0000000000000000),
                new Coordinate(1683245.2500000000000000, 7133982.5000000000000000),
                new Coordinate(1683255.1250000000000000, 7133958.0000000000000000),
                new Coordinate(1683263.0000000000000000, 7133950.0000000000000000),
                new Coordinate(1683279.0000000000000000, 7133938.5000000000000000),
                new Coordinate(1683311.1250000000000000, 7133923.0000000000000000),
                new Coordinate(1683351.2500000000000000, 7133910.5000000000000000),
                new Coordinate(1683367.3750000000000000, 7133903.0000000000000000),
                new Coordinate(1683399.3750000000000000, 7133881.5000000000000000),
                new Coordinate(1683424.5000000000000000, 7133840.5000000000000000),
                new Coordinate(1683438.0000000000000000, 7133824.0000000000000000),
                new Coordinate(1683446.0000000000000000, 7133818.5000000000000000),
                new Coordinate(1683454.0000000000000000, 7133815.0000000000000000),
                new Coordinate(1683470.1250000000000000, 7133811.5000000000000000),
                new Coordinate(1683486.2500000000000000, 7133807.0000000000000000),
                new Coordinate(1683518.3750000000000000, 7133795.5000000000000000),
                new Coordinate(1683529.5000000000000000, 7133795.0000000000000000),
                new Coordinate(1683524.1250000000000000, 7133787.0000000000000000),
                new Coordinate(1683521.2500000000000000, 7133778.0000000000000000),
                new Coordinate(1683519.1250000000000000, 7133760.0000000000000000),
                new Coordinate(1683501.8750000000000000, 7133759.0000000000000000),
                new Coordinate(1683461.6250000000000000, 7133766.5000000000000000),
                new Coordinate(1683445.3750000000000000, 7133766.0000000000000000),
                new Coordinate(1683410.8750000000000000, 7133753.0000000000000000),
                new Coordinate(1683391.5000000000000000, 7133742.5000000000000000),
                new Coordinate(1683382.2500000000000000, 7133739.0000000000000000),
                new Coordinate(1683364.6250000000000000, 7133730.5000000000000000),
                new Coordinate(1683358.0000000000000000, 7133724.5000000000000000),
                new Coordinate(1683336.0000000000000000, 7133734.0000000000000000),
                new Coordinate(1683279.6250000000000000, 7133746.5000000000000000),
                new Coordinate(1683263.5000000000000000, 7133748.5000000000000000),
                new Coordinate(1683247.3750000000000000, 7133744.0000000000000000),
                new Coordinate(1683231.0000000000000000, 7133732.0000000000000000),
                new Coordinate(1683225.7500000000000000, 7133724.0000000000000000),
                new Coordinate(1683226.5000000000000000, 7133707.5000000000000000),
                new Coordinate(1683234.2500000000000000, 7133683.5000000000000000),
                new Coordinate(1683236.2500000000000000, 7133659.0000000000000000),
                new Coordinate(1683238.2500000000000000, 7133651.0000000000000000),
                new Coordinate(1683238.5000000000000000, 7133643.0000000000000000),
                new Coordinate(1683230.3750000000000000, 7133637.0000000000000000),
                new Coordinate(1683222.3750000000000000, 7133643.5000000000000000),
                new Coordinate(1683189.3750000000000000, 7133700.5000000000000000),
                new Coordinate(1683179.3750000000000000, 7133717.0000000000000000),
                new Coordinate(1683171.6250000000000000, 7133725.0000000000000000),
                new Coordinate(1683151.6250000000000000, 7133730.0000000000000000),
                new Coordinate(1683156.6250000000000000, 7133738.0000000000000000),
                new Coordinate(1683154.1250000000000000, 7133746.0000000000000000),
                new Coordinate(1683148.0000000000000000, 7133754.5000000000000000),
                new Coordinate(1683124.5000000000000000, 7133777.5000000000000000),
                new Coordinate(1683110.2500000000000000, 7133794.0000000000000000),
                new Coordinate(1683106.1250000000000000, 7133802.5000000000000000),
                new Coordinate(1683094.5000000000000000, 7133835.0000000000000000),
                new Coordinate(1683085.0000000000000000, 7133851.0000000000000000),
                new Coordinate(1683077.1250000000000000, 7133858.5000000000000000),
                new Coordinate(1683070.7500000000000000, 7133867.0000000000000000),
                new Coordinate(1683037.3750000000000000, 7133934.5000000000000000),
                new Coordinate(1683024.8750000000000000, 7133972.0000000000000000),
                new Coordinate(1683007.1250000000000000, 7134007.0000000000000000),
                new Coordinate(1682994.3750000000000000, 7134045.0000000000000000),
                new Coordinate(1682977.0000000000000000, 7134078.5000000000000000),
                new Coordinate(1682969.0000000000000000, 7134083.5000000000000000),
                new Coordinate(1682952.8750000000000000, 7134082.5000000000000000),
                new Coordinate(1682944.6250000000000000, 7134078.5000000000000000),
                new Coordinate(1682928.3750000000000000, 7134066.0000000000000000),
                new Coordinate(1682920.2500000000000000, 7134062.0000000000000000),
                new Coordinate(1682912.1250000000000000, 7134063.5000000000000000),
                new Coordinate(1682904.1250000000000000, 7134070.0000000000000000),
                new Coordinate(1682882.3750000000000000, 7134095.0000000000000000),
                new Coordinate(1682866.2500000000000000, 7134121.5000000000000000),
                new Coordinate(1682858.7500000000000000, 7134140.5000000000000000),
                new Coordinate(1682858.0000000000000000, 7134148.5000000000000000),
                new Coordinate(1682862.7500000000000000, 7134156.5000000000000000),
                new Coordinate(1682915.6250000000000000, 7134212.0000000000000000),
                new Coordinate(1682929.1250000000000000, 7134230.5000000000000000),
                new Coordinate(1682937.2500000000000000, 7134238.0000000000000000),
                new Coordinate(1682960.5000000000000000, 7134248.5000000000000000),
                new Coordinate(1682968.6250000000000000, 7134253.0000000000000000),
                new Coordinate(1682976.7500000000000000, 7134253.0000000000000000),
                new Coordinate(1682984.7500000000000000, 7134249.0000000000000000),
                new Coordinate(1683000.7500000000000000, 7134238.5000000000000000),
                new Coordinate(1683016.7500000000000000, 7134230.5000000000000000),
                new Coordinate(1683024.8750000000000000, 7134229.0000000000000000),
                new Coordinate(1683033.0000000000000000, 7134233.5000000000000000),
                new Coordinate(1683041.2500000000000000, 7134241.5000000000000000),
                new Coordinate(1683049.3750000000000000, 7134246.0000000000000000),
                new Coordinate(1683065.6250000000000000, 7134253.0000000000000000),
                new Coordinate(1683106.1250000000000000, 7134257.0000000000000000),
                new Coordinate(1683138.5000000000000000, 7134257.0000000000000000),
                new Coordinate(1683163.2500000000000000, 7134252.0000000000000000),
                new Coordinate(1683171.3750000000000000, 7134255.5000000000000000),
                new Coordinate(1683173.5000000000000000, 7134222.5000000000000000),
                new Coordinate(1683178.5000000000000000, 7134206.5000000000000000),
                new Coordinate(1683184.3750000000000000, 7134190.0000000000000000),
                new Coordinate(1683190.3750000000000000, 7134182.0000000000000000),
                new Coordinate(1683206.3750000000000000, 7134169.0000000000000000)
            };
            return coordinateList;
        }


        private static IEnumerable<Coordinate> GetCoordinateList_2()
        {
            IEnumerable<Coordinate> coordinateList = new List<Coordinate>()
            {
                new Coordinate(1683589.2500000000000000, 7133882.0000000000000000),
                new Coordinate(1683565.6250000000000000, 7133859.0000000000000000),
                new Coordinate(1683561.3750000000000000, 7133851.0000000000000000),
                new Coordinate(1683553.1250000000000000, 7133827.0000000000000000),
                new Coordinate(1683543.8750000000000000, 7133811.0000000000000000),
                new Coordinate(1683529.5000000000000000, 7133795.0000000000000000),
                new Coordinate(1683518.3750000000000000, 7133795.5000000000000000),
                new Coordinate(1683486.2500000000000000, 7133807.0000000000000000),
                new Coordinate(1683470.1250000000000000, 7133811.5000000000000000),
                new Coordinate(1683454.0000000000000000, 7133815.0000000000000000),
                new Coordinate(1683446.0000000000000000, 7133818.5000000000000000),
                new Coordinate(1683438.0000000000000000, 7133824.0000000000000000),
                new Coordinate(1683424.5000000000000000, 7133840.5000000000000000),
                new Coordinate(1683399.3750000000000000, 7133881.5000000000000000),
                new Coordinate(1683367.3750000000000000, 7133903.0000000000000000),
                new Coordinate(1683351.2500000000000000, 7133910.5000000000000000),
                new Coordinate(1683311.1250000000000000, 7133923.0000000000000000),
                new Coordinate(1683279.0000000000000000, 7133938.5000000000000000),
                new Coordinate(1683263.0000000000000000, 7133950.0000000000000000),
                new Coordinate(1683255.1250000000000000, 7133958.0000000000000000),
                new Coordinate(1683245.2500000000000000, 7133982.5000000000000000),
                new Coordinate(1683229.5000000000000000, 7134015.0000000000000000),
                new Coordinate(1683223.1250000000000000, 7134023.5000000000000000),
                new Coordinate(1683200.5000000000000000, 7134068.5000000000000000),
                new Coordinate(1683200.6250000000000000, 7134077.0000000000000000),
                new Coordinate(1683206.5000000000000000, 7134101.0000000000000000),
                new Coordinate(1683193.6250000000000000, 7134117.5000000000000000),
                new Coordinate(1683189.6250000000000000, 7134125.5000000000000000),
                new Coordinate(1683188.8750000000000000, 7134133.5000000000000000),
                new Coordinate(1683193.8750000000000000, 7134149.5000000000000000),
                new Coordinate(1683198.0000000000000000, 7134157.5000000000000000),
                new Coordinate(1683206.3750000000000000, 7134169.0000000000000000),
                new Coordinate(1683230.3750000000000000, 7134152.0000000000000000),
                new Coordinate(1683246.3750000000000000, 7134144.5000000000000000),
                new Coordinate(1683270.5000000000000000, 7134138.5000000000000000),
                new Coordinate(1683302.7500000000000000, 7134134.0000000000000000),
                new Coordinate(1683335.5000000000000000, 7134135.0000000000000000),
                new Coordinate(1683391.3750000000000000, 7134134.5000000000000000),
                new Coordinate(1683415.6250000000000000, 7134131.5000000000000000),
                new Coordinate(1683431.6250000000000000, 7134125.5000000000000000),
                new Coordinate(1683439.6250000000000000, 7134120.5000000000000000),
                new Coordinate(1683446.7500000000000000, 7134112.0000000000000000),
                new Coordinate(1683450.0000000000000000, 7134104.0000000000000000),
                new Coordinate(1683450.7500000000000000, 7134096.0000000000000000),
                new Coordinate(1683450.0000000000000000, 7134088.0000000000000000),
                new Coordinate(1683447.6250000000000000, 7134080.0000000000000000),
                new Coordinate(1683447.1250000000000000, 7134071.5000000000000000),
                new Coordinate(1683454.2500000000000000, 7134063.5000000000000000),
                new Coordinate(1683462.2500000000000000, 7134059.5000000000000000),
                new Coordinate(1683502.3750000000000000, 7134035.5000000000000000),
                new Coordinate(1683534.2500000000000000, 7134012.5000000000000000),
                new Coordinate(1683547.0000000000000000, 7133996.0000000000000000),
                new Coordinate(1683554.8750000000000000, 7133980.0000000000000000),
                new Coordinate(1683561.1250000000000000, 7133953.0000000000000000),
                new Coordinate(1683561.6250000000000000, 7133925.5000000000000000),
                new Coordinate(1683563.8750000000000000, 7133906.5000000000000000),
                new Coordinate(1683570.8750000000000000, 7133890.0000000000000000),
                new Coordinate(1683578.8750000000000000, 7133884.0000000000000000),
                new Coordinate(1683589.2500000000000000, 7133882.0000000000000000)

            };
            return coordinateList;
        }

        //var outerRing = new LinearRing(new Coordinate[] {new Coordinate(1683206.3750000000000000, 7134169.0000000000000000), new Coordinate(1683198.0000000000000000, 7134157.5000000000000000), new Coordinate(1683193.8750000000000000, 7134149.5000000000000000), new Coordinate(1683188.8750000000000000, 7134133.5000000000000000), new Coordinate(1683189.6250000000000000, 7134125.5000000000000000), new Coordinate(1683193.6250000000000000, 7134117.5000000000000000), new Coordinate(1683206.5000000000000000, 7134101.0000000000000000), new Coordinate(1683200.6250000000000000, 7134077.0000000000000000), new Coordinate(1683200.5000000000000000, 7134068.5000000000000000), new Coordinate(1683223.1250000000000000, 7134023.5000000000000000), new Coordinate(1683229.5000000000000000, 7134015.0000000000000000), new Coordinate(1683245.2500000000000000, 7133982.5000000000000000), new Coordinate(1683255.1250000000000000, 7133958.0000000000000000), new Coordinate(1683263.0000000000000000, 7133950.0000000000000000), new Coordinate(1683279.0000000000000000, 7133938.5000000000000000), new Coordinate(1683311.1250000000000000, 7133923.0000000000000000), new Coordinate(1683351.2500000000000000, 7133910.5000000000000000), new Coordinate(1683367.3750000000000000, 7133903.0000000000000000), new Coordinate(1683399.3750000000000000, 7133881.5000000000000000), new Coordinate(1683424.5000000000000000, 7133840.5000000000000000), new Coordinate(1683438.0000000000000000, 7133824.0000000000000000), new Coordinate(1683446.0000000000000000, 7133818.5000000000000000), new Coordinate(1683454.0000000000000000, 7133815.0000000000000000), new Coordinate(1683470.1250000000000000, 7133811.5000000000000000), new Coordinate(1683486.2500000000000000, 7133807.0000000000000000), new Coordinate(1683518.3750000000000000, 7133795.5000000000000000), new Coordinate(1683529.5000000000000000, 7133795.0000000000000000), new Coordinate(1683524.1250000000000000, 7133787.0000000000000000), new Coordinate(1683521.2500000000000000, 7133778.0000000000000000), new Coordinate(1683519.1250000000000000, 7133760.0000000000000000), new Coordinate(1683501.8750000000000000, 7133759.0000000000000000), new Coordinate(1683461.6250000000000000, 7133766.5000000000000000), new Coordinate(1683445.3750000000000000, 7133766.0000000000000000), new Coordinate(1683410.8750000000000000, 7133753.0000000000000000), new Coordinate(1683391.5000000000000000, 7133742.5000000000000000), new Coordinate(1683382.2500000000000000, 7133739.0000000000000000), new Coordinate(1683364.6250000000000000, 7133730.5000000000000000), new Coordinate(1683358.0000000000000000, 7133724.5000000000000000), new Coordinate(1683336.0000000000000000, 7133734.0000000000000000), new Coordinate(1683279.6250000000000000, 7133746.5000000000000000), new Coordinate(1683263.5000000000000000, 7133748.5000000000000000), new Coordinate(1683247.3750000000000000, 7133744.0000000000000000), new Coordinate(1683231.0000000000000000, 7133732.0000000000000000), new Coordinate(1683225.7500000000000000, 7133724.0000000000000000), new Coordinate(1683226.5000000000000000, 7133707.5000000000000000), new Coordinate(1683234.2500000000000000, 7133683.5000000000000000), new Coordinate(1683236.2500000000000000, 7133659.0000000000000000), new Coordinate(1683238.2500000000000000, 7133651.0000000000000000), new Coordinate(1683238.5000000000000000, 7133643.0000000000000000), new Coordinate(1683230.3750000000000000, 7133637.0000000000000000), new Coordinate(1683222.3750000000000000, 7133643.5000000000000000), new Coordinate(1683189.3750000000000000, 7133700.5000000000000000), new Coordinate(1683179.3750000000000000, 7133717.0000000000000000), new Coordinate(1683171.6250000000000000, 7133725.0000000000000000), new Coordinate(1683151.6250000000000000, 7133730.0000000000000000), new Coordinate(1683156.6250000000000000, 7133738.0000000000000000), new Coordinate(1683154.1250000000000000, 7133746.0000000000000000), new Coordinate(1683148.0000000000000000, 7133754.5000000000000000), new Coordinate(1683124.5000000000000000, 7133777.5000000000000000), new Coordinate(1683110.2500000000000000, 7133794.0000000000000000), new Coordinate(1683106.1250000000000000, 7133802.5000000000000000), new Coordinate(1683094.5000000000000000, 7133835.0000000000000000), new Coordinate(1683085.0000000000000000, 7133851.0000000000000000), new Coordinate(1683077.1250000000000000, 7133858.5000000000000000), new Coordinate(1683070.7500000000000000, 7133867.0000000000000000), new Coordinate(1683037.3750000000000000, 7133934.5000000000000000), new Coordinate(1683024.8750000000000000, 7133972.0000000000000000), new Coordinate(1683007.1250000000000000, 7134007.0000000000000000), new Coordinate(1682994.3750000000000000, 7134045.0000000000000000), new Coordinate(1682977.0000000000000000, 7134078.5000000000000000), new Coordinate(1682969.0000000000000000, 7134083.5000000000000000), new Coordinate(1682952.8750000000000000, 7134082.5000000000000000), new Coordinate(1682944.6250000000000000, 7134078.5000000000000000), new Coordinate(1682928.3750000000000000, 7134066.0000000000000000), new Coordinate(1682920.2500000000000000, 7134062.0000000000000000), new Coordinate(1682912.1250000000000000, 7134063.5000000000000000), new Coordinate(1682904.1250000000000000, 7134070.0000000000000000), new Coordinate(1682882.3750000000000000, 7134095.0000000000000000), new Coordinate(1682866.2500000000000000, 7134121.5000000000000000), new Coordinate(1682858.7500000000000000, 7134140.5000000000000000), new Coordinate(1682858.0000000000000000, 7134148.5000000000000000), new Coordinate(1682862.7500000000000000, 7134156.5000000000000000), new Coordinate(1682915.6250000000000000, 7134212.0000000000000000), new Coordinate(1682929.1250000000000000, 7134230.5000000000000000), new Coordinate(1682937.2500000000000000, 7134238.0000000000000000), new Coordinate(1682960.5000000000000000, 7134248.5000000000000000), new Coordinate(1682968.6250000000000000, 7134253.0000000000000000), new Coordinate(1682976.7500000000000000, 7134253.0000000000000000), new Coordinate(1682984.7500000000000000, 7134249.0000000000000000), new Coordinate(1683000.7500000000000000, 7134238.5000000000000000), new Coordinate(1683016.7500000000000000, 7134230.5000000000000000), new Coordinate(1683024.8750000000000000, 7134229.0000000000000000), new Coordinate(1683033.0000000000000000, 7134233.5000000000000000), new Coordinate(1683041.2500000000000000, 7134241.5000000000000000), new Coordinate(1683049.3750000000000000, 7134246.0000000000000000), new Coordinate(1683065.6250000000000000, 7134253.0000000000000000), new Coordinate(1683106.1250000000000000, 7134257.0000000000000000), new Coordinate(1683138.5000000000000000, 7134257.0000000000000000), new Coordinate(1683163.2500000000000000, 7134252.0000000000000000), new Coordinate(1683171.3750000000000000, 7134255.5000000000000000), new Coordinate(1683173.5000000000000000, 7134222.5000000000000000), new Coordinate(1683178.5000000000000000, 7134206.5000000000000000), new Coordinate(1683184.3750000000000000, 7134190.0000000000000000), new Coordinate(1683190.3750000000000000, 7134182.0000000000000000), new Coordinate(1683206.3750000000000000, 7134169.0000000000000000)});





        [Test]
        public void CoordinatesTest()
        {
            var wkt = "POLYGON((1683589.2500000000000000 7133882.0000000000000000, 1683565.6250000000000000 7133859.0000000000000000, 1683561.3750000000000000 7133851.0000000000000000, 1683553.1250000000000000 7133827.0000000000000000, 1683543.8750000000000000 7133811.0000000000000000, 1683529.5000000000000000 7133795.0000000000000000, 1683518.3750000000000000 7133795.5000000000000000, 1683486.2500000000000000 7133807.0000000000000000, 1683470.1250000000000000 7133811.5000000000000000, 1683454.0000000000000000 7133815.0000000000000000, 1683446.0000000000000000 7133818.5000000000000000, 1683438.0000000000000000 7133824.0000000000000000, 1683424.5000000000000000 7133840.5000000000000000, 1683399.3750000000000000 7133881.5000000000000000, 1683367.3750000000000000 7133903.0000000000000000, 1683351.2500000000000000 7133910.5000000000000000, 1683311.1250000000000000 7133923.0000000000000000, 1683279.0000000000000000 7133938.5000000000000000, 1683263.0000000000000000 7133950.0000000000000000, 1683255.1250000000000000 7133958.0000000000000000, 1683245.2500000000000000 7133982.5000000000000000, 1683229.5000000000000000 7134015.0000000000000000, 1683223.1250000000000000 7134023.5000000000000000, 1683200.5000000000000000 7134068.5000000000000000, 1683200.6250000000000000 7134077.0000000000000000, 1683206.5000000000000000 7134101.0000000000000000, 1683193.6250000000000000 7134117.5000000000000000, 1683189.6250000000000000 7134125.5000000000000000, 1683188.8750000000000000 7134133.5000000000000000, 1683193.8750000000000000 7134149.5000000000000000, 1683198.0000000000000000 7134157.5000000000000000, 1683206.3750000000000000 7134169.0000000000000000, 1683230.3750000000000000 7134152.0000000000000000, 1683246.3750000000000000 7134144.5000000000000000, 1683270.5000000000000000 7134138.5000000000000000, 1683302.7500000000000000 7134134.0000000000000000, 1683335.5000000000000000 7134135.0000000000000000, 1683391.3750000000000000 7134134.5000000000000000, 1683415.6250000000000000 7134131.5000000000000000, 1683431.6250000000000000 7134125.5000000000000000, 1683439.6250000000000000 7134120.5000000000000000, 1683446.7500000000000000 7134112.0000000000000000, 1683450.0000000000000000 7134104.0000000000000000, 1683450.7500000000000000 7134096.0000000000000000, 1683450.0000000000000000 7134088.0000000000000000, 1683447.6250000000000000 7134080.0000000000000000, 1683447.1250000000000000 7134071.5000000000000000, 1683454.2500000000000000 7134063.5000000000000000, 1683462.2500000000000000 7134059.5000000000000000, 1683502.3750000000000000 7134035.5000000000000000, 1683534.2500000000000000 7134012.5000000000000000, 1683547.0000000000000000 7133996.0000000000000000, 1683554.8750000000000000 7133980.0000000000000000, 1683561.1250000000000000 7133953.0000000000000000, 1683561.6250000000000000 7133925.5000000000000000, 1683563.8750000000000000 7133906.5000000000000000, 1683570.8750000000000000 7133890.0000000000000000, 1683578.8750000000000000 7133884.0000000000000000, 1683589.2500000000000000 7133882.0000000000000000))";
            var coordinateList = WKTReader.CoordinatesFromWKT(wkt);

            var refCoordinateList = GetCoordinateList_2();

            Assert.AreEqual(refCoordinateList.Count(), coordinateList.Count());
            Assert.AreEqual(refCoordinateList.FirstOrDefault().X, coordinateList.FirstOrDefault().X);
            Assert.AreEqual(refCoordinateList.FirstOrDefault().Y, coordinateList.FirstOrDefault().Y);


        }



    }
}