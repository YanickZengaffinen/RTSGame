using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{

    [SerializeField]
    private string typeFileResource;

    [SerializeField]
    private Mesh testMesh;

    [SerializeField]
    private Material testMaterial;

    private EntityManager entityManager;
    private EntityArchetype unitArcheType;

    private long currentId = long.MinValue;

    private IDictionary<int, UnitType> unitTypesById;

    private RenderMesh sharedRenderMesh;

    private void Start()
    {
        entityManager = World.Active.EntityManager;

        unitArcheType = entityManager.CreateArchetype(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(Unit)
        );

        sharedRenderMesh = new RenderMesh() { mesh = testMesh, material = testMaterial };

        unitTypesById = new UnitTypeLoader().LoadFromJsonResource(typeFileResource).ToDictionary(x => x.id);

        SpawnAt(14, new float3(0,10,0));
        SpawnAt(-1, new float3(10,10,10));
    }

    public void SpawnAt(int id, float3 pos)
    {
        var type = unitTypesById[id];

        var entity = entityManager.CreateEntity(unitArcheType);
        entityManager.SetComponentData(entity, new Translation() { Value = pos });
        entityManager.SetComponentData(entity, new Unit(currentId++, type.id));
        entityManager.AddSharedComponentData(entity, sharedRenderMesh);

        entityManager.Instantiate(entity);
    }
}
