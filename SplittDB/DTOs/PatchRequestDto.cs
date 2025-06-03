namespace SplittDB.DTOs
{
    public abstract class PatchRequestDto
    {
        private readonly HashSet<string> _properties = new HashSet<string>();

        public bool HasProperty(string propertyName) => _properties.Contains(propertyName);
        protected void SetHasProperty(string propertyName) => _properties.Add(propertyName);
    }
}