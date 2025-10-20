// ------------------------------------------------------------------------------
//  File: IPoolableObject.cs
//  Author: Ran
//  Description: Interface for objects that can be pooled.
//  Created: 2025
//  
//  Copyright (c) 2025 Ran.
//  This script is part of the ran.utilities namespace.
//  Permission is granted to use, modify, and distribute this file freely
//  for both personal and commercial projects, provided that this notice
//  remains intact.
// ------------------------------------------------------------------------------

using UnityEngine;

namespace ran.utilities
{
    /// <summary>
    /// Interface for objects that can be pooled.
    /// </summary>
    public interface IPoolableObject
    {
        /// <summary>
        /// Return the GameObject of the object
        /// </summary>
        /// <value></value>
        GameObject gameObject { get; }

        /// <summary>
        /// Called when the object is spawned. Dont call this method directly, use <see cref="IPoolManager.SpawnObject(IPoolableObject)"/> instead
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// Called when the object is despawn. Dont call this method directly, use <see cref="IPoolManager.DespawnObject(IPoolableObject)"/> instead
        /// </summary>
        void OnDespawn();
    }
}
