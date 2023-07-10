using System;
using Xunit;
using OrderBot;

namespace OrderBot.tests
{
    public class OverUnderTest
    {
        [Fact]
        public void Test1()
        {

        }
        [Fact]
        public void TestWelcome()
        {
            Order oOrder = new Order();
            String sInput = oOrder.OnMessage("hello");
            Assert.True(sInput.Contains("Welcome"));
        }
    }
}
