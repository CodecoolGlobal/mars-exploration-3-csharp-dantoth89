using System;
using System.Collections.Generic;
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

        SortedSet<string> expected = new SortedSet<string>{"#","&","%","*",null};
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

    
    [Test]
    [TestCaseSource(nameof(MapCases))]
    public void TestMapIsNotEmpty(string filepath)
    {
        var map = _mapLoader.Load(filepath);
        int length = map.Representation.GetLength(0);
        
        Assert.That(length, Is.GreaterThan(0));
    }
    
    
    [Test]
    [TestCaseSource(nameof(MapCases))]
    public void TestMapIsNotThrowsAnyException(string filepath)
    {
        try
        {
            _mapLoader.Load(filepath);
        }
        catch (Exception ex)
        {
            Assert.Fail("Expected no exception, but got: " + ex.Message);
        }
    }
}