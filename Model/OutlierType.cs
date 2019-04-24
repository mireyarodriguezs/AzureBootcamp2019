namespace ServerlessDataPipeline.Model
{
    public enum OutlierType
    {
        RaUpperOutlier,
        RaLowerOutlier,
        RbUpperOutlier,
        RbLowerOutlier,
        RcUpperOutlier,
        RcLowerOutlier
    }

    public static class OutlierTypeExtensions
    {
        public static bool IsUpperOutlier(this OutlierType outlierType)
        {
            return outlierType == OutlierType.RaUpperOutlier ||
                   outlierType == OutlierType.RbUpperOutlier ||
                   outlierType == OutlierType.RcUpperOutlier;
        }

        public static bool IsLowerOutlier(this OutlierType outlierType)
        {
            return outlierType == OutlierType.RaLowerOutlier ||
                   outlierType == OutlierType.RbLowerOutlier ||
                   outlierType == OutlierType.RcLowerOutlier;
        }
    }
}
