using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Regulus.Project.UnboundarySnake
{
    using Extension;
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSnakeInit()
        {            
            var snake = new Snake();
            var body = snake.GetBody();
            var bodyitems = body.ToArray();

            Assert.AreEqual(1, snake.TotalLength);
            Assert.AreEqual(0, bodyitems[0].Position.X);
            Assert.AreEqual(0, bodyitems[0].Position.Y);
        }
        [TestMethod]
        public void TestSnakeMoveLeft()
        {
            var snake = new Snake();

            snake.Move(DIRECTION.Left);

            var body = snake.GetBody();
            var position = body.ToArray()[0].Position;

            Assert.AreEqual(1, snake.TotalLength);
            Assert.AreEqual(-1, position.X);
            Assert.AreEqual(0, position.Y);
        }

        [TestMethod]
        public void TestSnakeGrowUp()
        {
            var snake = new Snake();

            snake.GrowUp(1);

            Assert.AreEqual(2, snake.TotalLength);
            
        }

        [TestMethod]
        public void TestSnakeGrowUpMoveUp()
        {
            var snake = new Snake();

            snake.GrowUp(2);

            snake.Move(DIRECTION.Up);
            var body = snake.GetBody();
            var bodyitems = body.ToArray();


            Assert.AreEqual(3, snake.TotalLength);
            Assert.AreEqual(2, bodyitems.Length);

            Assert.AreEqual(0,bodyitems[0].Position.X);
            Assert.AreEqual(1,bodyitems[0].Position.Y);

            Assert.AreEqual(0,bodyitems[1].Position.X);
            Assert.AreEqual(0,bodyitems[1].Position.Y);
        }


        [TestMethod]
        public void TestSnakeGrowUpMove2Step()
        {
            var snake = new Snake();

            snake.GrowUp(2);

            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Right);

            var body = snake.GetBody();
            var bodyitems = body.ToArray();


            Assert.AreEqual(3, snake.TotalLength);
            Assert.AreEqual(3, bodyitems.Length);

            Assert.AreEqual(1, bodyitems[0].Position.X);
            Assert.AreEqual(1, bodyitems[0].Position.Y);

            Assert.AreEqual(0, bodyitems[1].Position.X);
            Assert.AreEqual(1, bodyitems[1].Position.Y);
            
            Assert.AreEqual(0, bodyitems[2].Position.X);
            Assert.AreEqual(0, bodyitems[2].Position.Y);
        }

        [TestMethod]
        public void TestSnakeBreak0Node()
        {
            var snake = new Snake();
            snake.GrowUp(2);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);

            var body = snake.GetBody();
            var bodyitems = body.ToArray();

            snake.Break(bodyitems[0]);

            body = snake.GetBody();
            bodyitems = body.ToArray();

            Assert.AreEqual(0, bodyitems[0].Position.X);
            Assert.AreEqual(3, bodyitems[0].Position.Y);

            Assert.AreEqual(0, bodyitems[1].Position.X);
            Assert.AreEqual(2, bodyitems[1].Position.Y);

            
            
        }

        [TestMethod]
        public void TestSnakeBreak1Node()
        {
            var snake = new Snake();
            snake.GrowUp(2);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);

            var body = snake.GetBody();
            var bodyitems = body.ToArray();

            snake.Break(bodyitems[1]);

            body = snake.GetBody();
            bodyitems = body.ToArray();

            Assert.AreEqual(0, bodyitems[0].Position.X);
            Assert.AreEqual(3, bodyitems[0].Position.Y);

            Assert.AreEqual(0, bodyitems[1].Position.X);
            Assert.AreEqual(2, bodyitems[1].Position.Y);



        }

        [TestMethod]
        public void TestSnakeBreak2Node()
        {
            var snake = new Snake();
            snake.GrowUp(2);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);
            snake.Move(DIRECTION.Up);

            var body = snake.GetBody();
            var bodyitems = body.ToArray();

            snake.Break(bodyitems[2]);

            body = snake.GetBody();
            bodyitems = body.ToArray();

            Assert.AreEqual(0, bodyitems[0].Position.X);
            Assert.AreEqual(3, bodyitems[0].Position.Y);

            Assert.AreEqual(0, bodyitems[1].Position.X);
            Assert.AreEqual(2, bodyitems[1].Position.Y);



        }

    }
}
