using System;
using CoreLibrary.Model.Serialization;
using CoreLibrary.Utility.Serialization;
using PdfRepresentation.Internals.Serialization;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class SerializerHelper
    {
        public static string SerializeAsString(Func<string> serialize)
        {
            var shouldSerializeDimensions = XmlDimensionsSerialization.ShouldSerializeDimensions;
            var instanceDefaultSerializationType = ObjectSerializer.Instance.DefaultSerializationType;
            ObjectSerializer.Instance.DefaultSerializationType = SerializationType.Json;
            var invoke = serialize.Invoke();
            XmlDimensionsSerialization.ShouldSerializeDimensions = false;
            XmlDimensionsSerialization.ShouldSerializeDimensions = shouldSerializeDimensions;
            ObjectSerializer.Instance.DefaultSerializationType = instanceDefaultSerializationType;
            return invoke;
        }
    }
}