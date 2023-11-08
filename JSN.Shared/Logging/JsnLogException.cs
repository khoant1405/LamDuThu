using System.Runtime.Serialization;

namespace JSN.Shared.Logging;

[Serializable]
public class JsnLogException : JsnException
{
    #region Constructor

    public JsnLogException(string msg) : base(msg)
    {
    }

    protected JsnLogException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    #endregion
}