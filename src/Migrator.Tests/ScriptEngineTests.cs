using System.IO;
using System.Reflection;
using Migrator.Compile;
using NUnit.Framework;

namespace Migrator.Tests
{
    [TestFixture]
    public class ScriptEngineTests
    {
        [Test]
        public void CanCompileAssemblies() 
        {
            var engine = new ScriptEngine();

            // This should let it work on windows or mono/unix I hope
            string dataPath = Path.Combine(Path.Combine("..", Path.Combine("src", "Migrator.Tests")), "Data");

            Assembly asm = engine.Compile(dataPath);
            Assert.IsNotNull(asm);

            var loader = new MigrationLoader(null, asm, false, null);
            Assert.AreEqual(2, loader.LastVersion);

            Assert.AreEqual(2, loader.GetMigrationTypes(asm, null).Count);
        }
    }
}