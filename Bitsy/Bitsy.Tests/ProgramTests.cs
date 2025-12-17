namespace Bitsy;

public class ProgramTests
{
    [Test]
    public void SimplestProgram()
    {
        var code = """
                   main(Bits b) {
                     return 0
                   }
                   """;

        var output = new Program().RunCode(code);
        
        Console.Write(code);
    }
}