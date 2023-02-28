using System;
using System.Collections.Generic;
using System.Linq;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using NUnit.Framework;

namespace CodeCool.MarsExploration.MapExplorerTests;

public class MapLoaderTests
{
    private readonly MapLoader _mapLoader = new MapLoader();
    private static readonly string WorkDir = System.Environment.CurrentDirectory.Replace("/bin/Debug/net6.0/", "");
    
    public static object[] MapCases =
    {
        new object[] { $@"{WorkDir}/Resources/exploration-0.map"},
        new object[] { $@"{WorkDir}/Resources/exploration-1.map"},
        new object[] { $@"{WorkDir}/Resources/exploration-2.map"}
    };

    [Test]
    [TestCaseSource(nameof(MapCases))]

    public void TestMapIsContainsAllSymbols(string filepath)
    {
        var map = _mapLoader.Load(filepath);
        Console.WriteLine(filepath);
        
        SortedSet<string> expected = new SortedSet<string>{"#"," ","&","%","*"};
        SortedSet<string> actual = new SortedSet<string>();
        
        for (int i = 0; i < map.Representation.GetLength(0); i++)
        {
            for (int j = 0; j < map.Representation.GetLength(1); j++)
            {
                actual.Add(map.Representation[i, j]!);
            }
        }
        
        Assert.That(expected, Is.EqualTo(actual));

    }

    
    
    
    
}