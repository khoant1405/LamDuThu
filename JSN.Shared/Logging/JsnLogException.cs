using System.Runtime.Serialization;

namespace JSN.Shared.Logging;

[Serializable]
public class JsnLogException : JsnException
{
    public JsnLogException(string msg) : base(msg)
    {
    }

    protected JsnLogException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}