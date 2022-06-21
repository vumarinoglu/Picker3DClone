using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class GamePools : MonoBehaviour
{
	public ObjectPool smallSpherePool;
	public ObjectPool mediumSpherePool;
	public ObjectPool largeSpherePool;
	public ObjectPool smallCubePool;
	public ObjectPool mediumCubePool;
	public ObjectPool largeCubePool;
	public ObjectPool smallCapsulePool;
	public ObjectPool mediumCapsulePool;
	public ObjectPool largeCapsulePool;
	public ObjectPool wingsPool;

	private readonly List<ObjectPool> collectiblePools = new List<ObjectPool>();

	/// <summary>
	/// Unity's Awake method.
	/// </summary>
	private void Awake()
	{
		Assert.IsNotNull(smallSpherePool);
		Assert.IsNotNull(mediumSpherePool);
		Assert.IsNotNull(largeSpherePool);
		Assert.IsNotNull(smallCubePool);
		Assert.IsNotNull(mediumCubePool);
		Assert.IsNotNull(largeCubePool);

		collectiblePools.Add(smallSpherePool);
		collectiblePools.Add(mediumSpherePool);
		collectiblePools.Add(largeSpherePool);
		collectiblePools.Add(smallCubePool);
		collectiblePools.Add(mediumCubePool);
		collectiblePools.Add(largeCubePool);
	}
}
