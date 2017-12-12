﻿using System;
using System.Collections.Generic;
using System.IO;
using WaveEngine.Common.Math;
using WaveEngine.Common.Physics2D;
using WaveEngine.Components.Graphics3D;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Managers;
using WaveEngine.Framework.Physics2D;
using WaveEngine.Framework.Services;

namespace P2PTank.Tools
{
    public class MapLoader
    {
        private EntityManager entityManager;
        private Entity map;
        private List<Vector2> spawns;

        public async void Load(string filePath, Material material, EntityManager entityManager)
        {
            this.entityManager = entityManager;

            this.map = new Entity()
                .AddComponent(new Transform2D());

            this.spawns = new List<Vector2>();

            var storage = WaveServices.Storage;

            if (!storage.ExistsContentFile(filePath))
            {
                throw new FileNotFoundException(filePath);
            }

            using (var stream = new StreamReader(WaveServices.Storage.OpenContentFile(filePath)))
            {
                int currentX = 0;
                int currentY = 0;

                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();

                    foreach (var character in line)
                    {
                        if (character == '1' || character == '2' || character == '3')
                        {
                            var digit = (int)Char.GetNumericValue(character);

                            var cube = this.CreateCube(currentX, currentY, 0, material);
                            var collider = this.CreateMapCollider(currentX, currentY);
                            cube.AddChild(collider);

                            if (digit > 1)
                            {
                                for (int i = 1; i <= digit; i++)
                                {
                                    this.CreateCube(currentX, currentY, i, material);
                                }
                            }
                        }
                        else if (character == '9')
                        {
                            this.spawns.Add(new Vector2(currentX * 46, currentY * 46));
                        }

                        currentX++;
                    }

                    currentY++;
                    currentX = 0;
                }
            }

            this.entityManager.Add(map);
        }

        public List<Vector2> GetSpawnPoints()
        {
            return this.spawns;
        }

        public Vector2 GetSpawnPoint(int index)
        {
            return this.spawns[index];
        }

        private Entity CreateMapCollider(int x, int y)
        {
            float size = 46;
            float midSize = size / 2.0f;
            var colliderEntity = new Entity()
              .AddComponent(new Transform2D()
              {
                  LocalPosition = new Vector2(x * size, y * size),
              })
              .AddComponent(new EdgeCollider2D()
              {
                  Vertices = new Vector2[]
                  {
                        new Vector2(- midSize, - midSize),
                        new Vector2(+ midSize, - midSize),
                        new Vector2(+ midSize, + midSize),
                        new Vector2(- midSize, + midSize),
                        new Vector2(- midSize, - midSize)
                  }
              });

            colliderEntity.Tag = GameConstants.TagCollider;
            colliderEntity.AddComponent(new RigidBody2D() { PhysicBodyType = RigidBodyType2D.Static });

            var collider = colliderEntity.FindComponent<Collider2D>(false);

            if (collider != null)
            {
                collider.CollisionCategories = ColliderCategory2D.Cat3;
                collider.CollidesWith = ColliderCategory2D.All;
                collider.Friction = 1.0f;
                collider.Restitution = 0.2f;
            }

            return colliderEntity;
        }
        
        private Entity CreateCube(int currentX, int currentY, int z, Material material)
        {
            var cube = new Entity { IsStatic = true }
                .AddComponent(new Transform3D() { LocalPosition = new Vector3(currentX, z + 0.5f, currentY) })
                .AddComponent(new CubeMesh())
                .AddComponent(new MeshRenderer())
                .AddComponent(new MaterialComponent { Material = material });

            this.map.AddChild(cube);

            return cube;
        }

        private Entity CreatePlane(Material material)
        {
            // TODO: Add floor!
            var cube = new Entity { IsStatic = true }
                .AddComponent(new Transform3D() { LocalPosition = new Vector3(0, 0, 0) })
                .AddComponent(new PlaneMesh())
                .AddComponent(new MeshRenderer())
                .AddComponent(new MaterialComponent { Material = material });

            this.map.AddChild(cube);

            return cube;
        }
    }
}
