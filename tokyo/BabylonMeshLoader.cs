using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tokyo
{
    public class BabylonMeshLoader : MeshLoader
    {
        private String folder;

        public BabylonMeshLoader(String folder)
        {
            this.folder = folder;
        }

        public Mesh[] Load(string modelName)
        {
            string modelString = File.ReadAllText(folder + "/" + modelName + ".json");
            dynamic model = JsonConvert.DeserializeObject(modelString);
            var meshes = new List<Mesh>();
            for (int meshIndex = 0; meshIndex < model.meshes.Count; meshIndex++)
            {
                var grid = model.meshes[meshIndex];
                var verticesArray = grid.vertices;
                var indicesArray = grid.indices;

                var uvCount = grid.uvCount.Value;
                int verticesStep = 1;

                switch ((int)uvCount)
                {
                    case 0:
                        verticesStep = 6;
                        break;
                    case 1:
                        verticesStep = 8;
                        break;
                    case 2:
                        verticesStep = 10;
                        break;
                }

                int verticesCount = verticesArray.Count / verticesStep;

                int facesCount = indicesArray.Count / 3;

                Mesh mesh = new Mesh(grid.name.Value, verticesCount, facesCount);
                for (int index = 0; index < verticesCount; index++)
                {
                    float x = (float)verticesArray[index * verticesStep].Value;
                    float y = (float)verticesArray[index * verticesStep + 1].Value;
                    float z = (float)verticesArray[index * verticesStep + 2].Value;

                    float nx = (float)verticesArray[index * verticesStep + 3].Value;
                    float ny = (float)verticesArray[index * verticesStep + 4].Value;
                    float nz = (float)verticesArray[index * verticesStep + 5].Value;

                    mesh.Vertices[index] = new Vertex { Coord = new Vector(x, y, z), Normal = new Vector(nx, ny, nz) };
                }

                for (int index = 0; index < facesCount; index++)
                {
                    int a = (int)indicesArray[index * 3].Value;
                    int b = (int)indicesArray[index * 3 + 1].Value;
                    int c = (int)indicesArray[index * 3 + 2].Value;

                    mesh.Surfaces[index] = new Surface(a, b, c);
                }

                var position = model.meshes[meshIndex].position;
                mesh.Position = new Vector((float)position[0].Value, (float)position[1].Value, (float)position[2].Value);

                meshes.Add(mesh);
            }

            return meshes.ToArray();
        }
    }
}
