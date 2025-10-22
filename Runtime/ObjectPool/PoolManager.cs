// ------------------------------------------------------------------------------
//  File: PoolManager.cs
//  Author: Ran
//  Description: Base class for managing object pools.
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
using System.Linq;
using UnityEngine;

namespace ran.utilities
{
    /// <summary>
    /// Acts as a base class for managing object pools. 
    /// Use this class as a reference in the inspector instead of using concrete implementations. <br/>
    /// Alternatively, use <see cref="IPoolManager"/> in the inspector for automatic component retrieval using <see cref="GameObject.GetComponent{T}()"/>.
    /// </summary>
    public abstract class PoolManager : MonoBehaviour, IPoolManager
    {
        [SerializeField] protected bool autoInitialize = true;
        [SerializeField] protected int initialPoolSize = 10;
        [SerializeField] protected bool hasMaxPoolSize;
        [SerializeField] protected int maxPoolSize = 10;
        [SerializeField] protected Transform container;

        protected readonly List<IPoolableObject> pool = new();

        /// <inheritdoc />
        public int PoolSize => pool.Count;
        /// <inheritdoc />
        public int MaxPoolSize => hasMaxPoolSize ? maxPoolSize : int.MaxValue;
        /// <summary>
        /// Checks if the pool has reached its maximum size.
        /// </summary>
        protected bool IsPoolFull
        {
            get
            {
                if (hasMaxPoolSize && PoolSize >= maxPoolSize)
                {
                    Debug.LogWarning($"Max pool size '{maxPoolSize}' has been reached for '{GetType().Name}'. Cannot create new object.");
                    return true;
                }

                return false;
            }
        }

        /// <inheritdoc />
        public int ActiveObjectsCount => pool.Count(obj => obj.gameObject.activeSelf);
        /// <inheritdoc />
        public int InactiveObjectsCount => pool.Count(obj => !obj.gameObject.activeSelf);
        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Validates serialized fields in the editor.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (hasMaxPoolSize && initialPoolSize > maxPoolSize)
            {
                Debug.LogWarning($"Max pool size '{maxPoolSize}' cannot be less than initial pool size '{initialPoolSize}'. Setting it to '{initialPoolSize}'.");
                maxPoolSize = initialPoolSize;
            }
        }

        /// <inheritdoc />
        public void ValidateInitialSize(int size)
        {
            if (size < 0)
            {
                Debug.LogWarning($"Initial pool size '{size}' cannot be negative. Setting it to 0.");

                size = 0;
            }

            initialPoolSize = size;
        }

        /// <summary>
        /// Always call the base Awake method when overriding this method.
        /// </summary>
        protected virtual void Awake()
        {
            if (autoInitialize) Initialize();
        }

        /// <inheritdoc />
        public void Initialize()
        {
            InitializePool();

            IsInitialized = true;
        }

        /// <summary>
        /// Initializes the pool by creating objects based on the initial pool size.
        /// </summary>
        protected virtual void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewObject();
            }
        }

        /// <summary>
        /// Creates a new object and adds it to the pool.
        /// </summary>
        /// <returns>The newly created object.</returns>
        protected virtual IPoolableObject CreateNewObject()
        {
            if (IsPoolFull) return null;

            IPoolableObject newObject = InstantiatePrefab();

            if (newObject == null) return null;

            pool.Add(newObject);

            newObject.OnDespawn();
            newObject.gameObject.name = $"{newObject.GetType().Name} {pool.Count}";
            return newObject;
        }

        /// <summary>
        /// Instantiates a new object from a prefab.
        /// </summary>
        /// <returns>The instantiated <see cref="IPoolableObject"/>.</returns>
        protected abstract IPoolableObject InstantiatePrefab();

        /// <inheritdoc />
        public abstract void SetPoolPrefabs<T>(IEnumerable<T> prefabs) where T : MonoBehaviour, IPoolableObject;

        /// <inheritdoc />
        public IEnumerable<T> GetAllPoolObjects<T>() where T : MonoBehaviour, IPoolableObject
        {
            return pool.OfType<T>();
        }

        /// <inheritdoc />
        public IEnumerable<T> GetAllPoolObjects<T>(Func<T, bool> condition) where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>().Where(condition);
        }

        /// <summary>
        /// Retrieves the first inactive object of the specified type from the pool.
        /// If no inactive object is found, the method returns null.
        /// </summary>
        /// <typeparam name="T">The type of object to retrieve. Must implement <see cref="IPoolableObject"/> and inherit from <see cref="MonoBehaviour"/>.</typeparam>
        /// <returns>The first inactive object of type <typeparamref name="T"/>, or null if none are available.</returns>
        protected virtual T GetAvailableObject<T>() where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>().FirstOrDefault(obj => !obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public virtual T TryGetAvailableObject<T>() where T : MonoBehaviour, IPoolableObject
        {
            T availableObject = GetAvailableObject<T>() ?? (T)CreateNewObject();
            availableObject?.OnSpawn();
            return availableObject;
        }

        /// <inheritdoc />
        public virtual T GetActiveObject<T>() where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>().FirstOrDefault(obj => obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public virtual IEnumerable<T> GetActiveObjects<T>() where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>(obj => obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public virtual T GetSpecificObject<T>(Func<T, bool> condition, bool alsoCheckInactive = false) where T : MonoBehaviour, IPoolableObject
        {
            if (alsoCheckInactive) return GetAllPoolObjects<T>(condition).FirstOrDefault();
            return GetAllPoolObjects<T>(condition).FirstOrDefault(obj => obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public bool AnyActiveObject<T>() where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>().Any(obj => obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public bool AnyInactiveObject<T>() where T : MonoBehaviour, IPoolableObject
        {
            return GetAllPoolObjects<T>().Any(obj => !obj.gameObject.activeSelf);
        }

        /// <inheritdoc />
        public int GetIndexOf(IPoolableObject obj)
        {
            return pool.IndexOf(obj);
        }

        /// <inheritdoc />
        public T GetObjectAt<T>(int index)
        {
            return (T)pool[index];
        }

        /// <inheritdoc />
        public virtual void SpawnObject(IPoolableObject obj)
        {
            obj.OnSpawn();
        }

        /// <inheritdoc />
        public virtual void DespawnObject(IPoolableObject obj)
        {
            obj.OnDespawn();
        }

        /// <inheritdoc />
        public virtual void DespawnAllObjects()
        {
            pool.ForEach(obj => obj.OnDespawn());
        }

        /// <inheritdoc />
        public virtual void SortObjects()
        {
            pool.Sort();
        }

        /// <inheritdoc />
        public virtual void SortObjects<T>(Comparison<T> comparison)
        {
            pool.Sort((obj1, obj2) => comparison((T)obj1, (T)obj2));
        }

        /// <inheritdoc />
        public void AddObject(IPoolableObject obj)
        {
            pool.Add(obj);
        }

        /// <inheritdoc />
        public void InsertObjectAt(int index, IPoolableObject obj)
        {
            pool.Insert(index, obj);
        }

        /// <inheritdoc />
        public void RemoveObject(IPoolableObject obj)
        {
            pool.Remove(obj);
        }

        /// <inheritdoc />
        public void RemoveObjectAt(int index)
        {
            pool.RemoveAt(index);
        }
    }

    /// <summary>
    /// A single-object pool manager for managing pools of one type of object.
    /// </summary>
    /// <typeparam name="V">The type of poolable object, must implement <see cref="IPoolableObject"/>.</typeparam>
    public abstract class SingleObjectPoolManager<V> : PoolManager where V : MonoBehaviour, IPoolableObject
    {
        [SerializeField] protected V prefab;

        /// <inheritdoc />
        protected override IPoolableObject InstantiatePrefab()
        {
            if (prefab == null)
            {
                Debug.LogWarning("Prefab is null. Please assign a prefab.");
                return null;
            }

            return Instantiate(prefab, container);
        }

        /// <inheritdoc />
        public override void SetPoolPrefabs<T>(IEnumerable<T> prefabs)
        {
            prefab = prefabs.Cast<V>()
                .OrderBy(_ => UnityEngine.Random.value)
                .FirstOrDefault();
        }
    }

    /// <summary>
    /// A pool manager that randomly selects prefabs for creating objects.
    /// </summary>
    /// <typeparam name="V">The type of poolable object, must implement <see cref="IPoolableObject"/>.</typeparam>
    public abstract class RandomObjectPoolManager<V> : PoolManager where V : MonoBehaviour, IPoolableObject
    {
        [SerializeField] protected V[] prefabs;

        /// <summary>
        /// Retrieves a random prefab from the pool.
        /// </summary>
        protected V GetRandomPrefab()
        {
            return prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
        }

        /// <inheritdoc />
        protected override IPoolableObject InstantiatePrefab()
        {
            if (!prefabs.Any())
            {
                Debug.LogWarning("Prefab array is empty. Please add prefabs to the array.");
                return null;
            }

            return Instantiate(GetRandomPrefab(), container);
        }

        /// <inheritdoc />
        public override void SetPoolPrefabs<T>(IEnumerable<T> prefabs)
        {
            this.prefabs = prefabs.Cast<V>()
                .ToArray();
        }
    }

    /// <summary>
    /// A multi-object pool manager that cycles through prefabs for creating objects.
    /// </summary>
    /// <typeparam name="V">The type of poolable object, must implement <see cref="IPoolableObject"/>.</typeparam>
    public abstract class MultiObjectPoolManager<V> : PoolManager where V : MonoBehaviour, IPoolableObject
    {
        [SerializeField] protected V[] prefabs;

        /// <summary>
        /// Retrieves the next prefab to use based on the pool size.
        /// </summary>
        protected V GetPrefab()
        {
            return prefabs[PoolSize % prefabs.Length];
        }

        /// <inheritdoc />
        protected override IPoolableObject InstantiatePrefab()
        {
            if (!prefabs.Any())
            {
                Debug.LogWarning("Prefab array is empty. Please add prefabs to the array.");
                return null;
            }

            return Instantiate(GetPrefab(), container);
        }

        /// <inheritdoc />
        public override void SetPoolPrefabs<T>(IEnumerable<T> prefabs)
        {
            this.prefabs = prefabs.Cast<V>()
                .ToArray();
        }
    }
}
