namespace ItemSystem
{
    [System.Serializable]
    public class TypeException : System.Exception
    {
        public TypeException() { }
        public TypeException(string message) : base(message) { }
        public TypeException(string message, System.Exception inner) : base(message, inner) { }
        protected TypeException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}