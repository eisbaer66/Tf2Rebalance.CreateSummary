using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpower.Model;

namespace ValveFormat.Superpower.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string dir = Directory.GetCurrentDirectory();
            string path = Path.Combine(dir, "1.txt");
            string content = File.ReadAllText(path);

            IList<Node> dict;
            string error;
            Position errorPosition;
            bool success = ValveParser.TryParse(content, out dict, out error, out errorPosition);

            Assert.IsTrue(success, error);

            List<Node> expectedNodes = new List<Node>
            {
                new Node
                {
                    Name = "custom_weapons_v3",
                    Childs = new List<Node>
                    {
                        new Node
                        {
                            Name = "*",
                            Childs = new List<Node>
                            {
                                new Node
                                {
                                    Name = "60",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "1",
                                            Value = "292 ; 58"
                                        },
                                        new Node
                                        {
                                            Name = "2",
                                            Value = "48 ; 2"
                                        },
                                        new Node
                                        {
                                            Name = "3",
                                            Value = "35 ; 2"
                                        },
                                        new Node
                                        {
                                            Name = "4",
                                            Value = "728 ; 1"
                                        },
                                        new Node
                                        {
                                            Name = "5",
                                            Value = "729 ; 0.65"
                                        },
                                        new Node
                                        {
                                            Name = "6",
                                            Value = "1 ; 0.9"
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };

            AssertEquallity(dict, expectedNodes);
        }

        [TestMethod]
        public void TestMethodRealanceExample()
        {
            string dir = Directory.GetCurrentDirectory();
            string path = Path.Combine(dir, "tf2rebalance_attributes.example.txt");
            string content = File.ReadAllText(path);

            IList<Node> dict;
            string error;
            Position errorPosition;
            bool success = ValveParser.TryParse(content, out dict, out error, out errorPosition);

            Assert.IsTrue(success, error);

            List<Node> expectedNodes = new List<Node>
            {
                new Node
                {
                    Name = "tf2rebalance_attributes",
                    Childs = new List<Node>
                    {
                        new Node
                        {
                            Name = "classes",
                            Childs = new List<Node>
                            {
                                new Node
                                {
                                    Name = "scout",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "info",
                                            Value = "This is a cool info piece.\nAwesome!"
                                        },
                                        new Node
                                        {
                                            Name = "attribute1",
                                            Childs = new List<Node>
                                            {
                                                new Node
                                                {
                                                    Name = "id",
                                                    Value = "26"
                                                },
                                                new Node
                                                {
                                                    Name = "value",
                                                    Value = "100"
                                                },
                                            }
                                        },
                                    }
                                },
                                new Node
                                {
                                    Name = "soldier",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "info",
                                            Value = "Soldier:\n- Your torso is now 100% bigger. Whoa.\n- +2 HP per second."
                                        },
                                        new Node
                                        {
                                            Name = "attribute1",
                                            Childs = new List<Node>
                                            {
                                                new Node
                                                {
                                                    Name = "id",
                                                    Value = "620"
                                                },
                                                new Node
                                                {
                                                    Name = "value",
                                                    Value = "2.0"
                                                },
                                            }
                                        },
                                        new Node
                                        {
                                            Name = "attribute2",
                                            Childs = new List<Node>
                                            {
                                                new Node
                                                {
                                                    Name = "id",
                                                    Value = "190"
                                                },
                                                new Node
                                                {
                                                    Name = "value",
                                                    Value = "2"
                                                },
                                            }
                                        },
                                    }
                                },
                            }
                        },
                        new Node
                        {
                            Name = "127",
                            Childs = new List<Node>
                            {
                                new Node
                                {
                                    Name = "keepattribs",
                                    Value = "1"
                                },
                                new Node
                                {
                                    Name = "info",
                                    Value = "Direct Hit:\n- 25% jump height.\n- -50% less clip size."
                                },
                                new Node
                                {
                                    Name = "attribute1",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "id",
                                            Value = "326"
                                        },
                                        new Node
                                        {
                                            Name = "value",
                                            Value = "1.25"
                                        },
                                    }
                                },
                                new Node
                                {
                                    Name = "attribute2",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "id",
                                            Value = "3"
                                        },
                                        new Node
                                        {
                                            Name = "value",
                                            Value = "1.50"
                                        },
                                    }
                                },
                            }
                        },
                        new Node
                        {
                            Name = "225 ; 574",
                            Childs = new List<Node>
                            {
                                new Node
                                {
                                    Name = "attribute1",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "id",
                                            Value = "201"
                                        },
                                        new Node
                                        {
                                            Name = "value",
                                            Value = "2.0"
                                        },
                                    }
                                },
                                new Node
                                {
                                    Name = "attribute2",
                                    Childs = new List<Node>
                                    {
                                        new Node
                                        {
                                            Name = "id",
                                            Value = "6"
                                        },
                                        new Node
                                        {
                                            Name = "value",
                                            Value = "2.0"
                                        },
                                    }
                                },
                            }
                        },
                        new Node
                        {
                            Name = "131",
                            Childs = new List<Node>
                            {
                                new Node
                                {
                                    Name = "info",
                                    Value = "Why are you using a shield?"
                                },
                                
                                new Node
                                {
                                    Name = "keepattribs",
                                    Value = "1"
                                },
                            }
                        },
                    }
                }
            };

            AssertEquallity(dict, expectedNodes);
        }

        private void AssertEquallity(IList<Node> nodes, IList<Node> expectedNodes)
        {
            if (expectedNodes == null && nodes == null)
                return;

            Assert.AreEqual(expectedNodes.Count, nodes.Count, "Nodes.Count");

            for (int i = 0; i < expectedNodes.Count; i++)
            {
                Node node = nodes[i];
                Node expectedNode = expectedNodes[i];

                Assert.AreEqual(expectedNode.Name, node.Name, "Names dont match");
                Assert.AreEqual(expectedNode.Value, node.Value, "Values dont match");

                AssertEquallity(expectedNode.Childs, node.Childs);
            }
        }
    }
}