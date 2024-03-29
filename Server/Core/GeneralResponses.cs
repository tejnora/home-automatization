﻿namespace Server.Core;

public enum ResponseType
{
    AccessDenied,
    Success,
    IncorrectLoginData,
    Failed
}

public class GeneralResponses : Define.IResponse
{
    public static GeneralResponses AccessDenied { get; } = new GeneralResponses() { Result = ResponseType.AccessDenied };
    public static GeneralResponses Success { get; } = new GeneralResponses() { Result = ResponseType.Success };
    public static GeneralResponses Failed { get; } = new GeneralResponses() { Result = ResponseType.Failed };
    public ResponseType Result { get; private set; }
}