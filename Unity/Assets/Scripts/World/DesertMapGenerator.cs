using UnityEngine;

namespace SniperOnline.World
{
    /// <summary>Builds a deterministic 4 km x 4 km desert test map at runtime.</summary>
    [DisallowMultipleComponent]
    public sealed class DesertMapGenerator : MonoBehaviour
    {
        [SerializeField, Min(512f)] private float mapSize = 4000f;
        [SerializeField, Range(16, 160)] private int gridResolution = 80;
        [SerializeField] private int seed = 508;
        [SerializeField] private float heightScale = 115f;

        private void Start() => Generate();

        [ContextMenu("Generate Desert")]
        public void Generate()
        {
            var old = transform.Find("GeneratedDesert");
            if (old != null)
            {
                if (Application.isPlaying) Destroy(old.gameObject);
                else DestroyImmediate(old.gameObject);
            }

            var root = new GameObject("GeneratedDesert").transform;
            root.SetParent(transform, false);
            var vertices = new Vector3[(gridResolution + 1) * (gridResolution + 1)];
            var uv = new Vector2[vertices.Length];
            var triangles = new int[gridResolution * gridResolution * 6];
            var random = new System.Random(seed);
            float offsetX = (float)random.NextDouble() * 1000f;
            float offsetZ = (float)random.NextDouble() * 1000f;
            int index = 0;
            for (int z = 0; z <= gridResolution; z++)
            for (int x = 0; x <= gridResolution; x++)
            {
                float px = (float)x / gridResolution * mapSize;
                float pz = (float)z / gridResolution * mapSize;
                float broad = Mathf.PerlinNoise((px + offsetX) / 900f, (pz + offsetZ) / 900f);
                float detail = Mathf.PerlinNoise((px + offsetX + 300f) / 210f, (pz + offsetZ + 300f) / 210f);
                float dune = Mathf.Sin((px + pz * 0.42f) / 185f) * 0.5f + 0.5f;
                float y = (broad * 0.72f + detail * 0.16f + dune * 0.12f) * heightScale;
                vertices[index] = new Vector3(px - mapSize * 0.5f, y, pz - mapSize * 0.5f);
                uv[index] = new Vector2((float)x / gridResolution, (float)z / gridResolution);
                index++;
            }

            int ti = 0;
            for (int z = 0; z < gridResolution; z++)
            for (int x = 0; x < gridResolution; x++)
            {
                int a = z * (gridResolution + 1) + x;
                int b = a + gridResolution + 1;
                triangles[ti++] = a; triangles[ti++] = b; triangles[ti++] = a + 1;
                triangles[ti++] = a + 1; triangles[ti++] = b; triangles[ti++] = b + 1;
            }

            var mesh = new Mesh { name = "ProceduralDesertMesh", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.vertices = vertices; mesh.uv = uv; mesh.triangles = triangles; mesh.RecalculateNormals();
            var terrain = new GameObject("SandTerrain");
            terrain.transform.SetParent(root, false);
            var filter = terrain.AddComponent<MeshFilter>(); filter.sharedMesh = mesh;
            var renderer = terrain.AddComponent<MeshRenderer>();
            var material = new Material(Shader.Find("Standard"));
            material.color = new Color(0.66f, 0.48f, 0.27f);
            renderer.sharedMaterial = material;
            terrain.AddComponent<MeshCollider>().sharedMesh = mesh;
            CreateRock(root, new Vector3(-720, 0, 430), new Vector3(210, 95, 170));
            CreateRock(root, new Vector3(850, 0, -610), new Vector3(280, 125, 220));
            CreateRock(root, new Vector3(120, 0, 1050), new Vector3(160, 70, 130));
        }

        private static void CreateRock(Transform parent, Vector3 position, Vector3 scale)
        {
            var rock = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rock.name = "DesertRockCover";
            rock.transform.SetParent(parent, false);
            rock.transform.localPosition = position + Vector3.up * (scale.y * 0.28f);
            rock.transform.localScale = scale;
            rock.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Standard")) { color = new Color(0.39f, 0.31f, 0.23f) };
        }
    }
}
