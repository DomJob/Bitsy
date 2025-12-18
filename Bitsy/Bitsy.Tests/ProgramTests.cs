using Bitsy.Evaluating;

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
        
        Assert.That(output.Count, Is.EqualTo(1));
        Assert.That(output.First(), Is.EqualTo(Bit.Zero));
    }
}