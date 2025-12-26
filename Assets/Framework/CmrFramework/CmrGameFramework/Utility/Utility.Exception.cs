namespace CmrGameFramework
{
    public static partial class Utility
    {
        public static class Exception
        {
            public static GameFrameworkException ThrowException(string message)
            {
                return new GameFrameworkException(message);
            }
            public static GameFrameworkException ThrowException(string message,System.Exception e)
            {
                return new GameFrameworkException(message,e);
            }
        }
    }
}