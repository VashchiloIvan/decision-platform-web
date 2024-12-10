using System;
using System.Collections.Generic;
using System.Threading;
using DecisionPlatformWeb.Service.Solver;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DecisionPlatformWeb.Tests.Service.Cache;

[TestClass]
[TestSubject(typeof(DecisionPlatformWeb.Service.Cache.Cache))]
public class CacheTest
{
    private DecisionPlatformWeb.Service.Cache.Cache _cache;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DecisionPlatformWeb.Service.Cache.Cache(1); // тайм-аут 1 минута
    }

    [TestMethod]
    public void AddObject_ShouldReturnNewGuid()
    {
        // Arrange
        var solver = new MultiStepSolver(null, null);

        // Act
        var guid = _cache.AddObject(solver);

        // Assert
        Assert.IsNotNull(guid);
    }

    [TestMethod]
    public void GetObject_ShouldReturnCorrectObject()
    {
        // Arrange
        var solver = new MultiStepSolver(null, null);
        var guid = _cache.AddObject(solver);

        // Act
        var result = _cache.GetObject(guid);

        // Assert
        Assert.AreEqual(solver, result);
    }

    [TestMethod]
    public void GetObject_ShouldReturnNull_WhenObjectDoesNotExist()
    {
        // Arrange
        var guid = Guid.NewGuid(); // Не добавляем в кэш

        // Act
        var result = _cache.GetObject(guid);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void UpdateObject_ShouldUpdateObjectInCache()
    {
        // Arrange
        var solver = new MultiStepSolver(null, null);
        var guid = _cache.AddObject(solver);
        var newSolver = new MultiStepSolver(null, null);

        // Act
        _cache.UpdateObject(guid, newSolver);
        var result = _cache.GetObject(guid);

        // Assert
        Assert.AreEqual(newSolver, result);
    }

    [TestMethod]
    [ExpectedException(typeof(KeyNotFoundException))]
    public void UpdateObject_ShouldThrowException_WhenObjectDoesNotExist()
    {
        // Arrange
        var newSolver = new MultiStepSolver(null, null);
        var guid = Guid.NewGuid(); // Не добавляем в кэш

        // Act
        _cache.UpdateObject(guid, newSolver);
    }

    [TestMethod]
    public void RemoveObject_ShouldRemoveObjectFromCache()
    {
        // Arrange
        var solver = new MultiStepSolver(null, null);
        var guid = _cache.AddObject(solver);

        // Act
        _cache.RemoveObject(guid);
        var result = _cache.GetObject(guid);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Cache_ShouldExpireObject_AfterTimeout()
    {
        // Arrange
        var solver = new MultiStepSolver(null, null);
        var guid = _cache.AddObject(solver);

        // Act
        Thread.Sleep(60000); // ждем 1 минуту, чтобы объект истек

        // Assert
        var result = _cache.GetObject(guid);
        Assert.IsNull(result);
    }
}