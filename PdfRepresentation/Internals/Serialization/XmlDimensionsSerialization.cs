using CoreLibrary.Model.Configuration.Hosting;

namespace PdfRepresentation.Internals.Serialization
{
    internal static class XmlDimensionsSerialization
    {
        public static bool ShouldSerializeDimensions
        {
            get
            {
                var argumentGetter = ArgumentGetterHost.Instance.ArgumentGetter;
                return argumentGetter?.BooleanFromArguments["ShouldSerializeDimensions", true] ?? false;
            }
            set
            {
                var argumentGetter = ArgumentGetterHost.Instance.ArgumentGetter;
                if (argumentGetter != null)
                {
                    argumentGetter.BooleanFromArguments["ShouldSerializeDimensions"] = value;
                }
            }
        }
    }
}