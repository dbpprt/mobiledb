using MobileDB.Stores.Json;

namespace MobileDB.Common.Attributes
{
    public class JsonAttribute : StoreAttribute
    {
        public JsonAttribute() : base(typeof (JsonStore))
        {
        }
    }
}