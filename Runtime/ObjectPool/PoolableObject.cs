// ------------------------------------------------------------------------------
//  File: PoolableObject.cs
//  Author: Ran
//  Description: Base class for all objects that can be pooled.
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
    /// As a base class for each GameObject that can be pooled.
    /// </summary>
    public abstract class PoolableObject : MonoBehaviour, IPoolableObject
    {
        public virtual void OnSpawn() => gameObject.SetActive(true);
        public virtual void OnDespawn() => gameObject.SetActive(false);
    }
}
