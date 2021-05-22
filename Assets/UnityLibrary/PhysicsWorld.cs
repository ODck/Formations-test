using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using Dck.Pathfinder;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace UnityLibrary
{
    public class PhysicsWorld : MonoBehaviour
    {
        public static World World;
        public static ContactListener ContactListener;

        public static void Init(GameMap gameMap)
        {
            World = new World(Vector2.Zero);
            ContactListener = new ContactListener();
            World.SetContactListener(ContactListener);

            for (var i = 0u; i < gameMap.Width; i++)
            {
                for (var j = 0u; j < gameMap.Height; j++)
                {
                    if (gameMap.GetCellAt(i, j) == MapCellType.Clear) continue;
                    var body = new BodyDef {BodyType = BodyType.StaticBody, Position = new Vector2(i, j)};
                    var wall = World.CreateBody(body);
                    var collider = new PolygonShape();
                    collider.SetAsBox(GameMap.CellSize / 2F - 0.05F, GameMap.CellSize / 2F - 0.05F);
                    var fixtureDef = new FixtureDef
                    {
                        Density = 0F,
                        Filter = new Filter
                            {CategoryBits = PhysicsCategory.CATEGORY_OBSTACLE, MaskBits = PhysicsMask.COLLIDE_NOTHING},
                        Shape = collider
                    };
                    wall.CreateFixture(fixtureDef);
                }
            }
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class PhysicsCategory
    {
        public const ushort CATEGORY_PLAYER = 0x0001;
        public const ushort CATEGORY_MINION = 0x0002;
        public const ushort CATEGORY_OBSTACLE = 0x0004;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class PhysicsMask
    {
        public const ushort MASK_PLAYER = PhysicsCategory.CATEGORY_OBSTACLE;
        public const ushort COLLIDE_MINIONS = PhysicsCategory.CATEGORY_MINION;
        public const ushort COLLIDE_OBSTACLES = PhysicsCategory.CATEGORY_OBSTACLE;
        public const ushort COLLIDE_OBSTACLES_MINIONS = PhysicsCategory.CATEGORY_MINION | PhysicsCategory.CATEGORY_OBSTACLE;
        public const ushort COLLIDE_NOTHING = 0xFFFF;
    }
    
    public class ContactListener : IContactListener
    {
        public readonly Dictionary<Fixture, List<Fixture>> Contacts = new Dictionary<Fixture, List<Fixture>>();
        public void BeginContact(Contact contact)
        {
            if ((contact.FixtureA.Filter.CategoryBits != PhysicsMask.COLLIDE_MINIONS && !contact.FixtureA.IsSensor)||
                contact.FixtureB.Filter.CategoryBits != PhysicsMask.COLLIDE_MINIONS && !contact.FixtureB.IsSensor) return;
            Debug.Log("2 AGENTS COLLIDING");
            if(!Contacts.ContainsKey(contact.FixtureA))
                Contacts.Add(contact.FixtureA, new List<Fixture>());
            Contacts[contact.FixtureA].Add(contact.FixtureB);
                
            if(!Contacts.ContainsKey(contact.FixtureB))
                Contacts.Add(contact.FixtureB, new List<Fixture>());
            Contacts[contact.FixtureB].Add(contact.FixtureA);
        }

        public void EndContact(Contact contact)
        {
            if ((contact.FixtureA.Filter.CategoryBits != PhysicsMask.COLLIDE_MINIONS && !contact.FixtureA.IsSensor)||
                contact.FixtureB.Filter.CategoryBits != PhysicsMask.COLLIDE_MINIONS && !contact.FixtureB.IsSensor) return;
            
            if(Contacts.ContainsKey(contact.FixtureA))
            {
                Contacts[contact.FixtureA].Remove(contact.FixtureB);
                if (Contacts[contact.FixtureA].Count == 0)
                    Contacts.Remove(contact.FixtureA);
            }
                
            if(Contacts.ContainsKey(contact.FixtureB))
            {
                Contacts[contact.FixtureB].Remove(contact.FixtureA);
                if (Contacts[contact.FixtureB].Count == 0)
                    Contacts.Remove(contact.FixtureB);
            }
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
            //throw new System.NotImplementedException();
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {
            //throw new System.NotImplementedException();
        }
    }
}