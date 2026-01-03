using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolWheel
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void SampleTest()
        {
            Assert.That(2, Is.EqualTo(1 + 1));
        }
    }
}
