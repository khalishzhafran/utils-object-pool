# Unity Object Pool Manager Utilities

This package provides a flexible and extensible object pooling system for Unity projects, including interfaces and base classes for poolable objects and pool managers.

## Features

-   Generic interfaces for poolable objects and pool managers
-   Base classes for pooled GameObjects and UI elements
-   Multiple pool manager types: single, random, and multi-prefab pools
-   Automatic pool initialization and size management
-   Easy integration with Unity inspector and serialization
-   Extensible for custom pooling logic

## Usage

### IPoolableObject

Implement `IPoolableObject` for any object you want to pool:

```csharp
using ran.utilities;

public class MyEnemy : PoolableObject
{
	public override void OnSpawn()
	{
		base.OnSpawn();
		// Custom spawn logic
	}

	public override void OnDespawn()
	{
		base.OnDespawn();
		// Custom despawn logic
	}
}
```

### PoolManager

Inherit from `PoolManager` or use provided variants for different pooling strategies:

-   `SingleObjectPoolManager<T>`: Pools a single prefab type.
-   `RandomObjectPoolManager<T>`: Pools multiple prefabs, randomly selecting on spawn.
-   `MultiObjectPoolManager<T>`: Pools multiple prefabs, cycling through them.

Example:

```csharp
public class EnemyPool : SingleObjectPoolManager<MyEnemy>
{
	// Assign prefab in inspector
}
```

### UI Pooling

For UI elements, inherit from `UIVisualPoolableObject<T>`:

```csharp
public class MyUIPopup : UIVisualPoolableObject<MyUIPopup>
{
	// Custom UI logic
}
```

## API Reference

### IPoolableObject

-   `GameObject gameObject`: Reference to the pooled GameObject
-   `OnSpawn()`: Called when object is activated
-   `OnDespawn()`: Called when object is deactivated

### IPoolManager

-   `PoolSize`, `MaxPoolSize`, `ActiveObjectsCount`, `InactiveObjectsCount`
-   `ValidateInitialSize(int size)`
-   `SetPoolPrefabs<T>(IEnumerable<T> prefabs)`
-   `TryGetAvailableObject<T>()`
-   `SpawnObject(IPoolableObject obj)`, `DespawnObject(IPoolableObject obj)`
-   Many more for advanced pool management

### PoolableObject

-   Base MonoBehaviour implementing `IPoolableObject`
-   Handles activation/deactivation

### PoolManager Variants

-   `SingleObjectPoolManager<T>`
-   `RandomObjectPoolManager<T>`
-   `MultiObjectPoolManager<T>`

## Installation

### Option 1: Unity Git Package Manager (Recommended)

Add the following line to your project's `manifest.json` dependencies:

```json
"com.litsch.utilities.poolmanager": "https://github.com/yourusername/utils-pool-manager.git"
```

### Option 2: Manual Copy

Copy the `Runtime/ObjectPool` folder into your Unity project's `Assets` directory.

## License

Copyright (c) 2025 Ran. Free to use, modify, and distribute for personal and commercial projects as long as this notice remains intact.
