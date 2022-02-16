using System.Runtime.Serialization;

namespace PhantomKit.Exceptions;

public abstract class PhantomKitException : Exception
{
    public PhantomKitException()
    {
    }

    protected PhantomKitException(SerializationInfo info, StreamingContext context) : base(info: info,
        context: context)
    {
    }

    public PhantomKitException(string? message) : base(message: message)
    {
    }

    public PhantomKitException(string? message, Exception? innerException) : base(message: message,
        innerException: innerException)
    {
    }
}