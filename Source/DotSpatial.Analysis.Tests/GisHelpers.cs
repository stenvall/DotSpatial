using System;
using System.Drawing;
using DotSpatial.Data;
using DotSpatial.Projections;

namespace DotSpatial.Analysis.Tests
{
    /// <summary>
    /// This class contains GIS helper functions (move to domain layer)
    /// </summary>
    /// <summary>
    /// This class contains GIS helper functions (move to domain layer)
    /// </summary>
    public static class GISHelpers
    {
        /// <summary>
        /// Srid of RT90
        /// </summary>
        public const int RT90 = 3021;
        /// <summary>
        /// Srid of Sweref99
        /// </summary>
        public const int SWEREF99 = 3006;

        private const string RT90_PROJ4 = @"+proj=tmerc +lat_0=0 +lon_0=15.80827777777778 +k=1 +x_0=1500000 +y_0=0 +ellps=bessel +towgs84=419.384,99.3335,591.345,0.850389,1.81728,-7.86224,-0.99496 +units=m +no_defs ";
        private const string SWEREF99TM_PROJ4 = @"+proj=utm +zone=33 +ellps=GRS80 +towgs84=0,0,0,0,0,0,0 +units=m +no_defs ";

        public static ProjectionInfo GetRT90_Projection()
        {
            var proj = ProjectionInfo.FromProj4String(RT90_PROJ4);
            proj.Name = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV.Name;
            proj.AuthorityCode = 3021;
            proj.Authority = "EPSG";
            return proj;
        }

        public static ProjectionInfo GetSWEREFf99_Projection()
        {
            var proj = ProjectionInfo.FromProj4String(SWEREF99TM_PROJ4);
            proj.Name = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM.Name;
            proj.AuthorityCode = 3006;
            proj.Authority = "EPSG";
            proj.LatitudeOfOrigin = 0;
            return proj;
        }

        /// <summary>
        /// Latitude from RT90 coordinate.
        /// </summary>
        /// <param name="eastCoordinate">East coordinte (RT90), meters.</param>
        /// <param name="northCoordinate">North coordinte (RT90), meters.</param>
        /// <returns>Latitude in WGS84 degrees </returns>
        public static double LatitudeFromRT90Coordinate(double eastCoordinate, double northCoordinate)
        {
            //var projection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            var projection = GetRT90_Projection();
            return LatitudeFromCoordinate(eastCoordinate, northCoordinate, projection);
        }

        /// <summary>
        /// Latitude from SWEREFF99TM coordinate.
        /// </summary>
        /// <param name="eastCoordinate">East coordinte (SWEREF99 TM), meters.</param>
        /// <param name="northCoordinate">North coordinte (SWEREF99 TM), meters.</param>
        /// <returns>Latitude in WGS84 degrees </returns>
        public static double LatitudeFromSweref99TMCoordinate(double eastCoordinate, double northCoordinate)
        {
            var projection = GetSWEREFf99_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            return LatitudeFromCoordinate(eastCoordinate, northCoordinate, projection);
        }

        /// <summary>
        /// Latitude from coordinate using SWEREF as default coordinate system
        /// </summary>
        /// <param name="eastCoordinate">East coordinate. Unit depending on projection</param>
        /// <param name="northCoordinate">North coordinate. Unit depending on projection</param>
        /// <param name="projection">[OPTIONAL] A DotSpatial <see href = "http://www.nudoq.org/#!/Packages/DotSpatial.Projections/DotSpatial.Projections/ProjectionInfo">
        /// ProjectionInfo </see>object. If not supplied, SWEREFF99 will be used. </param>
        /// <returns>Latitude in WGS84 degrees </returns>
        public static double LatitudeFromCoordinate(double eastCoordinate, double northCoordinate, ProjectionInfo projection = null)
        {
            if (projection == null)
                projection = GetSWEREFf99_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            else
                projection.UpdateAuthorityCodeForKnowCoordinateSystem();
            var targetProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
            var xy = new double[] { eastCoordinate, northCoordinate };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, projection, targetProjection, 0, 1);

            return xy[1];
        }

        public static PointF LatLongFromRT90gonv(PointF coordinate)
        {
            //var fromProjection = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            var fromProjection = GetRT90_Projection();
            var toProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
            var xy = new double[] { coordinate.X, coordinate.Y };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        public static PointF LatLongFromSweref99TM(PointF coordinate)
        {
            var fromProjection = GetSWEREFf99_Projection(); // KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            var toProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
            var xy = new double[] { coordinate.X, coordinate.Y };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eastCoordinate"></param>
        /// <param name="northCoordinate"></param>
        /// <returns></returns>
        public static PointF Rt9025gonVFromSweref99TM(double eastCoordinate, double northCoordinate)
        {
            var fromProjection = GetSWEREFf99_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            var toProjection = GetRT90_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.RT3825gonV;
            var xy = new double[] { eastCoordinate, northCoordinate };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        public static PointF Rt9025gonVFromSweref99TM(PointF coordinate)
        {
            return Rt9025gonVFromSweref99TM(coordinate.X, coordinate.Y);
        }

        public static PointF Rt9025gonVFromFromLongLat(double longitude, double latitude)
        {
            var fromProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
            var toProjection = GetRT90_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            var xy = new double[] { longitude, latitude };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        public static PointF Sweref99TMFromRT9025gonV(double eastCoordinate, double northCoordinate)
        {
            var fromProjection = GetRT90_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
            var toProjection = GetSWEREFf99_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            var xy = new double[] { eastCoordinate, northCoordinate };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        public static PointF Sweref99TMFromRT9025gonV(PointF coordinate)
        {
            return Sweref99TMFromRT9025gonV(coordinate.X, coordinate.Y);
        }

        public static PointF Sweref99TMFromFromLongLat(double longitude, double latitude)
        {
            var fromProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
            var toProjection = GetSWEREFf99_Projection(); //KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
            var xy = new double[] { longitude, latitude };
            var z = new double[] { 10.0 };
            Reproject.ReprojectPoints(xy, z, fromProjection, toProjection, 0, 1);

            return new PointF((float)xy[0], (float)xy[1]);
        }

        public static bool IsSameProjection(this ProjectionInfo thisProjection, ProjectionInfo otherProjection)
        {
            if (thisProjection.Equals(otherProjection)) //||
                                                        // (thisProjection.GeographicInfo.Meridian == otherProjection.GeographicInfo.Meridian && thisProjection.GeographicInfo.Datum == otherProjection.GeographicInfo.Datum))
                return true;

            if (thisProjection.AuthorityCode > 0 && otherProjection.AuthorityCode > 0 &&
                thisProjection.Authority.Equals(otherProjection.Authority, StringComparison.CurrentCultureIgnoreCase) &&
                thisProjection.AuthorityCode == otherProjection.AuthorityCode)
                return true;

            return false;
        }

        public static bool IsSameProjection(this ProjectionInfo projection, IFeatureSet other)
        {
            return projection.IsSameProjection(other.Projection);
        }

        public static bool IsSameProjection(this IFeatureSet featureSet, ProjectionInfo other)
        {
            return featureSet.Projection.IsSameProjection(other);
        }

        public static bool IsSameProjection(this IFeatureSet featureSet, IFeatureSet other)
        {
            return featureSet.Projection.IsSameProjection(other.Projection);
        }

        /// <summary>
        /// Gets a projection info from spatial reference id
        /// </summary>
        /// <param name="srid"></param>
        /// <returns></returns>
        public static ProjectionInfo ProjectionInfoFromSrid(int srid)
        {
            var proj = ProjectionInfo.FromEpsgCode(srid);
            if (srid == RT90 || srid == 2400)
            {
                proj = GetSwedishProjection("EPSG:3021");
            }
            else if (srid == SWEREF99)
            {
                proj = GetSwedishProjection("EPSG:3006");
            }
            return proj;
        }

        // DotSptatial does not return known projection name for EPSG:3006 etc.
        public static ProjectionInfo GetSwedishProjection(string authorityCode)
        {
            if (authorityCode.ToUpper() == "EPSG:3006" || authorityCode.ToUpper() == "EPSG:4619")
            {
                //var projInfo = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM;
                //projInfo.AuthorityCode = 3006;
                //projInfo.Authority = "EPSG";
                return GetSWEREFf99_Projection(); //;
            }

            if (authorityCode.ToUpper() == "EPSG:3007")
            {
                var projInfo = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991200;
                projInfo.AuthorityCode = 3007;
                projInfo.Authority = "EPSG";
                return projInfo;
            }

            if (authorityCode.ToUpper() == "EPSG:3008")
            {
                var projInfo = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991330;
                projInfo.AuthorityCode = 3008;
                projInfo.Authority = "EPSG";
                return projInfo;
            }

            //http://spatialreference.org/ref/epsg/3009/
            if (authorityCode.ToUpper() == "EPSG:3009")
            {
                var projInfo = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991630;
                projInfo.AuthorityCode = 3009;
                projInfo.Authority = "EPSG";
                return projInfo;
            }

            // and so forth...

            // http://spatialreference.org/ref/epsg/2400/
            if (authorityCode.ToUpper() == "EPSG:3021" || authorityCode.ToUpper() == "EPSG:2400")
            {
                return GetRT90_Projection();
                //var projInfo = KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV;
                //projInfo.AuthorityCode = 3021;
                //projInfo.Authority = "EPSG";
                //return projInfo;
            }

            return null;
        }

        // DotSptatial does not return known projection name for EPSG:3006 etc. Also set
        // Calling this function should not be required in DotSpatial 2.0
        public static void UpdateAuthorityCodeForKnowCoordinateSystem(this ProjectionInfo projection)
        {
            // EPSG:4619 was used for SWEREF99 TM by mistake in previous version of ShapeImporter
            if ((projection.Authority == "EPSG" && projection.AuthorityCode == 4619) ||
                projection.Name.ToUpper() == KnownCoordinateSystems.Geographic.Europe.SWEREF99.Name)
            {
                projection.Name = KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM.Name;
            }

            if ((projection.Authority == "EPSG" && projection.AuthorityCode == 3006) ||
                projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF99TM.Name)
            {
                projection.CopyProperties(GetSWEREFf99_Projection());
                return;
            }

            if (projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991200.Name)
            {
                projection.AuthorityCode = 3007;
                projection.Authority = "EPSG";
                return;
            }

            if (projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991330.Name)
            {
                projection.AuthorityCode = 3008;
                projection.Authority = "EPSG";
                return;
            }

            //http://spatialreference.org/ref/epsg/3009/
            if (projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991630.Name)
            {
                projection.AuthorityCode = 3009;
                projection.Authority = "EPSG";
                return;
            }

            //http://spatialreference.org/ref/epsg/3009/
            if (projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.SWEREF991545.Name)
            {
                projection.AuthorityCode = 3013;
                projection.Authority = "EPSG";
                return;
            }

            // and so forth...

            // EPSG:2004 is obsolete and same as 3021
            if ((projection.Authority == "EPSG" && projection.AuthorityCode == 2400) ||
                (projection.Authority == "EPSG" && projection.AuthorityCode == 3021) ||
                projection.Name.ToUpper() == "RT90_25_GON_W" || projection.Name.ToUpper() == "RT90 25 GON W" ||
                projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV.Name)
            {
                var rt90 = GetRT90_Projection();
                projection.CopyProperties(rt90); //KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV);
            }

            //else if (projection.Name == KnownCoordinateSystems.Projected.NationalGridsSweden.RT9025gonV.Name)
            //{
            //    projection.AuthorityCode = 3021;
            //    projection.Authority = "EPSG";
            //    return;
            //}

        }

    }
}

