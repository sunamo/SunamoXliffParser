// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
namespace SunamoXliffParser;

/*
 <header>
  <tool tool-id="MultilingualAppToolkit" tool-name="Multilingual App Toolkit" tool-version="3.1.1250.0" tool-company="Microsoft" />
 </header>
*/

public class XlfTool
{
    public string Company { get; }

    public string Id { get; }

    public string Name { get; }

    public string Version { get; }
}