using System.Text;
using SpecFlow.Internal.Json;
using XEvent.EventManagement.Domain.Contract.Commands;

namespace XEvent.AcceptanceTests.Drivers;

public static class HttpContentExtensions
{
    public static HttpContent ToHttpCommand(this object cmd)
    {
        string content = cmd.ToJson();
        HttpContent body = new StringContent(content, Encoding.UTF8, "application/json");

        return body;
    }
}