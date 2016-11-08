using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
//using DotSpatial.Topology;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;


namespace DotSpatial.Analysis.Tests
{
    public class RasterClipper
    {
        private readonly IRaster _sourceRaster;
        private readonly double _cellWidth;
        private readonly double _cellHeight;
        private readonly Extent _sourceRasterExtent;
        private readonly int _nbRows;
        private readonly int _nbColumns;

        public RasterClipper(IRaster sourceRaster)
        {
            _sourceRaster = sourceRaster;
            _cellWidth = _sourceRaster.CellWidth;
            _cellHeight = _sourceRaster.CellHeight;
            _sourceRasterExtent = _sourceRaster.Extent;
            _nbRows = _sourceRaster.NumRows;
            _nbColumns = _sourceRaster.NumColumns;
        }



        public IDictionary<Tuple<int, int>, double> GetCellsIntersectingFeatureButCenterDoesNotIntersect(IFeature feature)
        {
            var firstRow = getFirstRowIndex(feature.Geometry.Envelope);
            var lastRow = getLastRowIndex(feature.Geometry.Envelope);
            var firstCol = getFirstColIndex(feature.Geometry.Envelope);
            var lastCol = getLastColIndex(feature.Geometry.Envelope);
            var valueDictionary = new Dictionary<Tuple<int, int>, double>();
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = firstCol; j < lastCol; j++)
                {
                    var rasterCellPolygon = _sourceRaster.CreateFromCell(i, j);
                    if (feature.Geometry.Intersects(rasterCellPolygon))
                    {

                        if (!feature.Geometry.Contains(rasterCellPolygon.Centroid))
                        {
                            valueDictionary.Add(new Tuple<int, int>(i, j), _sourceRaster.Value[i, j]);
                        }
                    }
                }
            }
            return valueDictionary;
        }


        public IDictionary<Tuple<int, int>, double> GetCellsWithValue(int value)
        {
            var firstRow = _sourceRaster.StartRow;
            var lastRow = _sourceRaster.EndRow;
            var firstCol = _sourceRaster.StartColumn;
            var lastCol = _sourceRaster.EndColumn;
            var valueDictionary = new Dictionary<Tuple<int, int>, double>();
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = firstCol; j < lastCol; j++)
                {
                    
                    if ((int)_sourceRaster.Value[i, j] == value)
                    {
                        valueDictionary.Add(new Tuple<int, int>(i, j), _sourceRaster.Value[i, j]);
                    }
                }
            }
            return valueDictionary;
        }




        public IDictionary<Tuple<int, int>, double> GetCellsIntersectingFeatureHaveNoValue(IFeature feature)
        {
            var firstRow = getFirstRowIndex(feature.Geometry.Envelope);
            var lastRow = getLastRowIndex(feature.Geometry.Envelope);
            var firstCol = getFirstColIndex(feature.Geometry.Envelope);
            var lastCol = getLastColIndex(feature.Geometry.Envelope);
            var valueDictionary = new Dictionary<Tuple<int, int>, double>();
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = firstCol; j < lastCol; j++)
                {
                    var rasterCellPolygon = _sourceRaster.CreateFromCell(i, j);
                    if (feature.Geometry.Intersects(rasterCellPolygon))
                    {
                        if (_sourceRaster.Value[i, j] < 0)
                        {
                            valueDictionary.Add(new Tuple<int, int>(i, j), _sourceRaster.Value[i, j]);
                        }
                    }
                }
            }
            return valueDictionary;
        }

        public IDictionary<Tuple<int, int>, double> GetCellsWithValueOutsideFeature(IFeature feature)
        {
            var firstRow = getFirstRowIndex(feature.Geometry.Envelope);
            var lastRow = getLastRowIndex(feature.Geometry.Envelope);
            var firstCol = getFirstColIndex(feature.Geometry.Envelope);
            var lastCol = getLastColIndex(feature.Geometry.Envelope);
            var valueDictionary = new Dictionary<Tuple<int, int>, double>();
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = firstCol; j < lastCol; j++)
                {
                    var rasterCellPolygon = _sourceRaster.CreateFromCell(i, j);
                    if (!feature.Geometry.Intersects(rasterCellPolygon))
                    {
                        if (_sourceRaster.Value[i, j] >= 0)
                        {
                            valueDictionary.Add(new Tuple<int, int>(i, j), _sourceRaster.Value[i, j]);
                        }
                    }
                }
            }

            return valueDictionary;

        }





        private double getRasterSum(IRaster raster, int firstRow, int lastRow, int firstCol, int lastCol)
        {
            var sumTot = 0.0;
            for (int i = firstRow; i < lastRow; i++)
            {
                for (int j = firstCol; j < lastCol; j++)
                {
                    var val = raster.Value[i, j];
                    if (val > raster.NoDataValue)
                        sumTot += raster.Value[i, j];
                }
            }
            return sumTot;
        }

        // raster cell (0,0) is at raster extent min x, max y!
        private int getFirstRowIndex(IGeometry polygonExtent)
        {
            int i = (int)((_sourceRasterExtent.MaxY - polygonExtent.EnvelopeInternal.MaxY) / _cellHeight);
            return i;
        }

        private int getLastRowIndex(IGeometry polygonExtent)
        {
            int i = (int)Math.Ceiling((_sourceRasterExtent.MaxY - polygonExtent.EnvelopeInternal.MinY) / _cellHeight);
            if (i > _nbRows - 1)
                i = _nbRows - 1;
            return i;
        }

        private int getFirstColIndex(IGeometry polygonExtent)
        {
            int i = (int)((polygonExtent.EnvelopeInternal.MinX - _sourceRasterExtent.MinX) / _cellWidth);
            return i;
        }

        private int getLastColIndex(IGeometry polygonExtent)
        {
            int i = (int)Math.Ceiling((polygonExtent.EnvelopeInternal.MaxX - _sourceRasterExtent.MinX) / _cellWidth);
            if (i > _nbColumns - 1)
                i = _nbColumns - 1;
            return i;
        }
    }


}
