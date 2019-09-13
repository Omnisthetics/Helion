﻿using System;
using Helion.Util.Geometry.Vectors;
using Helion.World.Entities;
using Helion.World.Entities.Players;
using Helion.World.Geometry.Lines;
using Helion.World.Geometry.Sectors;
using Helion.World.Physics;

namespace Helion.World.Special.Specials
{
    public class TeleportSpecial : ISpecial
    {
        private const int TeleportFreezeTicks = 18;
        private const int TeleportOffsetDist = 16;
        
        private readonly EntityActivateSpecialEventArgs m_args;
        private readonly IWorld m_world;

        public TeleportSpecial(EntityActivateSpecialEventArgs args, IWorld world)
        {
            m_args = args;
            m_world = world;
        }

        public SpecialTickStatus Tick()
        {
            Entity entity = m_args.Entity;
            Entity? teleportSpot = FindTeleportSpot();
            if (teleportSpot == null)
                return SpecialTickStatus.Destroy;

            CreateTeleportFogAt(entity);

            entity.UnlinkFromWorld();

            entity.FrozenTics = TeleportFreezeTicks;
            entity.Velocity = Vec3D.Zero;
            entity.SetPosition(teleportSpot.Position);
            entity.AngleRadians = teleportSpot.AngleRadians;
            if (entity is Player player)
                player.PitchRadians = 0;

            m_world.Link(entity);

            entity.ResetInterpolation();
            entity.OnGround = entity.CheckOnGround();
            
            CreateDestinationTeleportFogAt(entity);

            return SpecialTickStatus.Destroy;
        }

        private void CreateTeleportFogAt(Entity entity)
        {
            // TODO: Spawn it slightly in front of the entity based on the angle.
            // TODO: Make a sound!
        }

        private void CreateDestinationTeleportFogAt(Entity entity)
        {
            // TODO: Switch to a LUT when the time comes.
            Vec3D offset = new Vec3D(Math.Sin(entity.AngleRadians), Math.Cos(entity.AngleRadians), 0) * TeleportOffsetDist;

            // TODO: Spawn thing
            // TODO: Make a sound!
        }

        public void Use()
        {
        }

        private Entity? FindTeleportSpot()
        {
            Line line = m_args.ActivateLineSpecial;
            int tid = line.Args.Arg0;
            int tag = line.Args.Arg1;

            if (tid == EntityManager.NoTid && tag == Sector.NoTag)
                return null;
            
            if (tid == EntityManager.NoTid)
            {
                foreach (Sector sector in m_world.FindBySectorTag(tag))
                    foreach (Entity entity in sector.Entities)
                        if (entity.Flags.IsTeleportSpot)
                            return entity;
            } 
            else if (tag == Sector.NoTag)
            {
                foreach (Entity entity in m_world.FindByTid(tid))
                    if (entity.Flags.IsTeleportSpot)
                        return entity;
            }
            else
            {
                foreach (Sector sector in m_world.FindBySectorTag(tag))
                    foreach (Entity entity in sector.Entities)
                        if (entity.ThingId == tid && entity.Flags.IsTeleportSpot)
                            return entity;
            }
            
            return null;
        }
    }
}