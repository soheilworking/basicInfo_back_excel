namespace ORG.BasicInfo.API.Extensions
{
    public class RemoveControllerSuffixTransformer : IOutboundParameterTransformer
    {
        public string TransformOutbound(object value)
        {
            if (value == null)
                return null;

            var name = value.ToString();  // مثلاً "ProvinceControllerCommands"
            var suffix = "Controller";
            var idx = name.IndexOf(suffix, StringComparison.Ordinal);
            if (idx >= 0)
                return name.Substring(0, idx);  // برمی‌گرداند "Province"

            return name;
        }
    }
}
