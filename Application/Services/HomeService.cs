using Application.Interfaces;

namespace Application.Services;

public class HomeService : IHomeService
{
    public string GetWelcomeMessage()
    {
        return "Witajcie w mojej kuchni!!! El primo 67";
    }
}