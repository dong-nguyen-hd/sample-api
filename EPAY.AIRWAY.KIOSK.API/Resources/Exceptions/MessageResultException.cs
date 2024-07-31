namespace EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;

public class MessageResultException : Exception
{
    #region Constructor

    public MessageResultException()
    {
    }

    public MessageResultException(string message)
        : base(message)
    {
    }

    public MessageResultException(string message, Exception inner)
        : base(message, inner)
    {
    }

    #endregion
}