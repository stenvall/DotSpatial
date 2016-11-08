using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotSpatial.Data;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;


namespace DotSpatial.Analysis.Tests
{
    public static class RasterCellPolygonFromCoordinate
    {
        public static Polygon CreateFromCell(this IRaster raster, int row, int col)
        {
            var c = raster.CellToProj(row, col);
            
            List<Coordinate> coords = new List<Coordinate>
            {
                new Coordinate(c.X - 0.5*raster.CellWidth, c.Y - 0.5*raster.CellHeight),
                new Coordinate(c.X - 0.5*raster.CellWidth, c.Y + 0.5*raster.CellHeight),
                new Coordinate(c.X + 0.5*raster.CellWidth, c.Y + 0.5*raster.CellHeight),
                new Coordinate(c.X + 0.5*raster.CellWidth, c.Y - 0.5*raster.CellHeight),
                new Coordinate(c.X - 0.5*raster.CellWidth, c.Y - 0.5*raster.CellHeight)
            };

            var outerRing = new LinearRing(coords.ToArray());

            var poly = new Polygon(outerRing);
            return poly;
        }
    } 
}
