using MobileDB.Stores;

namespace MobileDB.Common.Attributes
{
    public class LazyBsonAttribute : StoreAttribute
    {
        public LazyBsonAttribute() : base(typeof (BsonStore))
        {
        }
    }
}