namespace MobileDB.Common.Validation
{
    public class EntityValidationResult
    {
        public EntityValidationResult()
        {
        }

        public EntityValidationResult(string errorMessage)
        {
            Succeeded = false;
            ErrorMessage = errorMessage;
        }

        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; }

        public static EntityValidationResult Success()
        {
            return new EntityValidationResult
            {
                Succeeded = true
            };
        }
    }
}