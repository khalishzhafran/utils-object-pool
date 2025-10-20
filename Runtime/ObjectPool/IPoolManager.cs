// ------------------------------------------------------------------------------
//  File: IPoolManager.cs
//  Author: Ran
//  Description: Interface for managing object pools with functionality to create, retrieve, and manipulate pooled objects.
//  Created: 2025
//  
//  Copyright (c) 2025 Ran.
//  This script is part of the ran.utilities namespace.
//  Permission is granted to use, modify, and distribute this file freely
//  for both personal and commercial projects, provided that this notice
//  remains intact.
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ran.utilities
{
    /// <summary>
    /// Interface for managing object pools with functionality to create, retrieve, and manipulate pooled objects.
    /// </summary>
    public interface IPoolManager : IInitializable
    {
        /// <summary>
        /// Gets the total number of objects currently in the pool.
        /// </summary>
        /// <value>The total number of objects in the pool.</value>
        int PoolSize { get; }

        /// <summary>
        /// Gets the maximum allowed number of objects in the pool.
        /// </summary>
        /// <value>The maximum capacity of the pool.</value>
        int MaxPoolSize { get; }

        /// <summary>
        /// Gets the count of active (in-use) objects in the pool.
        /// </summary>
        /// <value>The number of objects that are currently active.</value>
        int ActiveObjectsCount { get; }

        /// <summary>
        /// Gets the count of inactive (available) objects in the pool.
        /// </summary>
        /// <value>The number of objects that are currently inactive.</value>
        int InactiveObjectsCount { get; }

        /// <summary>
        /// Validates and sets the initial size of the pool.
        /// </summary>
        /// <param name="size">The initial number of objects in the pool. Must be non-negative.</param>
        void ValidateInitialSize(int size);

        /// <summary>
        /// Sets the prefabs used to create new objects for the pool.
        /// </summary>
        /// <typeparam name="T">The type of the prefab objects, which must implement <see cref="IPoolableObject"/> and derive from <see cref="MonoBehaviour"/>.</typeparam>
        /// <param name="prefabs">A collection of prefab objects.</param>
        void SetPoolPrefabs<T>(IEnumerable<T> prefabs) where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Retrieves all objects currently in the pool.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <returns>An enumerable collection of objects in the pool.</returns>
        IEnumerable<T> GetAllPoolObjects<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Retrieves all objects in the pool that satisfy a specified condition.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <param name="condition">A predicate to filter the objects.</param>
        /// <returns>An enumerable collection of objects matching the condition.</returns>
        IEnumerable<T> GetAllPoolObjects<T>(Func<T, bool> condition) where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Tries to get an available (inactive) object from the pool. If none are available, creates a new object and activates it.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <returns>An available object of the specified type.</returns>
        T TryGetAvailableObject<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Retrieves an active (in-use) object from the pool.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <returns>An active object of the specified type, or <c>null</c> if none are active.</returns>
        T GetActiveObject<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Retrieves all active objects of a specified type from the pool.
        /// </summary>
        /// <typeparam name="T">The type of objects to retrieve.</typeparam>
        /// <returns>An enumerable collection of active objects.</returns>
        IEnumerable<T> GetActiveObjects<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Retrieves a specific object from the pool based on a condition.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve.</typeparam>
        /// <param name="condition">A predicate to filter objects.</param>
        /// <param name="alsoCheckInactive">Whether to include inactive objects in the search.</param>
        /// <returns>An object matching the condition, or <c>null</c> if no match is found.</returns>
        T GetSpecificObject<T>(Func<T, bool> condition, bool alsoCheckInactive = false) where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Checks if there is any active object of a specified type in the pool.
        /// </summary>
        /// <typeparam name="T">The type of object to check for.</typeparam>
        /// <returns><c>true</c> if there is at least one active object; otherwise, <c>false</c>.</returns>
        bool AnyActiveObject<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Checks if there is any inactive object of a specified type in the pool.
        /// </summary>
        /// <typeparam name="T">The type of object to check for.</typeparam>
        /// <returns><c>true</c> if there is at least one inactive object; otherwise, <c>false</c>.</returns>
        bool AnyInactiveObject<T>() where T : MonoBehaviour, IPoolableObject;

        /// <summary>
        /// Gets the index of a specific object in the pool.
        /// </summary>
        /// <param name="obj">The object whose index is to be found.</param>
        /// <returns>The zero-based index of the object, or -1 if not found.</returns>
        int GetIndexOf(IPoolableObject obj);

        /// <summary>
        /// Retrieves the object at a specific index in the pool.
        /// </summary>
        /// <typeparam name="T">The type of the object to retrieve.</typeparam>
        /// <param name="index">The zero-based index of the object.</param>
        /// <returns>The object at the specified index.</returns>
        T GetObjectAt<T>(int index);

        /// <summary>
        /// Activates a specific object in the pool.
        /// </summary>
        /// <param name="obj">The object to activate.</param>
        void SpawnObject(IPoolableObject obj);

        /// <summary>
        /// Deactivates a specific object in the pool.
        /// </summary>
        /// <param name="obj">The object to deactivate.</param>
        void DespawnObject(IPoolableObject obj);

        /// <summary>
        /// Deactivates all objects in the pool.
        /// </summary>
        void DespawnAllObjects();

        /// <summary>
        /// Sorts all objects in the pool using their default comparison logic.
        /// </summary>
        void SortObjects();

        /// <summary>
        /// Sorts all objects in the pool using a custom comparison logic.
        /// </summary>
        /// <typeparam name="T">The type of objects to sort.</typeparam>
        /// <param name="comparison">The comparison logic to apply.</param>
        void SortObjects<T>(Comparison<T> comparison);

        /// <summary>
        /// Adds an object to the pool.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        void AddObject(IPoolableObject obj);

        /// <summary>
        /// Inserts an object at a specific index in the pool.
        /// </summary>
        /// <param name="index">The zero-based index where the object should be inserted.</param>
        /// <param name="obj">The object to insert.</param>
        void InsertObjectAt(int index, IPoolableObject obj);

        /// <summary>
        /// Removes a specific object from the pool.
        /// </summary>
        /// <param name="obj">The object to remove.</param>
        void RemoveObject(IPoolableObject obj);

        /// <summary>
        /// Removes the object at a specific index in the pool.
        /// </summary>
        /// <param name="index">The zero-based index of the object to remove.</param>
        void RemoveObjectAt(int index);
    }
}
